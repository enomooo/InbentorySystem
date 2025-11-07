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
    public partial class ShohinRepositoryTestsSetup
    {
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
    }
}

