using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public sealed class ContractTests
{
    private GameManager gm;
    private TimeController tc;
    private ContractController cc;
    private GameObject gmObj;
    private GameObject tcObj;
    private GameObject ccObj;

    private void ClearSingletons()
    {
        SetStaticInstance<GameManager>(null);
        SetStaticInstance<TimeController>(null);
        SetStaticInstance<ContractController>(null);
        SetStaticInstance<GameEventController>(null);
    }

    private void SetStaticInstance<T>(T value) where T : class
    {
        var prop = typeof(T).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public);
        if (prop != null)
        {
            var setter = prop.GetSetMethod(true);
            setter?.Invoke(null, new object[] { value });
        }
    }

    [SetUp]
    public void SetUp()
    {
        ClearSingletons();

        gmObj = new GameObject("GameManager");
        gm = gmObj.AddComponent<GameManager>();
        typeof(GameManager).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(gm, null);
        gm.StartNewGame("Test Company");

        tcObj = new GameObject("TimeController");
        tc = tcObj.AddComponent<TimeController>();
        typeof(TimeController).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(tc, null);

        ccObj = new GameObject("ContractController");
        cc = ccObj.AddComponent<ContractController>();
        typeof(ContractController).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(cc, null);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(ccObj);
        Object.DestroyImmediate(tcObj);
        Object.DestroyImmediate(gmObj);
        ClearSingletons();
    }

    [Test]
    public void ContractGeneration_RespectsTechUnlocks()
    {
        // 1. Initially, only Vision contracts should be offered
        cc.OfferedContracts.Clear();
        cc.ActiveContracts.Clear();
        
        gm.LoadGameState("Test", 25000f, 10f, 0f, 1, 1, 800f, 0, 0f, false, false);
        cc.GenerateNewOfferedContract();
        
        Assert.AreEqual(1, cc.OfferedContracts.Count);
        Assert.AreEqual(PrototypeProjectController.ModelType.Vision, cc.OfferedContracts[0].modelType);

        // 2. Unlock NLP, should be able to generate NLP
        gm.UnlockNlp();
        cc.OfferedContracts.Clear();
        
        // Generate multiple times to ensure we hit NLP
        for (int i = 0; i < 20; i++)
        {
            cc.GenerateNewOfferedContract();
        }
        
        bool hasNlp = false;
        foreach (var c in cc.OfferedContracts)
        {
            if (c.modelType == PrototypeProjectController.ModelType.NLP) hasNlp = true;
        }
        Assert.IsTrue(hasNlp, "Should generate NLP contract since NLP is unlocked");
    }

    [Test]
    public void ContractAcceptance_DeductsFromOffers_AndAppliesUpfront()
    {
        cc.OfferedContracts.Clear();
        cc.ActiveContracts.Clear();

        cc.GenerateNewOfferedContract();
        Assert.AreEqual(1, cc.OfferedContracts.Count);
        Assert.AreEqual(0, cc.ActiveContracts.Count);

        var c = cc.OfferedContracts[0];
        float initialCash = gm.Cash;
        float upfront = c.upfrontPayment;

        cc.AcceptContract(c);

        Assert.AreEqual(0, cc.OfferedContracts.Count);
        Assert.AreEqual(1, cc.ActiveContracts.Count);
        Assert.IsTrue(c.isAccepted);
        Assert.AreEqual(initialCash + upfront, gm.Cash);
    }

    [Test]
    public void ContractAcceptance_EnforcesActiveLimitOfThree()
    {
        cc.OfferedContracts.Clear();
        cc.ActiveContracts.Clear();

        for (int i = 0; i < 4; i++)
        {
            var c = new ContractController.Contract
            {
                id = $"c_{i}",
                clientName = $"Client {i}",
                isAccepted = false
            };
            cc.OfferedContracts.Add(c);
        }

        cc.AcceptContract(cc.OfferedContracts[0]);
        cc.AcceptContract(cc.OfferedContracts[0]);
        cc.AcceptContract(cc.OfferedContracts[0]);
        
        // 4th acceptance should fail
        cc.AcceptContract(cc.OfferedContracts[0]);

        Assert.AreEqual(3, cc.ActiveContracts.Count);
        Assert.AreEqual(1, cc.OfferedContracts.Count);
    }

    [Test]
    public void ContractDelivery_AppliesPayout_AndRemovesActive()
    {
        cc.OfferedContracts.Clear();
        cc.ActiveContracts.Clear();

        var c = new ContractController.Contract
        {
            id = "c_1",
            clientName = "Stark Industries",
            modelType = PrototypeProjectController.ModelType.Vision,
            qualityThreshold = 60f,
            completionPayout = 5000f,
            isAccepted = true
        };
        cc.ActiveContracts.Add(c);

        float initialCash = gm.Cash;

        // Try delivering with too low quality - should fail
        bool result = cc.TryDeliverContract(PrototypeProjectController.ModelType.Vision, 50f);
        Assert.IsFalse(result);
        Assert.AreEqual(1, cc.ActiveContracts.Count);
        Assert.AreEqual(initialCash, gm.Cash);

        // Try delivering wrong type - should fail
        result = cc.TryDeliverContract(PrototypeProjectController.ModelType.NLP, 70f);
        Assert.IsFalse(result);
        Assert.AreEqual(1, cc.ActiveContracts.Count);

        // Deliver correctly
        result = cc.TryDeliverContract(PrototypeProjectController.ModelType.Vision, 65f);
        Assert.IsTrue(result);
        Assert.AreEqual(0, cc.ActiveContracts.Count);
        Assert.AreEqual(initialCash + 5000f, gm.Cash);
    }

    [Test]
    public void ContractFailure_AppliesPenalties_AndRemovesActive()
    {
        cc.OfferedContracts.Clear();
        cc.ActiveContracts.Clear();

        var c = new ContractController.Contract
        {
            id = "c_1",
            clientName = "Umbrella Corp",
            failurePenalty = 3000f,
            reputationPenalty = 10f,
            daysRemaining = 1,
            isAccepted = true
        };
        cc.ActiveContracts.Add(c);

        float initialCash = gm.Cash;
        float initialRep = gm.Reputation;

        // Force a day pass to trigger expiration failure
        typeof(ContractController).GetMethod("HandleDayPassed", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(cc, null);

        Assert.AreEqual(0, cc.ActiveContracts.Count);
        Assert.AreEqual(initialCash - 3000f, gm.Cash);
        Assert.AreEqual(initialRep - 10f, gm.Reputation);
    }

    [Test]
    public void GlobalAIEvents_ApplyShortageModifiers()
    {
        Assert.AreEqual(1.0f, gm.GpuCostMultiplier);

        var evObj = new GameObject("GameEventController");
        var ec = evObj.AddComponent<GameEventController>();
        typeof(GameEventController).GetMethod("Awake", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ec, null);

        var ev = new GameEvent
        {
            id = "global_gpu_shortage",
            title = "GPU Shortage Test"
        };

        // Choice A: absorb prices (+50% cost)
        typeof(GameEventController).GetMethod("ApplyGlobalEffects", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ec, new object[] { ev, true });
        Assert.AreEqual(1.5f, gm.GpuCostMultiplier);
        Assert.AreEqual(3, gm.GpuShortageRemainingMonths);

        // Advance a month
        gm.ApplyMonthlyBurn();
        Assert.AreEqual(2, gm.GpuShortageRemainingMonths);
        Assert.AreEqual(1.5f, gm.GpuCostMultiplier);

        // Advance two more months to expire
        gm.ApplyMonthlyBurn();
        gm.ApplyMonthlyBurn();
        Assert.AreEqual(0, gm.GpuShortageRemainingMonths);
        Assert.AreEqual(1.0f, gm.GpuCostMultiplier);

        Object.DestroyImmediate(evObj);
    }
}
