using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Manages the feed of posts for TechPulse. Generates posts periodically.
/// </summary>
public sealed class TechPulseFeed : MonoBehaviour
{
    public static TechPulseFeed Instance { get; private set; }

    private List<TechPulsePost> posts = new List<TechPulsePost>();
    private int postCounter;

    public IReadOnlyList<TechPulsePost> Posts => posts;
    public event Action<TechPulsePost> OnNewPost;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private int daysCounter;
    private const int AutoPostIntervalDays = 8;
    private const int InitialPostCount = 3;
    private const int MaxStoredPosts = 60;

    private void Start()
    {
        GenerateInitialPosts();
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed += HandleDayPassed;
        }
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed -= HandleDayPassed;
        }
    }

    private void HandleDayPassed()
    {
        daysCounter++;
        if (daysCounter >= AutoPostIntervalDays)
        {
            daysCounter = 0;
            GenerateRandomPost();
        }
    }

    public void AddPlayerPost(string content)
    {
        var gm = GameManager.Instance;
        var playerName = gm != null ? gm.CompanyName : "Player Lab";
        
        var post = new TechPulsePost
        {
            Id = $"post_{++postCounter}",
            AuthorName = playerName,
            AuthorHandle = "@" + playerName.Replace(" ", "").ToLower(),
            AuthorColor = GameDesignConstants.BrandPrimary,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today",
            Likes = UnityEngine.Random.Range(10, 50),
            Reposts = UnityEngine.Random.Range(0, 10),
            Replies = UnityEngine.Random.Range(1, 5),
            Category = TechPulsePost.PostCategory.PlayerPost
        };

        AddNewPost(post);
    }

    /// <summary>
    /// Generates and publishes an official launch post for a player's product.
    /// </summary>
    public void AddPlayerLaunchPost(string productName, float quality, string productType)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        string competitor = "NeuraCorp";
        if (CompetitorManager.Instance != null)
        {
            var comp = CompetitorManager.Instance.GetRandomCompany();
            if (comp != null) competitor = comp.Name;
        }

        var post = TechPulseContentGenerator.GeneratePlayerLaunchPost(
            gm.CompanyName,
            productName,
            quality,
            productType,
            competitor,
            gm.Followers,
            gm.Reputation,
            ++postCounter
        );

        AddNewPost(post);
    }

    /// <summary>
    /// Generates and publishes an organic player post (teaser, marketing, inactivity).
    /// </summary>
    public void AddOrganicPlayerPost(bool isDelay, int daysInactivity)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        string currentCategory = "Voz";
        if (PrototypeProjectController.Instance != null)
        {
            currentCategory = PrototypeProjectController.Instance.SelectedModelLabel;
        }

        var post = TechPulseContentGenerator.GenerateOrganicPost(
            gm.CompanyName,
            gm.Followers,
            gm.Reputation,
            currentCategory,
            ++postCounter,
            isDelay,
            daysInactivity
        );

        AddNewPost(post);
    }

    public void AddSystemPost(CompetitorCompany company, string content, TechPulsePost.PostCategory category)
    {
        var post = new TechPulsePost
        {
            Id = $"post_{++postCounter}",
            AuthorName = company.Name,
            AuthorHandle = company.Handle,
            AuthorColor = company.BrandColor,
            Content = content,
            Timestamp = TimeController.Instance != null ? TimeController.Instance.FormattedDate : "Today",
            Likes = UnityEngine.Random.Range(100, 10000) * company.StrengthTier,
            Reposts = UnityEngine.Random.Range(10, 1000) * company.StrengthTier,
            Replies = UnityEngine.Random.Range(5, 500) * company.StrengthTier,
            Category = category
        };

        AddNewPost(post);
    }

    private void AddNewPost(TechPulsePost post)
    {
        posts.Insert(0, post); // Add to top
        if (posts.Count > MaxStoredPosts)
        {
            posts.RemoveAt(posts.Count - 1);
        }
        OnNewPost?.Invoke(post);
    }

    private void GenerateInitialPosts()
    {
        if (CompetitorManager.Instance == null) return;

        // Generate a mix of interesting startup and rival announcements
        var companies = CompetitorManager.Instance.Companies;
        for (int i = 0; i < InitialPostCount; i++)
        {
            var comp = companies[UnityEngine.Random.Range(0, companies.Count)];
            var post = TechPulseContentGenerator.GenerateCompetitorPost(comp, ++postCounter);
            posts.Add(post);
        }
    }

    private void GenerateRandomPost()
    {
        if (CompetitorManager.Instance == null) return;

        float r = UnityEngine.Random.value;
        if (r < 0.70f)
        {
            var comp = CompetitorManager.Instance.GetRandomCompany();
            if (comp != null)
            {
                var post = TechPulseContentGenerator.GenerateCompetitorPost(comp, ++postCounter);
                AddNewPost(post);
            }
        }
        else
        {
            var gm = GameManager.Instance;
            if (gm != null)
            {
                string category = "Voz";
                if (PrototypeProjectController.Instance != null)
                {
                    category = PrototypeProjectController.Instance.SelectedModelLabel;
                }

                var comp = CompetitorManager.Instance.GetRandomCompany();
                string competitorName = comp != null ? comp.Name : "NeuraCorp";

                float quality = gm.ModelQuality > 0 ? gm.ModelQuality : 50f;

                var post = TechPulseContentGenerator.GenerateUserMentionPost(
                    gm.CompanyName,
                    gm.Followers,
                    gm.Reputation,
                    category,
                    competitorName,
                    ++postCounter,
                    quality
                );
                AddNewPost(post);
            }
        }
    }
}
