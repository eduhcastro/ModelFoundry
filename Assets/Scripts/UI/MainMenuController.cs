using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Main menu controller with animated entrance, company name input,
/// and scene transition to the gameplay scene.
/// </summary>
public sealed class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup titleGroup;
    [SerializeField] private CanvasGroup buttonsGroup;
    [SerializeField] private CanvasGroup newGamePanel;
    [SerializeField] private CanvasGroup settingsPanel;
    [SerializeField] private Image fadeOverlay;

    [Header("Title")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI subtitleText;
    [SerializeField] private TextMeshProUGUI versionText;

    [Header("Buttons")]
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsBackButton;

    [Header("New Game Panel")]
    [SerializeField] private TMP_InputField companyNameInput;
    [SerializeField] private Button confirmNewGameButton;
    [SerializeField] private Button cancelNewGameButton;
    [SerializeField] private TextMeshProUGUI companyPreviewText;

    [Header("Settings")]
    [SerializeField] private string gameplaySceneName = "GaragePrototype";

    private bool isTransitioning;

    private void Awake()
    {
        SetupButtons();
        HideAllPanels();
    }

    private void Start()
    {
        StartCoroutine(PlayEntranceAnimation());
    }

    private void SetupButtons()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(OnContinueClicked);
            // Disable continue if no save exists
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, "save.json");
            continueButton.interactable = System.IO.File.Exists(savePath);
        }

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (confirmNewGameButton != null)
            confirmNewGameButton.onClick.AddListener(OnConfirmNewGame);

        if (cancelNewGameButton != null)
            cancelNewGameButton.onClick.AddListener(OnCancelNewGame);

        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(OnSettingsBackClicked);

        if (companyNameInput != null)
            companyNameInput.onValueChanged.AddListener(OnCompanyNameChanged);
    }

    // ── Entrance animation ───────────────────────────────────────────

    private IEnumerator PlayEntranceAnimation()
    {
        // Start with everything hidden
        if (fadeOverlay != null)
        {
            fadeOverlay.color = GameDesignConstants.SurfaceDarkest;
            fadeOverlay.gameObject.SetActive(true);
        }

        if (titleGroup != null) titleGroup.alpha = 0f;
        if (buttonsGroup != null) buttonsGroup.alpha = 0f;

        // Fade from black
        yield return FadeOverlay(1f, 0f, 0.8f);

        // Title slide in
        if (titleGroup != null)
        {
            var rt = titleGroup.GetComponent<RectTransform>();
            if (rt != null)
            {
                var target = rt.anchoredPosition;
                rt.anchoredPosition = target + new Vector2(0f, 40f);
                UIAnimations.FadeCanvasGroup(this, titleGroup, 0f, 1f, 0.6f);
                UIAnimations.SlideRectTransform(this, rt, rt.anchoredPosition, target, 0.6f);
            }
            else
            {
                UIAnimations.FadeCanvasGroup(this, titleGroup, 0f, 1f, 0.6f);
            }
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // Buttons fade in
        if (buttonsGroup != null)
        {
            var rt = buttonsGroup.GetComponent<RectTransform>();
            if (rt != null)
            {
                var target = rt.anchoredPosition;
                rt.anchoredPosition = target + new Vector2(0f, -30f);
                UIAnimations.FadeCanvasGroup(this, buttonsGroup, 0f, 1f, 0.5f);
                UIAnimations.SlideRectTransform(this, rt, rt.anchoredPosition, target, 0.5f);
            }
            else
            {
                UIAnimations.FadeCanvasGroup(this, buttonsGroup, 0f, 1f, 0.5f);
            }
        }

        yield return new WaitForSecondsRealtime(0.2f);

        // Version text
        if (versionText != null)
        {
            versionText.text = "Prototype Build v0.3";
            versionText.color = GameDesignConstants.TextMuted;
        }
    }

    // ── Button handlers ──────────────────────────────────────────────

    private void OnNewGameClicked()
    {
        if (isTransitioning) return;

        ShowPanel(newGamePanel);
        HidePanel(buttonsGroup);

        if (companyNameInput != null)
        {
            companyNameInput.text = "";
            companyNameInput.Select();
        }
    }

    private void OnContinueClicked()
    {
        if (isTransitioning) return;
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ShouldLoadSaveOnStart = true;
        }
        StartCoroutine(TransitionToGame("Model Foundry"));
    }

    private void OnSettingsClicked()
    {
        if (isTransitioning) return;
        ShowPanel(settingsPanel);
        HidePanel(buttonsGroup);
    }

    private void OnQuitClicked()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnConfirmNewGame()
    {
        if (isTransitioning) return;

        var name = companyNameInput != null ? companyNameInput.text : "";
        if (string.IsNullOrWhiteSpace(name)) name = "Model Foundry";

        StartCoroutine(TransitionToGame(name));
    }

    private void OnCancelNewGame()
    {
        HidePanel(newGamePanel);
        ShowPanel(buttonsGroup);
    }

    private void OnSettingsBackClicked()
    {
        HidePanel(settingsPanel);
        ShowPanel(buttonsGroup);
    }

    private void OnCompanyNameChanged(string value)
    {
        if (companyPreviewText != null)
        {
            companyPreviewText.text = string.IsNullOrWhiteSpace(value)
                ? "Model Foundry"
                : value;
        }
    }

    // ── Scene transition ─────────────────────────────────────────────

    private IEnumerator TransitionToGame(string companyName)
    {
        isTransitioning = true;

        // Fade to black
        yield return FadeOverlay(0f, 1f, 0.6f);

        // Initialize GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartNewGame(companyName);
        }

        // Load scene
        SceneManager.LoadScene(gameplaySceneName);
    }

    // ── Panel helpers ────────────────────────────────────────────────

    private void HideAllPanels()
    {
        HidePanelImmediate(newGamePanel);
        HidePanelImmediate(settingsPanel);
    }

    private void ShowPanel(CanvasGroup panel)
    {
        if (panel == null) return;
        panel.gameObject.SetActive(true);
        UIAnimations.FadeCanvasGroup(this, panel, 0f, 1f, GameDesignConstants.AnimNormal);
    }

    private void HidePanel(CanvasGroup panel)
    {
        if (panel == null) return;
        UIAnimations.FadeCanvasGroup(this, panel, 1f, 0f, GameDesignConstants.AnimFast, () =>
        {
            panel.gameObject.SetActive(false);
        });
    }

    private void HidePanelImmediate(CanvasGroup panel)
    {
        if (panel == null) return;
        panel.alpha = 0f;
        panel.interactable = false;
        panel.blocksRaycasts = false;
        panel.gameObject.SetActive(false);
    }

    private IEnumerator FadeOverlay(float from, float to, float duration)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / duration);
            var c = fadeOverlay.color;
            c.a = Mathf.Lerp(from, to, t);
            fadeOverlay.color = c;
            yield return null;
        }

        var final = fadeOverlay.color;
        final.a = to;
        fadeOverlay.color = final;

        if (to <= 0f) fadeOverlay.gameObject.SetActive(false);
    }
}
