using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Infrastructure.Models;
using InbentorySystem.Pages.Ui.Shohin;
using InbentorySystem.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Integration.Shohin
{
    public class ShohinMenuIntegrationTests
    {
        private readonly ShohinModel resultShohin = new ShohinModel
        {
            ShohinCode = "A001",
            ShohinMeiKanji = "牛刀",
            Shiirene = 1500,
            Urine = 3000
        };

        [Fact] // IT-SM-01: 商品登録フォームで登録処理が呼ばれる
        public async Task ShohinMenu_ShouldRegisterShohin_WhenFormSubmitted()
        {
            using var ctx = new TestContext();

            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockService = new Mock<IShohinService>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel> { new ShiiresakiModel { ShiiresakiCode = "S001" } };

            mockRepo.Setup(r => r.CheckDuplicateCodeAsync("A001")).ReturnsAsync(false);
            mockRepo.Setup(r => r.RegisterAsync(It.IsAny<ShohinModel>())).ReturnsAsync(1);
            mockShiiresakiRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(shiiresakiData);
            
            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiList.Any(), TimeSpan.FromSeconds(2));

            cut.Find("input[id=ShohinCode]").Change("A001");
            cut.Find("input[id=KanjiName]").Change("牛刀");
            cut.Find("input[id=KanaName]").Change("ぎゅうとう");
            cut.Find("input[id=Shiirene]").Change("1500");
            cut.Find("input[id=Urine]").Change("3000");
            cut.Find("select[id=ShiiresakiCode]").Change(new ChangeEventArgs { Value = "S001" });

            await cut.Find("Form").SubmitAsync();

            mockRepo.Verify(r => r.RegisterAsync(It.Is<ShohinModel>(m => m.ShohinCode == "A001")), Times.Once);
        }

        [Fact] // IT-SM-02: 重複コードでエラーが表示される
        public async Task ShohinMenu_ShouldShowError_WhenDuplicateCode()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            var mockService = new Mock<IShohinService>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel> { new ShiiresakiModel { ShiiresakiCode = "S001" } };

            mockRepo.Setup(r => r.CheckDuplicateCodeAsync("A001")).ReturnsAsync(true);
            mockShiiresakiRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(shiiresakiData);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiList.Any(), TimeSpan.FromSeconds(2));

            cut.Find("input[id=ShohinCode]").Change("A001");
            cut.Find("input[id=KanjiName]").Change("ダミー漢字");
            cut.Find("input[id=KanaName]").Change("ダミーかな");
            cut.Find("input[id=Shiirene]").Change("1500");
            cut.Find("input[id=Urine]").Change("3000");
            cut.Find("select[id=ShiiresakiCode]").Change(new ChangeEventArgs { Value = "S001" });
            await cut.Find("form").SubmitAsync();

            cut.WaitForAssertion(() =>
            Assert.Contains("この商品コードは既に登録されています", cut.Markup));

            mockRepo.Verify(r => r.CheckDuplicateCodeAsync("A001"), Times.Once);
        }

        [Fact] // IT-SM-03: 修正キーワード入力 -> 修正画面に遷移
        public async Task ShohinMenu_ShouldNavigateToEditSelect_WhenEditKeywordEntered()
        {
            using var ctx = new TestContext();

            //Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockService = new Mock<IShohinService>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel> { new ShiiresakiModel { ShiiresakiCode = "S001" } };

            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
               .ReturnsAsync(new List<ShohinModel> { new ShohinModel { ShohinMeiKanji = "牛刀" } });

            mockService.SetupProperty(s => s.LastEditedShohin);
            mockService.Setup(s => s.SetLastEditedShohin(It.IsAny<ShohinModel>()));
            mockShiiresakiRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ShiiresakiModel>());

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.Render();

            cut.Find("input[id=editKeyword]").Change("牛刀");

            await cut.FindAll("button").First(b => b.TextContent.Contains("修正画面へ")).ClickAsync(new MouseEventArgs());

            Assert.Contains("/shohin/edit/select", nav.Uri);
        }

        [Fact] // IT-SM-04: 削除キーワード入力 -> 削除画面に遷移
        public async Task ShohinMenu_ShouldNavigateToDeleteSelect_WhenDeleteKeywordEntered()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                               .ReturnsAsync(new List<ShohinModel> { resultShohin });

            var mockService = new Mock<IShohinService>();

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            // Act
            var cut = ctx.RenderComponent<ShohinMenu>();
;
            cut.Render();

            cut.Find("input[id=editKeyword]").Change("牛刀");
            cut.FindAll("button").First(b => b.TextContent.Contains("削除画面へ")).Click();

            Assert.Contains("/shohin/delete/select", nav.Uri);
        }

    }
}
