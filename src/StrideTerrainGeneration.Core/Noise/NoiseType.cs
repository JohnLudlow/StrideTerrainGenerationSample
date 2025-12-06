namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Types of noise generators available.
/// </summary>
public enum NoiseType
{
    /// <summary>
    /// Perlin noise - smooth, natural-looking terrain.
    /// </summary>
    Perlin,

    /// <summary>
    /// Value noise - simpler, blocky patterns.
    /// </summary>
    Value,

    /// <summary>
    /// Constant value - useful for base layers or masks.
    /// </summary>
    Constant
}
