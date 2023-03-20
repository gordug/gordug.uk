using FluentAssertions;
using gordug.uk.Options;
using Xunit;

namespace gordug.uk.Tests;

public class SourceFilesTests : IDisposable
{
    private readonly CodeScrollerOptions _codeScrollerOptions;
    private readonly gordug.uk.Data.SourceFiles _sourceFiles;
    private readonly string _testDirectory;

    public SourceFilesTests()
    {
        _codeScrollerOptions = new CodeScrollerOptions
        {
            SourceFilesPath = Path.GetFullPath("wwwroot/SourceFiles/SourceFileTests"),
            SearchPattern = "*.cs, *.sh, *.html, *.css, *.js, *.bash, *.xml, *.ts, *.sql"
        };
        var options = Microsoft.Extensions.Options.Options.Create(_codeScrollerOptions);
        _sourceFiles = new gordug.uk.Data.SourceFiles(options);
        _testDirectory = CreateTestDirectory();
    }

    public void Dispose()
    {
        Directory.Delete(_testDirectory, true);
    }

    [Fact]
    public void SourceFilesPath_IsValidDirectory()
    {
        _codeScrollerOptions.SourceFilesPath.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Paths_ReturnsEmptyArray_WhenNoFiles()
    {
        var result = _sourceFiles.Paths();
        result.Should().BeEmpty();
    }

    [Fact]
    public void Paths_ReturnsCorrectFiles_WhenSourceFilesPresent()
    {
        var filenames = new[]
        {
            "file1.cs", "file2.sh", "file3.html", "file4.css", "file5.js", "file6.bash", "file7.xml", "file8.ts",
            "file9.sql"
        };
        CreateTestFiles(filenames);
        var result = _sourceFiles.Paths();
        result.Should().HaveSameCount(filenames);
        var enumerator = filenames.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var filename = enumerator.Current as string ?? string.Empty;
            result.Should().Contain(Path.Combine(_testDirectory, filename));
        }

        DeleteTestFiles("file1.cs", "file2.sh", "file3.html", "file4.css", "file5.js", "file6.bash", "file7.xml",
            "file8.ts", "file9.sql");
    }

    [Fact]
    public void Paths_IgnoresOtherFileTypes()
    {
        CreateTestFiles("file1.txt", "file2.jpg", "file3.pdf");
        var result = _sourceFiles.Paths();
        result.Should().BeEmpty();
        DeleteTestFiles("file1.txt", "file2.jpg", "file3.pdf");
    }

    private string CreateTestDirectory()
    {
        var testDirectory = _codeScrollerOptions.SourceFilesPath;
        if (!Directory.Exists(testDirectory)) Directory.CreateDirectory(testDirectory);
        return testDirectory;
    }

    private void CreateTestFiles(params string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(_testDirectory, fileName);
            File.WriteAllText(filePath, "test content");
        }
    }

    private void DeleteTestFiles(params string[] fileNames)
    {
        foreach (var fileName in fileNames)
        {
            var filePath = Path.Combine(_testDirectory, fileName);
            File.Delete(filePath);
        }
    }
}