namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene Cancellation() => new(
        "cancellation", "Under the hood: the polite stop flag", 760, 340,
        Nodes:
        [
            new("worker",  60,  60, 200, 90, "Worker loop", "iteration 1"),
            new("check",   60, 200, 200, 54, "checks the flag", "every round"),
            new("flag",   350, 130, 220, 56, "IsCancellationRequested", "= false", SceneNodeShape.Pill),
            new("cts",    520,  40, 190, 54, "another thread", "calls Cancel()"),
            new("stop",   480, 240, 190, 54, "stops cleanly", "finally runs", SceneNodeShape.Round),
        ],
        Edges:
        [
            new("e-loop",  160, 150, 160, 200),
            new("e-flag",  260, 227, 350, 165, null, Dashed: true),
            new("e-cts",   615,  94, 540, 150, "Cancel()", Dashed: true),
            new("e-stop",  460, 186, 520, 240, null, Dashed: true),
        ],
        Tokens: [ new("cpu", "CPU ⚡", "worker", Dx: 60, Dy: 28) ],
        Steps:
        [
            new() { Title = "A shared flag, checked politely",
                Narration = "A CancellationToken is just a flag two threads can see. The worker checks it at a SAFE point in every loop round.",
                Flow = ["e-loop", "e-flag"], Active = ["worker", "check", "flag"] },
            new() { Title = "Someone calls Cancel()",
                Narration = "A button click, a timeout, a shutdown — another thread calls Cancel() on the source. The worker is NOT interrupted mid-instruction.",
                Subs = new() { ["worker"] = "iteration 42" },
                Flow = ["e-cts"], Active = ["cts"] },
            new() { Title = "The flag flips to true",
                Narration = "Cancel() only flips the flag. Nothing else happens. The worker is still running its current iteration.",
                Subs = new() { ["flag"] = "= true" },
                Active = ["flag"], Dimmed = ["cts"] },
            new() { Title = "The worker notices — and exits cleanly",
                Narration = "Next check: true! The worker finishes its current step, runs its cleanup (finally), and stops. That is COOPERATIVE — nothing forces it.",
                Flow = ["e-stop"], Active = ["check", "stop"] },
            new() { Title = "Why not just kill the thread?",
                Narration = "Thread.Abort stops code BETWEEN any two instructions — locks stay held, files half-written. Cooperation stops exactly where your code decides it is safe.",
                Active = ["stop"] },
        ]);
}
