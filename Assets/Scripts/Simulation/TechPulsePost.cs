using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Data class representing a post in the TechPulse feed.
/// </summary>
public sealed class TechPulsePost
{
    public string Id { get; set; }
    public string AuthorName { get; set; }
    public string AuthorHandle { get; set; }
    public Color AuthorColor { get; set; }
    public string Content { get; set; }
    public string Timestamp { get; set; }
    public int Likes { get; set; }
    public int Reposts { get; set; }
    public int Replies { get; set; }
    public PostCategory Category { get; set; }

    // Advanced fields for simulated social network
    public string RelatedCompany { get; set; }
    public string RelatedProduct { get; set; }
    public float PerceivedQuality { get; set; }
    public int Reach { get; set; }
    public int Priority { get; set; }
    public string RelatedEvent { get; set; }
    public List<TechPulseComment> Comments { get; set; } = new List<TechPulseComment>();

    public enum PostCategory
    {
        ModelLaunch,
        Hiring,
        Partnership,
        Incident,
        Benchmark,
        Funding,
        Regulation,
        PlayerPost,
        Trending
    }
}
