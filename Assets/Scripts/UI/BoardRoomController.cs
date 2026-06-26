using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class BoardRoomController : MonoBehaviour
{
    public static BoardRoomController Instance { get; private set; }

    [Header("UI Panel Reference")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Button closeButton;

    [Header("VC Financing")]
    [SerializeField] private TextMeshProUGUI equityText;
    [SerializeField] private TextMeshProUGUI roundInfoText;
    [SerializeField] private Button acceptRoundButton;

    [Header("M&A (Mergers & Acquisitions)")]
    [SerializeField] private Button buyQuantumMindsButton;
    [SerializeField] private Button buyAnthroTechButton;
    [SerializeField] private float quantumMindsCost = 250000f;
    [SerializeField] private float anthroTechCost = 600000f;

    [Header("Board Goals & Trust")]
    [SerializeField] private TextMeshProUGUI boardTrustText;
    [SerializeField] private Image boardTrustFill;
    [SerializeField] private TextMeshProUGUI activeGoalText;
    [SerializeField] private TextMeshProUGUI remainingTimeText;

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
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HidePanel);
        }

        if (acceptRoundButton != null)
        {
            acceptRoundButton.onClick.AddListener(AcceptRound);
        }

        if (buyQuantumMindsButton != null)
        {
            buyQuantumMindsButton.onClick.AddListener(BuyQuantumMinds);
        }

        if (buyAnthroTechButton != null)
        {
            buyAnthroTechButton.onClick.AddListener(BuyAnthroTech);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateBoardUI;
        }

        UpdateBoardUI();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateBoardUI;
        }
    }

    public void ShowPanel()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 1f;
            panelGroup.blocksRaycasts = true;
            panelGroup.interactable = true;
        }
        UpdateBoardUI();
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

    public void AcceptRound()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        gm.AcceptNextFundingRound();
        UpdateBoardUI();
    }

    public void BuyQuantumMinds()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasAcquiredQuantumMinds) return;

        if (gm.SpendCash(quantumMindsCost))
        {
            gm.SetHasAcquiredQuantumMinds(true);
            ToastNotification.ShowGlobal("Acquired Quantum Minds! +30% Permanent training speed boost!", ToastNotification.Category.Success);
            if (TechPulseFeed.Instance != null)
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ M&A NEWS: @{gm.CompanyName.Replace(" ", "")} acquires AI startup Quantum Minds for $250k to accelerate training pipelines!");
            }
            UpdateBoardUI();
        }
        else
        {
            ToastNotification.ShowGlobal("Failed to acquire Quantum Minds! Insufficient cash.", ToastNotification.Category.Danger);
        }
    }

    public void BuyAnthroTech()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.HasAcquiredAnthroTech) return;

        if (gm.SpendCash(anthroTechCost))
        {
            gm.SetHasAcquiredAnthroTech(true);
            ToastNotification.ShowGlobal("Acquired AnthroTech! +10 Permanent base quality on all model launches!", ToastNotification.Category.Success);
            if (TechPulseFeed.Instance != null)
            {
                TechPulseFeed.Instance.AddPlayerPost($"▲ M&A NEWS: @{gm.CompanyName.Replace(" ", "")} acquires research lab AnthroTech for $600k to enhance model safety and alignment!");
            }
            UpdateBoardUI();
        }
        else
        {
            ToastNotification.ShowGlobal("Failed to acquire AnthroTech! Insufficient cash.", ToastNotification.Category.Danger);
        }
    }

    public void UpdateBoardUI()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Trust Bar
        if (boardTrustText != null)
        {
            boardTrustText.text = $"Board Trust: {gm.BoardTrust:F0}%";
        }
        if (boardTrustFill != null)
        {
            boardTrustFill.fillAmount = gm.BoardTrust / 100f;
            boardTrustFill.color = gm.BoardTrust < 40f ? GameDesignConstants.StatusDanger :
                                   gm.BoardTrust < 75f ? GameDesignConstants.StatusWarning :
                                                          GameDesignConstants.StatusSuccess;
        }

        // Equity
        if (equityText != null)
        {
            equityText.text = $"Founder Equity: {gm.FounderEquity:F0}%";
        }

        // Funding round
        if (roundInfoText != null && acceptRoundButton != null)
        {
            var btnText = acceptRoundButton.GetComponentInChildren<TextMeshProUGUI>();
            if (gm.FundingRound >= 4)
            {
                roundInfoText.text = "All funding rounds completed. The company is publicly traded.";
                acceptRoundButton.interactable = false;
                if (btnText != null) btnText.text = "IPO COMPLETED";
            }
            else
            {
                string nextRound = "";
                float baseCash = 0f;
                float equity = 0f;

                switch (gm.FundingRound)
                {
                    case 0:
                        nextRound = "Series A";
                        baseCash = 150000f;
                        equity = 15f;
                        break;
                    case 1:
                        nextRound = "Series B";
                        baseCash = 400000f;
                        equity = 20f;
                        break;
                    case 2:
                        nextRound = "Series C";
                        baseCash = 1000000f;
                        equity = 25f;
                        break;
                    case 3:
                        nextRound = "IPO";
                        baseCash = 5000000f;
                        equity = 40f;
                        break;
                }

                float finalCash = baseCash * (gm.HasFinanceLead ? 1.2f : 1.0f);
                roundInfoText.text = $"Next Round: {nextRound}\nGet ${finalCash:N0} for {equity}% equity.";
                acceptRoundButton.interactable = true;
                if (btnText != null) btnText.text = $"ACCEPT {nextRound.ToUpper()}";
            }
        }

        // M&A Buttons
        if (buyQuantumMindsButton != null)
        {
            var txt = buyQuantumMindsButton.GetComponentInChildren<TextMeshProUGUI>();
            if (gm.HasAcquiredQuantumMinds)
            {
                buyQuantumMindsButton.interactable = false;
                if (txt != null) txt.text = "ACQUIRED";
            }
            else
            {
                buyQuantumMindsButton.interactable = gm.Cash >= quantumMindsCost;
                if (txt != null) txt.text = $"BUY QUANTUM MINDS (${quantumMindsCost/1000f:F0}k)";
            }
        }

        if (buyAnthroTechButton != null)
        {
            var txt = buyAnthroTechButton.GetComponentInChildren<TextMeshProUGUI>();
            if (gm.HasAcquiredAnthroTech)
            {
                buyAnthroTechButton.interactable = false;
                if (txt != null) txt.text = "ACQUIRED";
            }
            else
            {
                buyAnthroTechButton.interactable = gm.Cash >= anthroTechCost;
                if (txt != null) txt.text = $"BUY ANTHROTECH (${anthroTechCost/1000f:F0}k)";
            }
        }

        // Active Goals
        if (activeGoalText != null && remainingTimeText != null)
        {
            if (gm.FundingRound == 0)
            {
                activeGoalText.text = "Board of Directors is not active yet.\n(Accept Series A to form the board)";
                remainingTimeText.text = "";
            }
            else
            {
                string goalDesc = "";
                switch (gm.ActiveBoardGoalType)
                {
                    case "Revenue":
                        goalDesc = $"Reach monthly revenue of at least ${gm.BoardGoalTarget:N0}";
                        break;
                    case "Followers":
                        goalDesc = $"Reach at least {gm.BoardGoalTarget:N0} followers on TechPulse";
                        break;
                    case "CostControl":
                        goalDesc = $"Keep monthly burn below ${gm.BoardGoalTarget:N0}";
                        break;
                    default:
                        goalDesc = "No active board goals.";
                        break;
                }

                activeGoalText.text = $"Current Meta:\n{goalDesc}";
                remainingTimeText.text = $"Time Remaining: {gm.BoardGoalRemainingMonths} months";
            }
        }
    }
}
