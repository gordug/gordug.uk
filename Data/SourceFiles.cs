namespace gordug.uk.Data;

public class SourceFiles : ISourceFiles
{
    private static readonly string SourceFilesPath = Path.GetFullPath("wwwroot/SourceFiles");
    private const string SearchPattern = "*.cs, *.sh, *.html, *.css, *.js, *.bash, *.xml, *.ts, *.sql";
    public string[] Paths()
    {
        var result = Directory.GetFiles(SourceFilesPath);
        return result;
    }
}