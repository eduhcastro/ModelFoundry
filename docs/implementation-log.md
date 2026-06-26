# Implementation Log

## Phase 0 - Pre-production

- **Design Conception**: Wrote the initial Game Design Document (*IA-TYCOON-PLAN.md*), defining the simulation metrics (Cash, Runway, Compute, Data, Reputation, Hype, etc.) and the main game loop.
- **Game Setup**: Created the Unity `6000.5.0f1` project using the Universal Render Pipeline (URP) for optimized 3D diorama graphics.
- **Versioning and Infrastructure**: Initialized the Git repository with Git LFS to cleanly manage heavy textures, 3D models (`.fbx`) and audio.
- **Folder Structure**: Organized the standard directory tree (`Assets/Scripts`, `Assets/Art`, `Assets/Prefabs`, `Assets/ScriptableObjects`, `Assets/Scenes`).

## Day 1 - Garage Prototype

- Created scene `Assets/Scenes/GaragePrototype.unity`.
- Added an orthographic isometric camera.
- Added floor, cutaway walls, desk, laptop, chair, GPU rack and whiteboard.
- Created base structure: `Assets/Art`, `Assets/Prefabs`, `Assets/Scripts`, `Assets/ScriptableObjects`.
- Imported `POLYGON Office Pack` into `Assets/PolygonOffice`.
- Scene uses pack prefabs: desk, chair, computer setup, server rack.

## Day 2 - Founder Movement

- Created `PrototypeEmployeeAgent`: walks through waypoints and animates typing.
- Created `PrototypeWorkstation`: clicking the station sends the character to work.
- Scene now has a low-poly founder, animated hands, status light and walking points.
- Visual loop: walk -> workstation -> typing -> walk.

## Review Day 1/2

- Fixed the pink material on POLYGON prefabs.
- Desk, chair, computer and server instances now use local URP materials.

## Day 3 - Project Panel

- Created `PrototypeProjectController`.
- UI shows `SupportBot v0.1`, cash, Start button and progress bar.
- Start launches the project, consumes money and fills progress.
- Founder switches to typing when the project starts.

## POLYGON visual review

- Removed code-generated visual models.
- Map recreated with `PolygonOffice` prefabs: floor, walls, door, desk, chair, computer, servers, whiteboard, sofa, table, coffee, plants.
- Founder now uses the `SM_Chr_Developer_Male_01` character.
- UI improved with `IMG_SLOT_*` slots.
- Placeholders created in `Assets/Art/ImageSlots`.

## Design Overhaul - Full Review (Day 3.5)

### Core Systems created

- **`GameDesignConstants.cs`** — Central design system with a premium dark palette (purple/teal/gold), per-department colors, animation constants, typography and layout.
- **`GameManager.cs`** — Singleton with full game state: cash, reputation, quality, team, revenue, burn, runway. Event system for reactive UI. Customizable company name.
- **`TimeController.cs`** — Game time system (Jan 2017+). 4 speeds: Paused, Normal (4s/day), Fast (2s), Ultra (1s). Day/week/month events.
- **`IsometricCameraController.cs`** — Camera with WASD pan, scroll zoom, Q/E 90° rotation, edge pan, smooth lerp, bounds.

### UI Components created

- **`UIAnimations.cs`** — Animation utilities: fade, scale, slide, pulse, color lerp, value counter. Easing curves (out cubic, out back).
- **`StylizedButton.cs`** — Premium button with hover (scale 1.04x + brighten), press (scale 0.96x + darken), disabled (opacity). Variants: Primary, Secondary, Danger, Accent.
- **`ResourceBar.cs`** — Animated resource bar with icon + label + value + fill. Auto K/M formatting. Optional health gradient.
- **`ToastNotification.cs`** — Stackable notifications with slide-in/fade-out. Categories: Info, Success, Warning, Danger. Auto-subscribes to GameManager.
- **`MainMenuController.cs`** — Main menu with animated entry (fade + slide), company-name input, fade-to-black scene transition.
- **`HUDController.cs`** — Full HUD: top bar (company, date, speed), resource bars (cash/rep/quality/team), financial info (burn, revenue, runway, clients).
- **`ProjectResultPanel.cs`** — Project result modal with star rating, metrics (quality/cost/clients/revenue/reputation), Accept/Refine/Abandon buttons.

