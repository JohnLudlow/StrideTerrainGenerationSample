using BenchmarkDotNet.Attributes;
using StrideTerrainGeneration.Core;
using StrideTerrainGeneration.Core.Configuration;

namespace StrideTerrainGeneration.Benchmarks;

[MemoryDiagnoser]
public class PerlinTerrainGeneratorBenchmarks
{
    private PerlinTerrainGenerator? _generator;

    [GlobalSetup]
    public void Setup()
    {
        _generator = new PerlinTerrainGenerator();
    }

    [Benchmark]
    public TerrainData Generate_64x64()
    {
        return _generator!.Generate(new TerrainConfig { Width = 64, Height = 64 });
    }

    [Benchmark]
    public TerrainData Generate_128x128()
    {
        return _generator!.Generate(new TerrainConfig { Width = 128, Height = 128 });
    }

    [Benchmark]
    public TerrainData Generate_256x256()
    {
        return _generator!.Generate(new TerrainConfig { Width = 256, Height = 256 });
    }

    [Benchmark]
    public TerrainData Generate_512x512()
    {
        return _generator!.Generate(new TerrainConfig { Width = 512, Height = 512 });
    }

    [Benchmark]
    public async Task<TerrainData> GenerateAsync_256x256()
    {
        return await _generator!.GenerateAsync(new TerrainConfig { Width = 256, Height = 256 });
    }
}
