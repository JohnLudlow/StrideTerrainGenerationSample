namespace StrideTerrainGeneration.Core;

/// <summary>
/// Represents generated terrain data with heightmap information.
/// </summary>
public class TerrainData
{
    /// <summary>
    /// Gets the width of the terrain in vertices.
    /// </summary>
    public int Width { get; }

    /// <summary>
    /// Gets the height of the terrain in vertices.
    /// </summary>
    public int Height { get; }

    /// <summary>
    /// Gets the heightmap data. Values range from 0.0 to 1.0.
    /// </summary>
    public float[,] Heightmap { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TerrainData"/> class.
    /// </summary>
    /// <param name="width">The width in vertices.</param>
    /// <param name="height">The height in vertices.</param>
    public TerrainData(int width, int height)
    {
        if (width <= 0)
            throw new ArgumentOutOfRangeException(nameof(width), "Width must be greater than zero.");
        if (height <= 0)
            throw new ArgumentOutOfRangeException(nameof(height), "Height must be greater than zero.");

        Width = width;
        Height = height;
        Heightmap = new float[width, height];
    }

    /// <summary>
    /// Gets the height value at the specified coordinates.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <returns>The height value at the specified position.</returns>
    public float GetHeight(int x, int y)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x));
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException(nameof(y));

        return Heightmap[x, y];
    }

    /// <summary>
    /// Sets the height value at the specified coordinates.
    /// </summary>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="value">The height value (0.0 to 1.0).</param>
    public void SetHeight(int x, int y, float value)
    {
        if (x < 0 || x >= Width)
            throw new ArgumentOutOfRangeException(nameof(x));
        if (y < 0 || y >= Height)
            throw new ArgumentOutOfRangeException(nameof(y));

        Heightmap[x, y] = Math.Clamp(value, 0.0f, 1.0f);
    }
}
