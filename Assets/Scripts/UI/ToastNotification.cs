using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Stackable toast notification system. Notifications slide in from the bottom-right,
/// display for a few seconds, then fade out. Supports category colors and icons.
/// </summary>
public sealed class ToastNotification : MonoBehaviour
{
    public enum Category
    {
        Info,
        Success,
        Warning,
        Danger
    }

    [Header("Pool")]
    [SerializeField] private int maxVisible = 5;
    [SerializeField] private float displayDuration = 4f;
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.5f;
    [SerializeField] private float slideDistance = 60f;

    [Header("Template")]
    [SerializeField] private GameObject toastTemplate;
    [SerializeField] private Transform toastContainer;

    private readonly Queue<ToastData> pendingQueue = new Queue<ToastData>();
    private readonly List<ActiveToast> activeToasts = new List<ActiveToast>();

    private struct ToastData
    {
        public string message;
        public Category category;
    }

    private class ActiveToast
    {
        public GameObject gameObject;
        public CanvasGroup canvasGroup;
        public RectTransform rectTransform;
        public float timer;
        public bool isFadingOut;
    }

    private void Awake()
    {
        if (toastTemplate != null)
        {
            toastTemplate.SetActive(false);
        }
    }

    private void Start()
    {
        // Auto-listen to GameManager notifications
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnNotification += msg => Show(msg, Category.Info);
        }
    }

    private void Update()
    {
        // Process active toasts
        for (var i = activeToasts.Count - 1; i >= 0; i--)
        {
            var toast = activeToasts[i];
            toast.timer -= Time.unscaledDeltaTime;

            if (toast.timer <= 0f && !toast.isFadingOut)
            {
                toast.isFadingOut = true;
                StartCoroutine(FadeOutAndRemove(toast, i));
            }
        }

        // Show pending toasts if space available
        while (pendingQueue.Count > 0 && activeToasts.Count < maxVisible)
        {
            var data = pendingQueue.Dequeue();
            CreateToast(data);
        }
    }

    /// <summary>Show a toast notification with a message and category.</summary>
    public void Show(string message, Category category = Category.Info)
    {
        var data = new ToastData { message = message, category = category };

        if (activeToasts.Count >= maxVisible)
        {
            pendingQueue.Enqueue(data);
            return;
        }

        CreateToast(data);
    }

    /// <summary>Static helper to show toast from anywhere.</summary>
    public static void ShowGlobal(string message, Category category = Category.Info)
    {
        var instance = FindAnyObjectByType<ToastNotification>();
        if (instance != null)
        {
            instance.Show(message, category);
        }
    }

    private void CreateToast(ToastData data)
    {
        if (toastTemplate == null || toastContainer == null)
        {
            return;
        }

        var go = Instantiate(toastTemplate, toastContainer);
        go.SetActive(true);

        var rt = go.GetComponent<RectTransform>();
        var cg = go.GetComponent<CanvasGroup>();
        if (cg == null) cg = go.AddComponent<CanvasGroup>();

        // Find text and accent bar in template
        var text = go.GetComponentInChildren<TextMeshProUGUI>();
        if (text != null)
        {
            text.text = data.message;
            text.color = GameDesignConstants.TextPrimary;
        }

        // Set accent color based on category
        var accentBar = go.transform.Find("AccentBar");
        if (accentBar != null)
        {
            var img = accentBar.GetComponent<Image>();
            if (img != null)
            {
                img.color = GetCategoryColor(data.category);
            }
        }

        // Set background
        var bg = go.GetComponent<Image>();
        if (bg != null)
        {
            bg.color = GameDesignConstants.SurfaceCard;
        }

        var toast = new ActiveToast
        {
            gameObject = go,
            canvasGroup = cg,
            rectTransform = rt,
            timer = displayDuration,
            isFadingOut = false
        };

        activeToasts.Add(toast);
        StartCoroutine(AnimateIn(toast));
    }

    private IEnumerator AnimateIn(ActiveToast toast)
    {
        if (toast == null || toast.rectTransform == null || toast.canvasGroup == null)
            yield break;

        var startPos = toast.rectTransform.anchoredPosition + new Vector2(slideDistance, 0f);
        var endPos = toast.rectTransform.anchoredPosition;
        var elapsed = 0f;

        toast.canvasGroup.alpha = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / fadeInDuration);
            t = 1f - Mathf.Pow(1f - t, 3f); // ease out cubic

            if (toast == null || toast.gameObject == null || toast.canvasGroup == null || toast.rectTransform == null)
                yield break;

            toast.canvasGroup.alpha = t;
            toast.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (toast == null || toast.gameObject == null || toast.canvasGroup == null || toast.rectTransform == null)
            yield break;

        toast.canvasGroup.alpha = 1f;
        toast.rectTransform.anchoredPosition = endPos;
    }

    private IEnumerator FadeOutAndRemove(ActiveToast toast, int index)
    {
        if (toast == null || toast.rectTransform == null || toast.canvasGroup == null)
            yield break;

        var elapsed = 0f;
        var startPos = toast.rectTransform.anchoredPosition;
        var endPos = startPos + new Vector2(slideDistance * 0.5f, 0f);

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = Mathf.Clamp01(elapsed / fadeOutDuration);

            if (toast == null || toast.gameObject == null || toast.canvasGroup == null || toast.rectTransform == null)
                yield break;

            toast.canvasGroup.alpha = 1f - t;
            toast.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }

        if (toast != null)
        {
            activeToasts.Remove(toast);
            if (toast.gameObject != null)
            {
                Destroy(toast.gameObject);
            }
        }
    }

    private static Color GetCategoryColor(Category cat)
    {
        switch (cat)
        {
            case Category.Success: return GameDesignConstants.StatusSuccess;
            case Category.Warning: return GameDesignConstants.StatusWarning;
            case Category.Danger:  return GameDesignConstants.StatusDanger;
            default:               return GameDesignConstants.StatusInfo;
        }
    }
}
