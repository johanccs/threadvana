namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene EventGate() => new(
        "event-gate", "Under the hood: waiting at the gate", 760, 330,
        Nodes:
        [
            new("line",   40, 130, 160, 90, "waiting threads", "blocked at WaitOne()"),
            new("gate",  300, 130, 170, 70, "the event", "CLOSED", SceneNodeShape.Round),
            new("thru",  560, 130, 160, 56, "continue running"),
            new("setter", 540, 30, 180, 54, "another thread", "calls Set()"),
        ],
        Edges:
        [
            new("e-w", 200, 165, 300, 165, "WaitOne()", Dashed: true),
            new("e-s", 630,  84, 460, 150, "Set()", Dashed: true),
            new("e-t", 470, 160, 560, 158, null, Dashed: true),
        ],
        Tokens:
        [
            new("A", "A", "line", Dx: -42, Dy: 12),
            new("B", "B", "line", Dx:  42, Dy: 12),
            new("sig", "Set()", "setter", Dx: -55, Dy: 15, Hidden: true),
        ],
        Steps:
        [
            new() { Title = "A gate threads wait at",
                Narration = "A ManualResetEvent starts CLOSED. Threads calling WaitOne() park here — asleep, burning no CPU — until someone opens the gate.",
                Flow = ["e-w"], Active = ["line", "gate"] },
            new() { Title = "Set() opens the gate",
                Narration = "Another thread finishes preparing the data and calls Set(). The gate swings open and STAYS open (that is the 'manual' part).",
                Show = ["sig"], Moves = new() { ["sig"] = new("gate", Dy: 22) },
                Subs = new() { ["gate"] = "OPEN" },
                Flow = ["e-s"], Active = ["setter", "gate"] },
            new() { Title = "EVERYONE waiting walks through",
                Narration = "A, B — and any thread that arrives later — all pass immediately. The gate only closes when someone calls Reset().",
                Moves = new() { ["A"] = new("thru", Dx: -42, Dy: 14), ["B"] = new("thru", Dx: 42, Dy: 14) },
                Flow = ["e-t"], Active = ["thru"] },
            new() { Title = "AutoResetEvent: one at a time",
                Narration = "The auto cousin lets exactly ONE waiter through, then closes itself. Perfect for 'next worker, please' scenarios.",
                Subs = new() { ["gate"] = "OPEN for ONE" },
                Active = ["gate"] },
            new() { Title = "The gate family",
                Narration = "CountdownEvent opens after N signals. Barrier opens when the WHOLE group has arrived. Same idea: threads wait, a condition opens the way.",
                Active = ["gate", "line"] },
        ]);
}
