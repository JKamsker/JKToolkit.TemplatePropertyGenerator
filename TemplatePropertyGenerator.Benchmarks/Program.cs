using BenchmarkDotNet.Running;

namespace JKToolkit.TemplatePropertyGenerator.Benchmarks;

internal class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BenchmarkTest>();
    }
}