// =====================================================================================
// From code to Foundry — AppHost
//
// Two agents, two models, one Foundry project. Everything is defined in this file.
// It replaces the team's az scripts, portal clicks, and shared .env file.
//
//   aspire run               -> creates Foundry in Azure (the first time) and runs the agents locally
//   aspire publish -o infra  -> generates Bicep for the Foundry account, project, and models
//   aspire deploy            -> provisions that infrastructure in Azure
//
// The agents are local processes that call the deployed models through the project API.
// =====================================================================================

using Aspire.Hosting.Foundry;

var builder = DistributedApplication.CreateBuilder(args);

// 1. Infrastructure: the Foundry account, one project, and two models.
var foundry  = builder.AddFoundry("foundry-globalainight");
var project = foundry.AddProject("project-globalainight");

var chat = project.AddModelDeployment("chat-globalainight", FoundryModel.OpenAI.Gpt5Mini);
var image = project.AddModelDeployment("image-globalainight", "gpt-image-1-mini", "2025-10-06", "OpenAI");

// Agents and the playground run only locally, so publish and deploy contain only the infrastructure.
if (builder.ExecutionContext.IsRunMode)
{
    // 2. Schedule agent: it only needs the chat model. DEFAULT_AD_PORT tells the agent which port to listen on.
    var schedule = builder.AddProject<Projects.AgendaAgent>("schedule-globalainight")
        .WithHttpEndpoint(targetPort: 9001, env: "DEFAULT_AD_PORT")
        .WithReference(project).WaitFor(project)
        .WithReference(chat).WaitFor(chat);

    // 3. Poster agent: the chat model decides WHAT to draw; the image model draws it.
    var poster = builder.AddProject<Projects.PosterAgent>("poster-globalainight")
        .WithHttpEndpoint(targetPort: 9002, env: "DEFAULT_AD_PORT")
        .WithReference(project).WaitFor(project)
        .WithReference(chat).WaitFor(chat)
        .WithReference(image).WaitFor(image);

    builder.AddProject<Projects.GlobalAiNightWeb>("playground-globalainight")
        .WithReference(schedule).WaitFor(schedule)
        .WithReference(poster).WaitFor(poster);
}

builder.Build().Run();
