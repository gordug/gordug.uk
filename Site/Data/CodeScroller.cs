using System.Runtime.CompilerServices;

namespace gordug.uk.Data;

public class CodeScroller : IDisposable, ICodeScroller
{
    private readonly IFileMonitor _fileMonitor;
    private readonly Random _random = Random.Shared;
    private readonly ISourceFiles _sourceFiles;
    private readonly ISyntaxHighlighter _syntaxHighlighter;
    private string _code = string.Empty;
    private bool _isRunning;
    private string[] _paths = Array.Empty<string>();

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
            var code = HighLightCodeLines(cancellationToken).GetAsyncEnumerator(cancellationToken);
            while (await code.MoveNextAsync())
            {
                yield return code.Current;
            }

            await Task.Delay(10000, cancellationToken);
            yield break;
        }

        StopWatching();
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

    /// <summary>
    /// Randomly chooses one file from the available paths and populates Code with the contents 
    /// </summary>
    internal string PickCodeFile()
    {
        return _paths.Length == 0 ? string.Empty : _paths[_random.Next(0, _paths.Length)];
    }

    internal async Task Start()
    {
        if (_isRunning) return;
        await Task.FromResult(_paths = _sourceFiles.Paths().Distinct().ToArray());
        StartWatching();
        _isRunning = true;
    }

    /// <summary>
    /// takes each line of code, highlights it and returns it as a string
    /// </summary>
    /// <returns>Each line of highlighted code</returns>
    internal async IAsyncEnumerable<string> HighLightCodeLines(
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (cancellationToken.IsCancellationRequested) break;
            await Task.Run(HighlightCode, cancellationToken);
            if (string.IsNullOrWhiteSpace(_code)) break;
            var lines = await Task.Run(SplitCodeLines, cancellationToken);

            if (lines.Length == 0) break;
            var codeEnumerator = lines.GetEnumerator();
            while (codeEnumerator.MoveNext())
            {
                if (cancellationToken.IsCancellationRequested) break;
                yield return codeEnumerator.Current as string ?? string.Empty;
            }

            yield break;
        }
    }

    /// <summary>
    /// Calls the SyntaxHighlighter to highlight the code and populates the Code property with the output
    /// </summary>
    internal void HighlightCode()
    {
        _syntaxHighlighter.Source = PickCodeFile();
        _syntaxHighlighter.Highlight();
        _code = _syntaxHighlighter.Output;
    }

    /// <summary>
    /// Adds a new line after the pre tag to allow the code to be trimmed in the view
    /// </summary>
    internal string[] SplitCodeLines()
    {
        _code = _code.Insert(
            _code.IndexOf(">", _code.IndexOf("pre style", StringComparison.Ordinal), StringComparison.Ordinal) + 1,
            "\n");
        return _code.Split("\n");
    }

    /// <summary>
    /// Starts the file monitor and populates the paths array with the paths of the files in the SourceFiles directory
    /// </summary>
    internal void StartWatching()
    {
        _paths = _sourceFiles.Paths();
        _fileMonitor.StartWatching(OnFileChanged);
    }

    /// <summary>
    /// Stops the file monitor
    /// </summary>
    internal void StopWatching()
    {
        _fileMonitor.StopWatching();
    }

    /// <summary>
    /// Action to be performed when a file is changed
    /// </summary>
    /// <param name="path">Path of the file changed from the raised event</param>
    internal void OnFileChanged(string path)
    {
        if (_paths.Contains(path)) return;
        _paths = _paths.Append(path).ToArray();
    }
}