using SourceGeneratorTestHelpers;
using JKToolKit.TemplatePropertyGenerator;

namespace JKToolKit.TemplatePropertyGenerator.Tests;

public class FormatPropertyGeneratorTests
{
    [Fact]
    public void Test1()
    {
        string input = """
            namespace Tests;

            [TemplateProperty("Hello", "Hello {value}")]
            public static partial class Consts
            {
            }
            """;

        var result = IncrementalGenerator.Run<TemplatePropertySourceGenerator>(input);

        var gensource = result.Results.First().GeneratedSources.FirstOrDefault(x => x.HintName == "Consts_Generated.cs").SourceText;
        Assert.NotNull(gensource);

        var genSourceString = gensource.ToString();

        // Assert in the same namespace
        Assert.Contains("namespace Tests", genSourceString);
    }
}