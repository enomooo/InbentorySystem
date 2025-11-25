using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Pages.Ui.Shohin;
using InbentorySystem.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InbentorySystem.Tests.Unit.Pages.Shohin
{
    public class ShohinSearchPageTests
    {
        private readonly TestContext ctx = new();
        private readonly Mock<IShohinRepository> mockRepo = new();
        private readonly Mock<IShohinService> mockService = new();
        private readonly FakeNavigationManager nav;

        private readonly ShohinModel DummyShohin = new()
        {
            ShohinCode = "A001",
            ShohinMeiKanji = "牛刀",
            Urine = 3000
        };

        public ShohinSearchPageTests()
        {
            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();
            nav = ctx.Services.GetRequiredService<FakeNavigationManager>();
        }

        [Fact] // UT-SS-01: 検索結果が表示される (正常系)
        public async Task ShohinSearch_ShohinRenderShohinList_WhenDataIsFound()
        {
            // Arrange
            var expectedList = new List<ShohinModel> { DummyShohin };
            var tcs = new TaskCompletionSource<List<ShohinModel>>();

            mockService.Setup(s => s.SearchShohinAsync("牛刀")).Returns(tcs.Task);

            nav.NavigateTo("http://localhost/shohin/search?q=牛刀");

            // Act1
            var cut = ctx.RenderComponent<ShohinSearch>();

            // ASSERT1 (ロード中): データが表示されていないことを確認
            Assert.Contains("データを読み込み中です", cut.Markup);

            // Act2
            tcs.SetResult(expectedList);

            cut.WaitForAssertion(() =>
            {
                Assert.DoesNotContain("データを読み込み中です", cut.Markup);
            }, TimeSpan.FromSeconds(3));

            await Task.Delay(10);

            // ASSERT2 : データが表示されていることを検証
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("牛刀", cut.Markup);
                Assert.Contains("3,000", cut.Markup);

                mockService.Verify(s => s.SearchShohinAsync("牛刀"), Times.Once);
            }, TimeSpan.FromSeconds(1));
        }

        [Fact] // UT-SS-02: 検索結果が0件なら警告表示 (データなし)
        public async Task ShohinSearch_ShouldShowWarning_WhenNoResults()
        {
            // Arrange 
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            mockService.Setup(r => r.SearchShohinAsync("なし"))
                .Returns(async () =>
                {
                    await Task.Delay(1);
                    return new List<ShohinModel>();
                });


            nav.NavigateTo("http://localhost/shohin/search?q=なし");

            // ACT
            var cut = ctx.RenderComponent<ShohinSearch>();

            await Task.Delay(10);

            // ASSERT
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
                Assert.DoesNotContain("データを読み込み中です", cut.Markup);

                mockService.Verify(s => s.SearchShohinAsync("なし"), Times.Once);
            });
        }
    }
}