using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JKToolKit.TemplatePropertyGenerator;

[Generator]
public class TemplatePropertySourceGenerator : IIncrementalGenerator
{
    private static string templateGeneratedCode = null;
    private static string templateClassCode = null;

    internal readonly static Assembly assembly = typeof(TemplatePropertySourceGenerator).Assembly;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null);

        var compilationAndClasses = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses, static (spc, source) => Execute(source.Left, source.Right, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
    {
        return node is ClassDeclarationSyntax classDeclarationSyntax && classDeclarationSyntax.AttributeLists.Count > 0;
    }

    private static ClassDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (IsTemplatePropertyAttribute(attributeSyntax))
                {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        foreach (var classDeclaration in classes.Distinct())
        {
            if (classDeclaration is null) continue;

            var namespaceName = GetNamespace(classDeclaration);
            var className = classDeclaration.Identifier.Text;

            var isPartial = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

            if (!isPartial)
            {
                // Show diagnostic
                context.ReportDiagnostic
                (
                    Diagnostic.Create
                    (
                        new DiagnosticDescriptor
                        (
                            id: "TPG001",
                            title: "Class must be static partial",
                            messageFormat: "Class must be static partial",
                            category: "TemplatePropertyGenerator",
                            defaultSeverity: DiagnosticSeverity.Error,
                            isEnabledByDefault: true
                        ),
                        classDeclaration.GetLocation()
                    )
                );
                continue;
            }

            var isStatic = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.StaticKeyword));


            var modifiers = isStatic ? "static partial" : "partial";
            var access = classDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)) ? "public" : "internal";


            var decl = $"{access} {modifiers} class {className}";

            var generatedCode = GenerateClassCode(classDeclaration, namespaceName, decl);

            context.AddSource($"{namespaceName}.{className}_Generated.cs", SourceText.From(generatedCode, Encoding.UTF8));
        }
    }

    private static string GenerateClassCode(ClassDeclarationSyntax classDeclaration, string namespaceName, string decl)
    {
        StringBuilder generatedCode = new();
        generatedCode.Append(RenderGeneratedCode(namespaceName, decl).Result);

        StringBuilder classCode = new();

        foreach (var attributeListSyntax in classDeclaration.AttributeLists)
        {
            foreach (AttributeSyntax attributeSyntax in attributeListSyntax.Attributes)
            {
                if (!IsTemplatePropertyAttribute(attributeSyntax))
                {
                    continue;
                }

                var attributeArguments = attributeSyntax.ArgumentList?.Arguments;
                if (attributeArguments == null || attributeArguments.Value.Count != 2)
                {
                    continue;
                }

                var name = attributeArguments.Value[0].Expression.ToString().Trim('"');
                var format = attributeArguments.Value[1].Expression.ToString().Trim('"');

                var formatVariables = FormattableHelpers.GetFormattableVariables(format); // Contains value, value1 ...

                var formatMethodParameters = string.Join(", ", formatVariables.Select(v => $"string {v}"));

                classCode.AppendLine(RenderClassCode(name, format, formatMethodParameters).Result);
            }
        }

        generatedCode.Replace("{{attributes}}", classCode.ToString());

        return generatedCode.ToString();
    }

    private static bool IsTemplatePropertyAttribute(AttributeSyntax attributeSyntax)
    {
        var name = attributeSyntax.Name.ToString();
        return string.Equals(name, "TemplatePropertyAttribute", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(name, "TemplateProperty", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetNamespace(SyntaxNode syntaxNode)
    {
        while (syntaxNode != null)
        {
            if (syntaxNode is BaseNamespaceDeclarationSyntax namespaceDeclaration)
            {
                return namespaceDeclaration.Name.ToString();
            }
            syntaxNode = syntaxNode.Parent;
        }
        return null;
    }

    private static async Task<string> RenderGeneratedCode(string @namespace, string decl)
    {
        if (templateGeneratedCode == null)
        {
            using (Stream s = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.AutoGeneratedProperty.txt"))
            {
                using (StreamReader r = new(s))
                {
                    templateGeneratedCode = await r.ReadToEndAsync();
                }
            }
        }

        StringBuilder templateGeneratedCodeSb = new();
        templateGeneratedCodeSb.Append(templateGeneratedCode);

        templateGeneratedCodeSb.Replace("{{namespace}}", $"namespace {@namespace}\n{{").Replace("{{namespaceTerminator}}", "}");
        templateGeneratedCodeSb.Replace("{{classDeclaration}}", $"{decl}\n{{");

        return templateGeneratedCodeSb.ToString();
    }

    private static async Task<string> RenderClassCode(string name, string format, string formatMethodParameters)
    {
        if (templateGeneratedCode == null)
        {
            using (Stream s = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.AutoGeneratedClassCode.txt"))
            {
                using (StreamReader r = new(s))
                {
                    templateClassCode = await r.ReadToEndAsync();
                }
            }
        }

        StringBuilder templateClassCodeSb = new();
        templateClassCodeSb.Append(templateClassCode);

        templateClassCodeSb.Replace("{{name}}", name);
        templateClassCodeSb.Replace("{{format}}", format);
        templateClassCodeSb.Replace("{{formatMethodParameters}}", formatMethodParameters);

        return templateClassCodeSb.ToString();
    }
}
