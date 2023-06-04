namespace gordug.uk.Data.CodeScroller;

public interface ICodeScroller
{
    IAsyncEnumerable<string> CodeScroll(CancellationToken cancellationToken = default);
}