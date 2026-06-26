using UnityEngine;

/// <summary>
/// Handles follower growth/decay, reputation updates, and reach logic for the Player's company.
/// </summary>
public sealed class TechPulseFollowerSystem : MonoBehaviour
{
    public static TechPulseFollowerSystem Instance { get; private set; }

    [Header("Inactivity Settings")]
    [SerializeField] private float inactivityDecayIntervalDays = 15f;
    [SerializeField] private int inactivityDecayFollowerCost = 5;

    private int daysSinceLastActivity;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed += OnDayPassed;
        }
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed -= OnDayPassed;
        }
    }

    private void OnDayPassed()
    {
        daysSinceLastActivity++;

        // Inactivity decay logic
        if (daysSinceLastActivity >= inactivityDecayIntervalDays)
        {
            daysSinceLastActivity = 0;
            ApplyInactivityDecay();
        }
    }

    public void RecordActivity()
    {
        daysSinceLastActivity = 0;
    }

    private void ApplyInactivityDecay()
    {
        if (GameManager.Instance == null) return;

        // Player is inactive - lose some reputation and followers
        int followerLoss = Mathf.Max(inactivityDecayFollowerCost, Mathf.RoundToInt(GameManager.Instance.Followers * 0.02f));
        float repLoss = -1.5f;
        if (GameManager.Instance.HasCommunityManager)
        {
            followerLoss = Mathf.Max(1, followerLoss / 2);
            repLoss = -0.75f;
        }
        GameManager.Instance.AddFollowers(-followerLoss);
        GameManager.Instance.AddReputation(repLoss);

        // Generate an organic delay post demanding news
        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.AddOrganicPlayerPost(true, daysSinceLastActivity);
        }

        ToastNotification.ShowGlobal("The market demands news! Lost some followers due to inactivity.", ToastNotification.Category.Warning);
    }

    /// <summary>
    /// Calculates follower growth based on product quality and company reputation.
    /// </summary>
    public void RegisterProductLaunch(string productName, float quality, string productType)
    {
        RecordActivity();

        if (GameManager.Instance == null) return;

        float reputationFactor = 1f + (GameManager.Instance.Reputation / 50f);
        int baseGrowth = 0;

        float repHypeMult = GameManager.Instance.ReputationBoostMultiplier;

        if (quality >= 75f) // Excellent
        {
            baseGrowth = UnityEngine.Random.Range(150, 400);
            GameManager.Instance.AddReputation(8f * repHypeMult);
        }
        else if (quality >= 45f) // Medium
        {
            baseGrowth = UnityEngine.Random.Range(30, 120);
            GameManager.Instance.AddReputation(3f * repHypeMult);
        }
        else // Weak
        {
            baseGrowth = UnityEngine.Random.Range(-10, 15);
            GameManager.Instance.AddReputation(-5f);
        }

        float followerHypeMult = repHypeMult > 1.0f ? 1.5f : 1.0f;
        if (GameManager.Instance.HasCommunityManager)
        {
            followerHypeMult *= 1.5f;
        }
        int calculatedFollowers = Mathf.RoundToInt(baseGrowth * reputationFactor * followerHypeMult);
        
        // Ensure we always have at least 1 follower
        if (calculatedFollowers != 0)
        {
            GameManager.Instance.AddFollowers(calculatedFollowers);
        }
    }

    /// <summary>
    /// Executes a marketing campaign which increases both reputation and followers at a financial cost.
    /// </summary>
    public void PerformMarketingCampaign(float cost, int baseFollowerGain, float reputationGain)
    {
        RecordActivity();

        if (GameManager.Instance == null) return;

        if (!GameManager.Instance.SpendCash(cost))
        {
            ToastNotification.ShowGlobal("Not enough cash for a marketing campaign!", ToastNotification.Category.Danger);
            return;
        }

        // Apply gains
        int followersToAdd = Mathf.RoundToInt(baseFollowerGain * (GameManager.Instance.HasCommunityManager ? 1.5f : 1.0f));
        GameManager.Instance.AddFollowers(followersToAdd);
        GameManager.Instance.AddReputation(reputationGain);

        // Generate organic marketing campaign post
        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.AddOrganicPlayerPost(false, 0);
        }

        ToastNotification.ShowGlobal($"Marketing campaign success! +{baseFollowerGain} Followers, +{reputationGain:F1} Rep!", ToastNotification.Category.Success);
    }
}
