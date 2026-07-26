// ThreadCraft.Sandbox — runs one submitted C# program in isolation.
// Protocol (docs/architecture.md §Sandbox protocol):
//   dotnet ThreadCraft.Sandbox.dll <sourceFile> --entry <Type.Method> --mode <harness|demo> --timeout <seconds>
//   stdout lines: OUT|<line>  TRACE|<json>  RESULT|<json exactly once, last>
//   exit codes: 0 = ran, 1 = infra failure, 2 = self-timeout
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using System.Text.Json;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

return await SandboxRunner.RunAsync(args);

internal static class SandboxRunner
{
    private static int _resultWritten; // 0 = not yet, 1 = written (Interlocked)

    public static async Task<int> RunAsync(string[] args)
    {
        try
        {
            var options = ParseArgs(args);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var source = await File.ReadAllTextAsync(options.SourceFile);
            var compilation = Compile(source);

            if (!compilation.Success)
            {
                EmitResult(new
                {
                    status = "compile-error",
                    error = string.Join("\n", compilation.Diagnostics),
                    durationMs = stopwatch.ElapsedMilliseconds
                });
                return 0;
            }

            RedirectConsole();

            // Watchdog: if user code overruns, report timeout and die fast.
            var timeoutMs = options.TimeoutSeconds * 1000L;
            _ = Task.Run(async () =>
            {
                await Task.Delay((int)timeoutMs);
                if (Interlocked.CompareExchange(ref _resultWritten, 1, 0) == 0)
                {
                    WriteResultLine(new
                    {
                        status = "timeout",
                        error = $"The code did not finish within {options.TimeoutSeconds} seconds.",
                        durationMs = timeoutMs
                    });
                }
                Environment.Exit(2);
            });

            return await ExecuteEntryPointAsync(options, compilation.Image!, stopwatch);
        }
        catch (Exception ex)
        {
            await Console.Error.WriteLineAsync($"sandbox infra failure: {ex}");
            return 1;
        }
    }

    private static async Task<int> ExecuteEntryPointAsync(
        SandboxOptions options, byte[] image, System.Diagnostics.Stopwatch stopwatch)
    {
        var alc = new AssemblyLoadContext("submission", isCollectible: true);
        try
        {
            Assembly assembly;
            using (var ms = new MemoryStream(image))
                assembly = alc.LoadFromStream(ms);

            var type = assembly.GetType(options.EntryType)
                ?? throw new InvalidOperationException($"Entry type '{options.EntryType}' not found.");
            var method = type.GetMethod(options.EntryMethod, BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException($"Entry method '{options.EntryMethod}' not found on '{options.EntryType}'.");

            object? returnValue;
            try
            {
                returnValue = method.Invoke(null, null);
                if (returnValue is Task task)
                {
                    await task;
                    if (options.Mode == "harness")
                        returnValue = task.GetType().GetProperty("Result")?.GetValue(task);
                }
            }
            catch (TargetInvocationException tie) when (tie.InnerException is not null)
            {
                return EmitRuntimeError(tie.InnerException, stopwatch);
            }
            catch (Exception ex) // async exceptions surface directly
            {
                return EmitRuntimeError(ex, stopwatch);
            }

            if (options.Mode == "harness")
                return EmitHarnessResult(returnValue, stopwatch);

            EmitResult(new { status = "completed", durationMs = stopwatch.ElapsedMilliseconds });
            return 0;
        }
        finally
        {
            alc.Unload();
        }
    }

    // HarnessResult is defined in the injected prelude (unknown at compile time here),
    // so its Checks are read via reflection.
    private static int EmitHarnessResult(object? harnessResult, System.Diagnostics.Stopwatch stopwatch)
    {
        if (harnessResult is null)
            return EmitRuntimeError(new InvalidOperationException("The harness returned no result."), stopwatch);

        var checks = new List<Dictionary<string, object?>>();
        var rawChecks = harnessResult.GetType().GetProperty("Checks")?.GetValue(harnessResult)
            as System.Collections.IEnumerable;

        var allPassed = true;
        if (rawChecks is not null)
        {
            foreach (var check in rawChecks)
            {
                var t = check.GetType();
                var passed = (bool)(t.GetProperty("Passed")?.GetValue(check) ?? false);
                allPassed &= passed;
                checks.Add(new Dictionary<string, object?>
                {
                    ["name"] = t.GetProperty("Name")?.GetValue(check)?.ToString() ?? "",
                    ["passed"] = passed,
                    ["expected"] = t.GetProperty("Expected")?.GetValue(check)?.ToString() ?? "",
                    ["actual"] = t.GetProperty("Actual")?.GetValue(check)?.ToString() ?? "",
                    ["message"] = t.GetProperty("Message")?.GetValue(check)?.ToString() ?? ""
                });
            }
        }

        EmitResult(new
        {
            status = allPassed && checks.Count > 0 ? "passed" : "tests-failed",
            checks,
            durationMs = stopwatch.ElapsedMilliseconds
        });
        return 0;
    }

    private static int EmitRuntimeError(Exception ex, System.Diagnostics.Stopwatch stopwatch)
    {
        EmitResult(new
        {
            status = "runtime-error",
            error = $"{ex.GetType().Name}: {ex.Message}",
            durationMs = stopwatch.ElapsedMilliseconds
        });
        return 0;
    }

    private static void EmitResult(object payload)
    {
        if (Interlocked.CompareExchange(ref _resultWritten, 1, 0) != 0) return; // exactly once
        WriteResultLine(payload);
        Environment.Exit(0); // do not linger on stray foreground threads the user started
    }

    private static void WriteResultLine(object payload)
    {
        var json = JsonSerializer.Serialize(payload, ResultJsonOptions);
        RealStdout.WriteLine("RESULT|" + json);
        RealStdout.Flush();
    }

    private static readonly JsonSerializerOptions ResultJsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // ---------------- compilation ----------------

    private static (bool Success, byte[]? Image, List<string> Diagnostics) Compile(string source)
    {
        var tree = CSharpSyntaxTree.ParseText(
            source,
            new CSharpParseOptions(LanguageVersion.CSharp12),
            path: "submission");

        var tpa = (string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!;
        var references = tpa.Split(Path.PathSeparator)
            .Where(p => p.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            .Select(p => (MetadataReference)MetadataReference.CreateFromFile(p))
            .ToList();

        var compilation = CSharpCompilation.Create(
            "Submission_" + Guid.NewGuid().ToString("N"),
            [tree],
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                nullableContextOptions: NullableContextOptions.Disable));

        using var peStream = new MemoryStream();
        var emit = compilation.Emit(peStream);

        if (!emit.Success)
        {
            var errors = emit.Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => $"{d.Id}: {d.GetMessage()}")
                .ToList();
            return (false, null, errors);
        }
        return (true, peStream.ToArray(), []);
    }

    // ---------------- console redirect ----------------

    // The untainted stdout stream, captured before any redirect. RESULT| lines are
    // always written here, so a misbehaving user writer can never swallow them.
    private static readonly TextWriter RealStdout =
        new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };

