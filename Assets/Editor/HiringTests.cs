using NUnit.Framework;
using UnityEngine;

public sealed class HiringTests
{
    [Test]
    public void SpendCash_DeductsFromCashBalance()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        Assert.AreEqual(25000f, gm.Cash);

        bool success = gm.SpendCash(5000f);
        Assert.IsTrue(success);
        Assert.AreEqual(20000f, gm.Cash);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void SpendCash_FailsWhenInsufficientCash()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        bool success = gm.SpendCash(30000f);
        Assert.IsFalse(success);
        Assert.AreEqual(25000f, gm.Cash);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void HireMLEngineer_ModifiesBurnAndTeamSize()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        Assert.AreEqual(1, gm.TeamSize);
        Assert.AreEqual(800f, gm.MonthlyBurn);

        gm.SetTeamSize(gm.TeamSize + 1);
        gm.SetMonthlyBurn(gm.MonthlyBurn + 1200f);

        Assert.AreEqual(2, gm.TeamSize);
        Assert.AreEqual(2000f, gm.MonthlyBurn);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void GameManager_LoadGameState_RestoresStateCorrectly()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();

        gm.LoadGameState("Saved Company", 15000f, 45f, 75f, 3, 2, 2200f, 12, 4500f, false, false, 10f);

        Assert.AreEqual("Saved Company", gm.CompanyName);
        Assert.AreEqual(15000f, gm.Cash);
        Assert.AreEqual(45f, gm.Reputation);
        Assert.AreEqual(75f, gm.ModelQuality);
        Assert.AreEqual(3, gm.TeamSize);
        Assert.AreEqual(2, gm.GpuCount);
        Assert.AreEqual(2300f, gm.MonthlyBurn);
        Assert.AreEqual(12, gm.TotalClients);
        Assert.AreEqual(4500f, gm.MonthlyRevenue);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void BuyGpuUpgrade_SucceedsAndModifiesState()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        Assert.AreEqual(1, gm.GpuCount);
        Assert.AreEqual(800f, gm.MonthlyBurn);
        float initialCash = gm.Cash; // 25000

        bool success = gm.BuyGpuUpgrade(10000f, 300f);
        Assert.IsTrue(success);
        Assert.AreEqual(2, gm.GpuCount);
        Assert.AreEqual(1100f, gm.MonthlyBurn);
        Assert.AreEqual(initialCash - 10000f, gm.Cash);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void BuyGpuUpgrade_FailsWhenInsufficientCash()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");
        
        gm.SpendCash(20000f);
        float remainingCash = gm.Cash; // 5000f

