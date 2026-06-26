using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Defines a rival AI company in the game. Implements attributes, capabilities, and state tracking.
/// </summary>
[System.Serializable]
public sealed class CompetitorCompany
{
    // --- Identification / Definition ---
    public string Id { get; set; }
    public string Name { get; set; }
    public string Handle { get; set; }
    public Color BrandColor { get; set; }
    public string Personality { get; set; } // Archetype: e.g., "Frontier", "VerticalB2B", "Marketplace", "Infrastructure"
    public int StrengthTier { get; set; } // Resumed UI Tier (1 to 5)
    public List<string> Strengths { get; set; } = new List<string>();
    public List<string> Weaknesses { get; set; } = new List<string>();

    // --- Runtime Financials ---
    public float Capital { get; set; }
    public float Revenue { get; set; }
    public float RunwayMonths { get; set; }

    // --- Core Capabilities ---
    public float Research { get; set; }
    public float Compute { get; set; }
    public float DataAccess { get; set; }
    public float Operations { get; set; }
    public float Distribution { get; set; }
    public float BrandTrust { get; set; }
    public float SafetyMaturity { get; set; }

    // --- Market Capabilities (0 to 100) ---
    public float AutomationCapability { get; set; } // Automation & Assistant market
    public float EnterpriseCapability { get; set; } // Enterprise operations market
    public float InfrastructureCapability { get; set; } // Cloud & Compute market

    // --- State/Cooldowns ---
    public int CooldownMonths { get; set; }
    public List<string> RecentIncidents { get; set; } = new List<string>();

    public CompetitorCompany(string id, string name, string handle, Color color, string personality, int strength)
    {
        Id = id;
        Name = name;
        Handle = handle;
        BrandColor = color;
        Personality = personality;
        StrengthTier = strength;

        // Default starting state
        Capital = strength * 50000f;
        Revenue = strength * 10000f;
        RunwayMonths = 12f;

        Research = strength * 15f;
        Compute = strength * 15f;
        DataAccess = strength * 15f;
        Operations = strength * 15f;
        Distribution = strength * 15f;
        BrandTrust = 75f;
        SafetyMaturity = strength * 10f;

        // Initialize market capabilities
        AutomationCapability = strength * 12f;
        EnterpriseCapability = strength * 12f;
        InfrastructureCapability = strength * 12f;
    }
}
