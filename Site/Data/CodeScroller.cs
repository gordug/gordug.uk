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

    /// <summary>
    /// CodeScroller constructor with DI for ISyntaxHighlighter, IFileMonitor and ISourceFiles
    /// </summary>
    /// <param name="syntaxHighlighter"></param>
    /// <param name="fileMonitor"></param>
    /// <param name="sourceFiles"></param>
    public CodeScroller(
        ISyntaxHighlighter syntaxHighlighter, 
        IFileMonitor fileMonitor, 
        ISourceFiles sourceFiles)
    {
        _syntaxHighlighter = syntaxHighlighter;
        _fileMonitor = fileMonitor;
        _sourceFiles = sourceFiles;
    }
    
    /// <summary>
    /// Returns a string of highlighted code from a random file in the SourceFiles directory per iteration
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
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
    
    internal async Task Start()
    {
        if (_isRunning) return;
        _paths = _sourceFiles.Paths().Distinct().ToArray();
        StartWatching();
        _isRunning = true;
    }

    /// <summary>
    /// takes each line of code, highlights it and returns it as a string
    /// </summary>
    /// <returns>Each line of highlighted code</returns>
    internal async IAsyncEnumerable<string> HighLightCodeLines([EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (cancellationToken.IsCancellationRequested) break;
            HighlightCode();
            if (string.IsNullOrWhiteSpace(_code)) break;
            SplitCodeLines();
            var lines = _code.Split("\n");
            if (lines.Length == 0) break;
            foreach (var line in lines)
            {
                if (cancellationToken.IsCancellationRequested) break;
                yield return line;
            }
            yield break;
        }
    }

    /// <summary>
    /// Calls the SyntaxHighlighter to highlight the code and populates the Code property with the output
    /// </summary>
    private void HighlightCode()
    {
        _syntaxHighlighter.Source = PickCodeFile();
        _syntaxHighlighter.Highlight();
        _code = _syntaxHighlighter.Output;
    }
    
    /// <summary>
    /// Adds a new line after the pre tag to allow the code to be trimmed in the view
    /// </summary>
    private void SplitCodeLines()
    {
        _code = _code.Insert(
            _code.IndexOf(">", _code.IndexOf("pre style", StringComparison.Ordinal), StringComparison.Ordinal) + 1,
            "\n");
    }
    
    /// <summary>
    /// Starts the file monitor and populates the paths array with the paths of the files in the SourceFiles directory
    /// </summary>
    private void StartWatching()
    {
        _paths = _sourceFiles.Paths();
        _fileMonitor.StartWatching(OnFileChanged);
    }
    
    /// <summary>
    /// Stops the file monitor
    /// </summary>
    private void StopWatching()
    {
        _fileMonitor.StopWatching();
    }

    /// <summary>
    /// Action to be performed when a file is changed
    /// </summary>
    /// <param name="path">Path of the file changed from the raised event</param>
    private void OnFileChanged(string path)
    {
        if (_paths.Contains(path)) return;
        _paths = _paths.Append(path).ToArray();
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    void IDisposable.Dispose()
    {
        if (!_isRunning) return;
        _fileMonitor.StopWatching();
        _isRunning = false;
    }
}