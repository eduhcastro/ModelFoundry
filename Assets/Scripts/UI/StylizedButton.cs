using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// Premium-feel button component. Add alongside a standard Unity Button.
/// Provides animated hover/press/disabled states with scale + color shifts.
/// Supports Primary, Secondary, and Danger visual variants.
/// </summary>
[RequireComponent(typeof(Button))]
public sealed class StylizedButton : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    public enum ButtonVariant
    {
        Primary,
        Secondary,
        Danger,
        Accent
    }

    [Header("Style")]
    [SerializeField] private ButtonVariant variant = ButtonVariant.Primary;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image glowImage;

    [Header("Override Colors (leave black to use defaults)")]
    [SerializeField] private Color overrideNormal = Color.clear;
    [SerializeField] private Color overrideHover  = Color.clear;

    private Button button;
    private Vector3 originalScale;
    private Color normalColor;
    private Color hoverColor;
    private Color pressColor;
    private Color disabledColor;
    private bool isHovered;

    private void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
        ApplyVariant();
    }

    private void OnEnable()
    {
        ApplyVariant();
        UpdateVisual(false);
    }

    private void Update()
    {
        if (button != null && !button.interactable)
        {
            SetBackgroundColor(disabledColor);
            return;
        }
    }

    public void SetVariant(ButtonVariant newVariant)
    {
        variant = newVariant;
        ApplyVariant();
    }

    // ── Pointer events ───────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        isHovered = true;
        transform.localScale = originalScale * GameDesignConstants.ButtonHoverScale;
        SetBackgroundColor(hoverColor);
        SetGlowAlpha(0.3f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        transform.localScale = originalScale;
        SetBackgroundColor(normalColor);
        SetGlowAlpha(0f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        transform.localScale = originalScale * GameDesignConstants.ButtonPressScale;
        SetBackgroundColor(pressColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button != null && !button.interactable) return;

        transform.localScale = isHovered
            ? originalScale * GameDesignConstants.ButtonHoverScale
            : originalScale;
        SetBackgroundColor(isHovered ? hoverColor : normalColor);
    }

    // ── Internals ────────────────────────────────────────────────────

    private void ApplyVariant()
    {
        if (overrideNormal != Color.clear)
        {
            normalColor   = overrideNormal;
            hoverColor    = overrideHover != Color.clear ? overrideHover : Brighten(normalColor, 0.12f);
            pressColor    = Darken(normalColor, 0.1f);
            disabledColor = GameDesignConstants.ButtonDisabled;
            UpdateVisual(false);
            return;
        }

        switch (variant)
        {
            case ButtonVariant.Primary:
                normalColor   = GameDesignConstants.ButtonPrimary;
                hoverColor    = GameDesignConstants.ButtonPrimaryHover;
                pressColor    = GameDesignConstants.ButtonPrimaryPress;
                break;

            case ButtonVariant.Secondary:
                normalColor   = GameDesignConstants.ButtonSecondary;
                hoverColor    = GameDesignConstants.ButtonSecondaryHover;
                pressColor    = new Color(1f, 1f, 1f, 0.06f);
                break;

            case ButtonVariant.Danger:
                normalColor   = GameDesignConstants.ButtonDanger;
                hoverColor    = GameDesignConstants.ButtonDangerHover;
                pressColor    = Darken(GameDesignConstants.ButtonDanger, 0.15f);
                break;

            case ButtonVariant.Accent:
                normalColor   = GameDesignConstants.BrandAccent;
                hoverColor    = Brighten(GameDesignConstants.BrandAccent, 0.1f);
                pressColor    = Darken(GameDesignConstants.BrandAccent, 0.1f);
                break;
        }

        disabledColor = GameDesignConstants.ButtonDisabled;
        UpdateVisual(false);
    }

    private void UpdateVisual(bool animated)
    {
        SetBackgroundColor(normalColor);
        SetGlowAlpha(0f);
    }

    private void SetBackgroundColor(Color color)
    {
        if (backgroundImage != null)
        {
            backgroundImage.color = color;
        }
    }

    private void SetGlowAlpha(float alpha)
    {
        if (glowImage == null) return;

        var c = glowImage.color;
        c.a = alpha;
        glowImage.color = c;
    }

    private static Color Brighten(Color c, float amount)
    {
        return new Color(
            Mathf.Clamp01(c.r + amount),
            Mathf.Clamp01(c.g + amount),
            Mathf.Clamp01(c.b + amount),
            c.a
        );
    }

    private static Color Darken(Color c, float amount)
    {
        return new Color(
            Mathf.Clamp01(c.r - amount),
            Mathf.Clamp01(c.g - amount),
            Mathf.Clamp01(c.b - amount),
            c.a
        );
    }
}
