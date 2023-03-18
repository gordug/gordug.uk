using System.Runtime.CompilerServices;

namespace gordug.uk.Data;

public class CodeScroller : IDisposable, ICodeScroller
{
    private readonly ISyntaxHighlighter _syntaxHighlighter;
    private readonly IFileMonitor _fileMonitor;
    private readonly ISourceFiles _sourceFiles;
    private readonly Random _random = Random.Shared;
    private string _code = string.Empty;
    private string[] _paths = Array.Empty<string>();
    private bool _isRunning;

    public CodeScroller(
        ISyntaxHighlighter syntaxHighlighter, 
        IFileMonitor fileMonitor, 
        ISourceFiles sourceFiles)
    {
        _syntaxHighlighter = syntaxHighlighter;
        _fileMonitor = fileMonitor;
        _sourceFiles = sourceFiles;
    }
    
    public async IAsyncEnumerable<string> CodeScroll([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Start();
        while (!cancellationToken.IsCancellationRequested)
        {
            await foreach (var line in HighLightCodeLines(cancellationToken))
            {
                yield return line;
            }
            await Task.Delay(10000, cancellationToken);
            yield break;
        }
        StopWatching();
    }

    /// <summary>
    /// Randomly chooses one file from the available paths and populates Code with the contents 
    /// </summary>
    private string PickCodeFile()
    {
        return _paths[_random.Next(0, _paths.Length)];
    }
    
    private async Task Start()
    {
        if (_isRunning) return;
        _paths = _sourceFiles.Paths().Distinct().ToArray();
        await Task.FromResult(StartWatching);
        _isRunning = true;
    }

    /// <summary>
    /// takes each line of code, highlights it and returns it as a string
    /// </summary>
    /// <returns>Each line of highlighted code</returns>
    private async IAsyncEnumerable<string> HighLightCodeLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            HighlightCode();
            SplitCodeLines();
            var lines = _code.Split("\n");
            foreach (var line in lines)
            {
                yield return line;
            }
            yield break;
        }
    }

    private void HighlightCode()
    {
        _syntaxHighlighter.Source = PickCodeFile();
        _syntaxHighlighter.Highlight();
        _code = _syntaxHighlighter.Output;
    }

    private void SplitCodeLines()
    {
        _code = _code.Insert(
            _code.IndexOf(">", _code.IndexOf("pre style", StringComparison.Ordinal), StringComparison.Ordinal) + 1,
            "\n");
    }

    private void StartWatching()
    {
        _paths = _sourceFiles.Paths();
        _fileMonitor.StartWatching(OnFileChanged);
    }

    private void StopWatching()
    {
        _fileMonitor.StopWatching();
    }

    private void OnFileChanged(string path)
    {
        if (_paths.Contains(path)) return;
        _paths = _paths.Append(path).ToArray();
    }

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    void IDisposable.Dispose()
    {
        if (!_isRunning) return;
        _fileMonitor.StopWatching();
        _isRunning = false;
    }
}