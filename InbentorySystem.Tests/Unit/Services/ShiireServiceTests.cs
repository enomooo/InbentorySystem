using InbentorySystem.Services;
using InbentorySystem.Data.Models;
using System;
using System.Collections.Generic;
using Xunit;
using System.Net.WebSockets;

namespace InbentorySystem.Tests.Unit.Services
{
    public class ShiireServiceTests
    {
        private readonly ShiireService _service = new();

        private readonly ShiireModel _dummyShiire = new()
        {
            ShiireNo = "100",
            ShohinCode = "S001",
            Quantity = 10,
            ShiireBi = new DateTime(2023, 11, 27)
        };

        [Fact] // UT-SHSV-01: 検索結果の保持と取得 (Set/GetSearchResults)
        public void SetGetSearchResults_ShouldHandleListCorrectly()
        {
            var results = new List<ShiireModel> { _dummyShiire };
            _service.SetSearchResults(results);

            Assert.Equal(results, _service.GetSearchResults());
            Assert.Single(_service.SearchResults);
        }

        [Fact] // UT-SHSV-02: 最終登録データが正しく保持されること (Set/GetLastRegisteredShiire)
        public void SetGetLastRegisteredShiire_ShouldHoldModel()
        {
            _service.SetLastRegisteredShiire(_dummyShiire);

            Assert.Equal(_dummyShiire, _service.LastRegisteredShiire);
        }

        [Fact] // UT-SHSV-03: 修正前後のデータが正しく保持されること (SetLastEditResults)
        public void SetLastEditResultsShouldHoldAllEditProperties()
        {
            // ARRANGE 
            var original = new ShiireModel { ShohinCode = "S001", Quantity = 5 };
            var updated = new ShiireModel { ShohinCode = "S001", Quantity = 15 };

            // ACT
            _service.SetLastEditResults(original, updated, 50, 60);

            // ASSERT
            Assert.Equal(original, _service.LastEditOriginalShiire);
            Assert.Equal(updated, _service.LastEditedShiire);
            Assert.Equal(50, _service.LastEditBeforeZaikoQuantity);
            Assert.Equal(60, _service.LastEditAfterZaikoQuantity);
            Assert.Equal(updated, _service.GetLastEditedShiire());
        }

        [Fact] // UT-SHSV-04: Clearメソッドで全てnullになること
        public void Clear_ShouldResetAllStateProperties()
        {
            // ARRANGE 
            _service.SetSearchResults(new List<ShiireModel> { _dummyShiire });
            _service.SetLastEditResults(_dummyShiire, _dummyShiire, 1, 1);
            _service.SetLastDeletedShiire(_dummyShiire);

            // ACT
            _service.Clear();

            // ASSERT
            Assert.Empty(_service.SearchResults);
            Assert.Null(_service.LastEditedShiire);
            Assert.Null(_service.LastDeletedShiire);
            Assert.Null(_service.LastEditOriginalShiire);
            Assert.Null(_service.LastEditBeforeZaikoQuantity);
            Assert.Null(_service.LastEditAfterZaikoQuantity);
        }

        [Fact] // UT-SHSV-05: 検索結果がある場合、正しいURIを返すこと
        public void DetermineNavigationUri_ShouldReturnListUri_WhenResultsExist()
        {
            var results = new List<ShiireModel> { _dummyShiire };
            var uri = _service.DetermineNavigationUri("2023-10-01", "S001", results, "Search");

            Assert.Equal("/shiirelist", uri);
        }

        [Fact] // UT-SHSV-06: 検索結果がない場合、nullを返すこと
        public void DetermineNavigationUri_ShouldReturnNull_WhenNoResults()
        {
            var results = new List<ShiireModel>();
            var uri = _service.DetermineNavigationUri("2023-10-01", "S001", results, "Search");

            Assert.Null(uri);
        }

        [Fact] // UT-SHSV-07: SetLastEditResultsが修正前後の全てを保持すること
        public void SetLastEditResults_ShouldHoldAllEditProperties()
        {
            // Arrange
            var original = new ShiireModel { ShohinCode = "S001", Quantity = 5 };
            var updated = new ShiireModel { ShohinCode = "S001", Quantity = 15 };

            // ACT: 修正前在庫 50, 修正後在庫 60 をセット
            _service.SetLastEditResults(original, updated, 50, 60);

            Assert.Equal(original, _service.LastEditOriginalShiire);
            Assert.Equal(updated, _service.LastEditedShiire);
            Assert.Equal(50, _service.LastEditBeforeZaikoQuantity);
            Assert.Equal(60, _service.LastEditAfterZaikoQuantity);
        }

        [Fact] // UT-SHSV-08: Clearメソッドが修正結果のプロパティをリセットすること
        public void Clear_ShouldResetAllEditProperties()
        {
            // Arrange
            _service.SetLastEditResults(_dummyShiire, _dummyShiire, 100, 95);

            // Act
            _service.Clear();

            // Assert
            Assert.Null(_service.LastEditOriginalShiire);
            Assert.Null(_service.LastEditedShiire);
            Assert.Null(_service.LastEditBeforeZaikoQuantity);
            Assert.Null(_service.LastEditAfterZaikoQuantity);
        }

        [Fact] // UT-SHSV-09: 最終削除データが正しく保持されること (Set/GetLastDeletedShiire)
        public void SetGetLastDeletedShiire_ShouldHoldModel()
        {
            _service.SetLastDeletedShiire(_dummyShiire);
            Assert.Equal(_dummyShiire, _service.LastDeletedShiire);
        }

        [Fact] // UT-SHSV-10: 削除後のClearで全てリセットされること
        public void Clear_ShouldResetAllStatePropertiesAfterDelete()
        {
            // Arrange
            _service.SetLastDeletedShiire(_dummyShiire);

            // Act
            _service.Clear();

            // Assert
            Assert.Null(_service.LastDeletedShiire);
        }
    }
}
