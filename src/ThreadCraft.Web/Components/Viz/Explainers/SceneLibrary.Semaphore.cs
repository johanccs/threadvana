namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene Semaphore() => new(
        "semaphore", "Under the hood: the counter is the bouncer", 760, 360,
        Nodes:
        [
            new("line",  30, 130, 150, 100, "Waiting line", "threads park here"),
            new("sem",  250,  25, 220,  52, "Semaphore", "free spaces: 3"),
            new("lot",  230, 105, 440, 150, "the protected resource", null, SceneNodeShape.Lane),
            new("s1",   255, 140, 120,  80, "Space 1", "free"),
            new("s2",   400, 140, 120,  80, "Space 2", "free"),
            new("s3",   545, 140, 120,  80, "Space 3", "free"),
        ],
        Edges:
        [
            new("e-in",  180, 170, 255, 175, "Wait()"),
            new("e-out", 665, 175, 730, 175, "Release()"),
        ],
        Tokens:
        [
            new("A", "A", "line", Dx: -38, Dy: 12),
            new("B", "B", "line", Dx:  38, Dy: 12),
            new("C", "C", "line", Dx: -38, Dy: 38),
            new("D", "D", "line", Dx:  38, Dy: 38),
        ],
        Steps:
        [
            new() { Title = "A bouncer with 3 keys",
                Narration = "A semaphore is just a counter: how many threads may be inside AT ONCE. This one allows 3 — the count starts at 3.",
                Active = ["sem"] },
            new() { Title = "A walks straight in",
                Narration = "Thread A calls Wait(). Count is 3, which is more than 0 → A enters immediately and the count drops to 2. No waiting at all.",
                Moves = new() { ["A"] = new("s1", Dy: 28) },
                Subs = new() { ["sem"] = "free spaces: 2", ["s1"] = "taken" },
                Flow = ["e-in"], Active = ["s1"] },
            new() { Title = "B and C fill the lot",
                Narration = "B (2 → 1) and C (1 → 0) take the last spaces. The lot is now FULL — the count is 0.",
                Moves = new() { ["B"] = new("s2", Dy: 28), ["C"] = new("s3", Dy: 28) },
                Subs = new() { ["sem"] = "free spaces: 0", ["s2"] = "taken", ["s3"] = "taken" },
                Active = ["s2", "s3"] },
            new() { Title = "D must wait",
                Narration = "D calls Wait() but the count is 0 — no spaces. D parks in the waiting line, burning no CPU. (With WaitAsync, D's Task pauses instead — zero threads held.)",
                Moves = new() { ["D"] = new("line", Dx: 0, Dy: 12) },
                Active = ["line", "sem"] },
            new() { Title = "Release() opens a spot",
                Narration = "A finishes and calls Release(). The count ticks 0 → 1 — and exactly ONE waiter is allowed through.",
                Hide = ["A"],
                Subs = new() { ["sem"] = "free spaces: 1", ["s1"] = "free" },
                Flow = ["e-out"], Active = ["sem"] },
            new() { Title = "D takes the freed space",
                Narration = "D wakes and enters (1 → 0). The count ALWAYS equals the number of free spaces — that invariant is the entire semaphore.",
                Moves = new() { ["D"] = new("s1", Dy: 28) },
                Subs = new() { ["sem"] = "free spaces: 0", ["s1"] = "taken" },
                Flow = ["e-in"], Active = ["s1"] },
        ]);
}
