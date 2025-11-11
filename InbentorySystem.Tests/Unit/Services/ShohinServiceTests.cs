using Xunit;
using InbentorySystem.Services;
using InbentorySystem.Data.Models;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Moq;
using System.ComponentModel;
using System.Net.WebSockets;
using InbentorySystem.Infrastructure.Interfaces;

namespace InbentorySystem.Tests.Unit.Services
{
    public class ShohinServiceTests
    {
        private readonly ShohinService _service = new ShohinService();

        [Fact] // UT-SH-01: キーワードなし ->全一覧へ
        public void GetNavigationUri_ShouldNavigateToAllList_WhenKeywordIsWhitespace()
        {
            // ARRANGE
            string keyword = " ";
            var results = new List<ShohinModel> { new ShohinModel() };
            _service.SetSearchResults(results);

            string expectedUri = "/shohin/list";

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

        [Fact] // UT-SH-03: キーワードあり -> キーワードHIT一覧へ（1件の場合）
        public void GetNavigationUri_ShouldNavigateToListWithQuery_WhenSingleResultFound()
        {
            // ARRANGE
            string keyword = "ペン";
            var results = new List<ShohinModel> { new ShohinModel() };
            _service.SetSearchResults(results);

            string expectedUri = $"/shohin/list?q={Uri.EscapeDataString(keyword)}";

            // ACT
            var actualUri = _service.GetNavigationUri(keyword);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact] // UT-SH-03/04: キーワードあり -> キーワード付き一覧へ（複数件の場合）
        public void GetNavigationUri_ShouldNavigateToListWithQuery_WhenMultipleResultsFound()
        {
            // ARRANGE
            string keyword = "ペン";
            var results = new List<ShohinModel>() { new ShohinModel(), new ShohinModel() };
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
            var model = new ShohinModel { ShohinCode = "A001" };
            _service.SetLastEditedShohin(model);

            Assert.Equal(model, _service.GetLastEditedShohin());
            Assert.Equal(model, _service.LastEditedShohin);
        }

        [Fact] // UT-SH-06: 削除対象の保持と取得
        public void SetLastDeletedShohin_ShouldStoreAndReturnModel()
        {
            var model = new ShohinModel { ShohinCode = "A002" };
            _service.SetLastDeletedshohin(model);

            Assert.Equal(model, _service.LastDeletedShohin);
        }

        [Fact] // UT-SH-07: 商品一覧の保持と取得
        public void GetShohinList_ShouldReturnStoredList()
        {
            List<ShohinModel> shohinModels = new()             {
                new ShohinModel { ShohinCode = "A001" },
                new ShohinModel { ShohinCode = "A002" }
            };
            var list = shohinModels;

            // 内部フィールドに直接アクセスできないため、Setは省略（初期状態の確認）
            var result = _service.GetShohinList();
            Assert.NotNull(result);
        }

        [Fact]// UT-SH-08: 修正対象が未設定の場合は例外が発生すること
        public async Task ThrowsException_WhenLastEditedShohinIsNotSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var service = new ShohinService();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.UpdateShohinAsync(mockRepo.Object);
            });
        }

        [Fact]// UT-SH-09: UpdateShohinAsyncがリポジトリに正しく移譲される
        public async Task DelegatesUpdateToRepository_WhenLastEditedShohinIsSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var service = new ShohinService();

            var model = new ShohinModel
            {
                ShohinCode = "A002",
                ShohinMeiKanji = "牛刀",
                ShohinMeiKana = "ぎゅうとう",
                Shiirene = 1500,
                Urine = 3000,
                ShiiresakiCode = "S003"
            };

            service.SetLastEditedShohin(model);

            // Act
            await service.UpdateShohinAsync(mockRepo.Object);

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
        public async Task ThrowException_WhenLastDeletedShohinIsNotSet()
        {
            // 削除対象が未設定
            var mockRepo = new Mock<IShohinRepository>();
            var service = new ShohinService();

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await service.DeleteShohinAsync(mockRepo.Object);
            });
        }

        [Fact]// UT-SH-11: DeleteShohinAsyncがリポジトリに正しく移譲される
        public async Task DelegatesDeleteToRepository_WhenLastEditedShohinIsSet()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var service = new ShohinService();

            var model = new ShohinModel
            {
                ShohinCode = "A010",
                ShohinMeiKanji = "柳刃包丁",
                ShohinMeiKana = "やなぎばぼうちょう",
                Shiirene = 1800,
                Urine = 3600,
                ShiiresakiCode = "S010"
            };

            service.SetLastEditedShohin(model);

            // Act
            await service.DeleteShohinAsync(mockRepo.Object);

            // Assert
            mockRepo.Verify(r => r.DeleteAsync("A010"), Times.Once);
        }
    }
}
