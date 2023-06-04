namespace gordug.uk.Data.CodeScroller;

public abstract class SyntaxHighlighter : ISyntaxHighlighter
{
    protected SyntaxHighlighter()
    {
        Output = string.Empty;
        Source = string.Empty;
    }

    public string Output { get; set; }
    public string Source { get; set; }

    public abstract void Highlight();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        Dispose(true);
    }

    private void Dispose(bool disposing)
    {
        if (!disposing) return;
        Output = string.Empty;
        Source = string.Empty;
    }
}