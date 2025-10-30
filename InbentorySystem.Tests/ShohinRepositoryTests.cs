using Xunit;
using Microsoft.Extensions.DependencyInjection;
using InbentorySystem.Data;
using InbentorySystem.Data.Models;
using InbentorySystem;
using System.Threading.Tasks;
using InbentorySystem.Tests;

public class ShohinRepositoryTests : IClassFixture<CustomWebApplicationFactory<InbentorySystem.Program>>
{
    private readonly CustomWebApplicationFactory<InbentorySystem.Program> _factory;

    public ShohinRepositoryTests(CustomWebApplicationFactory<InbentorySystem.Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterAsync_ShouldInsertShohinAndZaiko()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();

        var repository = scope.ServiceProvider.GetRequiredService<IShohinRepository>();

        var testShohin = new ShohinModel
        {
            ShohinCode = "T001",
            ShohinMeiKanji = "テスト商品",
            ShohinMeiKana = "テストショウヒン",
            Shiirene = 100,
            Urine = 200,
            ShiiresakiCode = "S001"
        };

        // Act
        var affectedRows = await repository.RegisterAsync(testShohin);

        // Assert
        Assert.Equal(1, affectedRows);

        var result = await repository.GetByCodeAsync("T001");
        Assert.NotNull(result);
        Assert.Equal("テスト商品", result.ShohinMeiKanji);


        await repository.DeleteAsync("T001");
    }

    [Fact]
    public async Task SearchAsync_ShouldReturnMatchingItems()
    {
        // Arrange
        using var scope = _factory.Services.CreateScope();
        var repository = scope.ServiceProvider.GetRequiredService<IShohinRepository>();

        // Act
        var results = await repository.SearchAsync("テスト");

        Assert.True(results.Count > 0);
        Assert.All(results, s => Assert.Contains("テスト", s.ShohinMeiKanji));
    }
}


