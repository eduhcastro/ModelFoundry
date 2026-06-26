using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class NOCController : MonoBehaviour
{
    public static NOCController Instance { get; private set; }

    [Header("UI Panel Reference")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Button closeButton;

    [Header("Resource Bars")]
    [SerializeField] private TextMeshProUGUI gpuCountText;
    [SerializeField] private TextMeshProUGUI energyText;
    [SerializeField] private Image energyFill;
    [SerializeField] private TextMeshProUGUI coolingText;
    [SerializeField] private Image coolingFill;

    [Header("System Status")]
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Image statusBackground;
    [SerializeField] private TextMeshProUGUI warningText;

    [Header("Upgrades")]
    [SerializeField] private Button upgradeGridButton;
    [SerializeField] private Button upgradeCoolingButton;
    [SerializeField] private float gridUpgradeCost = 8000f;
    [SerializeField] private float coolingUpgradeCost = 6000f;

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

        if (upgradeGridButton != null)
        {
            upgradeGridButton.onClick.AddListener(BuyGridUpgrade);
        }

        if (upgradeCoolingButton != null)
        {
            upgradeCoolingButton.onClick.AddListener(BuyCoolingUpgrade);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateNocUI;
        }

        UpdateNocUI();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateNocUI;
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
        UpdateNocUI();
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

    public void BuyGridUpgrade()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.BuyEnergyGridUpgrade(gridUpgradeCost))
        {
            ToastNotification.ShowGlobal("Power Grid upgraded successfully! +30kW Capacity, Burn Rate +$100/mo.", ToastNotification.Category.Success);
            UpdateNocUI();
        }
        else
        {
            ToastNotification.ShowGlobal("Failed to upgrade Power Grid! Insufficient cash.", ToastNotification.Category.Danger);
        }
    }

    public void BuyCoolingUpgrade()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.BuyCoolingUpgrade(coolingUpgradeCost))
        {
            ToastNotification.ShowGlobal("Cooling System upgraded successfully! +30kW Capacity, Burn Rate +$80/mo.", ToastNotification.Category.Success);
            UpdateNocUI();
        }
        else
        {
            ToastNotification.ShowGlobal("Failed to upgrade Cooling System! Insufficient cash.", ToastNotification.Category.Danger);
        }
    }

    public void UpdateNocUI()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gpuCountText != null)
        {
            gpuCountText.text = $"Active GPUs: {gm.GpuCount}";
        }

        // Energy Bar Update
        float energyUsage = gm.EnergyUsage;
        float energyCapacity = gm.EnergyCapacity;
        if (energyText != null)
        {
            energyText.text = $"Energy Load: {energyUsage:F1}kW / {energyCapacity:F1}kW";
        }
        if (energyFill != null)
        {
            energyFill.fillAmount = energyCapacity > 0 ? (energyUsage / energyCapacity) : 0f;
            energyFill.color = energyUsage > energyCapacity 
                ? GameDesignConstants.StatusDanger 
                : energyUsage > (energyCapacity * 0.8f) 
                    ? GameDesignConstants.StatusWarning 
                    : GameDesignConstants.StatusSuccess;
        }

        // Cooling Bar Update
        float coolingUsage = gm.CoolingUsage;
        float coolingCapacity = gm.CoolingCapacity;
        if (coolingText != null)
        {
            coolingText.text = $"Cooling Load: {coolingUsage:F1}kW / {coolingCapacity:F1}kW";
        }
        if (coolingFill != null)
        {
            coolingFill.fillAmount = coolingCapacity > 0 ? (coolingUsage / coolingCapacity) : 0f;
            coolingFill.color = coolingUsage > coolingCapacity 
                ? GameDesignConstants.StatusDanger 
                : coolingUsage > (coolingCapacity * 0.8f) 
                    ? GameDesignConstants.StatusWarning 
                    : GameDesignConstants.StatusSuccess;
        }

        // Overheat and Warning Text Status
        bool overload = gm.IsOverheating;
        if (statusText != null)
        {
            statusText.text = overload ? "OVERHEATING / CRITICAL" : "OPTIMAL";
            statusText.color = Color.white;
        }
        if (statusBackground != null)
        {
            statusBackground.color = overload ? GameDesignConstants.StatusDanger : GameDesignConstants.StatusSuccess;
        }

        if (warningText != null)
        {
            if (overload)
            {
                warningText.text = "WARNING: System overload detected! Training speed reduced by 50% due to emergency hardware throttling. Upgrade infrastructure immediately!";
                warningText.color = GameDesignConstants.StatusDanger;
            }
            else
            {
                warningText.text = "All systems operational. Server temperatures stable. Ready for model training.";
                warningText.color = GameDesignConstants.TextSecondary;
            }
        }

        // Upgrade Buttons Price Text
        if (upgradeGridButton != null)
        {
            var txt = upgradeGridButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = $"UPGRADE GRID (${gridUpgradeCost:N0})";
        }
        if (upgradeCoolingButton != null)
        {
            var txt = upgradeCoolingButton.GetComponentInChildren<TextMeshProUGUI>();
            if (txt != null) txt.text = $"UPGRADE COOLING (${coolingUpgradeCost:N0})";
        }
    }
}
