using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Pages.Ui.Shohin.Delete;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Unit.Pages.Shohin.ShohinDeletePageTests
{
    public class ShohinDeleteSelectPageTests
    {
        [Fact] // UT-SD-01: 検索結果が表示される
        public void ShohinDeleteSelect_ShouldRenderShohinList()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel>
                {
                new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀", ShohinMeiKana = "ぎゅうとう", Shiirene = 1500, Urine = 2900, ShiiresakiCode = "S001" }
                });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteSelect>(parameters => parameters.Add(p => p.q, "牛刀"));

            Assert.Contains("牛刀", cut.Markup);
            Assert.Contains("削除", cut.Markup);
        }

        [Fact] // UT-SD-02: 検索結果が0件なら警告表示
        public void ShohinDeleteSelect_ShohinShowWarning_WhenNoResults()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("なし"))
                .ReturnsAsync(new List<ShohinModel>());

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteSelect>(parameters => parameters.Add(p => p.q, "なし"));

            Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
        }

        [Fact] // UT-SD-03: クエリが空ならエラー表示
        public void ShohinDeleteSelect_ShouldShowError_WhenQueryIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteSelect>(parameters => parameters.Add(p => p.q, ""));

            Assert.Contains("検索条件が不正です", cut.Markup);
        }

        [Fact] // UT-SD-04: 削除ボタンで削除確認画面に遷移する
        public void ShohinDeleteSelect_ShouldNavigateToDeleteConfirm()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel>
                {
                new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀"}
                });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteSelect>(parameters => parameters.Add(p => p.q, "牛刀"));

            cut.Find("button.btn-danger").Click();

            Assert.Equal("/shohin/delete/A001", nav.Uri);
        }

        [Fact] // UT-SD-05: 戻るボタンでメニューに遷移する
        public void ShohinDeleteSeltect_ShouldNavigateBackToMenu()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel>());

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteSelect>(parameters => parameters.Add(p => p.q, "牛刀"));

            cut.Find("button.btn-secondary").Click();

            Assert.Equal("/shohin/menu", nav.Uri);
        }
    }
}
