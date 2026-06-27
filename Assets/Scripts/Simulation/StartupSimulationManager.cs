using System;
using UnityEngine;

public sealed class StartupSimulationManager : MonoBehaviour
{
    public static StartupSimulationManager Instance { get; private set; }

    public enum StartupStage
    {
        SoloFounder,
        FirstProduct,
        ProductMarketFit,
        ModelCompany,
        PlatformCompany,
        FrontierLab
    }

    public StartupStage Stage { get; private set; } = StartupStage.SoloFounder;
    public float ResearchSkill { get; private set; } = 8f;
    public float ModelCapability { get; private set; } = 5f;
    public float WebsiteQuality { get; private set; } = 10f;
    public float ProductQuality { get; private set; } = 5f;
    public float InfrastructureReliability { get; private set; } = 8f;
    public float Trust { get; private set; } = 12f;
    public float MaintenanceLoad { get; private set; } = 0f;
    public int Designers { get; private set; }
    public int Developers { get; private set; }
    public int Scientists { get; private set; }
    public int ProductSurfaces { get; private set; }
    public int ArenaRank { get; private set; } = 0;

    public event Action OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public static StartupSimulationManager EnsureExists()
    {
        if (Instance != null)
        {
            return Instance;
        }

        var go = new GameObject("StartupSimulationManager");
        return go.AddComponent<StartupSimulationManager>();
    }

    public void StudyPapers()
    {
        var team = TeamSimulationManager.EnsureExists();
        if (team != null && team.StartStudy(team.GetFounder(), "ai_papers"))
        {
            return;
        }

        Notify("The founder cannot start a new study track right now.");
    }

    public void ApplyStudyLearning(string primaryAttribute, float primaryGain, float secondaryGain)
    {
        var totalGain = primaryGain + secondaryGain;
        ResearchSkill = ClampStat(ResearchSkill + totalGain * 0.45f);
        ModelCapability = ClampStat(ModelCapability + (primaryAttribute == "Research" ? primaryGain * 0.22f : totalGain * 0.08f));
        ProductQuality = ClampStat(ProductQuality + (primaryAttribute == "Product" || primaryAttribute == "Design" ? primaryGain * 0.18f : 0f));
        InfrastructureReliability = ClampStat(InfrastructureReliability + (primaryAttribute == "Infrastructure" || primaryAttribute == "Engineering" ? primaryGain * 0.14f : 0f));
        Trust = ClampStat(Trust + Mathf.Clamp(totalGain * 0.08f, 0.1f, 1.2f));
        Notify(LocalizationManager.T("startup.news_study"));
        RecalculateCompanyEconomics();
    }

    public void ImproveWebsite()
    {
        if (!Spend(1200f))
        {
            return;
        }

        var designBonus = Designers > 0 ? 1.5f : 1f;
        WebsiteQuality = ClampStat(WebsiteQuality + UnityEngine.Random.Range(5f, 9f) * designBonus);
        ProductQuality = ClampStat(ProductQuality + UnityEngine.Random.Range(1f, 3f) * designBonus);
        Trust = ClampStat(Trust + UnityEngine.Random.Range(1f, 2.5f));
        Notify(LocalizationManager.T("startup.news_website"));
        RecalculateCompanyEconomics();
    }

