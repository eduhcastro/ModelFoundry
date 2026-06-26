using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public sealed class AnalyticsController : MonoBehaviour
{
    public static AnalyticsController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Button closeButton;

    [Header("Cash Chart")]
    [SerializeField] private RectTransform[] cashChartBars;
    [SerializeField] private TextMeshProUGUI[] cashChartLabels;
    [SerializeField] private TextMeshProUGUI maxCashLabel;

    [Header("Reputation Chart")]
    [SerializeField] private RectTransform[] repChartBars;
    [SerializeField] private TextMeshProUGUI[] repChartLabels;
    [SerializeField] private TextMeshProUGUI maxRepLabel;

    [Header("Competence Chart")]
    [SerializeField] private RectTransform[] compChartBars;
    [SerializeField] private TextMeshProUGUI[] compChartLabels;
    [SerializeField] private TextMeshProUGUI maxCompLabel;

    [Header("Model History")]
    [SerializeField] private TextMeshProUGUI recentModelsText;

    [Header("Stats Summary")]
    [SerializeField] private TextMeshProUGUI totalModelsText;
    [SerializeField] private TextMeshProUGUI avgQualityText;
    [SerializeField] private TextMeshProUGUI bestModelText;

    // History tracking
    private List<float> cashHistory = new List<float>();
    private List<float> followersHistory = new List<float>();
    private List<float> qualityHistory = new List<float>();
    private List<ModelRecord> modelHistory = new List<ModelRecord>();

    private const float MaxBarHeight = 65f; // Fit inside the slot height container

    // Dynamically generated line segments
    private List<RectTransform> cashLines = new List<RectTransform>();
    private List<RectTransform> followersLines = new List<RectTransform>();
    private List<RectTransform> qualityLines = new List<RectTransform>();

    public List<float> CashHistory => cashHistory;
    public List<float> FollowersHistory => followersHistory;
    public List<float> QualityHistory => qualityHistory;
    public List<ModelRecord> ModelHistoryLog => modelHistory;

    [System.Serializable]
    public struct ModelRecord
    {
        public string name;
        public float quality;
        public float revenue;
    }

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
        if (closeButton != null) closeButton.onClick.AddListener(HidePanel);

        if (TimeController.Instance != null)
            TimeController.Instance.OnMonthPassed += RecordMonthlySnapshot;

        if (GameManager.Instance != null)
        {
            cashHistory.Add(GameManager.Instance.Cash);
            followersHistory.Add(GameManager.Instance.Followers);
        }

        // Generate line segments behind the nodes
        InitializeLines(cashChartBars, cashLines, GameDesignConstants.BrandPrimary);
        InitializeLines(repChartBars, followersLines, GameDesignConstants.BrandSecondary);
        InitializeLines(compChartBars, qualityLines, GameDesignConstants.BrandAccent);

        HidePanel();
        RefreshAll();
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
            TimeController.Instance.OnMonthPassed -= RecordMonthlySnapshot;
    }

    public void TogglePanel()
    {
        if (panelGroup == null) return;
        bool visible = panelGroup.alpha > 0.5f;
        if (visible) HidePanel(); else ShowPanel();
    }

    public void ShowPanel()
    {
        if (panelGroup == null) return;
        panelGroup.alpha = 1f;
        panelGroup.blocksRaycasts = true;
        panelGroup.interactable = true;
        RefreshAll();
    }

    public void HidePanel()
    {
        if (panelGroup == null) return;
        panelGroup.alpha = 0f;
        panelGroup.blocksRaycasts = false;
        panelGroup.interactable = false;
    }

    private void InitializeLines(RectTransform[] dots, List<RectTransform> lines, Color color)
    {
        if (dots == null || dots.Length < 2) return;
        Transform container = dots[0].parent.parent; // BarsContainer

        for (int i = 0; i < dots.Length - 1; i++)
        {
            var lineObj = new GameObject($"Line_{i}", typeof(RectTransform));
            lineObj.transform.SetParent(container, false);
            lineObj.transform.SetAsFirstSibling(); // Draw line behind dots

            var rt = lineObj.GetComponent<RectTransform>();
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);

            var img = lineObj.AddComponent<Image>();
            img.color = new Color(color.r, color.g, color.b, 0.6f);

            lines.Add(rt);
        }
    }

    private void RecordMonthlySnapshot(int month = 0)
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        cashHistory.Add(gm.Cash);
        if (cashHistory.Count > 8) cashHistory.RemoveAt(0);

        followersHistory.Add(gm.Followers);
        if (followersHistory.Count > 8) followersHistory.RemoveAt(0);

        RefreshAll();
    }

    public void AddModelLaunch(string name, float quality, float revenue)
    {
        modelHistory.Insert(0, new ModelRecord { name = name, quality = quality, revenue = revenue });
        if (modelHistory.Count > 10) modelHistory.RemoveAt(modelHistory.Count - 1);

        qualityHistory.Add(quality);
        if (qualityHistory.Count > 8) qualityHistory.RemoveAt(0);

        RefreshAll();
    }

    public void RestoreHistory(List<float> cash, List<float> followers, List<float> quality, List<ModelRecord> models)
    {
        cashHistory = cash ?? new List<float>();
        followersHistory = followers ?? new List<float>();
        qualityHistory = quality ?? new List<float>();
        modelHistory = models ?? new List<ModelRecord>();
        RefreshAll();
    }

    private void RefreshAll()
    {
        UpdateChart(cashChartBars, cashLines, cashChartLabels, maxCashLabel, cashHistory, "$");
        UpdateChart(repChartBars, followersLines, repChartLabels, maxRepLabel, followersHistory, "");
        UpdateChart(compChartBars, qualityLines, compChartLabels, maxCompLabel, qualityHistory, "");
        UpdateModelList();
        UpdateStatsSummary();
    }

    private void UpdateChart(RectTransform[] dots, List<RectTransform> lines, TextMeshProUGUI[] labels, TextMeshProUGUI maxLabel, List<float> values, string prefix)
    {
        if (dots == null || dots.Length == 0) return;

        float maxVal = 1f;
        foreach (float val in values)
            if (val > maxVal) maxVal = val;

        if (maxLabel != null)
        {
            maxLabel.text = $"{prefix}{maxVal:N0}";
            maxLabel.color = GameDesignConstants.TextSecondary;
        }

        Vector2[] pointCoords = new Vector2[dots.Length];

        for (int i = 0; i < dots.Length; i++)
        {
            if (i < values.Count)
            {
                float ratio = Mathf.Clamp01(values[i] / maxVal);
                float height = ratio * MaxBarHeight;

                if (dots[i] != null)
                {
                    dots[i].gameObject.SetActive(true);

                    // Configure dots to be square data points
                    dots[i].anchorMin = new Vector2(0.5f, 0f);
                    dots[i].anchorMax = new Vector2(0.5f, 0f);
                    dots[i].pivot = new Vector2(0.5f, 0.5f);
                    dots[i].sizeDelta = new Vector2(8f, 8f);
                    dots[i].anchoredPosition = new Vector2(0f, height);

                    float posX = dots[i].parent.GetComponent<RectTransform>().anchoredPosition.x;
                    // Y position in BarsContainer space is slot bottom (-35f) + height
                    pointCoords[i] = new Vector2(posX, -35f + height);
                }

                if (labels[i] != null)
                {
                    labels[i].text = $"{prefix}{values[i]:F0}";
                    labels[i].color = GameDesignConstants.TextMuted;
                }
            }
            else
            {
                if (dots[i] != null)
                {
                    dots[i].gameObject.SetActive(false);
                }
                if (labels[i] != null)
                {
                    labels[i].text = "-";
                    labels[i].color = GameDesignConstants.TextMuted;
                }
            }
        }

        // Draw connecting segments
        if (lines != null)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (i < values.Count - 1 && dots[i] != null && dots[i + 1] != null)
                {
                    lines[i].gameObject.SetActive(true);
                    Vector2 pA = pointCoords[i];
                    Vector2 pB = pointCoords[i + 1];
                    Vector2 dir = pB - pA;
                    float dist = dir.magnitude;
                    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

                    lines[i].anchoredPosition = pA;
                    lines[i].sizeDelta = new Vector2(dist, 2f);
                    lines[i].localRotation = Quaternion.Euler(0f, 0f, angle);
                }
                else
                {
                    lines[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void UpdateModelList()
    {
        if (recentModelsText == null) return;

        if (modelHistory.Count == 0)
        {
            recentModelsText.text = "No models launched yet.";
            recentModelsText.color = GameDesignConstants.TextMuted;
        }
        else
        {
            var lines = new List<string>();
            for (int i = 0; i < Mathf.Min(modelHistory.Count, 6); i++)
            {
                var m = modelHistory[i];
                lines.Add($"#{(i + 1)} <b>{m.name}</b>\nQuality: {m.quality:F0}%  |  Rev: +${m.revenue:N0}/mo");
            }
            recentModelsText.text = string.Join("\n\n", lines);
            recentModelsText.color = GameDesignConstants.TextPrimary;
        }
    }

    private void UpdateStatsSummary()
    {
        if (totalModelsText == null || avgQualityText == null || bestModelText == null) return;

        totalModelsText.text = $"Total: {modelHistory.Count}";

        if (modelHistory.Count > 0)
        {
            float sum = 0f;
            float best = 0f;
            string bestName = "-";
            foreach (var m in modelHistory)
            {
                sum += m.quality;
                if (m.quality > best)
                {
                    best = m.quality;
                    bestName = m.name;
                }
            }
            avgQualityText.text = $"Avg Quality: {sum / modelHistory.Count:F0}%";
            bestModelText.text = $"Best: {bestName} ({best:F0}%)";
        }
        else
        {
            avgQualityText.text = "Avg Quality: -";
            bestModelText.text = "Best: -";
        }
    }
}
