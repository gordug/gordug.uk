namespace gordug.uk.Data;

public interface ICodeScroller
{
    IAsyncEnumerable<string> CodeScroll(CancellationToken cancellationToken = default);
}