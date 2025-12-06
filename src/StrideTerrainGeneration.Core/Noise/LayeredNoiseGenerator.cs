namespace StrideTerrainGeneration.Core.Noise;

/// <summary>
/// Combines multiple noise layers with different operations.
/// </summary>
public class LayeredNoiseGenerator : INoiseGenerator
{
    private readonly List<NoiseLayer> _layers;
    private int _seed;

    /// <summary>
    /// Gets or sets the seed for reproducible noise generation.
    /// </summary>
    public int Seed
    {
        get => _seed;
        set => _seed = value;
    }

    /// <summary>
    /// Gets the list of noise layers.
    /// </summary>
    public IReadOnlyList<NoiseLayer> Layers => _layers.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="LayeredNoiseGenerator"/> class.
    /// </summary>
    /// <param name="seed">The seed for random number generation.</param>
    public LayeredNoiseGenerator(int seed = 0)
    {
        _seed = seed;
        _layers = new List<NoiseLayer>();
    }

    /// <summary>
    /// Adds a noise layer to the generator.
    /// </summary>
    public void AddLayer(NoiseLayer layer)
    {
        _layers.Add(layer);
    }

    /// <summary>
    /// Removes a noise layer at the specified index.
    /// </summary>
    public void RemoveLayerAt(int index)
    {
        if (index >= 0 && index < _layers.Count)
        {
            _layers.RemoveAt(index);
        }
    }

    /// <summary>
    /// Clears all layers.
    /// </summary>
    public void ClearLayers()
    {
        _layers.Clear();
    }

    /// <summary>
    /// Generates noise value at the specified coordinates by combining all layers.
    /// </summary>
    public float Generate(float x, float y)
    {
        if (_layers.Count == 0)
            return 0f;

        float result = 0f;
        bool firstLayer = true;

        for (int i = 0; i < _layers.Count; i++)
        {
            var layer = _layers[i];
            if (!layer.Enabled)
                continue;

            float noiseValue = GenerateLayerNoise(layer, x, y, i);

            if (firstLayer)
            {
                result = noiseValue;
                firstLayer = false;
            }
            else
            {
                result = ApplyOperation(result, noiseValue, layer.Operation);
            }
        }

        return result;
    }

    private float GenerateLayerNoise(NoiseLayer layer, float x, float y, int layerIndex)
    {
        float noiseValue;

        switch (layer.NoiseType)
        {
            case NoiseType.Perlin:
                var perlin = new PerlinNoiseGenerator(_seed + layerIndex)
                {
                    Octaves = layer.Octaves,
                    Persistence = layer.Persistence,
                    Lacunarity = layer.Lacunarity
                };
                noiseValue = perlin.Generate(x * layer.Scale, y * layer.Scale);
                break;

            case NoiseType.Value:
                var value = new ValueNoiseGenerator(_seed + layerIndex)
                {
                    Octaves = layer.Octaves,
                    Persistence = layer.Persistence,
                    Lacunarity = layer.Lacunarity
                };
                noiseValue = value.Generate(x * layer.Scale, y * layer.Scale);
                break;

            case NoiseType.Constant:
                noiseValue = layer.ConstantValue;
                break;

            default:
                noiseValue = 0f;
                break;
        }

        return noiseValue * layer.Amplitude;
    }

    private static float ApplyOperation(float current, float layerValue, NoiseLayerOperation operation)
    {
        return operation switch
        {
            NoiseLayerOperation.Add => current + layerValue,
            NoiseLayerOperation.Subtract => current - layerValue,
            NoiseLayerOperation.Multiply => current * layerValue,
            NoiseLayerOperation.Replace => layerValue,
            _ => current
        };
    }
}
