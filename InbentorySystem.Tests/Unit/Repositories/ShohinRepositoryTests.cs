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
    /// <summary>
    /// ShohinRepositoryのテストクラス
    /// </summary>
    public class ShohinRepositoryTests
    {
        private readonly Mock<ISqlExecutor> _mockExecutor = default!;
        private readonly ShohinRepository _repository = default!;

        public ShohinRepositoryTests()
        {
            // IDbConnectionのモックを作成
            _mockExecutor = new Mock<ISqlExecutor>();

            // IDbConnectionFactoryのモックを作成し、テスト用の接続を返すように設定
            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(Mock.Of<IDbConnection>());

            // モックのFactoryを使ってリポジトリを初期化
            _repository = new ShohinRepository(mockFactory.Object, _mockExecutor.Object);
        }
        [Fact] // UT-SR-01: 商品コードが重複している場合のテスト
        public async Task CheckDuplicateCodeAsync_ShouldReturnTrue_WhenCodeExists()
        {
            // ARRANGE
            _mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<long>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(), // param
                It.IsAny<IDbTransaction?>())) // transaction も含める
                .ReturnsAsync(1L);

            string testCode = "A001";

            // ACT
            var result = await _repository.CheckDuplicateCodeAsync(testCode);

            // ASSERT
            Assert.True(result);
        }

        [Fact] // UT-SR-02: 商品コードが重複していない場合のテスト
        public async Task CheckDuplicateCodeAsync_ShouldReturnFalse_WhenCodeDoesNotExist()
        {
            // ARRANGE
            _mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<long>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(), // param
                It.IsAny<IDbTransaction?>())) // transaction も含める
                .ReturnsAsync(0L);

            string testCode = "A999";

            var result = await _repository.CheckDuplicateCodeAsync(testCode);

            // ASSERT
            Assert.False(result);
        }

        [Fact] // UT-SR-03:仕入先コードが存在する場合のテスト
        public async Task CheckShiiresakiExistsAsync_ShouldReturnTrue_WhenCodeExists()
        {
            // ARRANGE
            _mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<long>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(), // param
                It.IsAny<IDbTransaction?>())) // transaction も含める
                .ReturnsAsync(1L);

            string testCode = "S100";

            // ACT
            var result = await _repository.CheckShiiresakiExistsAsync(testCode);

            // ASSERT
            Assert.True(result);
        }

        [Fact] // UT-SR-04:仕入先コードが存在しない場合のテスト
        public async Task CheckShiiresakiExistsAsync_ShouldReturnFalse_WhenCodeDoesNotExists()
        {
            // ARRANGE
            _mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<long>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(), // param
                It.IsAny<IDbTransaction?>())) // transaction も含める
                .ReturnsAsync(0L);
            string testCode = "S999";

            // ACT
            var result = await _repository.CheckShiiresakiExistsAsync(testCode);

            // ASSERT
            Assert.False(result);
        }

        [Fact] // UT-SR-05: 全商品取得のテスト
        public async Task GetAllAsync_ShouldReturnAllShohin()
        {
            // ARRANGE
            var expected = new List<ShohinModel>
            {
                new ShohinModel { ShohinCode = "A001" },
                new ShohinModel { ShohinCode = "A002" }
            };

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

        [Fact] // UT-SR-07: 商品コード検索のテスト
        public async Task GetByCodeAsync_ShouldReturnShohin_WhenCodeExists()
        {
            // ARRANGE

            var expected = new ShohinModel { ShohinCode = "A001" };

            _mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<object?>(),
                null))
                .ReturnsAsync(expected);

            // ACT
            var result = await _repository.GetByCodeAsync("A001");

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("A001", result!.ShohinCode);
        }

        [Fact] // UT-SR-08: 商品登録のテスト（正常系）
        public async Task RegisterAsync_ShohinInsertShohinAndZaiko_WhenValidModel()
        {
            // ARRANGE
            var model = new ShohinModel { ShohinCode = "A001" };

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
            mockConnection.Setup(c => c.Open());

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()
                )).ReturnsAsync(1);

            mockExecutor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()
                )).ReturnsAsync(1);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            var result = await repo.RegisterAsync(model);

            // ASSERT
            Assert.Equal(1, result);
            mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Fact] // UT-SR-09: 商品修正のテスト（正常系）
        public async Task UpdateAsync_ShouldUpdateShohinAndZaiko_WhenValidModel()
        {
            var model = new ShohinModel
            {
                ShohinCode = "A001",
                ShohinMeiKanji = "修正商品",
                ShohinMeiKana = "しゅうせいしょうひん",
                Shiirene = 100,
                Urine = 150,
                ShiiresakiCode = "S001"
            };

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
            mockConnection.Setup(c => c.Open());

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()
                )).ReturnsAsync(1);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            var result = await repo.UpdateAsync(model);

            // ASSERT
            Assert.Equal(1, result);
            mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Fact] // UT-SR-10: 商品削除のテスト（正常系）
        public async Task DeleteAsync_ShouldDeleteShohinAndZaiko_WhenCodeIsValid()
        {
            var code = "A001";

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
            mockConnection.Setup(c => c.Open());

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()
                )).ReturnsAsync(1);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            await repo.DeleteAsync(code);

            // ASSERT
            mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]
        public async Task GetByCodeAsyncで指定コードの商品が取得出来ること()
        {
            // Arrange
            var mockExecutor = new Mock<ISqlExecutor>();
            var mockFactory = new Mock<IDbConnectionFactory>();
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.Open());
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<ShohinModel>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null
                )).ReturnsAsync(new ShohinModel
                {
                    ShohinCode = "A003",
                    ShohinMeiKanji = "ペティナイフ",
                    ShohinMeiKana = "ぺてぃないふ",
                    Shiirene = 800,
                    Urine = 1600,
                    ShiiresakiCode = "S004"
                });

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // Act
            var result = await repo.GetByCodeAsync("A003");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("ペティナイフ", result!.ShohinMeiKanji);
        }
    }
}