namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene AsyncStateMachine() => new(
        "async-state-machine", "Under the hood: the pausing method", 760, 400,
        Nodes:
        [
            new("c1",      40,  40, 230, 58, "① run to the first await", "synchronous, caller's thread"),
            new("c2",      40, 150, 230, 58, "② await Task.Delay(400)", "not done yet", SceneNodeShape.Round),
            new("c3",      40, 300, 230, 58, "③ after the await", "the continuation"),
            new("pool",   420,  30, 310, 160, "thread pool", null, SceneNodeShape.Lane),
            new("thA",    445,  75, 120, 48, "Thread A"),
            new("thB",    585,  75, 120, 48, "Thread B"),
            new("io",     455, 235, 220, 64, "the wait itself", "OS timer / I/O — NO thread", SceneNodeShape.Round),
            new("parked", 440, 330, 250, 50, "parked state machine", "bookmark: resume at ③"),
        ],
        Edges:
        [
            new("e-12",     155,  98, 155, 150),
            new("e-park",   270, 180, 440, 345, "state saved", Dashed: true),
            new("e-io",     270, 190, 455, 255, "timer started", Dashed: true),
            new("e-ding",   565, 299, 595, 190, "continuation queued", Dashed: true),
            new("e-resume", 585, 110, 270, 320, "resume at ③", Dashed: true),
        ],
        Tokens: [ new("thr", "thread ⚡", "thA", Dy: 36, Hidden: true) ],
        Steps:
        [
            new() { Title = "Starts like any method",
                Narration = "Thread A calls your async method and runs it line by line. Nothing magic — the async keyword changes nothing yet.",
                Show = ["thr"], Moves = new() { ["thr"] = new("c1", Dy: 24) },
                Subs = new() { ["thA"] = "running" },
                Active = ["c1"] },
            new() { Title = "Hit the await",
                Narration = "At await the method asks one question: is the task ALREADY finished? If yes, it simply continues. This one takes 400 ms — not done.",
                Moves = new() { ["thr"] = new("c2", Dy: 24) },
                Flow = ["e-12"], Active = ["c2"] },
            new() { Title = "The method PAUSES",
                Narration = "The compiler-built state machine packs itself into a box on the heap: a bookmark saying \"resume at ③\". The caller receives an unfinished Task.",
                Show = ["parked", "e-park"],
                Subs = new() { ["c2"] = "PAUSED here" },
                Flow = ["e-park"], Active = ["parked", "c2"] },
            new() { Title = "The thread goes home",
                Narration = "Thread A does NOT wait. It floats back to the pool, free for other work. Your method is paused — and holds ZERO threads.",
                Moves = new() { ["thr"] = new("thA", Dy: 36) },
                Subs = new() { ["thA"] = "free for work" },
                Active = ["thA"], Dimmed = ["c2"] },
            new() { Title = "The wait needs no thread",
                Narration = "Only two things exist now: the parked bookmark and an OS timer. No thread sits around for the 400 ms.",
                Flow = ["e-io"], Active = ["io", "parked"], Dimmed = ["c2"] },
            new() { Title = "Ding! The timer fires",
                Narration = "The OS tells the runtime: \"the delay is done\". The continuation is posted to the thread-pool queue.",
                Flow = ["e-ding"], Active = ["io", "pool"], Dimmed = ["c2"] },
            new() { Title = "ANY free worker resumes it",
                Narration = "A free worker — Thread B — picks it up and runs ③. Same method, possibly a DIFFERENT thread. The bookmark says where to continue.",
                Moves = new() { ["thr"] = new("c3", Dy: 24) },
                Subs = new() { ["thB"] = "running ③" },
                Flow = ["e-resume"], Active = ["c3", "thB"] },
            new() { Title = "Done — no thread was ever held",
                Narration = "The method finishes; the parked box is garbage-collected. Compare .Result: that would have held Thread A hostage for the whole 400 ms.",
                Moves = new() { ["thr"] = new("thB", Dy: 36) },
                Subs = new() { ["thB"] = "idle", ["c3"] = "completed ✓" },
                Active = ["c3"] },
        ]);
}

