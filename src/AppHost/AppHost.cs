// =====================================================================================
// From code to Foundry — AppHost
//
// Two agents, two models, one Foundry project. Everything is defined in this file.
// It replaces the team's az scripts, portal clicks, and shared .env file.
//
//   aspire run               -> creates Foundry in Azure (the first time) and runs the agents locally
//   aspire publish -o infra  -> generates Bicep for review or use in your pipeline
//   aspire deploy            -> publishes both agents as hosted agents in Foundry
// =====================================================================================

using Aspire.Hosting.Foundry;

var builder = DistributedApplication.CreateBuilder(args);

// 1. Infrastructure: the Foundry account, one project, and two models.
var foundry  = builder.AddFoundry("foundry");
var project = foundry.AddProject("project");

var chat = project.AddModelDeployment("chat", FoundryModel.OpenAI.Gpt5Mini);
var image = project.AddModelDeployment("image", "gpt-image-1-mini", "2025-10-06", "OpenAI");

// 2. Schedule agent: it only needs the chat model.
builder.AddProject<Projects.AgendaAgent>("schedule")
    .WithHttpEndpoint(targetPort: 9001)
    .WithReference(project).WaitFor(project)
    .WithReference(chat).WaitFor(chat)
    .AsHostedAgent(project);

// 3. Poster agent: the chat model decides WHAT to draw; the image model draws it.
builder.AddProject<Projects.PosterAgent>("poster")
    .WithHttpEndpoint(targetPort: 9002)
    .WithReference(project).WaitFor(project)
    .WithReference(chat).WaitFor(chat)
    .WithReference(image).WaitFor(image)
    .AsHostedAgent(project);

builder.Build().Run();
