using System.Collections.Generic;
using UnityEngine;

public static class TechPulseContentGenerator
{
    private static readonly string[] PlayerStrongLaunch =
    {
        "{company} shipped {product}. Strong early signal: {quality}% internal score, cleaner docs, and a product surface that developers can actually use.",
        "{product} is live. {company} is not pretending this is frontier yet, but the launch quality is real: {quality}%.",
        "Launch note: {company} released {product} with measured capability at {quality}%. The hard part now is reliability, support and cost control."
    };

    private static readonly string[] PlayerWeakLaunch =
    {
        "{company} launched {product} as an early MVP. Score: {quality}%. Useful, but not production-grade yet.",
        "{product} is out in preview. {company} says the goal is feedback, not hype. Current internal score: {quality}%.",
        "Early release from {company}: {product}. The model works in narrow cases, but serious evals are still ahead."
    };

    private static readonly string[] OrganicMentions =
    {
        "Tried {company}'s demo. The model is not huge, but the onboarding is surprisingly clear.",
        "Small AI labs that obsess over docs and evals will beat louder companies more often than people think.",
        "{company} is still tiny, but their product direction makes sense: ship, measure, fix, repeat.",
        "The real AI race is not only model score. It is uptime, cost per request, docs, safety and support.",
        "A good CLI with a careful permission model can matter more than a flashy benchmark screenshot."
    };

    private static readonly string[] InactivityMentions =
    {
        "{company} has been quiet for {days} days. In AI, silence can mean focus, but customers still need updates.",
        "No new launch from {company} in {days} days. Hope they are fixing reliability instead of chasing vanity benchmarks.",
        "{company}'s last update is getting old. Developer tools need constant maintenance, not just launch posts."
    };

    private static readonly string[] CompetitorPosts =
    {
        "{competitor} announced a new agent runtime with improved tool safety and enterprise audit logs.",
        "{competitor} secured additional GPU capacity. Expect faster training runs and more pressure on smaller labs.",
        "{competitor} published benchmark results, but engineers are asking whether the eval set was contaminated.",
        "{competitor} is cutting inference prices. Great for customers, brutal for labs with weak margins.",
        "{competitor} delayed a launch after red-team findings. This is what responsible deployment looks like.",
        "{competitor} hired a senior infrastructure team to reduce latency across multi-region inference."
    };

    private static readonly string[] CompetitorIncidents =
    {
        "{competitor} is investigating an outage in its inference API. Early reports point to overloaded GPU queues.",
        "{competitor}'s coding agent generated unsafe shell commands for several users. Rollback is in progress.",
        "{competitor} paused a public benchmark submission after data-contamination questions from researchers."
    };

    public static TechPulsePost GeneratePlayerLaunchPost(string company, string product, float quality, string category, string competitor, int followers, float reputation, int postIndex)
    {
        var templates = quality >= 55f ? PlayerStrongLaunch : PlayerWeakLaunch;
        var content = Fill(templates[Random.Range(0, templates.Length)], company, product, quality, competitor, 0);

        return new TechPulsePost
        {
            Id = $"player_launch_{postIndex}",
            AuthorName = company,
            AuthorHandle = "@" + company.Replace(" ", "").ToLowerInvariant(),
            AuthorColor = GameManager.Instance != null ? GameManager.Instance.CompanyColor : GameDesignConstants.BrandPrimary,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today",
            Category = TechPulsePost.PostCategory.ModelLaunch,
            RelatedCompany = company,
            RelatedProduct = product,
            PerceivedQuality = quality,
            Priority = 10,
            Likes = Mathf.RoundToInt(Mathf.Max(8, followers * Random.Range(0.05f, 0.16f))),
            Reposts = Mathf.RoundToInt(Mathf.Max(1, followers * Random.Range(0.01f, 0.04f))),
            Replies = Random.Range(1, 5),
            Comments = GenerateLaunchComments(company, product, quality, competitor)
        };
    }

