# Repository Guidelines

## Project Structure & Module Organization

This is a Unity project using Unity `6000.5.0f1`. Runtime and editor assets live under `Assets/`. Current key areas include `Assets/Scenes/` for scenes, `Assets/Settings/` for Universal Render Pipeline assets, and `Assets/TutorialInfo/` for template tutorial content. Package dependencies are declared in `Packages/manifest.json`; project-wide settings live in `ProjectSettings/`.

Keep generated Unity folders such as `Library/`, `Temp/`, `Logs/`, and `UserSettings/` out of source changes unless a repository policy explicitly says otherwise. Preserve `.meta` files whenever adding, moving, or deleting assets.

## Build, Test, and Development Commands

- Open locally: launch Unity Hub and open this repository folder.
- Batch test example:
  `Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -quit -logFile Logs/EditModeTests.log`
- Player build: use Unity Editor build settings unless a project build script is added later.

Run commands from the repository root. If Unity is not on `PATH`, use the full editor executable path for version `6000.5.0f1`.

## Coding Style & Naming Conventions

Use standard Unity C# conventions: four-space indentation, `PascalCase` for classes, methods, properties, and public fields, and `camelCase` for locals and private serialized fields. Prefer `[SerializeField] private` fields over public mutable fields for inspector wiring.

Name MonoBehaviour scripts after their class exactly, for example `PlayerController.cs` containing `PlayerController`. Keep scene, prefab, material, and asset names descriptive and stable.

## Testing Guidelines

The Unity Test Framework is installed as `com.unity.test-framework`. Place Edit Mode tests under `Assets/Tests/EditMode/` and Play Mode tests under `Assets/Tests/PlayMode/`. Name test classes after the behavior under test, such as `InventoryTests`, and use direct behavioral test names like `AddsItemWhenSlotIsEmpty`.

Run relevant Edit Mode or Play Mode tests before submitting changes. For scene or rendering changes, also verify behavior manually in the Unity Editor.

## Commit & Pull Request Guidelines

No local Git history is available in this checkout, so use concise imperative commit messages such as `Add player movement test` or `Update URP quality settings`. Keep commits focused on one logical change.

Pull requests should describe the change, list validation performed, and call out Unity version requirements. Include screenshots or short clips for visible scene, UI, rendering, or animation changes. Link related issues when applicable and mention any asset migrations or package changes.

## Agent-Specific Instructions

Inspect repository structure before editing, prefer small reviewable diffs, and do not overwrite existing Unity assets or settings without confirming intent. Always preserve UTF-8 text encoding and Unity `.meta` files.
