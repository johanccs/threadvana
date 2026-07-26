namespace ThreadCraft.Web.Components.Viz.Explainers;

/// <summary>Shape of a node in an explainer flowchart.</summary>
public enum SceneNodeShape { Box, Round, Pill, Lane }

/// <summary>A node (box) in the diagram. Text layout: title at top, sub below it.</summary>
public sealed record SceneNode(
    string Id, double X, double Y, double W, double H,
    string Title, string? Sub = null,
    SceneNodeShape Shape = SceneNodeShape.Box, string Css = "");

/// <summary>A straight arrow between two authored points. Drawn under the nodes.</summary>
public sealed record SceneEdge(
    string Id, double X1, double Y1, double X2, double Y2,
    string? Label = null, bool Dashed = false);

/// <summary>A small moving pill (a task, a thread, the CPU). Position is authored
/// relative to a node's centre; steps move it between nodes.</summary>
public sealed record SceneToken(
    string Id, string Text, string AtNode,
    double Dx = 0, double Dy = 0, bool Hidden = false);

/// <summary>Where a token should sit at a given step (node centre + offset).</summary>
public sealed record TokenMove(string NodeId, double Dx = 0, double Dy = 0);

/// <summary>
/// One beat of the animation. Active/Dimmed/Flow REPLACE each step;
/// Moves/Subs/Hide/Show are folded from step 0 up to the current step,
/// so stepping backwards always reproduces the same picture.
/// </summary>
public sealed record SceneStep
{
    public required string Title { get; init; }
    public required string Narration { get; init; }
    public string[] Active { get; init; } = [];
    public string[] Dimmed { get; init; } = [];
    public string[] Flow { get; init; } = [];
    public Dictionary<string, TokenMove> Moves { get; init; } = [];
    public Dictionary<string, string> Subs { get; init; } = [];
    public string[] Hide { get; init; } = [];
    public string[] Show { get; init; } = [];
}

/// <summary>A scripted animated flowchart that explains one concept.</summary>
public sealed record ExplainerScene(
    string Id, string Title, int ViewWidth, int ViewHeight,
    SceneNode[] Nodes, SceneEdge[] Edges, SceneToken[] Tokens, SceneStep[] Steps);
