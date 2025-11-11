using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Ui.ShohinKanriUi.Shohin;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Integration.Shohin
{
    public class ShohinSearchIntegrationTests
    {
        [Fact] // IT-SS-01: 検索結果が表示される
        public void ShohinSearch_ShouldRenderShohinList_WhenKeywordMatches()
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
            Assert.Contains("検索結果: **1 件**", cut.Markup);
        }

        [Fact] // IT-SS-02: 検索結果が0件なら警告表示
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

        [Fact] // IT-SS-03: クエリが空ならエラー表示
        public void ShohinSearch_ShohinShowError_WhenQueryIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, ""));

            Assert.Contains("検索条件が不正です", cut.Markup);
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
