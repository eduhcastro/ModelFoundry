using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Controls an AI project lifecycle: start → progress → result.
/// Integrates with GameManager for cash/state and shows ProjectResultPanel on completion.
/// </summary>
public sealed class PrototypeProjectController : MonoBehaviour
{
    [Header("Project Definition")]
    [SerializeField] private string projectName = "SupportBot v0.1";
    [SerializeField] private string projectDescription = "Customer support chatbot for small businesses";

    [Header("Tuning")]
    [SerializeField] private float projectDuration  = 12f;
    [SerializeField] private float projectCost      = 4200f;
    [SerializeField] private float burnPerSecond    = 350f;
    [SerializeField] private float baseQuality      = 55f;
    [SerializeField] private float qualityVariance   = 25f;

    [Header("Agent")]
    [SerializeField] private PrototypeEmployeeAgent assignedAgent;

    [Header("UI References")]
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_InputField modelNameInputField;
    [SerializeField] private Image progressFill;
    [SerializeField] private Image progressBackground;
    [SerializeField] private TextMeshProUGUI projectNameText;
    [SerializeField] private TextMeshProUGUI projectDescText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI cashText;
    [SerializeField] private TextMeshProUGUI costEstimateText;
    [SerializeField] private TextMeshProUGUI progressPercentText;
    [SerializeField] private Image panelBackground;
    [SerializeField] private Image headerBackground;

    [Header("Result Panel")]
    [SerializeField] private ProjectResultPanel resultPanel;

    public enum ModelType { NLP, Vision, Agentic }
    public enum DataType { Scraped, Licensed }

    public static PrototypeProjectController Instance { get; private set; }

    public string SelectedModelLabel => selectedModel.ToString();

    [Header("Model Selection UI Dependencies")]
    [SerializeField] private Button nlpModelButton;
    [SerializeField] private Button visionModelButton;
    [SerializeField] private Button agenticModelButton;
    [SerializeField] private Button scrapedDataButton;
    [SerializeField] private Button licensedDataButton;

    private ModelType selectedModel = ModelType.NLP;
    private DataType selectedData = DataType.Scraped;

    public ModelType SelectedModel => selectedModel;
    public DataType SelectedData => selectedData;

    private float progress;
    private float totalSpent;
    private bool isRunning;
    private bool isCompleted;
    private ProjectResultPanel.ProjectResult lastResult;

    private void OnEnable()
    {
        if (modelNameInputField != null && !isRunning)
        {
            modelNameInputField.text = projectName;
        }
    }

