using Xunit;

namespace ThreadCraft.Execution.Tests;

/// <summary>
/// All tests that spawn sandbox processes share this collection so they run
/// serially (keeps CPU and temp files predictable).
/// </summary>
[CollectionDefinition("Sandbox")]
public sealed class SandboxCollection;
