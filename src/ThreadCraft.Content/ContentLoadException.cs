namespace ThreadCraft.Content;

/// <summary>Thrown when lesson content on disk is malformed. Fails fast at startup.</summary>
public sealed class ContentLoadException : Exception
{
    public ContentLoadException(string message) : base(message) { }
}
