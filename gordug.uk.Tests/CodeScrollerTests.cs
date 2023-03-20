using FluentAssertions;
using gordug.uk.Data;
using gordug.uk.Options;
using Moq;
using Xunit;

namespace gordug.uk.Tests;

public class CodeScrollerTests : IDisposable
    {
        private readonly string _testDirectory;
        private readonly CodeScroller _codeScroller;
        

        public CodeScrollerTests()
        {
            _testDirectory = CreateTestDirectory();
            var testFilePath = Path.Combine(_testDirectory, "test-file.cs");
            File.WriteAllText(testFilePath, "using System;\n\nnamespace Test\n{\n    public class TestClass\n    {\n    }\n}\n");

            var syntaxHighlighter = new Mock<ISyntaxHighlighter>
            {
                CallBase = true // Allow calling the base implementation for properties
            };
            syntaxHighlighter.SetupSet(sh => sh.Source = It.IsAny<string>()).Callback<string>(source => syntaxHighlighter.Object.Source = source);
            syntaxHighlighter.Setup(sh => sh.Highlight()).Callback(() => syntaxHighlighter.Object.Output = "Highlighted code");

            var fileMonitorMock = new Mock<IFileMonitor>();
            var sourceFilesMock = new Mock<ISourceFiles>();
            sourceFilesMock.Setup(sf => sf.Paths()).Returns(new[] { testFilePath });

            _codeScroller = new CodeScroller(syntaxHighlighter.Object, fileMonitorMock.Object, sourceFilesMock.Object);
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