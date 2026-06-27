> **Status update (June 27, 2026):** this document is now historical/reference material for concept, tone, art direction and prototype scope. New gameplay progression, difficulty, AI Arena benchmarks, infrastructure, economy and future multiplayer work should follow **[docs/realistic_ai_company_progression_plan.md](./docs/realistic_ai_company_progression_plan.md)**.

## Visual direction: what we should absorb from *Game Dev Tycoon*

The reference conveys the right "fun management game" feel, with one important caveat:

- *Game Dev Tycoon* is **2.5D isometric / diorama**, not realistic 3D.
- Rooms read like a miniature open office so the player can observe everything.
- Characters are simple but have enough animation to feel alive: walking, typing, arguing at a whiteboard, recording video, testing something, celebrating or being overwhelmed.
- Most readability comes from **strong colors, clear silhouettes, large objects and very direct UI**.
- The game does not depend on complex graphics. It sells the fantasy through visible progress: leave the garage, get bigger rooms, hire people, open R&D, a secret lab, a video studio, etc.

The official Steam page confirms the core pitch is exactly starting in a garage, growing into bigger offices, hiring a team, researching technologies, creating special projects and unlocking labs; it is classified as management, simulation, strategy, economy, isometric/2D. ([Steam store][1])

For our project we will not copy their art, interface, layout, names, characters or assets. But we will use the same **product language**:

> isometric cutaway office + animated characters + specialized rooms + clear management menus + highly visible progress.

The difference is that the player does not make games — they found and scale an AI lab.

---

# Our game concept

## Working title

**Model Foundry**
Alternatives:

- **Frontier Labs**
- **Neural Empire**
- **Compute & Co.**
- **Inference Inc.**
- **Singularity Labs**
- **Model Tycoon**
- **The AI Lab**

I would avoid "AI Tycoon" because there are already games using that name or very close variants.

## Main fantasy

The player starts in 2016 or 2017 with a small tech startup and a tiny team.

Early on they do simple things:

- a customer-support chatbot;
- image classification;
- an OCR tool;
- a recommender;
- simple text generation;
- voice models;
- business automation.

As they grow, they move through phases equivalent to the real evolution of the industry:

1. **Automation startup**
2. **Machine learning company**
3. **Deep learning lab**
4. **AI API company**
5. **Foundation model builder**
6. **Datacenter operator**
7. **AGI / superintelligence lab**

The goal is not "click until you have more money"; it is to make decisions that shape the identity of the company:

- be a closed enterprise company;
- compete on open models;
- focus on cost and efficiency;
- be the leader in multimodality;
- build very safe AI;
- take risks to ship first;
- sell infrastructure;
- become a research company;
- sell consumer products directly.

---

# Gameplay structure

## Core loop

The cycle should be short, visible and addictive:

1. You receive a problem or opportunity.
2. Pick an AI project.
3. Assemble the team.
4. Set the budget.
5. Choose data, infrastructure and strategy.
6. The team works visually in the rooms.
7. Training finishes.
8. The model gets metrics.
9. You ship, sell, improve or abandon.
10. The company grows and unlocks new decisions.

Example:

> A company asks for an AI model for bank customer support.

The player can:

- use a smaller, cheaper model;
- buy licensed data;
- use public data and accept legal risk;
- hire a security specialist;
- prioritize latency;
- prioritize quality;
- sell as an API;
- deploy on-premise;
- open the weights;
- ship fast, before a competitor.

Each choice changes real in-game metrics.

---

# Core metrics

The heart of the game has to be easy to understand but deep when the player wants to optimize.

| Metric               | What it represents                          |
| -------------------- | -------------------------------------------- |
| **Cash**            | Available money                              |
| **Runway**          | How many months the company survives without revenue |
| **Compute**         | Training/inference capacity                  |
| **Data**            | Volume and quality of data                   |
| **Talent**          | Team quality                                 |
| **Research**        | Ability to innovate                          |
| **Model Quality**   | Technical result of the model                |
| **Inference Cost**  | How much it costs to serve users             |
| **Latency**         | Response speed                               |
| **Safety**          | Risk of problematic behavior                 |
| **Trust**           | Confidence from companies and users         |
| **Hype**            | Public / press attention                     |
| **Energy**          | Energy / datacenter cost                     |
| **Technical Debt**  | Fragile systems and maintenance cost         |
| **Regulatory Risk** | Legal and regulatory risk                    |

