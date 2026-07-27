using Microsoft.EntityFrameworkCore;
using ThreadCraft.Content;
using ThreadCraft.Core.Curriculum;
using ThreadCraft.Core.Execution;
using ThreadCraft.Core.Progress;
using ThreadCraft.Core.Validation;
using ThreadCraft.Execution;
using ThreadCraft.Web.Progress;
using ThreadCraft.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Course content (singleton: loaded once from disk at startup).
var contentRoot = ResolveContentRoot(builder.Configuration, builder.Environment);
builder.Services.AddSingleton<ICurriculumService>(_ => ContentCurriculumService.LoadFrom(contentRoot));

// Glossary lives next to the lessons folder (content/glossary.json).
var contentDirectory = Directory.GetParent(contentRoot)?.FullName ?? contentRoot;
builder.Services.AddSingleton(_ => GlossaryService.LoadFrom(Path.Combine(contentDirectory, "glossary.json")));

// Execution pipeline (docs/architecture.md §Frozen signatures).
builder.Services.AddSingleton(new ExecutionOptions { SandboxPath = Path.Combine(AppContext.BaseDirectory, "ThreadCraft.Sandbox.dll") });
builder.Services.AddSingleton<ICodeRunner, SandboxCodeRunner>();
builder.Services.AddSingleton<IExerciseValidator, RoslynExerciseValidator>();

// Progress: EF Core + SQLite. A context factory keeps the store thread-safe —
// every operation gets its own short-lived context.
builder.Services.AddDbContextFactory<ProgressDbContext>(options =>
    options.UseSqlite("Data Source=threadcraft-progress.db"));
builder.Services.AddScoped<IProgressStore, SqliteProgressStore>();

// UI helpers.
builder.Services.AddSingleton<MarkdownRenderer>();
builder.Services.AddScoped<ProgressSummaryService>();

// AI coach (OpenRouter). Key comes from Assistant:ApiKey or the OPENROUTER_API_KEY env var —
// without it the "Ask the coach" panel shows setup instructions instead of answers.
var assistantOptions =
    builder.Configuration.GetSection(AssistantOptions.SectionName).Get<AssistantOptions>()
    ?? new AssistantOptions();
if (string.IsNullOrWhiteSpace(assistantOptions.ApiKey))
{
    assistantOptions.ApiKey = builder.Configuration["OPENROUTER_API_KEY"] ?? "";
}
builder.Services.AddSingleton(assistantOptions);
builder.Services.AddSingleton<GlobalAssistantRateLimiter>();
builder.Services.AddScoped<AssistantRateLimiter>();
builder.Services.AddHttpClient<IAssistantService, OpenRouterAssistantService>(client =>
{
    client.BaseAddress = new Uri(assistantOptions.BaseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(90);
});

var app = builder.Build();

// Create the progress database file on first run.
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ProgressDbContext>>();
    await using var db = await contextFactory.CreateDbContextAsync();
    db.Database.EnsureCreated();
}

// sitemap.xml mirrors the shipped curriculum (SEO) — write before static files are served.
try
{
    SitemapGenerator.WriteToWwwRoot(app.Services.GetRequiredService<ICurriculumService>(), contentRoot);
}
catch (Exception ex)
{
    app.Logger.LogWarning(ex, "Sitemap generation failed; continuing without it.");
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<ThreadCraft.Web.Components.App>()
    .AddInteractiveServerRenderMode();

app.Run();

// Content root: configured value wins; otherwise walk up from the app folder
// until content/lessons is found (works both from the repo and a published output).
static string ResolveContentRoot(IConfiguration config, IHostEnvironment env)
{
    var configured = config["Curriculum:ContentRoot"];
    if (!string.IsNullOrWhiteSpace(configured))
        return Path.GetFullPath(configured);

    var dir = new DirectoryInfo(AppContext.BaseDirectory);
    while (dir is not null)
    {
        var candidate = Path.Combine(dir.FullName, "content", "lessons");
        if (Directory.Exists(candidate)) return candidate;
        dir = dir.Parent;
    }

    throw new InvalidOperationException(
        "Could not locate 'content/lessons'. Set Curriculum:ContentRoot in appsettings.json.");
}
