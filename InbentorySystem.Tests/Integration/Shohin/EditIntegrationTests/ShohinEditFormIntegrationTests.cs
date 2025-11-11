using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Ui.ShohinKanriUi.Shohin.Delete;
using InbentorySystem.Ui.ShohinKanriUi.Shohin.Edit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Integration.Shohin.EditIntegrationTests
{
    public class ShohinEditFormIntegrationTests
    {
        [Fact] // IT-SEF-01: 商品情報が表示される
        public void ShohinEditForm_ShouldRenderShohinDetails_WhenCodeIsValid()
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
            Assert.Contains("保存", cut.Markup);
        }

        [Fact] // IT-SEF-02: 商品コードが空ならエラー表示
        public void ShohinEditForm_ShouldShowError_WhenCodeIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, ""));

            Assert.Contains("商品コードが指定されていません", cut.Markup);
        }

        [Fact] // IT-SEF-03: 保存ボタンで更新処理と遷移が実行される
        public async Task ShohinEditForm_ShouldUpdateAndNavigate_WhenSaveClicked()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

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

            mockRepo.Setup(r => r.UpdateAsync(It.IsAny<ShohinModel>())).ReturnsAsync(1);

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinEditForm>(parameters => parameters.Add(p => p.ShohinCode, "A001"));

            cut.Find("input[id=ShohinMeiKanji").Change("刺身包丁");
            cut.Find("button.btn-primary").Click();

            await Task.Delay(10);

            Assert.Equal("/shohin/menu", nav.Uri);
            mockRepo.Verify(r => r.UpdateAsync(It.Is<ShohinModel>(m => m.ShohinMeiKanji == "刺身包丁")), Times.Once);
        }

        [Fact] // IT-SEF-04: 戻るボタンでメニューに遷移する
        public void ShohinEditForm_ShouldNavigateBackToMenu()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.GetByCodeAsync("A001"))
                .ReturnsAsync(new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀" });

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinEditForm>(parameters => parameters.Add(p => p.ShohinCode, "A001"));

            cut.Find("button.btn-secondary").Click();

            Assert.Equal("/shohin/menu", nav.Uri);
        }
    }
}