    private static void RedirectConsole()
    {
        Console.SetOut(new PrefixingWriter(RealStdout));
    }

    /// <summary>
    /// Prefixes every completed user line with OUT| — except lines already starting
    /// with TRACE|, which pass through untouched (they carry viz events).
    /// </summary>
    private sealed class PrefixingWriter : TextWriter
    {
        private readonly TextWriter _inner;
        private readonly object _gate = new();
        private string _pending = "";

        public PrefixingWriter(TextWriter inner) => _inner = inner;
        public override Encoding Encoding => Encoding.UTF8;

        public override void Write(char value)
        {
            lock (_gate)
            {
                if (value == '\n') FlushLine();
                else if (value != '\r') _pending += value;
            }
        }

        public override void Write(string? value)
        {
            if (value is null) return;
            lock (_gate)
            {
                foreach (var c in value)
                {
                    if (c == '\n') FlushLine();
                    else if (c != '\r') _pending += c;
                }
            }
        }

        public override void Flush()
        {
            lock (_gate) FlushLine();
            _inner.Flush();
        }

        private void FlushLine()
        {
            if (_pending.Length == 0) return;
            var line = _pending;
            _pending = "";
            _inner.WriteLine(line.StartsWith("TRACE|", StringComparison.Ordinal) ? line : "OUT|" + line);
        }
    }

    // ---------------- args ----------------

    private static SandboxOptions ParseArgs(string[] args)
    {
        if (args.Length < 1)
            throw new ArgumentException("usage: sandbox <sourceFile> --entry <Type.Method> --mode <harness|demo> --timeout <seconds>");

        var sourceFile = args[0];
        var entry = GetArg("--entry") ?? "Demo.RunAsync";
        var mode = GetArg("--mode") ?? "demo";
        var timeout = int.TryParse(GetArg("--timeout"), out var t) ? t : 10;

        var dot = entry.LastIndexOf('.');
        if (dot <= 0) throw new ArgumentException($"--entry must be Type.Method, got '{entry}'.");

        return new SandboxOptions(sourceFile, entry[..dot], entry[(dot + 1)..], mode, timeout);

        string? GetArg(string name)
        {
            for (var i = 1; i < args.Length - 1; i++)
                if (args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                    return args[i + 1];
            return null;
        }
    }

    private sealed record SandboxOptions(
        string SourceFile, string EntryType, string EntryMethod, string Mode, int TimeoutSeconds);
}

