using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using InbentorySystem.Data.Models;
using Bunit.TestDoubles;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Services.Interfaces;
using InbentorySystem.Ui.ShohinKanriUi.Shohin;

namespace InbentorySystem.Tests.Unit.Pages.Shohin.ShohinIndexPageTexts
{
    public class ShohinMenuPageTests
    {
        [Fact] // UT-SI-01: タイトルが表示される
        public void ShohinIndex_ShohinRenderTitle()
        {
            using var ctx = new TestContext();
            var cut = ctx.RenderComponent<ShohinMenu>();
            Assert.Contains("商品管理メニュー", cut.Markup);
        }

        [Fact] // UT-SI-02: 商品検索ボタンで遷移する
        public void ShihinIndex_ShouldNavigateToSearchResult()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(R => R.SearchByKeywordAsync("牛刀"))
                .ReturnsAsync(new List<ShohinModel> { new ShohinModel { ShohinMeiKanji = "牛刀" } });

            var mockService = new Mock<IShohinService>();
            mockService.Setup(s => s.GetNavigationUri("牛刀")).Returns("/shohin/list");

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);

            var cut = ctx.RenderComponent<ShohinMenu>();
            cut.Find("input[id=searchKeyword]").Change("牛刀");
            cut.Find("button.btn-info").Click();
        }

        [Fact] // UT-SI-03: 商品検索ボタンで該当なしの場合はエラー表示
        public void ShohinIndex_ShouldShowError_WhenSearchResultIsEmpty()
        {
            using var ctx = new TestContext();

            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("該当なし"))
                .ReturnsAsync(new List<ShohinModel>());

            var mockService = new Mock<IShohinService>();
            mockService.Setup(s => s.GetNavigationUri("該当なし")).Returns(string.Empty);

            ctx.Services.AddSingleton(mockRepo.Object);
            ctx.Services.AddSingleton(mockService.Object);

            var cut = ctx.RenderComponent<ShohinMenu>();
            cut.Find("input[id=searchKeyword]").Change("該当なし");
            cut.Find("button.btn-info").Click();

            Assert.Contains("該当する商品がありませんでした。", cut.Markup);
        }

        [Fact] // UT-SI-04: 新規登録フォームで登録処理が呼ばれる
        public async Task ShohinIndex_ShouldRegisterNewShohin()
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

        [Fact] // UT-SI-05: 登録フォームで重複コードはエラー表示
        public async Task ShohinIndex_ShoudShowError_WhenDuplicateCode()
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

        [Fact] // UT-SI-06: 修正ボタンで検索結果があれば遷移する
        public async Task ShohinIndex_ShouldNavigateToEditSelect()
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
            cut.FindAll("button").First(b => b.TextContent.Contains("修正画面へ")).Click();

            await Task.Delay(10);

            Assert.Contains("/shohin/edit/ShohinEditSelect", nav.Uri);
        }

        [Fact] // UT-SI-07: 削除ボタンで検索結果があれば遷移する
        public async Task ShohinIndex_ShohinNavidateToDeleteSelect()
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

            await Task.Delay(10);

            Assert.Contains("/shohin/delete/select", nav.Uri);
        }
    }
}

