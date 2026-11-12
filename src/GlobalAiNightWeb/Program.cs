using GlobalAiNightWeb.Components;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Aspire service discovery resolves these names to the local agent URLs.
builder.Services.AddHttpClient("agenda", client =>
    client.BaseAddress = new Uri("http://schedule-globalainight/"));
builder.Services.AddHttpClient("poster", client =>
    client.BaseAddress = new Uri("http://poster-globalainight/"));

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

app.UseAntiforgery();

app.MapDefaultEndpoints();
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