        Assert.AreEqual(1, gm.GpuCount);
        bool success = gm.BuyGpuUpgrade(10000f, 300f);
        Assert.IsFalse(success);
        Assert.AreEqual(1, gm.GpuCount);
        Assert.AreEqual(800f, gm.MonthlyBurn);
        Assert.AreEqual(remainingCash, gm.Cash);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void TimeController_LoadDateTime_RestoresTimeCorrectly()
    {
        var go = new GameObject("TimeController");
        var tc = go.AddComponent<TimeController>();

        tc.LoadDateTime(2023, 11, 15, 250);

        Assert.AreEqual(2023, tc.Year);
        Assert.AreEqual(11, tc.Month);
        Assert.AreEqual(15, tc.Day);
        Assert.AreEqual(250, tc.TotalDays);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void RecalculateMonthlyBurn_CorrectlyAppliesModifiers()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        // Base burn is 800
        Assert.AreEqual(800f, gm.MonthlyBurn);

        // Hire ML Engineer ($1,200/mo)
        gm.SetHasMLEngineer(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(2000f, gm.MonthlyBurn);

        // Upgrade to Tier 2 (+ $1,000/mo)
        gm.SetOfficeTier(2);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(3000f, gm.MonthlyBurn);

        // Hire Research Scientist ($2,500/mo)
        gm.SetHasResearchScientist(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(5500f, gm.MonthlyBurn);

        // Hire Data Engineer ($1,800/mo)
        gm.SetHasDataEngineer(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(7300f, gm.MonthlyBurn);

        // Buy GPUs (GpuCount = 2, so 1 extra GPU @ $300/mo)
        gm.BuyGpuUpgrade(0f, 300f);
        Assert.AreEqual(7600f, gm.MonthlyBurn);

        // Upgrade to Tier 3 (+ $3,500/mo instead of Tier 2's $1,000/mo)
        gm.SetOfficeTier(3);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(10100f, gm.MonthlyBurn);

        // Research Custom Silicon (halves GPU burn to $150/mo)
        gm.SetCustomSiliconResearched(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(9950f, gm.MonthlyBurn);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void ResearchUnlocks_AdvancedRAndD()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        Assert.IsFalse(gm.IsSafetyAlignmentResearched);
        Assert.IsFalse(gm.IsCustomSiliconResearched);

        gm.SetSafetyAlignmentResearched(true);
        gm.SetCustomSiliconResearched(true);

        Assert.IsTrue(gm.IsSafetyAlignmentResearched);
        Assert.IsTrue(gm.IsCustomSiliconResearched);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void SaveLoadSerialization_RestoresPhase4State()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();

        gm.LoadGameState("Saved Company", 15000f, 45f, 75f, 3, 2, 2200f, 12, 4500f, true, true, 10f, 100, 50, 3, true, true, true, true, true, true);

        Assert.AreEqual(3, gm.OfficeTier);
        Assert.IsTrue(gm.HasMLEngineer);
        Assert.IsTrue(gm.HasResearchScientist);
        Assert.IsTrue(gm.HasDataEngineer);
        Assert.IsTrue(gm.HasSafetyResearcher);
        Assert.IsTrue(gm.IsSafetyAlignmentResearched);
        Assert.IsTrue(gm.IsCustomSiliconResearched);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void AnalyticsHistory_RecordsAndRestoresCorrectly()
    {
        var go = new GameObject("AnalyticsController");
        var ac = go.AddComponent<AnalyticsController>();

        var awakeMethod = typeof(AnalyticsController).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (awakeMethod != null) awakeMethod.Invoke(ac, null);

        Assert.AreEqual(ac, AnalyticsController.Instance);

        ac.AddModelLaunch("TestModel", 85f, 1500f);
        Assert.AreEqual(1, ac.ModelHistoryLog.Count);
        Assert.AreEqual("TestModel", ac.ModelHistoryLog[0].name);
        Assert.AreEqual(85f, ac.ModelHistoryLog[0].quality);
        Assert.AreEqual(1500f, ac.ModelHistoryLog[0].revenue);
        Assert.AreEqual(1, ac.QualityHistory.Count);
        Assert.AreEqual(85f, ac.QualityHistory[0]);

        var cash = new System.Collections.Generic.List<float> { 1000f, 2000f };
        var followers = new System.Collections.Generic.List<float> { 10f, 20f };
        var quality = new System.Collections.Generic.List<float> { 80f, 90f };
        var models = new System.Collections.Generic.List<AnalyticsController.ModelRecord> {
            new AnalyticsController.ModelRecord { name = "ModelA", quality = 80f, revenue = 1000f }
        };

        ac.RestoreHistory(cash, followers, quality, models);
        Assert.AreEqual(2, ac.CashHistory.Count);
        Assert.AreEqual(2, ac.FollowersHistory.Count);
        Assert.AreEqual(2, ac.QualityHistory.Count);
        Assert.AreEqual(1, ac.ModelHistoryLog.Count);
        Assert.AreEqual("ModelA", ac.ModelHistoryLog[0].name);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void RecalculateMonthlyBurn_AppliesFase7Modifiers()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        // Base burn is 800
        Assert.AreEqual(800f, gm.MonthlyBurn);

        // Hire Infrastructure Engineer ($2,800/mo)
        gm.SetHasInfrastructureEngineer(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(3600f, gm.MonthlyBurn);

        // Hire GPU Technician ($1,900/mo)
        gm.SetHasGpuTechnician(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(5500f, gm.MonthlyBurn);

        // Hire MLOps Engineer ($2,200/mo)
        gm.SetHasMlopsEngineer(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(7700f, gm.MonthlyBurn);

        // Hire Backend Engineer ($1,500/mo)
        gm.SetHasBackendEngineer(true);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(9200f, gm.MonthlyBurn);

        // Buy Grid Upgrades (GridUpgrades = 2, so 2 @ $100/mo)
        gm.BuyEnergyGridUpgrade(0f);
        gm.BuyEnergyGridUpgrade(0f);
        Assert.AreEqual(9400f, gm.MonthlyBurn);

        // Buy Cooling Upgrades (CoolingUpgrades = 2, so 2 @ $80/mo)
        gm.BuyCoolingUpgrade(0f);
        gm.BuyCoolingUpgrade(0f);
        Assert.AreEqual(9560f, gm.MonthlyBurn);

        // Set Office Tier to 4 ($5,000/mo)
        gm.SetOfficeTier(4);
        gm.RecalculateMonthlyBurn();
        Assert.AreEqual(14560f, gm.MonthlyBurn);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void NocSystem_OverheatStateTriggeredCorrectly()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        // Base capacity: OfficeTier = 1, so BaseEnergyCapacity = 15f, BaseCoolingCapacity = 15f.
        // GpuCount = 1, so EnergyUsage = 10f, CoolingUsage = 10f.
        Assert.IsFalse(gm.IsOverheating);

        // Add GPUs so usage exceeds base capacity
        gm.BuyGpuUpgrade(0f, 0f);
        Assert.AreEqual(2, gm.GpuCount);
        Assert.IsTrue(gm.IsOverheating);

        // Upgrade Energy Grid (+30kW capacity, total capacity = 45kW)
        gm.BuyEnergyGridUpgrade(0f);
        Assert.IsTrue(gm.IsOverheating); // Cooling capacity (15f) is still exceeded by usage (20f)

        // Upgrade Cooling (+30kW capacity, total capacity = 45kW)
        gm.BuyCoolingUpgrade(0f);
        Assert.IsFalse(gm.IsOverheating);

        // Test reduction modifiers
        // Buy GPUs up to 5. EnergyUsage = 50kW, CoolingUsage = 50kW
        gm.BuyGpuUpgrade(0f, 0f);
        gm.BuyGpuUpgrade(0f, 0f);
        gm.BuyGpuUpgrade(0f, 0f);
        Assert.AreEqual(5, gm.GpuCount);
        Assert.IsTrue(gm.IsOverheating);

        // Hire Infrastructure Engineer -> EnergyUsage becomes 50 * 0.75 = 37.5kW (<= 45kW capacity)
        gm.SetHasInfrastructureEngineer(true);
        Assert.IsTrue(gm.IsOverheating); // Cooling is still overheating (50kW > 45kW capacity)

        // Hire GPU Technician -> CoolingUsage becomes 50 * 0.9 = 45kW (<= 45kW capacity)
        gm.SetHasGpuTechnician(true);
        Assert.IsFalse(gm.IsOverheating);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void BackendEngineer_IncreasesMaxContracts()
    {
        // Clear any existing static instances first
        var gmProp = typeof(GameManager).GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        gmProp?.GetSetMethod(true)?.Invoke(null, new object[] { null });

        var ccProp = typeof(ContractController).GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        ccProp?.GetSetMethod(true)?.Invoke(null, new object[] { null });

        var gmGo = new GameObject("GameManager");
        var gm = gmGo.AddComponent<GameManager>();
        typeof(GameManager).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(gm, null);
        gm.StartNewGame("Test Company");

        var ccGo = new GameObject("ContractController");
        var cc = ccGo.AddComponent<ContractController>();
        typeof(ContractController).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(cc, null);
        Assert.AreEqual(cc, ContractController.Instance);

        var contract1 = new ContractController.Contract { id = "c1", upfrontPayment = 100f, isAccepted = false };
        var contract2 = new ContractController.Contract { id = "c2", upfrontPayment = 100f, isAccepted = false };
        var contract3 = new ContractController.Contract { id = "c3", upfrontPayment = 100f, isAccepted = false };
        var contract4 = new ContractController.Contract { id = "c4", upfrontPayment = 100f, isAccepted = false };
        var contract5 = new ContractController.Contract { id = "c5", upfrontPayment = 100f, isAccepted = false };

        cc.OfferedContracts.Add(contract1);
        cc.OfferedContracts.Add(contract2);
        cc.OfferedContracts.Add(contract3);
        cc.OfferedContracts.Add(contract4);
        cc.OfferedContracts.Add(contract5);

        cc.AcceptContract(contract1);
        cc.AcceptContract(contract2);
        cc.AcceptContract(contract3);
        Assert.AreEqual(3, cc.ActiveContracts.Count);

        cc.AcceptContract(contract4);
        Assert.AreEqual(3, cc.ActiveContracts.Count);
        Assert.IsFalse(contract4.isAccepted);

        gm.SetHasBackendEngineer(true);

        cc.AcceptContract(contract4);
        Assert.AreEqual(4, cc.ActiveContracts.Count);
        Assert.IsTrue(contract4.isAccepted);

        cc.AcceptContract(contract5);
        Assert.AreEqual(4, cc.ActiveContracts.Count);
        Assert.IsFalse(contract5.isAccepted);

        Object.DestroyImmediate(ccGo);
        Object.DestroyImmediate(gmGo);

        // Clear static instances again at the end
        gmProp?.GetSetMethod(true)?.Invoke(null, new object[] { null });
        ccProp?.GetSetMethod(true)?.Invoke(null, new object[] { null });
    }

    [Test]
    public void VCFinancing_DilutesEquityAndAddsCash()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        Assert.AreEqual(100f, gm.FounderEquity);
        Assert.AreEqual(0, gm.FundingRound);
        float baseCash = gm.Cash;

        // Series A: dilutes 15% (equity -> 85%), cash +150k
        gm.AcceptNextFundingRound();
        Assert.AreEqual(85f, gm.FounderEquity);
        Assert.AreEqual(1, gm.FundingRound);
        Assert.AreEqual(baseCash + 150000f, gm.Cash);
        Assert.AreEqual(100f, gm.BoardTrust);
        Assert.AreNotEqual("None", gm.ActiveBoardGoalType);

        // Hire Finance Lead -> gives +20% cash round bonus
        gm.SetHasFinanceLead(true);
        baseCash = gm.Cash;

        // Series B: dilutes 20% (equity -> 65%), cash +400k * 1.2 = +480k
        gm.AcceptNextFundingRound();
        Assert.AreEqual(65f, gm.FounderEquity);
        Assert.AreEqual(2, gm.FundingRound);
        Assert.AreEqual(baseCash + 480000f, gm.Cash);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void BoardGoals_EvaluatesQuarterlySuccessAndFailure()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        // Accept Series A to initialize Board Room
        gm.AcceptNextFundingRound();
        Assert.AreEqual(100f, gm.BoardTrust);

        // Set up a goal: Followers target 200, remaining months = 3
        gm.LoadGameState(
            gm.CompanyName, gm.Cash, gm.Reputation, gm.ModelQuality, gm.TeamSize, gm.GpuCount, gm.MonthlyBurn, gm.TotalClients, gm.MonthlyRevenue,
            gm.IsNlpUnlocked, gm.IsAgenticUnlocked, gm.Competence, 100, gm.Following, gm.OfficeTier,
            gm.HasMLEngineer, gm.HasResearchScientist, gm.HasDataEngineer, gm.HasSafetyResearcher, gm.IsSafetyAlignmentResearched, gm.IsCustomSiliconResearched,
            gm.GpuCostMultiplier, gm.IsDataRegulationActive, gm.ReputationBoostMultiplier, gm.GpuShortageRemainingMonths, gm.HypeWaveRemainingMonths, gm.DataRegulationRemainingMonths,
            gm.HasInfrastructureEngineer, gm.HasGpuTechnician, gm.HasMlopsEngineer, gm.HasBackendEngineer, gm.EnergyGridUpgrades, gm.CoolingUpgrades,
            gm.FounderEquity, 100f, gm.FundingRound, gm.HasAcquiredQuantumMinds, gm.HasAcquiredAnthroTech,
            gm.HasFinanceLead, gm.HasRecruiter, gm.HasProductManager, gm.HasSalesExecutive, gm.HasCommunityManager,
            "Followers", 200f, 3, false
        );

        // Ticks 3 months. In month 1 and 2, remaining months drops, trust remains 100.
        gm.ApplyMonthlyBurn();
        Assert.AreEqual(100f, gm.BoardTrust);
        gm.ApplyMonthlyBurn();
        Assert.AreEqual(100f, gm.BoardTrust);

        // Tick 3rd month. Target is 200, current followers is 100 -> fails. Trust drops from 100 to 80.
        gm.ApplyMonthlyBurn();
        Assert.AreEqual(80f, gm.BoardTrust);

        // Set up next goal: Followers target 120, remaining months = 3. Set current followers to 150.
        gm.LoadGameState(
            gm.CompanyName, gm.Cash, gm.Reputation, gm.ModelQuality, gm.TeamSize, gm.GpuCount, gm.MonthlyBurn, gm.TotalClients, gm.MonthlyRevenue,
            gm.IsNlpUnlocked, gm.IsAgenticUnlocked, gm.Competence, 150, gm.Following, gm.OfficeTier,
            gm.HasMLEngineer, gm.HasResearchScientist, gm.HasDataEngineer, gm.HasSafetyResearcher, gm.IsSafetyAlignmentResearched, gm.IsCustomSiliconResearched,
            gm.GpuCostMultiplier, gm.IsDataRegulationActive, gm.ReputationBoostMultiplier, gm.GpuShortageRemainingMonths, gm.HypeWaveRemainingMonths, gm.DataRegulationRemainingMonths,
            gm.HasInfrastructureEngineer, gm.HasGpuTechnician, gm.HasMlopsEngineer, gm.HasBackendEngineer, gm.EnergyGridUpgrades, gm.CoolingUpgrades,
            gm.FounderEquity, 80f, gm.FundingRound, gm.HasAcquiredQuantumMinds, gm.HasAcquiredAnthroTech,
            gm.HasFinanceLead, gm.HasRecruiter, gm.HasProductManager, gm.HasSalesExecutive, gm.HasCommunityManager,
            "Followers", 120f, 3, false
        );

        gm.ApplyMonthlyBurn();
        gm.ApplyMonthlyBurn();
        // 3rd month tick -> success -> trust should go from 80 to 95.
        gm.ApplyMonthlyBurn();
        Assert.AreEqual(95f, gm.BoardTrust);

        Object.DestroyImmediate(go);
    }

    [Test]
    public void BoardRoom_GameOverOnZeroTrust()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        // Set up TimeController mock/dummy
        var tcGo = new GameObject("TimeController");
        var tc = tcGo.AddComponent<TimeController>();
        typeof(TimeController).GetMethod("Awake", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(tc, null);

        gm.AcceptNextFundingRound();

        // Set BoardTrust to 10f (so one failure causes GameOver) and target followers to 1000
        gm.LoadGameState(
            gm.CompanyName, gm.Cash, gm.Reputation, gm.ModelQuality, gm.TeamSize, gm.GpuCount, gm.MonthlyBurn, gm.TotalClients, gm.MonthlyRevenue,
            gm.IsNlpUnlocked, gm.IsAgenticUnlocked, gm.Competence, 10, gm.Following, gm.OfficeTier,
            gm.HasMLEngineer, gm.HasResearchScientist, gm.HasDataEngineer, gm.HasSafetyResearcher, gm.IsSafetyAlignmentResearched, gm.IsCustomSiliconResearched,
            gm.GpuCostMultiplier, gm.IsDataRegulationActive, gm.ReputationBoostMultiplier, gm.GpuShortageRemainingMonths, gm.HypeWaveRemainingMonths, gm.DataRegulationRemainingMonths,
            gm.HasInfrastructureEngineer, gm.HasGpuTechnician, gm.HasMlopsEngineer, gm.HasBackendEngineer, gm.EnergyGridUpgrades, gm.CoolingUpgrades,
            gm.FounderEquity, 10f, gm.FundingRound, gm.HasAcquiredQuantumMinds, gm.HasAcquiredAnthroTech,
            gm.HasFinanceLead, gm.HasRecruiter, gm.HasProductManager, gm.HasSalesExecutive, gm.HasCommunityManager,
            "Followers", 1000f, 1, false
        );

        Assert.IsFalse(gm.IsGameOver);
        gm.ApplyMonthlyBurn();
        Assert.IsTrue(gm.IsGameOver);

        Object.DestroyImmediate(tcGo);
        Object.DestroyImmediate(go);

        // Clear static instances
        var tcProp = typeof(TimeController).GetProperty("Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
        tcProp?.GetSetMethod(true)?.Invoke(null, new object[] { null });
    }

    [Test]
    public void StartupAcquisitions_AndSpecialists_ApplyBonuses()
    {
        var go = new GameObject("GameManager");
        var gm = go.AddComponent<GameManager>();
        gm.StartNewGame("Test Company");

        // 1. Finance Lead perks
        Assert.AreEqual(800f, gm.MonthlyBurn);
        gm.SetHasFinanceLead(true);
        // Base burn for Founder (800) + Finance Lead (3500) = 4300.
        // Discount 10% on burn: 4300 * 0.9 = 3870.
        Assert.AreEqual(3870f, gm.MonthlyBurn);

        // 2. M&A Quantum Minds & PM Speed modifier on training duration
        gm.SetHasAcquiredQuantumMinds(true);
        Assert.IsTrue(gm.HasAcquiredQuantumMinds);

        gm.SetHasAcquiredAnthroTech(true);
        Assert.IsTrue(gm.HasAcquiredAnthroTech);

        Object.DestroyImmediate(go);
    }
}
