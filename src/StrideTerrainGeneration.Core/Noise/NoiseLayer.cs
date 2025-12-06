namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Represents a single layer in a multi-layer noise configuration.
/// </summary>
public class NoiseLayer
{
    /// <summary>
    /// Gets or sets the type of noise for this layer.
    /// </summary>
    public NoiseType NoiseType { get; set; } = NoiseType.Perlin;

    /// <summary>
    /// Gets or sets the operation to combine this layer with previous layers.
    /// </summary>
    public NoiseLayerOperation Operation { get; set; } = NoiseLayerOperation.Add;

    /// <summary>
    /// Gets or sets the scale/frequency of the noise.
    /// </summary>
    public float Scale { get; set; } = 0.05f;

    /// <summary>
    /// Gets or sets the amplitude/strength of the noise layer.
    /// </summary>
    public float Amplitude { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the number of octaves (for fractal noise).
    /// </summary>
    public int Octaves { get; set; } = 4;

    /// <summary>
    /// Gets or sets the persistence (amplitude multiplier per octave).
    /// </summary>
    public float Persistence { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the lacunarity (frequency multiplier per octave).
    /// </summary>
    public float Lacunarity { get; set; } = 2.0f;

    /// <summary>
    /// Gets or sets the constant value (used when NoiseType is Constant).
    /// </summary>
    public float ConstantValue { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets whether this layer is enabled.
    /// </summary>
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Gets or sets a descriptive name for this layer.
    /// </summary>
    public string Name { get; set; } = "Layer";
}
