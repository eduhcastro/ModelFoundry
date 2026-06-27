# Model Foundry - Realistic Refactor Implementation Roadmap

Status: active implementation roadmap.

This document tracks the refactor that moves Model Foundry from the old advanced prototype systems into the new realistic AI-company experience defined in `docs/realistic_ai_company_progression_plan.md`.

Use this document for implementation sequencing. Use `docs/realistic_ai_company_progression_plan.md` for design intent.

## Refactor Principle

Do not delete Unity scene objects or old scripts blindly.

The old project has many serialized scene references and an editor bootstrap that can regenerate layouts. Removing scripts or moving scene objects directly can break scenes or cause the bootstrap to restore old layouts. The safe approach is:

1. Introduce new systems in parallel.
2. Hide/disable old advanced UI from the player-facing flow.
3. Keep old scripts compiling until their data is migrated.
4. Replace one domain at a time.
5. Only delete old scripts after no scene, prefab, test or bootstrap reference remains.

## Implemented In This Pass

### New Localization Layer

Implemented `Assets/Scripts/Core/LocalizationManager.cs`.

Current state:

- English-only string table.
- Simple `LocalizationManager.T(key)` lookup.
- Designed so more languages can be added later without rewriting UI scripts.

Next:

- Move all player-facing text into localization keys.
- Add language selection in settings.
- Add `pt-BR` only after English flow is stable.

### New Realistic Startup Simulation

Implemented `Assets/Scripts/Simulation/StartupSimulationManager.cs`.

Current state:

- New startup stages:
  - Solo Founder
  - First Product
  - Product-Market Fit
  - Model Company
  - Platform Company
  - Frontier Lab
- New core metrics:
  - Research skill
  - Model capability
  - Website quality
  - Product UX
  - Infrastructure reliability
  - Trust
  - Maintenance load
  - Product surfaces
  - AI Arena rank
- New actions:
  - Study papers
  - Improve website
  - Build prototype
  - Secure cloud hosting
  - Hire designer
  - Hire developer
  - Hire scientist
  - Launch CLI agent
  - Build Agent Studio
  - Submit to AI Arena
- Existing `GameManager` is still used for cash, burn, revenue, team size and compatibility.

Next:

- Persist these metrics in save/load.
- Add formal product records instead of only aggregate metrics.
- Add model lineage and benchmark result records.

### New Startup Dashboard UI

Implemented `Assets/Scripts/UI/StartupDashboardController.cs`.

Current state:

- Runtime-generated English UI.
- Shows stage, cash, burn, revenue, research, model, website, product, infra, trust, team and AI Arena rank.
- Provides buttons for the new early-game loop.
- Attaches automatically from `HUDController`.

Next:

- Replace the temporary generated layout with a proper designed panel.
- Add tooltips, costs, disabled states and forecast impact.
- Add tabs for Company, Product, Research, Infrastructure and Arena.

### Advanced Legacy UI Disabled

Updated `Assets/Scripts/UI/HUDController.cs`.

Current state:

- Keeps TechPulse.
- Hides old advanced dock buttons:
  - Research
  - Analytics
  - Hiring
  - GPU Upgrade
  - Board Room
  - Contracts
  - NOC
- Hides old project/summary/context panels.
- New dashboard becomes the player-facing control surface.

Important:

- Old systems still compile and remain in the project.
- This is intentional for safe migration.
- They should be deleted only after bootstrap, scene and save references are migrated.

### TechPulse Refactor

Updated:

- `Assets/Scripts/Simulation/TechPulseContentGenerator.cs`
- `Assets/Scripts/Core/CompetitorManager.cs`
- `Assets/Scripts/UI/TechPulseUI.cs`

Current state:

- TechPulse text is now English.
- Content focuses on realistic AI-company topics:
  - CLI agents
  - product reliability
  - docs
  - inference cost
  - GPU queues
  - benchmark contamination
  - cloud hosting
  - enterprise trust
- Competitor posts are now English.
- Player profile bio uses localization.

Next:

- Localize all TechPulse strings through `LocalizationManager`.
- Add post types for website launches, CLI adoption, benchmark submissions, outages and funding.
- Add lower post frequency controls in settings.

### Company Identity Preserved

Previous pass added:

- Company font choice
- Company color choice
- Company icon choice
- Top-bar icon
- TechPulse player avatar icon
- Save/load identity fields

This remains part of the new foundation.

## Not Yet Fully Removed

