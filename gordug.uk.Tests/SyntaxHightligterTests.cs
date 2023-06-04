using FluentAssertions;
using gordug.uk.Data.CodeScroller;
using Xunit;

namespace gordug.uk.Tests;

public class SyntaxHighlighterTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly string _testFilePath;

    public SyntaxHighlighterTests()
    {
        _testDirectory = CreateTestDirectory();
        _testFilePath = Path.Combine(_testDirectory, "testfile.cs");
        File.WriteAllText(_testFilePath,
            "using System;\n\nnamespace Test\n{\n    public class TestClass\n    {\n    }\n}\n");
    }

    public void Dispose()
    {
        Directory.Delete(_testDirectory, true);
    }

    [Fact]
    public void SyntaxHighlighterFile_Highlight_ShouldGenerateHighlightedOutput()
    {
        using var syntaxHighlighter = new SyntaxHighlighterFile();
        syntaxHighlighter.Source = _testFilePath;
        syntaxHighlighter.Highlight();

        syntaxHighlighter.Output.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SyntaxHighlighterFile_Highlight_ShouldNotGenerateOutput_ForInvalidFilePath()
    {
        using var syntaxHighlighter = new SyntaxHighlighterFile();
        syntaxHighlighter.Source = "nonexistent_file.cs";
        syntaxHighlighter.Highlight();

        syntaxHighlighter.Output.Should().BeEmpty();
    }

    [Fact]
    public void SyntaxHighlighterFile_Dispose_ShouldClearSourceAndOutput()
    {
        var syntaxHighlighter = new SyntaxHighlighterFile
        {
            Source = _testFilePath,
            Output = "highlighted output"
        };

        syntaxHighlighter.Dispose();

        syntaxHighlighter.Source.Should().BeEmpty();
        syntaxHighlighter.Output.Should().BeEmpty();
    }

    private static string CreateTestDirectory()
    {
        var testDirectory = Path.GetFullPath("wwwroot/SourceFiles/SyntaxHighlighterTests");
        if (!Directory.Exists(testDirectory))
        {
            Directory.CreateDirectory(testDirectory);
        }

        return testDirectory;
    }
}