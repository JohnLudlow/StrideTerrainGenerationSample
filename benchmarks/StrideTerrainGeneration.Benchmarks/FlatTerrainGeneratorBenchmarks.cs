using BenchmarkDotNet.Attributes;
using StrideTerrainGeneration.Core;
using StrideTerrainGeneration.Core.Configuration;

namespace StrideTerrainGeneration.Benchmarks;

/// <summary>
/// Benchmarks for flat terrain generation performance.
/// </summary>
[MemoryDiagnoser]
[MinColumn, MaxColumn, MeanColumn, MedianColumn]
public class FlatTerrainGeneratorBenchmarks
{
    private FlatTerrainGenerator? _generator;

    [GlobalSetup]
    public void Setup()
    {
        _generator = new FlatTerrainGenerator();
    }

    [Benchmark]
    public TerrainData Generate_64x64()
    {
        var config = new TerrainConfig { Width = 64, Height = 64, BaseHeight = 0.5f };
        return _generator!.Generate(config);
    }

    [Benchmark]
    public TerrainData Generate_128x128()
    {
        var config = new TerrainConfig { Width = 128, Height = 128, BaseHeight = 0.5f };
        return _generator!.Generate(config);
    }

    [Benchmark]
    public TerrainData Generate_256x256()
    {
        var config = new TerrainConfig { Width = 256, Height = 256, BaseHeight = 0.5f };
        return _generator!.Generate(config);
    }

    [Benchmark]
    public TerrainData Generate_512x512()
    {
        var config = new TerrainConfig { Width = 512, Height = 512, BaseHeight = 0.5f };
        return _generator!.Generate(config);
    }

    [Benchmark]
    public async Task<TerrainData> GenerateAsync_512x512()
    {
        var config = new TerrainConfig { Width = 512, Height = 512, BaseHeight = 0.5f };
        return await _generator!.GenerateAsync(config);
    }
}
