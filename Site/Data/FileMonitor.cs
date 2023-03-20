using Microsoft.Extensions.Options;

namespace gordug.uk.Data;

public class FileMonitor : IFileMonitor
{
    private List<FileSystemWatcher> _watcher = new();
    private Action<string>? _callback;
    private readonly IOptions<CodeScrollerOptions> _options;

    public FileMonitor(IOptions<CodeScrollerOptions> options)
    {
        _options = options;
    }
    
    public void StartWatching (Action<string>? callback)
    {
        var filterEnumerator = _options.Value.SearchPattern.Split(',').GetEnumerator();
        while (filterEnumerator.MoveNext())
        {
            var watcher = new FileSystemWatcher(_options.Value.SourceFilesPath,
                filterEnumerator.Current?.ToString()?.Trim() ?? string.Empty)
            {
                NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                               NotifyFilters.DirectoryName,
                EnableRaisingEvents = true,
                IncludeSubdirectories = true,
            };
            watcher.Changed += OnChanged;
            watcher.Created += OnChanged;
            watcher.Deleted += OnChanged;
            watcher.Renamed += OnChanged;
            _watcher.Add(watcher);
        }
        _callback = callback;
    }

    public void StopWatching()
    {
        _watcher?.ForEach(watcher => watcher.Dispose());
        _watcher?.Clear();
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _callback?.Invoke(e.FullPath);
    }
}