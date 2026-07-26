namespace ThreadCraft.Web.Components.Viz.Explainers;

public static partial class SceneLibrary
{
    private static ExplainerScene ThreadLocal() => new(
        "thread-local", "Under the hood: every thread gets its own copy", 760, 330,
        Nodes:
        [
            new("shared", 280,  25, 210, 54, "ordinary static field", "ONE value, shared by all", SceneNodeShape.Round),
            new("ta",     60, 150, 200, 56, "Thread A", "works with its own copy"),
            new("tb",    500, 150, 200, 56, "Thread B", "works with its own copy"),
            new("ca",     85, 245, 150, 48, "A's copy", "= 0"),
            new("cb",    525, 245, 150, 48, "B's copy", "= 0"),
        ],
        Edges:
        [
            new("e-ta", 160, 206, 160, 245, null, Dashed: true),
            new("e-tb", 600, 206, 600, 245, null, Dashed: true),
        ],
        Tokens: [ new("v", "42", "ta", Dx: 65, Dy: 16, Hidden: true) ],
        Steps:
        [
            new() { Title = "Ordinary statics are shared",
                Narration = "A normal static field has exactly ONE value. Every thread sees the same box — that is precisely where races come from.",
                Active = ["shared"] },
            new() { Title = "ThreadLocal: a copy per thread",
                Narration = "[ThreadStatic] or ThreadLocal<T> gives EACH thread its own private box. A and B look at 'the same' field and see different values.",
                Flow = ["e-ta", "e-tb"], Active = ["ta", "tb", "ca", "cb"] },
            new() { Title = "A writes 42 — only A's copy changes",
                Narration = "A stores 42. It lands in A's private box. There is no shared box to fight over, so there is nothing to lock.",
                Show = ["v"], Moves = new() { ["v"] = new("ca", Dx: 0, Dy: 26) },
                Subs = new() { ["ca"] = "= 42" },
                Flow = ["e-ta"], Active = ["ta", "ca"] },
            new() { Title = "B still sees 0",
                Narration = "B reads 'the same' field and gets its OWN value: 0. No lock, no wait, no race — by construction.",
                Active = ["tb", "cb"], Dimmed = ["ta", "ca"] },
            new() { Title = "When to reach for it",
                Narration = "Per-thread caches, counters, Random instances. (AsyncLocal<T> is the cousin whose value flows DOWN an async call chain.)",
                Active = ["ca", "cb"] },
        ]);
}
