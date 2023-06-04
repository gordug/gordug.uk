using PygmentSharp.Core;

namespace gordug.uk.Data.CodeScroller;

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