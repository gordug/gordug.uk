namespace gordug.uk.Data;

public interface ISyntaxHighlighter : IDisposable
{
    string Output { get; set; }
    string Source { get; set; }
    
    void Highlight();
}