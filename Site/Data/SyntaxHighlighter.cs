namespace gordug.uk.Data;

public abstract class SyntaxHighlighter : ISyntaxHighlighter
{
    public string Output { get; set; }
    public string Source { get; set; }

    protected SyntaxHighlighter()
    {
        Output = string.Empty;
        Source = string.Empty;
    }
    
    public abstract void Highlight();
    
    private void Dispose(bool disposing)
    {
        if (!disposing) return;
        Output = string.Empty;
        Source = string.Empty;
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }
}