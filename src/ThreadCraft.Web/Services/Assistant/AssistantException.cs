namespace ThreadCraft.Web.Services;

/// <summary>
/// A coach request failed in a way the learner can understand and act on.
/// The message is always friendly, plain English — never a raw exception dump.
/// </summary>
public sealed class AssistantException : Exception
{
    public AssistantException(string friendlyMessage) : base(friendlyMessage) { }
}
