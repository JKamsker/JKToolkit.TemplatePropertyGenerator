using BenchmarkDotNet.Running;

namespace JKToolKit.TemplatePropertyGenerator.Benchmarks;

internal static class Program
{
    public static void Main(string[] args)
    {
        BenchmarkRunner.Run<BenchmarkTest>();
    }
}