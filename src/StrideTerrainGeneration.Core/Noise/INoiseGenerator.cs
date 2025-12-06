namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Interface for noise generation algorithms.
/// </summary>
public interface INoiseGenerator
{
    /// <summary>
    /// Generates noise value at the specified 2D coordinates.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>Noise value typically in range [0, 1].</returns>
    float Generate(float x, float y);

    /// <summary>
    /// Gets or sets the seed for reproducible noise generation.
    /// </summary>
    int Seed { get; set; }
}
