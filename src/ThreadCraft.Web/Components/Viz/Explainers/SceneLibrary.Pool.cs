namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene ThreadPool() => new(
        "thread-pool", "Under the hood: the on-call team", 760, 360,
        Nodes:
        [
            new("you",    30, 140, 140, 70, "Your code", "Task.Run(…)"),
            new("queue", 250, 115, 180, 120, "Pool queue", "waiting: 0"),
            new("pool",  520,  25, 210, 300, "thread pool", null, SceneNodeShape.Lane),
            new("w1",    550,  60, 150, 64, "Worker 1", "idle"),
            new("w2",    550, 200, 150, 64, "Worker 2", "idle"),
        ],
        Edges:
        [
            new("e-sub", 170, 175, 250, 175, "hand in"),
            new("e-t1",  430, 140, 550,  92),
            new("e-t2",  430, 210, 550, 232),
        ],
        Tokens:
        [
            new("t1", "Task 1", "you", Hidden: true),
            new("t2", "Task 2", "you", Hidden: true),
            new("t3", "Task 3", "you", Hidden: true),
        ],
        Steps:
        [
            new() { Title = "Hand work to the pool",
                Narration = "Task.Run drops Task 1 into the pool's queue. Your thread does NOT run it — it just posts it, like mail.",
                Show = ["t1"], Moves = new() { ["t1"] = new("queue", Dy: 45) },
                Subs = new() { ["queue"] = "waiting: 1" },
                Flow = ["e-sub"], Active = ["you", "queue"] },
            new() { Title = "An on-call worker grabs it",
                Narration = "Worker 1 was already hired and idling. It picks Task 1 off the queue and runs it — no new thread is created.",
                Moves = new() { ["t1"] = new("w1", Dy: 21) },
                Subs = new() { ["queue"] = "waiting: 0", ["w1"] = "working…" },
                Flow = ["e-t1"], Active = ["w1"] },
            new() { Title = "More work arrives",
                Narration = "Task 2 and Task 3 land in the queue while Worker 1 is still busy.",
                Show = ["t2", "t3"],
                Moves = new() { ["t2"] = new("queue", Dx: -50, Dy: 45), ["t3"] = new("queue", Dx: 50, Dy: 45) },
                Subs = new() { ["queue"] = "waiting: 2" },
                Flow = ["e-sub"], Active = ["you", "queue"] },
            new() { Title = "Worker 2 takes Task 2",
                Narration = "Task 3 must wait — the pool keeps its team small ON PURPOSE (every extra thread costs about 1 MB of memory).",
                Moves = new() { ["t2"] = new("w2", Dy: 21) },
                Subs = new() { ["queue"] = "waiting: 1", ["w2"] = "working…" },
                Flow = ["e-t2"], Active = ["w2"] },
            new() { Title = "Task 3 waits its turn",
                Narration = "No free worker, no new hire. The queue holds Task 3 safely until somebody finishes.",
                Active = ["queue"] },
            new() { Title = "Reuse — the whole point",
                Narration = "Worker 1 finishes Task 1 and immediately picks up Task 3. SAME thread, next job. Threads hired so far: zero.",
                Hide = ["t1"], Moves = new() { ["t3"] = new("w1", Dy: 21) },
                Subs = new() { ["queue"] = "waiting: 0" },
                Flow = ["e-t1"], Active = ["w1"] },
            new() { Title = "Back on call",
                Narration = "All tasks done. Both workers go idle — alive and ready for the next batch. Nobody was created or destroyed. That is the thread pool.",
                Hide = ["t2", "t3"],
                Subs = new() { ["w1"] = "idle", ["w2"] = "idle" },
                Active = ["w1", "w2"] },
        ]);
}
