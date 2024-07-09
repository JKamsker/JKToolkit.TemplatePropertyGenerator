using SourceGeneratorTestHelpers;

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

        var gensource = result.Results.First().GeneratedSources
            .FirstOrDefault(x => x.HintName == "Tests.Consts_Generated.cs").SourceText;
        Assert.NotNull(gensource);

        var genSourceString = gensource.ToString();

        // Assert in the same namespace
        Assert.Contains("namespace Tests", genSourceString);
    }

    // Same class name in different namespaces
    [Fact]
    public void Test2()
    {
        string input = """
            namespace Tests;

            [TemplateProperty("Hello1", "Hello {value}")]
            public static partial class Consts
            {
            }
            namespace Tests2;

            [TemplateProperty("Hello2", "Hello {value}")]
            public static partial class Consts
            {
            }
            """;

        var result = IncrementalGenerator.Run<TemplatePropertySourceGenerator>(input);

        var sources = result.Results.First().GeneratedSources;

        Assert.Equal(2, sources.Count());

        var test1Consts = sources.FirstOrDefault(x => x.HintName == "Tests.Consts_Generated.cs");
        var test2Consts = sources.FirstOrDefault(x => x.HintName == "Tests2.Consts_Generated.cs");

        var genSourceString1 = test1Consts.SourceText.ToString();
        var genSourceString2 = test2Consts.SourceText.ToString();

        // Assert in the same namespace
        Assert.Contains("namespace Tests", genSourceString1);
        Assert.Contains("namespace Tests2", genSourceString2);
    }

    // tests if public partial class MyClass also works
    [Fact]
    public void Test3()
    {
        string input = """
            namespace Tests;

            [TemplateProperty("Hello", "Hello {value}")]
            public partial class MyClass
            {
            }
            """;

        var result = IncrementalGenerator.Run<TemplatePropertySourceGenerator>(input);

        var gensource = result.Results.First().GeneratedSources.FirstOrDefault(x => x.HintName == "Tests.MyClass_Generated.cs").SourceText;
        Assert.NotNull(gensource);

        var genSourceString = gensource.ToString();

        // Assert in the same namespace
        Assert.Contains("namespace Tests", genSourceString);
        Assert.Contains("public partial class MyClass", genSourceString);
    }

    [Fact]
    public void Test4()
    {
        string input = """
            namespace Tests;

            [TemplateProperty("Hello", "Hello {value}")]
            internal partial class MyClass
            {
            }
            """;

        var result = IncrementalGenerator.Run<TemplatePropertySourceGenerator>(input);

        var gensource = result.Results.First().GeneratedSources.FirstOrDefault(x => x.HintName == "Tests.MyClass_Generated.cs").SourceText;
        Assert.NotNull(gensource);

        var genSourceString = gensource.ToString();

        // Assert in the same namespace
        Assert.Contains("namespace Tests", genSourceString);
        Assert.Contains("internal partial class MyClass", genSourceString);
    }
}