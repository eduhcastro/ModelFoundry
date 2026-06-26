using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class GPUUpgradeController : MonoBehaviour
{
    [Header("Upgrade Config")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private float upgradeCost = 10000f;
    [SerializeField] private float monthlyBurnIncrease = 300f;

    [Header("Visual References")]
    [SerializeField] private GameObject secondGpuCabinet;

    [Header("Dependencies")]
    [SerializeField] private PrototypeProjectController projectController;

    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(BuyUpgrade);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateUpgradeButtonState;
        }

        UpdateUpgradeButtonState();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateUpgradeButtonState;
        }
    }

    public void BuyUpgrade()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.GpuCount >= 2)
        {
            ToastNotification.ShowGlobal("GPU Rack already upgraded!", ToastNotification.Category.Warning);
            return;
        }

        float currentCost = upgradeCost * gm.GpuCostMultiplier;
        if (!gm.BuyGpuUpgrade(currentCost, monthlyBurnIncrease))
        {
            ToastNotification.ShowGlobal("Not enough cash to upgrade GPU Rack!", ToastNotification.Category.Danger);
            return;
        }

        // Upgrade successful!
        if (secondGpuCabinet != null)
        {
            secondGpuCabinet.SetActive(true);
        }

        // Apply training speed bonus (20% faster, multiplier 0.8f)
        if (projectController != null)
        {
            projectController.SetDuration(projectController.ProjectDuration * 0.8f);
            ToastNotification.ShowGlobal("Compute upgraded! Training speed increased by 20%.", ToastNotification.Category.Info);
        }

        ToastNotification.ShowGlobal("Second GPU Rack purchased! Burn Rate +$300/mo.", ToastNotification.Category.Success);

        UpdateUpgradeButtonState();
    }

    public void UpdateUpgradeButtonState()
    {
        if (upgradeButton != null)
        {
            var gm = GameManager.Instance;
            if (gm != null && gm.GpuCount >= 2)
            {
                upgradeButton.interactable = false;
                var text = upgradeButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null) text.text = "UPGRADED";
            }
            else
            {
                upgradeButton.interactable = true;
                var text = upgradeButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && gm != null)
                {
                    float currentCost = upgradeCost * gm.GpuCostMultiplier;
                    text.text = $"BUY GPU RACK (${currentCost:N0})";
                }
            }
        }
    }
}