### Refactored scripts

- **`PrototypeProjectController.cs`** — Rewritten: integrates GameManager for cash, TimeController for pause, ProjectResultPanel for results. Quality/clients/revenue calculations with variance and team bonus. Decisions: Accept (launch), Refine (improve quality), Abandon (lose investment).
- **`PrototypeEmployeeAgent.cs`** — 5 states (Idle/Walking/Working/Celebrating/Panicking) with unique animations. Integrates Animator. Status light with smooth color lerp. Responds to game pause.
- **`PrototypeWorkstation.cs`** — Smooth screen color transition, flicker when active, emission glow, error flash, support for multiple screens.

## Day 4 - UI Integration, X Redesign & Scene Building

- **Automated Scene Creation**: Structured the editor script `GaragePrototypeBootstrap.cs` to cleanly and automatically build the `MainMenu` and `GaragePrototype` (Gameplay) scenes.
- **Canvas & HUD Assembly**: Created full Canvas with top bar (company name, date, speed controls), resource panel (with real 3D-baked icons from the Synty icon pack), bottom financial panel and Toast notification support.
- **TechPulse UI Redesign (X-style)**: Redesigned the TechPulse interface to look like the X (Twitter) app: pure black background, subtle post dividers (#2F3336), blue Post button (#1D9BF0), clean white/light-gray typography, and a feed of tweets from AI rivals.
- **Reference Coupling and Fallback**: Connected all `SerializeField` fields (speed buttons, resource bars, etc.) via serialization in the bootstrap script. Added a dynamic runtime resolver in `HUDController.cs` to ensure robust coupling of TechPulseUI.
- **Metrics Stabilization**: Fixed a bug in `PrototypeProjectController.cs` where metrics were recalculated on accepting the project. The result (quality, cost, clients, reputation and revenue) is now frozen on completion and faithfully applied to the GameManager.

## Day 5 - Recruitment & Employee Mechanics

- **ML Engineer Hiring**: Added a hire button on the `SummaryPanel` to recruit an ML Engineer for $5,000 (raising burn by $1,200/month and team size by +1).
- **Workstation & Pathfinding Mechanics**:
  - Configured a second desk (`Helper Desk` at `X = 1.1f`) in the office with monitor, chair and computer.
  - On hiring, the female developer character (`ML_Engineer_Preview`) is activated, dynamically initialized via script with its walking points (coffee, whiteboard, approach, server) and physically walks to work at the empty desk.
- **Training Acceleration**: Hiring the ML Engineer grants a permanent 40% boost to model training speed (reduces duration by 40%).

## Day 6 - Save/Load, Event System & TechPulse Integration

- **Local JSON Save/Load System**: Created `SaveLoadManager.cs` that saves full game state (resources, burn rate, date, hires and GPU upgrade level) into a local `save.json` in `Application.persistentDataPath` and rebuilds the state on load.
- **AI Timeline & Random Events**: Implemented `GameEventController.cs` with historical events at real AI milestones (Transformer in Jun 2017, GPT-2 in Feb 2019, GPT-3 in May 2020, ChatGPT in Nov 2022, Agentic Shift in Dec 2025, AI Summit in Jun 2026) that pause the game and force choices with reputation, cash and quality impacts.
- **Auto-Posting on TechPulse**: Successful model launches automatically generate posts in the TechPulse social feed announcing the model's technical quality.

## Phase 2 - Compute, Datasets & Model Selection (Day 7)

- **GPU Upgrades & Visual Cabinet**: Added a GPU upgrade button ($10k upfront, +$300/mo burn) that activates the second server rack (`GPU Rack B`) in the office and grants 20% AI training acceleration.
- **Model Selection & Data Sources**:
  - Added model options: Vision (fast/cheap), NLP (medium) and Agentic (slow/expensive).
  - Dataset choice: Web Scraping (free, quality penalty, doubles audit/safety risk) or Licensing ($1.5k upfront, quality bonus).
  - Project panel expanded from 300f to 440f height with dynamic selection buttons and real-time estimated cost calculation.
- **NUnit Tests**: Implemented unit tests in `HiringTests.cs` covering the success/failure flow of GPU upgrades.
- **Automated Compilation**: Rebuilt `MainMenu` and `GaragePrototype` scenes via batchmode command line without errors.

## Phase 3 - Marketing, Competitors and Office Expansion (Day 8)

- **TechPulse Interface Fix**: Resolved the post-feed bug that prevented posts from being displayed. Added a `LayoutElement` component to the `postPrefab` to prevent height collapse (125f) under `VerticalLayoutGroup` and `ContentSizeFitter`.
- **Initial Followers Mechanic**: The player starts the game with exactly **1 follower** and **1 following** (saved/loaded in JSON).
- **Corporate News and Competitor Incidents**: The feed now generates autonomous posts about rival layoffs, multi-million funding rounds and server/code incidents.
- **Organic Opinions and Player Mentions**: Common users post tagging the player's company (`@company`) reviewing and joking about the model based on its quality level and market reputation.
- **Inactivity Pressure**: If the player goes more than 15 days without launches, the system generates feed complaints mentioning the company, with penalties to reputation and followers.
- **Real Logo Integration**: Implemented dynamic loading and display in the post avatar of real logos from `MyAssets/Rivals/`:
  - `openai.png` → NeuraCorp
  - `anthropic.png` → AnthroTech
  - `grok.png` → Quantum Minds
  - Other rivals use the stylized first letter of the name in their brand color as fallback.

## Phase 4 - Office Expansion and Secret Lab (Day 9)

- **Physical Office Expansion (Tiers 2 and 3)**:
  - Implemented the visual controller `OfficeVisualController.cs` and the upgrade UI `OfficeUpgradeController.cs` to manage the physical expansion of the 2x2 diorama.
  - Tier 2 (Corporate Suite, $30k) expands the diorama to the left and deactivates the corresponding internal walls (X: -5f).
  - Tier 3 (Secret R&D Lab, $75k) expands the diorama backward and deactivates the rear walls (Z: 4f).
  - Additional workstations and preview characters activate as the office expands.
- **New Roles and Specialized Employees**:
  - Enabled hiring for advanced roles:
    - **Research Scientist**: Grants 50% reduction in research time for new technologies.
    - **Data Engineer**: Grants +5 base quality for all new models started.
    - **Safety Researcher**: Reduces the probability of security failures by 40% and mitigates the impact of negative events.
  - Characters created with appropriate male/female animations set up via bootstrap.
- **Glassmorphic Panels and Modernized HUD**:
  - Replaced immediate-buy on the HUD dock buttons with overlaid glassmorphic panels (Overlay Panels):
    - **HiringPanel** (hiring specialized employees).
    - **UpgradesPanel** (GPU and Office expansion).
    - **ResearchPanel** (R&D technology research).
    - **AnalyticsPanel** (metric and chart visualization).
    - **SystemPanel** (Save, Load and Quit).
- **Advanced Research (R&D)**:
  - Implemented `ResearchController.cs` with two main tech researches:
    - **Safety Alignment**: Reduces the chances of audit and data leakage from web-scraped datasets.
    - **Custom Silicon**: Offers a permanent 30% extra speed on model training.
- **Advanced Save/Load (JSON)**:
  - Updated `SaveLoadManager.cs` to serialize office tier level, hired employees, researched technologies and GPU states, keeping full backward compatibility.
- **Expansion Unit Tests**:
  - Added tests to `HiringTests.cs` covering burn-rate recalculation per hired role, R&D (Research) bonuses and office unlocks.
- **Error-Free Build**:
  - All dependencies and serialization wiring were automated in the `GaragePrototypeBootstrap.cs` script. The `MainMenu` and `GaragePrototype` scenes compiled successfully via command line with return code 0.

## Phase 5 - Company Charts and Launched Model Reports (Day 10)

- **Dynamic Premium Line Charts**:
  - Implemented dynamic line chart rendering in `AnalyticsController.cs`. Points are plotted proportionally to the max value (height cap 65f) and connected by stretched/rotated `Image` segments in the local space of the Canvas container.
  - Updated the three charts in the `GaragePrototypeBootstrap.cs` bootstrap:
    - **Cash Over Time** (monthly accumulated revenue/cash).
    - **Followers Over Time** (player's TechPulse follower growth).
    - **Model Quality History** (technical quality of the last 8 launched AI models).
- **Analytics Report and Metrics Panel**:
  - Stats panel updated to display total models launched, the company's average quality and the best model with its percentage.
  - Structured "Recent Model Launches" log shows the list of recent models with quality and corresponding monthly revenue contribution.
- **History Persistence (Save/Load)**:
  - `SaveLoadManager.cs` was extended to serialize all history lists and the recent launch log in JSON, keeping compatibility with old saves.
- **Unit Tests**:
  - Created the unit test `AnalyticsHistory_RecordsAndRestoresCorrectly` in `HiringTests.cs` to validate saving and restoring histories.
  - Adjusted the test `GameManager_LoadGameState_RestoresStateCorrectly` to reflect the dynamic burn-rate calculation from Phase 4. All 11 unit tests pass successfully.
- **Error-Free Compilation**:
  - Scenes rebuilt successfully through the bootstrap editor and batchmode compilation finished with exit code 0.

## Phase 6 - Advanced Contracts and Global AI Events (Day 11)

- **Procedural Commercial Contracts System**:
  - Implemented `ContractController.cs` to procedurally generate and manage commercial contract offers.
  - Contracts require specific model types (Vision, NLP, Agentic) unlocked by technology, quality goals, deadlines in days, and impose upfront payments, completion payouts and failure/late penalties (financial penalty and reputation loss).
  - Strict limit of at most 3 simultaneously active contracts.
- **Premium Contracts Panel (HUD Dock "C")**:
  - Added a new vertical dock button "C" on the HUD dock to open the contracts panel.
  - The panel dynamically and elegantly (with glassmorphism) shows active contracts with countdowns and available contracts with accept buttons.
- **Global AI Events and Dynamic Modifiers**:
  - Implemented regular occurrence of global market events every 3-6 months in `GameEventController.cs`:
    - **GPU Shortage**: Increases GPU upgrade cost by 50% for 3 months, with choices to absorb costs or lobby (reputation loss).
    - **EU Data Regulation**: A 3-month audit period that penalizes launches using web-scraped data, unless a Safety Researcher is hired.
    - **Generative AI Hype Wave**: Grants 2x reputation and 1.5x follower bonus for launches for 3 months, or a $2,000 research grant.
  - Event resolutions and contract actions (completion/breach) automatically post updates and press notes to the **TechPulse** feed.
- **Robust Unit Tests**:
  - Created the file `ContractTests.cs` (EditMode NUnit) containing 6 new tests that validate the full flow of technology-based generation, upfront-payment acceptance, slot limit, quality-goal delivery, late penalties and GPU-shortage modifiers.
  - Implemented static reset and singleton cleanup in test `SetUp`/`TearDown` and modified `GameManager.cs` initialization in EditMode (`Application.isPlaying` wrapper around `DontDestroyOnLoad`) to allow reliable, leak-free execution. All 17 project unit tests pass without failures.
- **Error-Free Compilation**:
  - Visual references and buttons were wired in the bootstrap script. The batchmode build of all scenes completed successfully.

## Phase 7 - Advanced Infrastructure and Modular Datacenters (Day 12)

- **Datacenter Mechanics and Physical Expansion (Tier 4)**:
  - Implemented the office upgrade to **Tier 4 (Modular Datacenter)** for $120,000 cash and a $5,000 monthly burn rate.
  - The expansion physically grows the diorama to the right side (coordinates X: 3f to 4f, Z: -2f to 3f), enabling new floors, LED-decorated walls, extra server racks and deactivating the (Tier 1) right corner walls to integrate the environment.
- **Energy and Thermal Management (Energy & Cooling)**:
  - Implemented dynamic power consumption and heat dissipation calculations based on the physical count of active GPUs.
  - Each GPU consumes and emits 10kW of heat. If the grid or cooling load capacity is exceeded, the system enters **Critical Overheat (IsOverheating)**, reducing training speed (doubling the duration of active AI projects).
  - Electric grid upgrades (+30kW, cost $8k, maintenance +$100/mo) and cooling system upgrades (+30kW, cost $6k, maintenance +$80/mo) in the NOC panel.
- **NOC Control Panel (Network Operations Center)**:
  - Replaced the redundant "L" (Load) side button with the new **"N" (NOC)** button on the vertical side HUD.
  - The panel reactively and glassmically displays the number of active GPUs, dynamic energy and cooling status bars, overload warnings and actions to purchase electric/thermal upgrades.
- **New Roles and Specialized Employees**:
  - **Infrastructure Engineer** (cost: $10k, salary: $2.8k/mo) — Reduces total GPU energy consumption by 25%.
  - **GPU Technician** (cost: $7k, salary: $1.9k/mo) — Reduces heat dissipation by 10% and accelerates training by 10%.
  - **MLOps Engineer** (cost: $9k, salary: $2.2k/mo) — Increases net model billing by +20% thanks to inference/serving cost optimization.
  - **Backend Engineer** (cost: $6k, salary: $1.5k/mo) — Permanently expands the simultaneous commercial contract capacity from 3 to 4 slots.
  - Integrated animation and physical movement of the corresponding characters (they walk to their respective new work desks and type when hired).
- **Data Persistence (Save/Load)**:
  - Updated `SaveLoadManager.cs` to serialize and deserialize all electrical states, upgrade counters and the hiring states of the 4 new specialist engineers.
- **Integration Unit Tests**:
  - Added tests in `HiringTests.cs` covering the financial impact of salaries and maintenance, the overheat and grid/cooling capacity state, and the contract slot expansion to 4 in `ContractController`. All 20 unit tests pass successfully.
- **Error-Free Build**:
  - All diorama and NOC panel visual dependencies and wiring were connected and generated successfully through the batchmode build of all scenes.

## Phase 8 - Investments, Board of Directors and Board Room (Day 13)

- **Venture Capital Funding Rounds (Series A, B, C) and IPO**:
  - Implemented the VC funding rounds system in `InvestmentController.cs`. Allows raising massive investments ($150k, $400k, $1.2M) in exchange for equity dilution, unlocked by reputation and follower milestones.
  - Implemented the IPO (Initial Public Offering) process after Series C, allowing the company to go public by selling remaining equity and raising huge public funds based on the company valuation.
- **Board of Directors and Investor Pressure**:
  - The board now requires procedural quarterly goals (minimum revenues, TechPulse follower growth and strict burn-rate control).
  - Implemented the **Board Trust** metric (0 to 100). Failing quarterly goals or making decisions against investors reduces trust. If it falls to zero, the player faces severe penalties (Breach of Trust fine) or CEO dismissal (immediate Game Over).
- **Mergers & Acquisitions (M&A)**:
  - Ability to acquire smaller startup competitors (such as **Quantum Minds** and **AnthroTech**) directly from the investment panel for a substantial cash cost.
  - The acquisition absorbs research patents and competitive intelligence, granting permanent +10% and +15% bonuses to AI model training speed.
- **5 New Specialized Employees**:
  - **Finance Lead**: Improves tax efficiency, reducing overall operating costs by 15% and facilitating fundraising with 20% higher valuations.
  - **Recruiter**: Reduces new-employee hiring costs and times by 30% and increases the base skill of new hires.
  - **Product Manager**: Reduces project development/training time by 10% and adds +3 base quality to models.
  - **Sales Executive**: Increases enterprise contract payouts by 20% and adds +1 active contract slot.
  - **Community Manager**: Increases TechPulse follower conversion by 25% and reduces reputation loss from inactivity.
  - Animations and physical movement integrated in the diorama: each character walks to their respective working chair and types.
- **Diorama and Board Room Visual Expansion**:
  - Physical implementation of the diorama for the board room. Activation of a dark-wood rectangular meeting table, black executive chairs, a large flat-screen TV for slide presentations on the wall and support workstations.
- **Board Room Panel and Interface (HUD Dock "B")**:
  - Added the **"B" (Board Room)** shortcut button on the vertical side HUD to show an elegant, glassmorphic panel.
  - The panel displays company equity status, board trust level, current quarter goals with day countdowns, VC/IPO funding round options and the list of startups available for acquisition.
- **Expanded Data Persistence (Save/Load)**:
  - Updated `SaveLoadManager.cs` to serialize and deserialize the investment round status (completed Series A/B/C/IPO), retained equity percentage, board trust level, quarterly goal progress, acquired startups and the new team hires.
- **Unit Tests**:
  - Added robust integration tests in `HiringTests.cs` covering the fundraising flow, validation and application of quarterly revenue/follower/burn goals, equity dilution and acquired-startup bonuses.
- **Error-Free Compilation**:
  - All visual references, panels and layouts were wired in the bootstrap editor. Successful compilation of all scenes via batchmode without compile errors.

## Phase 9 - AGI Era and Superintelligence Lab (Planned)

- **Secret R&D Lab**:
  - Unlock the national-security lab and advanced frontier research in pursuit of Artificial General Intelligence (AGI).
- **Alignment Crises and Safety Alerts (Safety / Alignment)**:
  - High-risk global events related to autonomous models going out of control, severe hallucinations in critical corporate systems and leakage of confidential models.
  - Pressure from governments and international regulators demanding audits and compliance with AI non-proliferation treaties.
- **Advanced Room Unlocks**:
  - **War Room & Crisis Center**: Rooms dedicated to emergency cybersecurity control and reputation/leak incident management.
- **Defense and Security Partnerships**:
  - Choices to close exclusive contracts with national security agencies or open the technology to open global consortia.
- **New Roles and Specialized Employees**:
  - **AI Safety Researcher** (Advanced Alignment Researcher): Mitigates 60% of catastrophic incident risks of large-scale models.
  - **Lawyer / Compliance** (Corporate Lawyer): Defends the company in lawsuits, reduces regulatory fines and facilitates high-risk data licensing.

## Phase 10 - Steam Demo, Tutorial and General Polish (Planned)

- **Tutorial and Onboarding Experience**:
  - Create a narrative tutorial integrated into the initial garage, guiding the player step by step through hiring the first employee, training models and delivering contracts.
- **Accessibility**:
  - Support for color-blindness filters, UI text resizing for readability (TechPulse, modals) and dynamic control remapping.
- **Steamworks Integration**:
  - Implement achievements, cloud saves (Steam Cloud) and global player stats.
- **Multilingual Localization**:
  - Prepare the game for multiple languages, initially focusing on Portuguese (PT-BR) and English (EN-US).
- **Launch Preparation (Marketing & Steam)**:
  - Produce and edit the official trailer, capture professional screenshots of the diorama and set up the public Steam store page for the public demo.
- **Economic Balancing and Market Simulation**:
  - Final tweaks to difficulty progression, enterprise contract profit margins, infrastructure costs and the speed at which rivals evolve across the different eras of the AI market.