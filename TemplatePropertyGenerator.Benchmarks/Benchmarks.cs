using BenchmarkDotNet.Attributes;

namespace JKToolKit.TemplatePropertyGenerator.Benchmarks;

public static class ConstsClass
{
    public static HelloClass Hello { get; } = new();

    public class HelloClass
    {
        public string? Template { get; }

        public string Format(string value)
            => $"Hello {value}, {value}!";

        public FormattableString AsFormattable(string value)
            => $"Hello {value}, {value}!";
    }
}

public static class ConstsStruct
{
    public static HelloStruct Hello => new();

    public struct HelloStruct
    {
        public const string Template = "Hello {value}";

        public string Format(string value)
        {
            return $"Hello {value}, {value}!";
        }

        public FormattableString AsFormattable(string value)
        {
            return $"Hello {value}, {value}!";
        }
    }
}

/*
| Method            | Mean     | Error    | StdDev   | Gen0   | Allocated |
|------------------ |---------:|---------:|---------:|-------:|----------:|
| ClassFormat       | 37.30 ns | 0.745 ns | 0.968 ns | 0.0057 |      48 B |
| StructFormat      | 39.71 ns | 0.825 ns | 0.982 ns | 0.0076 |      64 B |
| ClassFormattable  | 11.56 ns | 0.274 ns | 0.326 ns | 0.0086 |      72 B |
| StructFormattable | 12.04 ns | 0.277 ns | 0.272 ns | 0.0086 |      72 B |

//# 2

| Method            | Mean     | Error    | StdDev   | Gen0   | Allocated |
|------------------ |---------:|---------:|---------:|-------:|----------:|
| ClassFormat       | 39.08 ns | 0.712 ns | 0.666 ns | 0.0076 |      64 B |
| StructFormat      | 39.67 ns | 0.758 ns | 0.709 ns | 0.0076 |      64 B |
| ClassFormattable  | 11.53 ns | 0.275 ns | 0.403 ns | 0.0086 |      72 B |
| StructFormattable | 11.87 ns | 0.272 ns | 0.227 ns | 0.0086 |      72 B |
 */

[MemoryDiagnoser]
public class BenchmarkTest
{
    private const string TestValue = "world";

    [Benchmark]
    public string ClassFormat()
    {
        return ConstsClass.Hello.Format(TestValue);
    }

    [Benchmark]
    public string StructFormat()
    {
        return ConstsStruct.Hello.Format(TestValue);
    }

    // Formattable tests
    [Benchmark]
    public FormattableString ClassFormattable()
    {
        return ConstsClass.Hello.AsFormattable(TestValue);
    }

    [Benchmark]
    public FormattableString StructFormattable()
    {
        return ConstsStruct.Hello.AsFormattable(TestValue);
    }
}