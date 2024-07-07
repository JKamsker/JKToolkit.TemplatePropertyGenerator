using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace JKToolKit.TemplatePropertyGenerator;

internal class FormattableHelpers
{
    internal static List<string> GetFormattableVariables(string formatString)
    {
        var variables = new List<string>();
        var index = 0;
        while (index < formatString.Length)
        {
            var openBraceIndex = formatString.IndexOf('{', index);
            if (openBraceIndex == -1)
            {
                break;
            }

            // Check for escaped brace
            if (openBraceIndex < formatString.Length - 1 && formatString[openBraceIndex + 1] == '{')
            {
                index = openBraceIndex + 2;
                continue;
            }

            var closeBraceIndex = formatString.IndexOf('}', openBraceIndex);
            if (closeBraceIndex == -1)
            {
                break;
            }

            // Check for escaped closing brace
            if (closeBraceIndex < formatString.Length - 1 && formatString[closeBraceIndex + 1] == '}')
            {
                index = closeBraceIndex + 2;
                continue;
            }

            var variable = formatString.Substring(openBraceIndex + 1, closeBraceIndex - openBraceIndex - 1);
            if (!variables.Contains(variable))
            {
                variables.Add(variable);
            }

            index = closeBraceIndex + 1;
        }

        return variables;
    }
}