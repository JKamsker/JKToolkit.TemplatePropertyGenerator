using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace JKToolKit.TemplatePropertyGenerator;

[Generator]
public class TemplatePropertyAttributeSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        // Register a syntax receiver that will be created for each generation pass
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        // Retrieve the populated receiver
        //var receiver = (SyntaxReceiver)context.SyntaxReceiver;

        if (context.SyntaxReceiver is not SyntaxReceiver receiver)
        {
            return;
        }

        // Generate the attribute if it doesn't exist
        if (receiver.AttributeExists)
        {
            return;
        }

        var attributeSource = """
            global using JKToolKit.TemplatePropertyGenerator.Attributes;

            using System;

            namespace JKToolKit.TemplatePropertyGenerator.Attributes;

            [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
            internal class TemplatePropertyAttribute : Attribute
            {
                public string Name { get; }
                public string Format { get; }

                public TemplatePropertyAttribute(string name, string format)
                {
                    Name = name;
                    Format = format;
                }
            }
            """;
        context.AddSource("TemplatePropertyAttribute.g.cs", attributeSource);

        // Add other generation logic here
    }

    private sealed class SyntaxReceiver : ISyntaxReceiver
    {
        public bool AttributeExists { get; private set; }

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            // Check if the attribute already exists in the user's code
            if (syntaxNode is AttributeSyntax attributeSyntax &&
                attributeSyntax.Name.ToString() == "TemplatePropertyAttribute")
            {
                AttributeExists = true;
            }
        }
    }
}