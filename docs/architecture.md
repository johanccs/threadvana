# Architecture

## Projects & data flow

```
Browser (Blazor Server, InteractiveServer render mode)
   │  circuit (SignalR) — UI events + streamed execution events
ThreadCraft.Web
   ├─ ICurriculumService        ← ThreadCraft.Content (loads content/lessons at startup, singleton)
   ├─ IProgressStore            ← EF Core + SQLite (scoped), file: threadcraft-progress.db
   ├─ IExerciseValidator        ← ThreadCraft.Execution (Stage 1: Roslyn compile → Stage 2: sandbox)
   └─ ICodeRunner               ← ThreadCraft.Execution (spawns sandbox process, streams events)
ThreadCraft.Sandbox              ← separate process per run; compiles combined source, executes, streams
```

Contracts live in **ThreadCraft.Core** (`Curriculum/`, `Validation/`, `Execution/`,
`Tracing/`, `Progress/`). Read those files first — they are frozen.

## The "check my code" pipeline (two stages)

1. **Stage 1 — validity** (`RoslynExerciseValidator`, in-process):
   concatenate `prelude + userCode + harnessCode` with `#line 1 "user-code"` /
   `#line 1 "harness"` markers, compile with Roslyn against TPA references
   (see §Compiling). Only diagnostics from the **user-code** tree are reported as
   `CompileIssue`s (errors → `ValidationStatus.CompileError`; warnings are included
   with severity "warning" but do not block Stage 2). Each issue gets a
   junior-friendly `FriendlyMessage` (see §Friendly diagnostics).
2. **Stage 2 — correctness** (`SandboxCodeRunner` spawns `ThreadCraft.Sandbox`):
   writes the combined source to a temp file, spawns the process, streams lines,
   parses the final `RESULT|` envelope into `ValidationResult`. The host kills the
   process `TimeoutSeconds + 5` after start if it is still alive (deadlock safety).

## Sandbox protocol (exact — implement byte-for-byte)

**Invocation:** `dotnet "<SandboxPath>" <combinedSourceFile> --entry <Type.Method> --mode <harness|demo> --timeout <seconds>`
- `--entry __Harness.ValidateAsync` for exercises, `Demo.RunAsync` for demos.

**Stdout lines** (single stream, line-prefixed):
- `OUT|<text>` — one captured user console line (forwarded live).
- `TRACE|<json>` — one trace event, json with PascalCase props matching
  `ThreadCraft.Core.Tracing.TraceEvent` (TimestampMs, ThreadId, ThreadName, Kind, Label).
- `RESULT|<json>` — EXACTLY ONCE, last line before exit:
  `{"status":"passed"|"tests-failed"|"runtime-error"|"timeout"|"compile-error",
    "checks":[{"name","passed","expected","actual","message"}],
    "error":"...when runtime-error/compile-error...",
    "durationMs":1234}`
  Deserialize case-insensitively. `checks[].message` maps to `ValidationCheck.FriendlyMessage`.

**Exit codes:** 0 = ran (regardless of pass/fail), 1 = sandbox infra failure, 2 = self-timeout.

**Sandbox internals:**
1. Read combined source; compile (§Compiling) into an in-memory PE.
2. Compile failure → emit `RESULT|{"status":"compile-error",...}` with diagnostics, exit 0.
3. Redirect `Console.SetOut` to a thread-safe writer: lines starting with `TRACE|`
   pass through to real stdout; anything else is written as `OUT|<line>`.
   (Partial writes: buffer per-thread until newline; keep it simple with a lock.)
4. Load the PE into a **collectible `AssemblyLoadContext`**, find the entry type/method,
   invoke; if it returns `Task`, await it. Start a watchdog Task before invocation:
   at `--timeout`, emit `RESULT|{"status":"timeout"}` (once!) and `Environment.Exit(2)`.
5. Entry threw → `RESULT|{"status":"runtime-error","error":"<ex.GetType().Name>: <ex.Message>"}`.
6. Harness mode: entry returned `HarnessResult` → map to the RESULT envelope
   (`passed` if `result.Passed`, else `tests-failed`).
7. Demo mode: clean return → `RESULT|{"status":"completed"}`.
8. Any infrastructure exception → write to stderr, exit 1.

**Security note (intentional, documented):** the sandbox is process-isolated with a
timeout — a *learning* sandbox, not a hardened multi-tenant boundary. No restricted
API surface is enforced.

## Compiling (same approach in Execution and Sandbox)

```csharp
var tpa = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
var references = tpa.Split(Path.PathSeparator)
    .Where(p => p.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
    .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
    .ToList();

var compilation = CSharpCompilation.Create(
    assemblyName: "Submission_" + Guid.NewGuid().ToString("N"),
    syntaxTrees: trees,                       // parse with LanguageVersion.CSharp12
    references: references,
    options: new CSharpCompilationOptions(
        OutputKind.DynamicallyLinkedLibrary,
        nullableContextOptions: NullableContextOptions.Disable)); // less noise for juniors
```

