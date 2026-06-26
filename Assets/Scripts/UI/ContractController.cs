using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public sealed class ContractController : MonoBehaviour
{
    public static ContractController Instance { get; private set; }

    [System.Serializable]
    public class Contract
    {
        public string id;
        public string clientName;
        public PrototypeProjectController.ModelType modelType;
        public float qualityThreshold;
        public int durationDays;
        public int daysRemaining;
        public float upfrontPayment;
        public float completionPayout;
        public float failurePenalty;
        public float reputationPenalty;
        public bool isAccepted;
    }

    [Header("UI Reference")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Button closeButton;
    [SerializeField] private RectTransform scrollContent;

    // State
    private List<Contract> offeredContracts = new List<Contract>();
    private List<Contract> activeContracts = new List<Contract>();

    private int daysSinceLastGeneration = 0;
    private int generationIntervalDays = 15;

    public List<Contract> OfferedContracts => offeredContracts;
    public List<Contract> ActiveContracts => activeContracts;

    private static readonly string[] ClientNames = {
        "Cyberdyne Systems", "Stark Industries", "Tyrell Corp", "Umbrella Corp",
        "Weyland-Yutani", "Globex Corporation", "Initech", "Hooli", "Soylent Corp", "Aperture Science"
    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed += HandleDayPassed;
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }

        // Generate initial offered contracts
        if (offeredContracts.Count == 0 && activeContracts.Count == 0)
        {
            for (int i = 0; i < 3; i++)
            {
                GenerateNewOfferedContract();
            }
        }

        RefreshPanelUI();
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
        bool anyChanged = false;

        // Generate new offered contract every 15-30 days
        daysSinceLastGeneration++;
        if (daysSinceLastGeneration >= generationIntervalDays && offeredContracts.Count < 3)
        {
            daysSinceLastGeneration = 0;
            generationIntervalDays = UnityEngine.Random.Range(15, 30);
            GenerateNewOfferedContract();
            anyChanged = true;
        }

        // Countdown active contracts
        for (int i = activeContracts.Count - 1; i >= 0; i--)
        {
            var active = activeContracts[i];
            active.daysRemaining--;
            anyChanged = true;

            if (active.daysRemaining <= 0)
            {
                FailContract(active);
            }
        }

        if (anyChanged && panelGroup != null && panelGroup.alpha > 0.5f)
        {
            RefreshPanelUI();
        }
    }

    public void GenerateNewOfferedContract()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Choose allowed model types
        var allowedTypes = new List<PrototypeProjectController.ModelType> { PrototypeProjectController.ModelType.Vision };
        if (gm.IsNlpUnlocked) allowedTypes.Add(PrototypeProjectController.ModelType.NLP);
        if (gm.IsAgenticUnlocked) allowedTypes.Add(PrototypeProjectController.ModelType.Agentic);

        var modelType = allowedTypes[UnityEngine.Random.Range(0, allowedTypes.Count)];

        // Generate details based on model type
        string client = ClientNames[UnityEngine.Random.Range(0, ClientNames.Length)];
        float baseQuality = modelType switch
        {
            PrototypeProjectController.ModelType.Vision => 45f,
            PrototypeProjectController.ModelType.NLP => 55f,
            PrototypeProjectController.ModelType.Agentic => 70f,
            _ => 50f
        };

        // Scale quality threshold slightly based on player competence
        float quality = Mathf.Clamp(baseQuality + UnityEngine.Random.Range(-10f, 15f), 30f, 95f);

        int duration = modelType switch
        {
            PrototypeProjectController.ModelType.Vision => UnityEngine.Random.Range(20, 40),
            PrototypeProjectController.ModelType.NLP => UnityEngine.Random.Range(30, 50),
            PrototypeProjectController.ModelType.Agentic => UnityEngine.Random.Range(40, 70),
            _ => 30
        };

        float upfront = modelType switch
        {
            PrototypeProjectController.ModelType.Vision => UnityEngine.Random.Range(1000f, 2000f),
            PrototypeProjectController.ModelType.NLP => UnityEngine.Random.Range(2000f, 4000f),
            PrototypeProjectController.ModelType.Agentic => UnityEngine.Random.Range(4000f, 8000f),
            _ => 2000f
        };

        float completion = modelType switch
        {
            PrototypeProjectController.ModelType.Vision => UnityEngine.Random.Range(4000f, 7000f),
            PrototypeProjectController.ModelType.NLP => UnityEngine.Random.Range(8000f, 13000f),
            PrototypeProjectController.ModelType.Agentic => UnityEngine.Random.Range(15000f, 25000f),
            _ => 8000f
        };

        float penalty = modelType switch
        {
            PrototypeProjectController.ModelType.Vision => UnityEngine.Random.Range(2000f, 4000f),
            PrototypeProjectController.ModelType.NLP => UnityEngine.Random.Range(4000f, 8000f),
            PrototypeProjectController.ModelType.Agentic => UnityEngine.Random.Range(8000f, 15000f),
            _ => 4000f
        };

        float repPenalty = UnityEngine.Random.Range(5f, 15f);

        var c = new Contract
        {
            id = $"contract_{System.Guid.NewGuid().ToString().Substring(0, 8)}",
            clientName = client,
            modelType = modelType,
            qualityThreshold = quality,
            durationDays = duration,
            daysRemaining = duration,
            upfrontPayment = Mathf.Round(upfront / 100f) * 100f,
            completionPayout = Mathf.Round(completion / 100f) * 100f,
            failurePenalty = Mathf.Round(penalty / 100f) * 100f,
            reputationPenalty = Mathf.Round(repPenalty),
            isAccepted = false
        };

        offeredContracts.Add(c);
    }

    public void AcceptContract(Contract contract)
    {
        int maxContracts = 3;
        if (GameManager.Instance != null && GameManager.Instance.HasBackendEngineer) maxContracts++;
        if (GameManager.Instance != null && GameManager.Instance.HasSalesExecutive) maxContracts++;

        if (activeContracts.Count >= maxContracts)
        {
            ToastNotification.ShowGlobal($"Max active contracts reached ({maxContracts})!", ToastNotification.Category.Warning);
            return;
        }

        offeredContracts.Remove(contract);
        contract.isAccepted = true;
        activeContracts.Add(contract);

        // Apply upfront payment immediately
        if (GameManager.Instance != null)
        {
            float cashToAdd = contract.upfrontPayment;
            if (GameManager.Instance.HasSalesExecutive) cashToAdd *= 1.2f;
            GameManager.Instance.AddCash(cashToAdd);
            GameManager.Instance.SendNotification($"Accepted contract from {contract.clientName}! Upfront +${cashToAdd:N0}");
        }

        ToastNotification.ShowGlobal($"Accepted contract: {contract.clientName}", ToastNotification.Category.Success);

        RefreshPanelUI();
    }

    public bool TryDeliverContract(PrototypeProjectController.ModelType modelType, float quality)
    {
        for (int i = 0; i < activeContracts.Count; i++)
        {
            var active = activeContracts[i];
            if (active.modelType == modelType && quality >= active.qualityThreshold)
            {
                DeliverContract(active);
                return true;
            }
        }
        return false;
    }

    private void DeliverContract(Contract contract)
    {
        activeContracts.Remove(contract);

        if (GameManager.Instance != null)
        {
            float cashToAdd = contract.completionPayout;
            if (GameManager.Instance.HasSalesExecutive) cashToAdd *= 1.2f;
            GameManager.Instance.AddCash(cashToAdd);
            GameManager.Instance.SendNotification($"Delivered contract for {contract.clientName}! Payout +${cashToAdd:N0}");
        }

        ToastNotification.ShowGlobal($"Contract completed: {contract.clientName}", ToastNotification.Category.Success);

        if (TechPulseFeed.Instance != null)
        {
            float cashVal = contract.completionPayout;
            if (GameManager.Instance != null && GameManager.Instance.HasSalesExecutive) cashVal *= 1.2f;
            TechPulseFeed.Instance.AddPlayerPost($"▲ CONTRACT COMPLETED: Successfully delivered custom {contract.modelType} model to {contract.clientName}! Gained ${cashVal:N0}.");
        }

        RefreshPanelUI();
    }

    private void FailContract(Contract contract)
    {
        activeContracts.Remove(contract);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.SpendCash(contract.failurePenalty);
            GameManager.Instance.AddReputation(-contract.reputationPenalty);
            GameManager.Instance.SendNotification($"Contract failed for {contract.clientName}! Penalty -${contract.failurePenalty:N0}, -{contract.reputationPenalty:F0} Rep");
        }

        ToastNotification.ShowGlobal($"Contract failed: {contract.clientName}", ToastNotification.Category.Danger);

        if (TechPulseFeed.Instance != null)
        {
            TechPulseFeed.Instance.AddPlayerPost($"▲ CONTRACT BREACH: Failed to deliver {contract.modelType} model to {contract.clientName} within deadline. Paid penalty of ${contract.failurePenalty:N0}.");
        }

        RefreshPanelUI();
    }

    public void LoadContracts(List<Contract> offered, List<Contract> active)
    {
        offeredContracts = offered ?? new List<Contract>();
        activeContracts = active ?? new List<Contract>();
        RefreshPanelUI();
    }

    public void ShowPanel()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 1f;
            panelGroup.blocksRaycasts = true;
            panelGroup.interactable = true;
        }
        RefreshPanelUI();
    }

    public void HidePanel()
    {
        if (panelGroup != null)
        {
            if (panelGroup.alpha == 0f && !panelGroup.blocksRaycasts && !panelGroup.interactable)
                return;

            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }
        var hud = FindFirstObjectByType<HUDController>();
        if (hud != null)
        {
            hud.HideDockPanel(panelGroup);
        }
    }

    public void RefreshPanelUI()
    {
        if (scrollContent == null) return;

        // Clear existing rows
        foreach (Transform child in scrollContent)
        {
            Destroy(child.gameObject);
        }

        // Draw Active Contracts
        if (activeContracts.Count > 0)
        {
            CreateHeaderLabel("ACTIVE CONTRACTS");
            foreach (var active in activeContracts)
            {
                CreateContractRowUI(active);
            }
        }

        // Draw Offered Contracts
        CreateHeaderLabel("AVAILABLE CONTRACTS");
        if (offeredContracts.Count == 0)
        {
            CreateEmptyLabel("No contract offers available. Check back soon!");
        }
        else
        {
            foreach (var offered in offeredContracts)
            {
                CreateContractRowUI(offered);
            }
        }
    }

    private void CreateHeaderLabel(string text)
    {
        var labelObj = new GameObject("HeaderLabel");
        labelObj.transform.SetParent(scrollContent, false);
        var rect = labelObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(390f, 25f);
        var le = labelObj.AddComponent<LayoutElement>();
        le.preferredHeight = 25f;

        var txt = labelObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 11f;
        txt.fontStyle = FontStyles.Bold;
        txt.color = GameDesignConstants.BrandSecondary;
        txt.alignment = TextAlignmentOptions.Left;
    }

    private void CreateEmptyLabel(string text)
    {
        var labelObj = new GameObject("EmptyLabel");
        labelObj.transform.SetParent(scrollContent, false);
        var rect = labelObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(390f, 40f);
        var le = labelObj.AddComponent<LayoutElement>();
        le.preferredHeight = 40f;

        var txt = labelObj.AddComponent<TextMeshProUGUI>();
        txt.text = text;
        txt.fontSize = 12f;
        txt.color = GameDesignConstants.TextMuted;
        txt.alignment = TextAlignmentOptions.Center;
    }

    private void CreateContractRowUI(Contract contract)
    {
        var rowObj = new GameObject("ContractRow");
        rowObj.transform.SetParent(scrollContent, false);
        
        var rect = rowObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(390f, 130f);
        
        var le = rowObj.AddComponent<LayoutElement>();
        le.preferredHeight = 130f;
        le.minHeight = 130f;
        
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(rowObj.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.color = GameDesignConstants.SurfaceCard;
        
        var titleObj = new GameObject("ClientName");
        titleObj.transform.SetParent(rowObj.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(0f, 1f);
        titleRect.pivot = new Vector2(0f, 1f);
        titleRect.sizeDelta = new Vector2(250f, 25f);
        titleRect.anchoredPosition = new Vector2(15f, -10f);
        var titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = contract.clientName.ToUpper();
        titleText.fontSize = 13f;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = GameDesignConstants.TextPrimary;
        
        var detailsObj = new GameObject("Details");
        detailsObj.transform.SetParent(rowObj.transform, false);
        var detailsRect = detailsObj.AddComponent<RectTransform>();
        detailsRect.anchorMin = new Vector2(0f, 1f);
        detailsRect.anchorMax = new Vector2(0f, 1f);
        detailsRect.pivot = new Vector2(0f, 1f);
        detailsRect.sizeDelta = new Vector2(250f, 80f);
        detailsRect.anchoredPosition = new Vector2(15f, -35f);
        var detailsText = detailsObj.AddComponent<TextMeshProUGUI>();
        detailsText.fontSize = 11f;
        detailsText.color = GameDesignConstants.TextSecondary;
        detailsText.enableWordWrapping = true;
        
        string typeLabel = contract.modelType.ToString().ToUpper();
        string qualGoal = $"{contract.qualityThreshold:F0}% Quality";
        string payUp = $"Upfront: <color=#{ColorUtility.ToHtmlStringRGBA(GameDesignConstants.StatusSuccess)}>${contract.upfrontPayment:N0}</color>";
        string payCom = $"On Delivery: <color=#{ColorUtility.ToHtmlStringRGBA(GameDesignConstants.StatusSuccess)}>${contract.completionPayout:N0}</color>";
        string penalty = $"Penalty: <color=#{ColorUtility.ToHtmlStringRGBA(GameDesignConstants.StatusDanger)}>-${contract.failurePenalty:N0}</color>";
        
        if (contract.isAccepted)
        {
            string remainingText = contract.daysRemaining == 1 ? "1 day left" : $"{contract.daysRemaining} days left";
            detailsText.text = $"Type: <b>{typeLabel}</b> (Req: <b>{qualGoal}</b>)\n{payCom} | {penalty}\nTime: <color=#{ColorUtility.ToHtmlStringRGBA(GameDesignConstants.StatusWarning)}><b>{remainingText}</b></color>";
        }
        else
        {
            detailsText.text = $"Type: <b>{typeLabel}</b> (Req: <b>{qualGoal}</b>)\n{payUp} | {payCom}\nPenalty: -${contract.failurePenalty:N0}, -{contract.reputationPenalty:F0} Rep";
        }
        
        // Button Action
        var btnObj = new GameObject("Btn_Action");
        btnObj.transform.SetParent(rowObj.transform, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(1f, 0.5f);
        btnRect.anchorMax = new Vector2(1f, 0.5f);
        btnRect.pivot = new Vector2(1f, 0.5f);
        btnRect.sizeDelta = new Vector2(100f, 35f);
        btnRect.anchoredPosition = new Vector2(-15f, 0f);
        
        var btnImg = btnObj.AddComponent<Image>();
        var btnComp = btnObj.AddComponent<Button>();
        
        var btnTxtObj = new GameObject("Text");
        btnTxtObj.transform.SetParent(btnObj.transform, false);
        var btnTxtRect = btnTxtObj.AddComponent<RectTransform>();
        btnTxtRect.anchorMin = Vector2.zero;
        btnTxtRect.anchorMax = Vector2.one;
        btnTxtRect.offsetMin = Vector2.zero;
        btnTxtRect.offsetMax = Vector2.zero;
        var btnTxt = btnTxtObj.AddComponent<TextMeshProUGUI>();
        btnTxt.fontSize = 11f;
        btnTxt.alignment = TextAlignmentOptions.Center;
        btnTxt.fontStyle = FontStyles.Bold;
        btnTxt.color = Color.white;
        
        var stylized = btnObj.AddComponent<StylizedButton>();
        
        if (contract.isAccepted)
        {
            btnTxt.text = "ACTIVE";
            btnComp.interactable = false;
            stylized.SetVariant(StylizedButton.ButtonVariant.Secondary);
        }
        else
        {
            btnTxt.text = "ACCEPT";
            btnComp.onClick.AddListener(() => AcceptContract(contract));
            stylized.SetVariant(StylizedButton.ButtonVariant.Primary);
        }
    }
}
