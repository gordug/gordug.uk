using System.ComponentModel.DataAnnotations;

namespace gordug.uk.Models;

public class PasswordOptions
{
    public PasswordOptions()
    {
        Length = 12;
        IncludeSpecialCharacters = true;
        IncludeNumbers = true;
        IncludeLowercase = true;
        IncludeUppercase = true;
        ExcludeSimilarCharacters = false;
        ExcludeAmbiguousCharacters = false;
        ExcludeSequentialCharacters = false;
        RequireSpecialCharacters = false;
        RequireNumbers = false;
        RequireLowercase = false;
        RequireUppercase = false;
        Password = string.Empty;
    }

    [Range(8, 128)]
    public int Length { get; set; }

    public bool IncludeSpecialCharacters { get; set; }
    public bool IncludeNumbers { get; set; }
    public bool IncludeLowercase { get; set; }
    public bool IncludeUppercase { get; set; }
    public bool ExcludeSimilarCharacters { get; set; }
    public bool ExcludeAmbiguousCharacters { get; set; }
    public bool ExcludeSequentialCharacters { get; set; }
    public bool RequireSpecialCharacters { get; set; }
    public bool RequireNumbers { get; set; }
    public bool RequireLowercase { get; set; }
    public bool RequireUppercase { get; set; }
    public string Password { get; set; }
}
