using InbentorySystem.Data.Models;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

namespace InbentorySystem.Tests.Unit.Repositories.ShiireRepository
{
    public partial class ShiireRepositoryTestsSearch
    {
        [Fact] //UT-SHR-01: 月単位検索のテスト（正常系）
        public async Task SearchByMonthAsync_ShouldReturnMatchingShiire()
        {
            var expected = new List<ShiireModel> { new ShiireModel { ShiireNo = "1", ShohinCode = "S001" } };

            _mockExecutor.Setup(e => e.QueryAsync<ShiireMoedel>(
                It.IsAny<IDbConnection>(),
                It.Is<string>(sql => sql.Contains("WHERE shiire_bi >= @StartDate")),
                It.IsAny<DynamicParameters>(),
                null))
                .ReturnsAsync(expected);

            var result = await _repository.SearchByMonthAsync(2023, 10, "S001");

            Assert.Single(result);
            Assert.Equal("S001", result.First().ShohinCode);

            _mockExecutor.Verify(e => e.QueryAsync<ShiireMoedel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>(),
                null),
                Times.Once);
        }

        [Fact] // UT-SHR-02: 日付検索のテスト（正常系）
        public async Task SearchByDateAsync_ShouldReturnMatchingShiire()
        {
            var targetDate = new DateTime(2023, 10, 15);
            var expected = new List<ShiireModel> { new ShiireModel { ShiireNo = "2", ShohinCode = "S002" } };

            _mockExecutor.Verify(e => e.QueryAsync<ShiireMoedel>(
                It.IsAny<IDbConnection>(),
                It.IsAny<string>(),
                It.IsAny<DynamicParameters>(),
                null),
                .ReturnsAsync(expected);

            var result = await _ repository.SearchByDateAsync(targetDate, "S002");
            Assert.Single(result);
        }

        [Fact] // UT-SHR-03: 単一取得のテスト
        public async Task GetByDateAndCodeAsync_ShouldReturnSingleShiire()
        {
            var targetDate = new DateTime(2023, 10, 15);
            var expected = new ShiireModel { ShiireNo = "2", ShohinCode = "S002" };
        }









        }

