using StrideTerrainGeneration.Core.Configuration;

namespace StrideTerrainGeneration.Core;

/// <summary>
/// Interface for terrain generation implementations.
/// </summary>
public interface ITerrainGenerator
{
    /// <summary>
    /// Generates terrain data based on the provided configuration.
    /// </summary>
    /// <param name="config">The terrain configuration.</param>
    /// <returns>The generated terrain data.</returns>
    TerrainData Generate(TerrainConfig config);

    /// <summary>
    /// Asynchronously generates terrain data based on the provided configuration.
    /// </summary>
    /// <param name="config">The terrain configuration.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the generated terrain data.</returns>
    Task<TerrainData> GenerateAsync(TerrainConfig config, CancellationToken cancellationToken = default);
}