The player must not see twenty numbers at the start. Early on only these appear:

- money;
- reputation;
- quality;
- cost;
- team.

More technical metrics appear as the company evolves.

---

# Office rooms

The visual joy comes from watching the company change physically.

## Initial phase: garage

- one desk;
- laptop;
- whiteboard;
- a noisy server;
- coffee;
- the founder working alone;
- first freelancers.

## Startup phase

- engineering room;
- small meeting room;
- data room;
- GPU rack;
- support space;
- demo studio.

## Lab phase

- R&D Lab;
- evaluation room;
- safety / alignment room;
- data / licensing room;
- enterprise sales room;
- PR room;
- NOC / operations center;
- own datacenter.

## Hyperscale phase

- corporate building;
- modular datacenter;
- GPU cluster;
- crisis center;
- war room;
- secret lab;
- board room;
- international office;
- research campus.

Each room unlocks new systems, not just numeric bonuses.

Example:

| Room           | System unlocked                                |
| -------------- | ---------------------------------------------- |
| Data Lab       | Cleaning, curation and buying datasets        |
| R&D Lab        | New architectures and scientific breakthroughs |
| GPU Cluster    | Train bigger models                            |
| Evaluation Lab | Benchmarks, red teaming and quality            |
| Safety Lab     | Alignment, filters and incident reduction     |
| Sales Floor    | Enterprise contracts                           |
| PR Room        | Hype, press and crisis management              |
| Data Center    | Own infrastructure and scalable inference     |
| Board Room     | Investors, IPO, mergers and acquisitions       |

---

# Characters and animations

Characters should be simple, stylized and cheap to produce.

We should not aim for realistic characters or complex rigs. The best direction is:

- low-poly body;
- slightly enlarged head;
- clothes and accessories per profession;
- few facial details;
- short looped animations;
- clear reading even with a distant camera.

## Employee classes

- Founder / CEO
- Machine Learning Engineer
- Research Scientist
- Data Engineer
- MLOps Engineer
- Backend Engineer
- Product Manager
- Security Researcher
- AI Safety Researcher
- Sales Executive
- Lawyer / Compliance
- Community Manager
- Infrastructure Engineer
- GPU Technician
- Recruiter
- Finance Lead

## Useful animations

- walk;
- sit and type;
- work at a whiteboard;
- point at a chart;
- talk in a meeting;
- carry a server / equipment;
- operate a rack;
- celebrate a launch;
- panic during an incident;
- sleep at the desk;
- drink coffee;
- test a demo;
- present to investors.

These animations make the game feel alive even when the underlying system is fairly simple.

---

# Recommended art style

## Do not use "3D realism"

For this project realism would be expensive, slow to produce and largely unnecessary.

The ideal direction is:

- orthographic isometric camera;
- miniature rooms;
- cutaway walls;
- large recognizable objects;
- simple materials;
- soft lighting;
- subtle shadows;
- strong colors per department;
- cartoonish low-poly characters;
- clear, almost "board game" UI.

Reference feeling:

> A mix of *Game Dev Tycoon*, *Two Point Hospital*, *Startup Company* and a futuristic AI office.

## Identity per department

| Area             | Visual direction                                |
| ---------------- | ----------------------------------------------- |
| Engineering      | blue, monitors, cables, dashboards             |
| Research         | purple, boards, formulas, papers               |
| Data             | yellow, storage, pipelines, ingestion screens   |
| Safety           | dark red, alerts, red team                     |
| Sales            | green, presentations, contracts                |
| Infrastructure   | gray and blue, racks, ventilation, cables      |
| PR               | orange, cameras, press, social media           |
| Board            | black, wood, metric screens                    |

---

# Recommended engine

## Main recommendation: Unity 6.3 LTS + C#

I would use:

- **Engine:** Unity 6.3 LTS
- **Language:** C#
- **Rendering:** URP
- **UI:** UI Toolkit
- **Modeling:** Blender
- **Animation:** Unity Animator
- **Version control:** Git + Git LFS
- **Initial distribution:** Steam for Windows
- **Audio:** FMOD or Unity Audio, starting with Unity Audio
- **Save:** local JSON; Steam Cloud later

Unity 6.3 LTS has long-term support until December 2027 and Unity itself recommends LTS for projects entering production that need stability. ([Unity][2])

Unity makes sense for this game because:

