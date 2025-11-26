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

namespace InbentorySystem.Tests.Unit.Pages.Shohin.ShohinIndexPageTexts
{
    public class ShohinMenuPageTests
    {
        [Fact] // UT-SM-01: タイトルが表示される
        public void ShohinMenu_ShouldRenderTitle()
        {
            using var ctx = new TestContext();
            ctx.Services.AddSingleton(Mock.Of<IShohinRepository>());
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());
            ctx.Services.AddSingleton(Mock.Of<IShiiresakiRepository>());
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var cut = ctx.RenderComponent<ShohinMenu>();

            Assert.Contains("商品管理メニュー", cut.Markup);
        }

        [Fact] // UT-SM-02: 商品検索で該当なしの場合エラーメッセージが表示される
        public async Task ShohinMenu_Search_ShouldShowError_WhenNoResults()
        {
            using var ctx = new TestContext();

            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();

            mockRepo.Setup(r => r.SearchByKeywordAsync("テスト"))
                .ReturnsAsync(new List<ShohinModel>());

            mockShiiresakiRepo.Setup(r => r.GetAllAsync())
            .ReturnsAsync(new List<ShiiresakiModel>());

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            // Act
            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.WaitForAssertion(() =>
            Assert.True(cut.Instance.ShiiresakiList != null), TimeSpan.FromSeconds(1));

            cut.Find("#searchKeyword").Change("テスト");
            await cut.Find("button.btn-info").ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
                Assert.Contains("該当する商品はありませんでした", cut.Markup),
                TimeSpan.FromSeconds(1)
                );
        }

        [Fact] // UT-SI-03: 商品検索ボタンで該当なしの場合はエラー表示
        public async Task ShohinIndex_ShouldShowError_WhenSearchResultIsEmpty()
        {
            using var ctx = new TestContext();

            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockService = new Mock<IShohinService>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel>();

            mockRepo.Setup(r => r.SearchByKeywordAsync("該当なし"))
                .ReturnsAsync(new List<ShohinModel>());

            mockShiiresakiRepo.Setup(r => r.GetAllAsync())
               .ReturnsAsync(shiiresakiData);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiList is not null, TimeSpan.FromSeconds(1));

            cut.Find("input[id=searchKeyword]").Change("該当なし");

            await cut.Find("button.btn-info").ClickAsync(new MouseEventArgs());

            // Assert
            cut.WaitForAssertion(() =>
                Assert.Contains("該当する商品はありませんでした", cut.Markup),
                TimeSpan.FromSeconds(1)
                );
        }

        [Fact] // UT-SI-04: 新規登録フォームで登録処理が呼ばれる
        public async Task ShohinIndex_ShouldRegisterNewShohin()
        {
            using var ctx = new TestContext();

            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel> { new ShiiresakiModel { ShiiresakiCode = "S001" } };

            mockRepo.Setup(r => r.CheckDuplicateCodeAsync("A001")).ReturnsAsync(false);
            mockRepo.Setup(r => r.RegisterAsync(It.IsAny<ShohinModel>())).ReturnsAsync(1);

            mockShiiresakiRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(shiiresakiData);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            // Act
            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiList.Any(), TimeSpan.FromSeconds(2));

            cut.Find("input[id=ShohinCode]").Change("A001");
            cut.Find("input[id=KanjiName]").Change("牛刀");
            cut.Find("input[id=KanaName]").Change("ぎゅうとう");
            cut.Find("input[id=Shiirene]").Change("1500");
            cut.Find("input[id=Urine]").Change("3000");
            cut.Find("select[id=ShiiresakiCode]").Change("S001");

            await cut.Find("Form").SubmitAsync();

            // Assert
            mockRepo.Verify(r => r.RegisterAsync(It.Is<ShohinModel>(m => m.ShohinCode == "A001")), Times.Once);
        }

        [Fact] // UT-SI-05: 登録フォームで重複コードはエラー表示
        public async Task ShohinMenu_ShoudShowError_WhenDuplicateCode()
        {
            using var ctx = new TestContext();

            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel>();

            mockRepo.Setup(r => r.CheckDuplicateCodeAsync("A001")).ReturnsAsync(true);

            mockShiiresakiRepo.Setup(r => r.GetAllAsync())
               .ReturnsAsync(shiiresakiData);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinService>());
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            // Act
            var cut = ctx.RenderComponent<ShohinMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiList is not null, TimeSpan.FromSeconds(2));

            cut.Find("#ShohinCode").Change("A001");
            cut.Find("#KanjiName").Change("ダミー漢字");
            cut.Find("#KanaName").Change("ダミーかな");
            cut.Find("#Shiirene").Change(1500);
            cut.Find("#Urine").Change(3000);
            cut.Find("select[id=ShiiresakiCode]").Change(new ChangeEventArgs { Value = "S001" });


            await cut.Find("form").SubmitAsync();

            // Assert
            cut.WaitForAssertion(() =>
                Assert.Contains("この商品コードは既に登録されています", cut.Markup), TimeSpan.FromSeconds(1));
        }

        [Fact] // UT-SI-06: 修正ボタンで検索結果があれば遷移する
        public async Task ShohinIndex_ShouldNavigateToEditSelect()
        {
            using var ctx = new TestContext();

            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockService = new Mock<IShohinService>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel>();

            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel> { new ShohinModel { ShohinMeiKanji = "牛刀" } });

            mockShiiresakiRepo.Setup(r => r.GetAllAsync())
       .Returns(async () => { await Task.Delay(1); return shiiresakiData; });

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            // Act
            var cut = ctx.RenderComponent<ShohinMenu>();


            cut.Render();

            cut.Find("input[id=editKeyword]").Change("牛刀");

            await cut.FindAll("button").First(b => b.TextContent.Contains("修正画面へ")).ClickAsync(new MouseEventArgs());

            Assert.Contains("/shohin/edit/select", nav.Uri);
        }

        [Fact] // UT-SI-07: 削除ボタンで検索結果があれば遷移する
        public async Task ShohinIndex_ShohinNavidateToDeleteSelect()
        {
            using var ctx = new TestContext();
            
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var mockService = new Mock<IShohinService>();
            var mockShiiresakiRepo = new Mock<IShiiresakiRepository>();
            var shiiresakiData = new List<ShiiresakiModel>();

            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel> { new ShohinModel { ShohinMeiKanji = "牛刀" } });

            mockShiiresakiRepo.Setup(r => r.GetAllAsync())
      .Returns(async () => { await Task.Delay(1); return shiiresakiData; });

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);
            ctx.Services.AddSingleton(mockShiiresakiRepo.Object);
            ctx.Services.AddSingleton<FakeNavigationManager>();

            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            // Act
            var cut = ctx.RenderComponent<ShohinMenu>();

            await Task.Delay(50);
            cut.Render();

            cut.Find("input[id=deleteKeyword]").Change("牛刀");
            await cut.FindAll("button").First(b => b.TextContent.Contains("削除画面へ")).ClickAsync(new MouseEventArgs());

            Assert.Contains("/shohin/delete/select", nav.Uri);
        }
    }
}

