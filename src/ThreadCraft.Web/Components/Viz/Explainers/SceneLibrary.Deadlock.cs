namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene Deadlock() => new(
        "deadlock", "Under the hood: the circular wait", 760, 360,
        Nodes:
        [
            new("ta",  40,  60, 150, 56, "Thread A", "running"),
            new("tb", 570,  60, 150, 56, "Thread B", "running"),
            new("l1", 300,  50, 160, 56, "Lock 1", "free", SceneNodeShape.Round),
            new("l2", 300, 210, 160, 56, "Lock 2", "free", SceneNodeShape.Round),
            new("skull", 300, 295, 160, 48, "DEADLOCK", "nobody can move", SceneNodeShape.Pill),
        ],
        Edges:
        [
            new("e-a1", 190,  80, 300,  80, "takes", Dashed: true),
            new("e-b2", 570, 100, 460, 225, "takes", Dashed: true),
            new("e-a2", 190, 105, 300, 235, "wants…", Dashed: true),
            new("e-b1", 570,  78, 460,  78, "wants…", Dashed: true),
        ],
        Tokens:
        [
            new("k1", "key 1", "l1", Dy: 36),
            new("k2", "key 2", "l2", Dy: 36),
        ],
        Steps:
        [
            new() { Title = "Two locks, two threads",
                Narration = "A locks Lock1 then Lock2. B locks Lock2 then Lock1. Opposite order. It works 99% of the time — until the timing aligns.",
                Active = ["ta", "tb", "l1", "l2"] },
            new() { Title = "A grabs key 1",
                Narration = "Thread A enters Lock 1 and takes its key. Lock 1 is now HELD by A.",
                Moves = new() { ["k1"] = new("ta", Dx: 40, Dy: 18) },
                Subs = new() { ["l1"] = "held by A" },
                Flow = ["e-a1"], Active = ["ta", "l1"] },
            new() { Title = "B grabs key 2",
                Narration = "At the same moment, Thread B enters Lock 2 and takes ITS key. Both threads hold exactly one lock.",
                Moves = new() { ["k2"] = new("tb", Dx: -40, Dy: 18) },
                Subs = new() { ["l2"] = "held by B" },
                Flow = ["e-b2"], Active = ["tb", "l2"] },
            new() { Title = "A wants Lock 2 — but B has the key",
                Narration = "A reaches lock(Lock2) and blocks. A cannot move until B releases Lock 2. A keeps holding Lock 1 while waiting.",
                Subs = new() { ["ta"] = "BLOCKED" },
                Flow = ["e-a2"], Active = ["ta", "l2"], Dimmed = ["tb"] },
            new() { Title = "B wants Lock 1 — but A has the key",
                Narration = "B reaches lock(Lock1) and blocks too. Each holds what the other needs. Neither can EVER move: a circular wait.",
                Subs = new() { ["tb"] = "BLOCKED" },
                Flow = ["e-b1"], Active = ["skull", "l1"] },
            new() { Title = "The fix: one global order",
                Narration = "If EVERY thread takes locks in the SAME order (always Lock1 before Lock2), the circle cannot form. Deadlocks need a cycle — break it by convention.",
                Active = ["skull"] },
        ]);
}
