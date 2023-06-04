namespace gordug.uk.Data.CodeScroller;

public interface IFileMonitor
{
    void StartWatching(Action<string>? callback);
    void StopWatching();
}