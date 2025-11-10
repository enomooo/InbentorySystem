using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Components.Pages.Shohin;
using InbentorySystem.Components.Pages.Shohin.Delete;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using InbentorySystem.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Reflection.Metadata;
using Xunit;

namespace InbentorySystem.Tests.Integration.Shohin.DeleteIntegrationTests
{
    public class ShohinDeleteConfirmIntegrationTests
    {
        [Fact] // IT-SDC-01: 商品情報が表示される
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

        [Fact] // IT-SDC-02: 商品コードが空ならエラー表示
        public void ShohinDeleteConfirm_ShouldShowError_WhenCodeIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, ""));

            Assert.Contains("商品コードが指定されていません", cut.Markup);
        }

        [Fact] // IT-SDC-03: 削除ボタンで削除処理と遷移が実行される
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

        [Fact] // IT-SDC-04: 戻るボタンでメニューに遷移する
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

        [Fact] // IT-SDC-05: 商品が存在しない場合はエラー表示
        public void ShohinDeletableDeleteConfirm_ShouldShowError_WhenShohinNotFound()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.GetByCodeAsync("zzz000")).ReturnsAsync(null as ShohinModel);

            ctx.Services.AddSingleton(mockRepo.Object);

            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(parameters => parameters.ShohinCode, "zzz000"));

            Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
        }
    }
}
