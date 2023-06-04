using gordug.uk.Data.CodeScroller;
using gordug.uk.Data.PasswordGenerator;

namespace gordug.uk.Data;

public static class RegisterDemos
{
    public static void RegisterCodeScroller(this IServiceCollection services)
    {
        services.AddSingleton<IFileMonitor, FileMonitor>();
        services.AddTransient<ISyntaxHighlighter, SyntaxHighlighterFile>();
        services.AddTransient<ISourceFiles, SourceFiles>();
        services.AddSingleton<ICodeScroller, CodeScroller.CodeScroller>();
    }

    public static void RegisterPasswordGenerator(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient("PasswordGeneration", c =>
        {
            var baseAddress = configuration["PasswordGeneration:BaseAddress"];
            c.BaseAddress = new Uri(baseAddress);
        });
        services.AddTransient<IPasswordGeneration, PasswordGeneration>();
    }
}