using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages rival AI companies, their decision cycles, and market frontier calculations.
/// </summary>
public sealed class CompetitorManager : MonoBehaviour
{
    public static CompetitorManager Instance { get; private set; }

    private List<CompetitorCompany> companies = new List<CompetitorCompany>();

    public IReadOnlyList<CompetitorCompany> Companies => companies;

    // --- Market Frontiers (highest quality/capability scores in each market) ---
    public float FrontierAutomation { get; private set; } = 40f;
    public float FrontierEnterprise { get; private set; } = 40f;
    public float FrontierInfrastructure { get; private set; } = 50f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        
        Instance = this;
        InitializeCompanies();
    }

    private void Start()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnMonthPassed += ExecuteMonthlyCycle;
        }
        RecalculateFrontiers();
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnMonthPassed -= ExecuteMonthlyCycle;
        }
    }

    private void InitializeCompanies()
    {
        companies.Clear();

        // 1. HaploWorks - Vertical B2B automation (Tier 2 in 2016)
        var haplo = new CompetitorCompany("RIV_HAPLOWORKS", "HaploWorks", "@HaploWorks", GameDesignConstants.StatusSuccess, "VerticalB2B", 2);
        haplo.Strengths.Add("Durable recurring B2B contracts");
        haplo.Strengths.Add("Pragmatic enterprise focus");
        haplo.Weaknesses.Add("Weak frontier-model innovation");
        haplo.Weaknesses.Add("Limited consumer appeal");
        haplo.AutomationCapability = 45f;
        haplo.EnterpriseCapability = 40f;
        companies.Add(haplo);

        // 2. Vectoria - Content and data platform (Tier 2 in 2017/2016 prototype)
        var vectoria = new CompetitorCompany("RIV_VECTORIA", "Vectoria", "@Vectoria", GameDesignConstants.DeptPR, "Marketplace", 2);
        vectoria.Strengths.Add("Excellent distribution infrastructure");
        vectoria.Strengths.Add("Large licensed-data catalog");
        vectoria.Weaknesses.Add("Overdependent on creators");
        vectoria.Weaknesses.Add("Exposed to licensing disputes");
        vectoria.EnterpriseCapability = 45f;
        companies.Add(vectoria);

        // 3. NeuraForge - Aggressive frontier lab (Tier 3 in 2016)
        var neura = new CompetitorCompany("RIV_NEURAFORGE", "NeuraForge", "@NeuraForge", GameDesignConstants.BrandPrimary, "Frontier", 3);
        neura.Strengths.Add("Strong frontier research");
        neura.Strengths.Add("Exceptional fundraising ability");
        neura.Weaknesses.Add("Extremely high operating cost");
        neura.Weaknesses.Add("Overfocused on frontier research");
        neura.AutomationCapability = 50f;
        neura.EnterpriseCapability = 50f;
        neura.InfrastructureCapability = 35f;
        companies.Add(neura);

        // 4. Cloudharbor - Infrastructure and data centers (Tier 4 in 2016)
        var cloud = new CompetitorCompany("RIV_CLOUDHARBOR", "Cloudharbor", "@Cloudharbor", GameDesignConstants.DeptInfra, "Infrastructure", 4);
        cloud.Strengths.Add("Massive owned data-center capacity");
        cloud.Strengths.Add("Deep infrastructure capital");
        cloud.Weaknesses.Add("Weak end-user products");
        cloud.Weaknesses.Add("Highly exposed to energy costs");
        cloud.InfrastructureCapability = 65f;
        companies.Add(cloud);
    }

    public CompetitorCompany GetCompanyById(string id)
    {
        return companies.Find(c => c.Id == id);
    }

    public CompetitorCompany GetRandomCompany()
    {
        if (companies.Count == 0) return null;
        return companies[Random.Range(0, companies.Count)];
    }

    /// <summary>
    /// Recalculates the maximum capabilities in each active market (Frontier).
    /// </summary>
    public void RecalculateFrontiers()
    {
        float maxAuto = 30f;
        float maxEnt = 30f;
        float maxInfra = 40f;

        // Player's quality (GameManager.Instance.ModelQuality can represent current quality)
        float playerQual = GameManager.Instance != null ? GameManager.Instance.ModelQuality : 0f;

        foreach (var c in companies)
        {
            if (c.AutomationCapability > maxAuto) maxAuto = c.AutomationCapability;
            if (c.EnterpriseCapability > maxEnt) maxEnt = c.EnterpriseCapability;
            if (c.InfrastructureCapability > maxInfra) maxInfra = c.InfrastructureCapability;
        }

        // Compare against player's established models if applicable
        if (playerQual > maxAuto) maxAuto = playerQual;

        FrontierAutomation = maxAuto;
        FrontierEnterprise = maxEnt;
        FrontierInfrastructure = maxInfra;
    }

    /// <summary>
    /// Triggers the monthly strategic decisions and updates for all active rivals.
    /// </summary>
    private void ExecuteMonthlyCycle(int currentMonth)
    {
        foreach (var c in companies)
        {
            // Update financial runway and cooldowns
            if (c.CooldownMonths > 0)
            {
                c.CooldownMonths--;
            }

            // Monthly operational changes
            c.Capital += c.Revenue * 0.1f; // Earn net profit
            c.Research += c.StrengthTier * Random.Range(0.2f, 1.2f);
            c.Compute += c.StrengthTier * Random.Range(0.2f, 1.2f);

            // Execute strategic action if not in cooldown
            if (c.CooldownMonths == 0)
            {
                ExecuteRivalAction(c);
            }
        }

        RecalculateFrontiers();
    }

    private void ExecuteRivalAction(CompetitorCompany comp)
    {
        // 5 strategic actions: Launch Model, Reduce Price, Suffer Incident, Invest Datacenter, Partner
        int action = Random.Range(0, 5);
        comp.CooldownMonths = Random.Range(2, 4); // Cooldown of 2 to 3 months

        string content = "";
        TechPulsePost.PostCategory postCategory = TechPulsePost.PostCategory.Benchmark;

        switch (action)
        {
            case 0: // Launch Model
                float growth = Random.Range(5f, 15f) * (comp.Research / 25f);
                if (comp.Personality == "Frontier")
                {
                    comp.AutomationCapability = Mathf.Clamp(comp.AutomationCapability + growth * 1.2f, 0f, 100f);
                    comp.EnterpriseCapability = Mathf.Clamp(comp.EnterpriseCapability + growth * 0.8f, 0f, 100f);
                    content = $"We officially launched {comp.Name}-Chat V3 with stronger reasoning and coding benchmark results.";
                }
                else if (comp.Personality == "VerticalB2B")
                {
                    comp.AutomationCapability = Mathf.Clamp(comp.AutomationCapability + growth * 1.5f, 0f, 100f);
                    content = $"Introducing {comp.Name}-Service: tailored automation for enterprise support workflows.";
                }
                else if (comp.Personality == "Marketplace")
                {
                    comp.EnterpriseCapability = Mathf.Clamp(comp.EnterpriseCapability + growth * 1.4f, 0f, 100f);
                    content = $"New predictive enterprise analytics features are live on the {comp.Name} platform.";
                }
                else // Infrastructure
                {
                    comp.InfrastructureCapability = Mathf.Clamp(comp.InfrastructureCapability + growth * 1.3f, 0f, 100f);
                    content = $"Announcing {comp.Name}-Serverless: scalable inference with lower operational overhead.";
                }
                postCategory = TechPulsePost.PostCategory.ModelLaunch;
                break;

            case 1: // Reduce Price
                comp.Revenue += comp.StrengthTier * 500f; // Attracts some immediate cash flow
                content = $"We reduced API inference prices by 20%. {comp.Name} is pushing high-performance AI toward broader access.";
                postCategory = TechPulsePost.PostCategory.Partnership;
                break;

            case 2: // Suffer Incident (Crisis opportunity)
                comp.BrandTrust = Mathf.Max(20f, comp.BrandTrust - Random.Range(10f, 25f));
                content = $"We identified temporary data-center instability caused by energy fluctuations. {comp.Name} engineers are mitigating.";
                postCategory = TechPulsePost.PostCategory.Incident;
                
                // Give player notification of crisis
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.SendNotification($"CRISIS: {comp.Name} is facing service latency! Opportunity to steal clients.");
                }
                break;

            case 3: // Invest in Datacenter
                comp.Compute += 15f;
                comp.Capital -= 10000f;
                content = $"Fechamos a compra de novos clusters de supercomputadores para expandir nosso pipeline de treinamento. #ComputePower";
                postCategory = TechPulsePost.PostCategory.Funding;
                break;

            case 4: // Partner
                comp.Distribution += 10f;
                content = $"We announced a strategic partnership to embed {comp.Name} models and integrations into major enterprise systems.";
                postCategory = TechPulsePost.PostCategory.Partnership;
                break;
        }

        // Publish to TechPulse Feed
        if (TechPulseFeed.Instance != null && !string.IsNullOrEmpty(content))
        {
            TechPulseFeed.Instance.AddSystemPost(comp, content, postCategory);
        }
    }
}
