using UnityEngine;
using System;

/// <summary>
/// Central game state manager. Singleton that persists across scenes.
/// All gameplay systems read/write state through this manager.
/// </summary>
public sealed class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Company")]
    [SerializeField] private string companyName = "Model Foundry";

    [Header("Starting Values")]
    [SerializeField] private float startingCash       = 25000f;
    [SerializeField] private float startingReputation  = 10f;
    [SerializeField] private float startingMonthlyBurn = 800f;

    // ── Public state ─────────────────────────────────────────────────
    public string CompanyName    => companyName;
    public float  Cash           { get; private set; }
    public float  Reputation     { get; private set; }
    public float  ModelQuality   { get; private set; }
    public int    TeamSize       { get; private set; } = 1;
    public int    GpuCount       { get; private set; } = 1;
    public float  MonthlyBurn    { get; private set; }
    public int    TotalClients   { get; private set; }
    public float  MonthlyRevenue { get; private set; }
    public bool   IsNlpUnlocked  { get; private set; } = false;
    public bool   IsAgenticUnlocked { get; private set; } = false;
    public bool   IsGameRunning  { get; private set; }
    public float  Competence     { get; private set; }
    public int    Followers      { get; private set; } = 1;
    public int    Following      { get; private set; } = 1;

    // Phase 4 State
    public int  OfficeTier           { get; private set; } = 1;
    public bool HasMLEngineer        { get; private set; } = false;
    public bool HasResearchScientist { get; private set; } = false;
    public bool HasDataEngineer      { get; private set; } = false;
    public bool HasSafetyResearcher  { get; private set; } = false;
    public bool   IsSafetyAlignmentResearched { get; private set; } = false;
    public bool   IsCustomSiliconResearched   { get; private set; } = false;

    // Phase 7 State
    public bool HasInfrastructureEngineer { get; private set; } = false;
    public bool HasGpuTechnician         { get; private set; } = false;
    public bool HasMlopsEngineer         { get; private set; } = false;
    public bool HasBackendEngineer        { get; private set; } = false;
    public int  EnergyGridUpgrades       { get; private set; } = 0;
    public int  CoolingUpgrades          { get; private set; } = 0;

    // Phase 8 State
    public float FounderEquity           { get; private set; } = 100f;
    public float BoardTrust              { get; private set; } = 100f;
    public int   FundingRound            { get; private set; } = 0; // 0=none, 1=Series A, 2=B, 3=C, 4=IPO, 5=IPO done
    public bool  HasAcquiredQuantumMinds { get; private set; } = false;
    public bool  HasAcquiredAnthroTech   { get; private set; } = false;
    public bool  HasFinanceLead          { get; private set; } = false;
    public bool  HasRecruiter            { get; private set; } = false;
    public bool  HasProductManager        { get; private set; } = false;
    public bool  HasSalesExecutive       { get; private set; } = false;
    public bool  HasCommunityManager     { get; private set; } = false;
    public string ActiveBoardGoalType    { get; private set; } = "None"; // "Revenue", "Followers", "CostControl", "None"
    public float BoardGoalTarget         { get; private set; } = 0f;
    public int   BoardGoalRemainingMonths { get; private set; } = 0;
    public bool  IsGameOver              { get; private set; } = false;

    // Energy & Cooling calculations
    public float EnergyCapacity => GetBaseEnergyCapacity() + EnergyGridUpgrades * 30f;
    public float EnergyUsage    => GpuCount * 10f * (HasInfrastructureEngineer ? 0.75f : 1.0f);
    public float CoolingCapacity => GetBaseCoolingCapacity() + CoolingUpgrades * 30f;
    public float CoolingUsage    => GpuCount * 10f * (HasGpuTechnician ? 0.9f : 1.0f);
    public bool  IsOverheating   => (EnergyUsage > EnergyCapacity) || (CoolingUsage > CoolingCapacity);

    private float GetBaseEnergyCapacity()
    {
        if (OfficeTier == 1) return 15f;
        if (OfficeTier == 2) return 40f;
        if (OfficeTier == 3) return 80f;
        return 200f; // Tier 4
    }

    private float GetBaseCoolingCapacity()
    {
        if (OfficeTier == 1) return 15f;
        if (OfficeTier == 2) return 40f;
        if (OfficeTier == 3) return 80f;
        return 200f; // Tier 4
    }

    // Phase 6 Global Event Modifiers
    public float GpuCostMultiplier { get; set; } = 1.0f;
    public bool IsDataRegulationActive { get; set; } = false;
    public float ReputationBoostMultiplier { get; set; } = 1.0f;

    public int GpuShortageRemainingMonths { get; set; } = 0;
    public int HypeWaveRemainingMonths { get; set; } = 0;
    public int DataRegulationRemainingMonths { get; set; } = 0;

    public bool ShouldLoadSaveOnStart { get; set; } = false;

    // ── Events ───────────────────────────────────────────────────────
    public event Action<float> OnCashChanged;
    public event Action<float> OnReputationChanged;
    public event Action<float> OnQualityChanged;
    public event Action<int>   OnTeamChanged;
    public event Action<float> OnCompetenceChanged;
    public event Action        OnGameStateChanged;
    public event Action<string> OnNotification;

    // ── Lifecycle ────────────────────────────────────────────────────

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            if (Application.isPlaying)
            {
                Destroy(gameObject);
            }
            else
            {
                DestroyImmediate(gameObject);
            }
            return;
        }

        Instance = this;
        if (Application.isPlaying)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        if (!IsGameRunning)
        {
            StartNewGame("Model Foundry (Editor)");
        }
    }

    /// <summary>
    /// Call when starting a new game (from Main Menu).
    /// </summary>
    public void StartNewGame(string playerCompanyName)
    {
        companyName    = string.IsNullOrWhiteSpace(playerCompanyName) ? "Model Foundry" : playerCompanyName;
        Cash           = startingCash;
        Reputation     = startingReputation;
        ModelQuality   = 0f;
        TeamSize       = 1;
        GpuCount       = 1;
        MonthlyBurn    = startingMonthlyBurn;
        TotalClients   = 0;
        MonthlyRevenue = 0f;
        IsNlpUnlocked  = false;
        IsAgenticUnlocked = false;
        IsGameRunning  = true;
        Competence     = 10f;
        Followers      = 1;
        Following      = 1;

        // Phase 4 reset
        OfficeTier = 1;
        HasMLEngineer = false;
        HasResearchScientist = false;
        HasDataEngineer = false;
        HasSafetyResearcher = false;
        IsSafetyAlignmentResearched = false;
        IsCustomSiliconResearched = false;

        // Phase 6 reset
        GpuCostMultiplier = 1.0f;
        IsDataRegulationActive = false;
        ReputationBoostMultiplier = 1.0f;
        GpuShortageRemainingMonths = 0;
        HypeWaveRemainingMonths = 0;
        DataRegulationRemainingMonths = 0;

        // Phase 7 reset
        HasInfrastructureEngineer = false;
        HasGpuTechnician = false;
        HasMlopsEngineer = false;
        HasBackendEngineer = false;
        EnergyGridUpgrades = 0;
        CoolingUpgrades = 0;

        // Phase 8 reset
        FounderEquity = 100f;
        BoardTrust = 100f;
        FundingRound = 0;
        HasAcquiredQuantumMinds = false;
        HasAcquiredAnthroTech = false;
        HasFinanceLead = false;
        HasRecruiter = false;
        HasProductManager = false;
        HasSalesExecutive = false;
        HasCommunityManager = false;
        ActiveBoardGoalType = "None";
        BoardGoalTarget = 0f;
        BoardGoalRemainingMonths = 0;
        IsGameOver = false;

        BroadcastAll();
        OnNotification?.Invoke($"Welcome to {companyName}! Your AI journey begins.");
    }

    /// <summary>
    /// Re-initialize with current values (for scene reloads).
    /// </summary>
    public void ResumeGame()
    {
        IsGameRunning = true;
        BroadcastAll();
    }

    // ── Cash ─────────────────────────────────────────────────────────

    public void AddCash(float amount)
    {
        Cash += amount;
        OnCashChanged?.Invoke(Cash);
        OnGameStateChanged?.Invoke();
    }

    public bool SpendCash(float amount)
    {
        if (Cash < amount)
        {
            OnNotification?.Invoke("Not enough cash!");
            return false;
        }

        Cash -= amount;
        OnCashChanged?.Invoke(Cash);
        OnGameStateChanged?.Invoke();
        return true;
    }

    /// <summary>Called by TimeController each month to deduct burn.</summary>
    public void ApplyMonthlyBurn()
    {
        UpdateGlobalModifiersOnMonthPassed();

        Cash -= MonthlyBurn;
        Cash += MonthlyRevenue;

        var net = MonthlyRevenue - MonthlyBurn;
        var sign = net >= 0 ? "+" : "";
        OnNotification?.Invoke($"Monthly: {sign}${net:N0} (Revenue ${MonthlyRevenue:N0} − Burn ${MonthlyBurn:N0})");

        OnCashChanged?.Invoke(Cash);
        OnGameStateChanged?.Invoke();

        if (Cash <= 0f)
        {
            OnNotification?.Invoke("WARNING: You are out of cash! Find revenue fast.");
        }

        // Phase 8 Board Goals evaluation
        if (FundingRound > 0 && !IsGameOver)
        {
            BoardGoalRemainingMonths--;
            if (BoardGoalRemainingMonths <= 0)
            {
                EvaluateBoardGoal();
            }
        }
    }

    private void UpdateGlobalModifiersOnMonthPassed()
    {
        if (GpuShortageRemainingMonths > 0)
        {
            GpuShortageRemainingMonths--;
            if (GpuShortageRemainingMonths == 0)
            {
                GpuCostMultiplier = 1.0f;
                SendNotification("GPU shortage has ended. Prices are back to normal.");
            }
        }

        if (HypeWaveRemainingMonths > 0)
        {
            HypeWaveRemainingMonths--;
            if (HypeWaveRemainingMonths == 0)
            {
                ReputationBoostMultiplier = 1.0f;
                SendNotification("AI hype wave has cooled down.");
            }
        }

        if (DataRegulationRemainingMonths > 0)
        {
            DataRegulationRemainingMonths--;
            if (DataRegulationRemainingMonths == 0)
            {
                IsDataRegulationActive = false;
                SendNotification("Data regulation audit period has closed.");
            }
        }
    }

    // ── Reputation ───────────────────────────────────────────────────

    public void AddReputation(float amount)
    {
        Reputation = Mathf.Clamp(Reputation + amount, 0f, 100f);
        OnReputationChanged?.Invoke(Reputation);
        OnGameStateChanged?.Invoke();
    }

    // ── Quality ──────────────────────────────────────────────────────

    public void SetModelQuality(float quality)
    {
        ModelQuality = Mathf.Clamp(quality, 0f, 100f);
        OnQualityChanged?.Invoke(ModelQuality);
        OnGameStateChanged?.Invoke();
    }

    // ── Competence ───────────────────────────────────────────────────

    public void AddCompetence(float amount)
    {
        Competence = Mathf.Clamp(Competence + amount, 0f, 100f);
        OnCompetenceChanged?.Invoke(Competence);
        OnGameStateChanged?.Invoke();
    }

    public void SetCompetence(float value)
    {
        Competence = Mathf.Clamp(value, 0f, 100f);
        OnCompetenceChanged?.Invoke(Competence);
        OnGameStateChanged?.Invoke();
    }

    // ── Team ─────────────────────────────────────────────────────────

    public void SetTeamSize(int size)
    {
        TeamSize = Mathf.Max(1, size);
        OnTeamChanged?.Invoke(TeamSize);
        OnGameStateChanged?.Invoke();
    }

    // ── Clients & Revenue ────────────────────────────────────────────

    public void AddClients(int count)
    {
        TotalClients += count;
        OnGameStateChanged?.Invoke();
    }

    public void SetMonthlyRevenue(float revenue)
    {
        MonthlyRevenue = Mathf.Max(0f, revenue);
        OnGameStateChanged?.Invoke();
    }

    public void SetMonthlyBurn(float burn)
    {
        MonthlyBurn = Mathf.Max(0f, burn);
        OnGameStateChanged?.Invoke();
    }

    // ── Notifications ────────────────────────────────────────────────

    public void SendNotification(string message)
    {
        OnNotification?.Invoke(message);
    }

    // ── Helpers ──────────────────────────────────────────────────────

    /// <summary>Months of cash remaining at current burn rate (minus revenue).</summary>
    public float Runway
    {
        get
        {
            var netBurn = MonthlyBurn - MonthlyRevenue;
            if (netBurn <= 0f) return float.PositiveInfinity;
            return Cash / netBurn;
        }
    }

    private void BroadcastAll()
    {
        OnCashChanged?.Invoke(Cash);
        OnReputationChanged?.Invoke(Reputation);
        OnQualityChanged?.Invoke(ModelQuality);
        OnTeamChanged?.Invoke(TeamSize);
        OnCompetenceChanged?.Invoke(Competence);
        OnGameStateChanged?.Invoke();
    }

    public void LoadGameState(
        string companyName, 
        float cash, 
        float reputation, 
        float modelQuality, 
        int teamSize, 
        int gpuCount, 
        float monthlyBurn, 
        int totalClients, 
        float monthlyRevenue, 
        bool isNlpUnlocked, 
        bool isAgenticUnlocked, 
        float competence = 10f, 
        int followers = 1, 
        int following = 1,
        int officeTier = 1,
        bool hasMLEngineer = false,
        bool hasResearchScientist = false,
        bool hasDataEngineer = false,
        bool hasSafetyResearcher = false,
        bool isSafetyAlignmentResearched = false,
        bool isCustomSiliconResearched = false,
        float gpuCostMultiplier = 1.0f,
        bool isDataRegulationActive = false,
        float reputationBoostMultiplier = 1.0f,
        int gpuShortageRemainingMonths = 0,
        int hypeWaveRemainingMonths = 0,
        int dataRegulationRemainingMonths = 0,
        bool hasInfrastructureEngineer = false,
        bool hasGpuTechnician = false,
        bool hasMlopsEngineer = false,
        bool hasBackendEngineer = false,
        int energyGridUpgrades = 0,
        int coolingUpgrades = 0,
        float founderEquity = 100f,
        float boardTrust = 100f,
        int fundingRound = 0,
        bool hasAcquiredQuantumMinds = false,
        bool hasAcquiredAnthroTech = false,
        bool hasFinanceLead = false,
        bool hasRecruiter = false,
        bool hasProductManager = false,
        bool hasSalesExecutive = false,
        bool hasCommunityManager = false,
        string activeBoardGoalType = "None",
        float boardGoalTarget = 0f,
        int boardGoalRemainingMonths = 0,
        bool isGameOver = false)
    {
        this.companyName = companyName;
        Cash = cash;
        Reputation = reputation;
        ModelQuality = modelQuality;
        TeamSize = teamSize;
        GpuCount = gpuCount;
        MonthlyBurn = monthlyBurn;
        TotalClients = totalClients;
        MonthlyRevenue = monthlyRevenue;
        IsNlpUnlocked = isNlpUnlocked;
        IsAgenticUnlocked = isAgenticUnlocked;
        Competence = competence;
        Followers = Mathf.Max(1, followers);
        Following = Mathf.Max(1, following);

        // Phase 4 variables load
        OfficeTier = officeTier;
        HasMLEngineer = hasMLEngineer || (teamSize >= 2 && !hasResearchScientist && !hasDataEngineer && !hasSafetyResearcher && !hasFinanceLead && !hasRecruiter && !hasProductManager && !hasSalesExecutive && !hasCommunityManager);
        HasResearchScientist = hasResearchScientist;
        HasDataEngineer = hasDataEngineer;
        HasSafetyResearcher = hasSafetyResearcher;
        IsSafetyAlignmentResearched = isSafetyAlignmentResearched;
        IsCustomSiliconResearched = isCustomSiliconResearched;

        // Phase 6 load
        GpuCostMultiplier = gpuCostMultiplier;
        IsDataRegulationActive = isDataRegulationActive;
        ReputationBoostMultiplier = reputationBoostMultiplier;
        GpuShortageRemainingMonths = gpuShortageRemainingMonths;
        HypeWaveRemainingMonths = hypeWaveRemainingMonths;
        DataRegulationRemainingMonths = dataRegulationRemainingMonths;

        // Phase 7 variables load
        HasInfrastructureEngineer = hasInfrastructureEngineer;
        HasGpuTechnician = hasGpuTechnician;
        HasMlopsEngineer = hasMlopsEngineer;
        HasBackendEngineer = hasBackendEngineer;
        EnergyGridUpgrades = energyGridUpgrades;
        CoolingUpgrades = coolingUpgrades;

        // Phase 8 variables load
        FounderEquity = founderEquity;
        BoardTrust = boardTrust;
        FundingRound = fundingRound;
        HasAcquiredQuantumMinds = hasAcquiredQuantumMinds;
        HasAcquiredAnthroTech = hasAcquiredAnthroTech;
        HasFinanceLead = hasFinanceLead;
        HasRecruiter = hasRecruiter;
        HasProductManager = hasProductManager;
        HasSalesExecutive = hasSalesExecutive;
        HasCommunityManager = hasCommunityManager;
        ActiveBoardGoalType = string.IsNullOrEmpty(activeBoardGoalType) ? "None" : activeBoardGoalType;
        BoardGoalTarget = boardGoalTarget;
        BoardGoalRemainingMonths = boardGoalRemainingMonths;
        IsGameOver = isGameOver;

        RecalculateMonthlyBurn();
        BroadcastAll();
    }

    public void AddFollowers(int amount)
    {
        Followers = Mathf.Max(1, Followers + amount);
        OnGameStateChanged?.Invoke();
    }

    public void SetFollowers(int amount)
    {
        Followers = Mathf.Max(1, amount);
        OnGameStateChanged?.Invoke();
    }

    public void AddFollowing(int amount)
    {
        Following = Mathf.Max(1, Following + amount);
        OnGameStateChanged?.Invoke();
    }

    public void SetFollowing(int amount)
    {
        Following = Mathf.Max(1, amount);
        OnGameStateChanged?.Invoke();
    }

    public void UnlockNlp()
    {
        IsNlpUnlocked = true;
        OnGameStateChanged?.Invoke();
    }

    public void UnlockAgentic()
    {
        IsAgenticUnlocked = true;
        OnGameStateChanged?.Invoke();
    }

    public bool BuyGpuUpgrade(float cost, float monthlyBurnIncrease)
    {
        if (!SpendCash(cost))
        {
            return false;
        }

        GpuCount++;
        RecalculateMonthlyBurn();
        return true;
    }

    // Phase 4 Setters & Helpers
    public void SetOfficeTier(int tier)
    {
        OfficeTier = Mathf.Clamp(tier, 1, 4);
        RecalculateMonthlyBurn();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasMLEngineer(bool value)
    {
        HasMLEngineer = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasResearchScientist(bool value)
    {
        HasResearchScientist = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasDataEngineer(bool value)
    {
        HasDataEngineer = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasSafetyResearcher(bool value)
    {
        HasSafetyResearcher = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetSafetyAlignmentResearched(bool value)
    {
        IsSafetyAlignmentResearched = value;
        OnGameStateChanged?.Invoke();
    }

    public void SetCustomSiliconResearched(bool value)
    {
        IsCustomSiliconResearched = value;
        RecalculateMonthlyBurn();
        OnGameStateChanged?.Invoke();
    }

    // Phase 7 Setters & Upgrades
    public bool BuyEnergyGridUpgrade(float cost)
    {
        if (!SpendCash(cost)) return false;
        EnergyGridUpgrades++;
        RecalculateMonthlyBurn();
        OnGameStateChanged?.Invoke();
        return true;
    }

    public bool BuyCoolingUpgrade(float cost)
    {
        if (!SpendCash(cost)) return false;
        CoolingUpgrades++;
        RecalculateMonthlyBurn();
        OnGameStateChanged?.Invoke();
        return true;
    }

    public void SetHasInfrastructureEngineer(bool value)
    {
        HasInfrastructureEngineer = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasGpuTechnician(bool value)
    {
        HasGpuTechnician = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasMlopsEngineer(bool value)
    {
        HasMlopsEngineer = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasBackendEngineer(bool value)
    {
        HasBackendEngineer = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    private void UpdateTeamSize()
    {
        int size = 1;
        if (HasMLEngineer) size++;
        if (HasResearchScientist) size++;
        if (HasDataEngineer) size++;
        if (HasSafetyResearcher) size++;
        if (HasInfrastructureEngineer) size++;
        if (HasGpuTechnician) size++;
        if (HasMlopsEngineer) size++;
        if (HasBackendEngineer) size++;
        if (HasFinanceLead) size++;
        if (HasRecruiter) size++;
        if (HasProductManager) size++;
        if (HasSalesExecutive) size++;
        if (HasCommunityManager) size++;
        SetTeamSize(size);
    }

    public void RecalculateMonthlyBurn()
    {
        float baseBurn = startingMonthlyBurn;
        float salaries = 0f;
        if (HasMLEngineer) salaries += 1200f;
        if (HasResearchScientist) salaries += 2500f;
        if (HasDataEngineer) salaries += 1800f;
        if (HasSafetyResearcher) salaries += 3500f;
        if (HasInfrastructureEngineer) salaries += 2800f;
        if (HasGpuTechnician) salaries += 1900f;
        if (HasMlopsEngineer) salaries += 2200f;
        if (HasBackendEngineer) salaries += 1500f;
        if (HasFinanceLead) salaries += 3500f;
        if (HasRecruiter) salaries += 2500f;
        if (HasProductManager) salaries += 4000f;
        if (HasSalesExecutive) salaries += 3000f;
        if (HasCommunityManager) salaries += 2000f;

        int extraGpus = GpuCount - 1;
        float gpuBurnRate = IsCustomSiliconResearched ? 150f : 300f;
        float gpuBurn = extraGpus * gpuBurnRate;

        float officeBurn = 0f;
        if (OfficeTier == 2) officeBurn = 1000f;
        else if (OfficeTier == 3) officeBurn = 3500f;
        else if (OfficeTier == 4) officeBurn = 5000f;

        float upgradeBurn = EnergyGridUpgrades * 100f + CoolingUpgrades * 80f;

        float totalBurn = baseBurn + salaries + gpuBurn + officeBurn + upgradeBurn;
        if (HasFinanceLead)
        {
            totalBurn *= 0.9f; // 10% reduction in total monthly burn
        }

        MonthlyBurn = totalBurn;
    }

    // Phase 8 Setters & Methods
    public void SetHasFinanceLead(bool value)
    {
        HasFinanceLead = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasRecruiter(bool value)
    {
        HasRecruiter = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasProductManager(bool value)
    {
        HasProductManager = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasSalesExecutive(bool value)
    {
        HasSalesExecutive = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasCommunityManager(bool value)
    {
        HasCommunityManager = value;
        RecalculateMonthlyBurn();
        UpdateTeamSize();
        OnGameStateChanged?.Invoke();
    }

    public void SetHasAcquiredQuantumMinds(bool value)
    {
        HasAcquiredQuantumMinds = value;
        OnGameStateChanged?.Invoke();
    }

    public void SetHasAcquiredAnthroTech(bool value)
    {
        HasAcquiredAnthroTech = value;
        OnGameStateChanged?.Invoke();
    }

    public void AcceptNextFundingRound()
    {
        if (FundingRound >= 4) return;

        float baseCash = 0f;
        float equity = 0f;
        string roundName = "";

        switch (FundingRound)
        {
            case 0:
                baseCash = 150000f;
                equity = 15f;
                roundName = "Series A";
                break;
            case 1:
                baseCash = 400000f;
                equity = 20f;
                roundName = "Series B";
                break;
            case 2:
                baseCash = 1000000f;
                equity = 25f;
                roundName = "Series C";
                break;
            case 3:
                baseCash = 5000000f;
                equity = 40f;
                roundName = "IPO";
                break;
        }

        float finalCash = baseCash * (HasFinanceLead ? 1.2f : 1.0f);
        AddCash(finalCash);
        FounderEquity = Mathf.Max(0f, FounderEquity - equity);
        FundingRound++;

        if (FundingRound == 1) // Series A completed
        {
            BoardTrust = 100f;
            GenerateQuarterlyBoardGoal();
            SendNotification("Series A Funding Accepted! Board of Directors formed. Board Trust initialized at 100%.");
            ToastNotification.ShowGlobal("Series A Funding Accepted! Board of Directors formed.", ToastNotification.Category.Success);
        }
        else
        {
            SendNotification($"{roundName} Funding Accepted! +${finalCash:N0} cash, sold {equity}% equity.");
            ToastNotification.ShowGlobal($"{roundName} Accepted!", ToastNotification.Category.Success);
        }

        if (TechPulseFeed.Instance != null)
        {
            if (roundName == "IPO")
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ IPO SUCCESS: @{companyName.Replace(" ", "")} goes public! Raised ${finalCash/1000000f:F1}M, founder equity at {FounderEquity:F0}%.");
            }
            else
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ FUNDING ROUND: @{companyName.Replace(" ", "")} raises ${finalCash/1000f:F0}k in {roundName}! Valuation boosted by Finance Lead.");
            }
        }

        OnGameStateChanged?.Invoke();
    }

    public void GenerateQuarterlyBoardGoal()
    {
        if (FundingRound == 0)
        {
            ActiveBoardGoalType = "None";
            BoardGoalRemainingMonths = 0;
            BoardGoalTarget = 0f;
            return;
        }

        int goalIndex = UnityEngine.Random.Range(0, 3);
        BoardGoalRemainingMonths = 3;

        switch (goalIndex)
        {
            case 0:
                ActiveBoardGoalType = "Revenue";
                BoardGoalTarget = Mathf.Round((MonthlyRevenue * 1.25f + 2000f) / 1000f) * 1000f;
                SendNotification($"New Board Goal: Reach Monthly Revenue of ${BoardGoalTarget:N0} in 3 months.");
                ToastNotification.ShowGlobal("New Board Goal: Increase Revenue!", ToastNotification.Category.Info);
                break;
            case 1:
                ActiveBoardGoalType = "Followers";
                BoardGoalTarget = Mathf.RoundToInt((Followers * 1.3f + 50f) / 50f) * 50;
                SendNotification($"New Board Goal: Reach {BoardGoalTarget:N0} Followers in 3 months.");
                ToastNotification.ShowGlobal("New Board Goal: Increase Followers!", ToastNotification.Category.Info);
                break;
            case 2:
                ActiveBoardGoalType = "CostControl";
                BoardGoalTarget = Mathf.Round((MonthlyBurn + 1000f) / 500f) * 500f;
                SendNotification($"New Board Goal: Keep Monthly Burn below ${BoardGoalTarget:N0} for the quarter.");
                ToastNotification.ShowGlobal("New Board Goal: Control Expenses!", ToastNotification.Category.Info);
                break;
        }
        OnGameStateChanged?.Invoke();
    }

    private void EvaluateBoardGoal()
    {
        bool success = false;

        switch (ActiveBoardGoalType)
        {
            case "Revenue":
                success = MonthlyRevenue >= BoardGoalTarget;
                break;
            case "Followers":
                success = Followers >= BoardGoalTarget;
                break;
            case "CostControl":
                success = MonthlyBurn <= BoardGoalTarget;
                break;
            default:
                success = true;
                break;
        }

        if (success)
        {
            BoardTrust = Mathf.Clamp(BoardTrust + 15f, 0f, 100f);
            SendNotification($"Board Goal Met! Board Trust increased to {BoardTrust:F0}%.");
            ToastNotification.ShowGlobal("Board Goal Completed! Trust +15%.", ToastNotification.Category.Success);
            if (TechPulseFeed.Instance != null)
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ BOARD COMMENDATION: Management has met the quarterly target. Board trust is now {BoardTrust:F0}%.");
            }
        }
        else
        {
            BoardTrust = Mathf.Clamp(BoardTrust - 20f, 0f, 100f);
            SendNotification($"Board Goal Failed! Board Trust decreased to {BoardTrust:F0}%.");
            ToastNotification.ShowGlobal("Board Goal Failed! Trust -20%.", ToastNotification.Category.Danger);
            if (TechPulseFeed.Instance != null)
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ INVESTOR DISSATISFACTION: Fails to meet quarterly board requirements. Trust drops to {BoardTrust:F0}%.");
            }

            if (BoardTrust <= 0f)
            {
                TriggerGameOver();
                return;
            }
        }

        GenerateQuarterlyBoardGoal();
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;
        TimeController.Instance.SetSpeed(TimeController.Speed.Paused);
        OnGameStateChanged?.Invoke();
        SendNotification("GAME OVER: Fired by the Board of Directors!");
        ToastNotification.ShowGlobal("GAME OVER: Fired by the Board!", ToastNotification.Category.Danger);
    }
}
