namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene ThreadBasics() => new(
        "thread-basics", "Under the hood: one CPU, many threads", 760, 340,
        Nodes:
        [
            new("main",   60,  50, 180, 64, "Main thread", "your program"),
            new("worker", 60, 220, 180, 64, "Worker thread", "you started it"),
            new("sched", 320, 140, 140, 56, "OS scheduler", null, SceneNodeShape.Pill),
            new("cpu",   560, 130, 150, 76, "CPU core", "runs the code", SceneNodeShape.Round),
        ],
        Edges:
        [
            new("e-main",   240,  82, 560, 150),
            new("e-worker", 240, 252, 560, 186),
            new("e-sched",  460, 168, 560, 168, null, Dashed: true),
        ],
        Tokens: [ new("slice", "⚡ has the CPU", "sched", Dy: 34) ],
        Steps:
        [
            new() { Title = "Two threads, one CPU",
                Narration = "Your program (Main) and a Worker thread you started both want to run. There is only one free CPU core — someone must decide who gets it.",
                Active = ["main", "worker"] },
            new() { Title = "The scheduler is the referee",
                Narration = "The OS scheduler — part of the operating system itself — picks one thread and lends it the CPU for a few milliseconds.",
                Active = ["sched"], Flow = ["e-sched"] },
            new() { Title = "Main gets a time slice",
                Narration = "Main wins first: it runs its code at full speed. The Worker is ready but frozen — not running at all.",
                Moves = new() { ["slice"] = new("main", Dy: 21) },
                Active = ["main", "cpu"], Flow = ["e-main"], Dimmed = ["worker"] },
            new() { Title = "Context switch!",
                Narration = "Slice over! The scheduler freezes Main mid-sentence, remembers exactly where it stopped, and hands the CPU to the Worker.",
                Moves = new() { ["slice"] = new("worker", Dy: 21) },
                Active = ["sched", "worker"], Flow = ["e-worker"], Dimmed = ["main"] },
            new() { Title = "The Worker runs now",
                Narration = "Now the Worker executes while Main waits its turn. This swap happens thousands of times per second.",
                Active = ["worker", "cpu"], Flow = ["e-worker"], Dimmed = ["main"] },
            new() { Title = "So fast it looks parallel",
                Narration = "Back and forth, back and forth — both threads seem to run at the same time. That is why their Console output interleaves.",
                Moves = new() { ["slice"] = new("main", Dy: 21) },
                Active = ["main", "cpu"], Flow = ["e-main"], Dimmed = ["worker"] },
            new() { Title = "When a thread ends",
                Narration = "A finished thread never needs the CPU again. (With more cores, threads can also TRULY run at once — one per core.)",
                Moves = new() { ["slice"] = new("sched", Dy: 34) },
                Subs = new() { ["worker"] = "finished ✓" },
                Active = ["sched"] },
        ]);

    private static ExplainerScene ThreadJoin() => new(
        "thread-join", "Under the hood: what Join really does", 760, 340,
        Nodes:
        [
            new("main",   40,  60, 150, 56, "Main thread"),
            new("join",  300,  56, 170, 64, "worker.Join()", "the gate", SceneNodeShape.Round),
            new("cont",  580,  60, 140, 56, "next line"),
            new("worker", 300, 220, 170, 64, "Worker thread", "running…"),
        ],
        Edges:
        [
            new("e-mj", 190,  88, 300,  88),
            new("e-jc", 470,  88, 580,  88),
            new("e-wj", 385, 220, 385, 124, "finishes", Dashed: true),
        ],
        Tokens:
        [
            new("tm", "Main", "main", Dy: 20),
            new("tw", "work", "worker", Dy: 21),
        ],
        Steps:
        [
            new() { Title = "Main starts a Worker",
                Narration = "Both threads run side by side. Main reaches the line worker.Join() — meaning: \"wake me when THAT thread is done\".",
                Active = ["main", "worker"] },
            new() { Title = "Main calls Join",
                Narration = "Main slides up to the gate and stops there. Joining = parking yourself until the other thread ends.",
                Moves = new() { ["tm"] = new("join", Dy: 22) },
                Flow = ["e-mj"], Active = ["join"] },
            new() { Title = "Blocked — not busy",
                Narration = "Main is frozen at the gate: zero CPU, zero progress. The operating system will wake it when the Worker ends.",
                Active = ["join"], Dimmed = ["main"] },
            new() { Title = "The Worker keeps running",
                Narration = "The Worker is unaffected — it finishes its job at its own pace.",
                Active = ["worker"], Dimmed = ["main"] },
            new() { Title = "The Worker ends",
                Narration = "Its method returns and the thread is done — which is exactly the event Join was waiting for.",
                Hide = ["tw"], Subs = new() { ["worker"] = "finished ✓" },
                Active = ["worker"], Dimmed = ["main"] },
            new() { Title = "The gate opens",
                Narration = "The Worker's end unblocks Main instantly — no polling, no wasted CPU cycles.",
                Flow = ["e-wj"], Active = ["join"] },
            new() { Title = "Main continues",
                Narration = "Main resumes on the very next line — now guaranteed that the Worker's results are ready to use.",
                Moves = new() { ["tm"] = new("cont", Dy: 20) },
                Flow = ["e-jc"], Active = ["cont"] },
        ]);
}

