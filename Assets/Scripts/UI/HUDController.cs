using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class HUDController : MonoBehaviour
{
    [Header("Top Bar")]
    [SerializeField] private TextMeshProUGUI companyNameText;
    [SerializeField] private TextMeshProUGUI dateText;
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Image topBarBackground;

    [Header("Speed Buttons")]
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button fastButton;
    [SerializeField] private Button ultraButton;
    [SerializeField] private Image pauseHighlight;
    [SerializeField] private Image normalHighlight;
    [SerializeField] private Image fastHighlight;
    [SerializeField] private Image ultraHighlight;

    [Header("Resource Bars")]
    [SerializeField] private ResourceBar cashBar;
    [SerializeField] private ResourceBar reputationBar;
    [SerializeField] private ResourceBar qualityBar;
    [SerializeField] private ResourceBar teamBar;
    [SerializeField] private ResourceBar competenceBar;
    [SerializeField] private Image resourcePanelBackground;

    [Header("Bottom Info")]
    [SerializeField] private TextMeshProUGUI monthlyBurnText;
    [SerializeField] private TextMeshProUGUI monthlyRevenueText;
    [SerializeField] private TextMeshProUGUI runwayText;
    [SerializeField] private TextMeshProUGUI clientsText;

    [Header("Panels")]
    [SerializeField] private CanvasGroup hudCanvasGroup;

    [Header("TechPulse")]
    [SerializeField] private Button techPulseButton;
    [SerializeField] private TechPulseUI techPulseUI;

    [Header("Right Dock Panel Groups")]
    [SerializeField] private CanvasGroup researchPanelGroup;
    [SerializeField] private CanvasGroup analyticsPanelGroup;
    [SerializeField] private CanvasGroup hiringPanelGroup;
    [SerializeField] private CanvasGroup gpuPanelGroup;
    [SerializeField] private CanvasGroup boardRoomPanelGroup;
    [SerializeField] private CanvasGroup systemPanelGroup;
    [SerializeField] private CanvasGroup contractsPanelGroup;
    [SerializeField] private CanvasGroup nocPanelGroup;

    [Header("Right Dock Panel")]
    [SerializeField] private Button researchButton;
    [SerializeField] private Button analyticsButton;
    [SerializeField] private Button hiringButton;
    [SerializeField] private Button gpuUpgradeButton;
    [SerializeField] private Button boardRoomButton;
    [SerializeField] private Button systemButton;
    [SerializeField] private Button contractsButton;
    [SerializeField] private Button nocButton;

    [Header("System Options")]
    [SerializeField] private Button systemQuitButton;

    [Header("Dock Icon Highlights")]
    [SerializeField] private Image researchIconHighlight;
    [SerializeField] private Image analyticsIconHighlight;
    [SerializeField] private Image hiringIconHighlight;
    [SerializeField] private Image gpuIconHighlight;
    [SerializeField] private Image boardRoomIconHighlight;
    [SerializeField] private Image systemIconHighlight;
    [SerializeField] private Image contractsIconHighlight;
    [SerializeField] private Image nocIconHighlight;

    [Header("Right Click Context Menu")]
    [SerializeField] private GameObject contextMenuPanel;
    [SerializeField] private Button contextLaunchProductBtn;
    [SerializeField] private Button contextManageTeamBtn;
    [SerializeField] private Button contextMarketingBtn;
    [SerializeField] private Button contextTeaserBtn;

    [Header("Target Panels & Close Actions")]
    [SerializeField] private GameObject projectPanelObj;
    [SerializeField] private GameObject summaryPanelObj;
    [SerializeField] private Button projectCloseButton;
    [SerializeField] private Button summaryCloseButton;

    private void Awake()
    {
        SetupSpeedButtons();
        StyleBackgrounds();
        SetupRightDockButtons();
        SetupContextMenuListeners();
    }

    private void SetupContextMenuListeners()
    {
        if (contextLaunchProductBtn != null)
        {
            contextLaunchProductBtn.onClick.AddListener(() => {
                if (projectPanelObj != null) projectPanelObj.SetActive(true);
                if (contextMenuPanel != null) contextMenuPanel.SetActive(false);
            });
        }

        if (contextManageTeamBtn != null)
        {
            contextManageTeamBtn.onClick.AddListener(() => {
                if (summaryPanelObj != null) summaryPanelObj.SetActive(true);
                if (contextMenuPanel != null) contextMenuPanel.SetActive(false);
            });
        }

        if (contextMarketingBtn != null)
        {
            contextMarketingBtn.onClick.AddListener(() => {
                if (TechPulseFollowerSystem.Instance != null)
                {
                    TechPulseFollowerSystem.Instance.PerformMarketingCampaign(2000f, UnityEngine.Random.Range(50, 150), 3f);
                }
                if (contextMenuPanel != null) contextMenuPanel.SetActive(false);
            });
        }

        if (contextTeaserBtn != null)
        {
            contextTeaserBtn.onClick.AddListener(() => {
                if (TechPulseFollowerSystem.Instance != null)
                {
                    TechPulseFollowerSystem.Instance.RecordActivity();
                }
                if (TechPulseFeed.Instance != null)
                {
                    TechPulseFeed.Instance.AddOrganicPlayerPost(false, 0);
                    ToastNotification.ShowGlobal("Teaser published on TechPulse!", ToastNotification.Category.Success);
                }
                if (contextMenuPanel != null) contextMenuPanel.SetActive(false);
            });
        }

        if (projectCloseButton != null)
        {
            projectCloseButton.onClick.AddListener(() => {
                if (projectPanelObj != null) projectPanelObj.SetActive(false);
            });
        }

        if (summaryCloseButton != null)
        {
            summaryCloseButton.onClick.AddListener(() => {
                if (summaryPanelObj != null) summaryPanelObj.SetActive(false);
            });
        }

        if (contextMenuPanel != null)
        {
            contextMenuPanel.SetActive(false);
        }
    }

    private void SetupRightDockButtons()
    {
        if (techPulseButton != null && techPulseUI != null)
            techPulseButton.onClick.AddListener(ToggleTechPulse);

        if (researchButton != null)
            researchButton.onClick.AddListener(() => TogglePanel(researchPanelGroup, researchIconHighlight));

        if (contractsButton != null)
            contractsButton.onClick.AddListener(() => TogglePanel(contractsPanelGroup, contractsIconHighlight));

        if (analyticsButton != null)
            analyticsButton.onClick.AddListener(() => TogglePanel(analyticsPanelGroup, analyticsIconHighlight));

        if (hiringButton != null)
            hiringButton.onClick.AddListener(TriggerHiring);

        if (gpuUpgradeButton != null)
            gpuUpgradeButton.onClick.AddListener(TriggerGpuUpgrade);

        if (boardRoomButton != null)
            boardRoomButton.onClick.AddListener(() => TogglePanel(boardRoomPanelGroup, boardRoomIconHighlight));

        if (nocButton != null)
            nocButton.onClick.AddListener(() => TogglePanel(nocPanelGroup, nocIconHighlight));

        if (systemButton != null)
            systemButton.onClick.AddListener(() => TogglePanel(systemPanelGroup, systemIconHighlight));

        if (systemQuitButton != null)
            systemQuitButton.onClick.AddListener(QuitToMainMenu);
    }

    private void TriggerHiring()
    {
        TogglePanel(hiringPanelGroup, hiringIconHighlight);
    }

    private void TriggerGpuUpgrade()
    {
        TogglePanel(gpuPanelGroup, gpuIconHighlight);
    }



    private void Start()
    {
        SubscribeToEvents();
        RefreshAll();
        RefreshCompetenceBar();

        if (hudCanvasGroup != null)
            UIAnimations.FadeCanvasGroup(this, hudCanvasGroup, 0f, 1f, GameDesignConstants.AnimSlow);
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void Update()
    {
        UpdateTimeDisplay();
        HandleContextMenuInput();
    }

    private void HandleContextMenuInput()
    {
        if (Input.GetMouseButtonDown(1)) // Right click
        {
            if (contextMenuPanel != null)
            {
                contextMenuPanel.SetActive(true);
                contextMenuPanel.transform.position = Input.mousePosition;
                
                // Align within screen bounds
                var rect = contextMenuPanel.GetComponent<RectTransform>();
                if (rect != null)
                {
                    // Adjust position so it doesn't go off screen
                    Vector2 localPos;
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        contextMenuPanel.transform.parent as RectTransform,
                        Input.mousePosition,
                        null,
                        out localPos
                    );
                    
                    // Clamp localPos to parent container
                    RectTransform parentRect = contextMenuPanel.transform.parent as RectTransform;
                    if (parentRect != null)
                    {
                        float minX = -parentRect.rect.width / 2f;
                        float maxX = parentRect.rect.width / 2f - rect.rect.width;
                        float minY = -parentRect.rect.height / 2f + rect.rect.height;
                        float maxY = parentRect.rect.height / 2f;
                        
                        localPos.x = Mathf.Clamp(localPos.x, minX, maxX);
                        localPos.y = Mathf.Clamp(localPos.y, minY, maxY);
                        rect.anchoredPosition = localPos;
                    }
                }
            }
        }
        else if (Input.GetMouseButtonDown(0)) // Left click
        {
            if (contextMenuPanel != null && contextMenuPanel.activeSelf)
            {
                // Check if the click is outside the context menu
                var rect = contextMenuPanel.GetComponent<RectTransform>();
                if (rect != null)
                {
                    if (!RectTransformUtility.RectangleContainsScreenPoint(rect, Input.mousePosition, null))
                    {
                        contextMenuPanel.SetActive(false);
                    }
                }
            }
        }
    }

    private System.Collections.IEnumerator HideContextMenuNextFrame()
    {
        yield return null;
        if (contextMenuPanel != null)
        {
            // Check if mouse is actually over the context panel or if we just clicked a button
            contextMenuPanel.SetActive(false);
        }
    }

    private void SetupSpeedButtons()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(() => SetSpeed(TimeController.Speed.Paused));
        if (normalButton != null)
            normalButton.onClick.AddListener(() => SetSpeed(TimeController.Speed.Normal));
        if (fastButton != null)
            fastButton.onClick.AddListener(() => SetSpeed(TimeController.Speed.Fast));
        if (ultraButton != null)
            ultraButton.onClick.AddListener(() => SetSpeed(TimeController.Speed.Ultra));
    }

    private void StyleBackgrounds()
    {
        if (topBarBackground != null)
            topBarBackground.color = GameDesignConstants.SurfaceGlass;

        if (resourcePanelBackground != null)
            resourcePanelBackground.color = GameDesignConstants.SurfaceGlass;
    }

    private void SubscribeToEvents()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnCashChanged       += OnCashChanged;
        GameManager.Instance.OnReputationChanged  += OnReputationChanged;
        GameManager.Instance.OnQualityChanged     += OnQualityChanged;
        GameManager.Instance.OnTeamChanged        += OnTeamChanged;
        GameManager.Instance.OnCompetenceChanged  += OnCompetenceChanged;
        GameManager.Instance.OnGameStateChanged   += OnGameStateChanged;

        if (TimeController.Instance != null)
            TimeController.Instance.OnSpeedChanged += OnSpeedChanged;
    }

    private void UnsubscribeFromEvents()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.OnCashChanged       -= OnCashChanged;
        GameManager.Instance.OnReputationChanged  -= OnReputationChanged;
        GameManager.Instance.OnQualityChanged     -= OnQualityChanged;
        GameManager.Instance.OnTeamChanged        -= OnTeamChanged;
        GameManager.Instance.OnCompetenceChanged  -= OnCompetenceChanged;
        GameManager.Instance.OnGameStateChanged   -= OnGameStateChanged;

        if (TimeController.Instance != null)
            TimeController.Instance.OnSpeedChanged -= OnSpeedChanged;
    }

    private void OnCashChanged(float value)
    {
        if (cashBar != null) cashBar.SetValue(value);
    }

    private void OnReputationChanged(float value)
    {
        if (reputationBar != null) reputationBar.SetValue(value);
    }

    private void OnQualityChanged(float value)
    {
        if (qualityBar != null) qualityBar.SetValue(value);
    }

    private void OnTeamChanged(int value)
    {
        if (teamBar != null) teamBar.SetValue(value);
    }

    private void OnCompetenceChanged(float value)
    {
        RefreshCompetenceBar();
    }

    private void OnGameStateChanged()
    {
        RefreshBottomInfo();
    }

    private void OnSpeedChanged(TimeController.Speed speed)
    {
        UpdateSpeedHighlights(speed);
    }

    private void RefreshAll()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (companyNameText != null)
        {
            companyNameText.text = $"▲  {gm.CompanyName.ToUpper()}";
            companyNameText.color = GameDesignConstants.BrandSecondary; // cyber cyan
        }

        if (cashBar != null) cashBar.SetValueImmediate(gm.Cash);
        if (reputationBar != null) reputationBar.SetValueImmediate(gm.Reputation);
        if (qualityBar != null) qualityBar.SetValueImmediate(gm.ModelQuality);
        if (teamBar != null) teamBar.SetValueImmediate(gm.TeamSize);
        if (competenceBar != null) competenceBar.SetValueImmediate(gm.Competence);

        RefreshBottomInfo();

        if (TimeController.Instance != null)
            UpdateSpeedHighlights(TimeController.Instance.CurrentSpeed);
    }

    private void RefreshCompetenceBar()
    {
        if (competenceBar == null || GameManager.Instance == null) return;
        competenceBar.SetValue(GameManager.Instance.Competence);
    }

    private void RefreshBottomInfo()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (monthlyBurnText != null)
        {
            monthlyBurnText.text = $"Burn: ${gm.MonthlyBurn:N0}/mo";
            monthlyBurnText.color = GameDesignConstants.StatusDanger;
        }

        if (monthlyRevenueText != null)
        {
            monthlyRevenueText.text = $"Revenue: ${gm.MonthlyRevenue:N0}/mo";
            monthlyRevenueText.color = GameDesignConstants.StatusSuccess;
        }

        if (runwayText != null)
        {
            var runway = gm.Runway;
            if (float.IsPositiveInfinity(runway))
            {
                runwayText.text = "Runway: \u221E";
                runwayText.color = GameDesignConstants.StatusSuccess;
            }
            else
            {
                runwayText.text = $"Runway: {runway:F1}mo";
                runwayText.color = runway < 3f ? GameDesignConstants.StatusDanger :
                                   runway < 6f ? GameDesignConstants.StatusWarning :
                                                  GameDesignConstants.StatusSuccess;
            }
        }

        if (clientsText != null)
        {
            clientsText.text = $"Clients: {gm.TotalClients}";
            clientsText.color = GameDesignConstants.TextSecondary;
        }
    }

    private void UpdateTimeDisplay()
    {
        var tc = TimeController.Instance;
        if (tc == null) return;

        if (dateText != null)
        {
            dateText.text = tc.FormattedDate;
            dateText.color = GameDesignConstants.TextPrimary;
        }

        if (speedText != null)
        {
            speedText.text = tc.SpeedLabel;
            speedText.color = tc.CurrentSpeed == TimeController.Speed.Paused
                ? GameDesignConstants.StatusWarning
                : GameDesignConstants.BrandSecondary;
        }
    }

    private void SetSpeed(TimeController.Speed speed)
    {
        if (TimeController.Instance != null)
            TimeController.Instance.SetSpeed(speed);
    }

    private void UpdateSpeedHighlights(TimeController.Speed speed)
    {
        SetHighlight(pauseHighlight,  speed == TimeController.Speed.Paused);
        SetHighlight(normalHighlight, speed == TimeController.Speed.Normal);
        SetHighlight(fastHighlight,   speed == TimeController.Speed.Fast);
        SetHighlight(ultraHighlight,  speed == TimeController.Speed.Ultra);
    }

    private void SetHighlight(Image highlight, bool active)
    {
        if (highlight == null) return;
        highlight.color = active
            ? GameDesignConstants.BrandPrimary
            : Color.clear;
    }

    private void ToggleTechPulse()
    {
        if (techPulseUI == null) return;

        if (techPulseUI.GetComponent<CanvasGroup>() != null && techPulseUI.GetComponent<CanvasGroup>().alpha < 0.1f)
            CloseAllPanelsExcept(null);

        techPulseUI.Toggle();
    }

    private void TogglePanel(CanvasGroup targetPanel, Image iconHighlight = null)
    {
        if (targetPanel == null) return;

        bool currentlyVisible = targetPanel.alpha > 0.5f;
        CloseAllPanelsExcept(targetPanel);

        if (!currentlyVisible && techPulseUI != null)
            techPulseUI.Close();

        if (currentlyVisible)
        {
            HideDockPanel(targetPanel);
            if (iconHighlight != null) iconHighlight.color = Color.clear;
        }
        else
        {
            ShowDockPanel(targetPanel);
            if (iconHighlight != null) iconHighlight.color = GameDesignConstants.BrandSecondary;
        }
    }

    private void ShowDockPanel(CanvasGroup p)
    {
        if (p == null) return;
        p.alpha = 1f;
        p.blocksRaycasts = true;
        p.interactable = true;

        if (p == researchPanelGroup && ResearchController.Instance != null)
            ResearchController.Instance.ShowPanel();
        if (p == contractsPanelGroup && ContractController.Instance != null)
            ContractController.Instance.ShowPanel();
        if (p == analyticsPanelGroup && AnalyticsController.Instance != null)
            AnalyticsController.Instance.ShowPanel();
        if (p == nocPanelGroup && NOCController.Instance != null)
            NOCController.Instance.ShowPanel();
        if (p == boardRoomPanelGroup && BoardRoomController.Instance != null)
            BoardRoomController.Instance.ShowPanel();
    }

    public void HideDockPanel(CanvasGroup p)
    {
        if (p == null) return;
        
        // Prevent infinite recursion if already hidden
        if (p.alpha == 0f && !p.blocksRaycasts && !p.interactable)
            return;

        p.alpha = 0f;
        p.blocksRaycasts = false;
        p.interactable = false;

        if (p == researchPanelGroup && ResearchController.Instance != null)
            ResearchController.Instance.HidePanel();
        if (p == contractsPanelGroup && ContractController.Instance != null)
            ContractController.Instance.HidePanel();
        if (p == analyticsPanelGroup && AnalyticsController.Instance != null)
            AnalyticsController.Instance.HidePanel();
        if (p == nocPanelGroup && NOCController.Instance != null)
            NOCController.Instance.HidePanel();
        if (p == boardRoomPanelGroup && BoardRoomController.Instance != null)
            BoardRoomController.Instance.HidePanel();
    }

    private void CloseAllPanelsExcept(CanvasGroup excludePanel)
    {
        CanvasGroup[] panels = { researchPanelGroup, analyticsPanelGroup, hiringPanelGroup, gpuPanelGroup, boardRoomPanelGroup, systemPanelGroup, contractsPanelGroup, nocPanelGroup };
        Image[] highlights = { researchIconHighlight, analyticsIconHighlight, hiringIconHighlight, gpuIconHighlight, boardRoomIconHighlight, systemIconHighlight, contractsIconHighlight, nocIconHighlight };

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] != null && panels[i] != excludePanel)
            {
                HideDockPanel(panels[i]);
                if (highlights[i] != null) highlights[i].color = Color.clear;
            }
        }
    }

    private void QuitToMainMenu()
    {
        UnsubscribeFromEvents();
        if (TimeController.Instance != null)
            TimeController.Instance.SetSpeed(TimeController.Speed.Normal);
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
