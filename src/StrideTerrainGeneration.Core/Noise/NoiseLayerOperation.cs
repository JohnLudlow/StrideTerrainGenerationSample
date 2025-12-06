namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Defines how a noise layer is combined with the previous layer's result.
/// </summary>
public enum NoiseLayerOperation
{
    /// <summary>
    /// Adds the noise value to the current value.
    /// </summary>
    Add,

    /// <summary>
    /// Subtracts the noise value from the current value.
    /// </summary>
    Subtract,

    /// <summary>
    /// Multiplies the current value by the noise value.
    /// </summary>
    Multiply,

    /// <summary>
    /// Sets the value directly (replaces the current value).
    /// </summary>
    Replace
}
