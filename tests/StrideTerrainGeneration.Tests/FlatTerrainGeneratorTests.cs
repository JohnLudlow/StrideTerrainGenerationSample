using StrideTerrainGeneration.Core;
using StrideTerrainGeneration.Core.Configuration;

namespace StrideTerrainGeneration.Tests;

public class FlatTerrainGeneratorTests
{
    [Fact]
    public void Generate_WithDefaultConfig_ReturnsTerrainData()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 64, Height = 64, BaseHeight = 0.5f };

        // Act
        var terrain = generator.Generate(config);

        // Assert
        Assert.NotNull(terrain);
        Assert.Equal(64, terrain.Width);
        Assert.Equal(64, terrain.Height);
    }

    [Fact]
    public void Generate_WithSpecificBaseHeight_AllHeightsMatch()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 32, Height = 32, BaseHeight = 0.3f };

        // Act
        var terrain = generator.Generate(config);

        // Assert
        for (var x = 0; x < terrain.Width; x++)
        {
            for (var y = 0; y < terrain.Height; y++)
            {
                Assert.Equal(0.3f, terrain.GetHeight(x, y), precision: 5);
            }
        }
    }

    [Fact]
    public void Generate_WithDifferentDimensions_CreatesCorrectSize()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 128, Height = 256 };

        // Act
        var terrain = generator.Generate(config);

        // Assert
        Assert.Equal(128, terrain.Width);
        Assert.Equal(256, terrain.Height);
    }

    [Fact]
    public void Generate_WithZeroBaseHeight_AllHeightsAreZero()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 16, Height = 16, BaseHeight = 0.0f };

        // Act
        var terrain = generator.Generate(config);

        // Assert
        for (var x = 0; x < terrain.Width; x++)
        {
            for (var y = 0; y < terrain.Height; y++)
            {
                Assert.Equal(0.0f, terrain.GetHeight(x, y));
            }
        }
    }

    [Fact]
    public void Generate_WithMaxBaseHeight_AllHeightsAreOne()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 16, Height = 16, BaseHeight = 1.0f };

        // Act
        var terrain = generator.Generate(config);

        // Assert
        for (var x = 0; x < terrain.Width; x++)
        {
            for (var y = 0; y < terrain.Height; y++)
            {
                Assert.Equal(1.0f, terrain.GetHeight(x, y));
            }
        }
    }

    [Fact]
    public async Task GenerateAsync_WithConfig_ReturnsTerrainData()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 64, Height = 64, BaseHeight = 0.5f };

        // Act
        var terrain = await generator.GenerateAsync(config);

        // Assert
        Assert.NotNull(terrain);
        Assert.Equal(64, terrain.Width);
        Assert.Equal(64, terrain.Height);
    }

    [Fact]
    public async Task GenerateAsync_WithCancellation_ThrowsTaskCanceledException()
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = 512, Height = 512 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAsync<TaskCanceledException>(
            async () => await generator.GenerateAsync(config, cts.Token));
    }

    [Theory]
    [InlineData(64, 64)]
    [InlineData(128, 128)]
    [InlineData(256, 256)]
    [InlineData(512, 512)]
    public void Generate_VariousSizes_CompletesSuccessfully(int width, int height)
    {
        // Arrange
        var generator = new FlatTerrainGenerator();
        var config = new TerrainConfig { Width = width, Height = height };

        // Act
        var terrain = generator.Generate(config);

        // Assert
        Assert.Equal(width, terrain.Width);
        Assert.Equal(height, terrain.Height);
    }
}
