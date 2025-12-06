namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Perlin noise generator implementation using improved Perlin noise algorithm.
/// </summary>
public class PerlinNoiseGenerator : INoiseGenerator
{
    private readonly int[] _permutation;
    private int _seed;

    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            InitializePermutation();
        }
    }

    /// <summary>
    /// Gets or sets the frequency of the noise pattern.
    /// Higher values create more frequent variations.
    /// </summary>
    public float Frequency { get; set; } = 1.0f;

    /// <summary>
    /// Gets or sets the number of octaves for fractal noise.
    /// More octaves add finer detail but are slower.
    /// </summary>
    public int Octaves { get; set; } = 4;

    /// <summary>
    /// Gets or sets the persistence (amplitude multiplier per octave).
    /// Typical values: 0.5 for balanced detail.
    /// </summary>
    public float Persistence { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the lacunarity (frequency multiplier per octave).
    /// Typical values: 2.0 for standard fractal behavior.
    /// </summary>
    public float Lacunarity { get; set; } = 2.0f;

    public PerlinNoiseGenerator(int seed = 0)
    {
        _permutation = new int[512];
        _seed = seed;
        InitializePermutation();
    }

    private void InitializePermutation()
    {
        var random = new Random(_seed);
        var p = new int[256];
        
        for (var i = 0; i < 256; i++)
        {
            p[i] = i;
        }

        // Fisher-Yates shuffle
        for (var i = 255; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (p[i], p[j]) = (p[j], p[i]);
        }

        // Duplicate for wrapping
        for (var i = 0; i < 512; i++)
        {
            _permutation[i] = p[i & 255];
        }
    }

    public float Generate(float x, float y)
    {
        var total = 0f;
        var amplitude = 1f;
        var frequency = Frequency;
        var maxValue = 0f;

        for (var octave = 0; octave < Octaves; octave++)
        {
            total += GenerateOctave(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= Persistence;
            frequency *= Lacunarity;
        }

        // Normalize to [0, 1]
        return total / maxValue;
    }

    private float GenerateOctave(float x, float y)
    {
        // Find unit grid cell containing point
        var xi = (int)Math.Floor(x) & 255;
        var yi = (int)Math.Floor(y) & 255;

        // Get relative xy coordinates within cell
        var xf = x - (float)Math.Floor(x);
        var yf = y - (float)Math.Floor(y);

        // Compute fade curves
        var u = Fade(xf);
        var v = Fade(yf);

        // Hash coordinates of the 4 square corners
        var aa = _permutation[_permutation[xi] + yi];
        var ab = _permutation[_permutation[xi] + yi + 1];
        var ba = _permutation[_permutation[xi + 1] + yi];
        var bb = _permutation[_permutation[xi + 1] + yi + 1];

        // Blend results from the 4 corners
        var x1 = Lerp(Grad(aa, xf, yf), Grad(ba, xf - 1, yf), u);
        var x2 = Lerp(Grad(ab, xf, yf - 1), Grad(bb, xf - 1, yf - 1), u);

        return (Lerp(x1, x2, v) + 1) * 0.5f; // Map from [-1, 1] to [0, 1]
    }

    private static float Fade(float t)
    {
        // 6t^5 - 15t^4 + 10t^3
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private static float Grad(int hash, float x, float y)
    {
        // Convert low 4 bits of hash into 8 gradient directions
        var h = hash & 7;
        var u = h < 4 ? x : y;
        var v = h < 4 ? y : x;
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }
}
