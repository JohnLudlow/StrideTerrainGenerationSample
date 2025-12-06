namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Generates value noise (simpler than Perlin, more blocky).
/// </summary>
public class ValueNoiseGenerator : INoiseGenerator
{
    private int[] _permutation;
    private const int PermutationSize = 256;
    private int _seed;

    /// <summary>
    /// Gets or sets the seed for reproducible noise generation.
    /// </summary>
    public int Seed
    {
        get => _seed;
        set
        {
            _seed = value;
            _permutation = GeneratePermutation(_seed);
        }
    }

    /// <summary>
    /// Gets or sets the number of octaves for fractal noise.
    /// </summary>
    public int Octaves { get; set; } = 1;

    /// <summary>
    /// Gets or sets the persistence (amplitude decay per octave).
    /// </summary>
    public float Persistence { get; set; } = 0.5f;

    /// <summary>
    /// Gets or sets the lacunarity (frequency increase per octave).
    /// </summary>
    public float Lacunarity { get; set; } = 2.0f;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValueNoiseGenerator"/> class.
    /// </summary>
    /// <param name="seed">The seed for random number generation.</param>
    public ValueNoiseGenerator(int seed = 0)
    {
        _seed = seed;
        _permutation = GeneratePermutation(seed);
    }

    /// <summary>
    /// Generates noise value at the specified coordinates.
    /// </summary>
    public float Generate(float x, float y)
    {
        float total = 0f;
        float frequency = 1f;
        float amplitude = 1f;
        float maxValue = 0f;

        for (int i = 0; i < Octaves; i++)
        {
            total += ValueNoise(x * frequency, y * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= Persistence;
            frequency *= Lacunarity;
        }

        return total / maxValue;
    }

    private float ValueNoise(float x, float y)
    {
        int xi = (int)Math.Floor(x) & 255;
        int yi = (int)Math.Floor(y) & 255;

        float xf = x - (float)Math.Floor(x);
        float yf = y - (float)Math.Floor(y);

        // Interpolation curves
        float u = Fade(xf);
        float v = Fade(yf);

        // Get values at corners
        int a = _permutation[xi] + yi;
        int b = _permutation[(xi + 1) & 255] + yi;

        float val00 = Hash(a);
        float val10 = Hash(b);
        float val01 = Hash(a + 1);
        float val11 = Hash(b + 1);

        // Interpolate
        float x1 = Lerp(val00, val10, u);
        float x2 = Lerp(val01, val11, u);

        return Lerp(x1, x2, v);
    }

    private float Hash(int value)
    {
        return _permutation[value & 255] / 255f;
    }

    private static float Fade(float t)
    {
        return t * t * t * (t * (t * 6 - 15) + 10);
    }

    private static float Lerp(float a, float b, float t)
    {
        return a + t * (b - a);
    }

    private static int[] GeneratePermutation(int seed)
    {
        var random = new Random(seed);
        var perm = new int[PermutationSize];
        
        for (int i = 0; i < PermutationSize; i++)
        {
            perm[i] = i;
        }

        // Fisher-Yates shuffle
        for (int i = PermutationSize - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (perm[i], perm[j]) = (perm[j], perm[i]);
        }

        return perm;
    }
}
