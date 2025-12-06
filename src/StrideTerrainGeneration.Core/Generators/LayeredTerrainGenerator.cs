using StrideTerrainGeneration.Core.Configuration;
using StrideTerrainGeneration.Core.Noise;

namespace StrideTerrainGeneration.Core.Generators;

/// <summary>
/// Generates terrain using multiple layered noise sources.
/// </summary>
public class LayeredTerrainGenerator : ITerrainGenerator
{
    private readonly LayeredNoiseGenerator _noiseGenerator;

    /// <summary>
    /// Gets or sets the minimum height value.
    /// </summary>
    public float MinHeight { get; set; }

    /// <summary>
    /// Gets or sets the maximum height value.
    /// </summary>
    public float MaxHeight { get; set; } = 1.0f;

    /// <summary>
    /// Gets the layered noise generator.
    /// </summary>
    public LayeredNoiseGenerator NoiseGenerator => _noiseGenerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredTerrainGenerator"/> class.
    /// </summary>
    public LayeredTerrainGenerator()
    {
        _noiseGenerator = new LayeredNoiseGenerator();
    }

    /// <summary>
    /// Generates terrain data based on the configuration.
    /// </summary>
    public TerrainData Generate(TerrainConfig config)
    {
        _noiseGenerator.ClearLayers();
        
        // Re-initialize with config seed
        var generator = new LayeredNoiseGenerator(config.Seed);
        
        // Copy layers (would be configured externally)
        foreach (var layer in _noiseGenerator.Layers)
        {
            generator.AddLayer(layer);
        }

        var terrainData = new TerrainData(config.Width, config.Height);

        for (int y = 0; y < config.Height; y++)
        {
            for (int x = 0; x < config.Width; x++)
            {
                float noiseValue = generator.Generate(x, y);
                terrainData.SetHeight(x, y, MinHeight + noiseValue * (MaxHeight - MinHeight));
            }
        }

        return terrainData;
    }

    /// <summary>
    /// Generates terrain data asynchronously.
    /// </summary>
    public async Task<TerrainData> GenerateAsync(TerrainConfig config, CancellationToken cancellationToken = default)
    {
        return await Task.Run(() => Generate(config), cancellationToken);
    }
}
