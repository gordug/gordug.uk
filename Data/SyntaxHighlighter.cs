using PygmentSharp.Core.Formatting;

namespace gordug.uk.Data;

public abstract class SyntaxHighlighter : ISyntaxHighlighter
{
    internal readonly HtmlFormatter HtmlFormatter = new();
    public string Output { get; set; }
    public string Source { get; set; }

    protected SyntaxHighlighter()
    {
        Output = string.Empty;
        Source = string.Empty;
        HtmlFormatter.Options.ClassPrefix = "code-scroller";
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