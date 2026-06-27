using UnityEngine;

/// <summary>
/// Central design system for Model Foundry.
/// Every visual constant lives here so the entire game stays consistent.
/// Change a color once, it updates everywhere.
/// </summary>
public static class GameDesignConstants
{
    // ── Brand ────────────────────────────────────────────────────────
    public static readonly Color BrandPrimary   = Hex("#0D9488"); // teal
    public static readonly Color BrandSecondary = Hex("#0EA5E9"); // cyan
    public static readonly Color BrandAccent    = Hex("#F59E0B"); // warm golden amber
    public static readonly Color BrandGlow      = Hex("#93C5FD"); // soft blue glow

    // ── Surfaces ─────────────────────────────────────────────────────
    public static readonly Color SurfaceDarkest = Hex("#EAE7E2"); // light beige background
    public static readonly Color SurfaceDark    = Hex("#1C2333"); // top bar (slate dark)
    public static readonly Color SurfaceMid     = Hex("#FFFFFF"); // panels (white)
    public static readonly Color SurfaceLight   = Hex("#F3F4F6"); // elevated cards (light grey)
    public static readonly Color SurfaceCard    = new Color(1f, 1f, 1f, 0.96f);
    public static readonly Color SurfaceGlass   = new Color(1f, 1f, 1f, 0.78f);
    public static readonly Color SurfaceOverlay = new Color(0.09f, 0.13f, 0.22f, 0.6f);

    // ── Text ─────────────────────────────────────────────────────────
    public static readonly Color TextPrimary    = Hex("#1C2333"); // slate dark text
    public static readonly Color TextSecondary  = Hex("#4B5563"); // slate gray text
    public static readonly Color TextMuted      = Hex("#9CA3AF"); // light gray text
    public static readonly Color TextOnAccent   = Hex("#1C2333"); // dark text on light accents

    // ── Status ───────────────────────────────────────────────────────
    public static readonly Color StatusSuccess  = Hex("#10B981"); // vibrant emerald green
    public static readonly Color StatusWarning  = Hex("#FBBF24"); // radiant amber yellow
    public static readonly Color StatusDanger   = Hex("#EF4444"); // electric red
    public static readonly Color StatusInfo     = Hex("#3B82F6"); // tech blue
    public static readonly Color StatusCritical = Hex("#F43F5E"); // rose red

    // ── Department identity ──────────────────────────────────────────
    public static readonly Color DeptEngineering = Hex("#0EA5E9");
    public static readonly Color DeptResearch    = Hex("#8B5CF6");
    public static readonly Color DeptData        = Hex("#F59E0B");
    public static readonly Color DeptSafety      = Hex("#EF4444");
    public static readonly Color DeptSales       = Hex("#10B981");
    public static readonly Color DeptInfra       = Hex("#6B7280");
    public static readonly Color DeptPR          = Hex("#F97316");
    public static readonly Color DeptBoard       = Hex("#1C2333");

    // ── Resource bar colors ──────────────────────────────────────────
    public static readonly Color ResourceCash       = Hex("#10B981");
    public static readonly Color ResourceReputation = Hex("#F59E0B");
    public static readonly Color ResourceQuality    = Hex("#0D9488");
    public static readonly Color ResourceTeam       = Hex("#3B82F6");
    public static readonly Color ResourceBarBg      = Hex("#E5E7EB");

    // ── Employee status ──────────────────────────────────────────────
    public static readonly Color EmployeeIdle        = Hex("#6B7280");
    public static readonly Color EmployeeWalking     = Hex("#3B82F6");
    public static readonly Color EmployeeWorking     = Hex("#10B981");
    public static readonly Color EmployeeCelebrating = Hex("#F59E0B");
    public static readonly Color EmployeePanicking   = Hex("#EF4444");

    // ── Button variants ──────────────────────────────────────────────
    public static readonly Color ButtonPrimary       = Hex("#0D9488"); // Teal
    public static readonly Color ButtonPrimaryHover   = Hex("#14B8A6"); // Light teal
    public static readonly Color ButtonPrimaryPress   = Hex("#0F766E"); // Dark teal
    public static readonly Color ButtonSecondary      = new Color(1f, 1f, 1f, 0.8f); // White with alpha
    public static readonly Color ButtonSecondaryHover = new Color(0.9f, 0.95f, 0.95f, 1f);
    public static readonly Color ButtonDanger         = Hex("#EF4444");
    public static readonly Color ButtonDangerHover    = Hex("#F87171");
    public static readonly Color ButtonDisabled       = Hex("#E5E7EB");

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
