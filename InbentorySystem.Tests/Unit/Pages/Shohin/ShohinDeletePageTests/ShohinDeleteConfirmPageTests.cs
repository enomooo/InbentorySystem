using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Ui.ShohinKanriUi.Shohin.Delete;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Unit.Pages.Shohin.ShohinDeletePageTests
{
    public class ShohinDeleteConfirmPageTests
    {
        [Fact] // UT-SDC-01: 商品情報が表示される
        public void ShohinDeleteConfirm_ShouldRenderShohinDetails()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.GetByCodeAsync("A001"))
                .ReturnsAsync(new ShohinModel
                {
                    ShohinCode = "A001",
                    ShohinMeiKanji = "牛刀",
                    ShohinMeiKana = "ぎゅうとう",
                    Shiirene = 1500,
                    Urine = 2900,
                    ShiiresakiCode = "S001"
                });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, "A001"));

            Assert.Contains("牛刀", cut.Markup);
            Assert.Contains("削除を実行する", cut.Markup);
        }

        [Fact] // UT-SDC-02: 商品コードが空ならエラー表示
        public void ShohinDeleteConfirm_ShouldShowError_WhenCodeIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, ""));

            Assert.Contains("商品コードが指定されていません", cut.Markup);
        }

        [Fact] // UT-SDC-03: 削除ボタンで削除処理と遷移が実行される
        public async Task ShohinDeleteConfirm_ShouldDeleteAndNavigate()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.GetByCodeAsync("A001"))
                .ReturnsAsync(new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀" });

            mockRepo.Setup(r => r.DeleteAsync("A001")).Returns(Task.CompletedTask);

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, "A001"));

            cut.Find("button.btn-danger").Click();

            await Task.Delay(10);

            Assert.Equal("/shohin/menu", nav.Uri);
            mockRepo.Verify(r => r.DeleteAsync("A001"), Times.Once);
        }

        [Fact] // UT-SDC-04: 戻るボタンでメニューに遷移する
        public void ShohinDeleteConfirm_ShouldNavigateBack()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.GetByCodeAsync("A001"))
                .ReturnsAsync(new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀" });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, "A001"));

            cut.Find("button.btn-secondary").Click();

            Assert.Equal("/shohin/menu", nav.Uri);
        }
    }
}
