using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Pages.Ui.Shohin;
using InbentorySystem.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Threading.Tasks;

namespace InbentorySystem.Tests.Integration.Shohin
{
    public class ShohinSearchIntegrationTests
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

            [Fact] // IT-SS-01: 検索結果が表示される
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

            [Fact] // IT-SS-02: 検索結果が0件なら警告表示
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

            [Fact] // IT-SS-03: クエリが空ならエラー表示
            public async Task ShohinSearch_ShohinShowError_WhenQueryIsEmpty()
            {
                nav.NavigateTo("http://localhost/shohin/search?q=");

                var cut = ctx.RenderComponent<ShohinSearch>();


                await cut.InvokeAsync(async () => cut.Instance.LoadDataAsync());

                cut.WaitForAssertion(() =>
                {
                    Assert.Contains("検索条件が不正です", cut.Markup);
                });
            }

            [Fact] // IT-SS-04: "all"クエリで全件取得される
            public void ShohinSearch_ShouldRenderAllShohin_WhenQueryIsAll()
            {
                using var ctx = new TestContext();

                var mockRepo = new Mock<IShohinRepository>();
                mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(new List<ShohinModel>
                    {
                new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀" },
                new ShohinModel { ShohinCode = "A002", ShohinMeiKanji = "出刃包丁" }
                    });

                ctx.Services.AddSingleton(mockRepo.Object);

                var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "all"));

                Assert.Contains("牛刀", cut.Markup);
                Assert.Contains("出刃包丁", cut.Markup);
                Assert.Contains("検索結果: **2件**", cut.Markup);
            }

            [Fact] // IT-SS-05: 戻るボタンでメニューに遷移する
            public void ShohinSearch_ShouldNavigateBackToMenu()
            {
                using var ctx = new TestContext();
                var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

                var mockRepo = new Mock<IShohinRepository>();
                mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                    .ReturnsAsync(new List<ShohinModel>());

                ctx.Services.AddSingleton(mockRepo.Object);

                var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "牛刀"));

                cut.Find("button.btn-secondary").Click();

                Assert.Equal("/shohin/menu", nav.Uri);
            }
        }
    }
}
