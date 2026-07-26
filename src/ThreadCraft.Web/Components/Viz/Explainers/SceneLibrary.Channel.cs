namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene ChannelScene() => new(
        "channel", "Under the hood: the thread-safe pipe", 760, 340,
        Nodes:
        [
            new("prod",  30,  60, 160, 64, "Producer", "writes items"),
            new("pipe", 260, 150, 250, 100, "the channel", "0 queued", SceneNodeShape.Lane),
            new("cons", 570, 190, 160, 64, "Consumer", "awaits items"),
            new("done", 300,  30, 180, 50, "Complete()", "\"nothing more is coming\"", SceneNodeShape.Pill),
        ],
        Edges:
        [
            new("e-w", 190,  95, 300, 150, "WriteAsync", Dashed: true),
            new("e-r", 470, 200, 570, 215, "ReadAsync", Dashed: true),
            new("e-c", 390,  80, 390, 150, null, Dashed: true),
        ],
        Tokens:
        [
            new("i1", "item", "prod", Dx: 40, Dy: 20, Hidden: true),
            new("i2", "item", "prod", Dx: 40, Dy: 20, Hidden: true),
            new("i3", "item", "prod", Dx: 40, Dy: 20, Hidden: true),
        ],
        Steps:
        [
            new() { Title = "A pipe between two worlds",
                Narration = "The producer drops work in; the consumer pulls work out. Neither calls the other, neither shares a lock — the channel is the handoff.",
                Active = ["prod", "pipe", "cons"] },
            new() { Title = "Producer writes item 1",
                Narration = "WriteAsync drops an item into the channel. The producer immediately continues with its next piece of work.",
                Show = ["i1"], Moves = new() { ["i1"] = new("pipe", Dx: -70, Dy: 25) },
                Subs = new() { ["pipe"] = "1 queued" },
                Flow = ["e-w"], Active = ["prod", "pipe"] },
            new() { Title = "Consumer takes it",
                Narration = "The consumer was awaiting ReadAsync — the item wakes it up. First in, first out: items arrive in order.",
                Moves = new() { ["i1"] = new("cons", Dx: -40, Dy: 20) },
                Subs = new() { ["pipe"] = "0 queued", ["cons"] = "processing…" },
                Flow = ["e-r"], Active = ["cons"] },
            new() { Title = "Fast producer? Items pile up",
                Narration = "If the producer outpaces the consumer, items queue safely inside. A BOUNDED channel makes the producer's WriteAsync wait — backpressure instead of unbounded memory.",
                Show = ["i2", "i3"],
                Moves = new() { ["i2"] = new("pipe", Dx: 0, Dy: 25), ["i3"] = new("pipe", Dx: 70, Dy: 25) },
                Subs = new() { ["pipe"] = "2 queued" },
                Flow = ["e-w"], Active = ["pipe"] },
            new() { Title = "Complete() signals the end",
                Narration = "When there is no more work, the producer calls Complete(). This is a promise: nothing more will ever be written.",
                Flow = ["e-c"], Active = ["done", "prod"] },
            new() { Title = "Consumer drains, then stops",
                Narration = "The consumer keeps reading the remaining items; when the queue is empty AND Complete() was called, ReadAsync says 'done' and the loop ends cleanly.",
                Hide = ["i1", "i2", "i3"],
                Moves = new() { ["i2"] = new("cons", Dx: -40, Dy: 0), ["i3"] = new("cons", Dx: -40, Dy: 20) },
                Subs = new() { ["pipe"] = "0 queued", ["cons"] = "loop ends" },
                Flow = ["e-r"], Active = ["cons"] },
        ]);
}
