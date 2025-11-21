using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Pages.Ui.Shohin;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace InbentorySystem.Tests.Unit.Pages.Shohin
{
    public class ShohinSearchPageTests
    {
        private readonly Mock<IShohinRepository> _mockRepo = new();
        private readonly TestContext _ctx = new();

        public ShohinSearchPageTests()
        {
            // DI登録はコンストラクタで行う
            // ServiceContextのロックは、各テストメソッド内でctxを再初期化することで回避

            _ctx.Services.AddSingleton(_mockRepo.Object);
            // FakeNavigationManagerはTestContextが自動で提供
        }

        // ----------------------------------------------------------------------
        // UT-SS-01: 検索結果が表示される (正常系)
        // ----------------------------------------------------------------------
        [Fact]
        public async Task ShohinSearch_ShohinRenderShohinList_WhenDataIsFound()
        {
            // Arrange: 必要なサービスを再取得（TestContextを分離して使う）
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();
            var mockRepo = new Mock<IShohinRepository>();
            ctx.Services.AddSingleton(mockRepo.Object);

            var expectedData = new List<ShohinModel>
            {
                new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "牛刀", Urine = 3000 }
            };

            // データ取得にわずかな遅延をシミュレートし、非同期処理を明確にする
            mockRepo.Setup(r => r.SearchByKeywordAsync("牛刀"))
                .Returns(async () =>
                {
                    await Task.Delay(1); // 非同期処理を保証
                    return expectedData;
                });


            // ACT 1: クエリ q=牛刀 でナビゲートをシミュレート
            nav.NavigateTo("/shohin/search?q=牛刀");
            var cut = ctx.RenderComponent<ShohinSearch>();

            // ASSERT: 非同期ロード完了を待つ (データが表示されるまで待機)
            cut.WaitForAssertion(() =>
            {
                // UIにデータが反映されたことを検証
                Assert.Contains("牛刀", cut.Markup);
                Assert.Contains("3000", cut.Markup);

                // リポジトリが一度だけ呼ばれたことを検証
                mockRepo.Verify(r => r.SearchByKeywordAsync("牛刀"), Times.Once);

            }, TimeSpan.FromSeconds(2));
        }

        // ----------------------------------------------------------------------
        // UT-SS-02: 検索結果が0件なら警告表示 (データなし)
        // ----------------------------------------------------------------------
        [Fact]
        public async Task ShohinSearch_ShouldShowWarning_WhenNoResults()
        {
            using var ctx = new TestContext();
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();

            // ARRANGE: Mock設定 - 空リストを返す
            var mockRepo = new Mock<IShohinRepository>();
            mockRepo.Setup(r => r.SearchByKeywordAsync("なし"))
                .ReturnsAsync(new List<ShohinModel>()); // 空リストを返す
            ctx.Services.AddSingleton(mockRepo.Object);

            // ACT: クエリ q=なし でナビゲート
            nav.NavigateTo("/shohin/search?q=なし");
            var cut = ctx.RenderComponent<ShohinSearch>();

            // ASSERT: 警告メッセージが表示されるのを待つ
            cut.WaitForAssertion(() =>
            {
                Assert.Contains("該当する商品が見つかりませんでした", cut.Markup);
            });
        }

        // ... (他のテストも同様に async Task と await を使って修正) ...
    }
}