using StrideTerrainGeneration.Core.Noise;

namespace StrideTerrainGeneration.Tests.Noise;

/// <summary>
/// Tests for the LayeredNoiseGenerator class.
/// </summary>
public class LayeredNoiseGeneratorTests
{
    [Fact]
    public void Constructor_InitializesWithSeed()
    {
        // Arrange & Act
        var generator = new LayeredNoiseGenerator(42);

        // Assert
        Assert.Equal(42, generator.Seed);
        Assert.Empty(generator.Layers);
    }

    [Fact]
    public void AddLayer_AddsLayerToList()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        var layer = new NoiseLayer
        {
            Name = "Test Layer",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Add
        };

        // Act
        generator.AddLayer(layer);

        // Assert
        Assert.Single(generator.Layers);
        Assert.Equal("Test Layer", generator.Layers[0].Name);
    }

    [Fact]
    public void RemoveLayerAt_RemovesCorrectLayer()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        var layer1 = new NoiseLayer { Name = "Layer 1" };
        var layer2 = new NoiseLayer { Name = "Layer 2" };
        var layer3 = new NoiseLayer { Name = "Layer 3" };
        
        generator.AddLayer(layer1);
        generator.AddLayer(layer2);
        generator.AddLayer(layer3);

        // Act
        generator.RemoveLayerAt(1); // Remove middle layer

        // Assert
        Assert.Equal(2, generator.Layers.Count);
        Assert.Equal("Layer 1", generator.Layers[0].Name);
        Assert.Equal("Layer 3", generator.Layers[1].Name);
    }

    [Fact]
    public void RemoveLayerAt_InvalidIndex_DoesNothing()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer { Name = "Layer 1" });

        // Act
        generator.RemoveLayerAt(5);

        // Assert
        Assert.Single(generator.Layers);
    }

    [Fact]
    public void ClearLayers_RemovesAllLayers()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer { Name = "Layer 1" });
        generator.AddLayer(new NoiseLayer { Name = "Layer 2" });

        // Act
        generator.ClearLayers();

        // Assert
        Assert.Empty(generator.Layers);
    }

    [Fact]
    public void Generate_NoLayers_ReturnsZero()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();

        // Act
        var result = generator.Generate(0.5f, 0.5f);

        // Assert
        Assert.Equal(0f, result);
    }

    [Fact]
    public void Generate_SingleReplaceLayer_ReturnsNoiseValue()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator(12345);
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(0.5f, 0.5f);

        // Assert
        Assert.NotEqual(0f, result); // Should produce some noise
        Assert.InRange(result, -1.5f, 1.5f); // Reasonable range for Perlin noise
    }

    [Fact]
    public void Generate_ConstantNoiseType_ReturnsConstantValue()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 0.75f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(1.0f, 2.0f);

        // Assert
        Assert.Equal(0.75f, result);
    }

    [Fact]
    public void Generate_AddOperation_SumsValues()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 0.5f,
            Enabled = true
        });
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Add,
            ConstantValue = 0.3f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(0f, 0f);

        // Assert
        Assert.Equal(0.8f, result, precision: 5);
    }

    [Fact]
    public void Generate_SubtractOperation_SubtractsValues()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 1.0f,
            Enabled = true
        });
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Subtract,
            ConstantValue = 0.3f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(0f, 0f);

        // Assert
        Assert.Equal(0.7f, result, precision: 5);
    }

    [Fact]
    public void Generate_MultiplyOperation_MultipliesValues()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 0.5f,
            Enabled = true
        });
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Multiply,
            ConstantValue = 2.0f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(0f, 0f);

        // Assert
        Assert.Equal(1.0f, result, precision: 5);
    }

    [Fact]
    public void Generate_DisabledLayer_IsSkipped()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 0.5f,
            Enabled = true
        });
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Add,
            ConstantValue = 1.0f,
            Enabled = false // Disabled
        });

        // Act
        var result = generator.Generate(0f, 0f);

        // Assert
        Assert.Equal(0.5f, result); // Only first layer applied
    }

    [Fact]
    public void Generate_MultipleOperations_AppliesInOrder()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        
        // Base value: 10
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 10.0f,
            Enabled = true
        });
        
        // Multiply by 2: 10 * 2 = 20
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Multiply,
            ConstantValue = 2.0f,
            Enabled = true
        });
        
        // Add 5: 20 + 5 = 25
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Add,
            ConstantValue = 5.0f,
            Enabled = true
        });
        
        // Subtract 10: 25 - 10 = 15
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Subtract,
            ConstantValue = 10.0f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(0f, 0f);

        // Assert
        Assert.Equal(15.0f, result, precision: 5);
    }

    [Fact]
    public void Generate_SameSeedSameCoordinates_ReturnsSameValue()
    {
        // Arrange
        var generator1 = new LayeredNoiseGenerator(999);
        var generator2 = new LayeredNoiseGenerator(999);
        
        var layer = new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Enabled = true
        };
        
        generator1.AddLayer(layer);
        generator2.AddLayer(new NoiseLayer
        {
            NoiseType = layer.NoiseType,
            Operation = layer.Operation,
            Scale = layer.Scale,
            Amplitude = layer.Amplitude,
            Enabled = layer.Enabled
        });

        // Act
        var result1 = generator1.Generate(5.5f, 7.3f);
        var result2 = generator2.Generate(5.5f, 7.3f);

        // Assert
        Assert.Equal(result1, result2, precision: 5);
    }

    [Fact]
    public void Generate_DifferentSeeds_ReturnsDifferentValues()
    {
        // Arrange
        var generator1 = new LayeredNoiseGenerator(100);
        var generator2 = new LayeredNoiseGenerator(200);
        
        var layer = new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Enabled = true
        };
        
        generator1.AddLayer(layer);
        generator2.AddLayer(new NoiseLayer
        {
            NoiseType = layer.NoiseType,
            Operation = layer.Operation,
            Scale = layer.Scale,
            Amplitude = layer.Amplitude,
            Enabled = layer.Enabled
        });

        // Act
        var result1 = generator1.Generate(5.5f, 7.3f);
        var result2 = generator2.Generate(5.5f, 7.3f);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void Generate_ValueNoise_ProducesOutput()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator(42);
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Octaves = 3,
            Enabled = true
        });

        // Act
        var result = generator.Generate(10.0f, 20.0f);

        // Assert
        Assert.NotEqual(0f, result);
        Assert.InRange(result, -2.0f, 2.0f); // Value noise range
    }

    [Fact]
    public void Generate_AllLayersDisabled_ReturnsZero()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator();
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 5.0f,
            Enabled = false
        });
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Add,
            ConstantValue = 3.0f,
            Enabled = false
        });

        // Act
        var result = generator.Generate(0f, 0f);

        // Assert
        Assert.Equal(0f, result);
    }

    [Fact]
    public void Seed_CanBeUpdated()
    {
        // Arrange
        var generator = new LayeredNoiseGenerator(100);
        generator.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Enabled = true
        });

        var result1 = generator.Generate(1.0f, 1.0f);

        // Act
        generator.Seed = 200;
        var result2 = generator.Generate(1.0f, 1.0f);

        // Assert
        Assert.NotEqual(result1, result2);
    }

    [Fact]
    public void Generate_ComplexScenario_CombinesCorrectly()
    {
        // Arrange - Simulate a realistic terrain generation scenario
        var generator = new LayeredNoiseGenerator(777);
        
        // Base terrain (large features)
        generator.AddLayer(new NoiseLayer
        {
            Name = "Base Terrain",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.01f, // Large scale features
            Amplitude = 1.0f,
            Octaves = 4,
            Enabled = true
        });
        
        // Add small details
        generator.AddLayer(new NoiseLayer
        {
            Name = "Detail",
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.1f, // Small scale features
            Amplitude = 0.2f,
            Octaves = 2,
            Enabled = true
        });
        
        // Multiply for ridges
        generator.AddLayer(new NoiseLayer
        {
            Name = "Ridges",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Multiply,
            Scale = 0.05f,
            Amplitude = 1.2f,
            Enabled = true
        });

        // Act
        var result = generator.Generate(50.0f, 50.0f);

        // Assert - Just verify it produces reasonable output
        Assert.InRange(result, -5.0f, 5.0f);
    }
}
