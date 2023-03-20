namespace gordug.uk.Options;

public class CodeScrollerOptions
{
    public string SourceFilesPath { get; set; } = "wwwroot/SourceFiles";
    public string SearchPattern { get; set; } = "*.cs, *.sh, *.html, *.css, *.js, *.bash, *.xml, *.ts, *.sql";
}