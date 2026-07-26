namespace ThreadCraft.Execution;

/// <summary>Configuration for the execution pipeline. See docs/architecture.md.</summary>
public sealed record ExecutionOptions
{
    /// <summary>Full path to ThreadCraft.Sandbox.dll.</summary>
    public required string SandboxPath { get; init; }

    /// <summary>Extra seconds the host waits after TimeoutSeconds before killing the process.</summary>
    public int HostKillGraceSeconds { get; init; } = 5;
}
