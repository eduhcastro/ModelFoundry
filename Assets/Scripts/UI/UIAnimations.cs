using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Reusable UI animation helpers. Attach to any GameObject with a Canvas.
/// Uses coroutines — no external tween library needed.
/// All animations use unscaled time so they work while paused.
/// </summary>
public static class UIAnimations
{
    // ── Fade ─────────────────────────────────────────────────────────

    public static Coroutine FadeCanvasGroup(MonoBehaviour host, CanvasGroup group,
        float from, float to, float duration, System.Action onComplete = null)
    {
        return host.StartCoroutine(FadeRoutine(group, from, to, duration, onComplete));
    }

    private static IEnumerator FadeRoutine(CanvasGroup group, float from, float to,
        float duration, System.Action onComplete)
    {
        var elapsed = 0f;
        group.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
            group.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }

        group.alpha = to;
        group.interactable = to > 0.5f;
        group.blocksRaycasts = to > 0.5f;
        onComplete?.Invoke();
    }

    // ── Scale ────────────────────────────────────────────────────────

    public static Coroutine ScaleTransform(MonoBehaviour host, Transform target,
        Vector3 from, Vector3 to, float duration, System.Action onComplete = null)
    {
        return host.StartCoroutine(ScaleRoutine(target, from, to, duration, onComplete));
    }

    private static IEnumerator ScaleRoutine(Transform target, Vector3 from, Vector3 to,
        float duration, System.Action onComplete)
    {
        var elapsed = 0f;
        target.localScale = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = EaseOutBack(Mathf.Clamp01(elapsed / duration));
            target.localScale = Vector3.LerpUnclamped(from, to, t);
            yield return null;
        }

        target.localScale = to;
        onComplete?.Invoke();
    }

    // ── Slide ────────────────────────────────────────────────────────

    public static Coroutine SlideRectTransform(MonoBehaviour host, RectTransform target,
        Vector2 from, Vector2 to, float duration, System.Action onComplete = null)
    {
        return host.StartCoroutine(SlideRoutine(target, from, to, duration, onComplete));
    }

    private static IEnumerator SlideRoutine(RectTransform target, Vector2 from, Vector2 to,
        float duration, System.Action onComplete)
    {
        var elapsed = 0f;
        target.anchoredPosition = from;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
            target.anchoredPosition = Vector2.Lerp(from, to, t);
            yield return null;
        }

        target.anchoredPosition = to;
        onComplete?.Invoke();
    }

    // ── Pulse (continuous) ───────────────────────────────────────────

    public static Coroutine PulseScale(MonoBehaviour host, Transform target,
        float minScale, float maxScale, float speed)
    {
        return host.StartCoroutine(PulseRoutine(target, minScale, maxScale, speed));
    }

    private static IEnumerator PulseRoutine(Transform target, float minScale, float maxScale, float speed)
    {
        while (true)
        {
            var s = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f);
            target.localScale = new Vector3(s, s, s);
            yield return null;
        }
    }

    // ── Color lerp ───────────────────────────────────────────────────

    public static Coroutine LerpImageColor(MonoBehaviour host, Image image,
        Color from, Color to, float duration)
    {
        return host.StartCoroutine(ColorRoutine(image, from, to, duration));
    }

    private static IEnumerator ColorRoutine(Image image, Color from, Color to, float duration)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            image.color = Color.Lerp(from, to, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        image.color = to;
    }

    // ── Value counter (animated number) ──────────────────────────────

    public static Coroutine CountValue(MonoBehaviour host, float from, float to,
        float duration, System.Action<float> onUpdate)
    {
        return host.StartCoroutine(CountRoutine(from, to, duration, onUpdate));
    }

    private static IEnumerator CountRoutine(float from, float to, float duration,
        System.Action<float> onUpdate)
    {
        var elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var t = EaseOutCubic(Mathf.Clamp01(elapsed / duration));
            onUpdate?.Invoke(Mathf.Lerp(from, to, t));
            yield return null;
        }

        onUpdate?.Invoke(to);
    }

    // ── Easing functions ─────────────────────────────────────────────

    private static float EaseOutCubic(float t)
    {
        return 1f - Mathf.Pow(1f - t, 3f);
    }

    private static float EaseOutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    public static float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}
