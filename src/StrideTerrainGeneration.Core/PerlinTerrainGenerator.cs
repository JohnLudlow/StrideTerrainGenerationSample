using StrideTerrainGeneration.Core.Configuration;
using StrideTerrainGeneration.Core.Noise;

namespace StrideTerrainGeneration.Core;

/// <summary>
/// Generates terrain using Perlin noise for realistic height variations.
/// </summary>
public class PerlinTerrainGenerator : ITerrainGenerator
{
    private readonly PerlinNoiseGenerator _noiseGenerator;

    /// <summary>
    /// Gets or sets the scale of the noise pattern.
    /// Larger values create smoother, larger features.
    /// </summary>
    public float NoiseScale { get; set; } = 0.05f;

    /// <summary>
    /// Gets or sets the minimum height multiplier.
    /// </summary>
    public float MinHeight { get; set; } = 0.0f;

    /// <summary>
    /// Gets or sets the maximum height multiplier.
    /// </summary>
    public float MaxHeight { get; set; } = 1.0f;

    public PerlinTerrainGenerator()
    {
        _noiseGenerator = new PerlinNoiseGenerator
        {
            Octaves = 4,
            Persistence = 0.5f,
            Lacunarity = 2.0f
        };
    }

    public TerrainData Generate(TerrainConfig config)
    {
        if (config.Width <= 0 || config.Height <= 0)
        {
            throw new ArgumentException("Terrain dimensions must be positive.");
        }

        _noiseGenerator.Seed = config.Seed;
        _noiseGenerator.Frequency = NoiseScale;

        var terrainData = new TerrainData(config.Width, config.Height);

        for (var y = 0; y < config.Height; y++)
        {
            for (var x = 0; x < config.Width; x++)
            {
                var noiseValue = _noiseGenerator.Generate(x, y);
                var height = MinHeight + noiseValue * (MaxHeight - MinHeight);
                terrainData.SetHeight(x, y, height);
            }
        }

        return terrainData;
    }

    public Task<TerrainData> GenerateAsync(TerrainConfig config, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Generate(config), cancellationToken);
    }
}