- it is essentially a PC Steam game;
- we have a light 3D environment with many objects and UI;
- C# is good for simulation, economy and data systems;
- there is a lot of material, plugins and assets for offices, low-poly, animation and UI;
- it is easier to hire or find people who know Unity / C#;
- Steam and Unity have mature integration;
- the engine handles this kind of game with margin to spare.

Unity's UI Toolkit also supports building runtime UI with its own structure and style files; that is useful for the many menus, cards, tooltips, panels and dashboards this game will require. ([Unity Docs][3])

## Why not Unreal Engine?

Unreal would be overkill for this project.

It is excellent for:

- realistic graphics;
- FPS;
- large worlds;
- cinematics;
- high-fidelity characters.

But our game needs:

- productivity;
- dense UI;
- economy;
- simulation;
- isometric camera;
- stylized art;
- lightweight build;
- short production time.

Unity is more appropriate.

## And Godot?

Godot would be a good second option, especially if the goal is to avoid licence dependency and keep everything open source.

I would only choose Godot if one of these were a priority:

- very low budget;
- smaller project;
- a team already comfortable with Godot;
- a desire for fully open-source code and pipeline;
- a plan to use a very simple aesthetic.

For a commercial Steam production with large UI, bought assets, animations, several systems and the possibility of hiring freelancers, I would go with Unity.

---

# Technical architecture

The main rule is: **the game must be data-driven**.

We cannot hardcode models, employees, events, technologies and companies in scattered scripts.

## Suggested structure

```text
Assets/
  Art/
    Characters/
    Environments/
    Props/
    UI/
    VFX/
  Audio/
  Prefabs/
    Rooms/
    Employees/
    Props/
    UI/
  Scripts/
    Core/
    Simulation/
    Economy/
    Employees/
    Projects/
    Models/
    Research/
    Events/
    UI/
    Save/
  ScriptableObjects/
    Employees/
    Rooms/
    Technologies/
    Projects/
    Events/
    Models/
    Competitors/
  Scenes/
    MainMenu/
    Garage/
    Office_01/
    Office_02/
    Campus/
```

## Main systems

```text
GameManager
 ├── TimeSystem
 ├── EconomySystem
 ├── EmployeeSystem
 ├── ProjectSystem
 ├── ModelTrainingSystem
 ├── ResearchSystem
 ├── DataSystem
 ├── ComputeSystem
 ├── EventSystem
 ├── CompetitorSystem
 ├── ReputationSystem
 ├── SaveSystem
 └── UIStateSystem
```

## Configurable data

Each employee, project, technology and model should exist as a configurable asset.

Conceptual example:

```csharp
public class ModelDefinition
{
    public string id;
    public string displayName;

    public ModelFamily family;
    public int parameterTier;

    public float requiredCompute;
    public float requiredDataQuality;
    public float trainingDurationDays;

    public float baseCapability;
    public float baseInferenceCost;
    public float baseLatency;

    public float safetyRisk;
    public float hypePotential;
}
```

This way we can create new content without rewriting systems.

---

# How to adapt Game Dev Tycoon's logic to AI

| Game Dev Tycoon         | Our game                                |
| ----------------------- | --------------------------------------- |
| Game theme              | AI application area                     |
| Genre                   | Type of model                           |
| Platform                | Distribution channel                    |
| Engine                  | Training stack                          |
| Gameplay/Story/Graphics | Quality, cost, speed, safety            |
| Bugs                    | Hallucinations, failures, incidents     |
| Fans                    | Users, devs, clients, community         |
| Research                | Papers, architectures, hardware, tooling |
| Special projects        | Foundation models, datacenter, AGI lab  |
| Game report             | Benchmark report and technical postmortem |
| Publisher               | VC, enterprise client, cloud partner    |
| Competitor              | Rival lab                               |

Example project:

```text
Product:
Legal assistant for companies

Model:
Specialized LLM

Market:
B2B Enterprise

Strategy:
High precision + privacy

Data:
Licensed legal corpus

Deployment:
On-premise

Risk:
High cost, slow market, strong legal requirements
```

---

# First playable version: MVP

We should not start by trying to build "full OpenAI Tycoon".

The first version has to prove the game is fun in 20 to 30 minutes.

## MVP scope

### Scenario

A garage and a company with:

- 1 founder;
- 1 developer;
- 1 small GPU;
- 2 client types;
- 3 projects;
- 4 metrics;
- 1 competitor;
- 1 office upgrade system.

### Initial projects

