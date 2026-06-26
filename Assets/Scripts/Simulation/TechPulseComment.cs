using System;

/// <summary>
/// Data class representing a comment on a TechPulse post.
/// </summary>
[Serializable]
public sealed class TechPulseComment
{
    public string Id;
    public string AuthorName;
    public string AuthorHandle;
    public string Content;
    public string Timestamp;

    public TechPulseComment(string id, string authorName, string authorHandle, string content, string timestamp)
    {
        Id = id;
        AuthorName = authorName;
        AuthorHandle = authorHandle;
        Content = content;
        Timestamp = timestamp;
    }
}
