namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene LockKey() => new(
        "lock-key", "Under the hood: one key, one door", 760, 340,
        Nodes:
        [
            new("hook", 320,  25, 130, 46, "the ONE key", null, SceneNodeShape.Pill),
            new("door", 290, 120, 190, 90, "critical section", "shared state inside", SceneNodeShape.Round),
            new("line",  40, 130, 170, 90, "waiting threads", "blocked, no CPU"),
            new("done", 560, 130, 150, 56, "leaves", "work done"),
        ],
        Edges:
        [
            new("e-in",   210, 165, 290, 165, "lock(…)", Dashed: true),
            new("e-out",  480, 160, 560, 158, "key returned", Dashed: true),
            new("e-hook", 385,  71, 385, 120, null, Dashed: true),
        ],
        Tokens:
        [
            new("key", "key", "hook", Dy: 0),
            new("A", "A", "line", Dx: -42, Dy: 12),
            new("B", "B", "line", Dx:  42, Dy: 12),
            new("C", "C", "line", Dx:   0, Dy: 38),
        ],
        Steps:
        [
            new() { Title = "One key, one door",
                Narration = "A lock has exactly ONE key. Only the thread holding it may enter the critical section — the code that touches shared state.",
                Active = ["hook", "door"] },
            new() { Title = "A takes the key and enters",
                Narration = "A calls lock(…), finds the key free, and walks in. B and C arrive a moment later — no key, so they wait. Zero CPU burned while waiting.",
                Moves = new() { ["A"] = new("door", Dx: -35, Dy: 25), ["key"] = new("door", Dx: 35, Dy: 25) },
                Flow = ["e-in"], Active = ["door"] },
            new() { Title = "One inside, always",
                Narration = "While A works, the door is shut. B and C cannot enter no matter how long A takes — that guarantee is 'mutual exclusion'.",
                Active = ["door", "line"], Dimmed = ["line"] },
            new() { Title = "A leaves and returns the key",
                Narration = "Exiting the lock block puts the key back. The runtime wakes ONE waiter — there is no queue order guarantee.",
                Moves = new() { ["A"] = new("done", Dy: 14), ["key"] = new("hook", Dy: 0) },
                Flow = ["e-out", "e-hook"], Active = ["done", "hook"] },
            new() { Title = "B takes it next",
                Narration = "B grabs the key and enters. C keeps waiting. Same door, same rule, one at a time.",
                Moves = new() { ["B"] = new("door", Dx: -35, Dy: 25), ["key"] = new("door", Dx: 35, Dy: 25) },
                Hide = ["A"],
                Flow = ["e-in"], Active = ["door"] },
            new() { Title = "That is ALL a lock is",
                Narration = "Mutual exclusion via a single token. Monitor, Mutex, SpinLock, ReaderWriterLockSlim — all are variations on this one-key door.",
                Active = ["door", "hook"] },
        ]);
}
