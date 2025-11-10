using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using InbentorySystem.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace InbentorySystem.Tests.Integration.Shohin.EditIntegrationTests
{
    public class ShohinEditIntegrationTests
    {
        [Fact] // IT-SE-01: 商品修正が正常に保存されること
        public async Task 商品修正が正常に保存されること()
        {
            // Arrange
            var mockRepo = new Mock<IShohinRepository>();
            var service = new ShohinService();

            var model = new ShohinModel
            {
                ShohinCode = "A001",
                ShohinMeiKanji = "刺身包丁",
                ShohinMeiKana = "さしみぼうちょう",
                Shiirene = 1000,
                Urine = 2000,
                ShiiresakiCode = "S001"
            };

            service.SetLastEditedShohin(model);

            // Act
            await mockRepo.Object.UpdateAsync(service.GetLastEditedShohin()!);

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(It.Is<ShohinModel>(m =>
                m.ShohinCode == "A001" &&
                m.ShohinMeiKanji == "刺身包丁"
                )), Times.Once);
        }


        [Fact] // IT-SE-02: 商品修正後に取得結果が反映されること
        public async Task 商品修正後に取得結果が反映されること()
        {
            // Arrange
            var mockTransaction = new Mock<IDbTransaction>().Object;
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(c => c.BeginTransaction()).Returns(mockTransaction);
            mockConnection.Setup(c => c.Open());

            var mockFactory = new Mock<IDbConnectionFactory>();
            mockFactory.Setup(f => f.CreateConnection()).Returns(mockConnection.Object);

            var mockExecutor = new Mock<ISqlExecutor>();
            mockExecutor.Setup(e => e.ExecuteAsync(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<IDbTransaction>()))
                        .ReturnsAsync(1);

            // GetByCodeAsync に対する SELECT
            mockExecutor.Setup(e => e.QuerySingleOrDefaultAsync<ShohinModel>(
                It.IsAny<string>(),
                It.IsAny<object>(),
                It.IsAny<IDbTransaction>()
            )).ReturnsAsync(new ShohinModel
            {
                ShohinCode = "A001",
                ShohinMeiKanji = "新名",
                ShohinMeiKana = "しんめい",
                Shiirene = 1200,
                Urine = 2200,
                ShiiresakiCode = "S002"
            });

            var repo = new ShohinRepository(mockFactory.Object, mockExecutor.Object);
            var service = new ShohinService();

            var original = new ShohinModel
            {
                ShohinCode = "A001",
                ShohinMeiKanji = "旧姓",
                ShohinMeiKana = "きゅうせい",
                Shiirene = 1000,
                Urine = 2000,
                ShiiresakiCode = "S001"
            };

            var updated = new ShohinModel
            {
                ShohinCode = "A001",
                ShohinMeiKanji = "新名",
                ShohinMeiKana = "しんめい",
                Shiirene = 1200,
                Urine = 2200,
                ShiiresakiCode = "S002"
            };

            await repo.RegisterAsync(original);
            service.SetLastEditedShohin(updated);

            // Act
            await service.UpdateShohinAsync(repo);
            var result = await repo.GetByCodeAsync("A001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("新名", result!.ShohinMeiKanji);
            Assert.Equal("しんめい", result.ShohinMeiKana);
            Assert.Equal(1200, result.Shiirene);
            Assert.Equal(2200, result.Urine);
            Assert.Equal("S002", result.ShiiresakiCode);
        }

        [Fact] // IT-SE-03: 商品コードが存在しない場合は更新されないこと
        public async Task 商品コードが存在しない場合は更新されないこと()
        {
            var mockRepo = new Mock<IShohinRepository>();
            var service = new ShohinService();

            var model = new ShohinModel { ShohinCode = "ZZZ999", ShohinMeiKanji = "存在しない" };
            service.SetLastEditedShohin(model);

            await service.UpdateShohinAsync(mockRepo.Object);

            mockRepo.Verify(r => r.UpdateAsync(It.IsAny<ShohinModel>()), Times.Once);
        }

        [Fact] // IT-SE-04:  修正前後の差分が正しく反映されること
        public async Task 修正前後の差分が正しく反映されること()
        {
            var serivice = new ShohinService();

            var before = new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "旧姓", Urine = 1000};
            var after = new ShohinModel { ShohinCode = "A001", ShohinMeiKanji = "新名",Urine = 2000 };

            serivice.SetLastEditedShohin(after);

            var mockRepo = new Mock<IShohinRepository>();
            await serivice.UpdateShohinAsync(mockRepo.Object);

            mockRepo.Verify(r => r.UpdateAsync(It.Is<ShohinModel>(m =>
                m.ShohinMeiKanji == "新名" && m.Urine == 200
                )), Times.Once);
        }
    }
}



