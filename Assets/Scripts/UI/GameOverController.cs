using UnityEngine;
using UnityEngine.UI;
using TMPro;

public sealed class GameOverController : MonoBehaviour
{
    public static GameOverController Instance { get; private set; }

    [Header("UI Panel Reference")]
    [SerializeField] private CanvasGroup panelGroup;
    [SerializeField] private Button loadSaveButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI gameOverText;

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
        if (loadSaveButton != null)
        {
            loadSaveButton.onClick.AddListener(LoadLastSave);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(QuitToMainMenu);
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += CheckGameOverState;
        }

        HidePanel();
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= CheckGameOverState;
        }
    }

    private void CheckGameOverState()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameOver)
        {
            ShowPanel("The board lost all confidence in your leadership and removed you as CEO.");
        }
        else
        {
            HidePanel();
        }
    }

    public void ShowPanel(string reason)
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 1f;
            panelGroup.blocksRaycasts = true;
            panelGroup.interactable = true;
        }

        if (gameOverText != null)
        {
            gameOverText.text = reason;
        }

        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(TimeController.Speed.Paused);
        }
    }

    public void HidePanel()
    {
        if (panelGroup != null)
        {
            panelGroup.alpha = 0f;
            panelGroup.blocksRaycasts = false;
            panelGroup.interactable = false;
        }
    }

    private void LoadLastSave()
    {
        var sl = FindFirstObjectByType<SaveLoadManager>();
        if (sl != null)
        {
            sl.LoadGame();
            HidePanel();
        }
        else
        {
            ToastNotification.ShowGlobal("Failed to load last save: SaveLoadManager not found.", ToastNotification.Category.Danger);
        }
    }

    private void QuitToMainMenu()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.SetSpeed(TimeController.Speed.Normal);
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
