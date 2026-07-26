using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using ThreadCraft.Core.Execution;
using ThreadCraft.Core.Tracing;
using ThreadCraft.Core.Validation;

namespace ThreadCraft.Execution;

/// <summary>
/// Stage 2 of the pipeline: runs code in the ThreadCraft.Sandbox process and streams
/// back console lines, trace events and the final result (docs/architecture.md
/// §Sandbox protocol). The host enforces a hard kill at TimeoutSeconds +
/// HostKillGraceSeconds in addition to the sandbox's own watchdog.
/// </summary>
public sealed class SandboxCodeRunner : ICodeRunner
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ExecutionOptions _options;

    public SandboxCodeRunner(ExecutionOptions options) => _options = options;

    public async IAsyncEnumerable<ExecutionEvent> RunAsync(
        CodeRunRequest request,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_options.SandboxPath))
        {
            yield return new ExecutionEvent(
                ExecutionEventKind.Faulted,
                Text: $"The sandbox runner was not found at '{_options.SandboxPath}'. " +
                      "Build the ThreadCraft.Sandbox project first.");
            yield break;
        }

        var combined = CombinedSourceBuilder.BuildCombinedSource(request.Source, request.HarnessSource);
        var tempFile = Path.Combine(Path.GetTempPath(), $"threadcraft-{Guid.NewGuid():N}.cs");
        await File.WriteAllTextAsync(tempFile, combined, cancellationToken);

        var isHarness = request.HarnessSource is not null;
        using var process = StartProcess(tempFile, isHarness, request.TimeoutSeconds);

        var channel = Channel.CreateUnbounded<ExecutionEvent>();
        var readerTask = PumpStdoutAsync(process, isHarness, channel.Writer);

        // Hard kill: sandbox self-times-out at TimeoutSeconds; we add a grace period,
        // then kill the whole process tree (covers deadlocks that ignore the watchdog).
        var killAfter = TimeSpan.FromSeconds(request.TimeoutSeconds + _options.HostKillGraceSeconds);
        using var killTimer = new CancellationTokenSource();
        killTimer.CancelAfter(killAfter);
        killTimer.Token.Register(() => KillTree(process));
        await using var _ = cancellationToken.Register(() => KillTree(process));

        await foreach (var evt in channel.Reader.ReadAllAsync(CancellationToken.None))
            yield return evt;

        await readerTask; // surface reader exceptions here, after the stream drained

        killTimer.Cancel();
        try { if (!process.HasExited) KillTree(process); } catch { /* already gone */ }
        try { File.Delete(tempFile); } catch { /* best effort */ }
    }

    private Process StartProcess(string sourceFile, bool isHarness, int timeoutSeconds)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "dotnet",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = System.Text.Encoding.UTF8,
            StandardErrorEncoding = System.Text.Encoding.UTF8,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        psi.ArgumentList.Add("exec");
        psi.ArgumentList.Add(_options.SandboxPath);
        psi.ArgumentList.Add(sourceFile);
        psi.ArgumentList.Add("--entry");
        psi.ArgumentList.Add(isHarness ? "__Harness.ValidateAsync" : "Demo.RunAsync");
        psi.ArgumentList.Add("--mode");
        psi.ArgumentList.Add(isHarness ? "harness" : "demo");
        psi.ArgumentList.Add("--timeout");
        psi.ArgumentList.Add(timeoutSeconds.ToString());

        return Process.Start(psi)
            ?? throw new InvalidOperationException("Failed to start the sandbox process.");
    }

    /// <summary>
    /// Reads sandbox stdout line by line, translates the line protocol into events,
    /// and completes the channel when the process closes its output (exit or death).
    /// </summary>
    private static async Task PumpStdoutAsync(
        Process process, bool isHarness, ChannelWriter<ExecutionEvent> writer)
    {
        var console = new StringBuilder();
        var traces = new List<TraceEvent>();
        var sawResult = false;

        try
        {
            string? line;
            while ((line = await process.StandardOutput.ReadLineAsync()) is not null)
            {
                if (line.StartsWith("OUT|", StringComparison.Ordinal))
                {
                    var text = line[4..];
                    console.AppendLine(text);
                    await writer.WriteAsync(new ExecutionEvent(ExecutionEventKind.ConsoleLine, Text: text));
                }
                else if (line.StartsWith("TRACE|", StringComparison.Ordinal))
                {
                    var trace = TryParseTrace(line[6..]);
                    if (trace is not null)
                    {
                        traces.Add(trace);
                        await writer.WriteAsync(new ExecutionEvent(ExecutionEventKind.Trace, Trace: trace));
                    }
                }
                else if (line.StartsWith("RESULT|", StringComparison.Ordinal))
                {
                    sawResult = true;
                    var evt = BuildCompletion(line[7..], isHarness, console.ToString(), traces);
                    await writer.WriteAsync(evt);
                }
                else if (line.Length > 0)
                {
                    // Anything unprefixed (should not happen) is shown as console output.
                    console.AppendLine(line);
                    await writer.WriteAsync(new ExecutionEvent(ExecutionEventKind.ConsoleLine, Text: line));
                }
            }

            if (!sawResult)
            {
                var stderr = await process.StandardError.ReadToEndAsync();
                var detail = string.IsNullOrWhiteSpace(stderr)
                    ? "The sandbox process ended without producing a result."
                    : $"The sandbox failed: {Trim(stderr)}";
                await writer.WriteAsync(new ExecutionEvent(ExecutionEventKind.Faulted, Text: detail));
            }

            writer.Complete();
        }
        catch (Exception ex)
        {
            writer.Complete(ex);
        }
    }

    private static TraceEvent? TryParseTrace(string json)
    {
        try { return JsonSerializer.Deserialize<TraceEvent>(json, JsonOptions); }
        catch (JsonException) { return null; }
    }

    private static void KillTree(Process process)
    {
        try { if (!process.HasExited) process.Kill(entireProcessTree: true); }
        catch { /* already exited */ }
    }

    private static string Trim(string text) =>
        text.Length <= 500 ? text.Trim() : text[..500].Trim() + " …";

    /// <summary>Maps the RESULT envelope to a Completed (or Faulted) event.</summary>
    private static ExecutionEvent BuildCompletion(
        string json, bool isHarness, string consoleOutput, List<TraceEvent> traces)
    {
        ResultEnvelope envelope;
        try
        {
            envelope = JsonSerializer.Deserialize<ResultEnvelope>(json, JsonOptions)
                ?? new ResultEnvelope();
        }
        catch (JsonException ex)
        {
            return new ExecutionEvent(
                ExecutionEventKind.Faulted,
                Text: $"Could not read the sandbox result: {ex.Message}");
        }

        var duration = TimeSpan.FromMilliseconds(envelope.DurationMs);
        var traceCopy = traces.ToArray();

        switch (envelope.Status)
        {
            case "passed":
            case "tests-failed":
                return new ExecutionEvent(ExecutionEventKind.Completed, Result: new ValidationResult
                {
                    Status = envelope.Status == "passed" ? ValidationStatus.Passed : ValidationStatus.TestsFailed,
                    Checks = envelope.Checks.Select(c => new ValidationCheck(
                        c.Name, c.Passed, c.Expected, c.Actual, c.Message)).ToList(),
                    ConsoleOutput = consoleOutput,
                    TraceEvents = traceCopy,
                    Duration = duration
                });

            case "runtime-error":
                return new ExecutionEvent(ExecutionEventKind.Completed, Result: new ValidationResult
                {
                    Status = ValidationStatus.RuntimeError,
                    ConsoleOutput = consoleOutput,
                    TraceEvents = traceCopy,
                    RuntimeErrorMessage = envelope.Error ?? "Your code crashed while running.",
                    Duration = duration
                });

            case "timeout":
                return new ExecutionEvent(ExecutionEventKind.Completed, Result: new ValidationResult
                {
                    Status = ValidationStatus.RuntimeError,
                    ConsoleOutput = consoleOutput,
                    TraceEvents = traceCopy,
                    RuntimeErrorMessage =
                        "Your code did not finish in time. This usually means a deadlock " +
                        "(two threads waiting on each other forever) or a loop that never ends.",
                    Duration = duration
                });

            case "compile-error":
                // Stage 1 should have caught this — if it reached here, something drifted.
                return new ExecutionEvent(
                    ExecutionEventKind.Faulted,
                    Text: $"The sandbox could not compile the code: {envelope.Error}");

            default:
                return isHarness
                    ? new ExecutionEvent(ExecutionEventKind.Faulted, Text: $"Unknown sandbox status '{envelope.Status}'.")
                    : new ExecutionEvent(ExecutionEventKind.Completed, Result: new ValidationResult
                    {
                        Status = ValidationStatus.Passed, // demo mode: nothing to assert
                        ConsoleOutput = consoleOutput,
                        TraceEvents = traceCopy,
                        Duration = duration
                    });
        }
    }

    /// <summary>The sandbox's final RESULT| JSON line (camelCase properties).</summary>
    private sealed class ResultEnvelope
    {
        public string Status { get; set; } = "";
        public List<EnvelopeCheck> Checks { get; set; } = [];
        public string? Error { get; set; }
        public long DurationMs { get; set; }
    }

    private sealed class EnvelopeCheck
    {
        public string Name { get; set; } = "";
        public bool Passed { get; set; }
        public string Expected { get; set; } = "";
        public string Actual { get; set; } = "";
        public string Message { get; set; } = "";
    }
}