Emit to a `MemoryStream`; `EmitResult.Diagnostics` = full semantic diagnostics.
(Tradeoff: TPA gives implementation assemblies, not ref packs — acceptable here.)

## Prelude (injected source — EXACTLY this code; Execution owns it as an embedded resource)

```csharp
// NOTE: no `using System.Diagnostics` / `System.Text.Json` here on purpose — types are
// fully qualified. Hoisted into the combined file, those usings collide with learner
// usings (bare `ThreadState` is ambiguous between System.Diagnostics and System.Threading).
using System;
using System.Collections.Generic;
using System.Threading;

public static class Trace
{
    private static readonly System.Diagnostics.Stopwatch _clock = System.Diagnostics.Stopwatch.StartNew();
    private static readonly object _gate = new();

    public static void Log(string kind, string label)
    {
        var t = Thread.CurrentThread;
        var payload = System.Text.Json.JsonSerializer.Serialize(new Dictionary<string, object>
        {
            ["TimestampMs"] = _clock.ElapsedMilliseconds,
            ["ThreadId"] = t.ManagedThreadId,
            ["ThreadName"] = string.IsNullOrEmpty(t.Name) ? "Thread " + t.ManagedThreadId : t.Name!,
            ["Kind"] = kind,
            ["Label"] = label
        });
        lock (_gate) Console.WriteLine("TRACE|" + payload);
    }
}

public sealed class HarnessCheck
{
    public string Name { get; set; } = "";
    public bool Passed { get; set; }
    public string Expected { get; set; } = "";
    public string Actual { get; set; } = "";
    public string Message { get; set; } = "";
}

public sealed class HarnessResult
{
    public List<HarnessCheck> Checks { get; } = new();
    public bool Passed => Checks.TrueForAll(c => c.Passed);

    public void Add(string name, bool passed, string expected, string actual, string message)
        => Checks.Add(new HarnessCheck
        {
            Name = name, Passed = passed, Expected = expected,
            Actual = actual, Message = message
        });
}
```

Combined source = all using-directives (from prelude+user+harness) hoisted to the
top, deduplicated, then each part under a `#line 1 "<path>"` marker (a single file
forbids mid-file usings — hoisting is mandatory). Stage 1 instead parses each part
as its own syntax tree with the path set, so diagnostics carry the right path/line.
Demos: prelude + demoCode under path "demo".

## Friendly diagnostics (Stage 1)

Map common Roslyn ids to junior English (keep `RawMessage` alongside):
CS1002 "; expected" → "A statement is missing its semicolon at the end.";
CS0246 "type or namespace not found" → "This name is unknown — check the spelling or add a using at the top.";
CS1061 "no definition for" → "That method/property does not exist on this type — check the spelling.";
CS0029 "cannot implicitly convert" → "You are putting one type of value into a variable of a different type.";
CS4032/CS4033 "await in non-async" → "You used await inside a method that is not marked async — add async and return Task.";
fallback: "The compiler says: <raw>". Keep the table easy to extend (a Dictionary + formatter lambdas).

## Frozen implementation signatures (both platform & UI code against these)

```csharp
namespace ThreadCraft.Execution;

public sealed record ExecutionOptions
{
    /// <summary>Full path to ThreadCraft.Sandbox.dll.</summary>
    public required string SandboxPath { get; init; }
    /// <summary>Extra seconds the host waits after TimeoutSeconds before killing the process.</summary>
    public int HostKillGraceSeconds { get; init; } = 5;
}

public sealed class SandboxCodeRunner : ThreadCraft.Core.Execution.ICodeRunner
{
    public SandboxCodeRunner(ExecutionOptions options);
    public IAsyncEnumerable<ThreadCraft.Core.Execution.ExecutionEvent> RunAsync(
        ThreadCraft.Core.Execution.CodeRunRequest request,
        CancellationToken cancellationToken = default);
}

public sealed class RoslynExerciseValidator : ThreadCraft.Core.Validation.IExerciseValidator
{
    public RoslynExerciseValidator(ThreadCraft.Core.Execution.ICodeRunner runner);
    public Task<ThreadCraft.Core.Validation.ValidationResult> ValidateAsync(
        string userCode, ThreadCraft.Core.Curriculum.ExerciseDefinition exercise,
        CancellationToken cancellationToken = default);
}
```

DI registration (done by the Web layer):

```csharp
services.AddSingleton(new ExecutionOptions { SandboxPath = Path.Combine(AppContext.BaseDirectory, "ThreadCraft.Sandbox.dll") });
services.AddSingleton<ICodeRunner, SandboxCodeRunner>();
services.AddSingleton<IExerciseValidator, RoslynExerciseValidator>();
```

