using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Modal panel that appears when a project completes.
/// Shows quality, cost, clients gained, revenue, and reputation impact.
/// Offers Accept, Refine, or Abandon choices.
/// </summary>
public sealed class ProjectResultPanel : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private RectTransform panelRect;
    [SerializeField] private Image overlayImage;
    [SerializeField] private Image panelBackground;

    [Header("Project Info")]
    [SerializeField] private TextMeshProUGUI projectNameText;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Metrics")]
    [SerializeField] private TextMeshProUGUI qualityValueText;
    [SerializeField] private Image qualityFill;
    [SerializeField] private TextMeshProUGUI costValueText;
    [SerializeField] private TextMeshProUGUI clientsValueText;
    [SerializeField] private TextMeshProUGUI revenueValueText;
    [SerializeField] private TextMeshProUGUI reputationValueText;

    [Header("Star Rating")]
    [SerializeField] private Image[] starImages;

    [Header("Buttons")]
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button refineButton;
    [SerializeField] private Button abandonButton;

    [Header("Label Texts")]
    [SerializeField] private TextMeshProUGUI qualityLabel;
    [SerializeField] private TextMeshProUGUI costLabel;
    [SerializeField] private TextMeshProUGUI clientsLabel;
    [SerializeField] private TextMeshProUGUI revenueLabel;
    [SerializeField] private TextMeshProUGUI reputationLabel;

    /// <summary>Result data for the completed project.</summary>
    public struct ProjectResult
    {
        public string projectName;
        public float quality;       // 0-100
        public float totalCost;
        public int clientsGained;
        public float monthlyRevenue;
        public float reputationGain;
    }

    private ProjectResult currentResult;
    private System.Action<ResultAction> onDecision;

    public enum ResultAction
    {
        Accept,
        Refine,
        Abandon
    }

    private void Awake()
    {
        if (acceptButton != null)
            acceptButton.onClick.AddListener(() => OnDecision(ResultAction.Accept));
        if (refineButton != null)
            refineButton.onClick.AddListener(() => OnDecision(ResultAction.Refine));
        if (abandonButton != null)
            abandonButton.onClick.AddListener(() => OnDecision(ResultAction.Abandon));

        StylePanel();
        HideImmediate();
    }

    /// <summary>Show the result panel with animation.</summary>
    public void Show(ProjectResult result, System.Action<ResultAction> callback)
    {
        currentResult = result;
        onDecision = callback;

        gameObject.SetActive(true);
        PopulateData(result);
        StartCoroutine(AnimateIn());
    }

    /// <summary>Hide with animation.</summary>
    public void Hide()
    {
        StartCoroutine(AnimateOut());
    }

    private void HideImmediate()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;
        }
        gameObject.SetActive(false);
    }

    // ── Populate ─────────────────────────────────────────────────────

    private void PopulateData(ProjectResult r)
    {
        if (projectNameText != null)
        {
            projectNameText.text = r.projectName;
            projectNameText.color = GameDesignConstants.TextPrimary;
        }

        var qualityRating = GetQualityRating(r.quality);

        if (statusText != null)
        {
            statusText.text = $"Project Complete — {qualityRating}";
            statusText.color = GameDesignConstants.QualityColor(r.quality);
        }

        // Quality
        if (qualityValueText != null)
        {
            qualityValueText.text = $"{r.quality:F0}%";
            qualityValueText.color = GameDesignConstants.QualityColor(r.quality);
        }

        if (qualityFill != null)
        {
            qualityFill.fillAmount = r.quality / 100f;
            qualityFill.color = GameDesignConstants.QualityColor(r.quality);
        }

        // Stars
        var starCount = QualityToStars(r.quality);
        if (starImages != null)
        {
            for (var i = 0; i < starImages.Length; i++)
            {
                if (starImages[i] != null)
                {
                    starImages[i].color = i < starCount
                        ? GameDesignConstants.BrandAccent
                        : GameDesignConstants.TextMuted;
                }
            }
        }

        // Cost
        if (costValueText != null)
        {
            costValueText.text = $"${r.totalCost:N0}";
            costValueText.color = GameDesignConstants.StatusDanger;
        }

        // Clients
        if (clientsValueText != null)
        {
            clientsValueText.text = $"+{r.clientsGained}";
            clientsValueText.color = GameDesignConstants.StatusInfo;
        }

        // Revenue
        if (revenueValueText != null)
        {
            revenueValueText.text = $"${r.monthlyRevenue:N0}/mo";
            revenueValueText.color = GameDesignConstants.StatusSuccess;
        }

        // Reputation
        if (reputationValueText != null)
        {
            var sign = r.reputationGain >= 0 ? "+" : "";
            reputationValueText.text = $"{sign}{r.reputationGain:F1}";
            reputationValueText.color = r.reputationGain >= 0
                ? GameDesignConstants.StatusSuccess
                : GameDesignConstants.StatusDanger;
        }

        // Labels
        SetLabelColor(qualityLabel);
        SetLabelColor(costLabel);
        SetLabelColor(clientsLabel);
        SetLabelColor(revenueLabel);
        SetLabelColor(reputationLabel);
    }

    private void SetLabelColor(TextMeshProUGUI label)
    {
        if (label != null) label.color = GameDesignConstants.TextSecondary;
    }

    // ── Helpers ──────────────────────────────────────────────────────

    private static string GetQualityRating(float quality)
    {
        if (quality >= GameDesignConstants.QualityExcellent) return "★★★★★ EXCELLENT";
        if (quality >= GameDesignConstants.QualityGreat)     return "★★★★ GREAT";
        if (quality >= GameDesignConstants.QualityGood)      return "★★★ GOOD";
        if (quality >= GameDesignConstants.QualityDecent)    return "★★ DECENT";
        if (quality >= GameDesignConstants.QualityPoor)      return "★ POOR";
        return "✗ TERRIBLE";
    }

    private static int QualityToStars(float quality)
    {
        if (quality >= GameDesignConstants.QualityExcellent) return 5;
        if (quality >= GameDesignConstants.QualityGreat)     return 4;
        if (quality >= GameDesignConstants.QualityGood)      return 3;
        if (quality >= GameDesignConstants.QualityDecent)    return 2;
        if (quality >= GameDesignConstants.QualityPoor)      return 1;
        return 0;
    }

    // ── Animations ───────────────────────────────────────────────────

    private IEnumerator AnimateIn()
    {
        // Overlay fade
        if (overlayImage != null)
        {
            overlayImage.color = Color.clear;
            UIAnimations.LerpImageColor(this, overlayImage, Color.clear, GameDesignConstants.SurfaceOverlay, 0.3f);
        }

        // Panel scale + fade
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
        }

        if (panelRect != null)
        {
            UIAnimations.ScaleTransform(this, panelRect, Vector3.one * 0.85f, Vector3.one, 0.35f);
        }

        if (panelGroup != null)
        {
            UIAnimations.FadeCanvasGroup(this, panelGroup, 0f, 1f, 0.3f);
        }

        yield return new WaitForSecondsRealtime(0.35f);

        // Pause game
        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(TimeController.Speed.Paused);
        }
    }

    private IEnumerator AnimateOut()
    {
        if (panelGroup != null)
        {
            UIAnimations.FadeCanvasGroup(this, panelGroup, 1f, 0f, 0.25f);
        }

        if (overlayImage != null)
        {
            UIAnimations.LerpImageColor(this, overlayImage, overlayImage.color, Color.clear, 0.25f);
        }

        yield return new WaitForSecondsRealtime(0.3f);
        gameObject.SetActive(false);
    }

    // ── Decision ─────────────────────────────────────────────────────

    private void OnDecision(ResultAction action)
    {
        onDecision?.Invoke(action);
        Hide();
    }

    // ── Styling ──────────────────────────────────────────────────────

    private void StylePanel()
    {
        if (panelBackground != null)
        {
            panelBackground.color = GameDesignConstants.SurfaceDark;
        }
    }
}
