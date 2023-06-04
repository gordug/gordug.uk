using FluentAssertions;
using gordug.uk.Data.CodeScroller;
using gordug.uk.Options;
using Xunit;

namespace gordug.uk.Tests;

public class FileMonitorTests : IDisposable
{
    private readonly CodeScrollerOptions _codeScrollerOptions;
    private readonly FileMonitor _fileMonitor;
    private readonly string _testDirectory;

    public FileMonitorTests()
    {
        _codeScrollerOptions = new CodeScrollerOptions
        {
            SourceFilesPath = Path.GetFullPath("wwwroot/SourceFiles/FileMonitorTests"),
            SearchPattern = "*.cs, *.sh, *.html, *.css, *.js, *.bash, *.xml, *.ts, *.sql"
        };
        var options = Microsoft.Extensions.Options.Options.Create(_codeScrollerOptions);
        _fileMonitor = new FileMonitor(options);
        _testDirectory = CreateTestDirectory();
    }

    public void Dispose()
    {
        Directory.Delete(_testDirectory, true);
    }

    [Fact]
    public void FileMonitor_ShouldTriggerCallback_OnFileChange()
    {
        string? changedFile = null;
        var resetEvent = new ManualResetEvent(false);

        void Callback(string path)
        {
            changedFile = path;
            resetEvent.Set();
        }

        _fileMonitor.StartWatching(Callback);

        var testFilePath = Path.Combine(_testDirectory, "testfile.cs");
        File.WriteAllText(testFilePath, "test content");
        File.WriteAllText(testFilePath, "updated content");
        resetEvent.WaitOne(TimeSpan.FromSeconds(30));

        changedFile.Should().NotBeNull();
        changedFile.Should().Be(testFilePath);

        _fileMonitor.StopWatching();
        File.Delete(testFilePath);
    }

    private string CreateTestDirectory()
    {
        var testDirectory = _codeScrollerOptions.SourceFilesPath;
        if (!Directory.Exists(testDirectory)) Directory.CreateDirectory(testDirectory);
        return testDirectory;
    }
}