using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Components.Pages.Shohin;
using InbentorySystem.Components.Pages.Shohin.Edit;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using InbentorySystem.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection.Metadata;
using Xunit;


namespace InbentorySystem.Tests.Unit.Pages.Shohin.ShohinSearchPageTests
{
    public class ShohinSearchPageTests
    {
        [Fact] // UT-SS-01: 検索結果が表示される
        public void ShohinSearch_ShohinRenderShohinList()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel>
                {
                new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀", ShohinMeiKana = "ぎゅうとう", Shiirene = 1500, Urine = 2900, ShiiresakiCode = "S001" }
                });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "牛刀"));

            Assert.Contains("牛刀", cut.Markup);
            Assert.Contains("3000", cut.Markup);
        }

        [Fact] // UT-SS-02: 検索結果が0件なら警告表示
        public void ShohinSearch_ShouldShowWarning_WhenNoResults()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("なし"))
                .ReturnsAsync(new List<ShohinModel>());

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "なし"));

            Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
        }

        [Fact] // UT-SS-03: クエリが空ならエラー表示
        public void ShohinSearch_ShohinShowError_WhenQueryIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, ""));

            Assert.Contains("検索条件が不正です", cut.Markup);
        }

        [Fact] // UT-SS-04: 読み込み中メッセージが表示される
        public void ShohinSearch_ShouldShowLoadingMessage()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .Returns(async () =>
                {
                    await Task.Delay(50);
                    return new List<ShohinModel>();
                });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "牛刀"));

            Assert.Contains("データを読み込み中です", cut.Markup);
        }

        [Fact] // UT-SS-05: 戻るボタンでメニューに遷移する
        public void ShohinSearch_ShouldNavigateBackToMenu()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel>());

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "牛刀"));

            cut.Find("button").Click();

            Assert.Equal("/shohin/menu", nav.Uri);
        }
    }
}
