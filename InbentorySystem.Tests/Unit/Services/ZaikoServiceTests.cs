using InbentorySystem.Services;
using InbentorySystem.Infrastructure.Interfaces;
using Moq;
using System.Data;
using System.Threading.Tasks;
using Xunit;
using Dapper;

namespace InbentorySystem.Tests.Unit.Services
{
    public class ZaikoServiceTests
    {
        private readonly Mock<IDbConnectionFactory> _mockFactory;
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly ZaikoService _service;

        public ZaikoServiceTests()
        {
            _mockConnection = new Mock<IDbConnection>();
            _mockFactory = new Mock<IDbConnectionFactory>();
            _mockFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);
            _service = new ZaikoService(_mockFactory.Object);
        }

        [Fact] // UT-ZSV-01: 在庫数を取得できること (GetCurrentQuantityAsync)
        public async Task GetCurrentQuantityAsync_ShouldReturnQuantity()
        {
            // ARRANGE: DapperのQueryFirstOrDefaultAsyncの動作をシミュレート
            _mockConnection.Setup(c => c.QueryFirstOrDefaultAsync<int>(
                It.Is<string>(sql => sql.Contains("SELECT currentquantity")),
                It.IsAny<object>(),
                null, null, null))
                .ReturnsAsync(150);

            // ACT
            var result = await _service.GetCurrentQuantityAsync("S001");

            // ASSERT
            Assert.Equal(150, result);
        }

        [Fact] // UT-ZSV-02: 在庫数を正確に加算・減算するSQLが実行されること (UpdateQuantityAsync)
        public async Task UpdateQuantityAsync_ShouldExecuteUpdateSQLWithDifference()
        {
            var shohinCode = "S002";
            var quantityDiff = -10;

            // ARRANGE: ExecuteAsyncが呼ばれたことを検証
            _mockConnection.Setup(c => c.ExecuteAsync(
                It.Is<string>(sql => sql.Contains("UPDATE T_ZAIKO")),
                It.Is<object>(p =>
                    (string)p.GetType().GetProperty("shohinCode")!.GetValue(p)! == shohinCode &&
                    (int)p.GetType().GetProperty("QuantityDiff")!.GetValue(p)! == quantityDiff),
                null, null, null))
                .ReturnsAsync(1)
                .Verifiable();

            // ACT
            await _service.UpdateQuantityAsync(shohinCode, quantityDiff);

            // ASSERT: ExecuteAsyncが一度呼ばれたことを検証
            _mockConnection.Verify(c => c.ExecuteAsync(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null, null, null),
                Times.Once);
        }

        [Theory] // UT-ZSV-03: 在庫が十分か判定できること (IsStockSufficientAsync)
        [InlineData(100, 50, true)] // 現在100, 必要50 -> OK
        [InlineData(10, 15, false)] // 現在10, 必要15 -> NG
        [InlineData(20, 20, true)] // 現在20, 必要20 -> OK (境界値)
        public async Task IsStockSufficientAsync_ShouldReturnCorrectBool(int currentStock, int required, bool expected)
        {
            // ARRANGE: GetCurrentQuantityAsyncのモック設定
            _mockConnection.Setup(c => c.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                null, null, null))
                .ReturnsAsync(currentStock);

            // ACT
            var result = await _service.IsStockSufficientAsync("S003", required);

            // ASSERT
            Assert.Equal(expected, result);
        }
    }
}

