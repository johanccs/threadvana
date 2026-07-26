# Code Conventions (all C# in this repo)

- Target framework **net8.0**, C# 12, `<Nullable>enable</Nullable>`, implicit usings.
- **File-scoped namespaces** (`namespace ThreadCraft.Core;`), one public type per file,
  file name = type name.
- **Records** for data carriers (`LessonDefinition`, `TraceEvent`…), `required` for
  mandatory properties, `IReadOnlyList<T>`/`IReadOnlySet<T>` in public APIs.
- Async methods end in `Async` and take `CancellationToken cancellationToken = default`
  when cancellable. Never `async void` (except Blazor event handlers).
- XML doc comments on **public** APIs (one or two sentences, plain English).
  Comments explain *why*, not *what*. Junior audience: prefer one extra clarifying
  comment over one clever line.
- No `#region`. No abbreviations (`lesson`, not `lsn`). No Hungarian notation.
- Errors: throw specific exceptions with messages that say what was expected and
  what was found. Content problems throw `ContentLoadException` (fail fast at startup).
- Blazor: components end in `.razor` under `Components/`; parameters use
  `[Parameter] public required ...`; event callbacks use `EventCallback<T>`.
  Long-running work is streamed via `IAsyncEnumerable` + `await foreach`, never polled.
- Tests: xUnit, `[Fact]`/`[Theory]`, method names `What_is_tested__Expected_behavior`
  style variations are fine as long as they read like a sentence. No test may depend
  on wall-clock sleeps shorter than 50 ms or on machine core count.
- Formatting: `dotnet format` defaults (4-space indent, Allman braces for C#,
  K&R inside Razor markup is acceptable).

## Layering (who may reference whom)

```
ThreadCraft.Web ──► ThreadCraft.Content ──► ThreadCraft.Core
        │                                     ▲
        └──────► ThreadCraft.Execution ───────┘
ThreadCraft.Web ──► ThreadCraft.Sandbox (project ref, runs as separate process)
ThreadCraft.Sandbox: STANDALONE — references NOTHING in this repo.
```

- Core contains **only contracts and models** — no I/O, no Roslyn, no ASP.NET.
- Sandbox must stay dependency-free (besides Roslyn) because it is spawned as a
  separate process and must start fast.
