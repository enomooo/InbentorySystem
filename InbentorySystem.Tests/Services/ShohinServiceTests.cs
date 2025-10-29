using Xunit;
using InbentorySystem.Services;
using InbentorySystem.Data.Models;
using System.Collections.Generic;

namespace InbentorySystem.Tests.Services
{
    public class ShohinServiceTests
    {
        private readonly ShohinService _service = new ShohinService();

        [Fact] // UT-SH-01: キーワードなし ->全一覧へ
        public void DetermineNavigationUri_ShouldNavigateToAllList_WhenKeywordIsWhitespace()
        {
            // ARRANGE
            string keyword = " ";
            var results = new List<ShohinModel> { new ShohinModel() };
            string expectedUri = "/shohin/list";

            // ACT
            var actualUri = _service.DetermineNavigationUri(keyword, results);

            Assert.Equal(expectedUri, actualUri);
        }

        [Fact] // UT-SH-02: 結果0件 ->遷移しない
        public void DetermineNavigationUri_ShouldReturnEmpty_WhenNoResultsFound()
        {
            // ARRANGE
            string keyword = "存在しない";
            var results = new List<ShohinModel>();
            string expectedUri = string.Empty;

            // ACT
            var actualUri = _service.DetermineNavigationUri(keyword, results);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact] // UT-SH-03: キーワードあり -> キーワードHIT一覧へ（1件の場合）
        public void DetermineNavigationUri_ShouldNavigateToListWithQuery_WhenSingleResultsFound()
        {
            // ARRANGE
            string keyword = "ペン";
            var results = new List<ShohinModel> { new ShohinModel() };
            string expectedUri = $"/shohin/list?q={Uri.EscapeDataString(keyword)}";

            // ACT
            var actualUri = _service.DetermineNavigationUri(keyword, results);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }

        [Fact] // UT-SH-03/04: キーワードあり -> キーワード付き一覧へ（複数件の場合）
        public void DetermineNavigationUri_ShouldNavigateToListWithQuery_WhenMultipleResultsFound()
        {
            // ARRANGE
            string keyword = "ペン";
            var results = new List<ShohinModel>() { new ShohinModel() };
            string expectedUri = $"/shohin/list?q={Uri.EscapeDataString(keyword)}";

            // ACT
            var actualUri = _service.DetermineNavigationUri(keyword, results);

            // ASSERT
            Assert.Equal(expectedUri, actualUri);
        }



    }
}