1. Email classifier
2. Support chatbot
3. Image detector

### Decisions per project

- budget;
- quality vs speed;
- buy better data or not;
- hire a freelancer;
- use cloud or own hardware;
- ship fast or test more.

### Visible metrics

- money;
- reputation;
- quality;
- monthly cost;
- users / clients.

### MVP rooms

- garage;
- mini office;
- small R&D lab.

### MVP events

- a client complains about bad answers;
- a server goes down;
- a competitor launches a similar product;
- an investor asks for a meeting;
- a company offers a big contract;
- an employee asks for a raise;
- a data leak;
- a bad benchmark;
- unexpected virality.

---

# Development order

## Phase 0 — Pre-production, 2 weeks

Goal: validate the concept before producing art.

Deliverables:

- short game design document;
- list of metrics;
- progression map;
- menu wireframes;
- project-loop prototype;
- visual document;
- choose a working title;
- Git repository;
- Unity configured.

## Phase 1 — Vertical slice, 4 to 6 weeks

Goal: a small part of the game already looking like the final art.

Deliverables:

- one garage room;
- isometric camera;
- one animated character;
- one workstation;
- one AI project;
- progress bar;
- result with metrics;
- money;
- hiring one employee;
- a polished UI;
- simple save/load.

At the end of this phase we can already record a Steam video and show the idea.

## Phase 2 — Core loop, 6 to 10 weeks

Deliverables:

- projects with different categories;
- teams and attributes;
- research;
- models;
- data;
- compute;
- events;
- competitors;
- reputation;
- office expansion;
- a more balanced economy.

## Phase 3 — Content and depth, 8 to 12 weeks

Deliverables:

- more rooms;
- more employees;
- more models;
- more events;
- tech tree;
- enterprise contracts;
- reputation crises;
- safety / alignment;
- investment;
- competitor acquisition;
- datacenter.

## Phase 4 — Steam demo and polish

Deliverables:

- tutorial;
- achievements;
- balancing;
- accessibility;
- translations;
- Steam Cloud;
- trailer;
- screenshots;
- Steam page;
- a playable demo.

Steam offers features like achievements, stats and cloud saves; the SDK is required to upload builds to the platform, while the rest are optional. ([Steamworks][4])

---

# First practical sprint

I would start exactly like this.

## Day 1

- create Unity 6.3 LTS project;
- configure URP;
- create Git repository;
- configure Git LFS for `.fbx`, `.blend`, `.psd`, `.wav`, `.mp4`;
- create the `GaragePrototype` scene;
- add an orthographic isometric camera;
- add floor, cutaway walls and one desk.

## Day 2

- create a temporary low-poly character;
- implement walking between points;
- implement idle and typing animations;
- create an interactive workstation.

## Day 3

- create the project panel;
- create a simple project: "SupportBot v0.1";
- start the project;
- show a progress bar;
- consume money over time.

## Day 4

- when finished, generate the result:

  - quality;
  - cost;
  - clients;
  - reputation;
  - monthly revenue.

## Day 5

- create the hiring of an ML Engineer;
- increase work speed;
- show the employee walking to a free desk.

## Days 6 and 7

- add the first random event;
- create save/load;
- record a short video;
- test whether the loop is already fun without final art.

---

# Objective decision

I would stick with this stack:

```text
Engine: Unity 6.3 LTS
Language: C#
Render: URP
UI: UI Toolkit
3D: Blender
Animation: Mixamo temporarily + custom animations later
Versioning: Git + Git LFS
Initial platform: Windows / Steam
Save: local JSON
Steam later: Steamworks + Steam Cloud + Achievements
```

And the first goal should not be "make the entire game".

It should be to create, in up to six weeks, a demo with:

> garage + character working + one model being trained + business result + hiring + expansion into an R&D room.

When that version is fun, it makes sense to invest heavily in original assets, room modeling, events, the tech tree and long-term content.

[1]: https://store.steampowered.com/app/239820/Game_Dev_Tycoon/ "Game Dev Tycoon on Steam"
[2]: https://unity.com/releases/unity-6/support?utm_source=chatgpt.com "Unity 6 Releases & Support: LTS & Updates Releases"
[3]: https://docs.unity3d.com/6000.2/Documentation/Manual/UIElements.html?utm_source=chatgpt.com "UI Toolkit"
[4]: https://partner.steamgames.com/doc/sdk?utm_source=chatgpt.com "Steamworks SDK"
