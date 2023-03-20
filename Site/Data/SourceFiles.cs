using Microsoft.Extensions.Options;

namespace gordug.uk.Data;

public class SourceFiles : ISourceFiles
{
    private readonly IOptions<CodeScrollerOptions> _options;
    public SourceFiles(IOptions<CodeScrollerOptions> options)
    {
        _options = options;
    }
    public string[] Paths()
    {
        List<string> result = new();
        using var searchPattern = SearchPattern().GetEnumerator();
        while (searchPattern.MoveNext())
        {
            result.AddRange(Directory.GetFiles(Path.GetFullPath(_options.Value.SourceFilesPath), searchPattern.Current.Trim()));
        }
        return result.ToArray();
    }

    private IEnumerable<string> SearchPattern() => _options.Value.SearchPattern.Split(",");
}