using gordug.uk.Data.Common;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Serilog;
using Serilog.Core;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.RegisterCodeScroller();
builder.Services.RegisterPasswordGenerator(builder.Configuration);
builder.Services.AddSingleton<CancellationTokenSource>();
builder.Services.Configure<CodeScrollerOptions>(builder.Configuration.GetSection(nameof(CodeScrollerOptions)));
builder.Services.AddScoped<IClipboardService, ClipboardService>();
builder.Services.AddDataProtection()
       .UseCryptographicAlgorithms(
                                   new AuthenticatedEncryptorConfiguration
                                   {
                                       EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                                       ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                                   });

builder.Logging.ClearProviders();
Logger logger;
#if DEBUG
logger = new LoggerConfiguration()
         .Enrich.FromLogContext()
         .MinimumLevel.Verbose()
         .MinimumLevel.Override("Microsoft", LogEventLevel.Verbose)
         .WriteTo.Console()
         .CreateLogger();
#else
logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
    .WriteTo.Console()
    .CreateLogger();
#endif
builder.Logging.AddSerilog(logger);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
