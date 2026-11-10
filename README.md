# From code to Foundry: agent infrastructure with Aspire

> Microsoft Agent Framework + Microsoft Foundry + Aspire, without portal clicks or deployment scripts
> **Global AI Night 2026** · Online · November 12, 2026 · 40 minutes (35 + 5 for Q&A)
> Speaker: **Emiliano Montesdeoca del Puerto**, Cloud Solutions Team Lead @ Intelequia

This repository contains the talk slides (MARP) and a small .NET demo that moves two agents from a
local machine to Microsoft Foundry.

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
    ├── AppHost/                   # Foundry, project, two model deployments, and agents
    ├── ServiceDefaults/           # OpenTelemetry and health checks
    ├── AgendaAgent/               # gpt-5-mini + SearchSessions
    └── PosterAgent/               # gpt-5-mini + GeneratePoster (gpt-image-1-mini)
```

## 1. The talk in a nutshell

Writing agents with Microsoft Agent Framework is the easy part. Teams also need to create the
Foundry account and project, deploy models, grant access without sharing keys, inspect agent traces,
and deploy without relying on one person. This demo starts with two small .NET agents—one recommends
sessions using `gpt-5-mini`, and one creates session posters with an image model—and publishes them
to Foundry as hosted agents. An Aspire AppHost describes the infrastructure and connects each agent
only to the resources it needs.

The two commands are `aspire run` for local development (using real Foundry resources in Azure) and
`aspire deploy` to publish the agents.

**Agent Framework writes the agents, Foundry hosts them, and Aspire describes everything around them.**

## 2. The demo: two agents, two models

The demo is intentionally small: two independent agents, one tool per agent, and one AppHost.

| Resource | What it is | Where it runs |
|---|---|---|
| `foundry` | Microsoft Foundry account; local auth is disabled and Aspire creates the required roles | Azure |
| `project` | Foundry project and destination for the hosted agents | Azure |
| `chat` | `gpt-5-mini` deployment | Azure |
| `image` | `gpt-image-1-mini` deployment (`2025-10-06`) | Azure |
| `schedule` | Recommends sessions from sample data using `chat` and `SearchSessions` | Local with `aspire run`; Foundry with `aspire deploy` |
| `poster` | Uses `chat` to write an image description, then calls `GeneratePoster` to invoke `image` | Local with `aspire run`; Foundry with `aspire deploy` |

The image model does not chat or call tools: it receives a description and returns an image. The
`gpt-5-mini` agent understands the request, writes the description, and decides when to call the
image-generation tool. The poster agent saves its PNG in `posters/` and serves it from
`/posters/{fileName}` locally.

The sample schedule is **fictional**; its data lives in memory in `AgendaAgent/Tools.cs`. The agents
are independent on purpose. Connecting them is an optional tip in the talk.

The agents do not depend on Aspire. They read environment variables:

| Variable | Set locally by Aspire | Set in Foundry |
|---|---|---|
| Project endpoint | `PROJECT_URI` | `FOUNDRY_PROJECT_ENDPOINT` |
| Chat deployment | `CHAT_MODELNAME` | `CHAT_MODELNAME` (or `AZURE_AI_MODEL_DEPLOYMENT_NAME`) |
| Image deployment (poster only) | `IMAGE_MODELNAME` | `IMAGE_MODELNAME` |
| Port | `DEFAULT_AD_PORT` (9001 and 9002) | 8088 by default |
| Poster base URL | `POSTER_URL_BASE` or `http://localhost:{port}` | Configure a reachable storage or web URL before sharing images |
| Traces | `OTEL_EXPORTER_OTLP_ENDPOINT` (Aspire dashboard) | Application Insights |

The three demos are:

| # | What it shows | Command |
|---|---|---|
| 1 | Aspire provisions Foundry, runs both agents locally, generates a poster, and shows its `invoke_agent → chat → execute_tool GeneratePoster → images/generations` trace | `aspire run` |
| 2 | Bicep generated for the Foundry account, project, model deployments, and role assignments | `aspire publish -o infra` |
| 3 | The same agents as hosted agents in Foundry, with traces in Application Insights | `aspire deploy` |

## 3. Requirements

