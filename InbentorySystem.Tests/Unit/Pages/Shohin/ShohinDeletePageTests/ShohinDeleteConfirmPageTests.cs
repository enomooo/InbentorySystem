using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Pages.Ui.Shohin.Delete;
using InbentorySystem.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Unit.Pages.Shohin.ShohinDeletePageTests
{
    public class ShohinDeleteConfirmPageTests
    {
        // 共通セットアップ
        private readonly TestContext ctx = new();
        private readonly Mock<IShohinRepository> mockRepo = new();
        private readonly Mock<IShohinService> mockService = new();
        private readonly FakeNavigationManager nav;

        private readonly ShohinModel DummyShohin = new()
        {
            ShohinCode = "A001",
            ShohinMeiKanji = "牛刀",
            ShohinMeiKana = "ぎゅうとう",
            Shiirene = 1500,
            Urine = 2900,
            ShiiresakiCode = "S001"
        };

        public ShohinDeleteConfirmPageTests()
        {
            ctx.Services.AddSingleton<FakeNavigationManager>();
            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);

            nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

        }

        [Fact] // UT-SDC-01: 商品情報が表示される
        public async Task ShohinDeleteConfirm_ShouldRenderShohinDetailsFromService()
        {
            // Arrange
            mockService.Setup(s => s.GetLastDeletedShohin())
                .Returns(DummyShohin);

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, DummyShohin.ShohinCode));

            await Task.Delay(1);

            cut.WaitForAssertion(() =>
                Assert.Contains("牛刀", cut.Markup),
                TimeSpan.FromSeconds(2)
            );

            // Assert
            Assert.Contains("削除", cut.Find("button.btn-danger").TextContent);
        }

        [Fact] // UT-SDC-02: 削除ボタンでRepoが呼ばれ、結果画面に遷移する
        public async Task ShohinDeleteConfirm_ShouldSDeleteAndNavigateToResult()
        {
            // Arrange
            mockService.Setup(s => s.GetLastDeletedShohin()).Returns(DummyShohin);
            mockRepo.Setup(r => r.DeleteAsync(DummyShohin.ShohinCode)).Returns(Task.CompletedTask);

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, DummyShohin.ShohinCode));

            await cut.Find("button.btn-danger").ClickAsync(new MouseEventArgs());

            var expectedPath = $"/shohin/delete/result?Shohincode={DummyShohin.ShohinCode}";

            // Assert
            Assert.Contains(expectedPath, nav.Uri);

            mockRepo.Verify(r => r.DeleteAsync(DummyShohin.ShohinCode), Times.Once);
        }

        [Fact] // UT-SDC-03: 削除ボタンで削除処理と遷移が実行される
        public void ShohinDeleteConfirm_ShouldShowWarning_WhenModelIsNull()
        {
            // Arrange
            mockService.Setup(s => s.GetLastDeletedShohin()).Returns((ShohinModel?)null);

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteConfirm>();

            // Assert
            Assert.Contains("指定された商品コード", cut.Markup);
            Assert.Contains("見つかりませんでした", cut.Markup);
        }

        [Fact] // UT-SDC-04: 戻るボタンでメニューに遷移する
        public void ShohinDeleteConfirm_ShouldNavigateBack()
        {
            // Arrange
            mockService.Setup(s => s.GetLastDeletedShohin()).Returns(DummyShohin);

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, DummyShohin.ShohinCode));
            cut.Find("button.btn-secondary").Click();

            // Assert
            Assert.Equal("shohin/menu", nav.ToBaseRelativePath(nav.Uri));
        }
    }
}
