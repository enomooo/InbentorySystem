using Xunit;
using Moq;
using System.Data;
using System.Linq;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Extensions.Options;


namespace InbentorySystem.Tests.Unit.Repositories
{
    public partial class ShohinRepositoryTestsSetup
    {

        [Fact] // UT-SR-05: 全商品取得のテスト
        public async Task GetAllAsync_ShouldReturnAllShohin()
        {
            // ARRANGE
            var expected = new List<ShohinModel>
            {
                new ShohinModel { ShohinCode = "A001" },
                new ShohinModel { ShohinCode = "A002" }
            };

            // SQL実行結果として空リストを返すようにモックを設定
            _mockExecutor.Setup(e => e.QueryAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                null,
                null))
                .ReturnsAsync(expected);

            // ACT
            var result = await _repository.GetAllAsync();

            // ASSERT
            Assert.Equal(2, result.Count);
            Assert.Equal("A001", result.First().ShohinCode);
        }

        [Fact] // UT-SR-05b: 商品が0件の場合でも空リストを返す
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoShohinExists()
        {
            // ARRANGE: SQL実行結果として空リストを返すようにモックを設定
            _mockExecutor.Setup(e => e.QueryAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                null,
                null))
                .ReturnsAsync(new List<ShohinModel>());

            // ACT
            var result = await _repository.GetAllAsync();

            // ASSERT
            Assert.Empty(result);
        }

        [Fact] // UT-SR-06: キーワード検索のテスト
        public async Task SearchByKeywordAsync_ShohinReturnMatchingShohin()
        {
            // ARRANGE
            string keyword = "りんご";
            var expected = new List<ShohinModel>
            {
                new ShohinModel{ShohinMeiKanji = "青りんご"}
            };

            _mockExecutor.Setup(e => e.QueryAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(),
                null))
                .ReturnsAsync(expected);

            // ACT
            var result = await _repository.SearchByKeywordAsync(keyword);

            // ASSERT
            Assert.Single(result);
            Assert.Contains("りんご", result.First().ShohinMeiKanji);
        }

        [Fact] // UT-SR-06b: キーワードに一致する商品がない場合
        public async Task SearchByKeywordAsync_ShohinReturnList_WhenNoMatch()
        {
            // ARRANGE
            _mockExecutor.Setup(e => e.QueryAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(),
                null))
                .ReturnsAsync(new List<ShohinModel>());

            // ACT
            var result = await _repository.SearchByKeywordAsync("存在しないキーワード");

            // ASSERT
            Assert.Empty(result);
        }
    }
}

