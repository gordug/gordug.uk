using Microsoft.JSInterop;

namespace gordug.uk.Data.Common;

public interface IClipboardService
{
    ValueTask WriteTextAsync(string text);
}

internal class ClipboardService : IClipboardService
{
    private readonly IJSRuntime _jsRuntime;

    public ClipboardService(IJSRuntime jsRuntime)
        => _jsRuntime = jsRuntime;

    public ValueTask WriteTextAsync(string text)
    {
        return _jsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
    }
}
