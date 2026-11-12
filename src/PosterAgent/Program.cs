// =====================================================================================
// Agent 2: posters. Two models:
//   - gpt-5-mini (chat)          -> understands the request and writes the image description
//   - gpt-image-1-mini (image)   -> draws. GeneratePoster calls it; the agent does not.
// An image model does not chat or call tools, so the agent needs both models.
// =====================================================================================

using PosterAgent;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry.Hosting;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

var port = Environment.GetEnvironmentVariable("DEFAULT_AD_PORT") ?? "8088";
builder.WebHost.UseUrls($"http://+:{port}");

var chatModel = Config.RequiredEnv("CHAT_GLOBALAINIGHT_MODELNAME");
var imageDeploymentName = Config.RequiredEnv("IMAGE_GLOBALAINIGHT_MODELNAME");
var projectEndpoint = Config.ProjectEndpoint();
var credential = new DefaultAzureCredential();
var project = new AIProjectClient(projectEndpoint, credential);

var posterGenerator = new PosterGenerator(
    new HttpClient(),
    credential,
    projectEndpoint,
    imageDeploymentName,
    baseUrl: Config.Env("POSTER_URL_BASE") ?? $"http://localhost:{port}");

AIAgent agent = project
    .GetProjectOpenAIClient()
    .GetProjectResponsesClient()
    .AsIChatClient(chatModel)
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "PosterAgent", configure: options => options.EnableSensitiveData = true) // demos only
    .Build()
    .AsAIAgent(
        name: "poster-agent-globalainight",
        instructions: """
            You design posters for Global AI Night sessions.
            When asked for a poster, write a concise, concrete visual description in English
            (style, colors, composition; no text in the image), then call GeneratePoster.
            Return the URL provided by the tool and a short sentence about the poster in English.
            """,
        tools: [AIFunctionFactory.Create(posterGenerator.GeneratePoster)]);

builder.Services.AddFoundryResponses(agent);

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapFoundryResponses(); // POST /responses
app.MapGet("/posters/{fileName}", PosterGenerator.Serve);
app.Run();
