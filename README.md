# From code to Foundry: agent infrastructure with Aspire

This repository contains the talk slides (MARP) and a small .NET demo that moves two agents from a
local machine to Microsoft Foundry.

The slides are published on [GitHub Pages](https://emimontesdeoca.github.io/foundry-aspire-global-ai-night-2026/)
and are rebuilt on pushes to `main` by GitHub Actions.

```text
foundry-aspire-global-ai-night-2026/
├── README.md
├── slides/
│   ├── deck.md                    # MARP slides, with 40-minute speaker notes
│   └── assets/
│       ├── emiliano-avatar.jpg    # Speaker photo
│       ├── microsoft-mvp.png
│       ├── most-active-speaker-*.svg
│       └── fonts/                 # Space Grotesk and its OFL license
└── src/
    ├── GlobalAiNight.slnx         # .NET solution
    ├── Directory.Build.props
    ├── AppHost/                   # Foundry, project, models, agents, and playground
    ├── ServiceDefaults/           # OpenTelemetry, service discovery, and health checks
    ├── AgendaAgent/               # gpt-5-mini + SearchSessions
    ├── PosterAgent/               # gpt-5-mini + GeneratePoster (gpt-image-1-mini)
    └── GlobalAiNightWeb/          # Blazor Server playground for both agents
```

## 1. The talk in a nutshell

Writing agents with Microsoft Agent Framework is the easy part. Teams also need to create the
Foundry account and project, deploy models, grant access without sharing keys, inspect agent traces,
and deploy without relying on one person. This demo starts with two small .NET agents—one recommends
sessions using `gpt-5-mini`, and one creates session posters with an image model—that call models
deployed in a Foundry project. An Aspire AppHost describes the infrastructure and connects each agent
only to the resources it needs.

The talk follows the development-to-deployment workflow: connect local agents to real Foundry
resources in Azure with `aspire run`, review generated infrastructure with `aspire publish -o infra`,
then provision that infrastructure with `aspire deploy`. Aspire supplies endpoints and configuration
through references and brings local URLs, resource status, logs, and traces into the dashboard.
Subscription, region, permissions, and quota still need to be chosen and prepared.

The agents are not Foundry hosted agents: they are regular .NET processes that call the project's
Responses API with Entra ID. `Microsoft.Agents.AI.Foundry.Hosting` is used only to expose the
`POST /responses` endpoint that the playground calls.

**Agent Framework writes the agents, Foundry serves the models, and Aspire describes everything around them.**

## 2. The demo: two agents, two models

The demo is intentionally small: two independent agents, one tool per agent, and one AppHost.

| Resource | What it is | Where it runs |
|---|---|---|
| `foundry-globalainight` | Microsoft Foundry account; local auth is disabled and Aspire creates the required roles | Azure |
| `project-globalainight` | Foundry project that groups the models and exposes the Responses API | Azure |
| `chat-globalainight` | `gpt-5-mini` deployment | Azure |
| `image-globalainight` | `gpt-image-1-mini` deployment (`2025-10-06`) | Azure |
| `schedule-globalainight` | Recommends sessions from sample data using `chat-globalainight` and `SearchSessions` | Local with `aspire run` |
| `poster-globalainight` | Uses `chat-globalainight` to write an image description, then calls `GeneratePoster` to invoke `image-globalainight` | Local with `aspire run` |
| `playground-globalainight` | Blazor Server UI for testing both agents | Local with `aspire run` |

The image model does not chat or call tools: it receives a description and returns an image. The
`gpt-5-mini` agent understands the request, writes the description, and decides when to call the
image-generation tool. The poster agent saves its PNG in `posters/` and serves it from
`/posters/{fileName}` locally.

The sample schedule is **fictional**; its data lives in memory in `AgendaAgent/Tools.cs`. The agents
are independent on purpose. Connecting them is an optional tip in the talk.

The agents do not depend on Aspire. They read environment variables:

| Variable | Set by Aspire | Outside Aspire |
|---|---|---|
| Project endpoint | `PROJECT_GLOBALAINIGHT_URI` | `FOUNDRY_PROJECT_ENDPOINT` |
| Chat deployment | `CHAT_GLOBALAINIGHT_MODELNAME` | `CHAT_GLOBALAINIGHT_MODELNAME` |
| Port | `DEFAULT_AD_PORT` (9001 and 9002) | 8088 by default |
| Poster base URL | `POSTER_URL_BASE` or `http://localhost:{port}` | `POSTER_URL_BASE` |
| Traces | `OTEL_EXPORTER_OTLP_ENDPOINT` (Aspire dashboard) | Any OTLP endpoint |

The three demos are:

| # | What it shows | Command |
|---|---|---|
| 1 | Aspire provisions Foundry, runs both agents locally, generates a poster, and shows its `invoke_agent → chat → execute_tool GeneratePoster → /openai/v1/images/generations` trace | `aspire run` |
| 2 | Bicep generated for the Foundry account, project, and model deployments | `aspire publish -o infra` |
| 3 | The same Foundry account, project, and models provisioned in Azure (short demo) | `aspire deploy` |

## 3. Requirements

- **.NET 11 SDK** and **Aspire CLI 13.6 or later**. Install from [aspire.dev](https://aspire.dev/install/), then check with `aspire --version`.
- **Azure CLI**, signed in with `az login` to the tenant where you will create resources.
- An Azure subscription where your user can create resources and assign roles (Owner, or Contributor plus User Access Administrator at the subscription or resource-group scope).
- A region that supports `gpt-5-mini` and `gpt-image-1-mini`. The AppHost is configured for `swedencentral`; verify model availability and quota in your subscription before the talk.

NuGet packages:

| Project | Packages |
|---|---|
| `AppHost` | `Aspire.Hosting.Foundry` |
| `AgendaAgent`, `PosterAgent` | `Microsoft.Agents.AI.Foundry`, `Microsoft.Agents.AI.Foundry.Hosting` (prerelease), `Azure.Identity` |
| `ServiceDefaults` | `OpenTelemetry.*` |

## 4. Configure Azure (once)

The region, resource group, and credential source are in `src/AppHost/appsettings.json`. Set the
subscription ID in user secrets:

```powershell
cd src/AppHost
dotnet user-secrets set "Azure:SubscriptionId" "<your-subscription-id>"
```

If anything is missing, Aspire will ask for it in the dashboard when it starts.

## 5. Run locally (demo 1)

```powershell
cd src
aspire run
```

The first run provisions the Foundry account, project, and model deployments in Azure. This can take
a few minutes; later runs reuse the provisioned resources.

In the Aspire dashboard (the URL with a token appears in the console):

1. **Resources**: `foundry-globalainight`, `project-globalainight`, `chat-globalainight`, `image-globalainight`, `schedule-globalainight`, `poster-globalainight`, and `playground-globalainight`.
2. `poster-globalainight` → **Environment**: `PROJECT_GLOBALAINIGHT_URI`, `CHAT_GLOBALAINIGHT_MODELNAME`, and `IMAGE_GLOBALAINIGHT_MODELNAME`. Aspire supplies the image deployment name used by the Images API. The `schedule-globalainight` agent does not receive the image model.
3. Open `playground-globalainight` from the dashboard. It shows both agents side by side with a sample prompt; press **Enviar** on either one. The poster agent's local image is previewed in the page.
4. **Traces**: inspect `invoke_agent poster-agent-globalainight` → `chat gpt-5-mini` → `execute_tool GeneratePoster` → the Responses `image_generation` tool call.

The agents and the playground are added only in run mode, so `aspire publish` and `aspire deploy`
contain just the Foundry infrastructure.

You can also call the agents with `curl`:

```bash
curl -X POST http://localhost:9001/responses -H "Content-Type: application/json" -d '{"input": "Are there any sessions about RAG?"}'

curl -X POST http://localhost:9002/responses \
  -H "Content-Type: application/json" \
  -d '{"input": "Create a retro-style poster for the RAG session."}'
```

## 6. Inspect the infrastructure (demo 2)

```powershell
cd src
aspire publish -o infra
```

This generates Bicep for the application model in `infra/`. Inspect the Foundry account
(`disableLocalAuth: true`), project, and model deployments. Generated file
names may vary by Aspire version. `infra/` is ignored by Git because it is generated output.

## 7. Provision with `aspire deploy` (demo 3)

```powershell
cd src
aspire deploy
```

Aspire provisions the same Foundry account, project, and model deployments described in the
AppHost. The agents are not deployed: they remain regular processes that call the project's API.
Hosting them (App Service, Container Apps, etc.) is outside the scope of this demo.

**Cleanup:** delete the `rg-globalainight` resource group when finished.

## 8. Slides (MARP)

The presentation is in Spanish at `slides/deck.md`; code samples and identifiers use English.
The supplied Space Grotesk variable font is bundled in `slides/assets/fonts/` with its OFL license,
so MARP can render the deck consistently.

```powershell
cd slides
npx @marp-team/marp-cli deck.md -o deck.html --allow-local-files
npx @marp-team/marp-cli deck.md --pdf -o deck.pdf --allow-local-files
npx @marp-team/marp-cli -s .
```

Press **P** for presenter view in the HTML deck. The deck has 13 slides and reserves 23 minutes
for the three demos, with timing for a 40-minute slot:

| Minutes | Section | Slides |
|---|---|---|
| 0–0:30 | Title | 1 |
| 0:30–1:30 | About the speaker | 2 |
| 1:30–2:30 | The usual setup pain | 3 |
| 2:30–3:30 | Local development to Azure deployment | 4 |
| 3:30–5 | Aspire | 5 |
| 5–6:30 | Microsoft Foundry | 6 |
| 6:30–8 | How the pieces connect | 7 |
| 8–9 | The two demo agents | 8 |
| 9–20 | Demo 1: local run | 9 |
| 20–26 | Demo 2: generated Bicep | 10 |
| 26–32 | Demo 3: provision with `aspire deploy` | 11 |
| 32–35 | Development-to-deployment takeaways | 12 |
| 35–40 | Q&A | 13 |

## 9. Common issues

| Symptom | What to check |
|---|---|
| `aspire` command not found | Install the Aspire CLI and open a new terminal |
| Experimental API warning at build time | Add the warning ID printed by the build to `src/Directory.Build.props` |
| `AddFoundry` does not exist | Use `Aspire.Hosting.Foundry` and `using Aspire.Hosting.Foundry;` |
| `FoundryModel.OpenAI.Gpt5Mini` does not exist | Use the string overload of `AddModelDeployment` with the model version available in the Foundry catalog |
| Aspire asks for a subscription or region | Set `Azure:SubscriptionId` in user secrets; check `src/AppHost/appsettings.json` |
| Provisioning fails due to permissions | The signed-in user must be able to create resources and assign roles |
| `GetProjectOpenAIClient` does not exist | Add `using Azure.AI.Extensions.OpenAI;` or the `Azure.AI.Projects` package |
| 401 / `AuthenticationFailed` | Run `az login` in the correct tenant; the AppHost uses `CredentialSource: AzureCli` |
| 404 `DeploymentNotFound` | `CHAT_GLOBALAINIGHT_MODELNAME` and `IMAGE_GLOBALAINIGHT_MODELNAME` must be the deployment names (`chat-globalainight`, `image-globalainight`), not the catalog model IDs |
| Image deployment cannot be provisioned | Check regional availability, subscription quota, and access for the selected image model |
| Poster image generation returns 404 | The Foundry image API uses the resource endpoint `/openai/v1/images/generations` and the image deployment name from `IMAGE_GLOBALAINIGHT_MODELNAME` in the request body |
| Poster generation is slow or times out | Image generation can take several seconds; check quota if it takes more than a minute |
| Port conflict between agents | The AppHost assigns ports 9001 and 9002 with `WithHttpEndpoint` |
| 429 | Increase the model deployment's TPM quota |
| No traces in the dashboard | Check `AddServiceDefaults()`, `UseOpenTelemetry(sourceName: "AgendaAgent"/"PosterAgent")`, and `ServiceDefaults/Extensions.cs` |

The Foundry integration in Aspire and the Agent Framework hosting package are prerelease and may
change between versions. Build and run the complete demo before presenting.

## 10. References

- [Aspire Foundry integration](https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-host/)
- [Connect applications to Foundry](https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-connect/)
- [Aspire, Agent Framework, and Foundry sample](https://devblogs.microsoft.com/aspire/building-distributed-multi-agent-systems-with-aspire-and-microsoft-agent-framework/)
- [Foundry model availability by region](https://learn.microsoft.com/azure/foundry/foundry-models/concepts/models-sold-directly-by-azure-region-availability)
- [Agent Framework](https://learn.microsoft.com/agent-framework) · [GitHub repository](https://github.com/microsoft/agent-framework)

The schedule and speakers are fictional examples; the posters are AI-generated. Licensed under MIT.
