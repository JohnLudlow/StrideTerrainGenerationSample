using BenchmarkDotNet.Attributes;
using StrideTerrainGeneration.Core;
using StrideTerrainGeneration.Core.Generators;
using StrideTerrainGeneration.Core.Configuration;
using StrideTerrainGeneration.Core.Noise;

namespace StrideTerrainGeneration.Benchmarks;

[MemoryDiagnoser]
public class LayeredTerrainGeneratorBenchmarks
{
    private LayeredTerrainGenerator? _singleLayerGenerator;
    private LayeredTerrainGenerator? _twoLayerGenerator;
    private LayeredTerrainGenerator? _complexGenerator;
    private LayeredTerrainGenerator? _manyLayersGenerator;

    [GlobalSetup]
    public void Setup()
    {
        // Single layer (baseline)
        _singleLayerGenerator = new LayeredTerrainGenerator();
        _singleLayerGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Base",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.05f,
            Amplitude = 1.0f,
            Octaves = 4,
            Enabled = true
        });

        // Two layers (common case)
        _twoLayerGenerator = new LayeredTerrainGenerator();
        _twoLayerGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Base",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.05f,
            Amplitude = 1.0f,
            Octaves = 4,
            Enabled = true
        });
        _twoLayerGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Detail",
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.1f,
            Amplitude = 0.3f,
            Octaves = 2,
            Enabled = true
        });

        // Complex scenario (realistic terrain)
        _complexGenerator = new LayeredTerrainGenerator();
        _complexGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Base Terrain",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Replace,
            Scale = 0.01f,
            Amplitude = 1.0f,
            Octaves = 6,
            Persistence = 0.5f,
            Lacunarity = 2.0f,
            Enabled = true
        });
        _complexGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Mountains",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.05f,
            Amplitude = 0.8f,
            Octaves = 4,
            Enabled = true
        });
        _complexGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Hills",
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.08f,
            Amplitude = 0.4f,
            Octaves = 3,
            Enabled = true
        });
        _complexGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Ridges",
            NoiseType = NoiseType.Perlin,
            Operation = NoiseLayerOperation.Multiply,
            Scale = 0.03f,
            Amplitude = 1.2f,
            Octaves = 2,
            Enabled = true
        });
        _complexGenerator.NoiseGenerator.AddLayer(new NoiseLayer
        {
            Name = "Fine Detail",
            NoiseType = NoiseType.Value,
            Operation = NoiseLayerOperation.Add,
            Scale = 0.2f,
            Amplitude = 0.1f,
            Octaves = 1,
            Enabled = true
        });

        // Many layers (stress test)
        _manyLayersGenerator = new LayeredTerrainGenerator();
        for (int i = 0; i < 10; i++)
        {
            _manyLayersGenerator.NoiseGenerator.AddLayer(new NoiseLayer
            {
                Name = $"Layer {i}",
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
    }

    [Benchmark(Baseline = true)]
    public TerrainData SingleLayer_64x64()
    {
        return _singleLayerGenerator!.Generate(new TerrainConfig { Width = 64, Height = 64 });
    }

    [Benchmark]
    public TerrainData SingleLayer_128x128()
    {
        return _singleLayerGenerator!.Generate(new TerrainConfig { Width = 128, Height = 128 });
    }

    [Benchmark]
    public TerrainData SingleLayer_256x256()
    {
        return _singleLayerGenerator!.Generate(new TerrainConfig { Width = 256, Height = 256 });
    }

    [Benchmark]
    public TerrainData SingleLayer_512x512()
    {
        return _singleLayerGenerator!.Generate(new TerrainConfig { Width = 512, Height = 512 });
    }

    [Benchmark]
    public TerrainData TwoLayers_64x64()
    {
        return _twoLayerGenerator!.Generate(new TerrainConfig { Width = 64, Height = 64 });
    }

    [Benchmark]
    public TerrainData TwoLayers_128x128()
    {
        return _twoLayerGenerator!.Generate(new TerrainConfig { Width = 128, Height = 128 });
    }

    [Benchmark]
    public TerrainData TwoLayers_256x256()
    {
        return _twoLayerGenerator!.Generate(new TerrainConfig { Width = 256, Height = 256 });
    }

    [Benchmark]
    public TerrainData TwoLayers_512x512()
    {
        return _twoLayerGenerator!.Generate(new TerrainConfig { Width = 512, Height = 512 });
    }

    [Benchmark]
    public TerrainData ComplexLayers_64x64()
    {
        return _complexGenerator!.Generate(new TerrainConfig { Width = 64, Height = 64 });
    }

    [Benchmark]
    public TerrainData ComplexLayers_128x128()
    {
        return _complexGenerator!.Generate(new TerrainConfig { Width = 128, Height = 128 });
    }

    [Benchmark]
    public TerrainData ComplexLayers_256x256()
    {
        return _complexGenerator!.Generate(new TerrainConfig { Width = 256, Height = 256 });
    }

    [Benchmark]
    public TerrainData ComplexLayers_512x512()
    {
        return _complexGenerator!.Generate(new TerrainConfig { Width = 512, Height = 512 });
    }

    [Benchmark]
    public TerrainData ManyLayers_64x64()
    {
        return _manyLayersGenerator!.Generate(new TerrainConfig { Width = 64, Height = 64 });
    }

    [Benchmark]
    public TerrainData ManyLayers_128x128()
    {
        return _manyLayersGenerator!.Generate(new TerrainConfig { Width = 128, Height = 128 });
    }

    [Benchmark]
    public TerrainData ManyLayers_256x256()
    {
        return _manyLayersGenerator!.Generate(new TerrainConfig { Width = 256, Height = 256 });
    }

    [Benchmark]
    public async Task<TerrainData> SingleLayerAsync_256x256()
    {
        return await _singleLayerGenerator!.GenerateAsync(new TerrainConfig { Width = 256, Height = 256 });
    }

    [Benchmark]
    public async Task<TerrainData> ComplexLayersAsync_256x256()
    {
        return await _complexGenerator!.GenerateAsync(new TerrainConfig { Width = 256, Height = 256 });
    }
}
