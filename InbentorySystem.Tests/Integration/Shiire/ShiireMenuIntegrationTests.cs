using Bunit;
using Bunit.TestDoubles;
using InbentorySystem.Data.Models;
using InbentorySystem.Infrastructure.Interfaces;
using InbentorySystem.Infrastructure.Models;
using InbentorySystem.Pages.Ui.Shiire;
using InbentorySystem.Services.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace InbentorySystem.Tests.Integration.Shiire
{
    public class ShiireMenuIntegrationTests
    {
        private readonly ShiireModel _dummyShiire = new()
        {
            ShiireNo = "1",
            ShiireBi = new DateTime(2023, 10, 25),
            ShohinCode = "S001"
        };

        private readonly List<ShiiresakiModel> _dummyShiiresaki = new()
        {
            new ShiiresakiModel {ShiiresakiCode = "P001", ShiiresakiMeiKanji = "堺刃物" }
        };

        private readonly List<ShohinModel> _dummyShohin = new()
        {
            new ShohinModel {ShohinCode = "S001",ShohinMeiKana = "牛刀"}
        };

        private TestContext SetupContext(Mock<IShiireRepository> mockShiireRepo)
        {
            var ctx = new TestContext();
            ctx.Services.AddSingleton(mockShiireRepo.Object);
            ctx.Services.AddSingleton(Mock.Of<IShohinRepository>(
            r => r.GetAllAsync() == Task.FromResult(_dummyShohin)));
            ctx.Services.AddSingleton(Mock.Of<IShiiresakiRepository>(
                r => r.GetAllAsync() == Task.FromResult(_dummyShiiresaki)));
            ctx.Services.AddSingleton(Mock.Of<IShiireService>());
            ctx.Services.AddSingleton<FakeNavigationManager>();
            return ctx;
        }

        [Fact] //UT-SHM-01: 修正検索のテスト（成功時、ナビゲーション検証）
        public async Task SearchForEdit_ShouldNavigateToSelectScreen_WhenSuccessful()
        {
            var mockRepo = new Mock<IShiireRepository>();
            var ctx = SetupContext(mockRepo);
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();
            var targetDate = new DateTime(2023, 10, 25);

            // Arrange: Repositoryが成功結果を返すよう設定
            mockRepo.Setup(r => r.SearchByDateAsync(
                It.Is<DateTime>(d => d.Date == targetDate.Date),
                It.Is<string>(c => c == _dummyShiire.ShohinCode)))
                .ReturnsAsync(new List<ShiireModel> { _dummyShiire });

            // Act
            var cut = ctx.RenderComponent<ShiireMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiMasterList.Any());

            cut.Find("#editDate").Change("2023-10-25");
            cut.Find("#editShohinCode").Change(_dummyShiire.ShohinCode);

            await cut.FindAll("button").First(b => b.TextContent.Contains("修正画面へ")).ClickAsync(new MouseEventArgs());

            // ASSERT1: Repositoryが正しいDate Time引数で呼ばれたことを検証
            mockRepo.Verify(r => r.SearchByDateAsync(
                It.Is<DateTime>(d => d.Date == targetDate.Date),
                _dummyShiire.ShohinCode), Times.Once);

            // ASSERT2: 修正選択画面に遷移したことを検証
            Assert.Contains("/shiire/edit/select", nav.Uri);
        }

        [Fact] // UT-SHM-02: 修正検索のテスト
        public async Task SearchForEdit_ShouldShowError_WhenDateFormatIsInvalid()
        {
            var mockRepo = new Mock<IShiireRepository>();
            var ctx = SetupContext(mockRepo);

            // Act
            var cut = ctx.RenderComponent<ShiireMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiMasterList.Any());

            cut.Find("#editDate").Change("2023/10/25");
            cut.Find("#editShohinCode").Change("S001");

            await cut.FindAll("button").First( b => b.TextContent.Contains("修正画面へ")).ClickAsync(new MouseEventArgs());

            // Assert
            Assert.Contains("仕入日付の形式が正しくありません (YYYY-MM-DD形式で入力してください)", cut.Markup);
            mockRepo.Verify(r => r.SearchByDateAsync(It.IsAny<DateTime>(), It.IsAny<string>()), Times.Never);
        }

        [Fact] // UT-SHM-03: 削除検索のテスト（成功時、ナビゲーション検証）
        public async Task SearchForDelete_ShouldNavigateToSelectScreen_WhenSuccessful()
        {
            var mockRepo = new Mock<IShiireRepository>();
            var ctx = SetupContext(mockRepo);
            var nav = ctx.Services.GetRequiredService<FakeNavigationManager>();
            var targetDate = new DateTime(2023, 11, 01);

            // ARRANGE: Repositoryが成功結果を返すよう設定
            mockRepo.Setup(r => r.SearchByDateAsync(
                It.Is<DateTime>(d => d.Date == targetDate.Date),
                It.Is<string>(c => c == _dummyShiire.ShohinCode)))
                .ReturnsAsync(new List<ShiireModel> { _dummyShiire });

            // ACT
            var cut = ctx.RenderComponent<ShiireMenu>();

            cut.WaitForState(() => cut.Instance.ShiiresakiMasterList.Any());

            cut.Find("#deleteDate").Change("2023-11-01");
            cut.Find("#deleteShohinCode").Change(_dummyShiire.ShohinCode);

            await cut.FindAll("button").First(b => b.TextContent.Contains("削除画面へ")).ClickAsync(new MouseEventArgs());

            // Assert1: Repositoryが正しいDateTime引数で呼ばれたことを検証
            mockRepo.Verify(r => r.SearchByDateAsync(
            It.Is<DateTime>(d => d.Date == targetDate.Date),
            _dummyShiire.ShohinCode), Times.Once);

            // ASSERT 2: 削除選択画面に遷移したことを検証
            Assert.Contains("/shiire/delete/select", nav.Uri);
        }
    }
}