    private void Awake()
    {
        Instance = this;

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartProject);
        }

        if (nlpModelButton != null) nlpModelButton.onClick.AddListener(() => SetModelType(ModelType.NLP));
        if (visionModelButton != null) visionModelButton.onClick.AddListener(() => SetModelType(ModelType.Vision));
        if (agenticModelButton != null) agenticModelButton.onClick.AddListener(() => SetModelType(ModelType.Agentic));
        if (scrapedDataButton != null) scrapedDataButton.onClick.AddListener(() => SetDataType(DataType.Scraped));
        if (licensedDataButton != null) licensedDataButton.onClick.AddListener(() => SetDataType(DataType.Licensed));

        StylePanel();
        UpdateProjectSpecs();
        UpdateSelectionUi();
        UpdateUi();
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }
    }

    private void HandleGameStateChanged()
    {
        UpdateProjectSpecs();
        UpdateSelectionUi();
        UpdateUi();
    }

    private void Update()
    {
        if (!isRunning)
        {
            return;
        }

        var dt = Time.deltaTime;

        // Apply time scale based on game speed
        if (TimeController.Instance != null &&
            TimeController.Instance.CurrentSpeed == TimeController.Speed.Paused)
        {
            return;
        }

        progress += dt / projectDuration;

        // Burn cash through GameManager
        var burnAmount = burnPerSecond * dt;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpendCash(burnAmount);
        }
        totalSpent += burnAmount;

        if (progress >= 1f)
        {
            progress = 1f;
            CompleteProject();
        }

        UpdateUi();
    }

    // ── Actions ──────────────────────────────────────────────────────

    private void StartProject()
    {
        if (isRunning || isCompleted)
        {
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null) return;

        // Require model name
        if (modelNameInputField == null || string.IsNullOrWhiteSpace(modelNameInputField.text))
        {
            ToastNotification.ShowGlobal("Name your model before launching!", ToastNotification.Category.Warning);
            if (modelNameInputField != null) modelNameInputField.Select();
            return;
        }
        projectName = modelNameInputField.text.Trim();

        // Check competence requirement per model type
        float requiredCompetence = selectedModel switch
        {
            ModelType.Vision => 15f,
            ModelType.NLP => 25f,
            ModelType.Agentic => 50f,
            _ => 10f
        };

        if (gm.Competence < requiredCompetence)
        {
            ToastNotification.ShowGlobal(
                $"Competence too low! Need {requiredCompetence:F0} competence for {selectedModel}. Study or hire!",
                ToastNotification.Category.Warning);
            return;
        }

        // Check cash
        if (gm.Cash < projectCost * 0.2f)
        {
            ToastNotification.ShowGlobal("Not enough cash to start this project!", ToastNotification.Category.Danger);
            return;
        }

        isRunning = true;
        progress = 0f;
        totalSpent = 0f;

        if (TechPulseFollowerSystem.Instance != null)
        {
            TechPulseFollowerSystem.Instance.RecordActivity();
        }

        assignedAgent?.StartWork();

        if (statusText != null)
        {
            statusText.text = "TRAINING";
            statusText.color = GameDesignConstants.StatusInfo;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SendNotification($"Project started: {projectName}");
        }

        UpdateUi();
    }

    private void CompleteProject()
    {
        isRunning = false;
        isCompleted = true;

        // Generate result
        var quality = CalculateQuality();
        var clients = CalculateClients(quality);
        var revenue = CalculateRevenue(quality, clients);
        var repGain = CalculateReputationGain(quality);

        lastResult = new ProjectResultPanel.ProjectResult
        {
            projectName    = projectName,
            quality        = quality,
            totalCost      = totalSpent,
            clientsGained  = clients,
            monthlyRevenue = revenue,
            reputationGain = repGain
        };

        if (statusText != null)
        {
            statusText.text = "COMPLETE";
            statusText.color = GameDesignConstants.StatusSuccess;
        }

        // Update GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetModelQuality(quality);
            GameManager.Instance.SendNotification($"Project complete: {projectName} — Quality {quality:F0}%");
        }

        // Show result panel
        if (resultPanel != null)
        {
            resultPanel.Show(lastResult, OnResultDecision);
        }
    }

    private void OnResultDecision(ProjectResultPanel.ResultAction action)
    {
        switch (action)
        {
            case ProjectResultPanel.ResultAction.Accept:
                AcceptProject();
                break;

            case ProjectResultPanel.ResultAction.Refine:
                RefineProject();
                break;

            case ProjectResultPanel.ResultAction.Abandon:
                AbandonProject();
                break;
        }
    }

    private void AcceptProject()
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            float repGain = lastResult.reputationGain;
            if (repGain > 0f)
            {
                repGain *= gm.ReputationBoostMultiplier;
            }

            gm.AddClients(lastResult.clientsGained);
            gm.SetMonthlyRevenue(gm.MonthlyRevenue + lastResult.monthlyRevenue);
            gm.AddReputation(repGain);
            gm.AddCompetence(5f); // Launching builds competence

            // Check Data Regulation penalty
            if (gm.IsDataRegulationActive && selectedData == DataType.Scraped)
            {
                if (gm.HasSafetyResearcher)
                {
                    gm.AddReputation(10f);
                    ToastNotification.ShowGlobal("Data regulation compliance audited! Safety researcher secured +10 reputation.", ToastNotification.Category.Success);
                    if (TechPulseFeed.Instance != null)
                    {
                        TechPulseFeed.Instance.AddPlayerPost($"▲ COMPLIANCE: Audited under data regulations. Verified compliant by safety researcher. +10% Reputation!");
                    }
                }
                else
                {
                    gm.SpendCash(5000f);
                    gm.AddReputation(-15f);
                    ToastNotification.ShowGlobal("Data regulation penalty! Used scraped data without safety researcher. -$5,000, -15 reputation.", ToastNotification.Category.Danger);
                    if (TechPulseFeed.Instance != null)
                    {
                        TechPulseFeed.Instance.AddPlayerPost($"▲ AUDIT FAILURE: Fined $5,000 and penalized -15% Reputation for non-compliant scraped training data!");
                    }
                }
            }

            // Verify and deliver contract
            if (ContractController.Instance != null)
            {
                ContractController.Instance.TryDeliverContract(selectedModel, lastResult.quality);
            }
        }

        ToastNotification.ShowGlobal(
            $"{projectName} launched! +{lastResult.clientsGained} clients, +${lastResult.monthlyRevenue:N0}/mo revenue",
            ToastNotification.Category.Success
        );

        // Auto-publish to TechPulse and register to Follower System
        string modelTypeLabel = selectedModel switch
        {
            ModelType.Vision => "Vision",
            ModelType.NLP => "NLP",
            ModelType.Agentic => "Agentic",
            _ => "AI"
        };

        if (TechPulseFollowerSystem.Instance != null)
        {
            TechPulseFollowerSystem.Instance.RegisterProductLaunch(lastResult.projectName, lastResult.quality, modelTypeLabel);
        }

        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.AddPlayerLaunchPost(lastResult.projectName, lastResult.quality, modelTypeLabel);
        }

        if (AnalyticsController.Instance != null)
        {
            AnalyticsController.Instance.AddModelLaunch(lastResult.projectName, lastResult.quality, lastResult.monthlyRevenue);
        }

        // Resume time
        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(TimeController.Speed.Normal);
        }

        // Allow starting another project
        isCompleted = false;
        progress = 0f;
        UpdateUi();
    }

    private void RefineProject()
    {
        isCompleted = false;
        progress = 0.7f; // Start at 70% - refinement is shorter
        isRunning = true;
        baseQuality += 10f; // Refinement improves base quality

        ToastNotification.ShowGlobal(
            $"Refining {projectName}... improving quality.",
            ToastNotification.Category.Info
        );

        if (TechPulseFollowerSystem.Instance != null)
        {
            TechPulseFollowerSystem.Instance.RecordActivity();
        }

        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.AddOrganicPlayerPost(false, 0);
        }

        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(TimeController.Speed.Normal);
        }

        assignedAgent?.StartWork();
        UpdateUi();
    }

    private void AbandonProject()
    {
        ToastNotification.ShowGlobal(
            $"{projectName} abandoned. ${totalSpent:N0} lost.",
            ToastNotification.Category.Warning
        );

        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(TimeController.Speed.Normal);
        }

        isCompleted = false;
        progress = 0f;
        UpdateUi();
    }

    // ── Calculations ─────────────────────────────────────────────────

    private float CalculateQuality()
    {
        // Base quality + random variance + team bonus
        var teamBonus = GameManager.Instance != null ? GameManager.Instance.TeamSize * 3f : 0f;
        float baseQualValue = baseQuality + Random.Range(-qualityVariance, qualityVariance) + teamBonus;

        // Apply Data Engineer bonus (+20% quality)
        if (GameManager.Instance != null && GameManager.Instance.HasDataEngineer)
        {
            baseQualValue *= 1.2f;
        }

        // Apply Safety Alignment research bonus (+10 points)
        if (GameManager.Instance != null && GameManager.Instance.IsSafetyAlignmentResearched)
        {
            baseQualValue += 10f;
        }

        // Apply Product Manager bonus (+5 points)
        if (GameManager.Instance != null && GameManager.Instance.HasProductManager)
        {
            baseQualValue += 5f;
        }

        // Apply AnthroTech M&A bonus (+10 points)
        if (GameManager.Instance != null && GameManager.Instance.HasAcquiredAnthroTech)
        {
            baseQualValue += 10f;
        }

        return Mathf.Clamp(baseQualValue, 5f, 100f);
    }

    private int CalculateClients(float quality)
    {
        return Mathf.RoundToInt(quality * 0.1f * Random.Range(0.8f, 1.3f));
    }

    private float CalculateRevenue(float quality, int clients)
    {
        float rev = clients * quality * Random.Range(8f, 15f);
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.HasMlopsEngineer)
            {
                rev *= 1.20f; // +20% net revenue due to MLOps inference cost reduction
            }
            if (GameManager.Instance.HasSalesExecutive)
            {
                rev *= 1.20f; // +20% net revenue due to Sales Executive
            }
        }
        return rev;
    }

    private float CalculateReputationGain(float quality)
    {
        float rep;
        if (quality >= GameDesignConstants.QualityGreat) rep = Random.Range(3f, 6f);
        else if (quality >= GameDesignConstants.QualityGood)  rep = Random.Range(1f, 3f);
        else if (quality >= GameDesignConstants.QualityDecent) rep = Random.Range(0f, 1f);
        else rep = Random.Range(-2f, 0f);

        // Apply Safety Researcher bonus (+25% positive reputation impact)
        if (rep > 0f && GameManager.Instance != null && GameManager.Instance.HasSafetyResearcher)
        {
            rep *= 1.25f;
        }
        return rep;
    }

    public void SetDuration(float newDuration)
    {
        projectDuration = Mathf.Max(0.5f, newDuration);
        UpdateUi();
    }

    public float ProjectDuration => projectDuration;

    public void RecalculateSpecs()
    {
        UpdateProjectSpecs();
    }

    public void SetModelType(ModelType modelType)
    {
        if (isRunning) return;

        if (modelType == ModelType.NLP && GameManager.Instance != null && !GameManager.Instance.IsNlpUnlocked)
        {
            ToastNotification.ShowGlobal("Research NLP Chatbots first!", ToastNotification.Category.Warning);
            return;
        }
        if (modelType == ModelType.Agentic && GameManager.Instance != null && !GameManager.Instance.IsAgenticUnlocked)
        {
            ToastNotification.ShowGlobal("Research Agentic Coders first!", ToastNotification.Category.Warning);
            return;
        }

        selectedModel = modelType;

        // Reset text field so default name populates
        if (modelNameInputField != null && !isRunning)
        {
            modelNameInputField.text = "";
        }

        UpdateProjectSpecs();
        UpdateSelectionUi();
    }

    public void SetDataType(DataType dataType)
    {
        if (isRunning) return;
        selectedData = dataType;
        UpdateProjectSpecs();
        UpdateSelectionUi();
    }

    private void UpdateProjectSpecs()
    {
        float computeMultiplier = 1.0f;
        if (GameManager.Instance != null)
        {
            int extraGpus = GameManager.Instance.GpuCount - 1;
            computeMultiplier = Mathf.Max(0.1f, 1.0f - extraGpus * 0.15f);

            if (GameManager.Instance.HasGpuTechnician)
            {
                computeMultiplier *= 0.9f; // 10% faster training due to hardware tuning
            }
        }

        float baseDur;
        float baseCostVal;
        
        switch (selectedModel)
        {
            case ModelType.Vision:
                projectName = "ImageNet Classifier";
                projectDescription = "Fast vision classifier for product catalogs";
                baseDur = 8f;
                baseCostVal = 2000f;
                baseQuality = 65f;
                qualityVariance = 15f;
                break;
            case ModelType.NLP:
                projectName = "SupportBot v1.0";
                projectDescription = "Customer support chatbot for e-commerce";
                baseDur = 15f;
                baseCostVal = 4000f;
                baseQuality = 55f;
                qualityVariance = 20f;
                break;
            case ModelType.Agentic:
                projectName = "CodeDaemon v1.0";
                projectDescription = "Autonomous agentic coder for backend deployment";
                baseDur = 25f;
                baseCostVal = 8000f;
                baseQuality = 45f;
                qualityVariance = 25f;
                break;
            default:
                baseDur = 12f;
                baseCostVal = 4200f;
                break;
        }

        if (selectedData == DataType.Licensed)
        {
            baseCostVal += 1500f;
            baseQuality += 15f;
        }
        else
        {
            // Scraped data penalty is -15, but mitigated to -5 if we have Safety Researcher
            float penalty = (GameManager.Instance != null && GameManager.Instance.HasSafetyResearcher) ? -5f : -15f;
            baseQuality += penalty;
        }

        float teamMultiplier = 1.0f;
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.HasMLEngineer)
            {
                teamMultiplier = 0.6f;
            }
        }

        projectDuration = baseDur * teamMultiplier * computeMultiplier;

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.HasProductManager)
            {
                projectDuration *= 0.9f; // PM 10% faster training
            }
            if (GameManager.Instance.HasAcquiredQuantumMinds)
            {
                projectDuration *= 0.7f; // Quantum Minds M&A 30% faster training
            }
            if (GameManager.Instance.IsOverheating)
            {
                projectDuration *= 2.0f; // Overheated systems double the training duration
            }
        }

        projectCost = baseCostVal;

        UpdateUi();
    }

    private void UpdateSelectionUi()
    {
        bool nlpUnlocked = GameManager.Instance == null || GameManager.Instance.IsNlpUnlocked;
        bool agenticUnlocked = GameManager.Instance == null || GameManager.Instance.IsAgenticUnlocked;

        SetButtonActive(visionModelButton, selectedModel == ModelType.Vision, true);
        SetButtonActive(nlpModelButton, selectedModel == ModelType.NLP, nlpUnlocked);
        SetButtonActive(agenticModelButton, selectedModel == ModelType.Agentic, agenticUnlocked);
        SetButtonActive(scrapedDataButton, selectedData == DataType.Scraped, true);
        SetButtonActive(licensedDataButton, selectedData == DataType.Licensed, true);

        if (nlpModelButton != null)
        {
            var txt = nlpModelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = nlpUnlocked ? "NLP" : "NLP (Locked)";
        }
        if (agenticModelButton != null)
        {
            var txt = agenticModelButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = agenticUnlocked ? "Agentic" : "Agentic (Locked)";
        }
    }

    private void SetButtonActive(Button btn, bool active, bool interactable)
    {
        if (btn == null) return;
        btn.interactable = interactable;
        var stylized = btn.GetComponent<StylizedButton>();
        if (stylized != null)
        {
            stylized.SetVariant(active ? StylizedButton.ButtonVariant.Primary : StylizedButton.ButtonVariant.Secondary);
        }
    }

    // ── UI ────────────────────────────────────────────────────────────

    private void StylePanel()
    {
        if (panelBackground != null)
            panelBackground.color = GameDesignConstants.SurfaceCard;

        if (headerBackground != null)
            headerBackground.color = GameDesignConstants.SurfaceMid;

        if (progressBackground != null)
            progressBackground.color = GameDesignConstants.ResourceBarBg;
    }

    private void UpdateUi()
    {
        // Project info
        if (projectNameText != null)
        {
            projectNameText.text = projectName;
            projectNameText.color = GameDesignConstants.TextPrimary;
        }

        if (projectDescText != null)
        {
            projectDescText.text = projectDescription;
            projectDescText.color = GameDesignConstants.TextSecondary;
        }

        // Progress
        if (progressFill != null)
        {
            progressFill.fillAmount = progress;
            progressFill.color = isRunning
                ? GameDesignConstants.BrandPrimary
                : isCompleted
                    ? GameDesignConstants.StatusSuccess
                    : GameDesignConstants.TextMuted;
        }

        if (progressPercentText != null)
        {
            progressPercentText.text = $"{progress * 100f:F0}%";
            progressPercentText.color = GameDesignConstants.TextPrimary;
        }

        // Cash display (from GameManager)
        if (cashText != null && GameManager.Instance != null)
        {
            cashText.text = $"${GameManager.Instance.Cash:N0}";
            cashText.color = GameManager.Instance.Cash > 5000f
                ? GameDesignConstants.StatusSuccess
                : GameDesignConstants.StatusDanger;
        }

        // Cost estimate
        if (costEstimateText != null)
        {
            costEstimateText.text = isRunning
                ? $"Spent: ${totalSpent:N0}"
                : $"Est. Cost: ${projectCost:N0}";
            costEstimateText.color = GameDesignConstants.TextSecondary;
        }

        // Status
        if (statusText != null && !isRunning && !isCompleted)
        {
            statusText.text = "READY";
            statusText.color = GameDesignConstants.TextMuted;
        }

        // Button
        if (startButton != null)
        {
            startButton.interactable = !isRunning && !isCompleted;
        }
    }
}
