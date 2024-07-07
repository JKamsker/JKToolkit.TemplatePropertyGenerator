using static JKToolKit.TemplatePropertyGenerator.FormattableHelpers;

namespace JKToolKit.TemplatePropertyGenerator.Tests;

public class FormattableTests
{
    [Fact]
    public void GetFormattableVariables_ReturnsEmptyList_WhenNoVariables()
    {
        // Arrange
        var formatString = "Hello World!";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFormattableVariables_ReturnsSingleVariable()
    {
        // Arrange
        var formatString = "Hello {name}!";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Single(result);
        Assert.Contains("name", result);
    }

    [Fact]
    public void GetFormattableVariables_ReturnsMultipleVariables()
    {
        // Arrange
        var formatString = "Hello {firstName}, {lastName}!";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("firstName", result);
        Assert.Contains("lastName", result);
    }

    [Fact]
    public void GetFormattableVariables_IgnoresDuplicateVariables()
    {
        // Arrange
        var formatString = "{value}, {value}, {value1}";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("value", result);
        Assert.Contains("value1", result);
    }

    [Fact]
    public void GetFormattableVariables_HandlesMixedContent()
    {
        // Arrange
        var formatString = "Value1: {value1}, Value2: {value2}, Value1 Again: {value1}";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains("value1", result);
        Assert.Contains("value2", result);
    }

    [Fact]
    public void GetFormattableVariables_ReturnsVariables_WhenBracesAreNotClosed()
    {
        // Arrange
        var formatString = "Hello {name";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFormattableVariables_ReturnsVariables_WhenBracesAreNotOpened()
    {
        // Arrange
        var formatString = "Hello name}";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void GetFormattableVariables_IgnoresEscapedBraces()
    {
        // Arrange
        var formatString = "Hello {{name}}, {value1}, {{value2}}";

        // Act
        var result = GetFormattableVariables(formatString);

        // Assert
        Assert.Single(result);
        Assert.Contains("value1", result);
    }
}