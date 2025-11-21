using InbentorySystem.Services;
using InbentorySystem.Data.Models;
using Moq;
using InbentorySystem.Infrastructure.Interfaces;
using System.Linq;

namespace InbentorySystem.Tests.Unit.Services
{
    public class ShohinServiceTests
    {
        // サービスはテストごとに初期化されるようにする
        private ShohinService _service;

        private readonly Mock<IShohinRepository> _mockRepo;

        public ShohinServiceTests()
        {
            _mockRepo = new Mock<IShohinRepository>();
            _service = new ShohinService(_mockRepo.Object);
        }

        [Fact] // UT-SSV-01: クエリが空の場合、全件検索が実行されること
        public async Task SearchShohinAsync_RetrunsAll_WhenQueryIsEmpty()
        {
            // Arrange
            var expectedList = new List<ShohinModel>
            {
                new ShohinModel{ShohinCode = "ALL01"}
            };

            _mockRepo.Setup(r => r.GetAllAsync())
                        .ReturnsAsync(expectedList);

            // ACT
            var result = await _service.SearchShohinAsync("");

            // Assert
            Assert.Equal(expectedList, result);
            _mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
            _mockRepo.Verify(r => r.SearchByKeywordAsync(It.IsAny<string>()), Times.Never);
        }


        [Fact] // UT-SH-01: キーワードなし ->全一覧へ
        public void GetNavigationUri_ShouldNavigateToAllList_WhenKeywordIsWhitespace()
        {
            // ARRANGE
            string keyword = " ";
            var results = new List<ShohinModel> { new ShohinModel() };
            _service.SetSearchResults(results);

            string expectedUri = "/shohin/list?q=all";

            // ACT
            var actualUri = _service.GetNavigationUri(keyword);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact] // UT-SH-02: 結果0件 ->遷移しない
        public void GetNavigationUri_ShouldReturnEmpty_WhenNoResultsFound()
        {
            // ARRANGE
            string keyword = "存在しない";
            List<ShohinModel> results = new List<ShohinModel>();

            string expectedUri = string.Empty;

            // ACT
            var actualUri = _service.GetNavigationUri(keyword);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }

        [Theory] // UT-SH-03/04: キーワードあり -> キーワード付き一覧へ（１件　or　複数件の場合）
        [InlineData("ペン", 1)] // 1件の場合
        [InlineData("ペン", 2)] // 複数件の場合
        [InlineData("特殊記号&?", 5)] //特殊文字を含む場合
        public void GetNavigationUri_ShouldNavigateToListWithQuery_WhenResultFound(string keyword, int count)
        {
            // ARRANGE
            var results = Enumerable.Repeat(new ShohinModel(), count).ToList();
            _service.SetSearchResults(results);

            string expectedUri = $"/shohin/list?q={Uri.EscapeDataString(keyword)}";

            // ACT
            var actualUri = _service.GetNavigationUri(keyword);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact] // UT-SH-05: 修正対象の保持と取得
        public void SetLastEditedShohin_ShouldStoreAndReturnModel()
        {
            // ARRANGE
            var model = new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "編集済み" };

            // ACT
            _service.SetLastEditedShohin(model);

            // ASSERT
            // GettersとPropertiesの両方を検証
            Assert.Equal(model, _service.GetLastEditedShohin());
            Assert.Equal(model, _service.LastEditedShohin);
            Assert.Equal("編集済み", _service.LastEditedShohin!.ShohinMeiKanji);
        }

        [Fact] // UT-SH-06: 削除対象の保持と取得
        public void SetLastDeletedShohin_ShouldStoreAndReturnModel()
        {
            var model = new ShohinModel { ShohinCode = "A002" };
            _service.SetLastDeletedShohin(model);

            Assert.Equal(model, _service.LastDeletedShohin);
        }

        [Fact] // UT-SH-07: 商品一覧の保持と取得
        public void GetShohinList_ShouldReturnEmptyListInitially()
        {
            // ARRANGE/ACT
            var result = _service.GetShohinList();

            // ASSERT
            Assert.NotNull(result);
        }

        [Fact]// UT-SH-08: 修正対象が未設定の場合は例外が発生すること
        public async Task UpdateShohinAsync_ThrowsException_WhenLastEditedShohinIsNotSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.UpdateShohinAsync(mockRepo.Object)
            );
        }

        [Fact]// UT-SH-09: UpdateShohinAsyncがリポジトリに正しく移譲される
        public async Task UpdateShohinAsync_DelegatesUpdateToRepository_WhenLastEditedShohinIsSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();

            var model = new ShohinModel
            {
                ShohinCode = "A002",
                ShohinMeiKanji = "牛刀",
                ShohinMeiKana = "ぎゅうとう",
                Shiirene = 1500,
                Urine = 3000,
                ShiiresakiCode = "S003"
            };

            _service.SetLastEditedShohin(model);

            // Act
            await _service.UpdateShohinAsync(mockRepo.Object);

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.Is<ShohinModel>(m =>
                m.ShohinCode == "A002" &&
                m.ShohinMeiKanji == "牛刀" &&
                m.ShohinMeiKana == "ぎゅうとう" &&
                m.Shiirene == 1500 &&
                m.Urine == 3000 &&
                m.ShiiresakiCode == "S003"
                )), Times.Once);
        }

        [Fact] // UT-SH-10: 削除対象が未設定の場合は例外が発生すること
        public async Task DeleteShohinAsync_ThrowsException_WhenLastDeletedShohinIsNotSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.DeleteShohinAsync(mockRepo.Object) 
            );
        }

        [Fact]// UT-SH-11: DeleteShohinAsyncがリポジトリに正しく移譲される
        public async Task DeleteShohinAsync_DelegatesDeleteToRepository_WhenLastDeletedShohinIsSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();

            var model = new ShohinModel
            {
                ShohinCode = "A010",
                ShohinMeiKanji = "柳刃包丁",
                Shiirene = 1800,
            };

            _service.SetLastDeletedShohin(model);

            // Act
            await _service.DeleteShohinAsync(mockRepo.Object);

            // Assert
            mockRepo.Verify(r => r.DeleteAsync("A010"), Times.Once);
        }
    }
}
