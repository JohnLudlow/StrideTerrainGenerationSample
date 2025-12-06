using BenchmarkDotNet.Attributes;
using StrideTerrainGeneration.Core.Noise;

namespace StrideTerrainGeneration.Benchmarks;

/// <summary>
/// Benchmarks for LayeredNoiseGenerator to measure performance impact of different layer configurations.
/// </summary>
[MemoryDiagnoser]
public class LayeredNoiseGeneratorBenchmarks
{
    private LayeredNoiseGenerator? _singleLayer;
    private LayeredNoiseGenerator? _twoLayersAdd;
    private LayeredNoiseGenerator? _fourLayersMixed;
    private LayeredNoiseGenerator? _tenLayers;
    private LayeredNoiseGenerator? _perlinOnly;
    private LayeredNoiseGenerator? _valueOnly;
    private LayeredNoiseGenerator? _constantOnly;

    private const int SamplePoints = 1000;

    [GlobalSetup]
    public void Setup()
    {
        // Single layer baseline
        _singleLayer = new LayeredNoiseGenerator(42);
        _singleLayer.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Octaves = 4,
            Enabled = true
        });

        // Two layers with Add operation
        _twoLayersAdd = new LayeredNoiseGenerator(42);
        _twoLayersAdd.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.05f,
            Amplitude = 1.0f,
            Octaves = 4,
            Enabled = true
        });
        _twoLayersAdd.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.1f,
            Amplitude = 0.5f,
            Octaves = 2,
            Enabled = true
        });

        // Four layers with mixed operations
        _fourLayersMixed = new LayeredNoiseGenerator(42);
        _fourLayersMixed.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.03f,
            Amplitude = 1.0f,
            Octaves = 5,
            Enabled = true
        });
        _fourLayersMixed.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.08f,
            Amplitude = 0.4f,
            Octaves = 3,
            Enabled = true
        });
        _fourLayersMixed.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Multiply,
            Scale = 0.05f,
            Amplitude = 1.1f,
            Octaves = 2,
            Enabled = true
        });
        _fourLayersMixed.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Subtract,
            Scale = 0.15f,
            Amplitude = 0.2f,
            Octaves = 1,
            Enabled = true
        });

        // Ten layers (stress test)
        _tenLayers = new LayeredNoiseGenerator(42);
        for (int i = 0; i < 10; i++)
        {
            _tenLayers.AddLayer(new NoiseLayer
            {
                NoiseType = i % 2 == 0 ? NoiseType.Perlin : NoiseType.Value,
                Operation = i == 0 ? NoiseLayerOperation.Replace :
                           i % 3 == 0 ? NoiseLayerOperation.Add :
                           i % 3 == 1 ? NoiseLayerOperation.Multiply : NoiseLayerOperation.Subtract,
                Scale = 0.05f * (i + 1),
                Amplitude = 0.5f / (i + 1),
                Octaves = 2,
                Enabled = true
            });
        }

        // Perlin noise only (high octaves)
        _perlinOnly = new LayeredNoiseGenerator(42);
        _perlinOnly.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Octaves = 8,
            Persistence = 0.5f,
            Lacunarity = 2.0f,
            Enabled = true
        });

        // Value noise only (high octaves)
        _valueOnly = new LayeredNoiseGenerator(42);
        _valueOnly.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.1f,
            Amplitude = 1.0f,
            Octaves = 8,
            Persistence = 0.5f,
            Lacunarity = 2.0f,
            Enabled = true
        });

        // Constant values (minimal overhead)
        _constantOnly = new LayeredNoiseGenerator(42);
        _constantOnly.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Replace,
            ConstantValue = 0.5f,
            Enabled = true
        });
        _constantOnly.AddLayer(new NoiseLayer
        {
            NoiseType = NoiseType.Constant,
            Operation = NoiseLayerOperation.Add,
            ConstantValue = 0.3f,
            Enabled = true
        });
    }

    [Benchmark(Baseline = true)]
    public float SingleLayer_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _singleLayer!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float TwoLayersAdd_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _twoLayersAdd!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float FourLayersMixed_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _fourLayersMixed!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float TenLayers_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _tenLayers!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float PerlinOnly_HighOctaves_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _perlinOnly!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float ValueOnly_HighOctaves_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _valueOnly!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float ConstantOnly_1000Samples()
    {
        float sum = 0f;
        for (int i = 0; i < SamplePoints; i++)
        {
            sum += _constantOnly!.Generate(i * 0.1f, i * 0.1f);
        }
        return sum;
    }

    [Benchmark]
    public float SinglePoint_SingleLayer()
    {
        return _singleLayer!.Generate(5.5f, 7.3f);
    }

    [Benchmark]
    public float SinglePoint_FourLayersMixed()
    {
        return _fourLayersMixed!.Generate(5.5f, 7.3f);
    }

    [Benchmark]
    public float SinglePoint_TenLayers()
    {
        return _tenLayers!.Generate(5.5f, 7.3f);
    }
}
