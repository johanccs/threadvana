namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene ForegroundBackground() => new(
        "foreground-background", "Under the hood: who keeps the process alive", 760, 340,
        Nodes:
        [
            new("proc",  30,  30, 700, 280, "your process", null, SceneNodeShape.Lane),
            new("main",  70,  60, 160, 54, "Main thread", "foreground, always"),
            new("fg",    70, 180, 170, 54, "new Thread(…)", "foreground by default"),
            new("bg",   480, 180, 180, 54, "IsBackground = true", "…or a pool thread"),
            new("exit", 480,  55, 180, 56, "process exits?", "not yet", SceneNodeShape.Pill),
        ],
        Edges:
        [
            new("e-fg", 240, 207, 480,  90, "keeps alive", Dashed: true),
            new("e-bg", 570, 180, 570, 111, "ignored", Dashed: true),
        ],
        Tokens: [ new("cpu", "CPU ⚡", "main", Dx: 55, Dy: 15) ],
        Steps:
        [
            new() { Title = "Three threads in one process",
                Narration = "Main runs, plus two workers you started: one foreground, one background. The only difference is one boolean flag.",
                Active = ["main", "fg", "bg"] },
            new() { Title = "Main ends",
                Narration = "Main reaches the end and finishes. Does the process exit? NO — a process lives as long as any FOREGROUND thread runs.",
                Moves = new() { ["cpu"] = new("fg", Dx: 55, Dy: 15) },
                Subs = new() { ["main"] = "ended", ["exit"] = "still alive" },
                Flow = ["e-fg"], Active = ["fg", "exit"], Dimmed = ["main"] },
            new() { Title = "The foreground worker holds the door",
                Narration = "The background worker is also running — but it does NOT count. Only foreground threads keep the process alive.",
                Flow = ["e-fg"], Active = ["fg"], Dimmed = ["main", "bg"] },
            new() { Title = "Foreground ends too",
                Narration = "Now the last foreground thread finishes. Zero foreground threads remain…",
                Moves = new() { ["cpu"] = new("bg", Dx: -55, Dy: 15) },
                Subs = new() { ["fg"] = "ended" },
                Active = ["exit"], Dimmed = ["main", "fg"] },
            new() { Title = "Process exits — background is KILLED",
                Narration = "The process exits instantly and the background worker is killed mid-sentence, no finally, no cleanup. That is why pool threads (always background) can vanish under you.",
                Subs = new() { ["bg"] = "killed mid-work", ["exit"] = "exit" },
                Active = ["exit", "bg"], Dimmed = ["main", "fg"] },
        ]);
}