- **.NET 10 SDK** and **Aspire CLI 13.6 or later**. Install from [aspire.dev](https://aspire.dev/install/), then check with `aspire --version`.
- **Azure CLI**, signed in with `az login` to the tenant where you will create resources.
- **Docker** (or Podman) to build the agent images for demo 3.
- An Azure subscription where your user can create resources and assign roles (Owner, or Contributor plus User Access Administrator at the subscription or resource-group scope).
- **Foundry Project Manager** permissions on the project to publish hosted agents. If Aspire creates the project using your Owner identity, you already have the required access.
- A region that supports `gpt-5-mini`, `gpt-image-1-mini`, and hosted agents. The AppHost is configured for `swedencentral`; verify model availability and quota in your subscription before the talk.

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

1. **Resources**: `foundry`, `project`, `chat`, `image`, `schedule`, and `poster`.
2. `poster` → **Environment**: `PROJECT_URI`, `CHAT_MODELNAME`, and `IMAGE_MODELNAME`. The `schedule` agent does not receive the image deployment.
3. `schedule` → **Send Message**: *"I'm interested in deployment. Which session should I attend?"*
4. `poster` → **Send Message**: *"Create a poster for the Aspire session."* It takes longer; open the returned `http://localhost:9002/posters/poster-HHmmss.png` link.
5. **Traces**: inspect `invoke_agent poster-agent` → `chat gpt-5-mini` → `execute_tool GeneratePoster` → the HTTP call to `images/generations`.

You can also call the agents with `curl`:

```bash
curl -X POST http://localhost:9001/responses \
  -H "Content-Type: application/json" \
  -d '{"input": "Are there any sessions about RAG?"}'

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
(`disableLocalAuth: true`), project, model deployments, and agent role assignments. Generated file
names may vary by Aspire version. `infra/` is ignored by Git because it is generated output.

## 7. Deploy to Foundry (demo 3)

```powershell
cd src
aspire deploy
```

Aspire builds both agent images, pushes them to the project's container registry, and creates the
hosted agents in the Foundry project.

In the Foundry portal, open **Agents** → `schedule` → **Playground**. Traces appear under
**Observability → Tracing** (the project's Application Insights) after a minute or two.

The poster agent also runs after deployment, but the PNG is stored in the agent's session and its
local URL is not publicly reachable. To share posters from Foundry, add Blob Storage to the AppHost
and upload the PNG there. This is intentionally outside the minimal demo.

**Cleanup:** delete the `rg-global-ai-night-2026` resource group when finished.

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
| 2:30–3:30 | Proposed stack | 4 |
| 3:30–5 | Aspire | 5 |
| 5–6:30 | Microsoft Foundry | 6 |
| 6:30–8 | How the pieces connect | 7 |
| 8–9 | The two demo agents | 8 |
| 9–19 | Demo 1: local run | 9 |
| 19–22 | Demo 2: generated Bicep | 10 |
| 22–32 | Demo 3: deploy to Foundry | 11 |
| 32–35 | Takeaways | 12 |
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
| 404 `DeploymentNotFound` | `CHAT_MODELNAME` and `IMAGE_MODELNAME` must be the deployment names (`chat`, `image`), not the model names |
| Image deployment cannot be provisioned | Check regional availability, subscription quota, and access for the selected image model |
| `GetImageClient` is unavailable | Use the OpenAI SDK image client with a `BearerTokenPolicy` and the Foundry project endpoint |
| Poster generation is slow or times out | Image generation can take several seconds; check quota if it takes more than a minute |
| Port conflict between agents | The AppHost assigns ports 9001 and 9002 with `WithHttpEndpoint` |
| 429 | Increase the model deployment's TPM quota |
| No traces in the dashboard | Check `AddServiceDefaults()`, `UseOpenTelemetry(sourceName: "AgendaAgent"/"PosterAgent")`, and `ServiceDefaults/Extensions.cs` |
| `aspire deploy` fails while pushing images | Start Docker and verify that the project registry is accessible |
| A deployed agent does not start | Check the hosted agent logs in Foundry; common causes are a missing environment variable or role |

The Foundry integration in Aspire and the Agent Framework hosting package are prerelease and may
change between versions. Build and run the complete demo before presenting.

## 10. References

- [Aspire Foundry integration](https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-host/)
- [Connect applications to Foundry](https://aspire.dev/integrations/cloud/azure/azure-ai-foundry/azure-ai-foundry-connect/)
- [Aspire, Agent Framework, and Foundry sample](https://devblogs.microsoft.com/aspire/building-distributed-multi-agent-systems-with-aspire-and-microsoft-agent-framework/)
- [Agent Framework hosted agents](https://learn.microsoft.com/agent-framework/hosting/foundry-hosted-agent)
- [Deploy a hosted agent](https://learn.microsoft.com/azure/foundry/agents/how-to/deploy-hosted-agent)
- [Foundry model availability by region](https://learn.microsoft.com/azure/foundry/foundry-models/concepts/models-sold-directly-by-azure-region-availability)
- [Agent Framework](https://learn.microsoft.com/agent-framework) · [GitHub repository](https://github.com/microsoft/agent-framework)

The schedule and speakers are fictional examples; the posters are AI-generated. Licensed under MIT.
