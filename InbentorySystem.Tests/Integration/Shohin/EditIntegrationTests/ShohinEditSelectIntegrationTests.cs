using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Ui.ShohinKanriUi.Shohin;
using InbentorySystem.Ui.ShohinKanriUi.Shohin.Edit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Integration.Shohin.EditIntegrationTests
{
    public class ShohinEditSelectIntegrationTests
    {
        [Fact] // IT-SES-01: 検索結果が表示される
        public void ShohinEditSelect_ShouldRenderShohinList_WhenKeywordMatches()
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
            Assert.Contains("修正", cut.Markup);
        }

        [Fact] // IT-SES-02: 検索結果が0件なら警告表示
        public void ShohinEditSelect_ShouldShowWarning_WhenNoResults()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("なし"))
                .ReturnsAsync(new List<ShohinModel>());

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, "なし"));

            Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
        }
        [Fact] // IT-SES-03: クエリが空ならエラー表示
        public void ShohinEditSelect_ShouldShowError_WhenQueryIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinSearch>(parameters => parameters.Add(p => p.q, ""));

            Assert.Contains("検索条件が不正です", cut.Markup);
        }

        [Fact] // IT-SES-04: 修正ボタンで編集画面に遷移する
        public void ShohinEditSelect_ShouldNavigateToEditForm_WhenEditButtonClicked()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel>
            {
                new ShohinModel { ShohinCode= "A001", ShohinMeiKanji = "牛刀"}
            });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinEditSelect>(parameters => parameters.Add(p => p.q, "牛刀"));

            cut.Find("button.btn-primary").Click();

            Assert.Equal("/shohin/edit/A001", nav.Uri);
        }

        [Fact] // IT-SES-05: 戻るボタンでメニューに遷移する
        public void ShohinEditSelect_ShouldNavigateBackToMenu()
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
