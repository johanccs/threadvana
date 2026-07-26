namespace ThreadCraft.Execution;

/// <summary>
/// The source injected into every submission (demos and exercises).
/// Provides Trace (viz events) and HarnessResult/HarnessCheck (validation).
/// Keep byte-exact with docs/architecture.md §Prelude.
/// </summary>
public static class Prelude
{
    // NOTE: the prelude deliberately avoids `using System.Diagnostics` / `System.Text.Json`
    // (types are fully qualified instead). Those usings would be hoisted into the combined
    // sandbox file and can collide with the learner's own usings — e.g. bare `ThreadState`
    // becomes ambiguous between System.Diagnostics and System.Threading (CS0104).
    public const string Source = """
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
        """;
}
