using Bunit;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Services;
using InbentorySystem.Pages.Ui.Shohin.Edit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace InbentorySystem.Tests.Unit.Pages.Shohin
{
    public class ShohinEditPageTests
    {
        [Fact] // UT-SE-01: 編集対象の商品が表示される
        public void ShohinEdit_ShouldDisplayInitialValues()
        {
            var model = new ShohinModel
            {
                ShohinCode = "A001",
                ShohinMeiKanji = "牛刀",
                Shiirene = 1500,
                Urine = 3000
            };

            var service = new ShohinService();
            service.SetLastEditedShohin(model);

            // TestContext は bUnit が提供する Blazor テスト環境。
            using var ctx = new TestContext();
            ctx.Services.AddSingleton(service);

            // cutは描画されたコンポーネントのラッパーでコンポーネントをレンダリング
            var cut = ctx.RenderComponent<ShohinEditSelect>();

            // d=shohinMeiKanji の input 要素を検索,value="牛刀"であることを検証
            cut.Find("input[id=shohinMeiKanji]").MarkupMatches($"<input id=\" shohinMeiKanji\" value=\"牛刀\" />");
        }

        [Fact] // UT-SE-02: 商品名が編集できること
        public void ShohinEdit_ShouldAllowEditingShohinMeiKanji()
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
                    Urine = 3000,
                    ShiiresakiCode = "S001"
                });

            ctx.Services.AddSingleton(mockRepo.Object);
            var cut = ctx.RenderComponent<ShohinEditForm>(parameters => parameters.Add(p => p.ShohinCode, "A001"));

            var input = cut.Find("input[id=shohinMeiKanji]");
            input.Change("柳刃包丁");

            var updatedValue = cut.Instance.Shohin.ShohinMeiKanji;

            Assert.Equal("柳刃包丁", updatedValue);
        }

        [Fact] // UT-SE-03: 商品名が未入力の場合はバリデーションエラー
        public void ShohinEdit_ShohinShowError_WhenShohinMeiKanjiIsEmpty()
        {
            using var ctx = new TestContext();
            var service = new ShohinService();
            service.SetLastEditedShohin(new ShohinModel { ShohinMeiKanji = "牛刀" });

            ctx.Services.AddSingleton(service);
            var cut = ctx.RenderComponent<ShohinEditSelect>();

            cut.Find("form").Submit();

            Assert.Contains("商品名は必須です", cut.Markup);
        }
    }
}
