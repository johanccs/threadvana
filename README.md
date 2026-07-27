# ThreadCraft Academy

**Stop guessing what `await`, `lock`, and the thread pool do — watch them work, then prove it in the editor.**

> 🌐 **Live:** [threadcraft-academy.azurewebsites.net](https://threadcraft-academy.azurewebsites.net/)

ThreadCraft Academy is an interactive .NET multithreading course with 100 lessons, animated visual explainers, live sandbox demos, and auto-graded exercises. From your first thread to interview-ready concurrency patterns.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or newer
- PowerShell 5.1+ (Windows) or PowerShell 7+ (cross-platform)
- An [OpenRouter](https://openrouter.ai) API key *(optional — only needed for the AI Coach)*

---

## Quick Start (Local)

```powershell
# 1. Clone the repo
git clone <repo-url> threadcraft
cd threadcraft

# 2. One-click setup (checks SDK, builds, launches)
powershell -ExecutionPolicy Bypass -File setup.ps1

# 3. Open http://localhost:5080
```

**Setup script flags:**

| Flag | What it does |
|------|-------------|
| *(none)* | Build + launch in your browser |
| `-Clean` | Clean rebuild from scratch, then launch |
| `-NoBrowser` | Launch server without opening a browser |
| `-Publish` | Build for production to `./publish` (no launch) |

### Manual Start

```powershell
dotnet build ThreadCraft.slnx --configuration Release
dotnet run --project src/ThreadCraft.Web --configuration Release
```

The Setup page at `/setup` runs diagnostics to verify everything is working.

---

## AI Coach (Optional)

The AI Coach answers questions about the current lesson using OpenRouter. To enable it:

```powershell
# Set your API key as an environment variable
setx OPENROUTER_API_KEY "sk-or-v1-..."
```

Or add it to `src/ThreadCraft.Web/appsettings.json`:

```json
"Assistant": {
    "ApiKey": "sk-or-v1-...",
    "Model": "openai/gpt-oss-20b:free"
}
```

Without a key, the coach panel shows setup instructions instead.

---

## Deploy to Azure

### Option A: Azure App Service (Linux)

```bash
# 1. Publish the app
dotnet publish src/ThreadCraft.Web -c Release -o ./publish

# 2. Deploy with Azure CLI
az webapp up \
  --name threadcraft-academy \
  --resource-group threadcraft-rg \
  --plan threadcraft-plan \
  --sku B1 \
  --runtime "DOTNET:8.0" \
  --location eastus \
  --os-type Linux

# 3. Set the API key (optional)
az webapp config appsettings set \
  --name threadcraft-academy \
  --resource-group threadcraft-rg \
  --settings OPENROUTER_API_KEY="sk-or-v1-..."
```

First deployment takes ~2 minutes. The SQLite database is auto-created in the app's home directory.

### Option B: Azure App Service (Windows)

```powershell
# Publish
dotnet publish src/ThreadCraft.Web -c Release -o ./publish

# Create resources
az group create --name threadcraft-rg --location eastus
az appservice plan create --name threadcraft-plan --resource-group threadcraft-rg --sku B1
az webapp create --name threadcraft-academy --resource-group threadcraft-rg --plan threadcraft-plan --runtime "DOTNET:8.0"

# Deploy (from the publish folder)
Compress-Archive -Path .\publish\* -DestinationPath .\deploy.zip -Force
az webapp deploy --name threadcraft-academy --resource-group threadcraft-rg --src-path .\deploy.zip --type zip

# Set API key
az webapp config appsettings set --name threadcraft-academy --resource-group threadcraft-rg --settings OPENROUTER_API_KEY="sk-or-v1-..."
```

### Option C: Azure Container Apps

```bash
# Build and push the container
docker build -t threadcraft:latest .
docker tag threadcraft:latest <registry>.azurecr.io/threadcraft:latest
docker push <registry>.azurecr.io/threadcraft:latest

# Deploy
az containerapp create \
  --name threadcraft \
  --resource-group threadcraft-rg \
  --environment threadcraft-env \
  --image <registry>.azurecr.io/threadcraft:latest \
  --target-port 8080 \
  --ingress external \
  --env-vars OPENROUTER_API_KEY="sk-or-v1-..."
```

### Post-Deployment

- The app creates `threadcraft-progress.db` in the home directory automatically
- Set `ASPNETCORE_ENVIRONMENT=Production` for production logging
- Scale the App Service Plan to B2 or higher if you expect heavy traffic
- Enable **Always On** in the App Service configuration to prevent cold starts

---

## Project Structure

```
├── content/lessons/        # 100 lessons in 5 categories (Markdown + C# exercises)
├── docs/                   # Architecture, conventions, writing style
├── src/
│   ├── ThreadCraft.Core/   # Contracts: curriculum, validation, execution, progress
│   ├── ThreadCraft.Content/ # Loads & validates lesson files at startup
│   ├── ThreadCraft.Execution/ # Roslyn compiler + sandbox runner
│   ├── ThreadCraft.Sandbox/   # Isolated process that runs user code
│   └── ThreadCraft.Web/       # Blazor Server UI, AI coach, progress tracking
├── tests/                  # xUnit tests for content, execution, and web
├── setup.ps1               # One-click setup & launch
└── ThreadCraft.slnx        # Solution file
```

---

## Running Tests

```powershell
dotnet test ThreadCraft.slnx --configuration Release
```

To run only content validation tests:

```powershell
dotnet test tests/ThreadCraft.Content.Tests --filter RealContent
```

---

## License

Proprietary — all rights reserved.
