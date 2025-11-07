using Bunit;
using InbentorySystem.Components.Pages.Shohin;
using InbentorySystem.Components.Pages.Shohin.Edit;
using InbentorySystem.Data.Models;
using InbentorySystem.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using Xunit;

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
            var service = new ShohinService();
            service.SetLastEditedShohin(new ShohinModel { ShohinMeiKanji = "牛刀" });

            ctx.Services.AddSingleton(service);
            var cut = ctx.RenderComponent<ShohinEditSelect>();

            var input = cut.Find("input[id=shohinMeiKanji]");
            input.Change("柳刃包丁");

            Assert.Equal("柳刃包丁", service.LastEditedShohin.ShohinMeiKanji);
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
