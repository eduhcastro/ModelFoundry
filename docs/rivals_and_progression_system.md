# Technical Planning: Rivals and Time Progression

This document details *Model Foundry*'s rival company system, the mapping of the custom logos placed in `MyAssets/Rivals/`, the wealth tier and the time progression of each rival throughout the market simulation.

> **License note:** The files in `MyAssets/Rivals/` are private development references only. They must NOT be shipped in any public build, Steam page, trailer, screenshot or marketing material. Replace them with original art before any public demo.

---

## 1. Logo Mapping and "Premium" Competitors

In the `MyAssets/Rivals/` folder there are three image files:
* `openai.png`
* `anthropic.png`
* `grok.png`

They map directly to three major historical industry competitors, represented in-game by the following companies configured in the **`CompetitorManager`**:

| Logo | In-Game Competitor | Personality | Strength Tier (1-5) | Main Focus |
| :--- | :--- | :--- | :--- | :--- |
| **`openai.png`** | **NeuraCorp** | *Aggressive* | Tier 5 (Leader) | Fast launches, frontier models (GPT-like), massive VC capital. |
| **`anthropic.png`** | **AnthroTech** | *Cautious* | Tier 4 (Growing to 5) | Safety (alignment), enterprise-focused models, ethical posts. |
| **`grok.png`** | **Quantum Minds** | *Bold* | Tier 4 (Unlimited finances) | Open weights (initially), ironic/humorous posts, integration with automakers. |

---

## 2. Who Is the Richest Company? (Financial Hierarchy)

1. **TitanCloud** (Infrastructure): The in-game cloud compute provider. Although it does not compete with the player on models directly, it is the most valuable company in the ecosystem because it rents GPUs to everyone else.
2. **NeuraCorp** (`openai.png`): The richest among model builders. Started with strong VC backing in 2015/2016 and leads the global market share.
3. **Quantum Minds** (`grok.png`): Has the biggest late-game financial runway because it is funded by a conglomerate of satellites and electric vehicles.
4. **AnthroTech** (`anthropic.png`): Grows substantially by raising funds from large e-commerce and cloud corporations that want alternatives to NeuraCorp.

---

## 3. Timeline and Progression

The game progresses from **2017 to 2026** (the simulation's current year). Competitors emerge, gain traction or decline following technological eras:

### Era 1: Garage Startup (2017 - 2018)
The player competes with traditional-tech local competitors:
* **HorizonLabs** (Tier 2 - Consumer): Focuses on simple mobile classification apps and basic automations. It is the weakest competitor and lets the player test their first launches.
* **AtlasAI** (Tier 2 - Creative): A small studio trying to sell basic translation and assisted-design tools.

### Era 2: Deep Learning and Consolidation (2019 - 2021)
The market starts demanding bigger models (early Transformers):
* **PrimordialAI** (Tier 3 - Efficient): Specialists in model compression. They try to ship low-cost, ultra-low-latency products.
* **SakuraNet** (Tier 3 - Innovative): Lab focused on industrial robotics automations and advanced computer vision.
* **CortexAI** (Tier 3 - Pragmatic): Focuses on traditional corporate (B2B) business tools.

### Era 3: API Race and Foundation Models (2022 - 2024)
Generative AI explodes on TechPulse:
* **AnthroTech** (`anthropic.png` - Tier 4): Launches its first giant-context models based on safety (Constitutional AI), attracting big financial corporations.
* **MetaLogic AI** (Tier 4 - OpenSource): Releases competing open models, lowering the barrier to entry and challenging everyone else's margins.
* **Quantum Minds** (`grok.png` - Tier 4): Enters the market strong by integrating real-time social network data. Its TechPulse posts are cynical and humorous.

### Era 4: Frontier and the Race to AGI (2025 - 2026)
Reasoning models and autonomous agents dominate the top of the market:
* **NeuraCorp** (`openai.png` - Tier 5): Consumes gigantic volumes of energy with superclusters of compute, trying to keep a monopoly on frontier intelligence.
* **DeepForge Labs** (Tier 5 - Scientific): Publishes revolutionary papers that halve training cost, challenging NeuraCorp.
* **Nexus Systems** (Tier 5 - Enterprise): Specialists in national-security government integrations.

---

## 4. Visual Integration of Logos in the Game

For the logos in `MyAssets/Rivals` to appear in-game, we propose the following clean integration in the architecture:

1. **`CompetitorCompany` class**:
   - Add a `string LogoPath` or `Sprite Logo` field to represent the company's profile picture in TechPulse.
2. **Dynamic Loading**:
   - When `CompetitorManager` starts, the script tries to load sprites from the corresponding folder:
     - NeuraCorp loads `openai.png`.
     - AnthroTech loads `anthropic.png`.
     - Quantum Minds loads `grok.png`.
     - Other competitors use colors and symbols generated dynamically in `GaragePrototypeBootstrap`.
3. **Display in the Post Prefab**:
   - In `TechPulseUI.CreatePostUI`, instead of generating a generic character (`▲`) in the avatar, the system checks whether the company has a corresponding image defined in `CompetitorManager` and applies it to the `Image` component of `PostAvatar`.

---

## 5. How Rival Companies Act Autonomously

Rival companies' actions directly influence the player's gameplay through:
* **Market Quality Average**: The quality of rival launches defines the "gold standard" of the market. If the player launches a model below the competitor average, their reputation drops.
* **Hype Attacks on TechPulse**: Aggressive companies (NeuraCorp) post taunts if the player takes too long to launch something new.
* **Price and Customer Fluctuation**: When MetaLogic releases a big free open-source model, the player can lose monthly revenue unless they differentiate through superior quality or custom contracts.