using Xunit;
using Moq;
using System.Data;
using System.Linq;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using System.Threading.Tasks;
using System.Net.WebSockets;
using Microsoft.Extensions.Options;
using InbentorySystem.Infrastructure.Interfaces;


namespace InbentorySystem.Tests.Unit.Repositories
{
    public partial class ShohinRepositoryTestsSetup
    {
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

        [Fact] // UT-SR-10b: 削除時に例外が発生した場合はRollbackされること
        public async Task DeleteAsync_ShouldRollbackk_WhenExceptionOccurs()
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
                It.IsAny<IDbTransaction?>()))
                // 例外発生
                .ThrowsAsync(new Exception("削除失敗"));

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            await repo.DeleteAsync(code);

            // ASSERT(Rollback)
            mockTransaction.Verify(t => t.Rollback(), Times.Once);
        }

        [Fact] // UT-SR-10c: 削除対象が存在しない場合(ExcuteAsyncが0)
        public async Task DeleteAsync_ShouldReturnSilently_WhenNoRowsAffected()
        {
            var code = "zzzzz999";

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
                )).ReturnsAsync(0);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            await repo.DeleteAsync(code);

            // ASSERT(Commit)
            mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Fact]　// UT-SR-11: 商品削除のテスト（正常系）
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

        [Fact]　// UT-SR-11b: 存在しない商品コードを指定した場合
        public async Task GetByCodeAsync_ShouldReturnNull_WhenCodeDoesNotExist()
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
                null)).ReturnsAsync((ShohinModel?)null);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // Act
            var result = await repo.GetByCodeAsync("zz9z99z9");

            // Assert
            Assert.Null(result);
        }
    }
}
