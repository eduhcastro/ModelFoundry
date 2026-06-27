using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class TeamSimulationManager : MonoBehaviour
{
    public static TeamSimulationManager Instance { get; private set; }

    public enum TeamRole
    {
        ChiefExecutiveOfficer,
        ResearchStudy,
        Website,
        Prototype,
        Infrastructure,
        Arena,
        Rest
    }

    public enum StaffAttribute
    {
        Research,
        Engineering,
        Product,
        Design,
        Infrastructure,
        Safety,
        Communication,
        Focus,
        Leadership
    }

    [Serializable]
    public sealed class AttributeValue
    {
        public StaffAttribute Attribute;
        public float Value;
    }

    [Serializable]
    public sealed class StudyRecord
    {
        public string TrackId;
        public int CompletedCount;
    }

    [Serializable]
    public sealed class SkillPerk
    {
        public string Id;
        public string DisplayName;
        public string Description;
        public int Cost;
        public bool Unlocked;
    }

    [Serializable]
    public sealed class StudyProgram
    {
        public string Id;
        public string DisplayName;
        public string Description;
        public int DurationDays;
        public float CashCost;
        public StaffAttribute PrimaryAttribute;
        public StaffAttribute SecondaryAttribute;
        public float PrimaryBaseGain;
        public float SecondaryBaseGain;
    }

    [Serializable]
    public sealed class StaffProfile
    {
        public string Id;
        public string DisplayName;
        public string RoleTitle;
        public int Level;
        public float Salary;
        public float Burnout;
        public string Education;
        public string WorkHistory;
        public TeamRole Assignment;
        public int SkillPoints;
        public int EarnedSkillPoints;
        public string ActiveStudyId;
        public int StudyRemainingDays;
        public int StudyTotalDays;
        public List<AttributeValue> Attributes = new List<AttributeValue>();
        public List<StudyRecord> StudyHistory = new List<StudyRecord>();
        public List<SkillPerk> Perks = new List<SkillPerk>();
    }

    [SerializeField] private List<StaffProfile> staff = new List<StaffProfile>();
    [SerializeField] private List<StudyProgram> studyPrograms = new List<StudyProgram>();

    public IReadOnlyList<StaffProfile> Staff => staff;
    public IReadOnlyList<StudyProgram> StudyPrograms => studyPrograms;

    public event Action OnTeamChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        EnsureCatalogs();
        EnsureFounder();
    }

    private void OnEnable()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed += HandleDayPassed;
        }
    }

    private void OnDisable()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.OnDayPassed -= HandleDayPassed;
        }
    }

    public static TeamSimulationManager EnsureExists()
    {
        if (Instance != null)
        {
            return Instance;
        }

        var go = new GameObject("TeamSimulationManager");
        return go.AddComponent<TeamSimulationManager>();
    }

    public StaffProfile GetFounder()
    {
        EnsureFounder();
        return staff.Count > 0 ? staff[0] : null;
    }

    public StudyProgram GetStudyProgram(string studyId)
    {
        for (var i = 0; i < studyPrograms.Count; i++)
        {
            if (studyPrograms[i].Id == studyId)
            {
                return studyPrograms[i];
            }
        }

        return null;
    }

    public float PreviewPrimaryGain(StaffProfile profile, StudyProgram program)
    {
        if (profile == null || program == null)
        {
            return 0f;
        }

        return CalculateEffectiveGain(profile, program.Id, program.PrimaryAttribute, program.PrimaryBaseGain);
    }

    public float PreviewSecondaryGain(StaffProfile profile, StudyProgram program)
    {
        if (profile == null || program == null)
        {
            return 0f;
        }

        return CalculateEffectiveGain(profile, program.Id, program.SecondaryAttribute, program.SecondaryBaseGain);
    }

    public bool StartStudy(StaffProfile profile, string studyId)
    {
        var program = GetStudyProgram(studyId);
        if (profile == null || program == null)
        {
            return false;
        }

        if (!string.IsNullOrEmpty(profile.ActiveStudyId))
        {
            Notify($"{profile.DisplayName} is already studying.");
            return false;
        }

        var cost = GetStudyCost(profile, program);
        if (GameManager.Instance != null && !GameManager.Instance.SpendCash(cost))
        {
            Notify("Not enough cash to start this study track.");
            return false;
        }

        profile.ActiveStudyId = program.Id;
        profile.StudyTotalDays = program.DurationDays;
        profile.StudyRemainingDays = program.DurationDays;
        profile.Assignment = TeamRole.ResearchStudy;
        Notify($"{profile.DisplayName} started {program.DisplayName}.");
        OnTeamChanged?.Invoke();
        return true;
    }

    public void AssignRole(StaffProfile profile, TeamRole role)
    {
        if (profile == null)
        {
            return;
        }

        profile.Assignment = role;
        Notify($"{profile.DisplayName} assigned to {FormatRole(role)}.");
        OnTeamChanged?.Invoke();
    }

    public bool UnlockPerk(StaffProfile profile, string perkId)
    {
        if (profile == null)
        {
            return false;
        }

        var perk = profile.Perks.Find(item => item.Id == perkId);
        if (perk == null || perk.Unlocked)
        {
            return false;
        }

        if (profile.SkillPoints < perk.Cost)
        {
            Notify("Not enough skill points for this perk.");
            return false;
        }

        profile.SkillPoints -= perk.Cost;
        perk.Unlocked = true;
        Notify($"{profile.DisplayName} unlocked {perk.DisplayName}.");
        OnTeamChanged?.Invoke();
        return true;
    }

    public float GetAttribute(StaffProfile profile, StaffAttribute attribute)
    {
        if (profile == null)
        {
            return 0f;
        }

        var value = profile.Attributes.Find(item => item.Attribute == attribute);
        return value != null ? value.Value : 0f;
    }

    public float GetStudyCost(StaffProfile profile, StudyProgram program)
    {
        if (profile == null || program == null)
        {
            return 0f;
        }

        var multiplier = HasPerk(profile, "lean_reader") ? 0.9f : 1f;
        return Mathf.Round(program.CashCost * multiplier);
    }

    public int GetNextSkillPointCost(StaffProfile profile)
    {
        if (profile == null)
        {
            return 100;
        }

        return 100 + profile.EarnedSkillPoints * 75;
    }

    public static string FormatRole(TeamRole role)
    {
        switch (role)
        {
            case TeamRole.ChiefExecutiveOfficer: return "CEO";
            case TeamRole.ResearchStudy: return "Research Study";
            case TeamRole.Website: return "Website";
            case TeamRole.Prototype: return "Prototype";
            case TeamRole.Infrastructure: return "Infrastructure";
            case TeamRole.Arena: return "AI Arena";
            case TeamRole.Rest: return "Rest";
            default: return role.ToString();
        }
    }

    private void HandleDayPassed()
    {
        var changed = false;

        for (var i = 0; i < staff.Count; i++)
        {
            var profile = staff[i];
            if (string.IsNullOrEmpty(profile.ActiveStudyId))
            {
                continue;
            }

            profile.StudyRemainingDays = Mathf.Max(0, profile.StudyRemainingDays - 1);
            changed = true;

            if (profile.StudyRemainingDays == 0)
            {
                CompleteStudy(profile);
            }
        }

        if (changed)
        {
            OnTeamChanged?.Invoke();
        }
    }

    private void CompleteStudy(StaffProfile profile)
    {
        var program = GetStudyProgram(profile.ActiveStudyId);
        if (program == null)
        {
            profile.ActiveStudyId = string.Empty;
            return;
        }

        var primaryGain = CalculateEffectiveGain(profile, program.Id, program.PrimaryAttribute, program.PrimaryBaseGain);
        var secondaryGain = CalculateEffectiveGain(profile, program.Id, program.SecondaryAttribute, program.SecondaryBaseGain);
        AddAttribute(profile, program.PrimaryAttribute, primaryGain);
        AddAttribute(profile, program.SecondaryAttribute, secondaryGain);
        AddStudyRecord(profile, program.Id);
        AddSkillProgress(profile, Mathf.RoundToInt((primaryGain + secondaryGain) * 18f));
        ApplyCompanyLearning(program, primaryGain, secondaryGain);

        profile.ActiveStudyId = string.Empty;
        profile.StudyTotalDays = 0;
        profile.StudyRemainingDays = 0;
        Notify($"{profile.DisplayName} completed {program.DisplayName}.");
    }

    private float CalculateEffectiveGain(StaffProfile profile, string trackId, StaffAttribute attribute, float baseGain)
    {
        var current = GetAttribute(profile, attribute);
        var diminishing = Mathf.Pow(1f - Mathf.Clamp01(current / 100f), 2f);
        var repetition = 1f / (1f + GetStudyCount(profile, trackId) * 0.35f);
        var perkMultiplier = GetPerkStudyMultiplier(profile, attribute);
        return Mathf.Max(0.1f, baseGain * diminishing * repetition * perkMultiplier);
    }

    private float GetPerkStudyMultiplier(StaffProfile profile, StaffAttribute attribute)
    {
        var multiplier = 1f;

        if (HasPerk(profile, "deep_work") && (attribute == StaffAttribute.Research || attribute == StaffAttribute.Focus))
        {
            multiplier += 0.15f;
        }

        return multiplier;
    }

    private void AddSkillProgress(StaffProfile profile, int amount)
    {
        var progress = amount;

        while (progress >= GetNextSkillPointCost(profile))
        {
            progress -= GetNextSkillPointCost(profile);
            profile.SkillPoints++;
            profile.EarnedSkillPoints++;
        }
    }

    private void ApplyCompanyLearning(StudyProgram program, float primaryGain, float secondaryGain)
    {
        var simulation = StartupSimulationManager.EnsureExists();
        if (simulation == null)
        {
            return;
        }

        simulation.ApplyStudyLearning(program.PrimaryAttribute.ToString(), primaryGain, secondaryGain);
    }

    private void AddAttribute(StaffProfile profile, StaffAttribute attribute, float amount)
    {
        var value = profile.Attributes.Find(item => item.Attribute == attribute);
        if (value == null)
        {
            value = new AttributeValue { Attribute = attribute, Value = 0f };
            profile.Attributes.Add(value);
        }

        value.Value = Mathf.Clamp(value.Value + amount, 0f, 100f);
    }

    private void AddStudyRecord(StaffProfile profile, string trackId)
    {
        var record = profile.StudyHistory.Find(item => item.TrackId == trackId);
        if (record == null)
        {
            record = new StudyRecord { TrackId = trackId, CompletedCount = 0 };
            profile.StudyHistory.Add(record);
        }

        record.CompletedCount++;
    }

    private int GetStudyCount(StaffProfile profile, string trackId)
    {
        var record = profile.StudyHistory.Find(item => item.TrackId == trackId);
        return record != null ? record.CompletedCount : 0;
    }

    private bool HasPerk(StaffProfile profile, string perkId)
    {
        return profile != null && profile.Perks.Exists(item => item.Id == perkId && item.Unlocked);
    }

    private void EnsureCatalogs()
    {
        if (studyPrograms.Count == 0)
        {
            studyPrograms.Add(new StudyProgram
            {
                Id = "ai_papers",
                DisplayName = "AI Paper Reading",
                Description = "Read current model, agent and eval papers. Best early research literacy source.",
                DurationDays = 5,
                CashCost = 500f,
                PrimaryAttribute = StaffAttribute.Research,
                SecondaryAttribute = StaffAttribute.Focus,
                PrimaryBaseGain = 6f,
                SecondaryBaseGain = 2.5f
            });
            studyPrograms.Add(new StudyProgram
            {
                Id = "systems_notes",
                DisplayName = "Systems Design Notes",
                Description = "Study serving, training bottlenecks, reliability and cost tradeoffs.",
                DurationDays = 7,
                CashCost = 850f,
                PrimaryAttribute = StaffAttribute.Engineering,
                SecondaryAttribute = StaffAttribute.Infrastructure,
                PrimaryBaseGain = 5f,
                SecondaryBaseGain = 4f
            });
            studyPrograms.Add(new StudyProgram
            {
                Id = "ux_review",
                DisplayName = "Product UX Review",
                Description = "Study onboarding, website clarity and how users understand AI tools.",
                DurationDays = 4,
                CashCost = 650f,
                PrimaryAttribute = StaffAttribute.Product,
                SecondaryAttribute = StaffAttribute.Design,
                PrimaryBaseGain = 4.5f,
                SecondaryBaseGain = 4f
            });
            studyPrograms.Add(new StudyProgram
            {
                Id = "safety_cases",
                DisplayName = "Safety Case Studies",
                Description = "Study incidents, misuse, evaluation failures and safety process design.",
                DurationDays = 6,
                CashCost = 700f,
                PrimaryAttribute = StaffAttribute.Safety,
                SecondaryAttribute = StaffAttribute.Research,
                PrimaryBaseGain = 5.2f,
                SecondaryBaseGain = 2.8f
            });
            studyPrograms.Add(new StudyProgram
            {
                Id = "operating_cadence",
                DisplayName = "Founder Operating Cadence",
                Description = "Study planning, communication, focus and weekly operating discipline.",
                DurationDays = 3,
                CashCost = 300f,
                PrimaryAttribute = StaffAttribute.Leadership,
                SecondaryAttribute = StaffAttribute.Communication,
                PrimaryBaseGain = 3.8f,
                SecondaryBaseGain = 3.2f
            });
        }
    }

    private void EnsureFounder()
    {
        if (staff.Count > 0)
        {
            return;
        }

        var founder = new StaffProfile
        {
            Id = "founder",
            DisplayName = "Founder",
            RoleTitle = "Solo Founder / CEO",
            Level = 1,
            Salary = 0f,
            Burnout = 8f,
            Education = "Self-directed AI study, open-source courses, late-night papers.",
            WorkHistory = "Built small automation tools, demos and internal scripts before founding the company.",
            Assignment = TeamRole.ChiefExecutiveOfficer,
            SkillPoints = 1,
            EarnedSkillPoints = 0
        };

        AddStartingAttribute(founder, StaffAttribute.Research, 12f);
        AddStartingAttribute(founder, StaffAttribute.Engineering, 14f);
        AddStartingAttribute(founder, StaffAttribute.Product, 10f);
        AddStartingAttribute(founder, StaffAttribute.Design, 7f);
        AddStartingAttribute(founder, StaffAttribute.Infrastructure, 8f);
        AddStartingAttribute(founder, StaffAttribute.Safety, 6f);
        AddStartingAttribute(founder, StaffAttribute.Communication, 9f);
        AddStartingAttribute(founder, StaffAttribute.Focus, 11f);
        AddStartingAttribute(founder, StaffAttribute.Leadership, 8f);

        founder.Perks.Add(new SkillPerk { Id = "lean_reader", DisplayName = "Lean Reader", Description = "Study costs 10% less.", Cost = 1 });
        founder.Perks.Add(new SkillPerk { Id = "deep_work", DisplayName = "Deep Work", Description = "Research and Focus study gains are 15% stronger.", Cost = 2 });
        founder.Perks.Add(new SkillPerk { Id = "practical_builder", DisplayName = "Practical Builder", Description = "Prototype work benefits more from Engineering.", Cost = 2 });
        founder.Perks.Add(new SkillPerk { Id = "clear_communicator", DisplayName = "Clear Communicator", Description = "Website and documentation work benefits more from Communication.", Cost = 2 });
        founder.Perks.Add(new SkillPerk { Id = "cost_discipline", DisplayName = "Cost Discipline", Description = "Future salary and study pressure is easier to manage.", Cost = 3 });

        staff.Add(founder);
    }

    private static void AddStartingAttribute(StaffProfile profile, StaffAttribute attribute, float value)
    {
        profile.Attributes.Add(new AttributeValue { Attribute = attribute, Value = value });
    }

    private static void Notify(string message)
    {
        GameManager.Instance?.SendNotification(message);
        ToastNotification.ShowGlobal(message, ToastNotification.Category.Info);
    }
}
