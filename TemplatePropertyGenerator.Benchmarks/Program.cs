using BenchmarkDotNet.Running;

namespace JKToolKit.TemplatePropertyGenerator.Benchmarks;

internal class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<BenchmarkTest>();
    }
}