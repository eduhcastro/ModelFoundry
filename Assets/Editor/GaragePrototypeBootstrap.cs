using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public static class GaragePrototypeBootstrap
{
    private const string MainMenuScenePath = "Assets/Scenes/MainMenu.unity";
    private const string GameplayScenePath = "Assets/Scenes/GaragePrototype.unity";
    private const string RequestPath = "ProjectSettings/ModelFoundryBootstrap.request";
    private const string OfficePackPath = "DownloadedAssets/POLYGON_-_Office_Pack__v1.03_.unitypackage";
    private const string IconsPackPath = "DownloadedAssets/POLYGON___Icons_Pack___Art_by_Synty_v1_2_0.unitypackage";

    [InitializeOnLoadMethod]
    private static void QueueRequestedBuild()
    {
        if (!File.Exists(RequestPath))
        {
            return;
        }

        EditorApplication.delayCall += RunRequestedBuild;
    }

    private static void RunRequestedBuild()
    {
        if (!File.Exists(RequestPath))
        {
            return;
        }

        if (File.Exists(OfficePackPath))
        {
            AssetDatabase.ImportPackage(OfficePackPath, false);
        }

        if (File.Exists(IconsPackPath))
        {
            AssetDatabase.ImportPackage(IconsPackPath, false);
        }

        BuildAllScenes();
        File.Delete(RequestPath);
        AssetDatabase.Refresh();
    }

    [MenuItem("Model Foundry/Build Game Scenes")]
    public static void BuildAllScenes()
    {
        EnsureFolders();
        EnsureTmpResources();

        BuildMainMenuScene();
        BuildGameplayScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Model Foundry: All scenes built successfully!");
    }

    [MenuItem("Model Foundry/Force Rebuild Scenes (Reset Layout)")]
    public static void ForceRebuildAllScenes()
    {
        EnsureFolders();
        EnsureTmpResources();

        // Temporarily delete files to force a full clean regenerate
        if (File.Exists(MainMenuScenePath)) File.Delete(MainMenuScenePath);
        if (File.Exists(GameplayScenePath)) File.Delete(GameplayScenePath);

        BuildMainMenuScene();
        BuildGameplayScene();

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Model Foundry: All scenes force rebuilt from scratch successfully!");
    }

    [MenuItem("Model Foundry/Apply Realistic Refactor UI")]
    public static void ApplyRealisticRefactorUiToGameplayScene()
    {
        if (!File.Exists(GameplayScenePath))
        {
            Debug.LogWarning($"Model Foundry: Gameplay scene not found at {GameplayScenePath}. Run Build Game Scenes first.");
            return;
        }

        var scene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
        ApplyRealisticRefactorUi(scene);
        EditorSceneManager.SaveScene(scene, GameplayScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Model Foundry: Realistic refactor UI applied to the gameplay scene.");
    }

    private static void EnsureFolders()
    {
        foreach (var path in new[]
        {
            "Assets/Art",
            "Assets/Materials",
            "Assets/Prefabs",
            "Assets/Scripts/Core",
            "Assets/Scripts/Simulation",
            "Assets/Scripts/UI",
            "Assets/ScriptableObjects",
            "Assets/Scenes"
        })
        {
            Directory.CreateDirectory(path);
        }
    }

    private static void EnsureTmpResources()
    {
        if (!Directory.Exists("Assets/TextMesh Pro"))
        {
            string packagePath = "Packages/com.unity.ugui/Package Resources/TMP Essential Resources.unitypackage";
            if (File.Exists(packagePath))
            {
                AssetDatabase.ImportPackage(packagePath, false);
                AssetDatabase.Refresh();
            }
            else
            {
                // Fallback to menu command if package path not directly accessible relative to root
                EditorApplication.ExecuteMenuItem("Window/TextMeshPro/Import TMP Essential Resources");
                AssetDatabase.Refresh();
            }
        }

        // Force-initialize TMP_Settings to avoid NullReferenceException in batchmode/early execution
        try
        {
            var settings = AssetDatabase.LoadAssetAtPath<TMP_Settings>("Assets/TextMesh Pro/Resources/TMP Settings.asset");
            if (settings != null)
            {
                var field = typeof(TMP_Settings).GetField("s_Instance", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(null, settings);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning("Failed to pre-initialize TMP_Settings: " + ex.Message);
        }
    }

    // ── Build Main Menu Scene ──────────────────────────────────────────
    private static void BuildMainMenuScene()
    {
        if (File.Exists(MainMenuScenePath))
        {
            Debug.Log($"Model Foundry: Preserved existing MainMenu scene at {MainMenuScenePath}. Delete this file to regenerate.");
            EditorSceneManager.OpenScene(MainMenuScenePath, OpenSceneMode.Single);
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "MainMenu";

        ConvertPolygonMaterialsToUrp();

        // ── 3D Showcase Background ──────────────────────────────────
        var showcaseRoot = new GameObject("Menu Showcase");

        // Floor tiles
        for (var x = -3; x <= 3; x++)
        {
            for (var z = -1; z <= 2; z++)
            {
                PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Floor_Concrete_01.prefab",
                    $"Floor_{x}_{z}", new Vector3(x * 2f, 0f, z * 2f), Vector3.zero, Vector3.one, showcaseRoot.transform);
            }
        }

        // Back wall with windows
        for (var x = -3; x <= 3; x++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Blank_Window_01.prefab",
                $"BackWall_{x}", new Vector3(x * 2f, 0f, 5f), new Vector3(0f, 180f, 0f), Vector3.one, showcaseRoot.transform);
        }

        // Showcase desk setup (center-right of view)
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab",
            "ShowcaseDesk", new Vector3(2f, 0f, 1.5f), new Vector3(0f, 200f, 0f), Vector3.one, showcaseRoot.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab",
            "ShowcaseComputer", new Vector3(2f, 0.72f, 1.4f), new Vector3(0f, 200f, 0f), Vector3.one, showcaseRoot.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab",
            "ShowcaseChair", new Vector3(2f, 0f, 0.3f), new Vector3(0f, 15f, 0f), Vector3.one, showcaseRoot.transform);

        // Server rack (right)
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_01.prefab",
            "ShowcaseServer1", new Vector3(5.5f, 0f, 3.5f), new Vector3(0f, -90f, 0f), Vector3.one, showcaseRoot.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_02.prefab",
            "ShowcaseServer2", new Vector3(5.5f, 0f, 2.3f), new Vector3(0f, -90f, 0f), Vector3.one, showcaseRoot.transform);

        // Whiteboard (left)
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Whiteboard_01.prefab",
            "ShowcaseWhiteboard", new Vector3(-3f, 1.45f, 4.7f), new Vector3(0f, 180f, 0f), Vector3.one, showcaseRoot.transform);

        // Ambient deco
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Plant_01.prefab",
            "ShowcasePlant1", new Vector3(-5f, 0f, 3.5f), Vector3.zero, Vector3.one, showcaseRoot.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Plant_02.prefab",
            "ShowcasePlant2", new Vector3(6f, 0f, 0.5f), Vector3.zero, Vector3.one, showcaseRoot.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Kitchen Props/SM_Prop_CoffeeMachine_Dripper_01.prefab",
            "ShowcaseCoffee", new Vector3(-4.2f, 0f, 0f), new Vector3(0f, 90f, 0f), Vector3.one, showcaseRoot.transform);

        // Character standing in the scene
        var menuChar = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Male_01.prefab",
            "ShowcaseCharacter", new Vector3(0.5f, 0f, 0.5f), new Vector3(0f, -20f, 0f), Vector3.one, showcaseRoot.transform);

        // ── Lighting & Scene Atmosphere ─────────────────────────────
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.06f, 0.08f, 0.18f); // beautiful deep navy slate ambient

        // Key Light (warm, cozy sunlight/interior light)
        var light = new GameObject("Key Light");
        var lightComponent = light.AddComponent<Light>();
        lightComponent.type = LightType.Directional;
        lightComponent.intensity = 1.35f;
        lightComponent.color = new Color(1f, 0.92f, 0.83f); // rich warm white
        lightComponent.shadows = LightShadows.Soft;
        lightComponent.shadowStrength = 0.85f;
        lightComponent.shadowBias = 0.05f;
        light.transform.rotation = Quaternion.Euler(40f, -40f, 0f);

        // Fill Light (soft sky fill)
        var fillLight = new GameObject("Fill Light");
        var fillLightComponent = fillLight.AddComponent<Light>();
        fillLightComponent.type = LightType.Directional;
        fillLightComponent.intensity = 0.6f;
        fillLightComponent.color = new Color(0.85f, 0.88f, 0.92f); // soft sky reflection
        fillLight.transform.rotation = Quaternion.Euler(20f, 140f, 0f);

        // Accent Point Light on desk area (warm desk lamp)
        var accentLight = new GameObject("Accent Light");
        var accentLightComponent = accentLight.AddComponent<Light>();
        accentLightComponent.type = LightType.Point;
        accentLightComponent.intensity = 1.5f;
        accentLightComponent.range = 5f;
        accentLightComponent.color = new Color(1f, 0.9f, 0.8f); // warm incandescent desk lamp
        accentLight.transform.position = new Vector3(2f, 3f, 1.5f);

        // ── Camera (isometric-like, showcasing the office) ──────────
        var cameraObject = new GameObject("Menu Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = GameDesignConstants.SurfaceDarkest;
        camera.fieldOfView = 40f;
        cameraObject.transform.position = new Vector3(-1f, 5.5f, -5f);
        cameraObject.transform.rotation = Quaternion.Euler(30f, 20f, 0f);
        camera.tag = "MainCamera";
        cameraObject.AddComponent<MenuCameraOrbit>();

        // UI Event System
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();

        // Canvas
        var canvasObject = new GameObject("MainMenu UI");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        var menuController = canvasObject.AddComponent<MainMenuController>();

        // Semi-transparent overlay (allows 3D scene to show through)
        var bg = CreateUiRect("Background", canvasObject.transform, new Vector2(1920f, 1080f), Vector2.zero, new Color(0.05f, 0.05f, 0.1f, 0.55f));
        bg.anchorMin = Vector2.zero;
        bg.anchorMax = Vector2.one;
        bg.offsetMin = Vector2.zero;
        bg.offsetMax = Vector2.zero;

        // ── Glass Card Container (left-aligned for a modern split layout) ──
        var glassCard = CreatePremiumUiPanel("GlassCard", canvasObject.transform, new Vector2(480f, 700f), Vector2.zero, GameDesignConstants.SurfaceGlass, new Color(1f, 1f, 1f, 0.1f));
        glassCard.anchorMin = new Vector2(0f, 0f);
        glassCard.anchorMax = new Vector2(0f, 1f);
        glassCard.pivot = new Vector2(0f, 0.5f);
        glassCard.offsetMin = new Vector2(80f, 60f);
        glassCard.offsetMax = new Vector2(560f, -60f);

        // Title Group (inside glass card)
        var titleGroupObj = new GameObject("TitleGroup");
        titleGroupObj.transform.SetParent(glassCard, false);
        var titleGroupCG = titleGroupObj.AddComponent<CanvasGroup>();
        var titleGroupRect = titleGroupObj.AddComponent<RectTransform>();
        titleGroupRect.anchorMin = new Vector2(0.5f, 1f);
        titleGroupRect.anchorMax = new Vector2(0.5f, 1f);
        titleGroupRect.pivot = new Vector2(0.5f, 1f);
        titleGroupRect.sizeDelta = new Vector2(420f, 180f);
        titleGroupRect.anchoredPosition = new Vector2(0f, -40f);

        var titleText = CreateTMPText("TitleText", titleGroupObj.transform, "▲  MODEL FOUNDRY", 46f, TextAlignmentOptions.Center, GameDesignConstants.BrandPrimary, new Vector2(420f, 65f), new Vector2(0f, 0f));
        titleText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        titleText.anchorMin = new Vector2(0.5f, 1f);
        titleText.anchorMax = new Vector2(0.5f, 1f);
        titleText.pivot = new Vector2(0.5f, 1f);
        titleText.anchoredPosition = new Vector2(0f, -10f);

        var subtitleText = CreateTMPText("SubtitleText", titleGroupObj.transform, "N E U R A L   E M P I R E   S I M U L A T O R", 14f, TextAlignmentOptions.Center, GameDesignConstants.BrandSecondary, new Vector2(420f, 35f), new Vector2(0f, -80f));
        subtitleText.anchorMin = new Vector2(0.5f, 1f);
        subtitleText.anchorMax = new Vector2(0.5f, 1f);
        subtitleText.pivot = new Vector2(0.5f, 1f);

        // Decorative line separator
        var separator = CreateUiRect("Separator", titleGroupObj.transform, new Vector2(240f, 2f), new Vector2(0f, -115f), GameDesignConstants.BrandPrimary);
        separator.anchorMin = new Vector2(0.5f, 1f);
        separator.anchorMax = new Vector2(0.5f, 1f);
        separator.pivot = new Vector2(0.5f, 0.5f);

        var versionText = CreateTMPText("VersionText", titleGroupObj.transform, "F O U N D R Y   O S   v 2 . 0 . 0", 11f, TextAlignmentOptions.Center, GameDesignConstants.TextMuted, new Vector2(420f, 25f), new Vector2(0f, -135f));
        versionText.anchorMin = new Vector2(0.5f, 1f);
        versionText.anchorMax = new Vector2(0.5f, 1f);
        versionText.pivot = new Vector2(0.5f, 1f);

        // Buttons Group (inside glass card, centered vertically)
        var buttonsGroupObj = new GameObject("ButtonsGroup");
        buttonsGroupObj.transform.SetParent(glassCard, false);
        var buttonsGroupCG = buttonsGroupObj.AddComponent<CanvasGroup>();
        var buttonsGroupRect = buttonsGroupObj.AddComponent<RectTransform>();
        buttonsGroupRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonsGroupRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonsGroupRect.sizeDelta = new Vector2(340f, 300f);
        buttonsGroupRect.anchoredPosition = new Vector2(0f, -40f);

        var newGameBtn = CreateStylizedButton("Btn_NewGame", buttonsGroupObj.transform, "⚡  NEW GAME", StylizedButton.ButtonVariant.Primary, new Vector2(300f, 52f), new Vector2(0f, 90f));
        var continueBtn = CreateStylizedButton("Btn_Continue", buttonsGroupObj.transform, "▶  CONTINUE", StylizedButton.ButtonVariant.Secondary, new Vector2(300f, 52f), new Vector2(0f, 25f));
        var settingsBtn = CreateStylizedButton("Btn_Settings", buttonsGroupObj.transform, "⚙  SETTINGS", StylizedButton.ButtonVariant.Secondary, new Vector2(300f, 52f), new Vector2(0f, -40f));
        var quitBtn = CreateStylizedButton("Btn_Quit", buttonsGroupObj.transform, "✕  QUIT", StylizedButton.ButtonVariant.Danger, new Vector2(300f, 52f), new Vector2(0f, -105f));

        // New Game Panel
        var newGamePanelObj = new GameObject("NewGamePanel");
        newGamePanelObj.transform.SetParent(canvasObject.transform, false);
        var newGamePanelCG = newGamePanelObj.AddComponent<CanvasGroup>();
        newGamePanelCG.alpha = 0f;
        newGamePanelCG.blocksRaycasts = false;
        newGamePanelCG.interactable = false;
        var newGamePanelRect = newGamePanelObj.AddComponent<RectTransform>();
        newGamePanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        newGamePanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        newGamePanelRect.sizeDelta = new Vector2(500f, 520f);
        newGamePanelRect.anchoredPosition = Vector2.zero;

        // Card bg
        var ngBg = CreatePremiumUiPanel("CardBG", newGamePanelObj.transform, new Vector2(500f, 350f), Vector2.zero, GameDesignConstants.SurfaceCard, new Color(1f, 1f, 1f, 0.08f));
        
        var ngTitle = CreateTMPText("NewGameTitle", newGamePanelObj.transform, "START NEW FOUNDRY", 24f, TextAlignmentOptions.Center, GameDesignConstants.BrandAccent, new Vector2(400f, 40f), new Vector2(0f, 120f));
        ngTitle.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        CreateTMPText("CompanyNameLabel", newGamePanelObj.transform, "Name your AI lab:", 16f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(400f, 30f), new Vector2(0f, 60f));
        
        var companyInput = CreateInputField("CompanyNameInputField", newGamePanelObj.transform, "OpenAI-like name...", new Vector2(400f, 45f), new Vector2(0f, 15f));
        
        var previewTxt = CreateTMPText("CompanyPreviewText", newGamePanelObj.transform, "Future Lab, Inc.", 14f, TextAlignmentOptions.Center, GameDesignConstants.BrandSecondary, new Vector2(400f, 30f), new Vector2(0f, -30f));
        previewTxt.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Italic;

        var identityControlsRootObj = new GameObject("CompanyIdentityControls");
        identityControlsRootObj.transform.SetParent(newGamePanelObj.transform, false);
        var identityControlsRoot = identityControlsRootObj.AddComponent<RectTransform>();
        identityControlsRoot.anchorMin = new Vector2(0.5f, 0.5f);
        identityControlsRoot.anchorMax = new Vector2(0.5f, 0.5f);
        identityControlsRoot.pivot = new Vector2(0.5f, 0.5f);
        identityControlsRoot.sizeDelta = new Vector2(420f, 190f);
        identityControlsRoot.anchoredPosition = new Vector2(0f, -85f);

        var startBtn = CreateStylizedButton("Btn_ConfirmNewGame", newGamePanelObj.transform, "Launch Lab", StylizedButton.ButtonVariant.Primary, new Vector2(180f, 45f), new Vector2(-105f, -190f));
        var cancelBtn = CreateStylizedButton("Btn_CancelNewGame", newGamePanelObj.transform, "Cancel", StylizedButton.ButtonVariant.Secondary, new Vector2(180f, 45f), new Vector2(105f, -190f));

        // Settings Panel
        var settingsPanelObj = new GameObject("SettingsPanel");
        settingsPanelObj.transform.SetParent(canvasObject.transform, false);
        var settingsPanelCG = settingsPanelObj.AddComponent<CanvasGroup>();
        settingsPanelCG.alpha = 0f;
        settingsPanelCG.blocksRaycasts = false;
        settingsPanelCG.interactable = false;
        var settingsPanelRect = settingsPanelObj.AddComponent<RectTransform>();
        settingsPanelRect.anchorMin = new Vector2(0.5f, 0.5f);
        settingsPanelRect.anchorMax = new Vector2(0.5f, 0.5f);
        settingsPanelRect.sizeDelta = new Vector2(500f, 300f);
        settingsPanelRect.anchoredPosition = Vector2.zero;

        CreatePremiumUiPanel("CardBG", settingsPanelObj.transform, new Vector2(500f, 300f), Vector2.zero, GameDesignConstants.SurfaceCard, new Color(1f, 1f, 1f, 0.08f));
        CreateTMPText("SettingsTitle", settingsPanelObj.transform, "SETTINGS", 24f, TextAlignmentOptions.Center, GameDesignConstants.BrandPrimary, new Vector2(400f, 40f), new Vector2(0f, 100f));
        CreateTMPText("SettingsContent", settingsPanelObj.transform, "Resolution: 1920x1080 (Native)\nInput Mode: Default keyboard/mouse", 16f, TextAlignmentOptions.Center, GameDesignConstants.TextSecondary, new Vector2(400f, 100f), new Vector2(0f, 10f));

        var backSettingsBtn = CreateStylizedButton("Btn_BackSettings", settingsPanelObj.transform, "Back", StylizedButton.ButtonVariant.Secondary, new Vector2(180f, 45f), new Vector2(0f, -80f));

        // Fade Overlay
        var fadeObj = new GameObject("FadeOverlay");
        fadeObj.transform.SetParent(canvasObject.transform, false);
        var fadeImg = fadeObj.AddComponent<Image>();
        fadeImg.color = GameDesignConstants.SurfaceDarkest;
        var fadeRect = fadeObj.GetComponent<RectTransform>();
        fadeRect.anchorMin = Vector2.zero;
        fadeRect.anchorMax = Vector2.one;
        fadeRect.offsetMin = Vector2.zero;
        fadeRect.offsetMax = Vector2.zero;

        // Hook up MainMenuController serialized fields
        var serialized = new SerializedObject(menuController);
        serialized.FindProperty("titleGroup").objectReferenceValue = titleGroupCG;
        serialized.FindProperty("buttonsGroup").objectReferenceValue = buttonsGroupCG;
        serialized.FindProperty("newGamePanel").objectReferenceValue = newGamePanelCG;
        serialized.FindProperty("settingsPanel").objectReferenceValue = settingsPanelCG;
        serialized.FindProperty("fadeOverlay").objectReferenceValue = fadeImg;
        serialized.FindProperty("titleText").objectReferenceValue = titleText.GetComponent<TextMeshProUGUI>();
        serialized.FindProperty("subtitleText").objectReferenceValue = subtitleText.GetComponent<TextMeshProUGUI>();
        serialized.FindProperty("versionText").objectReferenceValue = versionText.GetComponent<TextMeshProUGUI>();
        serialized.FindProperty("newGameButton").objectReferenceValue = newGameBtn.GetComponent<Button>();
        serialized.FindProperty("continueButton").objectReferenceValue = continueBtn.GetComponent<Button>();
        serialized.FindProperty("settingsButton").objectReferenceValue = settingsBtn.GetComponent<Button>();
        serialized.FindProperty("quitButton").objectReferenceValue = quitBtn.GetComponent<Button>();
        serialized.FindProperty("companyNameInput").objectReferenceValue = companyInput;
        serialized.FindProperty("confirmNewGameButton").objectReferenceValue = startBtn.GetComponent<Button>();
        serialized.FindProperty("cancelNewGameButton").objectReferenceValue = cancelBtn.GetComponent<Button>();
        serialized.FindProperty("companyPreviewText").objectReferenceValue = previewTxt.GetComponent<TextMeshProUGUI>();
        serialized.FindProperty("identityControlsRoot").objectReferenceValue = identityControlsRoot;
        serialized.FindProperty("settingsBackButton").objectReferenceValue = backSettingsBtn.GetComponent<Button>();
        serialized.ApplyModifiedPropertiesWithoutUndo();

        // Also add GameManager to the Main Menu scene so it exists initially
        var gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        gmObj.AddComponent<CompetitorManager>();
        gmObj.AddComponent<TeamSimulationManager>();
        gmObj.AddComponent<TechPulseFeed>();
        gmObj.AddComponent<TechPulseFollowerSystem>();

        // Wire up back settings button manually
        var backBtn = backSettingsBtn.GetComponent<Button>();
        // We do it by serializing the Settings Panel back action
        var menuSerialized = new SerializedObject(menuController);
        // Bind settings panel Cancel settings action
        // Actually, MainMenuController has SetupButtons which binds click handlers for sub-panels.
        // Let's verify MainMenuController.cs to check how backSettingsBtn is wired or if we can wire it to the cancel button/close settings.
        // Ah, in MainMenuController, we see on line 74: `confirmNewGameButton` and `cancelNewGameButton` are registered.
        // Wait, did we map `backSettingsBtn`? No, let's see. In MainMenuController, backSettingsBtn is not serialized.
        // Oh! How does the settings panel close in MainMenuController?
        // Let's check MainMenuController.cs setting click handler:
        // Actually, let's check line 28 of MainMenuController: it has settingsButton, settingsPanel.
        // Let's check if MainMenuController has a back button serializable field. Let's do a search.
        // In MainMenuController, we see:
        //   [Header("Settings")]
        EditorSceneManager.SaveScene(scene, MainMenuScenePath);
    }

    // ── Build Gameplay Scene ──────────────────────────────────────────
    private static void BuildGameplayScene()
    {
        if (File.Exists(GameplayScenePath))
        {
            var existingScene = EditorSceneManager.OpenScene(GameplayScenePath, OpenSceneMode.Single);
            ApplyRealisticRefactorUi(existingScene);
            EditorSceneManager.SaveScene(existingScene, GameplayScenePath);
            Debug.Log($"Model Foundry: Migrated existing Gameplay scene at {GameplayScenePath} to the realistic refactor UI.");
            return;
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        scene.name = "GaragePrototype";

        ConvertPolygonMaterialsToUrp();
        ConvertIconsToSprites();

        BuildOfficeDiorama();
        var workstation = CreateWorkstation("Founder Workstation", new Vector3(-1.1f, 0.8f, 0.2f), new Vector3(-1.1f, 0f, -0.95f), new Vector3(-1.1f, 0f, -0.55f));
        var founderAgent = CreateFounderAgent(workstation);
        var workstation2 = CreateWorkstation("Helper Workstation", new Vector3(1.1f, 0.8f, 0.2f), new Vector3(1.1f, 0f, -0.95f), new Vector3(1.1f, 0f, -0.55f));

        // ── Lighting & Scene Atmosphere ─────────────────────────────
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight = new Color(0.55f, 0.55f, 0.58f); // clean neutral ambient daylight

        // Key Light (warm solar daylight)
        var light = new GameObject("Key Light");
        var lightComponent = light.AddComponent<Light>();
        lightComponent.type = LightType.Directional;
        lightComponent.intensity = 1.5f;
        lightComponent.color = new Color(1f, 0.96f, 0.9f); // warm solar white
        lightComponent.shadows = LightShadows.Soft;
        lightComponent.shadowStrength = 0.65f;
        lightComponent.shadowBias = 0.04f;
        light.transform.rotation = Quaternion.Euler(45f, -40f, 0f);

        // Fill Light (soft sky reflection fill)
        var fillLight = new GameObject("Fill Light");
        var fillLightComponent = fillLight.AddComponent<Light>();
        fillLightComponent.type = LightType.Directional;
        fillLightComponent.intensity = 0.6f;
        fillLightComponent.color = new Color(0.85f, 0.88f, 0.92f); // soft blue-grey sky reflection
        fillLight.transform.rotation = Quaternion.Euler(20f, 140f, 0f);

        // Camera (Isometric with Controller)
        var cameraObject = new GameObject("Isometric Camera");
        var camera = cameraObject.AddComponent<Camera>();
        camera.orthographic = true;
        camera.orthographicSize = 6.2f;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = GameDesignConstants.SurfaceDarkest;
        cameraObject.transform.position = new Vector3(7f, 7f, -7f);
        cameraObject.transform.rotation = Quaternion.Euler(35f, -45f, 0f);
        camera.tag = "MainCamera";
        
        // Camera Controller
        cameraObject.AddComponent<IsometricCameraController>();

        // Managers
        var gmObj = new GameObject("GameManager");
        gmObj.AddComponent<GameManager>();
        gmObj.AddComponent<CompetitorManager>();
        gmObj.AddComponent<TechPulseFeed>();
        gmObj.AddComponent<TechPulseFollowerSystem>();

        var timeObj = new GameObject("TimeController");
        timeObj.AddComponent<TimeController>();

        // UI Event System
        var eventSystem = new GameObject("EventSystem");
        eventSystem.AddComponent<EventSystem>();
        eventSystem.AddComponent<InputSystemUIInputModule>();

        // UI Canvas
        var canvasObject = new GameObject("HUD Canvas");
        var canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        canvasObject.AddComponent<GraphicRaycaster>();

        var hudCG = canvasObject.AddComponent<CanvasGroup>();
        var hudController = canvasObject.AddComponent<HUDController>();

        // ── FLOATING TOP BAR ─────────────────────────────────────────
        var topBar = CreatePremiumUiPanel("TopBar", canvasObject.transform, new Vector2(1872f, 60f), new Vector2(0f, -12f), GameDesignConstants.SurfaceCard, new Color(1f, 1f, 1f, 0.08f));
        topBar.anchorMin = new Vector2(0f, 1f);
        topBar.anchorMax = new Vector2(1f, 1f);
        topBar.pivot = new Vector2(0.5f, 1f);
        topBar.offsetMin = new Vector2(24f, -72f);
        topBar.offsetMax = new Vector2(-24f, -12f);

        var companyIconRect = CreateUiRect("CompanyIcon", topBar, new Vector2(34f, 34f), new Vector2(18f, 0f), Color.white);
        companyIconRect.anchorMin = new Vector2(0f, 0.5f);
        companyIconRect.anchorMax = new Vector2(0f, 0.5f);
        companyIconRect.pivot = new Vector2(0f, 0.5f);
        var companyIconImage = companyIconRect.GetComponent<Image>();
        companyIconImage.preserveAspect = true;

        var companyTextRect = CreateTMPText("CompanyText", topBar, "MODEL FOUNDRY", 18f, TextAlignmentOptions.Left, GameDesignConstants.BrandSecondary, new Vector2(360f, 40f), new Vector2(62f, 0f));
        companyTextRect.anchorMin = new Vector2(0f, 0.5f);
        companyTextRect.anchorMax = new Vector2(0f, 0.5f);
        companyTextRect.pivot = new Vector2(0f, 0.5f);
        companyTextRect.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var dateTextRect = CreateTMPText("DateText", topBar, "January 1, 2017", 18f, TextAlignmentOptions.Center, GameDesignConstants.TextPrimary, new Vector2(400f, 40f), Vector2.zero);
        dateTextRect.anchorMin = new Vector2(0.5f, 0.5f);
        dateTextRect.anchorMax = new Vector2(0.5f, 0.5f);
        dateTextRect.pivot = new Vector2(0.5f, 0.5f);

        var speedTextRect = CreateTMPText("SpeedText", topBar, "Normal", 15f, TextAlignmentOptions.Right, GameDesignConstants.TextSecondary, new Vector2(150f, 40f), new Vector2(-210f, 0f));
        speedTextRect.anchorMin = new Vector2(1f, 0.5f);
        speedTextRect.anchorMax = new Vector2(1f, 0.5f);
        speedTextRect.pivot = new Vector2(1f, 0.5f);

        // Speed Buttons Container
        var speedContainer = new GameObject("SpeedButtons");
        speedContainer.transform.SetParent(topBar, false);
        var speedContainerRect = speedContainer.AddComponent<RectTransform>();
        speedContainerRect.anchorMin = new Vector2(1f, 0.5f);
        speedContainerRect.anchorMax = new Vector2(1f, 0.5f);
        speedContainerRect.pivot = new Vector2(1f, 0.5f);
        speedContainerRect.sizeDelta = new Vector2(180f, 40f);
        speedContainerRect.anchoredPosition = new Vector2(-20f, 0f);

        var pauseBtn = CreateSpeedButton("PauseBtn", speedContainer.transform, "||", new Vector2(30f, 30f), new Vector2(-75f, 0f));
        var normalBtn = CreateSpeedButton("NormalBtn", speedContainer.transform, ">", new Vector2(30f, 30f), new Vector2(-25f, 0f));
        var fastBtn = CreateSpeedButton("FastBtn", speedContainer.transform, ">>", new Vector2(30f, 30f), new Vector2(25f, 0f));
        var ultraBtn = CreateSpeedButton("UltraBtn", speedContainer.transform, ">>>", new Vector2(30f, 30f), new Vector2(75f, 0f));

        var pauseHighlight = pauseBtn.transform.Find("Highlight").GetComponent<Image>();
        var normalHighlight = normalBtn.transform.Find("Highlight").GetComponent<Image>();
        var fastHighlight = fastBtn.transform.Find("Highlight").GetComponent<Image>();
        var ultraHighlight = ultraBtn.transform.Find("Highlight").GetComponent<Image>();

        // ── FLOATING RESOURCE PILLS (TOP-CENTER) ─────────────────────
        var resourcePanelObj = new GameObject("ResourcePillsContainer");
        resourcePanelObj.transform.SetParent(canvasObject.transform, false);
        var resourcePanel = resourcePanelObj.AddComponent<RectTransform>();
        resourcePanel.anchorMin = new Vector2(0.5f, 1f);
        resourcePanel.anchorMax = new Vector2(0.5f, 1f);
        resourcePanel.pivot = new Vector2(0.5f, 1f);
        resourcePanel.sizeDelta = new Vector2(960f, 40f);
        resourcePanel.anchoredPosition = new Vector2(0f, -90f);
        var resourcePanelImg = resourcePanelObj.AddComponent<Image>();
        resourcePanelImg.color = Color.clear;

        // Bake and load the actual 3D icons from PolygonIcons as 2D sprites
        Sprite iconCash = BakePrefabToSprite("Assets/Synty/PolygonIcons/Prefabs/SM_Icon_Coin_01.prefab", "icon_cash");
        Sprite iconRep = BakePrefabToSprite("Assets/Synty/PolygonIcons/Prefabs/SM_Icon_Crown_01.prefab", "icon_reputation");
        Sprite iconQual = BakePrefabToSprite("Assets/Synty/PolygonIcons/Prefabs/SM_Icon_Computer_CPU_01.prefab", "icon_quality");
        Sprite iconTeam = BakePrefabToSprite("Assets/Synty/PolygonIcons/Prefabs/SM_Icon_Face_Male_01.prefab", "icon_team");

        var pillWidth = 145f;
        var pillHeight = 32f;
        var spacing = 10f;
        var startX = -((pillWidth * 6f + spacing * 5f) / 2f) + (pillWidth / 2f);

        // Pill 1: Cash
        var cashBar = CreateResourceBar("CashBar", resourcePanel, "Cash", GameDesignConstants.ResourceCash, "$", "", 100000f, new Vector2(pillWidth, pillHeight), new Vector2(startX + 0 * (pillWidth + spacing), 0f), iconCash);

        // Pill 2: Runway (Custom Pill Text)
        var runwayPill = CreateUiPill("RunwayPill", resourcePanel, "Runway: -- mo", iconTeam, GameDesignConstants.ResourceTeam, new Vector2(pillWidth, pillHeight), new Vector2(startX + 1 * (pillWidth + spacing), 0f));
        var runwayTxt = runwayPill.transform.Find("Border/InnerBackground/LabelText").GetComponent<TextMeshProUGUI>();

        // Pill 3: Reputation
        var repBar = CreateResourceBar("ReputationBar", resourcePanel, "Reputation", GameDesignConstants.ResourceReputation, "", "%", 100f, new Vector2(pillWidth, pillHeight), new Vector2(startX + 2 * (pillWidth + spacing), 0f), iconRep);

        // Pill 4: Quality
        var qualBar = CreateResourceBar("QualityBar", resourcePanel, "Model Quality", GameDesignConstants.ResourceQuality, "", "%", 100f, new Vector2(pillWidth, pillHeight), new Vector2(startX + 3 * (pillWidth + spacing), 0f), iconQual);

        // Pill 5: NOC Load / Grid Status (Battery/CPU icon)
        var nocPill = CreateUiPill("NocPill", resourcePanel, "Grid: --%", iconQual, GameDesignConstants.BrandSecondary, new Vector2(pillWidth, pillHeight), new Vector2(startX + 4 * (pillWidth + spacing), 0f));
        var nocTxt = nocPill.transform.Find("Border/InnerBackground/LabelText").GetComponent<TextMeshProUGUI>();

        // Pill 6: Op. Cost (Burn Rate)
        var burnPill = CreateUiPill("BurnPill", resourcePanel, "Op. Cost: --", iconRep, GameDesignConstants.StatusDanger, new Vector2(pillWidth, pillHeight), new Vector2(startX + 5 * (pillWidth + spacing), 0f));
        var burnTxt = burnPill.transform.Find("Border/InnerBackground/LabelText").GetComponent<TextMeshProUGUI>();

        // Hidden Team and Competence bars to preserve HUDController refs
        var hiddenPanel = new GameObject("HiddenResourceBars");
        hiddenPanel.transform.SetParent(resourcePanel, false);
        hiddenPanel.SetActive(false);
        var teamBar = CreateResourceBar("TeamBar", hiddenPanel.transform, "Team Size", GameDesignConstants.ResourceTeam, "", " ML", 20f, new Vector2(10f, 10f), Vector2.zero, iconTeam);
        var competenceBar = CreateResourceBar("CompetenceBar", hiddenPanel.transform, "Competence", GameDesignConstants.BrandAccent, "", "%", 100f, new Vector2(10f, 10f), Vector2.zero, null);

        // ── OPERATIONAL TERMINAL (BOTTOM SUMMARY) ────────────────────
        var summaryPanel = CreatePremiumUiPanel("SummaryPanel", canvasObject.transform, new Vector2(320f, 240f), new Vector2(24f, 80f), GameDesignConstants.SurfaceMid, new Color(0.82f, 0.84f, 0.87f));
        summaryPanel.anchorMin = new Vector2(0f, 0f);
        summaryPanel.anchorMax = new Vector2(0f, 0f);
        summaryPanel.pivot = new Vector2(0f, 0f);

        CreateTMPText("SummaryTitle", summaryPanel, "▲  OPERATIONAL TERMINAL", 13f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(280f, 20f), new Vector2(16f, 100f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
 
        var sumCloseBtn = CreateStylizedButton("Btn_CloseSummary", summaryPanel, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, 100f));
        sumCloseBtn.anchorMin = new Vector2(1f, 0.5f);
        sumCloseBtn.anchorMax = new Vector2(1f, 0.5f);
        sumCloseBtn.pivot = new Vector2(1f, 0.5f);
        sumCloseBtn.anchoredPosition = new Vector2(-20f, 100f);

        var sumRunwayTxt = CreateTMPText("RunwayText", summaryPanel, "Runway: -- months", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextPrimary, new Vector2(280f, 20f), new Vector2(16f, 70f));
        var sumBurnTxt = CreateTMPText("BurnText", summaryPanel, "Burn Rate: $800/mo", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(280f, 20f), new Vector2(16f, 44f));
        var revTxt = CreateTMPText("RevenueText", summaryPanel, "Revenue: $0/mo", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(280f, 20f), new Vector2(16f, 18f));
        var clientsTxt = CreateTMPText("ClientsText", summaryPanel, "Active Clients: 0", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(280f, 20f), new Vector2(16f, -8f));

        var hireBtn = CreateStylizedButton("Btn_Hire", summaryPanel, "HIRE ML ($5k)", StylizedButton.ButtonVariant.Primary, new Vector2(136f, 40f), new Vector2(-73f, -70f));
        hireBtn.anchorMin = new Vector2(0.5f, 0.5f); hireBtn.anchorMax = new Vector2(0.5f, 0.5f); hireBtn.pivot = new Vector2(0.5f, 0.5f); hireBtn.anchoredPosition = new Vector2(-73f, -70f);

        var buyGpuBtn = CreateStylizedButton("Btn_BuyGpu", summaryPanel, "BUY GPU ($10k)", StylizedButton.ButtonVariant.Primary, new Vector2(136f, 40f), new Vector2(73f, -70f));
        buyGpuBtn.anchorMin = new Vector2(0.5f, 0.5f); buyGpuBtn.anchorMax = new Vector2(0.5f, 0.5f); buyGpuBtn.pivot = new Vector2(0.5f, 0.5f); buyGpuBtn.anchoredPosition = new Vector2(73f, -70f);
 
        summaryPanel.gameObject.SetActive(false);

        // ── BOTTOM ACTION BAR (CARD BUTTONS) ─────────────────────────
        var bottomBar = CreatePremiumUiPanel("BottomActionBar", canvasObject.transform, new Vector2(800f, 60f), new Vector2(0f, 20f), GameDesignConstants.SurfaceLight, new Color(0.82f, 0.84f, 0.87f));
        bottomBar.anchorMin = new Vector2(0.5f, 0f);
        bottomBar.anchorMax = new Vector2(0.5f, 0f);
        bottomBar.pivot = new Vector2(0.5f, 0f);

        var bHireBtn = CreateStylizedButton("Btn_BottomHire", bottomBar, "+ Hire Staff", StylizedButton.ButtonVariant.Secondary, new Vector2(180f, 44f), new Vector2(-288f, 0f));
        bHireBtn.anchorMin = new Vector2(0.5f, 0.5f); bHireBtn.anchorMax = new Vector2(0.5f, 0.5f); bHireBtn.pivot = new Vector2(0.5f, 0.5f); bHireBtn.anchoredPosition = new Vector2(-288f, 0f);

        var bStartBtn = CreateStylizedButton("Btn_BottomStartProject", bottomBar, "▶ Start Project", StylizedButton.ButtonVariant.Secondary, new Vector2(180f, 44f), new Vector2(-96f, 0f));
        bStartBtn.anchorMin = new Vector2(0.5f, 0.5f); bStartBtn.anchorMax = new Vector2(0.5f, 0.5f); bStartBtn.pivot = new Vector2(0.5f, 0.5f); bStartBtn.anchoredPosition = new Vector2(-96f, 0f);

        var bUpgradeBtn = CreateStylizedButton("Btn_BottomUpgrade", bottomBar, "⚡ Upgrade Infra", StylizedButton.ButtonVariant.Secondary, new Vector2(180f, 44f), new Vector2(96f, 0f));
        bUpgradeBtn.anchorMin = new Vector2(0.5f, 0.5f); bUpgradeBtn.anchorMax = new Vector2(0.5f, 0.5f); bUpgradeBtn.pivot = new Vector2(0.5f, 0.5f); bUpgradeBtn.anchoredPosition = new Vector2(96f, 0f);

        var bSpeedBtn = CreateStylizedButton("Btn_BottomSpeedUp", bottomBar, "⏱ Speed Up / Ops", StylizedButton.ButtonVariant.Secondary, new Vector2(180f, 44f), new Vector2(288f, 0f));
        bSpeedBtn.anchorMin = new Vector2(0.5f, 0.5f); bSpeedBtn.anchorMax = new Vector2(0.5f, 0.5f); bSpeedBtn.pivot = new Vector2(0.5f, 0.5f); bSpeedBtn.anchoredPosition = new Vector2(288f, 0f);

        // Hook up HUDController
        var serializedHUD = new SerializedObject(hudController);
        serializedHUD.FindProperty("companyNameText").objectReferenceValue = companyTextRect.GetComponent<TextMeshProUGUI>();
        serializedHUD.FindProperty("companyIconImage").objectReferenceValue = companyIconImage;
        serializedHUD.FindProperty("dateText").objectReferenceValue = dateTextRect.GetComponent<TextMeshProUGUI>();
        serializedHUD.FindProperty("speedText").objectReferenceValue = speedTextRect.GetComponent<TextMeshProUGUI>();
        serializedHUD.FindProperty("topBarBackground").objectReferenceValue = (topBar.Find("InnerBackground") != null ? topBar.Find("InnerBackground").GetComponent<Image>() : topBar.GetComponent<Image>());
        serializedHUD.FindProperty("pauseButton").objectReferenceValue = pauseBtn.GetComponent<Button>();
        serializedHUD.FindProperty("normalButton").objectReferenceValue = normalBtn.GetComponent<Button>();
        serializedHUD.FindProperty("fastButton").objectReferenceValue = fastBtn.GetComponent<Button>();
        serializedHUD.FindProperty("ultraButton").objectReferenceValue = ultraBtn.GetComponent<Button>();
        serializedHUD.FindProperty("pauseHighlight").objectReferenceValue = pauseHighlight;
        serializedHUD.FindProperty("normalHighlight").objectReferenceValue = normalHighlight;
        serializedHUD.FindProperty("fastHighlight").objectReferenceValue = fastHighlight;
        serializedHUD.FindProperty("ultraHighlight").objectReferenceValue = ultraHighlight;
        serializedHUD.FindProperty("cashBar").objectReferenceValue = cashBar;
        serializedHUD.FindProperty("reputationBar").objectReferenceValue = repBar;
        serializedHUD.FindProperty("qualityBar").objectReferenceValue = qualBar;
        serializedHUD.FindProperty("teamBar").objectReferenceValue = teamBar;
        serializedHUD.FindProperty("competenceBar").objectReferenceValue = competenceBar;
        serializedHUD.FindProperty("resourcePanelBackground").objectReferenceValue = resourcePanelImg;
        serializedHUD.FindProperty("monthlyBurnText").objectReferenceValue = burnTxt;
        serializedHUD.FindProperty("monthlyRevenueText").objectReferenceValue = revTxt;
        serializedHUD.FindProperty("runwayText").objectReferenceValue = runwayTxt;
        serializedHUD.FindProperty("clientsText").objectReferenceValue = clientsTxt.GetComponent<TextMeshProUGUI>();
        serializedHUD.FindProperty("nocText").objectReferenceValue = nocTxt;
        serializedHUD.FindProperty("bottomHireButton").objectReferenceValue = bHireBtn.GetComponent<Button>();
        serializedHUD.FindProperty("bottomStartProjectButton").objectReferenceValue = bStartBtn.GetComponent<Button>();
        serializedHUD.FindProperty("bottomUpgradeButton").objectReferenceValue = bUpgradeBtn.GetComponent<Button>();
        serializedHUD.FindProperty("bottomSpeedUpButton").objectReferenceValue = bSpeedBtn.GetComponent<Button>();
        serializedHUD.FindProperty("hudCanvasGroup").objectReferenceValue = hudCG;

        // ── FLOATING LEFT DOCK ICON STRIP ─────────────────────────────
        var dockStrip = CreatePremiumUiPanel("LeftDockStrip", canvasObject.transform, new Vector2(60f, 560f), new Vector2(24f, 0f), GameDesignConstants.SurfaceMid, new Color(0.82f, 0.84f, 0.87f));
        dockStrip.anchorMin = new Vector2(0f, 0.5f);
        dockStrip.anchorMax = new Vector2(0f, 0.5f);
        dockStrip.pivot = new Vector2(0f, 0.5f);

        var dockTechBtn = CreateStylizedButton("Dock_TechPulse", dockStrip, "T", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, 220f));
        var dockResearchBtn = CreateStylizedButton("Dock_Research", dockStrip, "R", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, 165f));
        var dockAnalyticsBtn = CreateStylizedButton("Dock_Analytics", dockStrip, "A", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, 110f));
        var dockHireBtn = CreateStylizedButton("Dock_Hire", dockStrip, "H", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, 55f));
        var dockGpuBtn = CreateStylizedButton("Dock_GPU", dockStrip, "G", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, 0f));
        var dockContractsBtn = CreateStylizedButton("Dock_Contracts", dockStrip, "C", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, -55f));
        var dockBoardRoomBtn = CreateStylizedButton("Dock_BoardRoom", dockStrip, "B", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, -110f));
        var dockNocBtn = CreateStylizedButton("Dock_Noc", dockStrip, "N", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, -165f));
        var dockSystemBtn = CreateStylizedButton("Dock_System", dockStrip, "M", StylizedButton.ButtonVariant.Secondary, new Vector2(44f, 44f), new Vector2(0f, -220f));

        // Dock icon highlights
        var dockTechHl = dockTechBtn.Find("Highlight") != null ? dockTechBtn.Find("Highlight").GetComponent<Image>() : null;
        if (dockTechHl == null) { var hl = new GameObject("Highlight"); hl.transform.SetParent(dockTechBtn, false); dockTechHl = hl.AddComponent<Image>(); dockTechHl.color = Color.clear; var hlRt = hl.GetComponent<RectTransform>(); hlRt.anchorMin = Vector2.zero; hlRt.anchorMax = Vector2.one; hlRt.offsetMin = Vector2.zero; hlRt.offsetMax = Vector2.zero; }
        var dockResearchHl = CreateDockHighlight(dockResearchBtn);
        var dockAnalyticsHl = CreateDockHighlight(dockAnalyticsBtn);
        var dockHireHl = CreateDockHighlight(dockHireBtn);
        var dockGpuHl = CreateDockHighlight(dockGpuBtn);
        var dockContractsHl = CreateDockHighlight(dockContractsBtn);
        var dockBoardRoomHl = CreateDockHighlight(dockBoardRoomBtn);
        var dockNocHl = CreateDockHighlight(dockNocBtn);
        var dockSystemHl = CreateDockHighlight(dockSystemBtn);

        serializedHUD.FindProperty("techPulseButton").objectReferenceValue = dockTechBtn.GetComponent<Button>();
        serializedHUD.FindProperty("researchButton").objectReferenceValue = dockResearchBtn.GetComponent<Button>();
        serializedHUD.FindProperty("analyticsButton").objectReferenceValue = dockAnalyticsBtn.GetComponent<Button>();
        serializedHUD.FindProperty("hiringButton").objectReferenceValue = dockHireBtn.GetComponent<Button>();
        serializedHUD.FindProperty("gpuUpgradeButton").objectReferenceValue = dockGpuBtn.GetComponent<Button>();
        serializedHUD.FindProperty("boardRoomButton").objectReferenceValue = dockBoardRoomBtn.GetComponent<Button>();
        serializedHUD.FindProperty("systemButton").objectReferenceValue = dockSystemBtn.GetComponent<Button>();
        serializedHUD.FindProperty("contractsButton").objectReferenceValue = dockContractsBtn.GetComponent<Button>();
        serializedHUD.FindProperty("nocButton").objectReferenceValue = dockNocBtn.GetComponent<Button>();

        serializedHUD.FindProperty("researchIconHighlight").objectReferenceValue = dockResearchHl;
        serializedHUD.FindProperty("analyticsIconHighlight").objectReferenceValue = dockAnalyticsHl;
        serializedHUD.FindProperty("hiringIconHighlight").objectReferenceValue = dockHireHl;
        serializedHUD.FindProperty("gpuIconHighlight").objectReferenceValue = dockGpuHl;
        serializedHUD.FindProperty("boardRoomIconHighlight").objectReferenceValue = dockBoardRoomHl;
        serializedHUD.FindProperty("systemIconHighlight").objectReferenceValue = dockSystemHl;
        serializedHUD.FindProperty("contractsIconHighlight").objectReferenceValue = dockContractsHl;
        serializedHUD.FindProperty("nocIconHighlight").objectReferenceValue = dockNocHl;

        serializedHUD.ApplyModifiedPropertiesWithoutUndo();

        // Variables used by later serialization
        var techPulseBtn = dockTechBtn;

        // ── PROJECT PANEL ────────────────────────────────────────────
        var projectPanel = CreatePremiumUiPanel("ProjectPanel", canvasObject.transform, new Vector2(380f, 440f), new Vector2(-24f, 0f), GameDesignConstants.SurfaceMid, new Color(0.82f, 0.84f, 0.87f));
        projectPanel.anchorMin = new Vector2(1f, 0.5f);
        projectPanel.anchorMax = new Vector2(1f, 0.5f);
        projectPanel.pivot = new Vector2(1f, 0.5f);

        // Teal accent bar at the top of the card
        var projAccentBar = CreateUiRect("AccentBar", projectPanel, new Vector2(380f, 6f), new Vector2(0f, 217f), GameDesignConstants.BrandPrimary);

        var headerBg = CreateUiRect("HeaderBg", projectPanel, new Vector2(380f, 50f), new Vector2(0f, 195f), GameDesignConstants.SurfaceLight);
        var projTitle = CreateTMPText("ProjectTitle", projectPanel, "SupportBot v1.0", 18f, TextAlignmentOptions.Left, GameDesignConstants.TextPrimary, new Vector2(340f, 30f), new Vector2(20f, 195f));
        projTitle.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var projCloseBtn = CreateStylizedButton("Btn_CloseProj", projectPanel, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, 195f));
        projCloseBtn.anchorMin = new Vector2(1f, 0.5f);
        projCloseBtn.anchorMax = new Vector2(1f, 0.5f);
        projCloseBtn.pivot = new Vector2(1f, 0.5f);
        projCloseBtn.anchoredPosition = new Vector2(-20f, 195f);

        var nameInput = CreateInputField("ModelNameInputField", projectPanel, "Model Name...", new Vector2(340f, 32f), new Vector2(20f, 150f));
        nameInput.GetComponent<RectTransform>().anchorMin = new Vector2(0f, 0.5f);
        nameInput.GetComponent<RectTransform>().anchorMax = new Vector2(0f, 0.5f);
        nameInput.GetComponent<RectTransform>().pivot = new Vector2(0f, 0.5f);
        nameInput.GetComponent<RectTransform>().anchoredPosition = new Vector2(20f, 150f);

        var projDesc = CreateTMPText("ProjectDesc", projectPanel, "Customer support chatbot for e-commerce", 13f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(340f, 40f), new Vector2(20f, 120f));
        
        // Model Selection Buttons
        CreateTMPText("ModelTypeLabel", projectPanel, "MODEL TYPE", 11f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(340f, 20f), new Vector2(20f, 95f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        
        var btnVision = CreateStylizedButton("Btn_ModelVision", projectPanel, "Vision", StylizedButton.ButtonVariant.Secondary, new Vector2(105f, 32f), new Vector2(-115f, 65f));
        var btnNLP = CreateStylizedButton("Btn_ModelNLP", projectPanel, "NLP", StylizedButton.ButtonVariant.Primary, new Vector2(105f, 32f), new Vector2(0f, 65f));
        var btnAgentic = CreateStylizedButton("Btn_ModelAgentic", projectPanel, "Agentic", StylizedButton.ButtonVariant.Secondary, new Vector2(105f, 32f), new Vector2(115f, 65f));

        btnVision.anchorMin = new Vector2(0.5f, 0.5f); btnVision.anchorMax = new Vector2(0.5f, 0.5f); btnVision.pivot = new Vector2(0.5f, 0.5f); btnVision.anchoredPosition = new Vector2(-115f, 65f);
        btnNLP.anchorMin = new Vector2(0.5f, 0.5f); btnNLP.anchorMax = new Vector2(0.5f, 0.5f); btnNLP.pivot = new Vector2(0.5f, 0.5f); btnNLP.anchoredPosition = new Vector2(0f, 65f);
        btnAgentic.anchorMin = new Vector2(0.5f, 0.5f); btnAgentic.anchorMax = new Vector2(0.5f, 0.5f); btnAgentic.pivot = new Vector2(0.5f, 0.5f); btnAgentic.anchoredPosition = new Vector2(115f, 65f);

        // Data Selection Buttons
        CreateTMPText("DataTypeLabel", projectPanel, "DATA SOURCE", 11f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(340f, 20f), new Vector2(20f, 30f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var btnScraped = CreateStylizedButton("Btn_DataScraped", projectPanel, "Scraped (Free)", StylizedButton.ButtonVariant.Primary, new Vector2(165f, 32f), new Vector2(-85f, 0f));
        var btnLicensed = CreateStylizedButton("Btn_DataLicensed", projectPanel, "Licensed ($1.5k)", StylizedButton.ButtonVariant.Secondary, new Vector2(165f, 32f), new Vector2(85f, 0f));

        btnScraped.anchorMin = new Vector2(0.5f, 0.5f); btnScraped.anchorMax = new Vector2(0.5f, 0.5f); btnScraped.pivot = new Vector2(0.5f, 0.5f); btnScraped.anchoredPosition = new Vector2(-85f, 0f);
        btnLicensed.anchorMin = new Vector2(0.5f, 0.5f); btnLicensed.anchorMax = new Vector2(0.5f, 0.5f); btnLicensed.pivot = new Vector2(0.5f, 0.5f); btnLicensed.anchoredPosition = new Vector2(85f, 0f);

        var projStatus = CreateTMPText("StatusText", projectPanel, "Ready to train model...", 14f, TextAlignmentOptions.Left, GameDesignConstants.BrandSecondary, new Vector2(340f, 30f), new Vector2(20f, -45f));
        
        var projCash = CreateTMPText("CashText", projectPanel, "Budget remaining: $25,000", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextPrimary, new Vector2(340f, 25f), new Vector2(20f, -75f));
        var projCostEst = CreateTMPText("CostEstimateText", projectPanel, "Estimated Training Cost: $4,000", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(340f, 25f), new Vector2(20f, -100f));

        // Progress bar
        var progressBack = CreateUiRect("ProgressBack", projectPanel, new Vector2(340f, 20f), new Vector2(20f, -130f), GameDesignConstants.ResourceBarBg);
        var progressFillObj = new GameObject("ProgressFill");
        progressFillObj.transform.SetParent(progressBack, false);
        var progressFill = progressFillObj.AddComponent<Image>();
        progressFill.color = GameDesignConstants.BrandPrimary;
        progressFill.type = Image.Type.Filled;
        progressFill.fillMethod = Image.FillMethod.Horizontal;
        progressFill.fillAmount = 0f;
        var progressFillRect = progressFillObj.GetComponent<RectTransform>();
        progressFillRect.anchorMin = Vector2.zero;
        progressFillRect.anchorMax = Vector2.one;
        progressFillRect.offsetMin = Vector2.zero;
        progressFillRect.offsetMax = Vector2.zero;

        var progressPercent = CreateTMPText("ProgressPercentText", progressBack, "0%", 11f, TextAlignmentOptions.Center, Color.white, new Vector2(100f, 20f), Vector2.zero);

        var startProjectBtn = CreateStylizedButton("Btn_StartProject", projectPanel, "TRAIN MODEL", StylizedButton.ButtonVariant.Primary, new Vector2(340f, 45f), new Vector2(20f, -180f));
        startProjectBtn.anchorMin = new Vector2(0f, 0.5f);
        startProjectBtn.anchorMax = new Vector2(0f, 0.5f);
        startProjectBtn.pivot = new Vector2(0f, 0.5f);
        startProjectBtn.anchoredPosition = new Vector2(20f, -180f);

        projectPanel.gameObject.SetActive(false);

        // ── RIGHT CLICK CONTEXT MENU ─────────────────────────────────
        var contextMenu = CreatePremiumUiPanel("RightClickContextMenu", canvasObject.transform, new Vector2(220f, 172f), Vector2.zero, GameDesignConstants.SurfaceCard, new Color(1f, 1f, 1f, 0.12f));
        var contextRt = contextMenu.GetComponent<RectTransform>();
        contextRt.anchorMin = new Vector2(0.5f, 0.5f);
        contextRt.anchorMax = new Vector2(0.5f, 0.5f);
        contextRt.pivot = new Vector2(0f, 1f);
        contextRt.sizeDelta = new Vector2(220f, 172f);

        var contextLaunchBtn = CreateStylizedButton("Btn_ContextLaunch", contextMenu, "Launch Product", StylizedButton.ButtonVariant.Primary, new Vector2(200f, 32f), new Vector2(10f, -24f));
        contextLaunchBtn.anchorMin = new Vector2(0f, 1f);
        contextLaunchBtn.anchorMax = new Vector2(0f, 1f);
        contextLaunchBtn.pivot = new Vector2(0f, 1f);

        var contextManageBtn = CreateStylizedButton("Btn_ContextManage", contextMenu, "Manage Team & GPUs", StylizedButton.ButtonVariant.Secondary, new Vector2(200f, 32f), new Vector2(10f, -62f));
        contextManageBtn.anchorMin = new Vector2(0f, 1f);
        contextManageBtn.anchorMax = new Vector2(0f, 1f);
        contextManageBtn.pivot = new Vector2(0f, 1f);

        var contextMarketingBtn = CreateStylizedButton("Btn_ContextMarketing", contextMenu, "Run Marketing Campaign", StylizedButton.ButtonVariant.Secondary, new Vector2(200f, 32f), new Vector2(10f, -100f));
        contextMarketingBtn.anchorMin = new Vector2(0f, 1f);
        contextMarketingBtn.anchorMax = new Vector2(0f, 1f);
        contextMarketingBtn.pivot = new Vector2(0f, 1f);

        var contextTeaserBtn = CreateStylizedButton("Btn_ContextTeaser", contextMenu, "Post Teaser Update", StylizedButton.ButtonVariant.Secondary, new Vector2(200f, 32f), new Vector2(10f, -138f));
        contextTeaserBtn.anchorMin = new Vector2(0f, 1f);
        contextTeaserBtn.anchorMax = new Vector2(0f, 1f);
        contextTeaserBtn.pivot = new Vector2(0f, 1f);

        // ── PROJECT RESULT PANEL ──────────────────────────────────────
        var resultPanelObj = new GameObject("ProjectResultPanel");

        // Build TechPulse
        var techPulsePanel = BuildTechPulsePanel(canvasObject.transform);
        var techUI = techPulsePanel.GetComponent<TechPulseUI>();
        
        // Serialize references on HUDController for runtime button listener setup
        var serializedHUD2 = new SerializedObject(hudController);
        serializedHUD2.FindProperty("techPulseButton").objectReferenceValue = techPulseBtn.GetComponent<Button>();
        serializedHUD2.FindProperty("techPulseUI").objectReferenceValue = techUI;

        serializedHUD2.FindProperty("contextMenuPanel").objectReferenceValue = contextMenu.gameObject;
        serializedHUD2.FindProperty("contextLaunchProductBtn").objectReferenceValue = contextLaunchBtn.GetComponent<Button>();
        serializedHUD2.FindProperty("contextManageTeamBtn").objectReferenceValue = contextManageBtn.GetComponent<Button>();
        serializedHUD2.FindProperty("contextMarketingBtn").objectReferenceValue = contextMarketingBtn.GetComponent<Button>();
        serializedHUD2.FindProperty("contextTeaserBtn").objectReferenceValue = contextTeaserBtn.GetComponent<Button>();
        serializedHUD2.FindProperty("projectPanelObj").objectReferenceValue = projectPanel.gameObject;
        serializedHUD2.FindProperty("summaryPanelObj").objectReferenceValue = summaryPanel.gameObject;
        serializedHUD2.FindProperty("projectCloseButton").objectReferenceValue = projCloseBtn.GetComponent<Button>();
        serializedHUD2.FindProperty("summaryCloseButton").objectReferenceValue = sumCloseBtn.GetComponent<Button>();

        serializedHUD2.ApplyModifiedPropertiesWithoutUndo();

        techPulseBtn.GetComponent<Button>().onClick.AddListener(techUI.Toggle);

        resultPanelObj.transform.SetParent(canvasObject.transform, false);
        var resultCG = resultPanelObj.AddComponent<CanvasGroup>();
        var resultPanelRect = resultPanelObj.AddComponent<RectTransform>();
        resultPanelRect.anchorMin = Vector2.zero;
        resultPanelRect.anchorMax = Vector2.one;
        resultPanelRect.offsetMin = Vector2.zero;
        resultPanelRect.offsetMax = Vector2.zero;

        var resultPanelScript = resultPanelObj.AddComponent<ProjectResultPanel>();

        var resultOverlay = CreateUiRect("Overlay", resultPanelObj.transform, new Vector2(1920f, 1080f), Vector2.zero, GameDesignConstants.SurfaceOverlay);
        resultOverlay.anchorMin = Vector2.zero;
        resultOverlay.anchorMax = Vector2.one;
        resultOverlay.offsetMin = Vector2.zero;
        resultOverlay.offsetMax = Vector2.zero;

        var resultCard = CreatePremiumUiPanel("ResultCard", resultPanelObj.transform, new Vector2(460f, 420f), Vector2.zero, GameDesignConstants.SurfaceCard, new Color(1f, 1f, 1f, 0.08f));
        resultCard.anchorMin = new Vector2(0.5f, 0.5f);
        resultCard.anchorMax = new Vector2(0.5f, 0.5f);
        resultCard.pivot = new Vector2(0.5f, 0.5f);

        var resultHeader = CreateUiRect("HeaderBg", resultCard, new Vector2(460f, 50f), new Vector2(0f, 185f), GameDesignConstants.SurfaceLight);
        var resultCardTitle = CreateTMPText("Title", resultCard, "PROJECT EVALUATION", 18f, TextAlignmentOptions.Center, GameDesignConstants.BrandAccent, new Vector2(400f, 30f), new Vector2(0f, 185f));
        resultCardTitle.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var resultProjName = CreateTMPText("ProjectName", resultCard, "SupportBot v0.1", 20f, TextAlignmentOptions.Center, Color.white, new Vector2(400f, 30f), new Vector2(0f, 135f));
        resultProjName.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var resultStatus = CreateTMPText("Status", resultCard, "Model training complete!", 14f, TextAlignmentOptions.Center, GameDesignConstants.BrandSecondary, new Vector2(400f, 25f), new Vector2(0f, 110f));

        // Stars container
        var starsContainer = new GameObject("Stars");
        starsContainer.transform.SetParent(resultCard, false);
        var starsRect = starsContainer.AddComponent<RectTransform>();
        starsRect.anchorMin = new Vector2(0.5f, 0.5f);
        starsRect.anchorMax = new Vector2(0.5f, 0.5f);
        starsRect.pivot = new Vector2(0.5f, 0.5f);
        starsRect.sizeDelta = new Vector2(200f, 30f);
        starsRect.anchoredPosition = new Vector2(0f, 75f);

        var starImages = new Image[5];
        for (int i = 0; i < 5; i++)
        {
            var starObj = new GameObject($"Star_{i}");
            starObj.transform.SetParent(starsContainer.transform, false);
            var starImg = starObj.AddComponent<Image>();
            starImg.color = GameDesignConstants.BrandAccent;
            var starRect = starObj.GetComponent<RectTransform>();
            starRect.sizeDelta = new Vector2(25f, 25f);
            starRect.anchoredPosition = new Vector2((i - 2) * 35f, 0f);
            starImages[i] = starImg;
        }

        // Metrics grid
        var qualityLabel = CreateTMPText("QualityLabel", resultCard, "Technical Quality:", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(200f, 25f), new Vector2(-100f, 30f));
        var qualityVal = CreateTMPText("QualityValue", resultCard, "85%", 14f, TextAlignmentOptions.Right, GameDesignConstants.StatusSuccess, new Vector2(100f, 25f), new Vector2(150f, 30f));
        qualityVal.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var costLabel = CreateTMPText("CostLabel", resultCard, "Development Cost:", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(200f, 25f), new Vector2(-100f, 5f));
        var costVal = CreateTMPText("CostValue", resultCard, "$4,350", 14f, TextAlignmentOptions.Right, Color.white, new Vector2(100f, 25f), new Vector2(150f, 5f));

        var clientsLabel = CreateTMPText("ClientsLabel", resultCard, "Estimated Clients:", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(200f, 25f), new Vector2(-100f, -20f));
        var clientsVal = CreateTMPText("ClientsValue", resultCard, "+12", 14f, TextAlignmentOptions.Right, GameDesignConstants.BrandSecondary, new Vector2(100f, 25f), new Vector2(150f, -20f));

        var revenueLabel = CreateTMPText("RevenueLabel", resultCard, "Monthly Revenue Gain:", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(200f, 25f), new Vector2(-100f, -45f));
        var revenueVal = CreateTMPText("RevenueValue", resultCard, "+$1,200/mo", 14f, TextAlignmentOptions.Right, GameDesignConstants.ResourceCash, new Vector2(100f, 25f), new Vector2(150f, -45f));

        var reputationLabel = CreateTMPText("ReputationLabel", resultCard, "Reputation Impact:", 14f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(200f, 25f), new Vector2(-100f, -70f));
        var reputationVal = CreateTMPText("ReputationValue", resultCard, "+4.5%", 14f, TextAlignmentOptions.Right, GameDesignConstants.ResourceReputation, new Vector2(100f, 25f), new Vector2(150f, -70f));

        // Result Decision Buttons
        var acceptBtn = CreateStylizedButton("Btn_Accept", resultCard, "ACCEPT & LAUNCH", StylizedButton.ButtonVariant.Primary, new Vector2(130f, 40f), new Vector2(-145f, -145f));
        var refineBtn = CreateStylizedButton("Btn_Refine", resultCard, "REFINE MODEL", StylizedButton.ButtonVariant.Secondary, new Vector2(130f, 40f), new Vector2(0f, -145f));
        var abandonBtn = CreateStylizedButton("Btn_Abandon", resultCard, "ABANDON", StylizedButton.ButtonVariant.Danger, new Vector2(130f, 40f), new Vector2(145f, -145f));

        // Hook up ProjectResultPanel fields
        var serializedResult = new SerializedObject(resultPanelScript);
        serializedResult.FindProperty("panelGroup").objectReferenceValue = resultCG;
        serializedResult.FindProperty("panelRect").objectReferenceValue = resultCard.GetComponent<RectTransform>();
        serializedResult.FindProperty("overlayImage").objectReferenceValue = resultOverlay.GetComponent<Image>();
        serializedResult.FindProperty("panelBackground").objectReferenceValue = (resultCard.Find("InnerBackground") != null ? resultCard.Find("InnerBackground").GetComponent<Image>() : resultCard.GetComponent<Image>());
        serializedResult.FindProperty("projectNameText").objectReferenceValue = resultProjName.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("statusText").objectReferenceValue = resultStatus.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("qualityValueText").objectReferenceValue = qualityVal.GetComponent<TextMeshProUGUI>();
        // Note: qualityFill is optional and can be null (we didn't create a fill bar for results, we have stars, so leave it null)
        serializedResult.FindProperty("costValueText").objectReferenceValue = costVal.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("clientsValueText").objectReferenceValue = clientsVal.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("revenueValueText").objectReferenceValue = revenueVal.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("reputationValueText").objectReferenceValue = reputationVal.GetComponent<TextMeshProUGUI>();
        
        var starsProp = serializedResult.FindProperty("starImages");
        starsProp.arraySize = starImages.Length;
        for (int i = 0; i < starImages.Length; i++)
        {
            starsProp.GetArrayElementAtIndex(i).objectReferenceValue = starImages[i];
        }

        serializedResult.FindProperty("acceptButton").objectReferenceValue = acceptBtn.GetComponent<Button>();
        serializedResult.FindProperty("refineButton").objectReferenceValue = refineBtn.GetComponent<Button>();
        serializedResult.FindProperty("abandonButton").objectReferenceValue = abandonBtn.GetComponent<Button>();

        serializedResult.FindProperty("qualityLabel").objectReferenceValue = qualityLabel.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("costLabel").objectReferenceValue = costLabel.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("clientsLabel").objectReferenceValue = clientsLabel.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("revenueLabel").objectReferenceValue = revenueLabel.GetComponent<TextMeshProUGUI>();
        serializedResult.FindProperty("reputationLabel").objectReferenceValue = reputationLabel.GetComponent<TextMeshProUGUI>();
        serializedResult.ApplyModifiedPropertiesWithoutUndo();

        // ── TOAST CONTAINER ──────────────────────────────────────────
        var toastContainerObj = new GameObject("ToastContainer");
        toastContainerObj.transform.SetParent(canvasObject.transform, false);
        var toastContainerRect = toastContainerObj.AddComponent<RectTransform>();
        toastContainerRect.anchorMin = new Vector2(1f, 0f);
        toastContainerRect.anchorMax = new Vector2(1f, 0f);
        toastContainerRect.pivot = new Vector2(1f, 0f);
        toastContainerRect.sizeDelta = new Vector2(360f, 400f);
        toastContainerRect.anchoredPosition = new Vector2(-24f, 24f);

        var vlg = toastContainerObj.AddComponent<VerticalLayoutGroup>();
        vlg.childAlignment = TextAnchor.LowerRight;
        vlg.childControlHeight = false;
        vlg.childControlWidth = false;
        vlg.childForceExpandHeight = false;
        vlg.childForceExpandWidth = false;
        vlg.spacing = 10f;

        // Toast template inside container
        var toastTemplate = new GameObject("ToastTemplate");
        toastTemplate.transform.SetParent(toastContainerObj.transform, false);
        toastTemplate.AddComponent<CanvasGroup>();
        var toastImg = toastTemplate.AddComponent<Image>();
        toastImg.color = GameDesignConstants.SurfaceCard;
        var toastTemplateRect = toastTemplate.GetComponent<RectTransform>();
        toastTemplateRect.sizeDelta = new Vector2(340f, 60f);

        var accentBar = new GameObject("AccentBar");
        accentBar.transform.SetParent(toastTemplate.transform, false);
        var accentBarImg = accentBar.AddComponent<Image>();
        accentBarImg.color = Color.white;
        var accentRect = accentBar.GetComponent<RectTransform>();
        accentRect.anchorMin = new Vector2(0f, 0f);
        accentRect.anchorMax = new Vector2(0f, 1f);
        accentRect.pivot = new Vector2(0f, 0.5f);
        accentRect.sizeDelta = new Vector2(6f, 0f);
        accentRect.anchoredPosition = Vector2.zero;

        var toastMsg = new GameObject("MessageText");
        toastMsg.transform.SetParent(toastTemplate.transform, false);
        var toastMsgTMP = toastMsg.AddComponent<TextMeshProUGUI>();
        toastMsgTMP.text = "Notification message";
        toastMsgTMP.fontSize = 14f;
        toastMsgTMP.alignment = TextAlignmentOptions.MidlineLeft;
        toastMsgTMP.color = GameDesignConstants.TextPrimary;
        toastMsgTMP.font = GetDefaultFont();
        var toastMsgRect = toastMsg.GetComponent<RectTransform>();
        toastMsgRect.anchorMin = Vector2.zero;
        toastMsgRect.anchorMax = Vector2.one;
        toastMsgRect.offsetMin = new Vector2(20f, 6f);
        toastMsgRect.offsetMax = new Vector2(-12f, -6f);

        // Toast Notification component hookup
        var toastNotif = toastContainerObj.AddComponent<ToastNotification>();
        var serializedToast = new SerializedObject(toastNotif);
        serializedToast.FindProperty("maxVisible").intValue = 5;
        serializedToast.FindProperty("toastTemplate").objectReferenceValue = toastTemplate;
        serializedToast.FindProperty("toastContainer").objectReferenceValue = toastContainerRect;
        serializedToast.ApplyModifiedPropertiesWithoutUndo();

        // ── PROJECT CONTROLLER HOOKUP ────────────────────────────────
        var projectController = canvasObject.AddComponent<PrototypeProjectController>();
        var serializedProj = new SerializedObject(projectController);
        serializedProj.FindProperty("assignedAgent").objectReferenceValue = founderAgent;
        serializedProj.FindProperty("startButton").objectReferenceValue = startProjectBtn.GetComponent<Button>();
        serializedProj.FindProperty("modelNameInputField").objectReferenceValue = nameInput;
        serializedProj.FindProperty("progressFill").objectReferenceValue = progressFill;
        serializedProj.FindProperty("progressBackground").objectReferenceValue = progressBack.GetComponent<Image>();
        serializedProj.FindProperty("projectNameText").objectReferenceValue = projTitle.GetComponent<TextMeshProUGUI>();
        serializedProj.FindProperty("projectDescText").objectReferenceValue = projDesc.GetComponent<TextMeshProUGUI>();
        serializedProj.FindProperty("statusText").objectReferenceValue = projStatus.GetComponent<TextMeshProUGUI>();
        serializedProj.FindProperty("cashText").objectReferenceValue = projCash.GetComponent<TextMeshProUGUI>();
        serializedProj.FindProperty("costEstimateText").objectReferenceValue = projCostEst.GetComponent<TextMeshProUGUI>();
        serializedProj.FindProperty("progressPercentText").objectReferenceValue = progressPercent.GetComponent<TextMeshProUGUI>();
        serializedProj.FindProperty("panelBackground").objectReferenceValue = (projectPanel.Find("InnerBackground") != null ? projectPanel.Find("InnerBackground").GetComponent<Image>() : projectPanel.GetComponent<Image>());
        serializedProj.FindProperty("headerBackground").objectReferenceValue = headerBg.GetComponent<Image>();
        serializedProj.FindProperty("resultPanel").objectReferenceValue = resultPanelScript;

        serializedProj.FindProperty("nlpModelButton").objectReferenceValue = btnNLP.GetComponent<Button>();
        serializedProj.FindProperty("visionModelButton").objectReferenceValue = btnVision.GetComponent<Button>();
        serializedProj.FindProperty("agenticModelButton").objectReferenceValue = btnAgentic.GetComponent<Button>();
        serializedProj.FindProperty("scrapedDataButton").objectReferenceValue = btnScraped.GetComponent<Button>();
        serializedProj.FindProperty("licensedDataButton").objectReferenceValue = btnLicensed.GetComponent<Button>();

        serializedProj.ApplyModifiedPropertiesWithoutUndo();

        // ── NEW PANELS & CONTROLLERS BUILD ───────────────────────────
        // Find Waypoints
        var coffeeWaypoint = GameObject.Find("Walk Point - Coffee");
        var whiteboardWaypoint = GameObject.Find("Walk Point - Whiteboard");
        var serverWaypoint = GameObject.Find("Walk Point - Server");

        // Find Workstations
        var researchWorkstation = GameObject.Find("Research Workstation")?.GetComponent<PrototypeWorkstation>();
        var dataWorkstation = GameObject.Find("Data Workstation")?.GetComponent<PrototypeWorkstation>();
        var safetyWorkstation = GameObject.Find("Safety Workstation")?.GetComponent<PrototypeWorkstation>();
        var infraWorkstation = GameObject.Find("Infrastructure Workstation")?.GetComponent<PrototypeWorkstation>();
        var gpuTechWorkstation = GameObject.Find("GPU Tech Workstation")?.GetComponent<PrototypeWorkstation>();
        var mlopsWorkstation = GameObject.Find("MLOps Workstation")?.GetComponent<PrototypeWorkstation>();
        var backendWorkstation = GameObject.Find("Backend Workstation")?.GetComponent<PrototypeWorkstation>();
        var financeWorkstation = GameObject.Find("Finance Workstation")?.GetComponent<PrototypeWorkstation>();
        var recruiterWorkstation = GameObject.Find("Recruiter Workstation")?.GetComponent<PrototypeWorkstation>();
        var pmWorkstation = GameObject.Find("PM Workstation")?.GetComponent<PrototypeWorkstation>();
        var salesWorkstation = GameObject.Find("Sales Workstation")?.GetComponent<PrototypeWorkstation>();
        var communityWorkstation = GameObject.Find("Community Workstation")?.GetComponent<PrototypeWorkstation>();

        // Find Character Previews
        var mlAgent = GameObject.Find("ML_Engineer_Preview");
        var scientistAgent = GameObject.Find("Research_Scientist_Preview");
        var dataAgent = GameObject.Find("Data_Engineer_Preview");
        var safetyAgent = GameObject.Find("Safety_Researcher_Preview");
        var infraAgent = GameObject.Find("Infrastructure_Engineer_Preview");
        var gpuTechAgent = GameObject.Find("GPU_Technician_Preview");
        var mlopsAgent = GameObject.Find("MLOps_Engineer_Preview");
        var backendAgent = GameObject.Find("Backend_Engineer_Preview");
        var financeAgent = GameObject.Find("Finance_Lead_Preview");
        var recruiterAgent = GameObject.Find("Recruiter_Preview");
        var pmAgent = GameObject.Find("Product_Manager_Preview");
        var salesAgent = GameObject.Find("Sales_Executive_Preview");
        var communityAgent = GameObject.Find("Community_Manager_Preview");

        // Instantiate Managers
        var hiringManagerObj = new GameObject("HiringManager");
        var hiringController = hiringManagerObj.AddComponent<HiringController>();

        var officeUpgradeObj = new GameObject("OfficeUpgradeManager");
        var officeUpgradeController = officeUpgradeObj.AddComponent<OfficeUpgradeController>();

        var gpuUpgradeObj = new GameObject("GPUUpgradeManager");
        var gpuUpgradeController = gpuUpgradeObj.AddComponent<GPUUpgradeController>();

        var researchManagerObj = new GameObject("ResearchManager");
        var researchController = researchManagerObj.AddComponent<ResearchController>();

        var analyticsManagerObj = new GameObject("AnalyticsManager");
        var analyticsController = analyticsManagerObj.AddComponent<AnalyticsController>();

        var saveLoadObj = new GameObject("SaveLoadManager");
        var saveLoadManager = saveLoadObj.AddComponent<SaveLoadManager>();

        // Find GPU Rack B
        var dioramaObj = GameObject.Find("Garage Office Diorama");
        GameObject gpuRackB = null;
        if (dioramaObj != null)
        {
            var t = dioramaObj.transform.Find("GPU Rack B");
            if (t != null) gpuRackB = t.gameObject;
        }

        // Build premium panels
        Button mlBtn, sciBtn, dataBtn, safetyBtn, infraBtn, gpuTechBtn, mlopsBtn, backendBtn, financeBtn, recruiterBtn, pmBtn, salesBtn, communityBtn;
        var hiringPanelGroup = BuildHiringPanel(
            canvasObject.transform, 
            out mlBtn, 
            out sciBtn, 
            out dataBtn, 
            out safetyBtn, 
            out infraBtn, 
            out gpuTechBtn, 
            out mlopsBtn, 
            out backendBtn,
            out financeBtn,
            out recruiterBtn,
            out pmBtn,
            out salesBtn,
            out communityBtn
        );

        Button upgradeGpuBtn, t2Btn, t3Btn, t4Btn;
        var gpuPanelGroup = BuildUpgradesPanel(canvasObject.transform, out upgradeGpuBtn, out t2Btn, out t3Btn, out t4Btn);

        var researchPanelGroup = BuildResearchPanel(canvasObject.transform, researchController);
        var analyticsPanelGroup = BuildAnalyticsPanel(canvasObject.transform, analyticsController);

        var contractController = canvasObject.AddComponent<ContractController>();
        var contractsPanelGroup = BuildContractsPanel(canvasObject.transform, contractController);

        Button quitBtn;
        var systemPanelGroup = BuildSystemPanel(canvasObject.transform, out quitBtn);

        // Build NOC panel
        Button gridUpgradeBtn, coolingUpgradeBtn, nocCloseBtn;
        TextMeshProUGUI gpuCountTxt, energyTxt, coolingTxt, statusTxt, warningTxt;
        Image energyBarFill, coolingBarFill, statusBg;
        var nocPanelGroup = BuildNOCPanel(
            canvasObject.transform,
            out gridUpgradeBtn,
            out coolingUpgradeBtn,
            out nocCloseBtn,
            out gpuCountTxt,
            out energyTxt,
            out energyBarFill,
            out coolingTxt,
            out coolingBarFill,
            out statusTxt,
            out statusBg,
            out warningTxt
        );

        // NOCController Component
        var nocManagerObj = new GameObject("NOCManager");
        var nocController = nocManagerObj.AddComponent<NOCController>();
        var serializedNoc = new SerializedObject(nocController);
        serializedNoc.FindProperty("panelGroup").objectReferenceValue = nocPanelGroup;
        serializedNoc.FindProperty("closeButton").objectReferenceValue = nocCloseBtn;
        serializedNoc.FindProperty("gpuCountText").objectReferenceValue = gpuCountTxt;
        serializedNoc.FindProperty("energyText").objectReferenceValue = energyTxt;
        serializedNoc.FindProperty("energyFill").objectReferenceValue = energyBarFill;
        serializedNoc.FindProperty("coolingText").objectReferenceValue = coolingTxt;
        serializedNoc.FindProperty("coolingFill").objectReferenceValue = coolingBarFill;
        serializedNoc.FindProperty("statusText").objectReferenceValue = statusTxt;
        serializedNoc.FindProperty("statusBackground").objectReferenceValue = statusBg;
        serializedNoc.FindProperty("warningText").objectReferenceValue = warningTxt;
        serializedNoc.FindProperty("upgradeGridButton").objectReferenceValue = gridUpgradeBtn;
        serializedNoc.FindProperty("upgradeCoolingButton").objectReferenceValue = coolingUpgradeBtn;
        serializedNoc.ApplyModifiedPropertiesWithoutUndo();

        // Wire HiringController
        var serializedHiring = new SerializedObject(hiringController);
        serializedHiring.FindProperty("projectController").objectReferenceValue = projectController;
        
        serializedHiring.FindProperty("mlButton").objectReferenceValue = mlBtn;
        serializedHiring.FindProperty("mlAgentObj").objectReferenceValue = mlAgent;
        serializedHiring.FindProperty("mlWorkstation").objectReferenceValue = workstation2;
        var mlWpsProp = serializedHiring.FindProperty("mlWaypoints");
        mlWpsProp.arraySize = 4;
        mlWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        mlWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        mlWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = workstation2.ApproachPoint;
        mlWpsProp.GetArrayElementAtIndex(3).objectReferenceValue = serverWaypoint.transform;

        serializedHiring.FindProperty("scientistButton").objectReferenceValue = sciBtn;
        serializedHiring.FindProperty("scientistAgentObj").objectReferenceValue = scientistAgent;
        serializedHiring.FindProperty("scientistWorkstation").objectReferenceValue = researchWorkstation;
        var sciWpsProp = serializedHiring.FindProperty("scientistWaypoints");
        sciWpsProp.arraySize = 3;
        sciWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        sciWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        sciWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = researchWorkstation != null ? researchWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("dataButton").objectReferenceValue = dataBtn;
        serializedHiring.FindProperty("dataAgentObj").objectReferenceValue = dataAgent;
        serializedHiring.FindProperty("dataWorkstation").objectReferenceValue = dataWorkstation;
        var dataWpsProp = serializedHiring.FindProperty("dataWaypoints");
        dataWpsProp.arraySize = 4;
        dataWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        dataWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        dataWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = dataWorkstation != null ? dataWorkstation.ApproachPoint : null;
        dataWpsProp.GetArrayElementAtIndex(3).objectReferenceValue = serverWaypoint.transform;

        serializedHiring.FindProperty("safetyButton").objectReferenceValue = safetyBtn;
        serializedHiring.FindProperty("safetyAgentObj").objectReferenceValue = safetyAgent;
        serializedHiring.FindProperty("safetyWorkstation").objectReferenceValue = safetyWorkstation;
        var safetyWpsProp = serializedHiring.FindProperty("safetyWaypoints");
        safetyWpsProp.arraySize = 4;
        safetyWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        safetyWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        safetyWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = safetyWorkstation != null ? safetyWorkstation.ApproachPoint : null;
        safetyWpsProp.GetArrayElementAtIndex(3).objectReferenceValue = serverWaypoint.transform;

        serializedHiring.FindProperty("infraButton").objectReferenceValue = infraBtn;
        serializedHiring.FindProperty("infraAgentObj").objectReferenceValue = infraAgent;
        serializedHiring.FindProperty("infraWorkstation").objectReferenceValue = infraWorkstation;
        var infraWpsProp = serializedHiring.FindProperty("infraWaypoints");
        infraWpsProp.arraySize = 3;
        infraWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        infraWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        infraWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = infraWorkstation != null ? infraWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("gpuTechButton").objectReferenceValue = gpuTechBtn;
        serializedHiring.FindProperty("gpuTechAgentObj").objectReferenceValue = gpuTechAgent;
        serializedHiring.FindProperty("gpuTechWorkstation").objectReferenceValue = gpuTechWorkstation;
        var gpuWpsProp = serializedHiring.FindProperty("gpuTechWaypoints");
        gpuWpsProp.arraySize = 3;
        gpuWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        gpuWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        gpuWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = gpuTechWorkstation != null ? gpuTechWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("mlopsButton").objectReferenceValue = mlopsBtn;
        serializedHiring.FindProperty("mlopsAgentObj").objectReferenceValue = mlopsAgent;
        serializedHiring.FindProperty("mlopsWorkstation").objectReferenceValue = mlopsWorkstation;
        var mlopsWpsProp = serializedHiring.FindProperty("mlopsWaypoints");
        mlopsWpsProp.arraySize = 3;
        mlopsWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        mlopsWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        mlopsWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = mlopsWorkstation != null ? mlopsWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("backendButton").objectReferenceValue = backendBtn;
        serializedHiring.FindProperty("backendAgentObj").objectReferenceValue = backendAgent;
        serializedHiring.FindProperty("backendWorkstation").objectReferenceValue = backendWorkstation;
        var backendWpsProp = serializedHiring.FindProperty("backendWaypoints");
        backendWpsProp.arraySize = 3;
        backendWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        backendWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        backendWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = backendWorkstation != null ? backendWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("financeButton").objectReferenceValue = financeBtn;
        serializedHiring.FindProperty("financeAgentObj").objectReferenceValue = financeAgent;
        serializedHiring.FindProperty("financeWorkstation").objectReferenceValue = financeWorkstation;
        var financeWpsProp = serializedHiring.FindProperty("financeWaypoints");
        financeWpsProp.arraySize = 3;
        financeWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        financeWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        financeWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = financeWorkstation != null ? financeWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("recruiterButton").objectReferenceValue = recruiterBtn;
        serializedHiring.FindProperty("recruiterAgentObj").objectReferenceValue = recruiterAgent;
        serializedHiring.FindProperty("recruiterWorkstation").objectReferenceValue = recruiterWorkstation;
        var recruiterWpsProp = serializedHiring.FindProperty("recruiterWaypoints");
        recruiterWpsProp.arraySize = 3;
        recruiterWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        recruiterWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        recruiterWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = recruiterWorkstation != null ? recruiterWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("pmButton").objectReferenceValue = pmBtn;
        serializedHiring.FindProperty("pmAgentObj").objectReferenceValue = pmAgent;
        serializedHiring.FindProperty("pmWorkstation").objectReferenceValue = pmWorkstation;
        var pmWpsProp = serializedHiring.FindProperty("pmWaypoints");
        pmWpsProp.arraySize = 3;
        pmWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        pmWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        pmWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = pmWorkstation != null ? pmWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("salesButton").objectReferenceValue = salesBtn;
        serializedHiring.FindProperty("salesAgentObj").objectReferenceValue = salesAgent;
        serializedHiring.FindProperty("salesWorkstation").objectReferenceValue = salesWorkstation;
        var salesWpsProp = serializedHiring.FindProperty("salesWaypoints");
        salesWpsProp.arraySize = 3;
        salesWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        salesWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        salesWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = salesWorkstation != null ? salesWorkstation.ApproachPoint : null;

        serializedHiring.FindProperty("communityButton").objectReferenceValue = communityBtn;
        serializedHiring.FindProperty("communityAgentObj").objectReferenceValue = communityAgent;
        serializedHiring.FindProperty("communityWorkstation").objectReferenceValue = communityWorkstation;
        var communityWpsProp = serializedHiring.FindProperty("communityWaypoints");
        communityWpsProp.arraySize = 3;
        communityWpsProp.GetArrayElementAtIndex(0).objectReferenceValue = coffeeWaypoint.transform;
        communityWpsProp.GetArrayElementAtIndex(1).objectReferenceValue = whiteboardWaypoint.transform;
        communityWpsProp.GetArrayElementAtIndex(2).objectReferenceValue = communityWorkstation != null ? communityWorkstation.ApproachPoint : null;

        serializedHiring.ApplyModifiedPropertiesWithoutUndo();

        // Wire OfficeUpgradeController
        var serializedUpgrade = new SerializedObject(officeUpgradeController);
        serializedUpgrade.FindProperty("upgradeT2Button").objectReferenceValue = t2Btn;
        serializedUpgrade.FindProperty("upgradeT3Button").objectReferenceValue = t3Btn;
        serializedUpgrade.FindProperty("upgradeT4Button").objectReferenceValue = t4Btn;
        serializedUpgrade.ApplyModifiedPropertiesWithoutUndo();

        // Wire GPUUpgradeController
        var serializedGpuUpgrade = new SerializedObject(gpuUpgradeController);
        serializedGpuUpgrade.FindProperty("upgradeButton").objectReferenceValue = upgradeGpuBtn;
        serializedGpuUpgrade.FindProperty("secondGpuCabinet").objectReferenceValue = gpuRackB;
        serializedGpuUpgrade.FindProperty("projectController").objectReferenceValue = projectController;
        serializedGpuUpgrade.ApplyModifiedPropertiesWithoutUndo();

        // Find SystemPanel buttons for SaveLoadManager
        var saveSystemBtn = systemPanelGroup.transform.Find("Btn_SaveGame")?.GetComponent<Button>();
        var loadSystemBtn = systemPanelGroup.transform.Find("Btn_LoadGame")?.GetComponent<Button>();

        // Wire SaveLoadManager
        var serializedSaveLoad = new SerializedObject(saveLoadManager);
        serializedSaveLoad.FindProperty("saveButton").objectReferenceValue = saveSystemBtn;
        serializedSaveLoad.FindProperty("loadButton").objectReferenceValue = loadSystemBtn;
        serializedSaveLoad.FindProperty("hiringController").objectReferenceValue = hiringController;
        serializedSaveLoad.FindProperty("secondGpuCabinet").objectReferenceValue = gpuRackB;
        serializedSaveLoad.ApplyModifiedPropertiesWithoutUndo();

        // BoardRoom UI Panel & Controller
        Button closeBoardRoomBtn, acceptRoundBtn, buyQuantumBtn, buyAnthroBtn;
        TextMeshProUGUI equityTxt, roundInfoTxt, boardTrustTxt, activeGoalTxt, remainingTimeTxt;
        Image boardTrustFillImg;
        var boardRoomPanelGroup = BuildBoardRoomPanel(
            canvasObject.transform,
            out closeBoardRoomBtn,
            out equityTxt,
            out roundInfoTxt,
            out acceptRoundBtn,
            out buyQuantumBtn,
            out buyAnthroBtn,
            out boardTrustTxt,
            out boardTrustFillImg,
            out activeGoalTxt,
            out remainingTimeTxt
        );

        var boardRoomManagerObj = new GameObject("BoardRoomManager");
        var boardRoomController = boardRoomManagerObj.AddComponent<BoardRoomController>();
        var serializedBoardRoom = new SerializedObject(boardRoomController);
        serializedBoardRoom.FindProperty("panelGroup").objectReferenceValue = boardRoomPanelGroup;
        serializedBoardRoom.FindProperty("closeButton").objectReferenceValue = closeBoardRoomBtn;
        serializedBoardRoom.FindProperty("equityText").objectReferenceValue = equityTxt;
        serializedBoardRoom.FindProperty("roundInfoText").objectReferenceValue = roundInfoTxt;
        serializedBoardRoom.FindProperty("acceptRoundButton").objectReferenceValue = acceptRoundBtn;
        serializedBoardRoom.FindProperty("buyQuantumMindsButton").objectReferenceValue = buyQuantumBtn;
        serializedBoardRoom.FindProperty("buyAnthroTechButton").objectReferenceValue = buyAnthroBtn;
        serializedBoardRoom.FindProperty("boardTrustText").objectReferenceValue = boardTrustTxt;
        serializedBoardRoom.FindProperty("boardTrustFill").objectReferenceValue = boardTrustFillImg;
        serializedBoardRoom.FindProperty("activeGoalText").objectReferenceValue = activeGoalTxt;
        serializedBoardRoom.FindProperty("remainingTimeText").objectReferenceValue = remainingTimeTxt;
        serializedBoardRoom.ApplyModifiedPropertiesWithoutUndo();

        // GameOver UI Panel & Controller
        Button loadSaveGameOverBtn, mainMenuGameOverBtn;
        TextMeshProUGUI reasonGameOverTxt;
        var gameOverPanelGroup = BuildGameOverPanel(
            canvasObject.transform,
            out loadSaveGameOverBtn,
            out mainMenuGameOverBtn,
            out reasonGameOverTxt
        );

        var gameOverManagerObj = new GameObject("GameOverManager");
        var gameOverController = gameOverManagerObj.AddComponent<GameOverController>();
        var serializedGameOver = new SerializedObject(gameOverController);
        serializedGameOver.FindProperty("panelGroup").objectReferenceValue = gameOverPanelGroup;
        serializedGameOver.FindProperty("loadSaveButton").objectReferenceValue = loadSaveGameOverBtn;
        serializedGameOver.FindProperty("mainMenuButton").objectReferenceValue = mainMenuGameOverBtn;
        serializedGameOver.FindProperty("gameOverText").objectReferenceValue = reasonGameOverTxt;
        serializedGameOver.ApplyModifiedPropertiesWithoutUndo();

        // Wire dock buttons & CanvasGroups in HUDController
        var serializedHUD3 = new SerializedObject(hudController);
        serializedHUD3.FindProperty("hiringPanelGroup").objectReferenceValue = hiringPanelGroup;
        serializedHUD3.FindProperty("gpuPanelGroup").objectReferenceValue = gpuPanelGroup;
        serializedHUD3.FindProperty("researchPanelGroup").objectReferenceValue = researchPanelGroup;
        serializedHUD3.FindProperty("analyticsPanelGroup").objectReferenceValue = analyticsPanelGroup;
        serializedHUD3.FindProperty("contractsPanelGroup").objectReferenceValue = contractsPanelGroup;
        serializedHUD3.FindProperty("systemPanelGroup").objectReferenceValue = systemPanelGroup;
        serializedHUD3.FindProperty("systemQuitButton").objectReferenceValue = quitBtn;
        serializedHUD3.FindProperty("nocPanelGroup").objectReferenceValue = nocPanelGroup;
        serializedHUD3.FindProperty("boardRoomPanelGroup").objectReferenceValue = boardRoomPanelGroup;
        serializedHUD3.ApplyModifiedPropertiesWithoutUndo();

        ApplyRealisticRefactorUi(scene);

        // Save Gameplay scene
        EditorSceneManager.SaveScene(scene, GameplayScenePath);

        // Add both scenes to build settings
        AddScenesToBuildSettings(new[] { MainMenuScenePath, GameplayScenePath });
    }

    private static void ApplyRealisticRefactorUi(Scene scene)
    {
        var canvas = FindGameplayCanvas(scene);
        if (canvas == null)
        {
            Debug.LogWarning("Model Foundry: Could not find a gameplay Canvas to apply the realistic refactor UI.");
            return;
        }

        if (Object.FindFirstObjectByType<TeamSimulationManager>() == null)
        {
            var gameManager = Object.FindFirstObjectByType<GameManager>();
            var host = gameManager != null ? gameManager.gameObject : new GameObject("GameManager");
            host.AddComponent<TeamSimulationManager>();
            EditorUtility.SetDirty(host);
        }

        foreach (var transform in FindSceneTransforms(scene))
        {
            if (transform == null)
            {
                continue;
            }

            var objectName = transform.name;
            if (objectName == "StartupDashboard")
            {
                Object.DestroyImmediate(transform.gameObject);
                continue;
            }

            if (objectName == "ContextMenuPanel" ||
                objectName == "RightClickContextMenu")
            {
                Object.DestroyImmediate(transform.gameObject);
                continue;
            }

            if (objectName == "BottomActionBar" ||
                objectName == "SummaryPanel" ||
                objectName == "ProjectPanel" ||
                objectName == "LeftDockStrip")
            {
                transform.gameObject.SetActive(false);
            }
        }

        var existing = canvas.GetComponentInChildren<StartupDashboardController>(true);
        if (existing == null)
        {
            var layer = new GameObject("StartupCommandLayer");
            layer.transform.SetParent(canvas.transform, false);
            existing = layer.AddComponent<StartupDashboardController>();
        }

        existing.gameObject.SetActive(true);
        existing.RebuildUi();
        EditorUtility.SetDirty(existing);
        EditorSceneManager.MarkSceneDirty(scene);
    }

    private static Canvas FindGameplayCanvas(Scene scene)
    {
        foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (canvas.gameObject.scene != scene)
            {
                continue;
            }

            if (canvas.GetComponent<HUDController>() != null)
            {
                return canvas;
            }
        }

        foreach (var canvas in Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (canvas.gameObject.scene == scene)
            {
                return canvas;
            }
        }

        return null;
    }

    private static IEnumerable<Transform> FindSceneTransforms(Scene scene)
    {
        foreach (var transform in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (transform != null && transform.gameObject.scene == scene)
            {
                yield return transform;
            }
        }
    }

    private static GameObject BuildTechPulsePanel(Transform parent)
    {
        var panelObj = new GameObject("TechPulsePanel");
        panelObj.transform.SetParent(parent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f); // Width 420, leaving 60px for top bar
        rt.anchoredPosition = new Vector2(0f, -30f); // Offset Y by -30 to account for top bar height

        // X style dark-mode background: pure black with gray border (#2F3336)
        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, Color.black, new Color(0.18f, 0.20f, 0.21f));
        
        // 1. Back Header (Top bar with title & close)
        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        
        // Title (Bold White)
        var titleTextRect = CreateTMPText("Title", header, "TechPulse Profile", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 22f), new Vector2(20f, -4f));
        titleTextRect.anchorMin = new Vector2(0, 1);
        titleTextRect.anchorMax = new Vector2(0, 1);
        titleTextRect.pivot = new Vector2(0, 1);
        var profileTitleTMP = titleTextRect.GetComponent<TextMeshProUGUI>();
        profileTitleTMP.fontStyle = FontStyles.Bold;

        // Subtitle/Post Count
        var postsCountRect = CreateTMPText("PostsCount", header, "0 posts", 11f, TextAlignmentOptions.Left, new Color(0.44f, 0.46f, 0.48f), new Vector2(250f, 18f), new Vector2(20f, -26f));
        postsCountRect.anchorMin = new Vector2(0, 1);
        postsCountRect.anchorMax = new Vector2(0, 1);
        postsCountRect.pivot = new Vector2(0, 1);

        // Close Button (Classic X)
        var closeBtnObj = CreateStylizedButton("Btn_Close", header, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, -12f));
        closeBtnObj.GetComponent<RectTransform>().anchorMin = new Vector2(1f, 1f);
        closeBtnObj.GetComponent<RectTransform>().anchorMax = new Vector2(1f, 1f);
        closeBtnObj.GetComponent<RectTransform>().pivot = new Vector2(1f, 1f);
        closeBtnObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-20f, -12f);

        // Header separator line (#2F3336)
        CreateUiRect("HeaderSeparator", header, new Vector2(420f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        // ── PROFILE PAGE CONTENT ──────────────────────────────────
        var profileContainer = CreateUiRect("ProfileContainer", panelObj.transform, new Vector2(420f, 260f), new Vector2(0f, -50f), Color.black);
        profileContainer.anchorMin = new Vector2(0f, 1f);
        profileContainer.anchorMax = new Vector2(1f, 1f);
        profileContainer.pivot = new Vector2(0.5f, 1f);

        // A. Cover Banner (Dark Navy)
        var coverBanner = CreateUiRect("CoverBanner", profileContainer, new Vector2(420f, 90f), Vector2.zero, new Color(0.09f, 0.13f, 0.24f));
        coverBanner.anchorMin = new Vector2(0f, 1f);
        coverBanner.anchorMax = new Vector2(1f, 1f);
        coverBanner.pivot = new Vector2(0.5f, 1f);

        // B. Overlapping circular avatar
        var userAvatar = CreateUiRect("UserAvatar", profileContainer, new Vector2(64f, 60f), new Vector2(20f, -65f), Color.black);
        userAvatar.anchorMin = new Vector2(0f, 1f);
        userAvatar.anchorMax = new Vector2(0f, 1f);
        userAvatar.pivot = new Vector2(0f, 1f);
        var avatarBorder = CreateUiRect("AvatarBorder", userAvatar, new Vector2(60f, 56f), Vector2.zero, new Color(0.11f, 0.15f, 0.26f));
        avatarBorder.anchorMin = new Vector2(0.5f, 0.5f);
        avatarBorder.anchorMax = new Vector2(0.5f, 0.5f);
        avatarBorder.pivot = new Vector2(0.5f, 0.5f);
        CreateTMPText("UserAvatarSymbol", avatarBorder, "▲", 24f, TextAlignmentOptions.Center, GameDesignConstants.BrandSecondary, new Vector2(60f, 56f), Vector2.zero);

        // C. Company Name (white bold, size 16)
        var compNameRect = CreateTMPText("ProfileCompanyName", profileContainer, "Company Name", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(380f, 22f), new Vector2(20f, -135f));
        compNameRect.anchorMin = new Vector2(0f, 1f);
        compNameRect.anchorMax = new Vector2(1f, 1f);
        compNameRect.pivot = new Vector2(0f, 1f);
        compNameRect.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // D. Company Handle (gray, size 12)
        var compHandleRect = CreateTMPText("ProfileCompanyHandle", profileContainer, "@company", 12f, TextAlignmentOptions.Left, new Color(0.44f, 0.46f, 0.48f), new Vector2(380f, 18f), new Vector2(20f, -157f));
        compHandleRect.anchorMin = new Vector2(0f, 1f);
        compHandleRect.anchorMax = new Vector2(1f, 1f);
        compHandleRect.pivot = new Vector2(0f, 1f);

        // E. Company Bio (white/gray, size 12)
        var compBioRect = CreateTMPText("ProfileCompanyBio", profileContainer, "Pioneering artificial intelligence research...", 12f, TextAlignmentOptions.TopLeft, new Color(0.91f, 0.91f, 0.92f), new Vector2(380f, 38f), new Vector2(20f, -177f));
        compBioRect.anchorMin = new Vector2(0f, 1f);
        compBioRect.anchorMax = new Vector2(1f, 1f);
        compBioRect.pivot = new Vector2(0f, 1f);
        compBioRect.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;

        // F. Followers / Following
        var followingRect = CreateTMPText("ProfileFollowing", profileContainer, "42 Following", 12f, TextAlignmentOptions.Left, Color.white, new Vector2(120f, 20f), new Vector2(20f, -220f));
        followingRect.anchorMin = new Vector2(0f, 1f);
        followingRect.anchorMax = new Vector2(0f, 1f);
        followingRect.pivot = new Vector2(0f, 1f);

        var followersRect = CreateTMPText("ProfileFollowers", profileContainer, "0 Followers", 12f, TextAlignmentOptions.Left, Color.white, new Vector2(180f, 20f), new Vector2(150f, -220f));
        followersRect.anchorMin = new Vector2(0f, 1f);
        followersRect.anchorMax = new Vector2(0f, 1f);
        followersRect.pivot = new Vector2(0f, 1f);

        // G. Tab Sub-Headers (Posts tab)
        var tabContainer = CreateUiRect("Tabs", profileContainer, new Vector2(420f, 32f), new Vector2(0f, -242f), Color.black);
        tabContainer.anchorMin = new Vector2(0f, 1f);
        tabContainer.anchorMax = new Vector2(1f, 1f);
        tabContainer.pivot = new Vector2(0.5f, 1f);

        var forYouText = CreateTMPText("PostsTabLabel", tabContainer, "Posts", 14f, TextAlignmentOptions.Center, Color.white, new Vector2(100f, 28f), new Vector2(-150f, 0f));
        forYouText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        
        var tabHighlight = CreateUiRect("TabUnderline", forYouText, new Vector2(45f, 3f), new Vector2(0f, -13f), new Color(0.11f, 0.61f, 0.94f));
        tabHighlight.anchorMin = new Vector2(0.5f, 0.5f);
        tabHighlight.anchorMax = new Vector2(0.5f, 0.5f);
        tabHighlight.pivot = new Vector2(0.5f, 0.5f);

        var repliesText = CreateTMPText("RepliesTabLabel", tabContainer, "Replies", 14f, TextAlignmentOptions.Center, new Color(0.44f, 0.46f, 0.48f), new Vector2(100f, 28f), new Vector2(-50f, 0f));
        var mediaText = CreateTMPText("MediaTabLabel", tabContainer, "Media", 14f, TextAlignmentOptions.Center, new Color(0.44f, 0.46f, 0.48f), new Vector2(100f, 28f), new Vector2(50f, 0f));
        var likesText = CreateTMPText("LikesTabLabel", tabContainer, "Likes", 14f, TextAlignmentOptions.Center, new Color(0.44f, 0.46f, 0.48f), new Vector2(100f, 28f), new Vector2(150f, 0f));

        // Compose Area separator line (#2F3336)
        CreateUiRect("ComposeSeparator", profileContainer, new Vector2(420f, 1f), new Vector2(0f, -275f), new Color(0.18f, 0.20f, 0.21f));

        // ── SCROLL VIEW FOR FEED ─────────────────────────────────────
        var scrollObj = new GameObject("Scroll View");
        scrollObj.transform.SetParent(panelObj.transform, false);
        var scrollRt = scrollObj.AddComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0f, 0f);
        scrollRt.anchorMax = new Vector2(1f, 1f);
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = new Vector2(0f, -325f); // Start exactly below the profile tabs & separator

        var scrollImg = scrollObj.AddComponent<Image>();
        scrollImg.color = Color.black;

        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 20f;
        
        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        var viewportMask = viewport.AddComponent<RectMask2D>();
        scrollRect.viewport = viewportRt;
        
        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0f, 0f);
        
        var contentImg = content.AddComponent<Image>();
        contentImg.color = Color.black;

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 0f;
        vlg.padding = new RectOffset(0, 0, 0, 0);
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRt;
        
        // ── POST PREFAB (pure black with separator borders only) ─────
        var postPrefab = new GameObject("TechPulsePostPrefab");
        var postRt = postPrefab.AddComponent<RectTransform>();
        postRt.sizeDelta = new Vector2(420f, 125f);
        var postLayout = postPrefab.AddComponent<LayoutElement>();
        postLayout.preferredHeight = 125f;
        postLayout.minHeight = 125f;
        
        // Inner Container for post
        var postContainer = CreateUiRect("PostContainer", postPrefab.transform, new Vector2(420f, 125f), Vector2.zero, Color.black);
        postContainer.anchorMin = Vector2.zero;
        postContainer.anchorMax = Vector2.one;
        postContainer.offsetMin = Vector2.zero;
        postContainer.offsetMax = Vector2.zero;
        
        // Bottom border line separating posts (#2F3336)
        var postBottomLine = CreateUiRect("BottomLine", postContainer.transform, new Vector2(420f, 1f), new Vector2(0f, -124f), new Color(0.18f, 0.20f, 0.21f));
        postBottomLine.anchorMin = new Vector2(0.5f, 0f);
        postBottomLine.anchorMax = new Vector2(0.5f, 0f);
        postBottomLine.pivot = new Vector2(0.5f, 0f);

        // Circular Post Avatar on left side
        var postAvatar = CreateUiRect("PostAvatar", postContainer.transform, new Vector2(38f, 38f), new Vector2(20f, -12f), Color.white);
        postAvatar.anchorMin = new Vector2(0f, 1f);
        postAvatar.anchorMax = new Vector2(0f, 1f);
        postAvatar.pivot = new Vector2(0f, 1f);
        var postAvatarImg = postAvatar.GetComponent<Image>();
        postAvatarImg.color = new Color(0.08f, 0.08f, 0.12f);
        CreateTMPText("AvatarSymbol", postAvatar.transform, "▲", 14f, TextAlignmentOptions.Center, GameDesignConstants.BrandPrimary, new Vector2(38f, 38f), Vector2.zero);

        var authNameRect = CreateTMPText("AuthorName", postContainer.transform, "Company Name", 15f, TextAlignmentOptions.Left, Color.white, new Vector2(140f, 20f), new Vector2(70f, -10f));
        authNameRect.anchorMin = new Vector2(0, 1);
        authNameRect.anchorMax = new Vector2(0, 1);
        authNameRect.pivot = new Vector2(0, 1);
        authNameRect.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var authHandleRect = CreateTMPText("AuthorHandle", postContainer.transform, "@company • Today", 13f, TextAlignmentOptions.Left, new Color(0.44f, 0.46f, 0.48f), new Vector2(190f, 20f), new Vector2(215f, -10f));
        authHandleRect.anchorMin = new Vector2(0, 1);
        authHandleRect.anchorMax = new Vector2(0, 1);
        authHandleRect.pivot = new Vector2(0, 1);

        var pContent = CreateTMPText("Content", postContainer.transform, "Post content goes here", 14f, TextAlignmentOptions.TopLeft, new Color(0.91f, 0.91f, 0.92f), new Vector2(330f, 52f), new Vector2(70f, -32f));
        pContent.anchorMin = new Vector2(0, 1);
        pContent.anchorMax = new Vector2(0, 1);
        pContent.pivot = new Vector2(0, 1);
        pContent.GetComponent<TextMeshProUGUI>().textWrappingMode = TextWrappingModes.Normal;

        var statsRect = CreateTMPText("Stats", postContainer.transform, "0 replies • 0 reposts • 0 likes", 13f, TextAlignmentOptions.Left, new Color(0.44f, 0.46f, 0.48f), new Vector2(330f, 20f), new Vector2(70f, -94f));
        statsRect.anchorMin = new Vector2(0, 1);
        statsRect.anchorMax = new Vector2(0, 1);
        statsRect.pivot = new Vector2(0, 1);
        
        // Hide prefab by moving it out of hierarchy
        postPrefab.SetActive(false);
        postPrefab.transform.SetParent(panelObj.transform, false);

        // ── UI SCRIPT SETUP ──────────────────────────────────────────
        var techUI = panelObj.AddComponent<TechPulseUI>();
        var serializedTech = new SerializedObject(techUI);
        serializedTech.FindProperty("mainPanel").objectReferenceValue = cg;
        serializedTech.FindProperty("scrollContent").objectReferenceValue = contentRt;
        serializedTech.FindProperty("postPrefab").objectReferenceValue = postPrefab;
        serializedTech.FindProperty("closeButton").objectReferenceValue = closeBtnObj.GetComponent<Button>();

        // Wire profile header text fields
        serializedTech.FindProperty("profileCompanyNameText").objectReferenceValue = compNameRect.GetComponent<TextMeshProUGUI>();
        serializedTech.FindProperty("profileCompanyHandleText").objectReferenceValue = compHandleRect.GetComponent<TextMeshProUGUI>();
        serializedTech.FindProperty("profileCompanyBioText").objectReferenceValue = compBioRect.GetComponent<TextMeshProUGUI>();
        serializedTech.FindProperty("profileFollowersText").objectReferenceValue = followersRect.GetComponent<TextMeshProUGUI>();
        serializedTech.FindProperty("profileFollowingText").objectReferenceValue = followingRect.GetComponent<TextMeshProUGUI>();
        serializedTech.FindProperty("profilePostsCountText").objectReferenceValue = postsCountRect.GetComponent<TextMeshProUGUI>();

        serializedTech.ApplyModifiedPropertiesWithoutUndo();

        return panelObj;
    }

    private static Sprite BakePrefabToSprite(string prefabPath, string spriteName)
    {
        string folderPath = "Assets/Synty/PolygonIcons/BakedSprites";
        string fullPath = $"{folderPath}/{spriteName}.png";

        // If the file exists and is not empty, use it, otherwise regenerate
        if (System.IO.File.Exists(fullPath))
        {
            var info = new System.IO.FileInfo(fullPath);
            if (info.Length > 1000)
            {
                var existingSprite = AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
                if (existingSprite != null) return existingSprite;
            }
        }

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"[BakePrefabToSprite] Prefab not found at: {prefabPath}");
            return null;
        }

        // --- SYNCHRONOUS CAMERA-BASED RENDERING IN URP ---
        var bakeRoot = new GameObject("BakeRoot");
        bakeRoot.transform.position = new Vector3(2000f, 2000f, 2000f);

        var inst = GameObject.Instantiate(prefab, bakeRoot.transform);
        inst.transform.localPosition = Vector3.zero;
        inst.transform.localRotation = Quaternion.identity;
        inst.transform.localScale = Vector3.one;

        var renderers = inst.GetComponentsInChildren<Renderer>();
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        bool hasBounds = false;
        foreach (var r in renderers)
        {
            if (!hasBounds)
            {
                bounds = r.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        Vector3 center = hasBounds ? bounds.center : bakeRoot.transform.position;
        float radius = hasBounds ? bounds.extents.magnitude : 1.0f;
        if (radius < 0.1f) radius = 1.0f;

        // Set up lights inside bake scene
        var keyLightObj = new GameObject("KeyLight");
        keyLightObj.transform.SetParent(bakeRoot.transform, false);
        var keyLight = keyLightObj.AddComponent<Light>();
        keyLight.type = LightType.Directional;
        keyLight.intensity = 1.6f;
        keyLight.color = Color.white;
        keyLightObj.transform.rotation = Quaternion.Euler(30f, -120f, 0f);

        var fillLightObj = new GameObject("FillLight");
        fillLightObj.transform.SetParent(bakeRoot.transform, false);
        var fillLight = fillLightObj.AddComponent<Light>();
        fillLight.type = LightType.Directional;
        fillLight.intensity = 0.5f;
        fillLight.color = new Color(0.85f, 0.92f, 1.0f);
        fillLightObj.transform.rotation = Quaternion.Euler(-30f, 45f, 0f);

        // Set up camera
        var camObj = new GameObject("BakeCam");
        camObj.transform.SetParent(bakeRoot.transform, false);
        var cam = camObj.AddComponent<Camera>();
        
        camObj.transform.position = center + new Vector3(0.6f, 0.6f, 1.2f).normalized * (radius * 2.6f);
        camObj.transform.LookAt(center);

        cam.orthographic = true;
        cam.orthographicSize = radius * 1.15f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0f, 0f, 0f, 0f);
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 100f;

        var rt = new RenderTexture(256, 256, 24, RenderTextureFormat.ARGB32);
        rt.Create();
        cam.targetTexture = rt;
        
        cam.Render();

        RenderTexture.active = rt;
        var tex = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);
        tex.Apply();

        RenderTexture.active = null;
        cam.targetTexture = null;
        rt.Release();
        GameObject.DestroyImmediate(rt);
        GameObject.DestroyImmediate(bakeRoot);

        byte[] bytes = tex.EncodeToPNG();
        GameObject.DestroyImmediate(tex);

        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

        System.IO.File.WriteAllBytes(fullPath, bytes);
        AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceSynchronousImport);

        var importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            AssetDatabase.ImportAsset(fullPath, ImportAssetOptions.ForceSynchronousImport);
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);

        return AssetDatabase.LoadAssetAtPath<Sprite>(fullPath);
    }

    private static void AddScenesToBuildSettings(string[] scenePaths)
    {
        var buildScenes = new System.Collections.Generic.List<EditorBuildSettingsScene>();
        
        // Add menu scene first
        buildScenes.Add(new EditorBuildSettingsScene(scenePaths[0], true));
        // Add gameplay scene second
        buildScenes.Add(new EditorBuildSettingsScene(scenePaths[1], true));

        // Add other existing scenes if present in build settings, preserving them
        foreach (var existing in EditorBuildSettings.scenes)
        {
            if (existing.path != scenePaths[0] && existing.path != scenePaths[1])
            {
                buildScenes.Add(existing);
            }
        }

        EditorBuildSettings.scenes = buildScenes.ToArray();
    }

    // ── Office Diorama Setup ──────────────────────────────────────────
    private static void BuildOfficeDiorama()
    {
        var root = new GameObject("Garage Office Diorama");
        var ovc = root.AddComponent<OfficeVisualController>();

        // T1 Left Walls group
        var t1LeftWalls = new GameObject("Old Left Walls");
        t1LeftWalls.transform.SetParent(root.transform, false);

        // T1 Back Walls group
        var t1BackWalls = new GameObject("Old Back Walls");
        t1BackWalls.transform.SetParent(root.transform, false);

        // T1 Right Walls group
        var t1RightWalls = new GameObject("Old Right Walls");
        t1RightWalls.transform.SetParent(root.transform, false);

        // Tier 2 Expansion group
        var t2ExpansionGroup = new GameObject("Office Expansion T2");
        t2ExpansionGroup.transform.SetParent(root.transform, false);

        // Tier 3 Secret Lab group
        var t3SecretLabGroup = new GameObject("Secret Lab T3");
        t3SecretLabGroup.transform.SetParent(root.transform, false);

        // Tier 4 Datacenter group
        var t4DatacenterGroup = new GameObject("Datacenter T4");
        t4DatacenterGroup.transform.SetParent(root.transform, false);

        // --- Build T1 ---
        // Concrete floor
        for (var x = -2; x <= 2; x++)
        {
            for (var z = -2; z <= 1; z++)
            {
                PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Floor_Concrete_01.prefab", "Concrete Floor Tile", new Vector3(x * 2f, 0f, z * 2f), Vector3.zero, Vector3.one, root.transform);
            }
        }

        // Back Window Wall (T1) -> goes to t1BackWalls group
        for (var x = -2; x <= 2; x++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Blank_Window_01.prefab", "Back Window Wall", new Vector3(x * 2f, 0f, 4f), new Vector3(0f, 180f, 0f), Vector3.one, t1BackWalls.transform);
        }

        // Left Brick Wall (T1) -> goes to t1LeftWalls group
        for (var z = -2; z <= 1; z++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Brick_01.prefab", "Left Brick Wall", new Vector3(-5f, 0f, z * 2f), new Vector3(0f, 90f, 0f), Vector3.one, t1LeftWalls.transform);
        }

        // Right Brick Wall (T1) -> goes to t1RightWalls group
        for (var z = -2; z <= 1; z++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Brick_01.prefab", "Right Brick Wall", new Vector3(5f, 0f, z * 2f), new Vector3(0f, -90f, 0f), Vector3.one, t1RightWalls.transform);
        }

        PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Door_01.prefab", "Garage Door", new Vector3(4.2f, 0f, -3.4f), new Vector3(0f, -90f, 0f), Vector3.one, root.transform);
        
        // Founder Workstation Setup
        var founderWorkstationObj = CreateWorkstation("Founder Workstation", new Vector3(-1.1f, 0.8f, 0.2f), new Vector3(-1.1f, 0f, -0.7f), new Vector3(-1.1f, 0f, -0.3f));
        founderWorkstationObj.transform.SetParent(root.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Founder Desk", new Vector3(-1.1f, 0f, 0.35f), new Vector3(0f, 180f, 0f), Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Founder Chair", new Vector3(-1.1f, 0f, -0.7f), Vector3.zero, Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Founder Computer", new Vector3(-1.1f, 0.72f, 0.25f), new Vector3(0f, 180f, 0f), Vector3.one, root.transform);

        // Helper Workstation Setup (ML Engineer)
        var workstation2 = CreateWorkstation("ML Engineer Workstation", new Vector3(1.1f, 0.8f, 0.2f), new Vector3(1.1f, 0f, -0.7f), new Vector3(1.1f, 0f, -0.3f));
        workstation2.transform.SetParent(root.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Helper Desk", new Vector3(1.1f, 0f, 0.35f), new Vector3(0f, 180f, 0f), Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Helper Chair", new Vector3(1.1f, 0f, -0.7f), Vector3.zero, Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Helper Computer", new Vector3(1.1f, 0.72f, 0.25f), new Vector3(0f, 180f, 0f), Vector3.one, root.transform);

        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_01.prefab", "GPU Rack A", new Vector3(3.25f, 0f, 2.55f), new Vector3(0f, -90f, 0f), Vector3.one, root.transform);
        var cabinet2 = PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_02.prefab", "GPU Rack B", new Vector3(3.25f, 0f, 1.35f), new Vector3(0f, -90f, 0f), Vector3.one, root.transform);
        if (cabinet2 != null)
        {
            cabinet2.SetActive(false);
        }

        // Cyber Teal Point Light on Server Cabinet
        var serverLight = new GameObject("ServerGlowLight");
        serverLight.transform.SetParent(root.transform, false);
        serverLight.transform.position = new Vector3(3.1f, 1.2f, 1.95f);
        var serverLightComp = serverLight.AddComponent<Light>();
        serverLightComp.type = LightType.Point;
        serverLightComp.color = GameDesignConstants.BrandSecondary; // cyber cyan
        serverLightComp.intensity = 3.5f;
        serverLightComp.range = 3.5f;

        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Whiteboard_01.prefab", "Roadmap Whiteboard", new Vector3(-2.6f, 1.45f, 3.75f), new Vector3(0f, 180f, 0f), Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Couch_01.prefab", "Nap Couch", new Vector3(1.4f, 0f, -2.8f), new Vector3(0f, 180f, 0f), Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_CoffeeTable_01.prefab", "Coffee Table", new Vector3(1.4f, 0f, -1.85f), Vector3.zero, Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Kitchen Props/SM_Prop_CoffeeMachine_Dripper_01.prefab", "Coffee Machine", new Vector3(-3.8f, 0f, -2.2f), new Vector3(0f, 90f, 0f), Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Plant_01.prefab", "Office Plant A", new Vector3(-4f, 0f, 3.1f), Vector3.zero, Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Plant_02.prefab", "Office Plant B", new Vector3(4.2f, 0f, -2.9f), Vector3.zero, Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Book_Group_01.prefab", "Research Papers", new Vector3(-2.1f, 0.74f, 0.25f), Vector3.zero, Vector3.one, root.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Desklamp_01.prefab", "Desk Lamp", new Vector3(-0.25f, 0.74f, 0.15f), Vector3.zero, Vector3.one, root.transform);

        // Cozy Warm Point Light on Desk Lamp
        var lampLight = new GameObject("LampGlowLight");
        lampLight.transform.SetParent(root.transform, false);
        lampLight.transform.position = new Vector3(-0.25f, 1.1f, 0.15f);
        var lampLightComp = lampLight.AddComponent<Light>();
        lampLightComp.type = LightType.Point;
        lampLightComp.color = GameDesignConstants.BrandAccent; // warm golden amber
        lampLightComp.intensity = 2.0f;
        lampLightComp.range = 2.0f;

        // --- Build T2 Left Expansion ---
        for (var x = -4; x <= -3; x++)
        {
            for (var z = -2; z <= 1; z++)
            {
                PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Floor_Concrete_01.prefab", "Concrete Floor Tile T2", new Vector3(x * 2f, 0f, z * 2f), Vector3.zero, Vector3.one, t2ExpansionGroup.transform);
            }
        }
        for (var z = -2; z <= 1; z++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Brick_01.prefab", "Left Brick Wall T2", new Vector3(-9f, 0f, z * 2f), new Vector3(0f, 90f, 0f), Vector3.one, t2ExpansionGroup.transform);
        }
        for (var x = -4; x <= -3; x++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Blank_Window_01.prefab", "Back Window Wall T2", new Vector3(x * 2f, 0f, 4f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);
        }

        var researchWorkstationObj = CreateWorkstation("Research Workstation", new Vector3(-6.5f, 0.8f, 1.2f), new Vector3(-6.5f, 0f, 0.05f), new Vector3(-6.5f, 0f, 0.45f));
        researchWorkstationObj.transform.SetParent(t2ExpansionGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Research Desk", new Vector3(-6.5f, 0f, 1.35f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Research Chair", new Vector3(-6.5f, 0f, 0.3f), Vector3.zero, Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Research Computer", new Vector3(-6.5f, 0.72f, 1.25f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);

        var dataWorkstationObj = CreateWorkstation("Data Workstation", new Vector3(-6.5f, 0.8f, -1.2f), new Vector3(-6.5f, 0f, -2.35f), new Vector3(-6.5f, 0f, -1.95f));
        dataWorkstationObj.transform.SetParent(t2ExpansionGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Data Desk", new Vector3(-6.5f, 0f, -1.05f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Data Chair", new Vector3(-6.5f, 0f, -2.1f), Vector3.zero, Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Data Computer", new Vector3(-6.5f, 0.72f, -1.15f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);

        var financeWorkstationObj = CreateWorkstation("Finance Workstation", new Vector3(-8.5f, 0.8f, 1.2f), new Vector3(-8.5f, 0f, 0.05f), new Vector3(-8.5f, 0f, 0.45f));
        financeWorkstationObj.transform.SetParent(t2ExpansionGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Finance Desk", new Vector3(-8.5f, 0f, 1.35f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Finance Chair", new Vector3(-8.5f, 0f, 0.3f), Vector3.zero, Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Finance Computer", new Vector3(-8.5f, 0.72f, 1.25f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);

        var recruiterWorkstationObj = CreateWorkstation("Recruiter Workstation", new Vector3(-8.5f, 0.8f, -1.2f), new Vector3(-8.5f, 0f, -2.35f), new Vector3(-8.5f, 0f, -1.95f));
        recruiterWorkstationObj.transform.SetParent(t2ExpansionGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Recruiter Desk", new Vector3(-8.5f, 0f, -1.05f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Recruiter Chair", new Vector3(-8.5f, 0f, -2.1f), Vector3.zero, Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Recruiter Computer", new Vector3(-8.5f, 0.72f, -1.15f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);

        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Plant_01.prefab", "Office Plant T2", new Vector3(-8f, 0f, 3.1f), Vector3.zero, Vector3.one, t2ExpansionGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Whiteboard_01.prefab", "Research Whiteboard", new Vector3(-7.5f, 1.45f, 3.75f), new Vector3(0f, 180f, 0f), Vector3.one, t2ExpansionGroup.transform);

        // --- Build T3 Secret Lab ---
        for (var x = -4; x <= 2; x++)
        {
            for (var z = 2; z <= 3; z++)
            {
                PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Floor_Concrete_01.prefab", "Concrete Floor Tile T3", new Vector3(x * 2f, 0f, z * 2f), Vector3.zero, Vector3.one, t3SecretLabGroup.transform);
            }
        }
        for (var x = -4; x <= 2; x++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Blank_Window_01.prefab", "Back Window Wall T3", new Vector3(x * 2f, 0f, 10f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        }
        for (var z = 2; z <= 3; z++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Brick_01.prefab", "Left Brick Wall T3", new Vector3(-9f, 0f, z * 2f), new Vector3(0f, 90f, 0f), Vector3.one, t3SecretLabGroup.transform);
        }
        for (var z = 2; z <= 3; z++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Brick_01.prefab", "Right Brick Wall T3", new Vector3(5f, 0f, z * 2f), new Vector3(0f, -90f, 0f), Vector3.one, t3SecretLabGroup.transform);
        }

        var safetyWorkstationObj = CreateWorkstation("Safety Workstation", new Vector3(0f, 0.8f, 6.5f), new Vector3(0f, 0f, 5.35f), new Vector3(0f, 0f, 5.75f));
        safetyWorkstationObj.transform.SetParent(t3SecretLabGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Safety Desk", new Vector3(0f, 0f, 6.65f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Safety Chair", new Vector3(0f, 0f, 5.6f), Vector3.zero, Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Safety Computer", new Vector3(0f, 0.72f, 6.55f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);

        // Community Manager Workstation (T3)
        var communityWorkstationObj = CreateWorkstation("Community Workstation", new Vector3(2.5f, 0.8f, 6.5f), new Vector3(2.5f, 0f, 5.35f), new Vector3(2.5f, 0f, 5.75f));
        communityWorkstationObj.transform.SetParent(t3SecretLabGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Community Desk", new Vector3(2.5f, 0f, 6.65f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Community Chair", new Vector3(2.5f, 0f, 5.6f), Vector3.zero, Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Community Computer", new Vector3(2.5f, 0.72f, 6.55f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);

        // Board Room (Conference Table & TV Screen)
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Table_Conference_01.prefab", "Conference Table", new Vector3(-3.0f, 0f, 6.5f), Vector3.zero, Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_04.prefab", "Board Chair 1", new Vector3(-4.2f, 0f, 6.5f), new Vector3(0f, 90f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_04.prefab", "Board Chair 2", new Vector3(-1.8f, 0f, 6.5f), new Vector3(0f, -90f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_04.prefab", "Board Chair 3", new Vector3(-3.0f, 0f, 7.4f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_04.prefab", "Board Chair 4", new Vector3(-3.0f, 0f, 5.6f), Vector3.zero, Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Wall Props/SM_Prop_TV_Wall_01.prefab", "Board TV Screen", new Vector3(-3.0f, 2.0f, 9.8f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);

        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_01.prefab", "Secret Server 1", new Vector3(-4f, 0f, 8.5f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_02.prefab", "Secret Server 2", new Vector3(-2f, 0f, 8.5f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_01.prefab", "Secret Server 3", new Vector3(2f, 0f, 8.5f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_02.prefab", "Secret Server 4", new Vector3(4f, 0f, 8.5f), new Vector3(0f, 180f, 0f), Vector3.one, t3SecretLabGroup.transform);

        var secretLight = new GameObject("SecretLabGlowLight");
        secretLight.transform.SetParent(t3SecretLabGroup.transform, false);
        secretLight.transform.position = new Vector3(0f, 2f, 8.5f);
        var secretLightComp = secretLight.AddComponent<Light>();
        secretLightComp.type = LightType.Point;
        secretLightComp.color = GameDesignConstants.BrandSecondary; // cyber cyan
        secretLightComp.intensity = 5.0f;
        secretLightComp.range = 8.0f;

        // --- Build T4 Modular Datacenter ---
        for (var x = 3; x <= 4; x++)
        {
            for (var z = -2; z <= 3; z++)
            {
                PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Floor_Concrete_01.prefab", "Concrete Floor Tile T4", new Vector3(x * 2f, 0f, z * 2f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
            }
        }
        for (var x = 3; x <= 4; x++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Blank_Window_01.prefab", "Back Window Wall T4", new Vector3(x * 2f, 0f, 10f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        }
        for (var z = -2; z <= 3; z++)
        {
            PlacePrefab("Assets/PolygonOffice/Prefabs/Buildings/SM_Bld_Wall_Brick_01.prefab", "Right Brick Wall T4", new Vector3(9f, 0f, z * 2f), new Vector3(0f, -90f, 0f), Vector3.one, t4DatacenterGroup.transform);
        }

        var infraWorkstationObj = CreateWorkstation("Infrastructure Workstation", new Vector3(5.5f, 0.8f, -1.2f), new Vector3(5.5f, 0f, -2.35f), new Vector3(5.5f, 0f, -1.95f));
        infraWorkstationObj.transform.SetParent(t4DatacenterGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Infrastructure Desk", new Vector3(5.5f, 0f, -1.05f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Infrastructure Chair", new Vector3(5.5f, 0f, -2.1f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Infrastructure Computer", new Vector3(5.5f, 0.72f, -1.15f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        var gpuTechWorkstationObj = CreateWorkstation("GPU Tech Workstation", new Vector3(7.5f, 0.8f, -1.2f), new Vector3(7.5f, 0f, -2.35f), new Vector3(7.5f, 0f, -1.95f));
        gpuTechWorkstationObj.transform.SetParent(t4DatacenterGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "GPU Tech Desk", new Vector3(7.5f, 0f, -1.05f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "GPU Tech Chair", new Vector3(7.5f, 0f, -2.1f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "GPU Tech Computer", new Vector3(7.5f, 0.72f, -1.15f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        var mlopsWorkstationObj = CreateWorkstation("MLOps Workstation", new Vector3(5.5f, 0.8f, 1.2f), new Vector3(5.5f, 0f, 0.05f), new Vector3(5.5f, 0f, 0.45f));
        mlopsWorkstationObj.transform.SetParent(t4DatacenterGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "MLOps Desk", new Vector3(5.5f, 0f, 1.35f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "MLOps Chair", new Vector3(5.5f, 0f, 0.3f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "MLOps Computer", new Vector3(5.5f, 0.72f, 1.25f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        var backendWorkstationObj = CreateWorkstation("Backend Workstation", new Vector3(7.5f, 0.8f, 1.2f), new Vector3(7.5f, 0f, 0.05f), new Vector3(7.5f, 0f, 0.45f));
        backendWorkstationObj.transform.SetParent(t4DatacenterGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Backend Desk", new Vector3(7.5f, 0f, 1.35f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Backend Chair", new Vector3(7.5f, 0f, 0.3f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Backend Computer", new Vector3(7.5f, 0.72f, 1.25f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        // Product Manager Workstation (T4)
        var pmWorkstationObj = CreateWorkstation("PM Workstation", new Vector3(5.5f, 0.8f, 3.6f), new Vector3(5.5f, 0f, 2.45f), new Vector3(5.5f, 0f, 2.85f));
        pmWorkstationObj.transform.SetParent(t4DatacenterGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "PM Desk", new Vector3(5.5f, 0f, 3.75f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "PM Chair", new Vector3(5.5f, 0f, 2.7f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "PM Computer", new Vector3(5.5f, 0.72f, 3.65f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        // Sales Executive Workstation (T4)
        var salesWorkstationObj = CreateWorkstation("Sales Workstation", new Vector3(7.5f, 0.8f, 3.6f), new Vector3(7.5f, 0f, 2.45f), new Vector3(7.5f, 0f, 2.85f));
        salesWorkstationObj.transform.SetParent(t4DatacenterGroup.transform, false);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Desk_01.prefab", "Sales Desk", new Vector3(7.5f, 0f, 3.75f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Furniture/SM_Prop_Chair_01.prefab", "Sales Chair", new Vector3(7.5f, 0f, 2.7f), Vector3.zero, Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Desk Props/SM_Prop_Computer_Setup_01.prefab", "Sales Computer", new Vector3(7.5f, 0.72f, 3.65f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_01.prefab", "Datacenter Server 1", new Vector3(6f, 0f, 8.5f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);
        PlacePrefab("Assets/PolygonOffice/Prefabs/Props/Misc/SM_Prop_Server_Cabinet_02.prefab", "Datacenter Server 2", new Vector3(8f, 0f, 8.5f), new Vector3(0f, 180f, 0f), Vector3.one, t4DatacenterGroup.transform);

        var datacenterLight = new GameObject("DatacenterGlowLight");
        datacenterLight.transform.SetParent(t4DatacenterGroup.transform, false);
        datacenterLight.transform.position = new Vector3(7f, 2f, 8.5f);
        var datacenterLightComp = datacenterLight.AddComponent<Light>();
        datacenterLightComp.type = LightType.Point;
        datacenterLightComp.color = GameDesignConstants.BrandSecondary; // cyber cyan
        datacenterLightComp.intensity = 5.0f;
        datacenterLightComp.range = 8.0f;

        var serializedOvc = new SerializedObject(ovc);
        serializedOvc.FindProperty("t2ExpansionGroup").objectReferenceValue = t2ExpansionGroup;
        serializedOvc.FindProperty("t3SecretLabGroup").objectReferenceValue = t3SecretLabGroup;
        serializedOvc.FindProperty("t4DatacenterGroup").objectReferenceValue = t4DatacenterGroup;
        serializedOvc.FindProperty("t1LeftWalls").objectReferenceValue = t1LeftWalls;
        serializedOvc.FindProperty("t1BackWalls").objectReferenceValue = t1BackWalls;
        serializedOvc.FindProperty("t1RightWalls").objectReferenceValue = t1RightWalls;
        serializedOvc.FindProperty("researchWorkstation").objectReferenceValue = researchWorkstationObj.gameObject;
        serializedOvc.FindProperty("dataWorkstation").objectReferenceValue = dataWorkstationObj.gameObject;
        serializedOvc.FindProperty("safetyWorkstation").objectReferenceValue = safetyWorkstationObj.gameObject;
        serializedOvc.FindProperty("infraWorkstation").objectReferenceValue = infraWorkstationObj.gameObject;
        serializedOvc.FindProperty("gpuTechWorkstation").objectReferenceValue = gpuTechWorkstationObj.gameObject;
        serializedOvc.FindProperty("mlopsWorkstation").objectReferenceValue = mlopsWorkstationObj.gameObject;
        serializedOvc.FindProperty("backendWorkstation").objectReferenceValue = backendWorkstationObj.gameObject;
        serializedOvc.FindProperty("financeWorkstation").objectReferenceValue = financeWorkstationObj.gameObject;
        serializedOvc.FindProperty("recruiterWorkstation").objectReferenceValue = recruiterWorkstationObj.gameObject;
        serializedOvc.FindProperty("pmWorkstation").objectReferenceValue = pmWorkstationObj.gameObject;
        serializedOvc.FindProperty("salesWorkstation").objectReferenceValue = salesWorkstationObj.gameObject;
        serializedOvc.FindProperty("communityWorkstation").objectReferenceValue = communityWorkstationObj.gameObject;
        serializedOvc.ApplyModifiedPropertiesWithoutUndo();

        t2ExpansionGroup.SetActive(false);
        t3SecretLabGroup.SetActive(false);
        t1LeftWalls.SetActive(true);
        t1BackWalls.SetActive(true);
    }

    private static GameObject PlacePrefab(string path, string name, Vector3 position, Vector3 rotation, Vector3 scale, Transform parent = null)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogWarning($"Missing POLYGON prefab: {path}");
            return null;
        }

        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        instance.name = name;
        if (parent != null)
        {
            instance.transform.SetParent(parent);
        }

        instance.transform.position = position;
        instance.transform.rotation = Quaternion.Euler(rotation);
        instance.transform.localScale = scale;

        ApplyRoomVisuals(instance, position, path);

        return instance;
    }

    private static void ApplyRoomVisuals(GameObject instance, Vector3 position, string path)
    {
        if (instance == null) return;

        bool isWall = path.Contains("Wall");
        bool isFloor = path.Contains("Floor");

        if (!isWall && !isFloor) return;

        // Determine quadrant colors
        Color targetColor = Color.white;
        if (position.x > 5.0f) // T4 - Datacenter
        {
            targetColor = isWall ? HexColor("#991B1B") : HexColor("#374151");
        }
        else if (position.z > 3.5f) // T3 - Secret Lab
        {
            targetColor = isWall ? HexColor("#C084FC") : HexColor("#E9D5FF");
        }
        else if (position.x < -5.0f) // T2 - Expansion
        {
            targetColor = isWall ? HexColor("#F5F5F4") : HexColor("#E7E5E4");
        }
        else // T1 - Garage
        {
            targetColor = isWall ? HexColor("#38BDF8") : HexColor("#0284C7");
        }

        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return;

        var mat = new Material(shader);
        mat.color = targetColor;
        mat.SetFloat("_Roughness", 0.8f);
        mat.SetFloat("_Metallic", 0.1f);

        var renderers = instance.GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r.gameObject.name.Contains("Glass") || r.gameObject.name.Contains("Window_Glass") || r.gameObject.name.Contains("glass"))
            {
                continue; // Skip glass
            }

            var mats = r.sharedMaterials;
            bool modified = false;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null && (mats[i].name.Contains("Glass") || mats[i].name.Contains("Trans") || mats[i].name.Contains("glass")))
                {
                    continue; // Skip glass materials
                }
                mats[i] = mat;
                modified = true;
            }
            if (modified)
            {
                r.sharedMaterials = mats;
            }
        }
    }

    private static Color HexColor(string hex)
    {
        ColorUtility.TryParseHtmlString(hex, out var c);
        return c;
    }

    private static PrototypeEmployeeAgent CreateFounderAgent(PrototypeWorkstation workstation)
    {
        var founder = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Male_01.prefab", "Founder Developer", new Vector3(-3.2f, 0f, -1.9f), Vector3.zero, Vector3.one);
        var helper = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Female_01.prefab", "ML_Engineer_Preview", new Vector3(1.4f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (helper != null)
        {
            helper.SetActive(false);
        }

        var waypoints = new[]
        {
            CreateWaypoint("Walk Point - Coffee", new Vector3(-3.2f, 0f, -1.9f)),
            CreateWaypoint("Walk Point - Whiteboard", new Vector3(-2.4f, 0f, 2.2f)),
            workstation.ApproachPoint,
            CreateWaypoint("Walk Point - Server", new Vector3(2.7f, 0f, 2f))
        };

        var agent = founder.AddComponent<PrototypeEmployeeAgent>();
        var serializedAgent = new SerializedObject(agent);
        serializedAgent.FindProperty("waypoints").arraySize = waypoints.Length;
        for (var i = 0; i < waypoints.Length; i++)
        {
            serializedAgent.FindProperty("waypoints").GetArrayElementAtIndex(i).objectReferenceValue = waypoints[i];
        }

        serializedAgent.FindProperty("workstation").objectReferenceValue = workstation;
        
        var anim = founder.GetComponentInChildren<Animator>();
        var mascController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/EmployeeLocomotion_Masc.controller");
        var femController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Animations/EmployeeLocomotion_Femn.controller");

        if (anim != null)
        {
            serializedAgent.FindProperty("animator").objectReferenceValue = anim;
            anim.applyRootMotion = false;
            if (mascController != null)
            {
                anim.runtimeAnimatorController = mascController;
            }
        }
        
        if (helper != null)
        {
            var helperAnim = helper.GetComponentInChildren<Animator>();
            if (helperAnim != null)
            {
                helperAnim.applyRootMotion = false;
                if (femController != null)
                {
                    helperAnim.runtimeAnimatorController = femController;
                }
            }
            
            var helperAgent = helper.GetComponent<PrototypeEmployeeAgent>();
            if (helperAgent == null) helperAgent = helper.AddComponent<PrototypeEmployeeAgent>();
            var serializedHelper = new SerializedObject(helperAgent);
            serializedHelper.FindProperty("animator").objectReferenceValue = helperAnim;
            serializedHelper.ApplyModifiedPropertiesWithoutUndo();
        }

        // Research Scientist Preview Setup
        var scientist = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Male_02.prefab", "Research_Scientist_Preview", new Vector3(1.0f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (scientist != null)
        {
            scientist.SetActive(false);
            var sciAnim = scientist.GetComponentInChildren<Animator>();
            if (sciAnim != null)
            {
                sciAnim.applyRootMotion = false;
                if (mascController != null) sciAnim.runtimeAnimatorController = mascController;
            }
            var sciAgent = scientist.GetComponent<PrototypeEmployeeAgent>();
            if (sciAgent == null) sciAgent = scientist.AddComponent<PrototypeEmployeeAgent>();
            var serializedSci = new SerializedObject(sciAgent);
            serializedSci.FindProperty("animator").objectReferenceValue = sciAnim;
            serializedSci.ApplyModifiedPropertiesWithoutUndo();
        }

        // Data Engineer Preview Setup
        var dataEng = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Female_02.prefab", "Data_Engineer_Preview", new Vector3(1.8f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (dataEng != null)
        {
            dataEng.SetActive(false);
            var dataAnim = dataEng.GetComponentInChildren<Animator>();
            if (dataAnim != null)
            {
                dataAnim.applyRootMotion = false;
                if (femController != null) dataAnim.runtimeAnimatorController = femController;
            }
            var dataAgent = dataEng.GetComponent<PrototypeEmployeeAgent>();
            if (dataAgent == null) dataAgent = dataEng.AddComponent<PrototypeEmployeeAgent>();
            var serializedData = new SerializedObject(dataAgent);
            serializedData.FindProperty("animator").objectReferenceValue = dataAnim;
            serializedData.ApplyModifiedPropertiesWithoutUndo();
        }

        // Safety Researcher Preview Setup
        var safetyRes = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Security_Female_01.prefab", "Safety_Researcher_Preview", new Vector3(2.2f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (safetyRes != null)
        {
            safetyRes.SetActive(false);
            var safetyAnim = safetyRes.GetComponentInChildren<Animator>();
            if (safetyAnim != null)
            {
                safetyAnim.applyRootMotion = false;
                if (femController != null) safetyAnim.runtimeAnimatorController = femController;
            }
            var safetyAgent = safetyRes.GetComponent<PrototypeEmployeeAgent>();
            if (safetyAgent == null) safetyAgent = safetyRes.AddComponent<PrototypeEmployeeAgent>();
            var serializedSafety = new SerializedObject(safetyAgent);
            serializedSafety.FindProperty("animator").objectReferenceValue = safetyAnim;
            serializedSafety.ApplyModifiedPropertiesWithoutUndo();
        }

        // Infrastructure Engineer Preview Setup
        var infraEng = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Business_Male_01.prefab", "Infrastructure_Engineer_Preview", new Vector3(2.6f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (infraEng != null)
        {
            infraEng.SetActive(false);
            var infraAnim = infraEng.GetComponentInChildren<Animator>();
            if (infraAnim != null)
            {
                infraAnim.applyRootMotion = false;
                if (mascController != null) infraAnim.runtimeAnimatorController = mascController;
            }
            var infraAgent = infraEng.GetComponent<PrototypeEmployeeAgent>();
            if (infraAgent == null) infraAgent = infraEng.AddComponent<PrototypeEmployeeAgent>();
            var serializedInfra = new SerializedObject(infraAgent);
            serializedInfra.FindProperty("animator").objectReferenceValue = infraAnim;
            serializedInfra.ApplyModifiedPropertiesWithoutUndo();
        }

        // GPU Technician Preview Setup
        var gpuTech = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Security_Male_01.prefab", "GPU_Technician_Preview", new Vector3(3.0f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (gpuTech != null)
        {
            gpuTech.SetActive(false);
            var gpuAnim = gpuTech.GetComponentInChildren<Animator>();
            if (gpuAnim != null)
            {
                gpuAnim.applyRootMotion = false;
                if (mascController != null) gpuAnim.runtimeAnimatorController = mascController;
            }
            var gpuAgent = gpuTech.GetComponent<PrototypeEmployeeAgent>();
            if (gpuAgent == null) gpuAgent = gpuTech.AddComponent<PrototypeEmployeeAgent>();
            var serializedGpu = new SerializedObject(gpuAgent);
            serializedGpu.FindProperty("animator").objectReferenceValue = gpuAnim;
            serializedGpu.ApplyModifiedPropertiesWithoutUndo();
        }

        // MLOps Engineer Preview Setup
        var mlopsEng = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Business_Female_01.prefab", "MLOps_Engineer_Preview", new Vector3(3.4f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (mlopsEng != null)
        {
            mlopsEng.SetActive(false);
            var mlopsAnim = mlopsEng.GetComponentInChildren<Animator>();
            if (mlopsAnim != null)
            {
                mlopsAnim.applyRootMotion = false;
                if (femController != null) mlopsAnim.runtimeAnimatorController = femController;
            }
            var mlopsAgent = mlopsEng.GetComponent<PrototypeEmployeeAgent>();
            if (mlopsAgent == null) mlopsAgent = mlopsEng.AddComponent<PrototypeEmployeeAgent>();
            var serializedMlops = new SerializedObject(mlopsAgent);
            serializedMlops.FindProperty("animator").objectReferenceValue = mlopsAnim;
            serializedMlops.ApplyModifiedPropertiesWithoutUndo();
        }

        // Backend Engineer Preview Setup
        var backendEng = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Business_Male_02.prefab", "Backend_Engineer_Preview", new Vector3(3.8f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (backendEng != null)
        {
            backendEng.SetActive(false);
            var backendAnim = backendEng.GetComponentInChildren<Animator>();
            if (backendAnim != null)
            {
                backendAnim.applyRootMotion = false;
                if (mascController != null) backendAnim.runtimeAnimatorController = mascController;
            }
            var backendAgent = backendEng.GetComponent<PrototypeEmployeeAgent>();
            if (backendAgent == null) backendAgent = backendEng.AddComponent<PrototypeEmployeeAgent>();
            var serializedBackend = new SerializedObject(backendAgent);
            serializedBackend.FindProperty("animator").objectReferenceValue = backendAnim;
            serializedBackend.ApplyModifiedPropertiesWithoutUndo();
        }

        // Finance Lead Preview Setup
        var financeLead = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Business_Male_01.prefab", "Finance_Lead_Preview", new Vector3(4.2f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (financeLead != null)
        {
            financeLead.SetActive(false);
            var financeAnim = financeLead.GetComponentInChildren<Animator>();
            if (financeAnim != null)
            {
                financeAnim.applyRootMotion = false;
                if (mascController != null) financeAnim.runtimeAnimatorController = mascController;
            }
            var financeAgent = financeLead.GetComponent<PrototypeEmployeeAgent>();
            if (financeAgent == null) financeAgent = financeLead.AddComponent<PrototypeEmployeeAgent>();
            var serializedFinance = new SerializedObject(financeAgent);
            serializedFinance.FindProperty("animator").objectReferenceValue = financeAnim;
            serializedFinance.ApplyModifiedPropertiesWithoutUndo();
        }

        // Recruiter Preview Setup
        var recruiter = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Business_Female_01.prefab", "Recruiter_Preview", new Vector3(4.6f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (recruiter != null)
        {
            recruiter.SetActive(false);
            var recruiterAnim = recruiter.GetComponentInChildren<Animator>();
            if (recruiterAnim != null)
            {
                recruiterAnim.applyRootMotion = false;
                if (femController != null) recruiterAnim.runtimeAnimatorController = femController;
            }
            var recruiterAgent = recruiter.GetComponent<PrototypeEmployeeAgent>();
            if (recruiterAgent == null) recruiterAgent = recruiter.AddComponent<PrototypeEmployeeAgent>();
            var serializedRecruiter = new SerializedObject(recruiterAgent);
            serializedRecruiter.FindProperty("animator").objectReferenceValue = recruiterAnim;
            serializedRecruiter.ApplyModifiedPropertiesWithoutUndo();
        }

        // Product Manager Preview Setup
        var pm = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Female_01.prefab", "Product_Manager_Preview", new Vector3(5.0f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (pm != null)
        {
            pm.SetActive(false);
            var pmAnim = pm.GetComponentInChildren<Animator>();
            if (pmAnim != null)
            {
                pmAnim.applyRootMotion = false;
                if (femController != null) pmAnim.runtimeAnimatorController = femController;
            }
            var pmAgent = pm.GetComponent<PrototypeEmployeeAgent>();
            if (pmAgent == null) pmAgent = pm.AddComponent<PrototypeEmployeeAgent>();
            var serializedPM = new SerializedObject(pmAgent);
            serializedPM.FindProperty("animator").objectReferenceValue = pmAnim;
            serializedPM.ApplyModifiedPropertiesWithoutUndo();
        }

        // Sales Executive Preview Setup
        var salesExec = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Business_Male_02.prefab", "Sales_Executive_Preview", new Vector3(5.4f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (salesExec != null)
        {
            salesExec.SetActive(false);
            var salesAnim = salesExec.GetComponentInChildren<Animator>();
            if (salesAnim != null)
            {
                salesAnim.applyRootMotion = false;
                if (mascController != null) salesAnim.runtimeAnimatorController = mascController;
            }
            var salesAgent = salesExec.GetComponent<PrototypeEmployeeAgent>();
            if (salesAgent == null) salesAgent = salesExec.AddComponent<PrototypeEmployeeAgent>();
            var serializedSales = new SerializedObject(salesAgent);
            serializedSales.FindProperty("animator").objectReferenceValue = salesAnim;
            serializedSales.ApplyModifiedPropertiesWithoutUndo();
        }

        // Community Manager Preview Setup
        var communityMgr = PlacePrefab("Assets/PolygonOffice/Prefabs/Characters/SM_Chr_Developer_Female_02.prefab", "Community_Manager_Preview", new Vector3(5.8f, 0f, -2.2f), new Vector3(0f, -35f, 0f), Vector3.one);
        if (communityMgr != null)
        {
            communityMgr.SetActive(false);
            var communityAnim = communityMgr.GetComponentInChildren<Animator>();
            if (communityAnim != null)
            {
                communityAnim.applyRootMotion = false;
                if (femController != null) communityAnim.runtimeAnimatorController = femController;
            }
            var communityAgent = communityMgr.GetComponent<PrototypeEmployeeAgent>();
            if (communityAgent == null) communityAgent = communityMgr.AddComponent<PrototypeEmployeeAgent>();
            var serializedCommunity = new SerializedObject(communityAgent);
            serializedCommunity.FindProperty("animator").objectReferenceValue = communityAnim;
            serializedCommunity.ApplyModifiedPropertiesWithoutUndo();
        }

        serializedAgent.ApplyModifiedPropertiesWithoutUndo();

        var serializedWorkstation = new SerializedObject(workstation);
        serializedWorkstation.FindProperty("assignedAgent").objectReferenceValue = agent;
        serializedWorkstation.ApplyModifiedPropertiesWithoutUndo();
        return agent;
    }

    private static PrototypeWorkstation CreateWorkstation(string name, Vector3 colliderCenter, Vector3 approachPos, Vector3 typingPos)
    {
        var station = new GameObject(name);
        var collider = station.AddComponent<BoxCollider>();
        collider.center = colliderCenter;
        collider.size = new Vector3(2.5f, 1.6f, 1.4f);

        var approach = CreateWaypoint(name + " Approach", approachPos);
        var work = CreateWaypoint(name + " Typing Point", typingPos);
        work.rotation = Quaternion.Euler(0f, 0f, 0f);

        var workstation = station.AddComponent<PrototypeWorkstation>();
        var serializedWorkstation = new SerializedObject(workstation);
        serializedWorkstation.FindProperty("approachPoint").objectReferenceValue = approach;
        serializedWorkstation.FindProperty("workPoint").objectReferenceValue = work;
        serializedWorkstation.ApplyModifiedPropertiesWithoutUndo();

        return workstation;
    }

    private static Transform CreateWaypoint(string name, Vector3 position)
    {
        var point = new GameObject(name);
        point.transform.position = position;
        return point.transform;
    }

    private static void ConvertPolygonMaterialsToUrp()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) return;

        var folders = new[] { "Assets/PolygonOffice", "Assets/Synty/PolygonIcons" };
        foreach (var guid in AssetDatabase.FindAssets("t:Material", folders))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null || material.shader == shader)
            {
                continue;
            }

            material.shader = shader;
            EditorUtility.SetDirty(material);
        }
        AssetDatabase.SaveAssets();
    }

    private static void ConvertIconsToSprites()
    {
        var guids = AssetDatabase.FindAssets("t:Texture2D", new[] { "Assets/Synty/PolygonIcons/Textures" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null && importer.textureType != TextureImporterType.Sprite)
            {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.alphaIsTransparency = true;
                importer.SaveAndReimport();
            }
        }
    }

    // ── UI Helper Methods ─────────────────────────────────────────────
    private static RectTransform CreateUiRect(string name, Transform parent, Vector2 size, Vector2 position, Color color)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        var image = obj.AddComponent<Image>();
        image.color = color;
        var rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return rect;
    }

    private static RectTransform CreatePremiumUiPanel(string name, Transform parent, Vector2 size, Vector2 position, Color bgColor, Color borderColor)
    {
        // 1. Parent object representing the panel container & acting as the 1px border outline
        var panelObj = new GameObject(name);
        panelObj.transform.SetParent(parent, false);
        var rect = panelObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var parentImg = panelObj.AddComponent<Image>();
        parentImg.color = borderColor;

        // 2. Child object representing the inner card body, offset by 1px on all sides
        var bg = new GameObject("InnerBackground");
        bg.transform.SetParent(panelObj.transform, false);
        var bgImg = bg.AddComponent<Image>();
        bgImg.color = bgColor;
        
        var bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.offsetMin = new Vector2(1f, 1f);
        bgRt.offsetMax = new Vector2(-1f, -1f);

        // 3. Glowing neon top accent line (cyber-accent)
        var accentLine = new GameObject("TopAccentLine");
        accentLine.transform.SetParent(panelObj.transform, false);
        var accentImg = accentLine.AddComponent<Image>();
        
        // Color theme based on the panel's name
        if (name.Contains("Project") || name.Contains("Research"))
            accentImg.color = GameDesignConstants.BrandPrimary; // Violet
        else if (name.Contains("Result") || name.Contains("Event"))
            accentImg.color = GameDesignConstants.BrandAccent; // Gold/Amber
        else
            accentImg.color = GameDesignConstants.BrandSecondary; // Cyan

        var accentRt = accentLine.GetComponent<RectTransform>();
        accentRt.anchorMin = new Vector2(0f, 1f);
        accentRt.anchorMax = new Vector2(1f, 1f);
        accentRt.pivot = new Vector2(0.5f, 1f);
        accentRt.sizeDelta = new Vector2(0f, 3f); // 3px high
        accentRt.anchoredPosition = Vector2.zero;
        accentRt.offsetMin = new Vector2(1f, -3f);
        accentRt.offsetMax = new Vector2(-1f, 0f);
        
        return rect;
    }


    private static RectTransform CreateTMPText(string name, Transform parent, string textContent, float fontSize, TextAlignmentOptions alignment, Color color, Vector2 size, Vector2 position)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        var text = obj.AddComponent<TextMeshProUGUI>();
        text.text = textContent;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        
        var defaultFont = GetDefaultFont();
        if (defaultFont != null)
        {
            text.font = defaultFont;
        }
        
        var rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        return rect;
    }

    private static RectTransform CreateStylizedButton(string name, Transform parent, string label, StylizedButton.ButtonVariant variant, Vector2 size, Vector2 position)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var img = obj.AddComponent<Image>();
        img.color = Color.white; // Base color, overridden by StylizedButton

        var btn = obj.AddComponent<Button>();
        btn.transition = Button.Transition.None;

        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(obj.transform, false);
        var labelTMP = labelObj.AddComponent<TextMeshProUGUI>();
        labelTMP.text = label;
        labelTMP.fontSize = GameDesignConstants.FontButton;
        labelTMP.alignment = TextAlignmentOptions.Center;
        labelTMP.color = variant == StylizedButton.ButtonVariant.Secondary
            ? GameDesignConstants.TextPrimary
            : Color.white;
        labelTMP.font = GetDefaultFont();

        var labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        var stylized = obj.AddComponent<StylizedButton>();
        var serialized = new SerializedObject(stylized);
        serialized.FindProperty("variant").enumValueIndex = (int)variant;
        serialized.FindProperty("backgroundImage").objectReferenceValue = img;
        // glowImage can be null
        serialized.ApplyModifiedPropertiesWithoutUndo();

        return rect;
    }

    private static RectTransform CreateSpeedButton(string name, Transform parent, string label, Vector2 size, Vector2 position)
    {
        var obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        var rect = obj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        var img = obj.AddComponent<Image>();
        img.sprite = uiSprite;
        img.type = Image.Type.Sliced;
        img.color = new Color(1f, 1f, 1f, 0.15f);

        var btn = obj.AddComponent<Button>();

        var hl = new GameObject("Highlight");
        hl.transform.SetParent(obj.transform, false);
        var hlImg = hl.AddComponent<Image>();
        hlImg.sprite = uiSprite;
        hlImg.type = Image.Type.Sliced;
        hlImg.color = GameDesignConstants.BrandSecondary;
        var hlRect = hl.GetComponent<RectTransform>();
        hlRect.anchorMin = Vector2.zero;
        hlRect.anchorMax = Vector2.one;
        hlRect.offsetMin = Vector2.zero;
        hlRect.offsetMax = Vector2.zero;
        hlImg.gameObject.SetActive(false);

        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(obj.transform, false);
        var labelTMP = labelObj.AddComponent<TextMeshProUGUI>();
        labelTMP.text = label;
        labelTMP.fontSize = 12f;
        labelTMP.alignment = TextAlignmentOptions.Center;
        labelTMP.color = Color.white;
        labelTMP.font = GetDefaultFont();

        var labelRect = labelObj.GetComponent<RectTransform>();
        labelRect.anchorMin = Vector2.zero;
        labelRect.anchorMax = Vector2.one;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        return rect;
    }

    private static TMP_InputField CreateInputField(string name, Transform parent, string placeholder, Vector2 size, Vector2 position)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        var rect = root.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var image = root.AddComponent<Image>();
        image.color = GameDesignConstants.SurfaceDarkest;

        // Viewport
        var viewport = new GameObject("TextArea");
        viewport.transform.SetParent(root.transform, false);
        var viewRect = viewport.AddComponent<RectTransform>();
        viewRect.anchorMin = Vector2.zero;
        viewRect.anchorMax = Vector2.one;
        viewRect.offsetMin = new Vector2(10f, 5f);
        viewRect.offsetMax = new Vector2(-10f, -5f);
        viewport.AddComponent<RectMask2D>();

        // Text
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(viewport.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.fontSize = 16f;
        text.color = Color.white;
        text.font = GetDefaultFont();

        // Placeholder
        var placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(viewport.transform, false);
        var placeholderRect = placeholderObj.AddComponent<RectTransform>();
        placeholderRect.anchorMin = Vector2.zero;
        placeholderRect.anchorMax = Vector2.one;
        placeholderRect.offsetMin = Vector2.zero;
        placeholderRect.offsetMax = Vector2.zero;
        var placeholderText = placeholderObj.AddComponent<TextMeshProUGUI>();
        placeholderText.text = placeholder;
        placeholderText.fontSize = 16f;
        placeholderText.color = new Color(1f, 1f, 1f, 0.35f);
        placeholderText.font = GetDefaultFont();
        placeholderText.fontStyle = FontStyles.Italic;

        var inputField = root.AddComponent<TMP_InputField>();
        inputField.textViewport = viewRect;
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;
        
        return inputField;
    }

    private static ResourceBar CreateResourceBar(string name, Transform parent, string label, Color color, string prefix, string suffix, float maxVal, Vector2 size, Vector2 position, Sprite iconSprite = null)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        var rect = root.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        // 1. Outline Border
        var borderObj = new GameObject("Border");
        borderObj.transform.SetParent(root.transform, false);
        var borderImg = borderObj.AddComponent<Image>();
        borderImg.sprite = uiSprite;
        borderImg.type = Image.Type.Sliced;
        borderImg.color = new Color(0.82f, 0.84f, 0.87f); // thin light grey border
        var borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;

        // 2. Inner Background
        var bgObj = new GameObject("InnerBackground");
        bgObj.transform.SetParent(borderObj.transform, false);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.sprite = uiSprite;
        bgImg.type = Image.Type.Sliced;
        bgImg.color = Color.white; // clean white body
        var bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = new Vector2(1f, 1f);
        bgRect.offsetMax = new Vector2(-1f, -1f);

        // 3. Icon
        var iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(bgObj.transform, false);
        var iconImg = iconObj.AddComponent<Image>();
        if (iconSprite != null)
        {
            iconImg.sprite = iconSprite;
            iconImg.color = color;
        }
        else
        {
            iconImg.color = color;
        }
        var iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.sizeDelta = new Vector2(18f, 18f);
        iconRect.anchoredPosition = new Vector2(8f, 0f);

        // 4. Text Content (Combining Label and Value in a single text or layout)
        var labelTMP = CreateTMPText("LabelText", bgObj.transform, label, 11f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(size.x - 30f, 24f), new Vector2(30f, 0f));
        labelTMP.anchorMin = new Vector2(0f, 0.5f);
        labelTMP.anchorMax = new Vector2(0f, 0.5f);
        labelTMP.pivot = new Vector2(0f, 0.5f);

        var valueTMP = CreateTMPText("ValueText", bgObj.transform, prefix + "0" + suffix, 11f, TextAlignmentOptions.Right, GameDesignConstants.TextPrimary, new Vector2(80f, 24f), new Vector2(-8f, 0f));
        valueTMP.anchorMin = new Vector2(1f, 0.5f);
        valueTMP.anchorMax = new Vector2(1f, 0.5f);
        valueTMP.pivot = new Vector2(1f, 0.5f);

        var bar = root.AddComponent<ResourceBar>();
        var serialized = new SerializedObject(bar);
        serialized.FindProperty("iconImage").objectReferenceValue = iconImg;
        serialized.FindProperty("labelText").objectReferenceValue = labelTMP.GetComponent<TextMeshProUGUI>();
        serialized.FindProperty("valueText").objectReferenceValue = valueTMP.GetComponent<TextMeshProUGUI>();
        serialized.FindProperty("backgroundImage").objectReferenceValue = bgImg;
        serialized.FindProperty("label").stringValue = label;
        serialized.FindProperty("barColor").colorValue = color;
        serialized.FindProperty("prefix").stringValue = prefix;
        serialized.FindProperty("suffix").stringValue = suffix;
        serialized.FindProperty("maxValue").floatValue = maxVal;
        serialized.ApplyModifiedPropertiesWithoutUndo();

        return bar;
    }

    private static GameObject CreateUiPill(string name, Transform parent, string label, Sprite iconSprite, Color color, Vector2 size, Vector2 position)
    {
        var root = new GameObject(name);
        root.transform.SetParent(parent, false);
        var rect = root.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;

        var uiSprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

        // 1. Outline Border
        var borderObj = new GameObject("Border");
        borderObj.transform.SetParent(root.transform, false);
        var borderImg = borderObj.AddComponent<Image>();
        borderImg.sprite = uiSprite;
        borderImg.type = Image.Type.Sliced;
        borderImg.color = new Color(0.82f, 0.84f, 0.87f); // thin light grey border
        var borderRect = borderObj.GetComponent<RectTransform>();
        borderRect.anchorMin = Vector2.zero;
        borderRect.anchorMax = Vector2.one;
        borderRect.offsetMin = Vector2.zero;
        borderRect.offsetMax = Vector2.zero;

        // 2. Inner Background
        var bgObj = new GameObject("InnerBackground");
        bgObj.transform.SetParent(borderObj.transform, false);
        var bgImg = bgObj.AddComponent<Image>();
        bgImg.sprite = uiSprite;
        bgImg.type = Image.Type.Sliced;
        bgImg.color = Color.white; // clean white body
        var bgRect = bgObj.GetComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = new Vector2(1f, 1f);
        bgRect.offsetMax = new Vector2(-1f, -1f);

        // 3. Icon
        var iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(bgObj.transform, false);
        var iconImg = iconObj.AddComponent<Image>();
        if (iconSprite != null)
        {
            iconImg.sprite = iconSprite;
            iconImg.color = color;
        }
        else
        {
            iconImg.color = color;
        }
        var iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0f, 0.5f);
        iconRect.anchorMax = new Vector2(0f, 0.5f);
        iconRect.pivot = new Vector2(0f, 0.5f);
        iconRect.sizeDelta = new Vector2(18f, 18f);
        iconRect.anchoredPosition = new Vector2(8f, 0f);

        // 4. Combined Label & Value Text
        var labelTMP = CreateTMPText("LabelText", bgObj.transform, label, 11f, TextAlignmentOptions.Left, GameDesignConstants.TextPrimary, new Vector2(size.x - 30f, 24f), new Vector2(30f, 0f));
        labelTMP.anchorMin = new Vector2(0f, 0.5f);
        labelTMP.anchorMax = new Vector2(0f, 0.5f);
        labelTMP.pivot = new Vector2(0f, 0.5f);

        return root;
    }

    private static Image CreateDockHighlight(RectTransform parentBtn)
    {
        var existing = parentBtn.Find("Highlight");
        if (existing != null) return existing.GetComponent<Image>();
        var hl = new GameObject("Highlight");
        hl.transform.SetParent(parentBtn, false);
        var img = hl.AddComponent<Image>();
        img.color = Color.clear;
        var r = hl.GetComponent<RectTransform>();
        r.anchorMin = Vector2.zero;
        r.anchorMax = Vector2.one;
        r.offsetMin = Vector2.zero;
        r.offsetMax = Vector2.zero;
        return img;
    }

    private static TMP_FontAsset GetDefaultFont()
    {
        var font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");
        if (font == null)
        {
            var guids = AssetDatabase.FindAssets("t:TMP_FontAsset");
            if (guids.Length > 0)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(path);
            }
        }
        return font;
    }

    // ── ADDITIONAL PROCEDURAL PANEL BUILDERS (PHASE 4) ────────────────
    private static CanvasGroup BuildHiringPanel(
        Transform canvasParent, 
        out Button mlBtn, 
        out Button sciBtn, 
        out Button dataBtn, 
        out Button safetyBtn,
        out Button infraBtn,
        out Button gpuTechBtn,
        out Button mlopsBtn,
        out Button backendBtn,
        out Button financeBtn,
        out Button recruiterBtn,
        out Button pmBtn,
        out Button salesBtn,
        out Button communityBtn)
    {
        var panelObj = new GameObject("HiringPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        CreateTMPText("Title", header, "Talent Acquisition", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        CreateUiRect("HeaderSeparator", header, new Vector2(420f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        var scrollObj = new GameObject("Scroll View");
        scrollObj.transform.SetParent(panelObj.transform, false);
        var scrollRt = scrollObj.AddComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = new Vector2(0f, -52f);

        var scrollImg = scrollObj.AddComponent<Image>();
        scrollImg.color = Color.black;

        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 20f;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewport.AddComponent<RectMask2D>();
        scrollRect.viewport = viewportRt;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0f, 0f);

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 15f;
        vlg.padding = new RectOffset(15, 15, 15, 15);
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRt;

        CreateHiringRow(content.transform, "ML Engineer", "Salary: $1,200/mo\nCost: $5,000\n+40% Training speed, +15 Competence.", out mlBtn);
        CreateHiringRow(content.transform, "Research Scientist", "Salary: $2,500/mo\nCost: $8,000\n+30% Research speed, +25 Competence.\nRequires Office Tier 2.", out sciBtn);
        CreateHiringRow(content.transform, "Data Engineer", "Salary: $1,800/mo\nCost: $6,000\n+20% Model quality, +20 Competence.\nRequires Office Tier 2.", out dataBtn);
        CreateHiringRow(content.transform, "Safety Researcher", "Salary: $3,500/mo\nCost: $12,000\nMitigates data penalty to -5, halves scraped event rate, +25% rep.\nRequires Office Tier 3.", out safetyBtn);
        CreateHiringRow(content.transform, "Infrastructure Engineer", "Salary: $2,800/mo\nCost: $10,000\nReduces total GPU electricity draw by 25%.\nRequires Office Tier 4.", out infraBtn);
        CreateHiringRow(content.transform, "GPU Technician", "Salary: $1,900/mo\nCost: $7,000\nReduces GPU heat by 10% and speeds up training by 10%.\nRequires Office Tier 4.", out gpuTechBtn);
        CreateHiringRow(content.transform, "MLOps Engineer", "Salary: $2,200/mo\nCost: $9,000\nIncreases final model revenue by 20%.\nRequires Office Tier 4.", out mlopsBtn);
        CreateHiringRow(content.transform, "Backend Engineer", "Salary: $1,500/mo\nCost: $6,000\nIncreases active contracts limit from 3 to 4.\nRequires Office Tier 4.", out backendBtn);

        CreateHiringRow(content.transform, "Finance Lead", "Salary: $3,500/mo\nCost: $15,000\n+20% cash from funding rounds, reduces total burn rate by 10%.\nRequires Office Tier 2.", out financeBtn);
        CreateHiringRow(content.transform, "Recruiter", "Salary: $2,500/mo\nCost: $12,000\nReduces hiring costs of all other roles by 30%.\nRequires Office Tier 2.", out recruiterBtn);
        CreateHiringRow(content.transform, "Community Manager", "Salary: $2,000/mo\nCost: $10,000\n+50% organic follower growth, halves inactiveness loss.\nRequires Office Tier 3.", out communityBtn);
        CreateHiringRow(content.transform, "Product Manager", "Salary: $4,000/mo\nCost: $18,000\n+5 base quality to releases, -10% model training time.\nRequires Office Tier 4.", out pmBtn);
        CreateHiringRow(content.transform, "Sales Executive", "Salary: $3,000/mo\nCost: $14,000\n+20% contract payout & model release revenue, +1 active contract slot.\nRequires Office Tier 4.", out salesBtn);

        return cg;
    }

    private static GameObject CreateHiringRow(Transform parent, string title, string details, out Button hireBtn)
    {
        var row = new GameObject("HiringRow");
        row.transform.SetParent(parent, false);
        var rect = row.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(390f, 140f);
        var le = row.AddComponent<LayoutElement>();
        le.preferredHeight = 140f;
        le.minHeight = 140f;

        CreatePremiumUiPanel("RowBG", row.transform, rect.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var nameText = CreateTMPText("RoleName", row.transform, title, 14f, TextAlignmentOptions.Left, Color.white, new Vector2(240f, 20f), new Vector2(15f, 45f));
        nameText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var descText = CreateTMPText("RoleDetails", row.transform, details, 11f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(240f, 80f), new Vector2(15f, -10f));
        descText.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;

        var btnRect = CreateStylizedButton("Btn_Hire", row.transform, "HIRE", StylizedButton.ButtonVariant.Primary, new Vector2(100f, 35f), new Vector2(130f, 0f));
        hireBtn = btnRect.GetComponent<Button>();

        return row;
    }

    private static CanvasGroup BuildUpgradesPanel(Transform canvasParent, out Button buyGpuBtn, out Button t2Btn, out Button t3Btn, out Button t4Btn)
    {
        var panelObj = new GameObject("UpgradesPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        CreateTMPText("Title", header, "Upgrades & Infrastructure", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        CreateUiRect("HeaderSeparator", header, new Vector2(420f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        var gpuCard = CreatePremiumUiPanel("GpuCard", panelObj.transform, new Vector2(380f, 130f), new Vector2(0f, 120f), GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));
        gpuCard.anchorMin = new Vector2(0.5f, 0.5f);
        gpuCard.anchorMax = new Vector2(0.5f, 0.5f);
        gpuCard.pivot = new Vector2(0.5f, 0.5f);
        CreateTMPText("GpuTitle", gpuCard, "GPU Cabinet Expansion", 14f, TextAlignmentOptions.Left, Color.white, new Vector2(220f, 20f), new Vector2(15f, 40f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("GpuDesc", gpuCard, "Install an extra GPU server. Costs $10,000 upfront. Adds $300/mo burn rate.", 11f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(220f, 60f), new Vector2(15f, -5f)).GetComponent<TextMeshProUGUI>().enableWordWrapping = true;
        var buyGpuBtnRect = CreateStylizedButton("Btn_BuyGpu", gpuCard, "BUY GPU ($10k)", StylizedButton.ButtonVariant.Primary, new Vector2(110f, 40f), new Vector2(120f, 0f));
        buyGpuBtn = buyGpuBtnRect.GetComponent<Button>();

        var officeCard = CreatePremiumUiPanel("OfficeCard", panelObj.transform, new Vector2(380f, 380f), new Vector2(0f, -150f), GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));
        officeCard.anchorMin = new Vector2(0.5f, 0.5f);
        officeCard.anchorMax = new Vector2(0.5f, 0.5f);
        officeCard.pivot = new Vector2(0.5f, 0.5f);
        CreateTMPText("OfficeTitle", officeCard, "Office Facility Upgrades", 14f, TextAlignmentOptions.Left, Color.white, new Vector2(350f, 20f), new Vector2(15f, 165f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        CreateTMPText("T2Title", officeCard, "Tier 2: Corporate Suite", 12f, TextAlignmentOptions.Left, Color.white, new Vector2(220f, 20f), new Vector2(15f, 115f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("T2Desc", officeCard, "Unlocks ML Engineer & Scientist. Burn Rate +$1,000/mo.", 10f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(220f, 40f), new Vector2(15f, 85f)).GetComponent<TextMeshProUGUI>().enableWordWrapping = true;
        var t2BtnRect = CreateStylizedButton("Btn_T2Suite", officeCard, "BUY SUITE ($30k)", StylizedButton.ButtonVariant.Primary, new Vector2(110f, 35f), new Vector2(120f, 105f));
        t2Btn = t2BtnRect.GetComponent<Button>();

        CreateTMPText("T3Title", officeCard, "Tier 3: Secret R&D Lab", 12f, TextAlignmentOptions.Left, Color.white, new Vector2(220f, 20f), new Vector2(15f, 30f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("T3Desc", officeCard, "Unlocks Safety Researcher & Advanced R&D tech. Burn Rate +$2,500/mo.", 10f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(220f, 40f), new Vector2(15f, 0f)).GetComponent<TextMeshProUGUI>().enableWordWrapping = true;
        var t3BtnRect = CreateStylizedButton("Btn_T3Lab", officeCard, "BUY LAB ($75k)", StylizedButton.ButtonVariant.Primary, new Vector2(110f, 35f), new Vector2(120f, 20f));
        t3Btn = t3BtnRect.GetComponent<Button>();

        CreateTMPText("T4Title", officeCard, "Tier 4: Modular Datacenter", 12f, TextAlignmentOptions.Left, Color.white, new Vector2(220f, 20f), new Vector2(15f, -55f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        CreateTMPText("T4Desc", officeCard, "Unlocks Datacenter area, NOC system, and 4 new specialists. Burn Rate +$5,000/mo.", 10f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(220f, 40f), new Vector2(15f, -85f)).GetComponent<TextMeshProUGUI>().enableWordWrapping = true;
        var t4BtnRect = CreateStylizedButton("Btn_T4Datacenter", officeCard, "BUY T4 ($120k)", StylizedButton.ButtonVariant.Primary, new Vector2(110f, 35f), new Vector2(120f, -65f));
        t4Btn = t4BtnRect.GetComponent<Button>();

        return cg;
    }

    private static CanvasGroup BuildResearchPanel(Transform canvasParent, ResearchController researchController)
    {
        var panelObj = new GameObject("ResearchPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        CreateTMPText("Title", header, "R&D Department", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var closeBtnRect = CreateStylizedButton("Btn_Close", header, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, -12f));
        closeBtnRect.anchorMin = new Vector2(1f, 1f);
        closeBtnRect.anchorMax = new Vector2(1f, 1f);
        closeBtnRect.pivot = new Vector2(1f, 1f);
        closeBtnRect.anchoredPosition = new Vector2(-20f, -12f);

        CreateUiRect("HeaderSeparator", header, new Vector2(420f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        var scrollObj = new GameObject("Scroll View");
        scrollObj.transform.SetParent(panelObj.transform, false);
        var scrollRt = scrollObj.AddComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = new Vector2(0f, -52f);

        var scrollImg = scrollObj.AddComponent<Image>();
        scrollImg.color = Color.black;

        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 20f;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewport.AddComponent<RectMask2D>();
        scrollRect.viewport = viewportRt;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0f, 0f);

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 15f;
        vlg.padding = new RectOffset(15, 15, 15, 15);
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRt;

        Button btnGeneral, btnNlp, btnAgentic, btnSafety, btnSilicon;
        Image fillGeneral, fillNlp, fillAgentic, fillSafety, fillSilicon;
        TextMeshProUGUI txtGeneral, txtNlp, txtAgentic, txtSafety, txtSilicon;

        CreateResearchRow(content.transform, "Competence Training", "Gain +15 general team competence. Fast study.", out btnGeneral, out fillGeneral, out txtGeneral);
        CreateResearchRow(content.transform, "NLP Chatbots", "Unlock Natural Language Processing models. Enables chatbot projects.", out btnNlp, out fillNlp, out txtNlp);
        CreateResearchRow(content.transform, "Agentic Coders", "Unlock autonomous agentic software development projects. Requires NLP.", out btnAgentic, out fillAgentic, out txtAgentic);
        CreateResearchRow(content.transform, "Safety Alignment", "Adds +10 base quality to all future models & blocks data leaks. Requires Lab.", out btnSafety, out fillSafety, out txtSafety);
        CreateResearchRow(content.transform, "Custom Silicon", "Halves the GPU server burn rate overhead permanently. Requires Lab.", out btnSilicon, out fillSilicon, out txtSilicon);

        var serializedResearch = new SerializedObject(researchController);
        serializedResearch.FindProperty("panelGroup").objectReferenceValue = cg;
        serializedResearch.FindProperty("closeButton").objectReferenceValue = closeBtnRect.GetComponent<Button>();

        serializedResearch.FindProperty("studyNlpButton").objectReferenceValue = btnNlp;
        serializedResearch.FindProperty("studyAgenticButton").objectReferenceValue = btnAgentic;
        serializedResearch.FindProperty("studyGeneralButton").objectReferenceValue = btnGeneral;
        serializedResearch.FindProperty("studySafetyAlignmentButton").objectReferenceValue = btnSafety;
        serializedResearch.FindProperty("studyCustomSiliconButton").objectReferenceValue = btnSilicon;

        serializedResearch.FindProperty("nlpProgressFill").objectReferenceValue = fillNlp;
        serializedResearch.FindProperty("agenticProgressFill").objectReferenceValue = fillAgentic;
        serializedResearch.FindProperty("generalProgressFill").objectReferenceValue = fillGeneral;
        serializedResearch.FindProperty("safetyAlignmentProgressFill").objectReferenceValue = fillSafety;
        serializedResearch.FindProperty("customSiliconProgressFill").objectReferenceValue = fillSilicon;

        serializedResearch.FindProperty("nlpProgressText").objectReferenceValue = txtNlp;
        serializedResearch.FindProperty("agenticProgressText").objectReferenceValue = txtAgentic;
        serializedResearch.FindProperty("generalProgressText").objectReferenceValue = txtGeneral;
        serializedResearch.FindProperty("safetyAlignmentProgressText").objectReferenceValue = txtSafety;
        serializedResearch.FindProperty("customSiliconProgressText").objectReferenceValue = txtSilicon;
        serializedResearch.ApplyModifiedPropertiesWithoutUndo();

        return cg;
    }

    private static void CreateResearchRow(Transform parent, string title, string desc, out Button btn, out Image fillImg, out TextMeshProUGUI txt)
    {
        var row = new GameObject("ResearchRow");
        row.transform.SetParent(parent, false);
        var rect = row.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(390f, 130f);
        var le = row.AddComponent<LayoutElement>();
        le.preferredHeight = 130f;
        le.minHeight = 130f;

        CreatePremiumUiPanel("RowBG", row.transform, rect.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var nameText = CreateTMPText("TechTitle", row.transform, title, 13f, TextAlignmentOptions.Left, Color.white, new Vector2(240f, 20f), new Vector2(15f, 40f));
        nameText.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var descText = CreateTMPText("TechDesc", row.transform, desc, 10f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(240f, 50f), new Vector2(15f, 2f));
        descText.GetComponent<TextMeshProUGUI>().enableWordWrapping = true;

        var progressBack = CreateUiRect("ProgressBack", row.transform, new Vector2(230f, 14f), new Vector2(15f, -40f), GameDesignConstants.ResourceBarBg);
        progressBack.anchorMin = new Vector2(0f, 0.5f);
        progressBack.anchorMax = new Vector2(0f, 0.5f);
        progressBack.pivot = new Vector2(0f, 0.5f);

        var fillObj = new GameObject("Fill");
        fillObj.transform.SetParent(progressBack, false);
        fillImg = fillObj.AddComponent<Image>();
        fillImg.color = GameDesignConstants.BrandPrimary;
        fillImg.type = Image.Type.Filled;
        fillImg.fillMethod = Image.FillMethod.Horizontal;
        fillImg.fillAmount = 0f;
        var fillRt = fillObj.GetComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;

        var pctText = CreateTMPText("PercentText", row.transform, "0%", 10f, TextAlignmentOptions.Left, Color.white, new Vector2(40f, 14f), new Vector2(255f, -40f));
        pctText.anchorMin = new Vector2(0f, 0.5f);
        pctText.anchorMax = new Vector2(0f, 0.5f);
        pctText.pivot = new Vector2(0f, 0.5f);
        txt = pctText.GetComponent<TextMeshProUGUI>();

        var btnRect = CreateStylizedButton("Btn_Study", row.transform, "STUDY", StylizedButton.ButtonVariant.Secondary, new Vector2(100f, 32f), new Vector2(130f, 15f));
        btn = btnRect.GetComponent<Button>();
    }

    private static CanvasGroup BuildAnalyticsPanel(Transform canvasParent, AnalyticsController analyticsController)
    {
        var panelObj = new GameObject("AnalyticsPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(880f, 540f);
        rt.anchoredPosition = Vector2.zero;

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var header = CreateUiRect("Header", panelObj.transform, new Vector2(880f, 50f), new Vector2(0f, 245f), Color.black);
        header.anchorMin = new Vector2(0.5f, 1f);
        header.anchorMax = new Vector2(0.5f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        CreateTMPText("Title", header, "Corporate Performance Dashboard", 18f, TextAlignmentOptions.Left, Color.white, new Vector2(400f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var closeBtnRect = CreateStylizedButton("Btn_Close", header, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, -12f));
        closeBtnRect.anchorMin = new Vector2(1f, 1f);
        closeBtnRect.anchorMax = new Vector2(1f, 1f);
        closeBtnRect.pivot = new Vector2(1f, 1f);
        closeBtnRect.anchoredPosition = new Vector2(-20f, -12f);

        CreateUiRect("HeaderSeparator", header, new Vector2(880f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        var leftGroup = new GameObject("ChartsGroup");
        leftGroup.transform.SetParent(panelObj.transform, false);
        var leftRt = leftGroup.AddComponent<RectTransform>();
        leftRt.anchorMin = new Vector2(0f, 0f);
        leftRt.anchorMax = new Vector2(0f, 1f);
        leftRt.pivot = new Vector2(0f, 0.5f);
        leftRt.offsetMin = new Vector2(20f, 20f);
        leftRt.offsetMax = new Vector2(500f, -70f);

        RectTransform[] cashBars, repBars, compBars;
        TextMeshProUGUI[] cashLabels, repLabels, compLabels;
        TextMeshProUGUI maxCash, maxRep, maxComp;

        BuildChart(leftGroup.transform, "Cash Over Time", new Vector2(0f, 135f), GameDesignConstants.ResourceCash, out cashBars, out cashLabels, out maxCash);
        BuildChart(leftGroup.transform, "Followers Over Time", new Vector2(0f, -15f), GameDesignConstants.ResourceReputation, out repBars, out repLabels, out maxRep);
        BuildChart(leftGroup.transform, "Model Quality History", new Vector2(0f, -165f), GameDesignConstants.BrandAccent, out compBars, out compLabels, out maxComp);

        var rightGroup = new GameObject("StatsGroup");
        rightGroup.transform.SetParent(panelObj.transform, false);
        var rightRt = rightGroup.AddComponent<RectTransform>();
        rightRt.anchorMin = new Vector2(1f, 0f);
        rightRt.anchorMax = new Vector2(1f, 1f);
        rightRt.pivot = new Vector2(1f, 0.5f);
        rightRt.offsetMin = new Vector2(520f, 20f);
        rightRt.offsetMax = new Vector2(-20f, -70f);

        var statsCard = CreatePremiumUiPanel("StatsCard", rightGroup.transform, new Vector2(320f, 120f), new Vector2(0f, 140f), GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));
        statsCard.anchorMin = new Vector2(0.5f, 0.5f);
        statsCard.anchorMax = new Vector2(0.5f, 0.5f);
        statsCard.pivot = new Vector2(0.5f, 0.5f);
        CreateTMPText("StatsTitle", statsCard, "Operational Statistics", 12f, TextAlignmentOptions.Left, GameDesignConstants.BrandSecondary, new Vector2(280f, 20f), new Vector2(15f, 40f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var totalModels = CreateTMPText("TotalModelsText", statsCard, "Total: 0", 11f, TextAlignmentOptions.Left, Color.white, new Vector2(280f, 18f), new Vector2(15f, 15f)).GetComponent<TextMeshProUGUI>();
        var avgQuality = CreateTMPText("AvgQualityText", statsCard, "Avg Quality: -", 11f, TextAlignmentOptions.Left, Color.white, new Vector2(280f, 18f), new Vector2(15f, -5f)).GetComponent<TextMeshProUGUI>();
        var bestModel = CreateTMPText("BestModelText", statsCard, "Best: -", 11f, TextAlignmentOptions.Left, Color.white, new Vector2(280f, 18f), new Vector2(15f, -25f)).GetComponent<TextMeshProUGUI>();

        var historyCard = CreatePremiumUiPanel("HistoryCard", rightGroup.transform, new Vector2(320f, 260f), new Vector2(0f, -70f), GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));
        historyCard.anchorMin = new Vector2(0.5f, 0.5f);
        historyCard.anchorMax = new Vector2(0.5f, 0.5f);
        historyCard.pivot = new Vector2(0.5f, 0.5f);
        CreateTMPText("HistoryTitle", historyCard, "Recent Model Launches", 12f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(280f, 20f), new Vector2(15f, 110f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var recentModels = CreateTMPText("RecentModelsText", historyCard, "No models launched yet.", 11f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(280f, 190f), new Vector2(15f, 10f)).GetComponent<TextMeshProUGUI>();
        recentModels.enableWordWrapping = true;

        var serializedAnalytics = new SerializedObject(analyticsController);
        serializedAnalytics.FindProperty("panelGroup").objectReferenceValue = cg;
        serializedAnalytics.FindProperty("closeButton").objectReferenceValue = closeBtnRect.GetComponent<Button>();

        var cashBarsProp = serializedAnalytics.FindProperty("cashChartBars");
        cashBarsProp.arraySize = 8;
        var cashLabelsProp = serializedAnalytics.FindProperty("cashChartLabels");
        cashLabelsProp.arraySize = 8;
        for (int i = 0; i < 8; i++)
        {
            cashBarsProp.GetArrayElementAtIndex(i).objectReferenceValue = cashBars[i];
            cashLabelsProp.GetArrayElementAtIndex(i).objectReferenceValue = cashLabels[i];
        }
        serializedAnalytics.FindProperty("maxCashLabel").objectReferenceValue = maxCash;

        var repBarsProp = serializedAnalytics.FindProperty("repChartBars");
        repBarsProp.arraySize = 8;
        var repLabelsProp = serializedAnalytics.FindProperty("repChartLabels");
        repLabelsProp.arraySize = 8;
        for (int i = 0; i < 8; i++)
        {
            repBarsProp.GetArrayElementAtIndex(i).objectReferenceValue = repBars[i];
            repLabelsProp.GetArrayElementAtIndex(i).objectReferenceValue = repLabels[i];
        }
        serializedAnalytics.FindProperty("maxRepLabel").objectReferenceValue = maxRep;

        var compBarsProp = serializedAnalytics.FindProperty("compChartBars");
        compBarsProp.arraySize = 8;
        var compLabelsProp = serializedAnalytics.FindProperty("compChartLabels");
        compLabelsProp.arraySize = 8;
        for (int i = 0; i < 8; i++)
        {
            compBarsProp.GetArrayElementAtIndex(i).objectReferenceValue = compBars[i];
            compLabelsProp.GetArrayElementAtIndex(i).objectReferenceValue = compLabels[i];
        }
        serializedAnalytics.FindProperty("maxCompLabel").objectReferenceValue = maxComp;

        serializedAnalytics.FindProperty("recentModelsText").objectReferenceValue = recentModels;
        serializedAnalytics.FindProperty("totalModelsText").objectReferenceValue = totalModels;
        serializedAnalytics.FindProperty("avgQualityText").objectReferenceValue = avgQuality;
        serializedAnalytics.FindProperty("bestModelText").objectReferenceValue = bestModel;
        serializedAnalytics.ApplyModifiedPropertiesWithoutUndo();

        return cg;
    }

    private static void BuildChart(Transform parent, string title, Vector2 position, Color barColor, out RectTransform[] bars, out TextMeshProUGUI[] labels, out TextMeshProUGUI maxLabel)
    {
        var chartCard = CreatePremiumUiPanel("ChartCard", parent, new Vector2(460f, 130f), position, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));
        chartCard.anchorMin = new Vector2(0.5f, 0.5f);
        chartCard.anchorMax = new Vector2(0.5f, 0.5f);
        chartCard.pivot = new Vector2(0.5f, 0.5f);

        CreateTMPText("ChartTitle", chartCard, title, 11f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 20f), new Vector2(15f, 48f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        maxLabel = CreateTMPText("MaxLabel", chartCard, "--", 10f, TextAlignmentOptions.Right, GameDesignConstants.TextMuted, new Vector2(150f, 20f), new Vector2(290f, 48f)).GetComponent<TextMeshProUGUI>();

        bars = new RectTransform[8];
        labels = new TextMeshProUGUI[8];

        var barsContainer = new GameObject("BarsContainer");
        barsContainer.transform.SetParent(chartCard, false);
        var containerRt = barsContainer.AddComponent<RectTransform>();
        containerRt.anchorMin = Vector2.zero;
        containerRt.anchorMax = Vector2.one;
        containerRt.offsetMin = new Vector2(15f, 20f);
        containerRt.offsetMax = new Vector2(-15f, -30f);

        float spacing = 430f / 8f;
        for (int i = 0; i < 8; i++)
        {
            float posX = i * spacing + 10f;
            
            var slot = CreateUiRect($"Slot_{i}", barsContainer.transform, new Vector2(12f, 70f), new Vector2(posX, 0f), new Color(1f, 1f, 1f, 0.05f));
            slot.anchorMin = new Vector2(0f, 0.5f);
            slot.anchorMax = new Vector2(0f, 0.5f);
            slot.pivot = new Vector2(0.5f, 0.5f);

            var bar = new GameObject($"Bar_{i}");
            bar.transform.SetParent(slot, false);
            var barImg = bar.AddComponent<Image>();
            barImg.color = barColor;
            var barRt = bar.GetComponent<RectTransform>();
            barRt.anchorMin = new Vector2(0.5f, 0f);
            barRt.anchorMax = new Vector2(0.5f, 0f);
            barRt.pivot = new Vector2(0.5f, 0f);
            barRt.sizeDelta = new Vector2(12f, 0f);
            barRt.anchoredPosition = Vector2.zero;
            bars[i] = barRt;

            var lbl = CreateTMPText($"Label_{i}", barsContainer.transform, "-", 8f, TextAlignmentOptions.Center, GameDesignConstants.TextMuted, new Vector2(40f, 14f), new Vector2(posX, -15f));
            lbl.anchorMin = new Vector2(0f, 0.5f);
            lbl.anchorMax = new Vector2(0f, 0.5f);
            lbl.pivot = new Vector2(0.5f, 0.5f);
            labels[i] = lbl.GetComponent<TextMeshProUGUI>();
        }
    }

    private static CanvasGroup BuildSystemPanel(Transform canvasParent, out Button quitBtn)
    {
        var panelObj = new GameObject("SystemPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(360f, 260f);
        rt.anchoredPosition = Vector2.zero;

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var header = CreateUiRect("Header", panelObj.transform, new Vector2(360f, 50f), new Vector2(0f, 105f), Color.black);
        header.anchorMin = new Vector2(0.5f, 1f);
        header.anchorMax = new Vector2(0.5f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        CreateTMPText("Title", header, "System Panel", 16f, TextAlignmentOptions.Center, Color.white, new Vector2(250f, 30f), Vector2.zero).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        CreateUiRect("HeaderSeparator", header, new Vector2(360f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        var saveBtnRect = CreateStylizedButton("Btn_SaveGame", panelObj.transform, "SAVE SYSTEM STATE", StylizedButton.ButtonVariant.Primary, new Vector2(280f, 42f), new Vector2(0f, 40f));
        var loadBtnRect = CreateStylizedButton("Btn_LoadGame", panelObj.transform, "LOAD SYSTEM STATE", StylizedButton.ButtonVariant.Secondary, new Vector2(280f, 42f), new Vector2(0f, -15f));
        var quitBtnRect = CreateStylizedButton("Btn_QuitToMenu", panelObj.transform, "QUIT TO MAIN MENU", StylizedButton.ButtonVariant.Danger, new Vector2(280f, 42f), new Vector2(0f, -70f));

        quitBtn = quitBtnRect.GetComponent<Button>();

        return cg;
    }

    private static CanvasGroup BuildContractsPanel(Transform canvasParent, ContractController contractController)
    {
        var panelObj = new GameObject("ContractsPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        CreateTMPText("Title", header, "Commercial Contracts", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var closeBtnRect = CreateStylizedButton("Btn_Close", header, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, -12f));
        closeBtnRect.anchorMin = new Vector2(1f, 1f);
        closeBtnRect.anchorMax = new Vector2(1f, 1f);
        closeBtnRect.pivot = new Vector2(1f, 1f);
        closeBtnRect.anchoredPosition = new Vector2(-20f, -12f);

        CreateUiRect("HeaderSeparator", header, new Vector2(420f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        var scrollObj = new GameObject("Scroll View");
        scrollObj.transform.SetParent(panelObj.transform, false);
        var scrollRt = scrollObj.AddComponent<RectTransform>();
        scrollRt.anchorMin = Vector2.zero;
        scrollRt.anchorMax = Vector2.one;
        scrollRt.offsetMin = Vector2.zero;
        scrollRt.offsetMax = new Vector2(0f, -52f);

        var scrollImg = scrollObj.AddComponent<Image>();
        scrollImg.color = Color.clear;

        var scrollRect = scrollObj.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 20f;

        var viewport = new GameObject("Viewport");
        viewport.transform.SetParent(scrollObj.transform, false);
        var viewportRt = viewport.AddComponent<RectTransform>();
        viewportRt.anchorMin = Vector2.zero;
        viewportRt.anchorMax = Vector2.one;
        viewportRt.offsetMin = Vector2.zero;
        viewportRt.offsetMax = Vector2.zero;
        viewport.AddComponent<RectMask2D>();
        scrollRect.viewport = viewportRt;

        var content = new GameObject("Content");
        content.transform.SetParent(viewport.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0f, 1f);
        contentRt.anchorMax = new Vector2(1f, 1f);
        contentRt.pivot = new Vector2(0.5f, 1f);
        contentRt.sizeDelta = new Vector2(0f, 0f);

        var vlg = content.AddComponent<VerticalLayoutGroup>();
        vlg.childControlHeight = false;
        vlg.childControlWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 15f;
        vlg.padding = new RectOffset(15, 15, 15, 15);
        var csf = content.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        scrollRect.content = contentRt;

        var serializedController = new SerializedObject(contractController);
        serializedController.FindProperty("panelGroup").objectReferenceValue = cg;
        serializedController.FindProperty("closeButton").objectReferenceValue = closeBtnRect.GetComponent<Button>();
        serializedController.FindProperty("scrollContent").objectReferenceValue = contentRt;
        serializedController.ApplyModifiedPropertiesWithoutUndo();

        return cg;
    }

    private static CanvasGroup BuildNOCPanel(
        Transform canvasParent,
        out Button gridUpgradeBtn,
        out Button coolingUpgradeBtn,
        out Button closeBtn,
        out TextMeshProUGUI gpuCountTxt,
        out TextMeshProUGUI energyTxt,
        out Image energyBarFill,
        out TextMeshProUGUI coolingTxt,
        out Image coolingBarFill,
        out TextMeshProUGUI statusTxt,
        out Image statusBg,
        out TextMeshProUGUI warningTxt)
    {
        var panelObj = new GameObject("NOCPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        // Header
        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        CreateTMPText("Title", header, "NOC Operations", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var closeBtnRect = CreateStylizedButton("Btn_Close", header, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, -12f));
        closeBtnRect.anchorMin = new Vector2(1f, 1f);
        closeBtnRect.anchorMax = new Vector2(1f, 1f);
        closeBtnRect.pivot = new Vector2(1f, 1f);
        closeBtnRect.anchoredPosition = new Vector2(-20f, -12f);
        closeBtn = closeBtnRect.GetComponent<Button>();

        CreateUiRect("HeaderSeparator", header, new Vector2(420f, 1f), new Vector2(0f, -50f), new Color(0.18f, 0.20f, 0.21f));

        // GPU Count text
        var gpuCountRect = CreateTMPText("GpuCountText", panelObj.transform, "Active GPUs: 1", 14f, TextAlignmentOptions.Left, Color.white, new Vector2(380f, 25f), new Vector2(20f, 420f));
        gpuCountRect.anchorMin = new Vector2(0f, 0.5f);
        gpuCountRect.anchorMax = new Vector2(1f, 0.5f);
        gpuCountRect.pivot = new Vector2(0.5f, 0.5f);
        gpuCountTxt = gpuCountRect.GetComponent<TextMeshProUGUI>();
        gpuCountTxt.fontStyle = FontStyles.Bold;

        // Energy Bar
        var energyLabelRect = CreateTMPText("EnergyLabel", panelObj.transform, "Energy Load: 0kW / 0kW", 13f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(380f, 20f), new Vector2(20f, 370f));
        energyLabelRect.anchorMin = new Vector2(0f, 0.5f);
        energyLabelRect.anchorMax = new Vector2(1f, 0.5f);
        energyLabelRect.pivot = new Vector2(0.5f, 0.5f);
        energyTxt = energyLabelRect.GetComponent<TextMeshProUGUI>();

        var energyBarBg = CreateUiRect("EnergyBarBg", panelObj.transform, new Vector2(380f, 20f), new Vector2(20f, 340f), GameDesignConstants.ResourceBarBg);
        energyBarBg.anchorMin = new Vector2(0f, 0.5f);
        energyBarBg.anchorMax = new Vector2(1f, 0.5f);
        energyBarBg.pivot = new Vector2(0.5f, 0.5f);

        var energyFillObj = new GameObject("EnergyFill");
        energyFillObj.transform.SetParent(energyBarBg, false);
        energyBarFill = energyFillObj.AddComponent<Image>();
        energyBarFill.color = GameDesignConstants.StatusSuccess;
        energyBarFill.type = Image.Type.Filled;
        energyBarFill.fillMethod = Image.FillMethod.Horizontal;
        energyBarFill.fillAmount = 0f;
        var energyFillRect = energyFillObj.GetComponent<RectTransform>();
        energyFillRect.anchorMin = Vector2.zero;
        energyFillRect.anchorMax = Vector2.one;
        energyFillRect.offsetMin = Vector2.zero;
        energyFillRect.offsetMax = Vector2.zero;

        // Cooling Bar
        var coolingLabelRect = CreateTMPText("CoolingLabel", panelObj.transform, "Cooling Load: 0kW / 0kW", 13f, TextAlignmentOptions.Left, GameDesignConstants.TextSecondary, new Vector2(380f, 20f), new Vector2(20f, 290f));
        coolingLabelRect.anchorMin = new Vector2(0f, 0.5f);
        coolingLabelRect.anchorMax = new Vector2(1f, 0.5f);
        coolingLabelRect.pivot = new Vector2(0.5f, 0.5f);
        coolingTxt = coolingLabelRect.GetComponent<TextMeshProUGUI>();

        var coolingBarBg = CreateUiRect("CoolingBarBg", panelObj.transform, new Vector2(380f, 20f), new Vector2(20f, 260f), GameDesignConstants.ResourceBarBg);
        coolingBarBg.anchorMin = new Vector2(0f, 0.5f);
        coolingBarBg.anchorMax = new Vector2(1f, 0.5f);
        coolingBarBg.pivot = new Vector2(0.5f, 0.5f);

        var coolingFillObj = new GameObject("CoolingFill");
        coolingFillObj.transform.SetParent(coolingBarBg, false);
        coolingBarFill = coolingFillObj.AddComponent<Image>();
        coolingBarFill.color = GameDesignConstants.StatusSuccess;
        coolingBarFill.type = Image.Type.Filled;
        coolingBarFill.fillMethod = Image.FillMethod.Horizontal;
        coolingBarFill.fillAmount = 0f;
        var coolingFillRect = coolingFillObj.GetComponent<RectTransform>();
        coolingFillRect.anchorMin = Vector2.zero;
        coolingFillRect.anchorMax = Vector2.one;
        coolingFillRect.offsetMin = Vector2.zero;
        coolingFillRect.offsetMax = Vector2.zero;

        // Status Indicator Box
        var statusBoxRect = CreateUiRect("StatusBox", panelObj.transform, new Vector2(380f, 40f), new Vector2(20f, 190f), GameDesignConstants.StatusSuccess);
        statusBoxRect.anchorMin = new Vector2(0f, 0.5f);
        statusBoxRect.anchorMax = new Vector2(1f, 0.5f);
        statusBoxRect.pivot = new Vector2(0.5f, 0.5f);
        statusBg = statusBoxRect.GetComponent<Image>();

        var statusTextRect = CreateTMPText("StatusText", statusBoxRect, "OPTIMAL", 14f, TextAlignmentOptions.Center, Color.white, new Vector2(360f, 30f), Vector2.zero);
        statusTextRect.anchorMin = new Vector2(0.5f, 0.5f);
        statusTextRect.anchorMax = new Vector2(0.5f, 0.5f);
        statusTextRect.pivot = new Vector2(0.5f, 0.5f);
        statusTxt = statusTextRect.GetComponent<TextMeshProUGUI>();
        statusTxt.fontStyle = FontStyles.Bold;

        // Warning Text Box
        var warningBoxRect = CreateTMPText("WarningText", panelObj.transform, "All systems operational.", 11f, TextAlignmentOptions.TopLeft, GameDesignConstants.TextSecondary, new Vector2(380f, 70f), new Vector2(20f, 110f));
        warningBoxRect.anchorMin = new Vector2(0f, 0.5f);
        warningBoxRect.anchorMax = new Vector2(1f, 0.5f);
        warningBoxRect.pivot = new Vector2(0.5f, 0.5f);
        warningTxt = warningBoxRect.GetComponent<TextMeshProUGUI>();
        warningTxt.enableWordWrapping = true;

        // Upgrades Section Label
        CreateTMPText("UpgradesLabel", panelObj.transform, "INFRASTRUCTURE UPGRADES", 12f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(380f, 20f), new Vector2(20f, 50f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Grid Upgrade Button
        var gridUpgradeBtnRect = CreateStylizedButton("Btn_UpgradeGrid", panelObj.transform, "UPGRADE GRID ($8k)", StylizedButton.ButtonVariant.Primary, new Vector2(380f, 40f), new Vector2(20f, 0f));
        gridUpgradeBtnRect.anchorMin = new Vector2(0f, 0.5f);
        gridUpgradeBtnRect.anchorMax = new Vector2(1f, 0.5f);
        gridUpgradeBtnRect.pivot = new Vector2(0.5f, 0.5f);
        gridUpgradeBtnRect.anchoredPosition = new Vector2(20f, 0f);
        gridUpgradeBtn = gridUpgradeBtnRect.GetComponent<Button>();

        // Cooling Upgrade Button
        var coolingUpgradeBtnRect = CreateStylizedButton("Btn_UpgradeCooling", panelObj.transform, "UPGRADE COOLING ($6k)", StylizedButton.ButtonVariant.Primary, new Vector2(380f, 40f), new Vector2(20f, -50f));
        coolingUpgradeBtnRect.anchorMin = new Vector2(0f, 0.5f);
        coolingUpgradeBtnRect.anchorMax = new Vector2(1f, 0.5f);
        coolingUpgradeBtnRect.pivot = new Vector2(0.5f, 0.5f);
        coolingUpgradeBtnRect.anchoredPosition = new Vector2(20f, -50f);
        coolingUpgradeBtn = coolingUpgradeBtnRect.GetComponent<Button>();

        return cg;
    }

    private static CanvasGroup BuildBoardRoomPanel(
        Transform canvasParent,
        out Button closeBtn,
        out TextMeshProUGUI equityTxt,
        out TextMeshProUGUI roundInfoTxt,
        out Button acceptRoundBtn,
        out Button buyQuantumMindsBtn,
        out Button buyAnthroTechBtn,
        out TextMeshProUGUI boardTrustTxt,
        out Image boardTrustFillImg,
        out TextMeshProUGUI activeGoalTxt,
        out TextMeshProUGUI remainingTimeTxt)
    {
        var panelObj = new GameObject("BoardRoomPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(1f, 0f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(420f, -60f);
        rt.anchoredPosition = new Vector2(0f, -30f);

        CreatePremiumUiPanel("Background", panelObj.transform, rt.sizeDelta, Vector2.zero, GameDesignConstants.SurfaceCard, new Color(0.18f, 0.20f, 0.21f));

        // Header
        var header = CreateUiRect("Header", panelObj.transform, new Vector2(420f, 50f), Vector2.zero, Color.black);
        header.anchorMin = new Vector2(0f, 1f);
        header.anchorMax = new Vector2(1f, 1f);
        header.pivot = new Vector2(0.5f, 1f);
        header.anchoredPosition = new Vector2(0f, 0f);
        CreateTMPText("Title", header, "Board Room", 16f, TextAlignmentOptions.Left, Color.white, new Vector2(250f, 30f), new Vector2(20f, 0f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        var closeBtnRect = CreateStylizedButton("Btn_Close", header, "x", StylizedButton.ButtonVariant.Secondary, new Vector2(25f, 25f), new Vector2(-20f, -12f));
        closeBtn = closeBtnRect.GetComponent<Button>();

        // Content container
        var content = new GameObject("Content");
        content.transform.SetParent(panelObj.transform, false);
        var contentRt = content.AddComponent<RectTransform>();
        contentRt.anchorMin = Vector2.zero;
        contentRt.anchorMax = Vector2.one;
        contentRt.offsetMin = new Vector2(20f, 20f);
        contentRt.offsetMax = new Vector2(-20f, -60f);

        // 1. VC Financing Section
        CreateTMPText("VC_Header", content.transform, "VC FINANCING", 14f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(380f, 25f), new Vector2(0f, -20f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        equityTxt = CreateTMPText("EquityText", content.transform, "Founder Equity: 100%", 13f, TextAlignmentOptions.Left, Color.white, new Vector2(380f, 20f), new Vector2(0f, -50f)).GetComponent<TextMeshProUGUI>();
        roundInfoTxt = CreateTMPText("RoundInfoText", content.transform, "Offer: Series A - $150k for 15% equity", 12f, TextAlignmentOptions.Left, new Color(0.7f, 0.7f, 0.7f), new Vector2(380f, 40f), new Vector2(0f, -80f)).GetComponent<TextMeshProUGUI>();
        var acceptRoundBtnRect = CreateStylizedButton("Btn_AcceptRound", content.transform, "ACCEPT FUNDING ROUND", StylizedButton.ButtonVariant.Primary, new Vector2(380f, 35f), new Vector2(0f, -120f));
        acceptRoundBtn = acceptRoundBtnRect.GetComponent<Button>();

        // 2. Board Goals Section
        CreateTMPText("Board_Header", content.transform, "BOARD TRUST & GOALS", 14f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(380f, 25f), new Vector2(0f, -180f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        boardTrustTxt = CreateTMPText("TrustText", content.transform, "Board Trust: 100%", 13f, TextAlignmentOptions.Left, Color.white, new Vector2(380f, 20f), new Vector2(0f, -210f)).GetComponent<TextMeshProUGUI>();

        // Trust Bar Background
        var trustBarBg = CreateUiRect("TrustBarBg", content.transform, new Vector2(380f, 20f), new Vector2(0f, -235f), new Color(0.1f, 0.1f, 0.1f));
        // Trust Bar Fill
        var trustBarFill = CreateUiRect("TrustBarFill", trustBarBg.transform, new Vector2(380f, 20f), Vector2.zero, GameDesignConstants.BrandPrimary);
        trustBarFill.anchorMin = new Vector2(0f, 0.5f);
        trustBarFill.anchorMax = new Vector2(0f, 0.5f);
        trustBarFill.pivot = new Vector2(0f, 0.5f);
        boardTrustFillImg = trustBarFill.gameObject.GetComponent<Image>();
        boardTrustFillImg.color = GameDesignConstants.BrandPrimary;

        activeGoalTxt = CreateTMPText("ActiveGoalText", content.transform, "Active Goal: None", 13f, TextAlignmentOptions.Left, Color.white, new Vector2(380f, 40f), new Vector2(0f, -270f)).GetComponent<TextMeshProUGUI>();
        remainingTimeTxt = CreateTMPText("RemainingTimeText", content.transform, "Time Remaining: --", 12f, TextAlignmentOptions.Left, new Color(0.7f, 0.7f, 0.7f), new Vector2(380f, 20f), new Vector2(0f, -305f)).GetComponent<TextMeshProUGUI>();

        // 3. M&A Section
        CreateTMPText("MA_Header", content.transform, "MERGERS & ACQUISITIONS", 14f, TextAlignmentOptions.Left, GameDesignConstants.BrandAccent, new Vector2(380f, 25f), new Vector2(0f, -350f)).GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;
        var buyQuantumBtnRect = CreateStylizedButton("Btn_BuyQuantum", content.transform, "ACQUIRE QUANTUM MINDS ($250k)", StylizedButton.ButtonVariant.Secondary, new Vector2(380f, 35f), new Vector2(0f, -390f));
        buyQuantumMindsBtn = buyQuantumBtnRect.GetComponent<Button>();

        var buyAnthroBtnRect = CreateStylizedButton("Btn_BuyAnthro", content.transform, "ACQUIRE ANTHROTECH ($600k)", StylizedButton.ButtonVariant.Secondary, new Vector2(380f, 35f), new Vector2(0f, -440f));
        buyAnthroTechBtn = buyAnthroBtnRect.GetComponent<Button>();

        return cg;
    }

    private static CanvasGroup BuildGameOverPanel(
        Transform canvasParent,
        out Button loadSaveBtn,
        out Button mainMenuBtn,
        out TextMeshProUGUI reasonTxt)
    {
        var panelObj = new GameObject("GameOverPanel");
        panelObj.transform.SetParent(canvasParent, false);
        var cg = panelObj.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.blocksRaycasts = false;
        cg.interactable = false;

        var rt = panelObj.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        // Dark overlay background
        CreateUiRect("Overlay", panelObj.transform, new Vector2(1920f, 1080f), Vector2.zero, new Color(0f, 0f, 0f, 0.85f));

        // Premium Modal Card
        var card = CreatePremiumUiPanel("GameOverCard", panelObj.transform, new Vector2(500f, 340f), Vector2.zero, GameDesignConstants.SurfaceCard, new Color(1f, 1f, 1f, 0.08f));
        card.anchorMin = new Vector2(0.5f, 0.5f);
        card.anchorMax = new Vector2(0.5f, 0.5f);
        card.pivot = new Vector2(0.5f, 0.5f);

        // Header Title
        var title = CreateTMPText("Title", card, "GAME OVER - FIRED", 18f, TextAlignmentOptions.Center, GameDesignConstants.ResourceQuality, new Vector2(460f, 35f), new Vector2(0f, 120f));
        title.GetComponent<TextMeshProUGUI>().fontStyle = FontStyles.Bold;

        // Reason Text
        var reasonObj = CreateTMPText("ReasonText", card, "O conselho de administracao da sua empresa perdeu toda a confianca na sua gestao (Confianca = 0%) e destituiu voce do cargo de CEO.", 13f, TextAlignmentOptions.Center, Color.white, new Vector2(440f, 120f), new Vector2(0f, 20f));
        reasonTxt = reasonObj.GetComponent<TextMeshProUGUI>();

        // Buttons
        var loadSaveBtnRect = CreateStylizedButton("Btn_ReloadSave", card, "RELOAD LAST SAVE", StylizedButton.ButtonVariant.Primary, new Vector2(200f, 42f), new Vector2(-110f, -100f));
        loadSaveBtn = loadSaveBtnRect.GetComponent<Button>();

        var mainMenuBtnRect = CreateStylizedButton("Btn_MainMenu", card, "RETURN TO MAIN MENU", StylizedButton.ButtonVariant.Danger, new Vector2(200f, 42f), new Vector2(110f, -100f));
        mainMenuBtn = mainMenuBtnRect.GetComponent<Button>();

        return cg;
    }
}
