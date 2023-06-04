using gordug.uk.Models;

namespace gordug.uk.Data.PasswordGenerator;

public interface IPasswordGeneration
{
    Task<string> GeneratePasswordAsync(PasswordOptions options);
}