# ThreadCraft Academy

**Stop guessing what `await`, `lock`, and the thread pool do — watch them work, then prove it in the editor.**

> 🌐 **Live:** [threadcraft-academy.azurewebsites.net](https://threadcraft-academy.azurewebsites.net/) · **About:** [johanccs.github.io/threadvana](https://johanccs.github.io/threadvana/)

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

## Deployment

This app deploys automatically via GitHub Actions on every push to `main` — see
[`.github/workflows/ci.yml`](.github/workflows/ci.yml) for the pipeline (build, test,
publish, deploy). No manual steps are required.

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
