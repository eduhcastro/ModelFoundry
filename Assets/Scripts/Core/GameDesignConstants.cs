using UnityEngine;

/// <summary>
/// Central design system for Model Foundry.
/// Every visual constant lives here so the entire game stays consistent.
/// Change a color once, it updates everywhere.
/// </summary>
public static class GameDesignConstants
{
    // ── Brand ────────────────────────────────────────────────────────
    public static readonly Color BrandPrimary   = Hex("#8B5CF6"); // vibrant electric violet
    public static readonly Color BrandSecondary = Hex("#06B6D4"); // neon cyan
    public static readonly Color BrandAccent    = Hex("#F59E0B"); // warm golden amber
    public static readonly Color BrandGlow      = Hex("#C084FC"); // soft violet glow

    // ── Surfaces ─────────────────────────────────────────────────────
    public static readonly Color SurfaceDarkest = Hex("#05050A"); // deepest bg (space slate)
    public static readonly Color SurfaceDark    = Hex("#090D1A"); // main bg (sleek navy slate)
    public static readonly Color SurfaceMid     = Hex("#111827"); // panels (zinc dark)
    public static readonly Color SurfaceLight   = Hex("#1E293B"); // elevated cards
    public static readonly Color SurfaceCard    = new Color(0.06f, 0.08f, 0.15f, 0.96f);
    public static readonly Color SurfaceGlass   = new Color(0.06f, 0.08f, 0.15f, 0.78f);
    public static readonly Color SurfaceOverlay = new Color(0.02f, 0.03f, 0.06f, 0.72f);

    // ── Text ─────────────────────────────────────────────────────────
    public static readonly Color TextPrimary    = Hex("#F8FAFC"); // crisp slate-50 white
    public static readonly Color TextSecondary  = Hex("#94A3B8"); // silver-gray slate-400
    public static readonly Color TextMuted      = Hex("#64748B"); // muted slate-500
    public static readonly Color TextOnAccent   = Hex("#090D1A"); // dark text on light accents

    // ── Status ───────────────────────────────────────────────────────
    public static readonly Color StatusSuccess  = Hex("#10B981"); // vibrant emerald green
    public static readonly Color StatusWarning  = Hex("#FBBF24"); // radiant amber yellow
    public static readonly Color StatusDanger   = Hex("#EF4444"); // electric red
    public static readonly Color StatusInfo     = Hex("#3B82F6"); // tech blue
    public static readonly Color StatusCritical = Hex("#F43F5E"); // rose red

    // ── Department identity ──────────────────────────────────────────
    public static readonly Color DeptEngineering = Hex("#0984E3");
    public static readonly Color DeptResearch    = Hex("#6C5CE7");
    public static readonly Color DeptData        = Hex("#FDCB6E");
    public static readonly Color DeptSafety      = Hex("#D63031");
    public static readonly Color DeptSales       = Hex("#00B894");
    public static readonly Color DeptInfra       = Hex("#636E72");
    public static readonly Color DeptPR          = Hex("#E17055");
    public static readonly Color DeptBoard       = Hex("#2D3436");

    // ── Resource bar colors ──────────────────────────────────────────
    public static readonly Color ResourceCash       = Hex("#00B894");
    public static readonly Color ResourceReputation = Hex("#A29BFE");
    public static readonly Color ResourceQuality    = Hex("#74B9FF");
    public static readonly Color ResourceTeam       = Hex("#FDCB6E");
    public static readonly Color ResourceBarBg      = new Color(0.1f, 0.1f, 0.15f, 0.7f);

    // ── Employee status ──────────────────────────────────────────────
    public static readonly Color EmployeeIdle        = Hex("#636E72");
    public static readonly Color EmployeeWalking     = Hex("#74B9FF");
    public static readonly Color EmployeeWorking     = Hex("#00B894");
    public static readonly Color EmployeeCelebrating = Hex("#FDCB6E");
    public static readonly Color EmployeePanicking   = Hex("#E17055");

    // ── Button variants ──────────────────────────────────────────────
    public static readonly Color ButtonPrimary       = Hex("#8B5CF6"); // Electric violet
    public static readonly Color ButtonPrimaryHover   = Hex("#A78BFA"); // Light violet on hover
    public static readonly Color ButtonPrimaryPress   = Hex("#7C3AED"); // Deep violet on press
    public static readonly Color ButtonSecondary      = new Color(1f, 1f, 1f, 0.06f); // Glass border/surface
    public static readonly Color ButtonSecondaryHover = new Color(1f, 1f, 1f, 0.12f);
    public static readonly Color ButtonDanger         = Hex("#EF4444"); // Electric red
    public static readonly Color ButtonDangerHover    = Hex("#F87171"); // Lighter red
    public static readonly Color ButtonDisabled       = new Color(0.15f, 0.17f, 0.25f, 0.4f);

    // ── Animation timing (seconds) ──────────────────────────────────
    public const float AnimFast     = 0.12f;
    public const float AnimNormal   = 0.25f;
    public const float AnimSlow     = 0.45f;
    public const float AnimVerySlow = 0.8f;
    public const float AnimEntrance = 0.6f;

    // ── Button feel ──────────────────────────────────────────────────
    public const float ButtonHoverScale  = 1.04f;
    public const float ButtonPressScale  = 0.96f;
    public const float ButtonHoverAlpha  = 0.15f;

    // ── Typography (for runtime creation) ────────────────────────────
    public const float FontTitle    = 42f;
    public const float FontHeader   = 28f;
    public const float FontSubhead  = 22f;
    public const float FontBody     = 16f;
    public const float FontCaption  = 13f;
    public const float FontSmall    = 11f;
    public const float FontButton   = 18f;

    // ── Layout ───────────────────────────────────────────────────────
    public const float PanelPadding    = 16f;
    public const float CardCornerRadius = 8f;
    public const float ElementSpacing   = 8f;

    // ── Quality rating thresholds ────────────────────────────────────
    public const float QualityPoor     = 25f;
    public const float QualityDecent   = 50f;
    public const float QualityGood     = 70f;
    public const float QualityGreat    = 85f;
    public const float QualityExcellent = 95f;

    /// <summary>Returns a color from health ratio: green (1.0) → yellow (0.5) → red (0.0).</summary>
    public static Color HealthGradient(float t)
    {
        t = Mathf.Clamp01(t);
        if (t > 0.5f)
            return Color.Lerp(StatusWarning, StatusSuccess, (t - 0.5f) * 2f);
        return Color.Lerp(StatusDanger, StatusWarning, t * 2f);
    }

    /// <summary>Returns star-rating color: 1★ red → 3★ yellow → 5★ green.</summary>
    public static Color QualityColor(float quality)
    {
        if (quality >= QualityGreat) return StatusSuccess;
        if (quality >= QualityGood)  return StatusInfo;
        if (quality >= QualityDecent) return StatusWarning;
        return StatusDanger;
    }

    private static Color Hex(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }
}