    public static TechPulsePost GenerateOrganicPost(string company, int followers, float reputation, string category, int postIndex, bool isDelay, int daysInactivity)
    {
        var templates = isDelay ? InactivityMentions : OrganicMentions;
        var content = Fill(templates[Random.Range(0, templates.Length)], company, "the latest build", reputation, "a larger rival", daysInactivity);

        return new TechPulsePost
        {
            Id = $"organic_{postIndex}",
            AuthorName = PickUserName(),
            AuthorHandle = PickUserHandle(),
            AuthorColor = GameDesignConstants.TextSecondary,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today",
            Category = isDelay ? TechPulsePost.PostCategory.Incident : TechPulsePost.PostCategory.Trending,
            RelatedCompany = company,
            Likes = Random.Range(2, 30),
            Reposts = Random.Range(0, 6),
            Replies = Random.Range(0, 4)
        };
    }

    public static TechPulsePost GenerateUserMentionPost(string company, int followers, float reputation, string category, string competitor, int postIndex, float quality)
    {
        var content = Fill(OrganicMentions[Random.Range(0, OrganicMentions.Length)], company, "the product", quality, competitor, 0);

        return new TechPulsePost
        {
            Id = $"mention_{postIndex}",
            AuthorName = PickUserName(),
            AuthorHandle = PickUserHandle(),
            AuthorColor = GameDesignConstants.TextSecondary,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today",
            Category = TechPulsePost.PostCategory.Trending,
            RelatedCompany = company,
            PerceivedQuality = quality,
            Likes = Random.Range(3, 45),
            Reposts = Random.Range(0, 8),
            Replies = Random.Range(0, 5)
        };
    }

    public static TechPulsePost GenerateCompetitorPost(CompetitorCompany company, int postIndex)
    {
        var useIncident = Random.value < 0.25f;
        var templates = useIncident ? CompetitorIncidents : CompetitorPosts;
        var content = templates[Random.Range(0, templates.Length)].Replace("{competitor}", company.Name);

        return new TechPulsePost
        {
            Id = $"competitor_{postIndex}",
            AuthorName = company.Name,
            AuthorHandle = company.Handle,
            AuthorColor = company.BrandColor,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today",
            Category = useIncident ? TechPulsePost.PostCategory.Incident : TechPulsePost.PostCategory.Benchmark,
            RelatedCompany = company.Name,
            Likes = Random.Range(80, 1200) * Mathf.Max(1, company.StrengthTier),
            Reposts = Random.Range(8, 160) * Mathf.Max(1, company.StrengthTier),
            Replies = Random.Range(4, 90) * Mathf.Max(1, company.StrengthTier)
        };
    }

    private static List<TechPulseComment> GenerateLaunchComments(string company, string product, float quality, string competitor)
    {
        var comments = new List<TechPulseComment>();
        var commentCount = Random.Range(1, 4);

        for (var i = 0; i < commentCount; i++)
        {
            var positive = quality >= 55f;
            var content = positive
                ? $"{product} feels promising. The docs and reliability will decide whether teams adopt it."
                : $"{product} is early. I would not use it in production until evals and hosting improve.";

            comments.Add(new TechPulseComment(
                $"comment_{Random.Range(1000, 9999)}",
                PickUserName(),
                PickUserHandle(),
                content,
                TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today"));
        }

        return comments;
    }

    private static string Fill(string template, string company, string product, float quality, string competitor, int days)
    {
        return template
            .Replace("{company}", company)
            .Replace("{product}", product)
            .Replace("{quality}", quality.ToString("F0"))
            .Replace("{competitor}", competitor)
            .Replace("{days}", days.ToString());
    }

    private static string PickUserName()
    {
        string[] names = { "Maya Chen", "DevOps Weekly", "Nora Patel", "BuildShip Daily", "Alex Rivera", "Infra Notes", "Sam Carter" };
        return names[Random.Range(0, names.Length)];
    }

    private static string PickUserHandle()
    {
        string[] handles = { "@maya_codes", "@devops_weekly", "@nora_eval", "@buildship", "@alexr", "@infra_notes", "@sam_ai" };
        return handles[Random.Range(0, handles.Length)];
    }
}
