using UnityEngine;

/// <summary>
/// Workstation with improved visual feedback.
/// Screen color transitions smoothly and shows different colors for different states.
/// </summary>
public sealed class PrototypeWorkstation : MonoBehaviour
{
    [SerializeField] private Transform approachPoint;
    [SerializeField] private Transform workPoint;
    [SerializeField] private Renderer screenRenderer;
    [SerializeField] private PrototypeEmployeeAgent assignedAgent;

    [Header("Visual Feedback")]
    [SerializeField] private Renderer[] additionalScreens;
    [SerializeField] private Light screenGlow;
    [SerializeField] private float glowIntensity = 0.5f;

    public Transform ApproachPoint => approachPoint;
    public Transform WorkPoint => workPoint;

    public PrototypeEmployeeAgent AssignedAgent
    {
        get => assignedAgent;
        set => assignedAgent = value;
    }

    private Color currentColor;
    private Color targetColor;
    private bool isActive;

    private void Awake()
    {
        targetColor = GameDesignConstants.DeptEngineering;
        currentColor = targetColor;
        ApplyScreenColor(targetColor);
    }

    private void Update()
    {
        // Smooth color transition
        if (currentColor != targetColor)
        {
            currentColor = Color.Lerp(currentColor, targetColor, 6f * Time.deltaTime);
            ApplyScreenColor(currentColor);
        }

        // Subtle screen flicker when active
        if (isActive)
        {
            var flicker = 1f + Mathf.Sin(Time.time * 30f) * 0.02f;
            ApplyScreenBrightness(flicker);
        }
    }

    private void OnMouseDown()
    {
        if (assignedAgent != null)
        {
            assignedAgent.StartWork();
        }
    }

    public void SetActive(bool active)
    {
        isActive = active;

        if (active)
        {
            targetColor = GameDesignConstants.EmployeeWorking;
            SetGlow(true);
        }
        else
        {
            targetColor = GameDesignConstants.DeptEngineering;
            SetGlow(false);
        }
    }

    /// <summary>Set screen to a specific color (for special events).</summary>
    public void SetScreenColor(Color color)
    {
        targetColor = color;
    }

    /// <summary>Flash screen red briefly (for errors/incidents).</summary>
    public void FlashError()
    {
        targetColor = GameDesignConstants.StatusDanger;
        Invoke(nameof(ResetColor), 0.5f);
    }

    private void ResetColor()
    {
        targetColor = isActive
            ? GameDesignConstants.EmployeeWorking
            : GameDesignConstants.DeptEngineering;
    }

    private void ApplyScreenColor(Color color)
    {
        if (screenRenderer != null)
        {
            screenRenderer.material.color = color;
            screenRenderer.material.SetColor("_EmissionColor", color * 0.5f);
        }

        if (additionalScreens == null) return;

        foreach (var screen in additionalScreens)
        {
            if (screen != null)
            {
                screen.material.color = color;
                screen.material.SetColor("_EmissionColor", color * 0.5f);
            }
        }
    }

    private void ApplyScreenBrightness(float multiplier)
    {
        if (screenRenderer == null) return;

        var baseColor = targetColor * multiplier;
        screenRenderer.material.SetColor("_EmissionColor", baseColor * 0.5f);
    }

    private void SetGlow(bool active)
    {
        if (screenGlow == null) return;

        screenGlow.enabled = active;
        if (active)
        {
            screenGlow.color = targetColor;
            screenGlow.intensity = glowIntensity;
        }
    }
}
