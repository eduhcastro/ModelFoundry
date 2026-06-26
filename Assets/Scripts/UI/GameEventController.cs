using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public sealed class GameEventController : MonoBehaviour
{
    public static GameEventController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button choiceAButton;
    [SerializeField] private Button choiceBButton;

    private List<GameEvent> historicalEvents = new List<GameEvent>();
    private List<GameEvent> randomEvents = new List<GameEvent>();
    
    private GameEvent activeEvent;
    private TimeController.Speed speedBeforeEvent;
    private int monthsSinceLastGlobalEvent = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        InitializeEvents();
    }

    private void Start()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnMonthPassed += CheckEventsOnMonthPass;
        }

        if (choiceAButton != null)
        {
            choiceAButton.onClick.AddListener(SelectChoiceA);
        }
        
        if (choiceBButton != null)
        {
            choiceBButton.onClick.AddListener(SelectChoiceB);
        }

        HidePanel();
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnMonthPassed -= CheckEventsOnMonthPass;
        }
    }

    private void InitializeEvents()
    {
        // Populate historical events based on research
        historicalEvents.Add(new GameEvent {
            id = "transformer",
            title = "THE TRANSFORMER REVOLUTION",
            description = "Google researchers publish 'Attention Is All You Need', introducing the Transformer architecture. It completely changes natural language processing.",
            isHistorical = true,
            triggerYear = 2017,
            triggerMonth = 6,
            choiceAText = "Adopt Immediately (-$3k)",
            choiceBText = "Wait and See",
            cashEffectA = -3000f,
            qualityEffectA = 15f,
            toastEffectA = "Adopted Transformers! Training quality boosted."
        });

        historicalEvents.Add(new GameEvent {
            id = "gpt2",
            title = "GPT-2 SAFETY DEBATE",
            description = "OpenAI builds a powerful 1.5 Billion parameter model but halts the release due to fears of deep fakes and spam generation.",
            isHistorical = true,
            triggerYear = 2019,
            triggerMonth = 2,
            choiceAText = "Advocate for Open Source",
            choiceBText = "Advocate for Safe APIs",
            repEffectA = 15f,
            toastEffectA = "Community loves the open source stance! Reputation +15%",
            cashEffectB = 2000f,
            qualityEffectB = 5f,
            toastEffectB = "Corporate partners award safe alignment. Received $2k!"
        });

        historicalEvents.Add(new GameEvent {
            id = "gpt3",
            title = "GPT-3 & VC HYPING",
            description = "GPT-3 is launched. AI startup hype is peaking, and venture capitalists are throwing money at neural labs.",
            isHistorical = true,
            triggerYear = 2020,
            triggerMonth = 5,
            choiceAText = "Pitch to VCs (+$15k Cash)",
            choiceBText = "Stay Independent (+10% Rep)",
            cashEffectA = 15000f,
            repEffectA = -5f,
            toastEffectA = "Secured VC funding! Cash +$15,000.",
            repEffectB = 10f,
            qualityEffectB = 5f,
            toastEffectB = "Remained independent. Community reputation boosted."
        });

        historicalEvents.Add(new GameEvent {
            id = "chatgpt",
            title = "THE CHATGPT MOMENT",
            description = "ChatGPT is released. It reaches 100M users in record time, transforming AI into a mainstream global phenomenon.",
            isHistorical = true,
            triggerYear = 2022,
            triggerMonth = 11,
            choiceAText = "Integrate Chat Interface (-$4k)",
            choiceBText = "Stick to Backend Contracts",
            cashEffectA = -4000f,
            repEffectA = 10f,
            qualityEffectA = 10f,
            toastEffectA = "Launched chatbot! Reputation and model quality increased.",
            cashEffectB = 3000f,
            toastEffectB = "Maintained B2B focus. Earned +$3,000 contract revenue."
        });

        historicalEvents.Add(new GameEvent {
            id = "agentic",
            title = "THE AGENTIC SHIFT",
            description = "AI shifts from passive chat to active agents that can autonomously code and execute complex tasks.",
            isHistorical = true,
            triggerYear = 2025,
            triggerMonth = 12,
            choiceAText = "Automate Operations (-$2k)",
            choiceBText = "Keep Humans (+10% Rep)",
            cashEffectA = -2000f,
            monthlyBurnEffectA = -300f,
            toastEffectA = "Automated infra! Monthly burn reduced by $300.",
            repEffectB = 10f,
            qualityEffectB = 10f,
            toastEffectB = "Hired top human talent. Quality and Reputation boosted."
        });

        historicalEvents.Add(new GameEvent {
            id = "summit",
            title = "GLOBAL AI SUMMIT (2026)",
            description = "We have reached June 24, 2026. A global summit gathers in London to decide on the future of autonomous agent regulation.",
            isHistorical = true,
            triggerYear = 2026,
            triggerMonth = 6,
            choiceAText = "Participate in Safety Talks",
            choiceBText = "Lobby for Open Research",
            repEffectA = 20f,
            toastEffectA = "Academics praise your safety commitment. Reputation +20%!",
            qualityEffectB = 15f,
            toastEffectB = "Engineers love the open research stand. Quality +15%!"
        });

        // Populate random events
        randomEvents.Add(new GameEvent {
            id = "gpu_shortage",
            title = "GPU SHORTAGE",
            description = "A massive chip supply chain bottleneck has spiked the price of GPU cloud instances.",
            isHistorical = false,
            choiceAText = "Pay Premium (-$4k Cash)",
            choiceBText = "Suffer Delays (-10% Quality)",
            cashEffectA = -4000f,
            toastEffectA = "Paid GPU premium. Training speed remains stable.",
            qualityEffectB = -10f,
            toastEffectB = "Delayed training. Model quality decreased due to lack of compute."
        });

        randomEvents.Add(new GameEvent {
            id = "data_leak",
            title = "DATA SECURITY SCARE",
            description = "A minor breach reports that proprietary training logs were temporarily exposed.",
            isHistorical = false,
            choiceAText = "Pay PR firm (-$1.5k Cash)",
            choiceBText = "Do Nothing (-15% Rep)",
            cashEffectA = -1500f,
            repEffectA = 5f,
            toastEffectA = "PR firm contained the story. Rep safe.",
            repEffectB = -15f,
            toastEffectB = "Press caught wind of the leak. Reputation dropped by 15%."
        });

        randomEvents.Add(new GameEvent {
            id = "poaching",
            title = "TALENT POACHING WAR",
            description = "A competitor is trying to poach your top developers with huge signing bonuses.",
            isHistorical = false,
            choiceAText = "Counter Offer (-$3k Cash)",
            choiceBText = "Let them walk (-1 Team)",
            cashEffectA = -3000f,
            repEffectA = 5f,
            toastEffectA = "Developers stayed. Reputation +5%.",
            teamSizeEffectB = -1,
            toastEffectB = "Developer left. Team size reduced."
        });

        randomEvents.Add(new GameEvent {
            id = "hallucination_crisis",
            title = "MODEL HALLUCINATION CRISIS",
            description = "One of your deployed models starts generating bizarre hallucinations, leading to corporate customer complaints.",
            isHistorical = false,
            choiceAText = "Compensate Clients (-$2k)",
            choiceBText = "Ignore Outcry (-15% Rep)",
            cashEffectA = -2000f,
            repEffectA = 5f,
            toastEffectA = "Clients compensated. Brand trust preserved.",
            repEffectB = -15f,
            toastEffectB = "Public outcry damages reputation."
        });

        randomEvents.Add(new GameEvent {
            id = "cloud_credits",
            title = "CLOUD CREDITS DONATION",
            description = "A major technology partner donates cloud compute credits to your research startup.",
            isHistorical = false,
            choiceAText = "Use for Research (+10% Quality)",
            choiceBText = "Convert to Cash (+$3k)",
            qualityEffectA = 10f,
            toastEffectA = "Research capabilities boosted by cloud credits!",
            cashEffectB = 3000f,
            toastEffectB = "Converted credits. Gained +$3,000 cash."
        });

        randomEvents.Add(new GameEvent {
            id = "open_source_release",
            title = "OPEN SOURCE BREAKTHROUGH",
            description = "An open-source consortium releases a powerful pre-trained foundation model.",
            isHistorical = false,
            choiceAText = "Study Base Model (-$1k)",
            choiceBText = "Ignore Release",
            cashEffectA = -1000f,
            repEffectA = 5f,
            qualityEffectA = 10f,
            toastEffectA = "Studied base model. Team competence and quality increased.",
            toastEffectB = "Ignored release. Focus remains unchanged."
        });

        randomEvents.Add(new GameEvent {
            id = "viral_hype",
            title = "VIRAL INFERENCE HYPE",
            description = "A viral video features your AI lab. Traffic spikes, putting stress on servers.",
            isHistorical = false,
            choiceAText = "Upgrade Servers (-$2k)",
            choiceBText = "Let it Ride (+5% Rep)",
            cashEffectA = -2000f,
            repEffectA = 15f,
            toastEffectA = "Upgraded servers! Lab reputation surged (+15%).",
            repEffectB = 5f,
            toastEffectB = "Servers lagged under heavy load, but gained moderate hype."
        });

        randomEvents.Add(new GameEvent {
            id = "competitor_poaching_clients",
            title = "COMPETITOR POACHING",
            description = "A rival AI lab is aggressively undercutting your prices and stealing your clients with a shiny new benchmark.",
            isHistorical = false,
            choiceAText = "Drop Prices (-$3k Cash)",
            choiceBText = "Focus on Quality (+5% Quality)",
            cashEffectA = -3000f,
            repEffectA = 5f,
            toastEffectA = "Price war initiated. Retained clients but at a cost.",
            qualityEffectB = 5f,
            toastEffectB = "Doubled down on quality. R&D team delivered improvements."
        });

        randomEvents.Add(new GameEvent {
            id = "breakthrough_paper",
            title = "BREAKTHROUGH PAPER",
            description = "Your team publishes a paper on an efficient attention mechanism that cuts training costs by 20%.",
            isHistorical = false,
            choiceAText = "Publish (+10% Rep)",
            choiceBText = "Keep Secret (-$2k Burn)",
            repEffectA = 10f,
            toastEffectA = "Paper went viral! Community reputation surged.",
            monthlyBurnEffectB = -200f,
            toastEffectB = "Kept the technique proprietary. Burn reduced by $200/mo."
        });

        randomEvents.Add(new GameEvent {
            id = "keynote_invite",
            title = "KEYNOTE INVITATION",
            description = "A major AI conference invites your founder to deliver a keynote speech.",
            isHistorical = false,
            choiceAText = "Accept (+8% Rep, -$1k)",
            choiceBText = "Stay Focused (+5% Quality)",
            cashEffectA = -1000f,
            repEffectA = 8f,
            toastEffectA = "Keynote was a hit! Industry reputation improved.",
            qualityEffectB = 5f,
            toastEffectB = "Team stayed heads-down. Engineering excellence improved."
        });

        randomEvents.Add(new GameEvent {
            id = "server_crash",
            title = "SERVER CRASH",
            description = "A critical cooling failure takes down your main inference cluster during peak hours.",
            isHistorical = false,
            choiceAText = "Emergency Fix (-$4k Cash)",
            choiceBText = "Reroute Traffic (-10% Quality)",
            cashEffectA = -4000f,
            repEffectA = 5f,
            toastEffectA = "Fixed the cooling system. Expensive but reputation intact.",
            qualityEffectB = -10f,
            toastEffectB = "Model quality suffered due to degraded inference routing."
        });

        randomEvents.Add(new GameEvent {
            id = "startup_acquihire",
            title = "ACQUI-HIRE OFFER",
            description = "A large AI company wants to acquire your startup for $25k and hire your whole team.",
            isHistorical = false,
            choiceAText = "Accept Offer (+$25k Cash)",
            choiceBText = "Stay Independent (+$25k Revenue)",
            cashEffectA = 25000f,
            toastEffectA = "Acquired! Cash infusion of $25,000 bolsters runway.",
            cashEffectB = -5000f,
            monthlyRevenueEffectB = 5000f,
            toastEffectB = "Rejected the offer and landed a mega-deal with a client instead!"
        });

        randomEvents.Add(new GameEvent {
            id = "employee_burnout",
            title = "TEAM BURNOUT",
            description = "Your ML engineers are pushing unsustainable hours. Productivity is dropping across the board.",
            isHistorical = false,
            choiceAText = "Mandatory Break (+$2k, +10% Qual)",
            choiceBText = "Push Through (-10% Quality)",
            cashEffectA = -2000f,
            qualityEffectA = 10f,
            toastEffectA = "Team rested and returned with fresh ideas. Quality improved!",
            qualityEffectB = -10f,
            toastEffectB = "Exhausted engineers made critical errors. Quality dropped."
        });

        randomEvents.Add(new GameEvent {
            id = "dataset_windfall",
            title = "DATASET WINDFALL",
            description = "A research institution releases a massive high-quality dataset perfect for training your next model.",
            isHistorical = false,
            choiceAText = "License Dataset (-$1.5k)",
            choiceBText = "Scrape Web for Free",
            cashEffectA = -1500f,
            qualityEffectA = 10f,
            repEffectA = 5f,
            toastEffectA = "Licensed premium dataset. Quality and reputation improved.",
            repEffectB = -5f,
            qualityEffectB = -5f,
            toastEffectB = "Web-scraped data contained noise and bias. Quality suffered."
        });
    }

    private void CheckEventsOnMonthPass(int month)
    {
        if (TimeController.Instance == null) return;
        
        int year = TimeController.Instance.Year;
        
        // 1. Check historical events first
        foreach (var ev in historicalEvents)
        {
            if (ev.triggerYear == year && ev.triggerMonth == month)
            {
                TriggerEvent(ev);
                return;
            }
        }
        
        // 2. Check for Global AI Event every 3-6 months
        monthsSinceLastGlobalEvent++;
        if (monthsSinceLastGlobalEvent >= 3 + UnityEngine.Random.Range(0, 4))
        {
            monthsSinceLastGlobalEvent = 0;
            TriggerGlobalAIEvent();
            return;
        }
        
        // 3. Otherwise check for random event (15% base chance, 30% if web scraping data was used)
        float chance = 0.15f;
        var proj = FindFirstObjectByType<PrototypeProjectController>();
        if (proj != null && proj.SelectedData == PrototypeProjectController.DataType.Scraped)
        {
            // Mitigated back to 0.15f if we have Safety Researcher
            chance = (GameManager.Instance != null && GameManager.Instance.HasSafetyResearcher) ? 0.15f : 0.30f;
        }

        if (Random.value < chance)
        {
            GameEvent ev = null;
            // If using scraped data, 60% chance to prioritize data leak scare event (unless Safety Alignment is researched!)
            if (chance > 0.15f && Random.value < 0.6f)
            {
                if (GameManager.Instance == null || !GameManager.Instance.IsSafetyAlignmentResearched)
                {
                    ev = randomEvents.Find(e => e.id == "data_leak");
                }
            }
            
            if (ev == null)
            {
                // Filter out data_leak if Safety Alignment is researched
                var pool = randomEvents;
                if (GameManager.Instance != null && GameManager.Instance.IsSafetyAlignmentResearched)
                {
                    pool = randomEvents.FindAll(e => e.id != "data_leak");
                }
                if (pool.Count > 0)
                {
                    ev = pool[Random.Range(0, pool.Count)];
                }
            }
            
            if (ev != null)
            {
                TriggerEvent(ev);
            }
        }
    }

    public void TriggerEvent(GameEvent ev)
    {
        activeEvent = ev;
        
        if (TimeController.Instance != null)
        {
            speedBeforeEvent = TimeController.Instance.CurrentSpeed;
            TimeController.Instance.SetSpeed(TimeController.Speed.Paused);
        }
        
        if (titleText != null) titleText.text = ev.title;
        if (descriptionText != null) descriptionText.text = ev.description;
        
        if (choiceAButton != null)
        {
            var txt = choiceAButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = ev.choiceAText;
        }

        if (choiceBButton != null)
        {
            var txt = choiceBButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = ev.choiceBText;
        }
        
        ShowPanel();
    }

    private void SelectChoiceA()
    {
        if (activeEvent == null) return;
        ApplyGlobalEffects(activeEvent, true);
        ApplyEffects(activeEvent.cashEffectA, activeEvent.repEffectA, activeEvent.qualityEffectA, activeEvent.teamSizeEffectA, activeEvent.monthlyBurnEffectA, activeEvent.toastEffectA, activeEvent.monthlyRevenueEffectA);
        CloseEvent();
    }

    private void SelectChoiceB()
    {
        if (activeEvent == null) return;
        ApplyGlobalEffects(activeEvent, false);
        ApplyEffects(activeEvent.cashEffectB, activeEvent.repEffectB, activeEvent.qualityEffectB, activeEvent.teamSizeEffectB, activeEvent.monthlyBurnEffectB, activeEvent.toastEffectB, activeEvent.monthlyRevenueEffectB);
        CloseEvent();
    }

    private void ApplyGlobalEffects(GameEvent ev, bool isChoiceA)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (ev.id == "global_gpu_shortage")
        {
            if (isChoiceA)
            {
                gm.GpuCostMultiplier = 1.5f;
                gm.GpuShortageRemainingMonths = 3;
                ToastNotification.ShowGlobal("GPU shortage active! Acquisition cost +50% for 3 months.", ToastNotification.Category.Warning);
                if (TechPulseFeed.Instance != null)
                {
                    TechPulseFeed.Instance.AddPlayerPost($"▲ MARKET ALERT: GPU shortage pricing absorbed. Upgrades cost +50% for next 3 months.");
                }
            }
            else
            {
                gm.AddReputation(-10f);
                gm.GpuCostMultiplier = 1.0f; // Remains normal
                ToastNotification.ShowGlobal("Lobbied for subsidies. GPU cost remains normal. Lost 10 reputation.", ToastNotification.Category.Info);
                if (TechPulseFeed.Instance != null)
                {
                    TechPulseFeed.Instance.AddPlayerPost($"▲ PR ALERT: Political lobbying secured GPU chip pricing, but drew academic criticism. -10% Reputation.");
                }
            }
        }
        else if (ev.id == "global_data_regulation")
        {
            // Activate data regulation audit period
            gm.IsDataRegulationActive = true;
            gm.DataRegulationRemainingMonths = 3;
            ToastNotification.ShowGlobal("Data regulation audit window open! Launching models trained on scraped data will be audited.", ToastNotification.Category.Warning);
            if (TechPulseFeed.Instance != null)
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ REGULATION: New privacy laws went into effect. Auditing unlicensed data usage.");
            }
        }
        else if (ev.id == "global_hype_wave")
        {
            if (isChoiceA)
            {
                gm.ReputationBoostMultiplier = 2.0f;
                gm.HypeWaveRemainingMonths = 3;
                ToastNotification.ShowGlobal("AI Hype active! Double launch reputation and 1.5x follower growth for 3 months.", ToastNotification.Category.Success);
                if (TechPulseFeed.Instance != null)
                {
                    TechPulseFeed.Instance.AddPlayerPost($"▲ TRENDING: Riding the hype wave! Public interest in AI is at an all-time SOTA peak.");
                }
            }
            else
            {
                gm.AddCash(2000f);
                ToastNotification.ShowGlobal("Maintained focus. Received $2,000 research grant.", ToastNotification.Category.Info);
                if (TechPulseFeed.Instance != null)
                {
                    TechPulseFeed.Instance.AddPlayerPost($"▲ R&D: Focused on fundamentals. Received $2,000 corporate research grant.");
                }
            }
        }
    }

    private void TriggerGlobalAIEvent()
    {
        int eventIndex = UnityEngine.Random.Range(0, 3);
        GameEvent ev = null;

        if (eventIndex == 0) // GPU Shortage
        {
            ev = new GameEvent
            {
                id = "global_gpu_shortage",
                title = "GLOBAL GPU SHORTAGE",
                description = "A severe wafer manufacturing shortage has caused custom cloud GPU costs to surge by 50% globally.",
                choiceAText = "Absorb Pricing (Buy GPUs at +50%)",
                choiceBText = "Lobby for Subsidy (-10 Rep, normal cost)"
            };
        }
        else if (eventIndex == 1) // Data Regulation
        {
            ev = new GameEvent
            {
                id = "global_data_regulation",
                title = "EU DATA COMPLIANCE INITIATIVE",
                description = "Strict new data governance regulations are enacted. Unlicensed web-scraped data models will face strict audits. Safety researchers can guarantee compliance.",
                choiceAText = "Cooperate with Regulators",
                choiceBText = "Request Extension"
            };
        }
        else // Hype Wave
        {
            ev = new GameEvent
            {
                id = "global_hype_wave",
                title = "GENERATIVE AI HYPE WAVE",
                description = "A massive hype cycle sweeps the tech industry. Investors and businesses are scrambling for models, boosting public interest.",
                choiceAText = "Capitalize on Hype (2x Rep, 1.5x Followers)",
                choiceBText = "Maintain R&D Focus (+$2k Grant)"
            };
        }

        if (ev != null)
        {
            TriggerEvent(ev);
        }
    }

    private void ApplyEffects(float cash, float rep, float qual, int team, float burn, string toast, float revenueEffect = 0f)
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (cash != 0f)
            {
                if (cash < 0) gm.SpendCash(-cash);
                else gm.AddCash(cash);
            }
            
            if (rep != 0f)
            {
                gm.AddReputation(rep);
            }
            
            if (qual != 0f)
            {
                gm.SetModelQuality(gm.ModelQuality + qual);
            }
            
            if (team != 0)
            {
                gm.SetTeamSize(gm.TeamSize + team);
            }
            
            if (burn != 0f)
            {
                gm.SetMonthlyBurn(gm.MonthlyBurn + burn);
            }

            if (revenueEffect != 0f)
            {
                gm.SetMonthlyRevenue(gm.MonthlyRevenue + revenueEffect);
            }
        }
        
        if (!string.IsNullOrEmpty(toast))
        {
            ToastNotification.ShowGlobal(toast, ToastNotification.Category.Info);
            
            if (TechPulseFeed.Instance != null && activeEvent != null)
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ DECISION: {activeEvent.title}\n{toast}");
            }
        }
    }

    private void CloseEvent()
    {
        HidePanel();
        activeEvent = null;
        
        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(speedBeforeEvent);
        }
    }

    private void ShowPanel()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 1f;
            panelGroup.blocksRaycasts = true;
            panelGroup.interactable = true;
        }
    }

    private void HidePanel()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }
    }
}

[System.Serializable]
public class GameEvent
{
    public string id;
    public string title;
    public string description;
    
    public bool isHistorical;
    public int triggerYear;
    public int triggerMonth;
    
    public string choiceAText;
    public string choiceBText;
    
    public float cashEffectA = 0f;
    public float repEffectA = 0f;
    public float qualityEffectA = 0f;
    public int teamSizeEffectA = 0;
    public float monthlyBurnEffectA = 0f;
    public float monthlyRevenueEffectA = 0f;
    public string toastEffectA;
    
    public float cashEffectB = 0f;
    public float repEffectB = 0f;
    public float qualityEffectB = 0f;
    public int teamSizeEffectB = 0;
    public float monthlyBurnEffectB = 0f;
    public float monthlyRevenueEffectB = 0f;
    public string toastEffectB;
}
