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
    public class ShohinDeleteSelectPageTests
    {
        private readonly TestContext ctx = new();
        private readonly Mock<IShohinRepository> mockRepo = new();
        private readonly Mock<IShohinService> mockService = new();

        private readonly ShohinModel DummyResults = new()
        {
            ShohinCode = "A001",
            ShohinMeiKanji = "牛刀",
            ShohinMeiKana = "ぎゅうとう",
            Shiirene = 1500,
            Urine = 2900,
            ShiiresakiCode = "S001"
        };

        public ShohinDeleteSelectPageTests()
        {
            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();
        }

        [Fact] // UT-SD-01: 検索結果が表示される
        public async Task ShohinDeleteSelect_ShouldRenderShohinList()
        {
            // Arrange
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .Returns(async () =>
                {
                    await Task.Delay(1);
                    return new List<ShohinModel> { new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀", Shiirene = 1500 } };
                });

            nav.NavigateTo("http://localhost/shohin/delete/select?q=牛刀");

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteSelect>();

            await Task.Delay(10);

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("牛刀", cut.Markup);
                Assert.Contains("削除", cut.Markup);
                Assert.Contains("検索結果: **1 件**", cut.Markup);
            }, TimeSpan.FromSeconds(1));
        }

        [Fact] // UT-SD-02: 検索結果が0件なら警告表示
        public async Task ShohinDeleteSelect_ShohinShowWarning_WhenNoResults()
        {
            // Arrange
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();


            mockRepo.Setup(r => r.SearchByKeywordAsync("なし"))
                .Returns(async () =>
                {
                    await Task.Delay(1);
                    return new List<ShohinModel>();
                });

            nav.NavigateTo("http://localhost/shohin/delete/select?q=牛刀");

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteSelect>();

            await Task.Delay(10);

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
            }, TimeSpan.FromSeconds(1));
        }

        [Fact] // UT-SD-03: クエリが空ならエラー表示
        public void ShohinDeleteSelect_ShouldShowError_WhenQueryIsEmpty()
        {
            // Arrange
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            nav.NavigateTo("http://localhost/shohin/delete/select");

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteSelect>();

            Assert.Contains("検索条件が不正です", cut.Markup);
        }

        [Fact] // UT-SD-04: 削除ボタンで削除確認画面に遷移する
        public async Task ShohinDeleteSelect_ShouldNavigateToDeleteConfirm()
        {
            // Arrange
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var shohinList = new List<ShohinModel>
            {
                new ShohinModel{ShohinCode = "A001",
                    ShohinMeiKanji = "牛刀",
                    ShohinMeiKana = "ぎゅうとう",
                    Shiirene = 1500,
                    Urine = 2900,
                    ShiiresakiCode = "S001"
                }
            };
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .Returns(async () => { await Task.Delay(1); return shohinList; });

            // Act
            nav.NavigateTo("http://localhost/shohin/delete/select?q=牛刀");

            var cut = ctx.RenderComponent<ShohinDeleteSelect>();

            // Assert
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("牛刀", cut.Markup);
            }, TimeSpan.FromSeconds(1));

            await cut.Find("button.btn-danger").ClickAsync(new MouseEventArgs());

            mockService.Verify(s => s.SetLastDeletedShohin(It.Is<ShohinModel>(m => m.ShohinCode == "A001")), Times.Once);
            Assert.Contains("/shohin/delete/confirm/A001", nav.Uri);
        }

        [Fact] // UT-SD-05: 戻るボタンでメニューに遷移する
        public void ShohinDeleteConfirm_ShouldNavigateBack()
        {
            // Arrange
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();
            mockService.Setup(s => s.GetLastDeletedShohin()).Returns(DummyResults);

            // Act
            var cut = ctx.RenderComponent<ShohinDeleteConfirm>(parameters => parameters.Add(p => p.ShohinCode, DummyResults.ShohinCode));
            cut.Find("button.btn-secondary").Click();

            // Assert
            Assert.Equal("shohin/menu", nav.ToBaseRelativePath(nav.Uri));
        }
    }
}
