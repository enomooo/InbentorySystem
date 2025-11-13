using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Services.Interfaces;
using InbentorySystem.Pages.Ui.Shohin;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Integration.Shohin
{
    public class ShohinMenuIntegrationTests
    {
        [Fact] // IT-SM-01: 商品登録フォームで登録処理が呼ばれる
        public async Task ShohinMenu_ShouldRegisterShohin_WhenFormSubmitted()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.CheckDuplicateCodeAsync("A001")).ReturnsAsync(false);
            mockRepo.Setup(r => r.RegisterAsync(It.IsAny<ShohinModel>())).ReturnsAsync(1);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.Find("input[id=ShohinCode]").Change("A001");
            cut.Find("input[id=KanjiName]").Change("牛刀");
            cut.Find("input[id=KanaName]").Change("ぎゅうとう");
            cut.Find("input[id=Shiirene]").Change("1500");
            cut.Find("input[id=Urine]").Change("3000");
            cut.Find("input[id=ShiiresakiCode]").Change("S001");

            await cut.Find("Form").SubmitAsync();

            mockRepo.Verify(r => r.RegisterAsync(It.Is<ShohinModel>(m => m.ShohinCode == "A001")), Times.Once);
        }

        [Fact] // IT-SM-02: 重複コードでエラーが表示される
        public async Task ShohinMenu_ShouldShowError_WhenDuplicateCode()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.CheckDuplicateCodeAsync("A001")).ReturnsAsync(true);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.Find("input[id=ShohinCode]").Change("A001");
            await cut.Find("form").SubmitAsync();

            Assert.Contains("この商品コードは既に登録されています", cut.Markup);
        }

        [Fact] // IT-SM-03: 修正キーワード入力 -> 修正画面に遷移
        public void ShohinMenu_ShouldNavigateToEditSelect_WhenEditKeywordEntered()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel> { new ShohinModel { ShohinMeiKanji = "牛刀" } });

            var mockService = new Mock<IShohinService>();
            mockService.SetupProperty(s => s.LastEditedShohin);
            mockService.Setup(s => s.SetLastEditedShohin(It.IsAny<ShohinModel>()));


            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.Find("input[id=editKeyword]").Change("牛刀");
            cut.FindAll("button").First(b => b.TextContent.Contains("修正画面へ")).Click();

            Assert.Contains("/shohin/edit/select", nav.Uri);
        }

        [Fact] // IT-SM-04: 削除キーワード入力 -> 削除画面に遷移
        public void ShohinMenu_ShouldNavigateToDeleteSelect_WhenDeleteKeywordEntered()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel> { new ShohinModel { ShohinMeiKanji = "牛刀" } });

            var mockService = new Mock<IShohinService>();

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.Find("input[id=editKeyword]").Change("牛刀");
            cut.FindAll("button").First(b => b.TextContent.Contains("削除画面へ")).Click();

            Assert.Contains("/shohin/delete/select", nav.Uri);
        }

    }
}
