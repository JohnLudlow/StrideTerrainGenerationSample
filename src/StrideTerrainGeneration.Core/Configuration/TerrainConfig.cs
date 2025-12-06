namespace StrideTerrainGeneration.Core.Configuration;

/// <summary>
/// Configuration settings for terrain generation.
/// </summary>
public class TerrainConfig
{
    /// <summary>
    /// Gets or sets the width of the terrain in vertices.
    /// </summary>
    public int Width { get; set; } = 256;

    /// <summary>
    /// Gets or sets the height of the terrain in vertices.
    /// </summary>
    public int Height { get; set; } = 256;

    /// <summary>
    /// Gets or sets the seed for reproducible generation.
    /// </summary>
    public int Seed { get; set; } = 0;

    /// <summary>
    /// Gets or sets the base height level for flat terrain (0.0 to 1.0).
    /// </summary>
    public float BaseHeight { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the scale of the terrain in world units.
    /// </summary>
    public float Scale { get; set; } = 1.0f;
}
