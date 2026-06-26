using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class OfficeUpgradeController : MonoBehaviour
{
    [Header("Upgrade Buttons")]
    [SerializeField] private Button upgradeT2Button;
    [SerializeField] private Button upgradeT3Button;
    [SerializeField] private Button upgradeT4Button;

    [Header("Pricing Config")]
    [SerializeField] private float t2Cost = 30000f;
    [SerializeField] private float t3Cost = 75000f;
    [SerializeField] private float t4Cost = 120000f;

    private void Start()
    {
        if (upgradeT2Button != null)
        {
            upgradeT2Button.onClick.AddListener(BuyT2Upgrade);
        }

        if (upgradeT3Button != null)
        {
            upgradeT3Button.onClick.AddListener(BuyT3Upgrade);
        }

        if (upgradeT4Button != null)
        {
            upgradeT4Button.onClick.AddListener(BuyT4Upgrade);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += UpdateButtonsState;
        }

        UpdateButtonsState();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= UpdateButtonsState;
        }
    }

    public void BuyT2Upgrade()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.OfficeTier >= 2)
        {
            ToastNotification.ShowGlobal("Office Tier 2 already upgraded!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(t2Cost))
        {
            ToastNotification.ShowGlobal("Not enough cash to upgrade to Corporate Suite!", ToastNotification.Category.Danger);
            return;
        }

        // Upgrade successful
        gm.SetOfficeTier(2);
        
        if (OfficeVisualController.Instance != null)
        {
            OfficeVisualController.Instance.ApplyOfficeVisuals(2);
        }

        ToastNotification.ShowGlobal("Corporate Suite unlocked! Burn Rate +$1,000/mo. ML Engineer & Scientist slots open!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void BuyT3Upgrade()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.OfficeTier < 2)
        {
            ToastNotification.ShowGlobal("Must upgrade to Corporate Suite (Tier 2) first!", ToastNotification.Category.Warning);
            return;
        }

        if (gm.OfficeTier >= 3)
        {
            ToastNotification.ShowGlobal("Secret R&D Lab already unlocked!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(t3Cost))
        {
            ToastNotification.ShowGlobal("Not enough cash to upgrade to Secret R&D Lab!", ToastNotification.Category.Danger);
            return;
        }

        // Upgrade successful
        gm.SetOfficeTier(3);
        
        if (OfficeVisualController.Instance != null)
        {
            OfficeVisualController.Instance.ApplyOfficeVisuals(3);
        }

        ToastNotification.ShowGlobal("Secret R&D Lab unlocked! Burn Rate +$2,500/mo. Safety Researcher & Advanced Techs unlocked!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void BuyT4Upgrade()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (gm.OfficeTier < 3)
        {
            ToastNotification.ShowGlobal("Must upgrade to Secret R&D Lab (Tier 3) first!", ToastNotification.Category.Warning);
            return;
        }

        if (gm.OfficeTier >= 4)
        {
            ToastNotification.ShowGlobal("Modular Datacenter already unlocked!", ToastNotification.Category.Warning);
            return;
        }

        if (!gm.SpendCash(t4Cost))
        {
            ToastNotification.ShowGlobal("Not enough cash to upgrade to Modular Datacenter!", ToastNotification.Category.Danger);
            return;
        }

        // Upgrade successful
        gm.SetOfficeTier(4);
        
        if (OfficeVisualController.Instance != null)
        {
            OfficeVisualController.Instance.ApplyOfficeVisuals(4);
        }

        ToastNotification.ShowGlobal("Modular Datacenter unlocked! Burn Rate +$5,000/mo. Infrastructure, GPU Tech, MLOps, & Backend Engineer slots open!", ToastNotification.Category.Success);
        UpdateButtonsState();
    }

    public void UpdateButtonsState()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Update Tier 2 Button
        if (upgradeT2Button != null)
        {
            var text = upgradeT2Button.GetComponentInChildren<TextMeshProUGUI>();
            if (gm.OfficeTier >= 2)
            {
                upgradeT2Button.interactable = false;
                if (text != null) text.text = "UPGRADED";
            }
            else
            {
                upgradeT2Button.interactable = true;
                if (text != null) text.text = $"BUY SUITE (${t2Cost:N0})";
            }
        }

        // Update Tier 3 Button
        if (upgradeT3Button != null)
        {
            var text = upgradeT3Button.GetComponentInChildren<TextMeshProUGUI>();
            if (gm.OfficeTier >= 3)
            {
                upgradeT3Button.interactable = false;
                if (text != null) text.text = "UPGRADED";
            }
            else if (gm.OfficeTier < 2)
            {
                upgradeT3Button.interactable = false;
                if (text != null) text.text = "LOCKED (Req. Tier 2)";
            }
            else
            {
                upgradeT3Button.interactable = true;
                if (text != null) text.text = $"BUY LAB (${t3Cost:N0})";
            }
        }

        // Update Tier 4 Button
        if (upgradeT4Button != null)
        {
            var text = upgradeT4Button.GetComponentInChildren<TextMeshProUGUI>();
            if (gm.OfficeTier >= 4)
            {
                upgradeT4Button.interactable = false;
                if (text != null) text.text = "UPGRADED";
            }
            else if (gm.OfficeTier < 3)
            {
                upgradeT4Button.interactable = false;
                if (text != null) text.text = "LOCKED (Req. Tier 3)";
            }
            else
            {
                upgradeT4Button.interactable = true;
                if (text != null) text.text = $"BUY DATACENTER (${t4Cost:N0})";
            }
        }
    }
}
