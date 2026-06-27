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
    [SerializeField] private Transform identityControlsRoot;

    [Header("Settings")]
    [SerializeField] private string gameplaySceneName = "GaragePrototype";

    private bool isTransitioning;
    private string selectedFontKey = "default";
    private Color selectedCompanyColor;
    private string selectedIconKey = "pixflow";
    private bool identityControlsCreated;
    private readonly System.Collections.Generic.List<Button> fontButtons = new System.Collections.Generic.List<Button>();
    private readonly System.Collections.Generic.List<Button> colorButtons = new System.Collections.Generic.List<Button>();
    private readonly System.Collections.Generic.List<Button> iconButtons = new System.Collections.Generic.List<Button>();

    private void Awake()
    {
        selectedCompanyColor = CompanyIdentityCatalog.DefaultColor;
        selectedIconKey = CompanyIdentityCatalog.DefaultIconKey;
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
        EnsureIdentityControls();
        RefreshIdentityControls();

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
            CompanyIdentityCatalog.ApplyToCompanyText(companyPreviewText, selectedFontKey, selectedCompanyColor);
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
            GameManager.Instance.StartNewGame(companyName, selectedFontKey, selectedCompanyColor, selectedIconKey);
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

    private void EnsureIdentityControls()
    {
        if (identityControlsCreated || newGamePanel == null)
        {
            return;
        }

        identityControlsCreated = true;

        var panelRect = newGamePanel.GetComponent<RectTransform>();
        if (panelRect != null && panelRect.sizeDelta.y < 500f)
        {
            panelRect.sizeDelta = new Vector2(panelRect.sizeDelta.x, 520f);
        }

        MovePanelButton(confirmNewGameButton, new Vector2(-105f, -190f));
        MovePanelButton(cancelNewGameButton, new Vector2(105f, -190f));

        var root = identityControlsRoot;
        if (root == null)
        {
            var rootObject = new GameObject("CompanyIdentityControls");
            rootObject.transform.SetParent(newGamePanel.transform, false);
            var rootRect = rootObject.AddComponent<RectTransform>();
            rootRect.anchorMin = new Vector2(0.5f, 0.5f);
            rootRect.anchorMax = new Vector2(0.5f, 0.5f);
            rootRect.pivot = new Vector2(0.5f, 0.5f);
            rootRect.sizeDelta = new Vector2(420f, 190f);
            rootRect.anchoredPosition = new Vector2(0f, -85f);
            root = rootObject.transform;
        }

        CreateLabel(root, "FONT", new Vector2(-185f, 70f));
        var fontStartX = -120f;
        for (var i = 0; i < CompanyIdentityCatalog.FontOptions.Length; i++)
        {
            var option = CompanyIdentityCatalog.FontOptions[i];
            var button = CreateChoiceButton(root, "Font_" + option.Key, option.Label, new Vector2(fontStartX + i * 82f, 70f), new Vector2(76f, 30f));
            button.onClick.AddListener(() =>
            {
                selectedFontKey = option.Key;
                RefreshIdentityControls();
            });
            fontButtons.Add(button);
        }

        CreateLabel(root, "COLOR", new Vector2(-185f, 20f));
        var colorStartX = -120f;
        for (var i = 0; i < CompanyIdentityCatalog.ColorOptions.Length; i++)
        {
            var color = CompanyIdentityCatalog.ColorOptions[i];
            var button = CreateColorButton(root, "Color_" + i, color, new Vector2(colorStartX + i * 44f, 20f));
            button.onClick.AddListener(() =>
            {
                selectedCompanyColor = color;
                RefreshIdentityControls();
            });
            colorButtons.Add(button);
        }

        CreateLabel(root, "ICON", new Vector2(-185f, -40f));
        var iconStartX = -120f;
        for (var i = 0; i < CompanyIdentityCatalog.IconOptions.Length; i++)
        {
            var option = CompanyIdentityCatalog.IconOptions[i];
            var button = CreateIconButton(root, "Icon_" + option.Key, option.Label, option.Key, new Vector2(iconStartX + i * 120f, -42f));
            button.onClick.AddListener(() =>
            {
                selectedIconKey = option.Key;
                RefreshIdentityControls();
            });
            iconButtons.Add(button);
        }
    }

    private void RefreshIdentityControls()
    {
        if (companyPreviewText != null)
        {
            companyPreviewText.text = companyNameInput != null && !string.IsNullOrWhiteSpace(companyNameInput.text)
                ? companyNameInput.text
                : "Model Foundry";
            CompanyIdentityCatalog.ApplyToCompanyText(companyPreviewText, selectedFontKey, selectedCompanyColor);
        }

        for (var i = 0; i < fontButtons.Count; i++)
        {
            var selected = CompanyIdentityCatalog.FontOptions[i].Key == selectedFontKey;
            SetButtonVisual(fontButtons[i], selected ? selectedCompanyColor : GameDesignConstants.SurfaceLight, selected ? Color.white : GameDesignConstants.TextPrimary);
        }

        for (var i = 0; i < colorButtons.Count; i++)
        {
            var selected = CompanyIdentityCatalog.ColorToHex(CompanyIdentityCatalog.ColorOptions[i]) == CompanyIdentityCatalog.ColorToHex(selectedCompanyColor);
            var label = colorButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = selected ? "✓" : "";
            }
        }

        for (var i = 0; i < iconButtons.Count; i++)
        {
            var selected = CompanyIdentityCatalog.IconOptions[i].Key == selectedIconKey;
            SetButtonVisual(iconButtons[i], selected ? selectedCompanyColor : GameDesignConstants.SurfaceLight, selected ? Color.white : GameDesignConstants.TextPrimary);
        }
    }

    private void MovePanelButton(Button button, Vector2 position)
    {
        if (button == null)
        {
            return;
        }

        var rect = button.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchoredPosition = position;
        }
    }

    private void CreateLabel(Transform parent, string text, Vector2 position)
    {
        var labelObject = new GameObject(text + "Label");
        labelObject.transform.SetParent(parent, false);
        var rect = labelObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(70f, 24f);
        rect.anchoredPosition = position;
        var label = labelObject.AddComponent<TextMeshProUGUI>();
        label.text = text;
        label.fontSize = 11f;
        label.fontStyle = FontStyles.Bold;
        label.alignment = TextAlignmentOptions.Left;
        label.color = GameDesignConstants.TextSecondary;
    }

    private Button CreateChoiceButton(Transform parent, string name, string label, Vector2 position, Vector2 size)
    {
        var buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent, false);
        var rect = buttonObject.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        var image = buttonObject.AddComponent<Image>();
        image.color = GameDesignConstants.SurfaceLight;
        var button = buttonObject.AddComponent<Button>();

        var labelObject = new GameObject("Label");
        labelObject.transform.SetParent(buttonObject.transform, false);
        var labelRect = labelObject.AddComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        var text = labelObject.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 10f;
        text.fontStyle = FontStyles.Bold;
        text.alignment = TextAlignmentOptions.Center;
        text.color = GameDesignConstants.TextPrimary;
        return button;
    }

    private Button CreateColorButton(Transform parent, string name, Color color, Vector2 position)
    {
        var button = CreateChoiceButton(parent, name, "", position, new Vector2(32f, 32f));
        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = color;
        }
        return button;
    }

    private Button CreateIconButton(Transform parent, string name, string label, string iconKey, Vector2 position)
    {
        var button = CreateChoiceButton(parent, name, label, position, new Vector2(110f, 42f));
        var iconSprite = CompanyIdentityCatalog.LoadCompanyIcon(iconKey);
        if (iconSprite == null)
        {
            return button;
        }

        var iconObject = new GameObject("Icon");
        iconObject.transform.SetParent(button.transform, false);
        var iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.sizeDelta = new Vector2(28f, 28f);
        iconRect.anchoredPosition = new Vector2(6f, 0f);
        var image = iconObject.AddComponent<Image>();
        image.sprite = iconSprite;
        image.preserveAspect = true;
        return button;
    }

    private void SetButtonVisual(Button button, Color background, Color foreground)
    {
        if (button == null)
        {
            return;
        }

        var image = button.GetComponent<Image>();
        if (image != null)
        {
            image.color = background;
        }

        var label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.color = foreground;
        }
    }
}
