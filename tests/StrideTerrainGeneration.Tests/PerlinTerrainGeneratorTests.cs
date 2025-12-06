using StrideTerrainGeneration.Core;
using StrideTerrainGeneration.Core.Configuration;

namespace StrideTerrainGeneration.Tests;

public class PerlinTerrainGeneratorTests
{
    [Fact]
    public void Generate_WithValidConfig_ReturnsTerrainData()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator();
        var config = new TerrainConfig { Width = 64, Height = 64, Seed = 12345 };

        // Act
        var result = generator.Generate(config);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(64, result.Width);
        Assert.Equal(64, result.Height);
    }

    [Fact]
    public void Generate_WithSameSeed_ProducesSameResults()
    {
        // Arrange
        var generator1 = new PerlinTerrainGenerator();
        var generator2 = new PerlinTerrainGenerator();
        var config = new TerrainConfig { Width = 32, Height = 32, Seed = 42 };

        // Act
        var result1 = generator1.Generate(config);
        var result2 = generator2.Generate(config);

        // Assert
        for (var y = 0; y < config.Height; y++)
        {
            for (var x = 0; x < config.Width; x++)
            {
                Assert.Equal(result1.GetHeight(x, y), result2.GetHeight(x, y), 5);
            }
        }
    }

    [Fact]
    public void Generate_WithDifferentSeeds_ProducesDifferentResults()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator();
        var config1 = new TerrainConfig { Width = 32, Height = 32, Seed = 1 };
        var config2 = new TerrainConfig { Width = 32, Height = 32, Seed = 2 };

        // Act
        var result1 = generator.Generate(config1);
        var result2 = generator.Generate(config2);

        // Assert
        var differenceCount = 0;
        for (var y = 0; y < 32; y++)
        {
            for (var x = 0; x < 32; x++)
            {
                if (Math.Abs(result1.GetHeight(x, y) - result2.GetHeight(x, y)) > 0.01f)
                {
                    differenceCount++;
                }
            }
        }

        Assert.True(differenceCount > 100, $"Expected significant differences, found only {differenceCount}");
    }

    [Fact]
    public void Generate_HeightValues_AreWithinExpectedRange()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator
        {
            MinHeight = 0.0f,
            MaxHeight = 1.0f
        };
        var config = new TerrainConfig { Width = 64, Height = 64 };

        // Act
        var result = generator.Generate(config);

        // Assert
        for (var y = 0; y < config.Height; y++)
        {
            for (var x = 0; x < config.Width; x++)
            {
                var height = result.GetHeight(x, y);
                Assert.InRange(height, -0.1f, 1.1f); // Small tolerance for floating point
            }
        }
    }

    [Fact]
    public void Generate_CreatesVariedTerrain()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator();
        var config = new TerrainConfig { Width = 64, Height = 64 };

        // Act
        var result = generator.Generate(config);

        // Assert - terrain should have variation (not all same height)
        var firstHeight = result.GetHeight(0, 0);
        var hasVariation = false;

        for (var y = 0; y < config.Height && !hasVariation; y++)
        {
            for (var x = 0; x < config.Width && !hasVariation; x++)
            {
                if (Math.Abs(result.GetHeight(x, y) - firstHeight) > 0.01f)
                {
                    hasVariation = true;
                }
            }
        }

        Assert.True(hasVariation, "Perlin terrain should have height variation");
    }

    [Fact]
    public async Task GenerateAsync_WithValidConfig_ReturnsTerrainData()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator();
        var config = new TerrainConfig { Width = 64, Height = 64 };

        // Act
        var result = await generator.GenerateAsync(config);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(64, result.Width);
        Assert.Equal(64, result.Height);
    }

    [Fact]
    public async Task GenerateAsync_WithCancellation_ThrowsOperationCanceledException()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator();
        var config = new TerrainConfig { Width = 512, Height = 512 };
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act & Assert
        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            async () => await generator.GenerateAsync(config, cts.Token));
    }

    [Fact]
    public void Generate_WithInvalidDimensions_ThrowsArgumentException()
    {
        // Arrange
        var generator = new PerlinTerrainGenerator();
        var config = new TerrainConfig { Width = 0, Height = 64 };

        // Act & Assert
        Assert.Throws<ArgumentException>(() => generator.Generate(config));
    }
}
