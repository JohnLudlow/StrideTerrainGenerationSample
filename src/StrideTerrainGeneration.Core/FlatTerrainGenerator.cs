using StrideTerrainGeneration.Core.Configuration;

namespace StrideTerrainGeneration.Core;

/// <summary>
/// Generates flat terrain at a constant height level.
/// </summary>
public class FlatTerrainGenerator : ITerrainGenerator
{
    /// <summary>
    /// Generates flat terrain data based on the provided configuration.
    /// </summary>
    /// <param name="config">The terrain configuration.</param>
    /// <returns>The generated flat terrain data.</returns>
    public TerrainData Generate(TerrainConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var terrain = new TerrainData(config.Width, config.Height);
        var baseHeight = Math.Clamp(config.BaseHeight, 0.0f, 1.0f);

        for (var x = 0; x < config.Width; x++)
        {
            for (var y = 0; y < config.Height; y++)
            {
                terrain.SetHeight(x, y, baseHeight);
            }
        }

        return terrain;
    }

    /// <summary>
    /// Asynchronously generates flat terrain data based on the provided configuration.
    /// </summary>
    /// <param name="config">The terrain configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the generated terrain data.</returns>
    public Task<TerrainData> GenerateAsync(TerrainConfig config, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Generate(config);
        }, cancellationToken);
    }
}
