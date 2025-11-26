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
using InbentorySystem.Infrastructure.Repository;


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
                It.Is<IDbTransaction?>(t => t == mockTransaction.Object)
                )).ReturnsAsync(1);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            await repo.DeleteAsync(code);

            // ASSERT
            mockTransaction.Verify(t => t.Commit(), Times.Once);
            mockTransaction.Verify(t => t.Rollback(), Times.Never);

            mockExecutor.Verify(
                e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()),
                Times.Exactly(3),
                "T_ZAIKOとM_SHOHINの２つのDELETEが実行されるべきです。");
        }

        [Fact] // UT-SR-10b: 削除時に例外が発生した場合はRollbackされること
        public async Task DeleteAsync_ShouldRollbackk_WhenExceptionOccurs()
        {
            // ARRANGE
            var code = "A001";

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()))
                .ThrowsAsync(new Exception("削除失敗"));

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT & ASSERT(例外が再スローされることを検証し、処理を進める)
            await Assert.ThrowsAsync<Exception>(() => repo.DeleteAsync(code));

            mockTransaction.Verify(t => t.Rollback(), Times.Once);
            mockTransaction.Verify(t => t.Commit(), Times.Never);  
        }

        [Fact] // UT-SR-10c: 削除対象が存在しない場合(ExcuteAsyncが0)
        public async Task DeleteAsync_ShouldReturnSilently_WhenNoRowsAffected()
        {
            // ARRANGE
            var code = "zzzzz999";

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
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

            // ASSERT
            mockTransaction.Verify(t => t.Commit(), Times.Once);
            mockTransaction.Verify(t => t.Rollback(), Times.Never);

            mockExecutor.Verify(
            e => e.ExecuteAsync(It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<IDbTransaction?>()),
            Times.Exactly(3));
        }

        [Fact]　// UT-SR-11: 商品削除のテスト（正常系）
        public async Task GetByCodeAsync_ShouldReturnShohin_WhenCodeExists()
        {
            // Arrange
            var shohinCode = "A003";
            var expectedModel = new ShohinModel
            {
                ShohinCode = shohinCode,
                ShohinMeiKanji = "ペティナイフ",
                ShohinMeiKana = "ぺてぃないふ",
                Shiirene = 800,
                Urine = 1600,
                ShiiresakiCode = "S004"
            
            };

            var mockExecutor = new Mock<ISqlExecutor>();
            var mockFactory = new Mock<IDbConnectionFactory>();
            var mockConnection = new Mock<IDbConnection>();

            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
                It.Is<string>(sql => sql.Contains("WHERE shohin_code = @ShohinCode")),
                It.Is<object>(p => p != null && p.ToString()!.Contains(shohinCode)),
                It.IsAny<IDbTransaction>()
                )).ReturnsAsync(expectedModel);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // Act
            var result = await repo.GetByCodeAsync(shohinCode);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(shohinCode, result!.ShohinCode);
            Assert.Equal("ペティナイフ", result.ShohinMeiKanji);
        }

        [Fact]　// UT-SR-11b: 存在しない商品コードを指定した場合
        public async Task GetByCodeAsync_ShouldReturnNull_WhenCodeDoesNotExist()
        {
            // Arrange
            var mockExecutor = new Mock<ISqlExecutor>();
            var mockFactory = new Mock<IDbConnectionFactory>();
            var mockConnection = new Mock<IDbConnection>();

            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<ShohinModel>(
                It.IsAny<IDbConnection>(),
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