    public void BuildPrototype()
    {
        if (!Spend(2500f))
        {
            return;
        }

        var devBonus = Developers > 0 ? 1.35f : 1f;
        var researchBonus = 1f + ResearchSkill / 120f;
        ModelCapability = ClampStat(ModelCapability + UnityEngine.Random.Range(4f, 8f) * researchBonus);
        ProductQuality = ClampStat(ProductQuality + UnityEngine.Random.Range(4f, 7f) * devBonus);
        MaintenanceLoad = ClampStat(MaintenanceLoad + UnityEngine.Random.Range(2f, 4f));
        ProductSurfaces = Mathf.Max(ProductSurfaces, 1);
        Notify(LocalizationManager.T("startup.news_prototype"));
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void SecureCloudHosting()
    {
        if (!Spend(8000f))
        {
            return;
        }

        InfrastructureReliability = ClampStat(InfrastructureReliability + UnityEngine.Random.Range(14f, 22f));
        MaintenanceLoad = ClampStat(MaintenanceLoad + 5f);
        Notify(LocalizationManager.T("startup.news_hosting"));
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void HireDesigner()
    {
        if (!Spend(6000f))
        {
            return;
        }

        Designers++;
        WebsiteQuality = ClampStat(WebsiteQuality + 8f);
        ProductQuality = ClampStat(ProductQuality + 7f);
        Trust = ClampStat(Trust + 2f);
        Notify(LocalizationManager.T("startup.news_designer"));
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void HireDeveloper()
    {
        if (!Spend(9000f))
        {
            return;
        }

        Developers++;
        ProductQuality = ClampStat(ProductQuality + 8f);
        InfrastructureReliability = ClampStat(InfrastructureReliability + 4f);
        Notify(LocalizationManager.T("startup.news_developer"));
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void HireScientist()
    {
        if (!Spend(18000f))
        {
            return;
        }

        Scientists++;
        ResearchSkill = ClampStat(ResearchSkill + 16f);
        ModelCapability = ClampStat(ModelCapability + 8f);
        Notify(LocalizationManager.T("startup.news_researcher"));
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void LaunchCliAgent()
    {
        if (Developers == 0)
        {
            Notify(LocalizationManager.T("startup.fail_require_dev"));
            return;
        }

        if (ModelCapability < 22f)
        {
            Notify(LocalizationManager.T("startup.fail_require_model"));
            return;
        }

        if (InfrastructureReliability < 20f)
        {
            Notify(LocalizationManager.T("startup.fail_require_infra"));
            return;
        }

        if (!Spend(6000f))
        {
            return;
        }

        ProductSurfaces++;
        ProductQuality = ClampStat(ProductQuality + UnityEngine.Random.Range(5f, 9f));
        Trust = ClampStat(Trust + UnityEngine.Random.Range(2f, 5f));
        MaintenanceLoad = ClampStat(MaintenanceLoad + UnityEngine.Random.Range(7f, 11f));
        Notify(LocalizationManager.T("startup.news_cli"));
        PublishTechPulse("We launched an early CLI coding agent. It is small, careful, permissioned, and built for real repositories.");
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void BuildAgentStudio()
    {
        if (Developers < 1 || Designers < 1)
        {
            Notify("Agent Studio requires at least one developer and one designer.");
            return;
        }

        if (ModelCapability < 35f || InfrastructureReliability < 35f)
        {
            Notify("Agent Studio needs stronger model capability and hosting reliability.");
            return;
        }

        if (!Spend(14000f))
        {
            return;
        }

        ProductSurfaces++;
        ProductQuality = ClampStat(ProductQuality + UnityEngine.Random.Range(7f, 12f));
        Trust = ClampStat(Trust + UnityEngine.Random.Range(2f, 4f));
        MaintenanceLoad = ClampStat(MaintenanceLoad + UnityEngine.Random.Range(10f, 16f));
        Notify(LocalizationManager.T("startup.news_builder"));
        PublishTechPulse("Agent Studio is live in private preview: templates, tools, approvals, logs and cost controls.");
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public void SubmitToArena()
    {
        if (ModelCapability < 18f)
        {
            Notify(LocalizationManager.T("startup.fail_require_model"));
            return;
        }

        if (!Spend(3500f))
        {
            return;
        }

        var compositeScore = GetCompositeScore();
        ArenaRank = Mathf.Max(1, 120 - Mathf.RoundToInt(compositeScore));
        Trust = ClampStat(Trust + Mathf.Clamp(compositeScore / 20f, 1f, 6f));
        Notify(LocalizationManager.T("startup.news_arena"));
        PublishTechPulse($"AI Arena submission completed. Composite score: {compositeScore:F1}. Public rank: #{ArenaRank}.");
        RecalculateStage();
        RecalculateCompanyEconomics();
    }

    public float GetCompositeScore()
    {
        return Mathf.Clamp(
            ModelCapability * 0.38f +
            ProductQuality * 0.18f +
            InfrastructureReliability * 0.16f +
            ResearchSkill * 0.14f +
            Trust * 0.14f -
            MaintenanceLoad * 0.08f,
            0f,
            100f);
    }

    public string GetNextGoal()
    {
        switch (Stage)
        {
            case StartupStage.SoloFounder: return LocalizationManager.T("startup.goal_0");
            case StartupStage.FirstProduct: return LocalizationManager.T("startup.goal_1");
            case StartupStage.ProductMarketFit: return LocalizationManager.T("startup.goal_2");
            case StartupStage.ModelCompany: return LocalizationManager.T("startup.goal_3");
            default: return LocalizationManager.T("startup.goal_4");
        }
    }

    private bool Spend(float amount)
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            return false;
        }

        if (gm.SpendCash(amount))
        {
            return true;
        }

        Notify(LocalizationManager.T("startup.fail_cash"));
        return false;
    }

    private void RecalculateStage()
    {
        var score = GetCompositeScore();
        var nextStage = StartupStage.SoloFounder;

        if (score >= 72f && ProductSurfaces >= 5 && Scientists >= 2)
        {
            nextStage = StartupStage.FrontierLab;
        }
        else if (score >= 55f && ProductSurfaces >= 3)
        {
            nextStage = StartupStage.PlatformCompany;
        }
        else if (score >= 40f && Scientists >= 1)
        {
            nextStage = StartupStage.ModelCompany;
        }
        else if (score >= 28f && ProductSurfaces >= 2)
        {
            nextStage = StartupStage.ProductMarketFit;
        }
        else if (score >= 18f && ProductSurfaces >= 1)
        {
            nextStage = StartupStage.FirstProduct;
        }

        Stage = nextStage;
    }

    private void RecalculateCompanyEconomics()
    {
        var gm = GameManager.Instance;
        if (gm == null)
        {
            return;
        }

        var payroll = Designers * 1800f + Developers * 2600f + Scientists * 5200f;
        var infrastructure = Mathf.Max(0f, InfrastructureReliability - 10f) * 45f;
        var maintenance = MaintenanceLoad * 65f;
        var revenue = ProductSurfaces * ProductQuality * WebsiteQuality * 0.55f;

        if (InfrastructureReliability < 25f && ProductSurfaces > 1)
        {
            revenue *= 0.7f;
        }

        gm.SetMonthlyBurn(800f + payroll + infrastructure + maintenance);
        gm.SetMonthlyRevenue(Mathf.Max(0f, revenue));
        gm.SetModelQuality(GetCompositeScore());
        gm.SetTeamSize(1 + Designers + Developers + Scientists);
        gm.AddCompetence(0f);
        OnStateChanged?.Invoke();
    }

    private void Notify(string message)
    {
        GameManager.Instance?.SendNotification(message);
        ToastNotification.ShowGlobal(message, ToastNotification.Category.Info);
        OnStateChanged?.Invoke();
    }

    private void PublishTechPulse(string content)
    {
        TechPulseFeed.Instance?.AddPlayerPost(content);
        TechPulseFollowerSystem.Instance?.RecordActivity();
    }

    private static float ClampStat(float value)
    {
        return Mathf.Clamp(value, 0f, 100f);
    }
}
