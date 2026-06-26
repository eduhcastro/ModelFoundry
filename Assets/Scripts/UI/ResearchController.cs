using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class ResearchController : MonoBehaviour
{
    public static ResearchController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Button closeButton;
    [SerializeField] private Button studyNlpButton;
    [SerializeField] private Button studyAgenticButton;
    [SerializeField] private Button studyGeneralButton;
    [SerializeField] private Button studySafetyAlignmentButton;
    [SerializeField] private Button studyCustomSiliconButton;

    [Header("Progress Fills")]
    [SerializeField] private Image nlpProgressFill;
    [SerializeField] private Image agenticProgressFill;
    [SerializeField] private Image generalProgressFill;
    [SerializeField] private Image safetyAlignmentProgressFill;
    [SerializeField] private Image customSiliconProgressFill;

    [Header("Progress Texts")]
    [SerializeField] private TextMeshProUGUI nlpProgressText;
    [SerializeField] private TextMeshProUGUI agenticProgressText;
    [SerializeField] private TextMeshProUGUI generalProgressText;
    [SerializeField] private TextMeshProUGUI safetyAlignmentProgressText;
    [SerializeField] private TextMeshProUGUI customSiliconProgressText;

    private bool isResearching = false;
    private string activeResearch = ""; // "NLP", "Agentic", "General", "SafetyAlignment", "CustomSilicon"
    private float progress = 0f;
    private int daysRequired = 0;
    private int daysElapsed = 0;

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
        if (studyNlpButton != null) studyNlpButton.onClick.AddListener(() => StartResearch("NLP", 3000f, 15));
        if (studyAgenticButton != null) studyAgenticButton.onClick.AddListener(() => StartResearch("Agentic", 7500f, 30));
        if (studyGeneralButton != null) studyGeneralButton.onClick.AddListener(() => StartResearch("General", 1000f, 7));
        if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.onClick.AddListener(() => StartResearch("SafetyAlignment", 15000f, 30));
        if (studyCustomSiliconButton != null) studyCustomSiliconButton.onClick.AddListener(() => StartResearch("CustomSilicon", 30000f, 45));

        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed += HandleDayPassed;
        }

        HidePanel();
        UpdateUI();
    }

    private void OnDestroy()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed -= HandleDayPassed;
        }
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
        UpdateUI();
    }

    public void HidePanel()
    {
        if (panelGroup == null) return;
        panelGroup.alpha = 0f;
        panelGroup.blocksRaycasts = false;
        panelGroup.interactable = false;
    }

    private void StartResearch(string type, float cost, int baseDays)
    {
        if (isResearching)
        {
            ToastNotification.ShowGlobal("Already researching another technology!", ToastNotification.Category.Warning);
            return;
        }

        var gm = GameManager.Instance;
        if (gm == null) return;

        // Verify requirements
        if (type == "Agentic" && !gm.IsNlpUnlocked)
        {
            ToastNotification.ShowGlobal("Must unlock NLP Chatbots first!", ToastNotification.Category.Warning);
            return;
        }

        if (type == "NLP" && gm.IsNlpUnlocked)
        {
            ToastNotification.ShowGlobal("NLP Chatbots already researched!", ToastNotification.Category.Warning);
            return;
        }

        if (type == "Agentic" && gm.IsAgenticUnlocked)
        {
            ToastNotification.ShowGlobal("Agentic Coders already researched!", ToastNotification.Category.Warning);
            return;
        }

        if ((type == "SafetyAlignment" || type == "CustomSilicon") && gm.OfficeTier < 3)
        {
            ToastNotification.ShowGlobal("Requires Office Tier 3 (Secret R&D Lab)!", ToastNotification.Category.Warning);
            return;
        }

        if (type == "SafetyAlignment" && gm.IsSafetyAlignmentResearched)
        {
            ToastNotification.ShowGlobal("Safety Alignment already researched!", ToastNotification.Category.Warning);
            return;
        }

        if (type == "CustomSilicon" && gm.IsCustomSiliconResearched)
        {
            ToastNotification.ShowGlobal("Custom Silicon already researched!", ToastNotification.Category.Warning);
            return;
        }

        if (gm.Cash < cost)
        {
            ToastNotification.ShowGlobal("Not enough cash to fund research!", ToastNotification.Category.Danger);
            return;
        }

        // Deduct cost
        gm.SpendCash(cost);

        // Calculate days with discounts: Research Scientist (30% discount), ML Engineer (20% discount)
        float teamMultiplier = 1.0f;
        if (gm.HasResearchScientist) teamMultiplier *= 0.7f;
        if (gm.HasMLEngineer) teamMultiplier *= 0.8f;
        int targetDays = Mathf.Max(1, Mathf.RoundToInt(baseDays * teamMultiplier));

        isResearching = true;
        activeResearch = type;
        progress = 0f;
        daysRequired = targetDays;
        daysElapsed = 0;

        ToastNotification.ShowGlobal($"Started studying {type}! Est. time: {targetDays} days.", ToastNotification.Category.Info);
        UpdateUI();
    }

    private void HandleDayPassed()
    {
        if (!isResearching) return;

        daysElapsed++;
        progress = (float)daysElapsed / daysRequired;

        if (daysElapsed >= daysRequired)
        {
            CompleteResearch();
        }
        else
        {
            UpdateUI();
        }
    }

    private void CompleteResearch()
    {
        isResearching = false;
        progress = 1.0f;

        var gm = GameManager.Instance;
        if (gm != null)
        {
            if (activeResearch == "NLP")
            {
                gm.UnlockNlp();
                gm.AddCompetence(20f);
                ToastNotification.ShowGlobal("RESEARCH COMPLETE: NLP Chatbots unlocked! +20 Competence", ToastNotification.Category.Success);
            }
            else if (activeResearch == "Agentic")
            {
                gm.UnlockAgentic();
                gm.AddCompetence(30f);
                ToastNotification.ShowGlobal("RESEARCH COMPLETE: Agentic Coders unlocked! +30 Competence", ToastNotification.Category.Success);
            }
            else if (activeResearch == "General")
            {
                gm.AddCompetence(15f);
                ToastNotification.ShowGlobal("STUDY COMPLETE: General competence improved! +15", ToastNotification.Category.Success);
            }
            else if (activeResearch == "SafetyAlignment")
            {
                gm.SetSafetyAlignmentResearched(true);
                gm.AddCompetence(25f);
                ToastNotification.ShowGlobal("RESEARCH COMPLETE: Safety Alignment implemented! Quality +10 points.", ToastNotification.Category.Success);
            }
            else if (activeResearch == "CustomSilicon")
            {
                gm.SetCustomSiliconResearched(true);
                gm.AddCompetence(35f);
                ToastNotification.ShowGlobal("RESEARCH COMPLETE: Custom Silicon developed! GPU Server burn halved.", ToastNotification.Category.Success);
            }
        }

        activeResearch = "";
        UpdateUI();
    }

    public void UpdateUI()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Study General Button
        if (isResearching && activeResearch == "General")
        {
            if (studyGeneralButton != null) studyGeneralButton.interactable = false;
            if (studyGeneralButton != null) studyGeneralButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDYING";
            if (generalProgressFill != null) generalProgressFill.fillAmount = progress;
            if (generalProgressText != null) generalProgressText.text = $"{progress * 100f:F0}%";
        }
        else
        {
            if (studyGeneralButton != null) studyGeneralButton.interactable = !isResearching;
            if (studyGeneralButton != null) studyGeneralButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDY GENERAL ($1k)";
            if (generalProgressFill != null) generalProgressFill.fillAmount = 0f;
            if (generalProgressText != null) generalProgressText.text = $"Competence: {gm.Competence:F0}";
        }

        // Study NLP Button & Unlock State
        if (gm.IsNlpUnlocked)
        {
            if (studyNlpButton != null) studyNlpButton.interactable = false;
            if (studyNlpButton != null) studyNlpButton.GetComponentInChildren<TextMeshProUGUI>().text = "UNLOCKED";
            if (nlpProgressFill != null) nlpProgressFill.fillAmount = 1f;
            if (nlpProgressText != null) nlpProgressText.text = "100%";
        }
        else if (isResearching && activeResearch == "NLP")
        {
            if (studyNlpButton != null) studyNlpButton.interactable = false;
            if (studyNlpButton != null) studyNlpButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDYING";
            if (nlpProgressFill != null) nlpProgressFill.fillAmount = progress;
            if (nlpProgressText != null) nlpProgressText.text = $"{progress * 100f:F0}%";
        }
        else
        {
            if (studyNlpButton != null) studyNlpButton.interactable = !isResearching;
            if (studyNlpButton != null) studyNlpButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDY NLP ($3k)";
            if (nlpProgressFill != null) nlpProgressFill.fillAmount = 0f;
            if (nlpProgressText != null) nlpProgressText.text = "0%";
        }

        // Study Agentic Button & Unlock State
        bool canStudyAgentic = gm.IsNlpUnlocked && !isResearching;
        if (gm.IsAgenticUnlocked)
        {
            if (studyAgenticButton != null) studyAgenticButton.interactable = false;
            if (studyAgenticButton != null) studyAgenticButton.GetComponentInChildren<TextMeshProUGUI>().text = "UNLOCKED";
            if (agenticProgressFill != null) agenticProgressFill.fillAmount = 1f;
            if (agenticProgressText != null) agenticProgressText.text = "100%";
        }
        else if (isResearching && activeResearch == "Agentic")
        {
            if (studyAgenticButton != null) studyAgenticButton.interactable = false;
            if (studyAgenticButton != null) studyAgenticButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDYING";
            if (agenticProgressFill != null) agenticProgressFill.fillAmount = progress;
            if (agenticProgressText != null) agenticProgressText.text = $"{progress * 100f:F0}%";
        }
        else
        {
            if (studyAgenticButton != null) studyAgenticButton.interactable = canStudyAgentic;
            if (studyAgenticButton != null) studyAgenticButton.GetComponentInChildren<TextMeshProUGUI>().text = gm.IsNlpUnlocked ? "STUDY AGENTIC ($7.5k)" : "LOCKED (Requires NLP)";
            if (agenticProgressFill != null) agenticProgressFill.fillAmount = 0f;
            if (agenticProgressText != null) agenticProgressText.text = "0%";
        }

        // Safety Alignment
        if (gm.IsSafetyAlignmentResearched)
        {
            if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.interactable = false;
            if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.GetComponentInChildren<TextMeshProUGUI>().text = "UNLOCKED";
            if (safetyAlignmentProgressFill != null) safetyAlignmentProgressFill.fillAmount = 1f;
            if (safetyAlignmentProgressText != null) safetyAlignmentProgressText.text = "100%";
        }
        else if (isResearching && activeResearch == "SafetyAlignment")
        {
            if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.interactable = false;
            if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDYING";
            if (safetyAlignmentProgressFill != null) safetyAlignmentProgressFill.fillAmount = progress;
            if (safetyAlignmentProgressText != null) safetyAlignmentProgressText.text = $"{progress * 100f:F0}%";
        }
        else
        {
            bool canStudySafety = gm.OfficeTier >= 3 && !isResearching;
            if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.interactable = canStudySafety;
            if (studySafetyAlignmentButton != null) studySafetyAlignmentButton.GetComponentInChildren<TextMeshProUGUI>().text = gm.OfficeTier >= 3 ? "SAFETY ALIGN ($15k)" : "LOCKED (Req. Lab)";
            if (safetyAlignmentProgressFill != null) safetyAlignmentProgressFill.fillAmount = 0f;
            if (safetyAlignmentProgressText != null) safetyAlignmentProgressText.text = "0%";
        }

        // Custom Silicon
        if (gm.IsCustomSiliconResearched)
        {
            if (studyCustomSiliconButton != null) studyCustomSiliconButton.interactable = false;
            if (studyCustomSiliconButton != null) studyCustomSiliconButton.GetComponentInChildren<TextMeshProUGUI>().text = "UNLOCKED";
            if (customSiliconProgressFill != null) customSiliconProgressFill.fillAmount = 1f;
            if (customSiliconProgressText != null) customSiliconProgressText.text = "100%";
        }
        else if (isResearching && activeResearch == "CustomSilicon")
        {
            if (studyCustomSiliconButton != null) studyCustomSiliconButton.interactable = false;
            if (studyCustomSiliconButton != null) studyCustomSiliconButton.GetComponentInChildren<TextMeshProUGUI>().text = "STUDYING";
            if (customSiliconProgressFill != null) customSiliconProgressFill.fillAmount = progress;
            if (customSiliconProgressText != null) customSiliconProgressText.text = $"{progress * 100f:F0}%";
        }
        else
        {
            bool canStudySilicon = gm.OfficeTier >= 3 && !isResearching;
            if (studyCustomSiliconButton != null) studyCustomSiliconButton.interactable = canStudySilicon;
            if (studyCustomSiliconButton != null) studyCustomSiliconButton.GetComponentInChildren<TextMeshProUGUI>().text = gm.OfficeTier >= 3 ? "CUSTOM SILICON ($30k)" : "LOCKED (Req. Lab)";
            if (customSiliconProgressFill != null) customSiliconProgressFill.fillAmount = 0f;
            if (customSiliconProgressText != null) customSiliconProgressText.text = "0%";
        }
    }
}
