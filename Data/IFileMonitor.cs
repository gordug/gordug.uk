namespace gordug.uk.Data;

public interface IFileMonitor
{
    void StartWatching (Action<string>? callback);
    void StopWatching();
}