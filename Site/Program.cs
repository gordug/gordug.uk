var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IFileMonitor, FileMonitor>();
builder.Services.AddTransient<ISyntaxHighlighter, SyntaxHighlighterFile>();
builder.Services.AddTransient<ISourceFiles, SourceFiles>();
builder.Services.AddSingleton<ICodeScroller, CodeScroller>();
builder.Services.AddSingleton<CancellationTokenSource>();
builder.Services.Configure<CodeScrollerOptions>(builder.Configuration.GetSection(nameof(CodeScrollerOptions)));

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