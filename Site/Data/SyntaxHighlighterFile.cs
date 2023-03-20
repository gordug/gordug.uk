using PygmentSharp.Core;

namespace gordug.uk.Data;

public class SyntaxHighlighterFile : SyntaxHighlighter, ISyntaxHighlighter
{
    public override void Highlight()
    {
        if (!Path.Exists(Source)) return;
        Output = Pygmentize.File(Source)
            .ToHtml()
            .AsString();
    }
}