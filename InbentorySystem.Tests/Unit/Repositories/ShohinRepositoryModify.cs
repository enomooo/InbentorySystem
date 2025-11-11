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
    /// <summary>
    /// ShohinRepositoryのテストクラス
    /// </summary>
    public partial class ShohinRepositoryTestsSetup
    {
        [Fact] // UT-SR-08: 商品登録のテスト（正常系）
        public async Task RegisterAsync_ShohinInsertShohinAndZaiko_WhenValidModel()
        {
            // ARRANGE

            // 登録対象の商品モデル
            var model = new ShohinModel { ShohinCode = "A001" };

            // 接続とトランザクションのモックを用意
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            // トランザクション処理が始まったら、モックを返す
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
            mockConnection.Setup(c => c.Open());

            // 接続生成の制御
            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            // ExcuteAsyncを2回呼び出すため、明示的に同じ設定を２回記述
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

            // モック依存でShohinRepositoryを構築
            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            var result = await repo.RegisterAsync(model);

            // ASSERT
            Assert.Equal(1, result);

            // トランザクションがCommit()が呼ばれたかをVerifyで検証
            mockTransaction.Verify(t => t.Commit(), Times.Once);
        }

        [Fact] // UT-SR-08b: 登録時に例外が発生した場合はrollbackされる
        public async Task RegisterAsync_ShouldRollback_WhenExceptionOccurs()
        {
            // ARRANGE

            // 登録対象の商品モデル
            var model = new ShohinModel { ShohinCode = "A001" };

            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction.Object);
            mockConnection.Setup(c => c.Open());

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            // 例外スロー
            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object?>(),
                It.IsAny<IDbTransaction?>()
                )).ThrowsAsync(new Exception("SQL失敗"));

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            await Assert.ThrowsAsync<Exception>(() => repo.RegisterAsync(model));

            // ASSERT
            mockTransaction.Verify(t =>t.Rollback(), Times.Once);
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

        [Fact] // UT-SR-09b: 修正処理の時にExecuteAsyncが実行数0をを返した場合
        public async Task UpdateAsync_ShouldReturnZero_WhenNoRowsAffected()
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
                // return => 0
                )).ReturnsAsync(0);

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);

            // ACT
            var result = await repo.UpdateAsync(model);

            // ASSERT
            Assert.Equal(0, result);
            mockTransaction.Verify(t => t.Commit(), Times.Once);
        }
    }
}