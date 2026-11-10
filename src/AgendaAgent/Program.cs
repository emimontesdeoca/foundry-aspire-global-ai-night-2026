// =====================================================================================
// Agent 1: the schedule. Microsoft Agent Framework + gpt-5-mini + one tool.
// It knows nothing about Aspire. It reads environment variables and exposes /responses.
// =====================================================================================

using AgendaAgent;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Foundry.Hosting;
using Microsoft.Extensions.AI;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Aspire injects DEFAULT_AD_PORT; 8088 is the hosted agents' default.
var port = Environment.GetEnvironmentVariable("DEFAULT_AD_PORT") ?? "8088";
builder.WebHost.UseUrls($"http://+:{port}");

// Locally, Aspire injects PROJECT_URI and CHAT_MODELNAME through the AppHost references.
// In Foundry, the platform injects FOUNDRY_PROJECT_ENDPOINT.
var endpoint = Config.ProjectEndpoint();
var model = Config.Env("CHAT_MODELNAME") ?? Config.Env("AZURE_AI_MODEL_DEPLOYMENT_NAME") ?? "chat";

AIAgent agent = new AIProjectClient(endpoint, new DefaultAzureCredential())
    .GetProjectOpenAIClient()
    .GetProjectResponsesClient()
    .AsIChatClient(model)
    .AsBuilder()
    .UseOpenTelemetry(sourceName: "AgendaAgent", configure: options => options.EnableSensitiveData = true) // demos only
    .Build()
    .AsAIAgent(
        name: "schedule-agent",
        instructions: """
            You are the Global AI Night schedule assistant.
            Help attendees choose sessions based on their interests.
            Always use the SearchSessions tool; never invent sessions or times.
            Respond in English, in no more than three or four lines.
            """,
        tools: [AIFunctionFactory.Create(Tools.SearchSessions)]);

builder.Services.AddFoundryResponses(agent);

var app = builder.Build();
app.MapDefaultEndpoints();
app.MapFoundryResponses(); // POST /responses
app.Run();
