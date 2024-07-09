namespace JKToolKit.TemplatePropertyGenerator.ConsoleTests;

internal static class Program
{
    private static void Main(string[] args)
    {
        MyClass m = new();
        

        Console.WriteLine(Consts1.Hello.Format("World"));
    }
}

[TemplateProperty("Hello", "Hello {value}, {value}!")]
public partial class MyClass
{

}

[TemplateProperty("Hello", "Hello {value}, {value}!")]
public static partial class Consts
{
}

public static partial class Consts1
{
    public static HelloStruct Hello => new();

    public struct HelloStruct
    {
        public const string Template = "Hello {value}";

        public FormattableString Format(string value)
        {
            return $"Hello {value}, {value}!";
        }
    }
}