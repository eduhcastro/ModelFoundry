using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed class StartupDashboardController : MonoBehaviour
{
    private const string WindowName = "DecisionWindow_TranslucentBlurStyle";
    private const string ContentName = "WindowContent";

    private StartupSimulationManager simulation;
    private TeamSimulationManager team;
    private TextMeshProUGUI goalText;
    private TextMeshProUGUI windowTitleText;
    private TextMeshProUGUI windowMetaText;
    private Transform contentRoot;
    private CanvasGroup windowGroup;
    private TeamSimulationManager.StaffProfile selectedStaff;
    private string currentView = "research";

    public static StartupDashboardController AttachTo(Canvas canvas)
    {
        if (canvas == null)
        {
            return null;
        }

        var existing = canvas.GetComponentInChildren<StartupDashboardController>(true);
        if (existing != null)
        {
            EnsureCanvasInteraction(canvas);
            existing.transform.SetAsLastSibling();
            existing.EnsureBuilt();
            return existing;
        }

        EnsureCanvasInteraction(canvas);
        var root = new GameObject("StartupCommandLayer");
        root.transform.SetParent(canvas.transform, false);
        root.transform.SetAsLastSibling();
        var controller = root.AddComponent<StartupDashboardController>();
        controller.EnsureBuilt();
        return controller;
    }

    public void RebuildUi()
    {
        ResolveManagers();
        ClearChildren(transform);
        ConfigureRoot();
        BuildGoalChecklist();
        BuildIconDock();
        BuildDecisionWindow();
        CloseWindow();
        Refresh();
    }

    private void Awake()
    {
        ResolveManagers();
        EnsureBuilt();
    }

    private void OnEnable()
    {
        ResolveManagers();

        if (simulation != null)
        {
            simulation.OnStateChanged += Refresh;
        }

        if (team != null)
        {
            team.OnTeamChanged += RefreshCurrentView;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged += Refresh;
        }

        Refresh();
    }

    private void OnDisable()
    {
        if (simulation != null)
        {
            simulation.OnStateChanged -= Refresh;
        }

        if (team != null)
        {
            team.OnTeamChanged -= RefreshCurrentView;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged -= Refresh;
        }
    }

    private void Update()
    {
        HandleKeyboardShortcuts();
    }

    private void ResolveManagers()
    {
        simulation = StartupSimulationManager.EnsureExists();
        team = TeamSimulationManager.EnsureExists();
        selectedStaff ??= team?.GetFounder();
    }

    private void EnsureBuilt()
    {
        CacheSceneReferences();
        if (transform.childCount == 0 || goalText == null || windowGroup == null || contentRoot == null)
        {
            RebuildUi();
        }
    }

    private void ConfigureRoot()
    {
        var rect = GetComponent<RectTransform>();
        if (rect == null)
        {
            rect = gameObject.AddComponent<RectTransform>();
        }

        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        transform.SetAsLastSibling();

        var image = GetComponent<Image>();
        if (image != null)
        {
            DestroyUnityObject(image);
        }
    }

    private void BuildGoalChecklist()
    {
        var panel = CreatePanel("GoalChecklist", transform, new Vector2(310f, 176f), new Vector2(24f, -104f), new Color(1f, 1f, 1f, 0.9f));
        panel.anchorMin = new Vector2(0f, 1f);
        panel.anchorMax = new Vector2(0f, 1f);
        panel.pivot = new Vector2(0f, 1f);

        var title = CreateText("Title", panel, "Next Goals", 15f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(268f, 24f), new Vector2(18f, -14f), TopLeft(), TopLeft(), TopLeft());

        goalText = CreateText("GoalItems", panel, "", 12f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(goalText.rectTransform, new Vector2(270f, 116f), new Vector2(18f, -48f), TopLeft(), TopLeft(), TopLeft());
    }

    private void BuildIconDock()
    {
        var dock = CreatePanel("IconDock", transform, new Vector2(408f, 62f), new Vector2(0f, 22f), new Color(1f, 1f, 1f, 0.9f));
        dock.anchorMin = new Vector2(0.5f, 0f);
        dock.anchorMax = new Vector2(0.5f, 0f);
        dock.pivot = new Vector2(0.5f, 0f);

        var layout = dock.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(12, 12, 8, 8);
        layout.spacing = 8f;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;
        layout.childAlignment = TextAnchor.MiddleCenter;

        CreateDockButton(dock, "Research", "R", OpenResearch);
        CreateDockButton(dock, "Product", "P", OpenProduct);
        CreateDockButton(dock, "Team", "T", OpenTeam);
        CreateDockButton(dock, "Infrastructure", "I", OpenInfrastructure);
        CreateDockButton(dock, "Arena", "A", OpenArena);
        CreateDockButton(dock, "TechPulse", "N", ToggleTechPulse);
    }

    private void BuildDecisionWindow()
    {
        var window = CreatePanel(WindowName, transform, new Vector2(720f, 560f), new Vector2(-34f, 0f), new Color(1f, 1f, 1f, 0.92f));
        window.anchorMin = new Vector2(1f, 0.5f);
        window.anchorMax = new Vector2(1f, 0.5f);
        window.pivot = new Vector2(1f, 0.5f);
        windowGroup = window.gameObject.AddComponent<CanvasGroup>();

        windowTitleText = CreateText("WindowTitle", window, "", 22f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(windowTitleText.rectTransform, new Vector2(470f, 34f), new Vector2(28f, -24f), TopLeft(), TopLeft(), TopLeft());

        windowMetaText = CreateText("WindowMeta", window, "", 11f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(windowMetaText.rectTransform, new Vector2(560f, 38f), new Vector2(28f, -62f), TopLeft(), TopLeft(), TopLeft());

        var closeButton = CreateSmallButton("CloseWindow", window, "X", new Vector2(38f, 34f), new Vector2(-24f, -24f), true);
        closeButton.onClick.AddListener(CloseWindow);

        var content = new GameObject(ContentName);
        content.transform.SetParent(window, false);
        contentRoot = content.transform;
        var contentRect = content.AddComponent<RectTransform>();
        SetRect(contentRect, new Vector2(664f, 430f), new Vector2(28f, -106f), TopLeft(), TopLeft(), TopLeft());
        var layout = content.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10f;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;
    }

    private void OpenResearch()
    {
        currentView = "research";
        OpenWindow();
        SetHeader("Research Study", "Pick a person and a study track. Study takes days, costs cash, and has diminishing returns.");
        PopulateResearch();
    }

    private void HandleKeyboardShortcuts()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            return;
        }

        if (keyboard.rKey.wasPressedThisFrame)
        {
            OpenResearch();
        }
        else if (keyboard.tKey.wasPressedThisFrame)
        {
            OpenTeam();
        }
        else if (keyboard.pKey.wasPressedThisFrame)
        {
            OpenProduct();
        }
        else if (keyboard.iKey.wasPressedThisFrame)
        {
            OpenInfrastructure();
        }
        else if (keyboard.aKey.wasPressedThisFrame)
        {
            OpenArena();
        }
        else if (keyboard.escapeKey.wasPressedThisFrame)
        {
            CloseWindow();
        }
    }

    private void OpenProduct()
    {
        currentView = "product";
        OpenWindow();
        SetHeader("Product Surface", "The product flow will later use assigned staff. For now, these are compatibility actions.");
        ClearContent();
        CreateActionCard("Improve website", "$1.2k", "Sharpen positioning, docs and buyer trust.", () => RunAction(simulation.ImproveWebsite));
        CreateActionCard("Build prototype", "$2.5k", "Create a rough internal prototype. Adds maintenance load.", () => RunAction(simulation.BuildPrototype));
        CreateActionCard("Launch CLI agent", "$6k", "Requires developer, model capability and hosting reliability.", () => RunAction(simulation.LaunchCliAgent));
    }

    private void OpenTeam()
    {
        currentView = "team";
        selectedStaff ??= team.GetFounder();
        OpenWindow();
        SetHeader("Team Management", "Select a person, inspect attributes, assign role, study, or spend skill points.");
        PopulateTeam();
    }

    private void OpenInfrastructure()
    {
        currentView = "infra";
        OpenWindow();
        SetHeader("Infrastructure", "Early hosting decisions unlock more realistic product and model work.");
        ClearContent();
        CreateActionCard("Secure cloud hosting", "$8k", "Increase reliability for training, inference and product launches.", () => RunAction(simulation.SecureCloudHosting));
        CreateActionCard("Assign founder to infrastructure", "Role", "Focus the founder on infra work before buying capacity.", () => AssignSelected(TeamSimulationManager.TeamRole.Infrastructure));
    }

    private void OpenArena()
    {
        currentView = "arena";
        OpenWindow();
        SetHeader("AI Arena", "Public benchmark work should be prepared by people and submitted only when credible.");
        ClearContent();
        CreateActionCard("Assign founder to Arena prep", "Role", "Focus on evals, benchmark reading and submission readiness.", () => AssignSelected(TeamSimulationManager.TeamRole.Arena));
        CreateActionCard("Submit benchmark run", "$3.5k", "Expose the model to public scoring and trust effects.", () => RunAction(simulation.SubmitToArena));
    }

    private void PopulateResearch()
    {
        selectedStaff ??= team.GetFounder();
        ClearContent();
        CreateStaffSummary(selectedStaff);

        foreach (var program in team.StudyPrograms)
        {
            CreateStudyCard(selectedStaff, program);
        }
    }

    private void PopulateTeam()
    {
        ClearContent();
        selectedStaff ??= team.GetFounder();

        foreach (var staffMember in team.Staff)
        {
            CreateEmployeeRow(staffMember);
        }

        CreateStaffProfile(selectedStaff);
        CreateRoleActions(selectedStaff);
        CreateSkillTree(selectedStaff);
    }

    private void CreateEmployeeRow(TeamSimulationManager.StaffProfile profile)
    {
        var row = CreatePanel("Employee_" + profile.Id, contentRoot, new Vector2(0f, 68f), Vector2.zero, new Color(0.95f, 0.97f, 1f, 0.95f));
        row.gameObject.AddComponent<LayoutElement>().preferredHeight = 68f;

        var avatar = CreatePanel("HeadPreview", row, new Vector2(48f, 48f), new Vector2(14f, -10f), new Color(0.12f, 0.16f, 0.22f, 1f));
        avatar.anchorMin = TopLeft();
        avatar.anchorMax = TopLeft();
        avatar.pivot = TopLeft();
        var avatarText = CreateText("Initials", avatar, "CEO", 14f, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        Stretch(avatarText.rectTransform, Vector2.zero, Vector2.zero);

        var title = CreateText("Name", row, $"{profile.DisplayName}  |  {profile.RoleTitle}", 13f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(420f, 20f), new Vector2(76f, -10f), TopLeft(), TopLeft(), TopLeft());

        var meta = CreateText("Meta", row, $"Assignment: {TeamSimulationManager.FormatRole(profile.Assignment)}  |  Skill points: {profile.SkillPoints}  |  Burnout: {profile.Burnout:F0}", 11f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(meta.rectTransform, new Vector2(472f, 20f), new Vector2(76f, -36f), TopLeft(), TopLeft(), TopLeft());

        var select = CreateSmallButton("Select", row, "VIEW", new Vector2(72f, 34f), new Vector2(-18f, -17f), true);
        select.onClick.AddListener(() =>
        {
            selectedStaff = profile;
            PopulateTeam();
        });
    }

    private void CreateStaffProfile(TeamSimulationManager.StaffProfile profile)
    {
        var card = CreatePanel("StaffProfile", contentRoot, new Vector2(0f, 174f), Vector2.zero, new Color(1f, 1f, 1f, 0.96f));
        card.gameObject.AddComponent<LayoutElement>().preferredHeight = 174f;

        var title = CreateText("Title", card, "Profile", 14f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(200f, 22f), new Vector2(14f, -10f), TopLeft(), TopLeft(), TopLeft());

        var bio = CreateText("Bio", card, $"{profile.Education}\n{profile.WorkHistory}", 11f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(bio.rectTransform, new Vector2(300f, 60f), new Vector2(14f, -36f), TopLeft(), TopLeft(), TopLeft());

        var attributes = CreateText("Attributes", card, BuildAttributeText(profile), 11f, FontStyles.Normal, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(attributes.rectTransform, new Vector2(310f, 124f), new Vector2(330f, -10f), TopLeft(), TopLeft(), TopLeft());

        var studyButton = CreateSmallButton("Study", card, "STUDY", new Vector2(82f, 34f), new Vector2(14f, -122f), TopLeft());
        studyButton.onClick.AddListener(OpenResearch);
    }

    private void CreateRoleActions(TeamSimulationManager.StaffProfile profile)
    {
        var card = CreatePanel("RoleFlow", contentRoot, new Vector2(0f, 92f), Vector2.zero, new Color(0.95f, 0.97f, 1f, 0.95f));
        card.gameObject.AddComponent<LayoutElement>().preferredHeight = 92f;

        var title = CreateText("Title", card, "Role Flow", 13f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(160f, 20f), new Vector2(14f, -10f), TopLeft(), TopLeft(), TopLeft());

        CreateRoleButton(card, "Website", new Vector2(14f, -46f), profile, TeamSimulationManager.TeamRole.Website);
        CreateRoleButton(card, "Prototype", new Vector2(118f, -46f), profile, TeamSimulationManager.TeamRole.Prototype);
        CreateRoleButton(card, "Research", new Vector2(238f, -46f), profile, TeamSimulationManager.TeamRole.ResearchStudy);
        CreateRoleButton(card, "Infra", new Vector2(346f, -46f), profile, TeamSimulationManager.TeamRole.Infrastructure);
        CreateRoleButton(card, "Rest", new Vector2(438f, -46f), profile, TeamSimulationManager.TeamRole.Rest);
    }

    private void CreateSkillTree(TeamSimulationManager.StaffProfile profile)
    {
        var card = CreatePanel("SkillTree", contentRoot, new Vector2(0f, 148f), Vector2.zero, new Color(1f, 1f, 1f, 0.96f));
        card.gameObject.AddComponent<LayoutElement>().preferredHeight = 148f;

        var title = CreateText("Title", card, $"Skill Tree  |  Points: {profile.SkillPoints}", 13f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(260f, 20f), new Vector2(14f, -10f), TopLeft(), TopLeft(), TopLeft());

        for (var i = 0; i < profile.Perks.Count && i < 5; i++)
        {
            var perk = profile.Perks[i];
            var x = 14f + i * 126f;
            var perkButton = CreateSmallButton("Perk_" + perk.Id, card, perk.Unlocked ? "OWNED" : $"{perk.Cost} PT", new Vector2(92f, 34f), new Vector2(x, -42f), TopLeft());
            var label = CreateText("PerkLabel_" + perk.Id, card, perk.DisplayName, 10f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
            SetRect(label.rectTransform, new Vector2(112f, 42f), new Vector2(x, -82f), TopLeft(), TopLeft(), TopLeft());
            perkButton.interactable = !perk.Unlocked;
            perkButton.onClick.AddListener(() =>
            {
                team.UnlockPerk(profile, perk.Id);
                PopulateTeam();
            });
        }
    }

    private void CreateStaffSummary(TeamSimulationManager.StaffProfile profile)
    {
        var summary = CreatePanel("SelectedStaff", contentRoot, new Vector2(0f, 78f), Vector2.zero, new Color(0.95f, 0.97f, 1f, 0.95f));
        summary.gameObject.AddComponent<LayoutElement>().preferredHeight = 78f;

        var title = CreateText("Title", summary, $"{profile.DisplayName} - {TeamSimulationManager.FormatRole(profile.Assignment)}", 13f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(360f, 20f), new Vector2(14f, -10f), TopLeft(), TopLeft(), TopLeft());

        var activeStudy = string.IsNullOrEmpty(profile.ActiveStudyId)
            ? "No active study"
            : $"Studying {team.GetStudyProgram(profile.ActiveStudyId)?.DisplayName}: {profile.StudyRemainingDays}/{profile.StudyTotalDays} days left";

        var meta = CreateText("Meta", summary, $"{activeStudy}\nRepeated study gives smaller gains. Specialize when broad gains slow down.", 11f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(meta.rectTransform, new Vector2(560f, 42f), new Vector2(14f, -34f), TopLeft(), TopLeft(), TopLeft());
    }

    private void CreateStudyCard(TeamSimulationManager.StaffProfile profile, TeamSimulationManager.StudyProgram program)
    {
        var card = CreatePanel("Study_" + program.Id, contentRoot, new Vector2(0f, 92f), Vector2.zero, new Color(1f, 1f, 1f, 0.96f));
        card.gameObject.AddComponent<LayoutElement>().preferredHeight = 92f;

        var cost = team.GetStudyCost(profile, program);
        var primaryGain = team.PreviewPrimaryGain(profile, program);
        var secondaryGain = team.PreviewSecondaryGain(profile, program);

        var title = CreateText("Title", card, program.DisplayName, 13f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(title.rectTransform, new Vector2(280f, 20f), new Vector2(14f, -10f), TopLeft(), TopLeft(), TopLeft());

        var meta = CreateText("Meta", card, $"{program.DurationDays} days  |  ${cost:N0}  |  +{primaryGain:F1} {program.PrimaryAttribute}, +{secondaryGain:F1} {program.SecondaryAttribute}", 11f, FontStyles.Bold, GameDesignConstants.BrandPrimary, TextAlignmentOptions.Left);
        SetRect(meta.rectTransform, new Vector2(430f, 18f), new Vector2(14f, -34f), TopLeft(), TopLeft(), TopLeft());

        var body = CreateText("Body", card, program.Description, 11f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(body.rectTransform, new Vector2(470f, 32f), new Vector2(14f, -56f), TopLeft(), TopLeft(), TopLeft());

        var start = CreateSmallButton("Start", card, "START", new Vector2(76f, 34f), new Vector2(-18f, -29f), true);
        start.interactable = string.IsNullOrEmpty(profile.ActiveStudyId);
        start.onClick.AddListener(() =>
        {
            team.StartStudy(profile, program.Id);
            PopulateResearch();
        });
    }

    private void CreateActionCard(string title, string cost, string description, Action action)
    {
        var card = CreatePanel("Action_" + title.Replace(" ", ""), contentRoot, new Vector2(0f, 86f), Vector2.zero, new Color(1f, 1f, 1f, 0.96f));
        card.gameObject.AddComponent<LayoutElement>().preferredHeight = 86f;

        var titleText = CreateText("Title", card, title, 13f, FontStyles.Bold, GameDesignConstants.TextPrimary, TextAlignmentOptions.Left);
        SetRect(titleText.rectTransform, new Vector2(300f, 20f), new Vector2(14f, -11f), TopLeft(), TopLeft(), TopLeft());

        var costText = CreateText("Cost", card, cost, 12f, FontStyles.Bold, GameDesignConstants.BrandPrimary, TextAlignmentOptions.Right);
        SetRect(costText.rectTransform, new Vector2(100f, 20f), new Vector2(-104f, -11f), TopRight(), TopRight(), TopRight());

        var bodyText = CreateText("Description", card, description, 11f, FontStyles.Normal, GameDesignConstants.TextSecondary, TextAlignmentOptions.Left);
        SetRect(bodyText.rectTransform, new Vector2(492f, 38f), new Vector2(14f, -38f), TopLeft(), TopLeft(), TopLeft());

        var runButton = CreateSmallButton("Run", card, "GO", new Vector2(58f, 34f), new Vector2(-18f, -38f), true);
        runButton.onClick.AddListener(() => action?.Invoke());
    }

    private void CreateRoleButton(Transform parent, string label, Vector2 position, TeamSimulationManager.StaffProfile profile, TeamSimulationManager.TeamRole role)
    {
        var button = CreateSmallButton("Role_" + role, parent, label, new Vector2(88f, 30f), position, TopLeft());
        button.onClick.AddListener(() =>
        {
            team.AssignRole(profile, role);
            PopulateTeam();
        });
    }

    private void AssignSelected(TeamSimulationManager.TeamRole role)
    {
        selectedStaff ??= team.GetFounder();
        team.AssignRole(selectedStaff, role);
        RefreshCurrentView();
    }

    private void RunAction(Action action)
    {
        action?.Invoke();
        RefreshCurrentView();
    }

    private void RefreshCurrentView()
    {
        ResolveManagers();
        Refresh();

        if (windowGroup == null || !windowGroup.gameObject.activeSelf)
        {
            return;
        }

        switch (currentView)
        {
            case "research": PopulateResearch(); break;
            case "team": PopulateTeam(); break;
            case "product": OpenProduct(); break;
            case "infra": OpenInfrastructure(); break;
            case "arena": OpenArena(); break;
        }
    }

    private void OpenWindow()
    {
        if (windowGroup == null)
        {
            return;
        }

        windowGroup.gameObject.SetActive(true);
        windowGroup.alpha = 1f;
        windowGroup.interactable = true;
        windowGroup.blocksRaycasts = true;
    }

    private void CloseWindow()
    {
        if (windowGroup == null)
        {
            return;
        }

        windowGroup.alpha = 0f;
        windowGroup.interactable = false;
        windowGroup.blocksRaycasts = false;
        windowGroup.gameObject.SetActive(false);
    }

    private void SetHeader(string title, string meta)
    {
        if (windowTitleText != null)
        {
            windowTitleText.text = title;
        }

        if (windowMetaText != null)
        {
            windowMetaText.text = meta;
        }
    }

    private void ClearContent()
    {
        if (contentRoot != null)
        {
            ClearChildren(contentRoot);
        }
    }

    private void ToggleTechPulse()
    {
        var canvas = GetComponentInParent<Canvas>();
        var techPulse = canvas != null ? canvas.GetComponentInChildren<TechPulseUI>(true) : null;
        techPulse?.Toggle();
    }

    private void Refresh()
    {
        ResolveManagers();

        if (goalText == null || simulation == null)
        {
            return;
        }

        var founder = team.GetFounder();
        var websiteReady = simulation.WebsiteQuality >= 16f;
        var prototypeReady = simulation.ProductSurfaces > 0 || simulation.ProductQuality >= 12f;
        var studying = founder != null && !string.IsNullOrEmpty(founder.ActiveStudyId);

        goalText.text =
            $"{Check(studying || simulation.ResearchSkill > 12f)} Study papers with the founder\n" +
            $"{Check(websiteReady)} Improve the website clarity\n" +
            $"{Check(prototypeReady)} Build a working prototype\n" +
            $"Current: {simulation.GetNextGoal()}";
    }

    private static string Check(bool done)
    {
        return done ? "[x]" : "[ ]";
    }

    private string BuildAttributeText(TeamSimulationManager.StaffProfile profile)
    {
        return
            $"Research {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Research):F0}   Engineering {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Engineering):F0}\n" +
            $"Product {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Product):F0}   Design {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Design):F0}\n" +
            $"Infrastructure {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Infrastructure):F0}   Safety {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Safety):F0}\n" +
            $"Communication {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Communication):F0}   Focus {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Focus):F0}\n" +
            $"Leadership {team.GetAttribute(profile, TeamSimulationManager.StaffAttribute.Leadership):F0}";
    }

    private void CacheSceneReferences()
    {
        goalText = transform.Find("GoalChecklist/GoalItems")?.GetComponent<TextMeshProUGUI>();
        windowGroup = transform.Find(WindowName)?.GetComponent<CanvasGroup>();
        windowTitleText = transform.Find(WindowName + "/WindowTitle")?.GetComponent<TextMeshProUGUI>();
        windowMetaText = transform.Find(WindowName + "/WindowMeta")?.GetComponent<TextMeshProUGUI>();
        contentRoot = transform.Find(WindowName + "/" + ContentName);
    }

    private Button CreateDockButton(Transform parent, string tooltipName, string icon, Action onClick)
    {
        var buttonObj = new GameObject("Btn_" + tooltipName);
        buttonObj.transform.SetParent(parent, false);
        var rect = buttonObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(46f, 46f);
        var layout = buttonObj.AddComponent<LayoutElement>();
        layout.preferredWidth = 46f;
        layout.preferredHeight = 46f;
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.13f, 0.18f, 0.92f);
        var button = buttonObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());

        var label = CreateText("Icon", buttonObj.transform, icon, 18f, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        Stretch(label.rectTransform, Vector2.zero, Vector2.zero);
        return button;
    }

    private Button CreateSmallButton(string name, Transform parent, string label, Vector2 size, Vector2 position, bool topRight)
    {
        return CreateSmallButton(name, parent, label, size, position, topRight ? TopRight() : TopLeft());
    }

    private Button CreateSmallButton(string name, Transform parent, string label, Vector2 size, Vector2 position, Vector2 anchor)
    {
        var buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        var rect = buttonObj.AddComponent<RectTransform>();
        SetRect(rect, size, position, anchor, anchor, anchor);
        var image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.1f, 0.13f, 0.18f, 0.95f);
        var button = buttonObj.AddComponent<Button>();

        var text = CreateText("Label", buttonObj.transform, label, 11f, FontStyles.Bold, Color.white, TextAlignmentOptions.Center);
        Stretch(text.rectTransform, Vector2.zero, Vector2.zero);
        return button;
    }

    private static RectTransform CreatePanel(string name, Transform parent, Vector2 size, Vector2 position, Color color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        var image = obj.AddComponent<Image>();
        image.color = color;
        return rect;
    }

    private static void EnsureCanvasInteraction(Canvas canvas)
    {
        if (canvas.GetComponent<GraphicRaycaster>() == null)
        {
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }

        if (EventSystem.current != null)
        {
            return;
        }

        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();
    }

    private static TextMeshProUGUI CreateText(string name, Transform parent, string value, float size, FontStyles style, Color color, TextAlignmentOptions alignment)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = Vector2.zero;
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.text = value;
        text.fontSize = size;
        text.fontStyle = style;
        text.color = color;
        text.alignment = alignment;
        text.textWrappingMode = TextWrappingModes.Normal;
        return text;
    }

    private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }

    private static void SetRect(RectTransform rect, Vector2 size, Vector2 position, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = pivot;
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
    }

    private static Vector2 TopLeft()
    {
        return new Vector2(0f, 1f);
    }

    private static Vector2 TopRight()
    {
        return new Vector2(1f, 1f);
    }

    private static void ClearChildren(Transform parent)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            DestroyUnityObject(parent.GetChild(i).gameObject);
        }
    }

    private static void DestroyUnityObject(UnityEngine.Object target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }
}
