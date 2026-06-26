using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Animated resource bar that shows an icon, label, numeric value, and a fill bar.
/// The value animates smoothly on change and the bar color shifts based on health.
/// </summary>
public sealed class ResourceBar : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI labelText;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private Image fillImage;
    [SerializeField] private Image backgroundImage;

    [Header("Configuration")]
    [SerializeField] private string label = "Cash";
    [SerializeField] private Color barColor = GameDesignConstants.ResourceCash;
    [SerializeField] private string prefix = "$";
    [SerializeField] private string suffix = "";
    [SerializeField] private float maxValue = 100000f;
    [SerializeField] private bool useHealthGradient;
    [SerializeField] private float animationSpeed = 5f;

    private float currentDisplayValue;
    private float targetValue;
    private float currentFill;

    private void Awake()
    {
        if (labelText != null)
        {
            labelText.text = label;
            labelText.color = GameDesignConstants.TextSecondary;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = GameDesignConstants.ResourceBarBg;
        }

        if (fillImage != null)
        {
            fillImage.color = barColor;
        }
    }

    private void Update()
    {
        if (Mathf.Approximately(currentDisplayValue, targetValue) &&
            Mathf.Approximately(currentFill, GetTargetFill()))
        {
            return;
        }

        currentDisplayValue = Mathf.Lerp(currentDisplayValue, targetValue, animationSpeed * Time.unscaledDeltaTime);
        currentFill = Mathf.Lerp(currentFill, GetTargetFill(), animationSpeed * Time.unscaledDeltaTime);

        // Snap if close enough
        if (Mathf.Abs(currentDisplayValue - targetValue) < 0.5f)
        {
            currentDisplayValue = targetValue;
        }

        UpdateDisplay();
    }

    /// <summary>
    /// Set the value to display. The bar will animate to this value.
    /// </summary>
    public void SetValue(float value)
    {
        targetValue = value;
    }

    /// <summary>
    /// Set value without animation (for initialization).
    /// </summary>
    public void SetValueImmediate(float value)
    {
        targetValue = value;
        currentDisplayValue = value;
        currentFill = GetTargetFill();
        UpdateDisplay();
    }

    /// <summary>
    /// Update maximum value (for scaling the fill bar).
    /// </summary>
    public void SetMaxValue(float max)
    {
        maxValue = Mathf.Max(1f, max);
    }

    /// <summary>
    /// Change bar color at runtime.
    /// </summary>
    public void SetBarColor(Color color)
    {
        barColor = color;
        if (fillImage != null && !useHealthGradient)
        {
            fillImage.color = barColor;
        }
    }

    private float GetTargetFill()
    {
        return Mathf.Clamp01(targetValue / maxValue);
    }

    private void UpdateDisplay()
    {
        if (valueText != null)
        {
            if (currentDisplayValue >= 1000000f)
            {
                valueText.text = $"{prefix}{currentDisplayValue / 1000000f:F1}M{suffix}";
            }
            else if (currentDisplayValue >= 1000f)
            {
                valueText.text = $"{prefix}{currentDisplayValue / 1000f:F1}K{suffix}";
            }
            else
            {
                valueText.text = $"{prefix}{currentDisplayValue:F0}{suffix}";
            }

            valueText.color = GameDesignConstants.TextPrimary;
        }

        if (fillImage != null)
        {
            fillImage.fillAmount = currentFill;

            if (useHealthGradient)
            {
                fillImage.color = GameDesignConstants.HealthGradient(currentFill);
            }
        }
    }
}
