using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Infrastructure.Repository;
using Moq;
using System;
using System.Data;
using System.Threading.Tasks;
using Xunit;

namespace InbentorySystem.Tests.Unit.Repositories
{
    public partial class ShiireRepositoryTestsModify
    {
        protected readonly Mock<ISqlExecutor> _mockExecutor;
        protected readonly ShiireRepository _repository;

        [Fact] // UT-SHR-04: 登録のテスト（正常系、トランザクション検証）
        public async Task RegisterAsync_ShouldInsertShiireAndZaiko_WithTransaction()
        {
            // ARRANGE
            var model = new ShiireModel
            {
                ShiireBi = DateTime.Now,
                ShohinCode = "S001",
                ShiiresakiCode = "P001",
                Quantity = 5
            };

            _mockExecutor.Setup(e => e.ExecuteInTransactionAsync(
                It.Is<string>(sql => sql.Contains("INSERT INTO t_shiire") && sql.Contains("ON CONFLICT")),
                It.IsAny<object>()))
                .ReturnsAsync(2);

            // ACT
            var result = await _repository.RegisterAsync(model);

            // ASSERT
            Assert.Equal(2, result);

            _mockExecutor.Verify(e => e.ExecuteInTransactionAsync(
                It.IsAny<string>(),
                It.IsAny<object>()),
                Times.Once);
        }

        [Fact] // UT-SHR-05: 修正のテスト（正常系、在庫差分検証）
        public async Task UpdateAsync_ShouldCalculateDifferenceAndUpdateZaiko()
        {
            // ARRANGE
            var originalShiire = new ShiireModel { ShiireNo = "1", ShohinCode = "S001", Quantity = 10 };
            var updatedShiire = new ShiireModel { ShiireNo = "1", ShohinCode = "S001", Quantity = 15, ShiireBi = DateTime.Now };

            _mockExecutor.Setup(e => e.QueryFirstOrDefaultAsync<int>(
                It.IsAny<IDbConnection>(),
                It.Is<string>(sql => sql.Contains("SELECT suryo FROM t_shiire")),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()))
                .ReturnsAsync(10);

            _mockExecutor.Setup(e => e.ExecuteAsync(
                It.Is<string>(sql => sql.Contains("UPDATE t_shiire")),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            _mockExecutor.Setup(e => e.ExecuteAsync(
                It.Is<string>(sql => sql.Contains("UPDATE t_zaiko")),
                It.Is<object>(p => (int)p.GetType().GetProperty("QuantityDifference")!
                                    .GetValue(p)! == 5),
                It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1)
                .Verifiable();

            var repo = (ShiireRepository)Activator.CreateInstance(typeof(ShiireRepository),
                                                                    Mock.Of<IDbConnectionFactory>(),
                                                                    _mockExecutor.Object)!;

            // ACT
            var result = await repo.UpdateAsync(updatedShiire);

            // ASSERT
            Assert.Equal(1, result);
            _mockExecutor.Verify();
        }

        [Fact] // UT-SHR-06: 削除のテスト（正常系、在庫払い戻し検証）
        public async Task DeleteAsync_ShouldDeleteShiireAndRefundZaiko()
        {
            // ARRANGE
            var targetDate = new DateTime(2023, 10, 15);
            var targetCode = "S001";
            var targetQuantity = 5;

            _mockExecutor.Setup(e => e.ExecuteAsync(
               It.Is<string>(sql => sql.Contains("UPDATE t_zaiko")),
               It.Is<object>(p => (int)p.GetType().GetProperty("QuantityDifference")!
                                    .GetValue(p)! == 5),
               It.IsAny<IDbTransaction>()))
               .ReturnsAsync(1);

            _mockExecutor.Setup(e => e.ExecuteAsync(
                It.Is<string>(sql => sql.Contains("DELETE FROM T_SHIIRE")),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()))
                .ReturnsAsync(1);

            var repo = (ShiireRepository)Activator.CreateInstance(typeof(ShiireRepository),
                                                                    Mock.Of<IDbConnectionFactory>(),
                                                                    _mockExecutor.Object)!;

            // ACT
            var result = await repo.DeleteAsync(targetDate, targetCode, targetQuantity);

            // ASSERT
            Assert.Equal(1, result);
            _mockExecutor.Verify(e => e.ExecuteAsync(
                It.Is<string>(sql => sql.Contains("UPDATE t_zaiko")),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()),
                Times.Once);
        }
    }
}
