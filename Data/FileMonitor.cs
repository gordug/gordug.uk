namespace gordug.uk.Data;

public class FileMonitor : IFileMonitor
{
    private FileSystemWatcher? _watcher;
    private readonly string _path;
    private readonly string _filter;
    private Action<string>? _callback;

    public FileMonitor()
    {
        _path = "wwwroot/SourceFiles";
        _filter = "*.cs";
    }
    
    public void StartWatching (Action<string>? callback)
    {
        _watcher = new FileSystemWatcher(_path, _filter)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            EnableRaisingEvents = true
        };
        _watcher.Changed += OnChanged;
        _callback = callback;
    }
    
    public void StopWatching()
    {
        _watcher?.Dispose();
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        _callback?.Invoke(e.FullPath);
    }
}