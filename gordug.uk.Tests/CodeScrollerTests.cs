using FluentAssertions;
using gordug.uk.Data;
using Moq;
using Xunit;

namespace gordug.uk.Tests;

public class CodeScrollerTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly string _testFilePath;
        private readonly CodeScroller _codeScroller;
        private Mock<ISyntaxHighlighter> _syntaxHighlighter;
        private Mock<IFileMonitor> _fileMonitorMock;
        private Mock<ISourceFiles> _sourceFilesMock;

        public CodeScrollerTests()
        {
            _testDirectory = CreateTestDirectory();
            _testFilePath = Path.Combine(_testDirectory, "test-file.cs");
            File.WriteAllText(_testFilePath,
                "using System;\n\nnamespace Test\n{\n    public class TestClass\n    {\n    }\n}\n");

            _syntaxHighlighter = new Mock<ISyntaxHighlighter>
            {
                CallBase = true // Allow calling the base implementation for properties
            };
            _syntaxHighlighter.Setup(sh => sh.Highlight())
                .Callback(() => _syntaxHighlighter.Object.Output = "Highlighted code");

            _fileMonitorMock = new Mock<IFileMonitor>();
            _sourceFilesMock = new Mock<ISourceFiles>();
            _sourceFilesMock.Setup(sf => sf.Paths()).Returns(new[] { _testFilePath });

            _codeScroller = new CodeScroller(_syntaxHighlighter.Object, _fileMonitorMock.Object, _sourceFilesMock.Object);
        }
        
        [Fact]
        public async Task CodeScroller_StartWatching_Should_CallFileMonitorStartWatching()
        {
            await _codeScroller.Start();

            _fileMonitorMock.Verify(fm => fm.StartWatching(It.IsAny<Action<string>>()), Times.Once);
        }
        
        [Fact]
        public async Task CodeScroller_StartWatching_Should_CallSourceFilesPaths()
        {
            await _codeScroller.Start();

            _sourceFilesMock.Verify(sf => sf.Paths(), Times.AtLeast(1));
        }
        
        [Fact]
        public async Task CodeScroller_StopWatching_Should_CallFileMonitorStopWatching()
        {
            await _codeScroller.Start();
            _codeScroller.StopWatching();

            _fileMonitorMock.Verify(fm => fm.StopWatching(), Times.Once);
        }
        
        [Fact]
        public async Task CodeScroller_PickCodeFile_ShouldReturnRandomFile()
        {
            await _codeScroller.Start();
            _codeScroller.PickCodeFile().Should().NotBeNullOrEmpty();
            _codeScroller.PickCodeFile().Should().Match(_testFilePath);
        }

        [Fact]
        public async Task HighlightCode_ShouldSetSourceAndHighlight()
        {
            var testFilePath = Path.Combine(_testDirectory, "test-file.cs");
            await _codeScroller.Start();
            _codeScroller.HighlightCode();
        }

        [Fact]
        public async Task CodeScroller_HighLightCodeLines_ShouldYieldHighlightedCodeLines()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            await _codeScroller.Start();

            await foreach (var line in _codeScroller.HighLightCodeLines(cancellationTokenSource.Token))
            {
                line.Should().NotBeNullOrEmpty();
                line.Should().Be("Highlighted code");
                break;
            }
        }

        [Fact]
        public async Task CodeScroller_Dispose_ShouldStopWatching()
        {
            using var cancellationTokenSource = new CancellationTokenSource();
            await _codeScroller.Start();
            
            cancellationTokenSource.Cancel();

            _codeScroller.Invoking(cs => ((IDisposable)cs).Dispose()).Should().NotThrow();
        }

        private string CreateTestDirectory()
        {
            var testDirectory = Path.GetFullPath("wwwroot/SourceFiles/TestDirectory");
            if (!Directory.Exists(testDirectory))
            {
                Directory.CreateDirectory(testDirectory);
            }
            return testDirectory;
        }

        public void Dispose()
        {
            Directory.Delete(_testDirectory, true);
        }
    }