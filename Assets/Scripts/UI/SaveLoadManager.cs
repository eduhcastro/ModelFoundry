using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

public sealed class SaveLoadManager : MonoBehaviour
{
    [Header("UI Buttons")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button loadButton;

    [Header("Dependencies")]
    [SerializeField] private HiringController hiringController;
    [SerializeField] private GameObject secondGpuCabinet;

    private string SavePath => Path.Combine(Application.persistentDataPath, "save.json");

    private void Start()
    {
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(SaveGame);
        }
        
        if (loadButton != null)
        {
            loadButton.onClick.AddListener(LoadGame);
        }

        if (GameManager.Instance != null && GameManager.Instance.ShouldLoadSaveOnStart)
        {
            GameManager.Instance.ShouldLoadSaveOnStart = false; // Reset the flag
            LoadGame();
        }
    }

    public void SaveGame()
    {
        var gm = GameManager.Instance;
        var tc = TimeController.Instance;

        if (gm == null || tc == null) return;

        var data = new SaveData
        {
            companyName = gm.CompanyName,
            cash = gm.Cash,
            reputation = gm.Reputation,
            modelQuality = gm.ModelQuality,
            teamSize = gm.TeamSize,
            monthlyBurn = gm.MonthlyBurn,
            totalClients = gm.TotalClients,
            monthlyRevenue = gm.MonthlyRevenue,

            year = tc.Year,
            month = tc.Month,
            day = tc.Day,
            totalDays = tc.TotalDays,

            hasHiredMLEngineer = gm.HasMLEngineer,
            gpuCount = gm.GpuCount,
            isNlpUnlocked = gm.IsNlpUnlocked,
            isAgenticUnlocked = gm.IsAgenticUnlocked,
            competence = gm.Competence,
            followers = gm.Followers,
            following = gm.Following,

            officeTier = gm.OfficeTier,
            hasHiredResearchScientist = gm.HasResearchScientist,
            hasHiredDataEngineer = gm.HasDataEngineer,
            hasHiredSafetyResearcher = gm.HasSafetyResearcher,
            isSafetyAlignmentResearched = gm.IsSafetyAlignmentResearched,
            isCustomSiliconResearched = gm.IsCustomSiliconResearched,

            // Phase 6 variables
            gpuCostMultiplier = gm.GpuCostMultiplier,
            isDataRegulationActive = gm.IsDataRegulationActive,
            reputationBoostMultiplier = gm.ReputationBoostMultiplier,
            gpuShortageRemainingMonths = gm.GpuShortageRemainingMonths,
            hypeWaveRemainingMonths = gm.HypeWaveRemainingMonths,
            dataRegulationRemainingMonths = gm.DataRegulationRemainingMonths,

            // Phase 7 variables
            hasHiredInfrastructureEngineer = gm.HasInfrastructureEngineer,
            hasHiredGpuTechnician = gm.HasGpuTechnician,
            hasHiredMlopsEngineer = gm.HasMlopsEngineer,
            hasHiredBackendEngineer = gm.HasBackendEngineer,
            energyGridUpgrades = gm.EnergyGridUpgrades,
            coolingUpgrades = gm.CoolingUpgrades,

            // Phase 8 variables
            fundingRound = gm.FundingRound,
            founderEquity = gm.FounderEquity,
            boardTrust = gm.BoardTrust,
            activeBoardGoalType = gm.ActiveBoardGoalType,
            boardGoalTarget = gm.BoardGoalTarget,
            boardGoalRemainingMonths = gm.BoardGoalRemainingMonths,
            isGameOver = gm.IsGameOver,
            hasAcquiredQuantumMinds = gm.HasAcquiredQuantumMinds,
            hasAcquiredAnthroTech = gm.HasAcquiredAnthroTech,
            hasHiredFinanceLead = gm.HasFinanceLead,
            hasHiredRecruiter = gm.HasRecruiter,
            hasHiredProductManager = gm.HasProductManager,
            hasHiredSalesExecutive = gm.HasSalesExecutive,
            hasHiredCommunityManager = gm.HasCommunityManager
        };

        if (AnalyticsController.Instance != null)
        {
            data.cashHistory = new List<float>(AnalyticsController.Instance.CashHistory);
            data.followersHistory = new List<float>(AnalyticsController.Instance.FollowersHistory);
            data.qualityHistory = new List<float>(AnalyticsController.Instance.QualityHistory);
            data.modelHistory = new List<AnalyticsController.ModelRecord>(AnalyticsController.Instance.ModelHistoryLog);
        }

        if (ContractController.Instance != null)
        {
            data.offeredContracts = new List<ContractController.Contract>(ContractController.Instance.OfferedContracts);
            data.activeContracts = new List<ContractController.Contract>(ContractController.Instance.ActiveContracts);
        }

        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);
            ToastNotification.ShowGlobal("Game saved successfully!", ToastNotification.Category.Success);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            ToastNotification.ShowGlobal("Save failed!", ToastNotification.Category.Danger);
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            ToastNotification.ShowGlobal("No saved game found!", ToastNotification.Category.Warning);
            return;
        }

        var gm = GameManager.Instance;
        var tc = TimeController.Instance;

        if (gm == null || tc == null) return;

        try
        {
            string json = File.ReadAllText(SavePath);
            var data = JsonUtility.FromJson<SaveData>(json);

            // Restore GameManager state
            gm.LoadGameState(
                data.companyName,
                data.cash,
                data.reputation,
                data.modelQuality,
                data.teamSize,
                data.gpuCount,
                data.monthlyBurn,
                data.totalClients,
                data.monthlyRevenue,
                data.isNlpUnlocked,
                data.isAgenticUnlocked,
                data.competence,
                data.followers,
                data.following,
                data.officeTier == 0 ? 1 : data.officeTier,
                data.hasHiredMLEngineer,
                data.hasHiredResearchScientist,
                data.hasHiredDataEngineer,
                data.hasHiredSafetyResearcher,
                data.isSafetyAlignmentResearched,
                data.isCustomSiliconResearched,
                data.gpuCostMultiplier == 0f ? 1.0f : data.gpuCostMultiplier,
                data.isDataRegulationActive,
                data.reputationBoostMultiplier == 0f ? 1.0f : data.reputationBoostMultiplier,
                data.gpuShortageRemainingMonths,
                data.hypeWaveRemainingMonths,
                data.dataRegulationRemainingMonths,
                data.hasHiredInfrastructureEngineer,
                data.hasHiredGpuTechnician,
                data.hasHiredMlopsEngineer,
                data.hasHiredBackendEngineer,
                data.energyGridUpgrades,
                data.coolingUpgrades,
                data.founderEquity == 0f ? 100f : data.founderEquity,
                data.boardTrust == 0f && data.fundingRound == 0 ? 100f : data.boardTrust,
                data.fundingRound,
                data.hasAcquiredQuantumMinds,
                data.hasAcquiredAnthroTech,
                data.hasHiredFinanceLead,
                data.hasHiredRecruiter,
                data.hasHiredProductManager,
                data.hasHiredSalesExecutive,
                data.hasHiredCommunityManager,
                string.IsNullOrEmpty(data.activeBoardGoalType) ? "None" : data.activeBoardGoalType,
                data.boardGoalTarget,
                data.boardGoalRemainingMonths,
                data.isGameOver
            );

            // Restore Analytics history
            if (AnalyticsController.Instance != null)
            {
                AnalyticsController.Instance.RestoreHistory(
                    data.cashHistory,
                    data.followersHistory,
                    data.qualityHistory,
                    data.modelHistory
                );
            }

            // Restore Contracts state
            if (ContractController.Instance != null)
            {
                ContractController.Instance.LoadContracts(data.offeredContracts, data.activeContracts);
            }

            // Restore Office expansion visual state
            if (OfficeVisualController.Instance != null)
            {
                OfficeVisualController.Instance.ApplyOfficeVisuals(gm.OfficeTier);
            }

            // Restore second GPU Cabinet visual state
            if (secondGpuCabinet != null)
            {
                secondGpuCabinet.SetActive(data.gpuCount >= 2);
            }

            // Restore TimeController state
            tc.LoadDateTime(data.year, data.month, data.day, data.totalDays);

            // Restore HiringController visual and runtime state for ALL roles
            if (hiringController != null)
            {
                hiringController.LoadHiredStates(
                    gm.HasMLEngineer,
                    gm.HasResearchScientist,
                    gm.HasDataEngineer,
                    gm.HasSafetyResearcher,
                    gm.HasInfrastructureEngineer,
                    gm.HasGpuTechnician,
                    gm.HasMlopsEngineer,
                    gm.HasBackendEngineer,
                    gm.HasFinanceLead,
                    gm.HasRecruiter,
                    gm.HasProductManager,
                    gm.HasSalesExecutive,
                    gm.HasCommunityManager
                );
            }

            ToastNotification.ShowGlobal("Game loaded successfully!", ToastNotification.Category.Success);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            ToastNotification.ShowGlobal("Load failed!", ToastNotification.Category.Danger);
        }
    }

    [System.Serializable]
    private class SaveData
    {
        public string companyName;
        public float cash;
        public float reputation;
        public float modelQuality;
        public int teamSize;
        public float monthlyBurn;
        public int totalClients;
        public float monthlyRevenue;

        public int year;
        public int month;
        public int day;
        public int totalDays;

        public bool hasHiredMLEngineer;
        public int gpuCount;
        public bool isNlpUnlocked;
        public bool isAgenticUnlocked;
        public float competence;
        public int followers;
        public int following;

        public int officeTier;
        public bool hasHiredResearchScientist;
        public bool hasHiredDataEngineer;
        public bool hasHiredSafetyResearcher;
        public bool isSafetyAlignmentResearched;
        public bool isCustomSiliconResearched;

        // Analytics histories
        public List<float> cashHistory;
        public List<float> followersHistory;
        public List<float> qualityHistory;
        public List<AnalyticsController.ModelRecord> modelHistory;

        // Phase 6 variables
        public float gpuCostMultiplier;
        public bool isDataRegulationActive;
        public float reputationBoostMultiplier;
        public int gpuShortageRemainingMonths;
        public int hypeWaveRemainingMonths;
        public int dataRegulationRemainingMonths;

        // Phase 7 variables
        public bool hasHiredInfrastructureEngineer;
        public bool hasHiredGpuTechnician;
        public bool hasHiredMlopsEngineer;
        public bool hasHiredBackendEngineer;
        public int energyGridUpgrades;
        public int coolingUpgrades;

        // Phase 8 variables
        public int fundingRound;
        public float founderEquity;
        public float boardTrust;
        public string activeBoardGoalType;
        public float boardGoalTarget;
        public int boardGoalRemainingMonths;
        public bool isGameOver;
        public bool hasAcquiredQuantumMinds;
        public bool hasAcquiredAnthroTech;
        public bool hasHiredFinanceLead;
        public bool hasHiredRecruiter;
        public bool hasHiredProductManager;
        public bool hasHiredSalesExecutive;
        public bool hasHiredCommunityManager;

        public List<ContractController.Contract> offeredContracts;
        public List<ContractController.Contract> activeContracts;
    }
}