The following systems are hidden from the new player flow but still exist:

- `PrototypeProjectController`
- `HiringController`
- `ResearchController`
- `GPUUpgradeController`
- `OfficeUpgradeController`
- `ContractController`
- `NOCController`
- `BoardRoomController`
- `AnalyticsController`
- `GameEventController`
- Old binary employee flags inside `GameManager`

They should be treated as legacy compatibility until migrated.

## Roadmap

### Phase 1 - Stabilize New Loop

- Persist `StartupSimulationManager` state.
- Add costs and requirements to button labels.
- Add disabled-state explanations.
- Add starter tutorial beats:
  - Study papers
  - Improve website
  - Build prototype
  - Hire first developer/designer
  - Launch CLI agent
  - Submit to AI Arena
- Verify scene startup in Unity Editor.

### Phase 2 - Product Records

- Create `ProductSurface` data model.
- Product types:
  - Website
  - Web app
  - API
  - SDK
  - CLI
  - IDE extension
  - Agent Studio
  - Enterprise dashboard
- Track UX, reliability, docs, maintenance, conversion and revenue.
- Replace aggregate `ProductSurfaces` with actual records.

### Phase 3 - Staff Profiles

- Create `EmployeeProfile`.
- Replace old boolean hires with employee instances.
- Roles:
  - Developer
  - Designer
  - Research Scientist
  - Infrastructure Engineer
  - Technical Writer
  - Developer Advocate
  - Support Engineer
  - Security Engineer
  - Product Manager
- Add salary, skills, burnout and retention.

### Phase 4 - Model and Benchmark Records

- Create `ModelLineageRecord`.
- Create `BenchmarkResult`.
- Add AI Arena boards:
  - General
  - Coding
  - Agents
  - Vision
  - Enterprise
  - Safety
  - MLPerf Training
  - MLPerf Inference
- Replace single `ModelQuality` as the primary progression metric.

### Phase 5 - Infrastructure and Hosting

- Create hosting contracts:
  - Cloud credits
  - Spot GPU
  - Reserved GPU
  - Enterprise cloud
  - Colocation rack
  - Owned data-center module
- Track training capacity, inference capacity, latency, uptime and cost per request.
- Replace simple GPU-count upgrade from the old prototype.

### Phase 6 - TechPulse 2.0

- Convert TechPulse to localized post templates.
- Add event-driven posts:
  - Website launch
  - Docs improvement
  - CLI release
  - Agent Studio preview
  - AI Arena result
  - Hosting outage
  - Inference bill spike
  - Security incident
  - Hiring announcement
- Reduce random noise further.

### Phase 7 - Legacy Deletion

Only after Phases 1-6:

- Remove hidden legacy UI panels from bootstrap.
- Remove old controllers from scene generation.
- Migrate save data.
- Remove old tests or rewrite them around new systems.
- Delete old scripts once no references remain.

## Scene and Furniture Strategy

There are two safe ways to edit existing furniture and scene layout.

### Recommended: Scene Override Script

Use a small scene override script that runs after bootstrap/scene load and moves, hides or replaces objects by stable names.

Advantages:

- Survives scene rebuilds.
- Easy to review.
- Can be versioned as code.
- Avoids manual scene drift.
- Can be disabled if it causes a layout problem.

Best for:

- Moving desks
- Hiding old props
- Activating new workstations
- Repositioning cameras
- Adding placeholder product stations
- Disabling old advanced-room visuals

### Manual Scene Editing

Manual scene editing is possible, but risky here.

Risk:

- `GaragePrototypeBootstrap.cs` can regenerate scenes.
- If a force rebuild runs, manual changes can be overwritten.
- Existing scripts may recreate old objects or old serialized references.

Manual editing is acceptable only when:

- The bootstrap is also updated.
- The changed scene is intentionally committed.
- The team agrees not to force rebuild without preserving the layout.

### Practical Rule

For this project, prefer code-driven scene overrides until the bootstrap is retired or rewritten. Once the new systems are stable, rebuild the scene intentionally around the new startup office and remove the old bootstrap behavior.

## Validation

Current validation for this pass:

- `dotnet build Assembly-CSharp.csproj --no-restore` passes.
- Unity EditMode tests were not run in this pass because Unity is not available through `Unity.exe` on PATH in the current shell.

Known warning class:

- Many existing fields are reported as unassigned by plain C# build because Unity assigns them through scenes/serialization.
- These warnings existed before and are not direct compile blockers.
