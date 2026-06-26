# Model Foundry

> **Status: this project was NOT finished.** It is an unfinished prototype and is kept here for study and reference only.

**Model Foundry** is a management/tycoon simulation game about founding and scaling
an AI company. You start in a garage in 2016 with a tiny team and grow all the way to
a frontier AGI lab, deciding strategy, hiring, research, data sources, compute
infrastructure, marketing and distribution along the way.

The full design document, game loop, metrics, room progression and development plan
live in **[IA-TYCOON-PLAN.md](./IA-TYCOON-PLAN.md)**. Read that file first to
understand the intended story and scope.

---

## Important licence notice (please read)

This repository contains third-party paid assets that **must not be shipped in any
built project** without a valid licence:

- `Assets/PolygonOffice/` — **Polygon Office Pack** by Synty Studios. This is a
  paid asset store pack that requires the purchase of a commercial licence.
- `Assets/Synty/` — additional Synty icon/art assets, also paid/licensed.

Do **not** use `PolygonOffice` / `Synty` art in a built, public, released, commercial
or distributed project unless you have purchased the corresponding licence from the
Unity Asset Store / Synty Studios. Keep them only for internal development and
prototyping. Before any public build, refactor the code to use original, free or
explicitly licensed replacements.

The same rule applies to the reference logos in `MyAssets/Rivals/`
(`openai.png`, `anthropic.png`, `grok.png`, etc.). These are private development
references only — they are **not** to be published in any public build, Steam page,
trailer or marketing material.

---

## AI models used to generate the code

The C# source code in this repository was generated with the help of the following
AI assistants:

- **Unity AI** (Unity AI Assistant)
- **Claude Opus 4.6** (Anthropic)
- **Gemini Pro 3.5 Flash** (Google)

---

## Project at a glance

- **Engine:** Unity `6000.5.0f1` (URP)
- **Language:** C#
- **UI:** uGUI (TextMeshPro) — UI Toolkit migration was planned
- **Tests:** Unity Test Framework (EditMode NUnit), under `Assets/Tests/EditMode/`
- **Save system:** local JSON (`save.json` in `Application.persistentDataPath`)
- **Target platform:** Windows / Steam (not yet integrated)

Folder layout:

```text
Assets/
  Scripts/        Core, Simulation, UI systems
  Prefabs/         Rooms, employees, props, UI
  ScriptableObjects/
  Scenes/          MainMenu, GaragePrototype
  Art/             Placeholder image slots
  Materials/       URP materials
  Animations/
  PolygonOffice/   PAID Synty asset — do not ship
  Synty/           PAID Synty asset — do not ship
docs/              Design and implementation history (English)
MyAssets/Rivals/   Private reference logos — do not ship
```

---

## Documentation index

| Document | Description |
|---|---|
| [IA-TYCOON-PLAN.md](./IA-TYCOON-PLAN.md) | Main design document: concept, metrics, rooms, art direction, tech stack, MVP and development phases. |
| [docs/implementation-log.md](./docs/implementation-log.md) | Phase-by-phase implementation log of what was actually built. |
| [docs/art-direction-notes.md](./docs/art-direction-notes.md) | Short art-direction rules (isometric, PolygonOffice usage, image slots). |
| [docs/techpulse_diagnosis_and_plan.md](./docs/techpulse_diagnosis_and_plan.md) | Diagnosis of the TechPulse feed bug and modular plan for a living social network. |
| [docs/rivals_and_progression_system.md](./docs/rivals_and_progression_system.md) | Rival companies, logo mapping, wealth hierarchy and 2017–2026 timeline. |
| [docs/model_foundry_rivals_and_progression_v2.md](./docs/model_foundry_rivals_and_progression_v2.md) | v2 design: markets, eras up to 2032, rival archetypes, decision engine, TechPulse rules, data model. |

---

## What is implemented

The prototype reached **Phase 8** of the planned 10 phases:

- Garage and office diorama (PolygonOffice prefabs) with isometric camera.
- Founder + specialised hires (ML Engineer, Research Scientist, Data Engineer,
  Safety Researcher, Infrastructure Engineer, GPU Technician, MLOps Engineer,
  Backend Engineer, Finance Lead, Recruiter, Product Manager, Sales Executive,
  Community Manager).
- Project training loop with model selection (Vision / NLP / Agentic) and dataset
  choice (web scraping vs licensed).
- Office expansion tiers 1–4 (Corporate Suite, Secret R&D Lab, Modular Datacenter,
  Board Room).
- GPU upgrades, energy/thermal management and NOC panel.
- Procedural commercial contracts (max 3–4 active) with quality goals, deadlines,
  upfront payouts and penalties.
- Global AI timeline events and dynamic market modifiers (GPU shortage, EU data
  regulation, generative AI hype wave).
- TechPulse social feed (X/Twitter-style) with competitor posts, organic mentions,
  inactivity pressure and player launch reactions.
- Venture capital rounds (Series A/B/C) and IPO, Board of Directors with quarterly
  goals and Board Trust mechanic, M&A of small rivals.
- Dynamic line charts (cash over time, followers, model quality) and analytics panel.
- Local JSON save/load.
- EditMode NUnit tests (~20 tests passing as of Phase 8).

## What is NOT finished

- **Phase 9 (AGI era, Safety/Alignment crises, War Room, advanced safety hires)** — planned, not built.
- **Phase 10 (Steam demo, tutorial, accessibility, Steamworks integration, localization, final balancing)** — planned, not built.
- Final original art (PolygonOffice/Synty must be replaced before any public build).
- Final original rival logos (reference logos must be replaced).
- Steam integration (Achievements, Steam Cloud, stats).
- Localization pass.
- Marketing material (trailer, screenshots, Steam page).

---

## Running the project

1. Open Unity Hub and add this folder with Unity `6000.5.0f1`.
2. Open the project and let it import (this includes the paid PolygonOffice/Synty assets already present in the repo).
3. Open `Assets/Scenes/MainMenu.unity` or `Assets/Scenes/GaragePrototype.unity` and press Play.

For batch test runs from the repo root:

```bash
Unity.exe -batchmode -projectPath . -runTests -testPlatform EditMode -quit -logFile Logs/EditModeTests.log
```
(Use the full path to the Unity `6000.5.0f1` executable if Unity is not on `PATH`.)

---

## Licence

This repository is shared for educational/reference purposes only. No commercial
release is permitted without:

1. Replacing all paid `PolygonOffice` / `Synty` assets with licensed or original art.
2. Replacing all reference rival logos in `MyAssets/Rivals/` with original art.
3. Acquiring the appropriate commercial licences for any remaining third-party asset.

The code itself, where originally authored for this project, may be reused and
adapted — but the assets carry their own licensing requirements.