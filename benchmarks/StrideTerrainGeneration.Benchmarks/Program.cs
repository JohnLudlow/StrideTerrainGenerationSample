using BenchmarkDotNet.Running;

namespace StrideTerrainGeneration.Benchmarks;

internal class Program
{
    private static void Main(string[] args)
    {
        BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
    }
}
