# Model Foundry — Rival System and Market Progression
**Version:** 0.2  
**Status:** design base for the Unity prototype  
**Scope:** campaign, rivals, markets, TechPulse and simulation rules

---

## 1. Product decision

*Model Foundry* is a management sim about founding and scaling an AI company. The player must be able to win through their own strategy — efficiency, vertical products, safety, data, distribution, open models or frontier research — and not just by accumulating money.

Competitors exist to create:
- market context;
- pressure and opportunities;
- stories on TechPulse;
- positioning decisions;
- the feeling of an evolving industry.

They **must not** act like a list of bosses with fixed strength, nor like companies that cheat to forever outpace the player.

### Design goals

1. The player starts small but has credible routes to lead a niche or the market.
2. Each rival has advantages, limits and a readable identity.
3. Comparison is done by **market and product**, not by a global "quality average".
4. Every big rival move creates at least one possible response opportunity.
5. The world advances in time, but results vary between campaigns.
6. References to the real industry are abstract; the universe, brands and products in the game are fictional.

---

## 2. Mandatory brand and logo rule

The current files in `MyAssets/Rivals/` must be treated as **private development references**, not as launch assets.

Received files:
- `openai.png`
- `anthropic.png`
- `grok.png`
- `haploLogo.png`
- `pixflow.jpg`
- `freepik.jpg`
- `minilist.jpg`
- `company-logo-designed.jpg`

### Usage policy

- Do not publish these logos, names or visual identities of third parties in a build, Steam page, trailer, screenshots or marketing.
- Do not use names that mislead the player about a relationship, license or endorsement with real companies.
- Before a public demo, replace all of them with original logos created for *Model Foundry* or explicitly licensed.
- Keep external references outside `Assets/`, for example in `Reference/DoNotShip/`, and ignore them in Git. This reduces the risk of an asset entering the build by mistake.
- Do a trademark check before locking any final commercial name. The names below are internal working names.

### Temporary reference mapping

| ID | Fictional company shown in game | Current reference file | Role in market | Status for launch |
|---|---|---:|---|---|
| `RIV_NEURAFORGE` | NeuraForge | `openai.png` | Frontier lab | Replace logo |
| `RIV_AEGIS` | Aegis Research | `anthropic.png` | Safety and enterprise | Replace logo |
| `RIV_PULSEFRAME` | Pulseframe | `grok.png` | Consumer, real-time data and hype | Replace logo |
| `RIV_HAPLOWORKS` | HaploWorks | `haploLogo.png` | Vertical B2B automation | Verify origin/license; prefer to replace |
| `RIV_PRISMFLOW` | Prismflow | `pixflow.jpg` | Creative, video and multimodality | Verify origin/license; prefer to replace |
| `RIV_VECTORIA` | Vectoria | `freepik.jpg` | Visual content and data platform | Replace logo |
| `RIV_MINICORE` | MiniCore | `minilist.jpg` | Small AI, on-device and efficiency | Verify origin/license; prefer to replace |
| `RIV_CLOUDHARBOR` | Cloudharbor | `company-logo-designed.jpg` | Cloud, GPUs and infrastructure | Verify origin/license; prefer to replace |

> **Note:** the reference file does not determine the company's final identity. Name, symbol, palette and personality must be original.

---

## 3. Market structure

The game will not have a single "best AI" scoreboard. Each launch belongs to a market with different criteria.

### Playable markets

| Market | Example products | Most important metrics |
|---|---|---|
| Assistants and automation | chatbots, copilots, support | quality, cost, reliability, integration |
| Enterprise and operations | OCR, classification, forecast, workflows | security, precision, support, compliance |
| Creativity and media | image, video, voice, design | aesthetic quality, speed, license, control |
| Vision and industry | inspection, robotics, visual analysis | precision, latency, robustness, hardware |
| Foundation models | LLMs, multimodal, reasoning | capability, context, inference cost, safety |
| Edge and devices | local models, embedded AI | efficiency, size, privacy, battery |
| Infrastructure | cloud, GPUs, serving, data | availability, price, capacity, energy |

### Per-market product score

Each product gets an internal 0-100 score per dimension. The commercial score varies by market.

```text
ProductScore =
  Capability
  + Reliability
  + CostEfficiency
  + SafetyAndCompliance
  + DistributionFit
  + UserExperience
```

The weighting changes per segment. A banking model values safety and reliability; a creative tool values visual quality and speed; an on-device model values efficiency.

**Rule:** a player launch below the technical frontier can still win if it delivers better cost, integration, privacy, UX, niche fit or distribution.

---

## 4. Campaign and technological eras

### Default duration

- Start: January **2016**
- End of main campaign: December **2032**
- After the end: sandbox mode with generated events
- Scale: one in-game month is the main economic unit; weeks exist for animations, tasks and short alerts.

The campaign uses years as reference, but it is an alternate history. The player can anticipate certain advances, delay them, or force the market to react to one of their own discoveries.

### Global evolution rules

Each era has:
- a minimum date;
- enabled technologies;
- an expected `MarketFrontier`;
- transition events;
- new competitor classes;
- new client demands.

The date opens possibilities; it does not grant automatic power. To take advantage of a new era the company needs team, research, data, compute and cash.

### Eras

| Era | Base period | Theme | What changes for the player |
|---|---|---|---|
| 1. Applied AI | 2016–2017 | automation, classification, basic vision | first contracts, small datasets, expensive cloud |
| 2. Operational deep learning | 2018–2020 | specialized models and pipelines | MLOps, proprietary data, small GPU clusters |
| 3. Generation and platforms | 2021–2022 | text, image, voice generation and APIs | platform products, content risks, scale |
| 4. Foundation models | 2023–2025 | LLMs, multimodality and open weights | high costs, benchmarks, big contracts, global competition |
| 5. Reasoning and agents | 2026–2028 | reasoning, tools and autonomous flows | safety, observability, autonomy and incidents |
| 6. Autonomous ecosystems | 2029–2032 | multi-agent systems and own infrastructure | datacenter, governance, energy, acquisitions and market leadership |

### Era transition

To keep the campaign alive, an era advances when:

```text
The calendar reached the minimum date
AND
a relevant market event or discovery occurred
AND
at least one company reached the required technical capacity
```

The player can be that company. If they make a discovery before rivals, they get a leadership window and others must react — instead of immediately receiving the same technology.

---

## 5. Rival companies

### 5.1 NeuraForge — frontier lab

| Field | Definition |
|---|---|
| ID | `RIV_NEURAFORGE` |
| Archetype | Frontier / aggressive |
| Entry | 2016 |
| Markets | foundation models, APIs, agents |
| Strengths | research, fundraising, launch and brand |
| Weaknesses | operating cost, public incidents, excessive frontier focus |
| Strength curve | Tier 2 in 2016 → Tier 4 in 2022 → Tier 5 in 2025 |
| Typical response | fast launch, benchmark, selective price cut, recruiting |

NeuraForge is the technical reference of the market at specific moments, but it should not dominate every segment. It can ignore mid-size contracts, local products and specific integrations — natural space for the player.

### 5.2 Aegis Research — safety and enterprise

| Field | Definition |
|---|---|
| ID | `RIV_AEGIS` |
| Archetype | cautious / research-first |
| Entry | 2021 |
| Markets | enterprise, foundation models, safety |
| Strengths | trust, compliance, client retention |
| Weaknesses | slower launches, high cost, less consumer appeal |
| Strength curve | absent → Tier 3 in 2022 → Tier 5 in 2027 |
| Typical response | audit, safe launch, corporate partnership, technical report |

Aegis is strong when the market suffers incidents. It should not punish the player for being bold; it should convert market fear into premium contracts.

### 5.3 Pulseframe — real-time data and consumer product

| Field | Definition |
|---|---|
| ID | `RIV_PULSEFRAME` |
| Archetype | bold / viral |
| Entry | 2023 |
| Markets | consumer, search, assistants and agents |
| Strengths | distribution, speed, hype and behavior data |
| Weaknesses | volatile reputation, privacy, product stability |
| Strength curve | absent → Tier 3 in 2024 → Tier 4 in 2026 |
| Typical response | provocative posts, beta launch, unexpected integration, controversy |

Pulseframe should move TechPulse but not be "infinite money". In exchange for fast growth, it accumulates crisis risk and instability.

### 5.4 HaploWorks — vertical automation

| Field | Definition |
|---|---|
| ID | `RIV_HAPLOWORKS` |
| Archetype | pragmatic / B2B |
| Entry | 2016 |
| Markets | OCR, support, workflows, forecasting |
| Strengths | recurring contracts, deployment, disciplined cost |
| Weaknesses | little frontier innovation, less flashy product |
| Strength curve | Tier 2 → Tier 3 |
| Typical response | contract discount, regional expansion, integration partnership |

HaploWorks is the most important early rival. It teaches that a useful, well-sold product can beat more sophisticated technology.

### 5.5 Prismflow — creation and multimodality

| Field | Definition |
|---|---|
| ID | `RIV_PRISMFLOW` |
| Archetype | creative / trend-driven |
| Entry | 2018 |
| Markets | image, video, audio and creative tools |
| Strengths | creative community, visual launches, virality |
| Weaknesses | rights issues, moderation and client fidelity |
| Strength curve | Tier 2 → Tier 4 |
| Typical response | creative challenge, feature launch, community campaign |

### 5.6 Vectoria — content, data and distribution

| Field | Definition |
|---|---|
| ID | `RIV_VECTORIA` |
| Archetype | platform / marketplace |
| Entry | 2017 |
| Markets | media, licensed data, creative tools |
| Strengths | distribution, catalog, data partnerships |
| Weaknesses | dependence on creators and licensing disputes |
| Strength curve | Tier 2 → Tier 3 |
| Typical response | exclusive licensing, bundle, partner campaign |

Vectoria does not need to build the best model; it can reduce a rival lab's advantage by owning data or distribution.

### 5.7 MiniCore — efficiency and on-device

| Field | Definition |
|---|---|
| ID | `RIV_MINICORE` |
| Archetype | efficient / engineering-first |
| Entry | 2019 |
| Markets | edge, devices, cheap inference |
| Strengths | latency, cost, privacy and hardware |
| Weaknesses | frontier capacity, dependence on device partners |
| Strength curve | Tier 2 → Tier 4 |
| Typical response | compression, compact model, hardware contract |

MiniCore creates a valid route against the parameter race: win by cost and local distribution.

### 5.8 Cloudharbor — infrastructure and energy

| Field | Definition |
|---|---|
| ID | `RIV_CLOUDHARBOR` |
| Archetype | infrastructure / platform |
| Entry | 2016 |
| Markets | cloud, compute, serving, energy |
| Strengths | capital, capacity, datacenter and contracts |
| Weaknesses | does not have the best end product; exposure to energy and regulation |
| Strength curve | Tier 4 → Tier 5 |
| Typical response | GPU price change, new datacenter, exclusive partnership, limited capacity |

Cloudharbor is richer than model labs in many moments, but it does not directly dispute the player's reputation on each product. It changes the cost and availability of compute for everyone.

---

## 6. Strength, wealth and uncertainty

### Do not use a fixed financial hierarchy

The phrase "company X is always the richest" reduces replayability and makes the economy predictable. Instead, each company has capital bands and a financial strategy.

| Band | Meaning |
|---|---|
| Low | needs revenue or a round soon |
| Stable | can fund normal projects |
| High | can afford medium-term bets |
| Strategic | can influence the market, hire and buy capacity |
| Hyperscale | can build infrastructure and absorb shocks |

The game conveys this information through visible signals:
- office size;
- round announcements;
- datacenter expansion;
- mass hiring;
- posture in contracts;
- TechPulse posts;
- market reports.

The exact rival cash value is hidden, except when leaks, IPOs or reporting reveal it.

### Internal attributes of each company

```text
Capital
Research
Compute
DataAccess
Operations
Distribution
BrandTrust
SafetyMaturity
DomainCapability[market]
CurrentStrategy
FinancialRunway
ActiveProjects
RecentIncidents
```

The **Tier** is only a UI summary. The real result comes from the combination of these attributes.

---

## 7. Competitor decision engine

Every month, each active company picks one strategic action. A company cannot execute two big actions in a row without cost, cooldown or available capacity.

### Monthly process

```text
1. Update cash, revenue, costs and ongoing projects.
2. Measure opportunities and threats per market.
3. Identify the company's top priority.
4. Select an allowed action.
5. Resolve result, market impact and TechPulse.
6. Apply cooldowns and create leads for the player.
```

### Possible priorities

- protect market share;
- respond to a player launch;
- recover reputation;
- raise capital;
- reduce cost;
- enter a new market;
- acquire talent or a company;
- launch a model;
- publish research;
- close a partnership;
- create an open standard;
- survive a crisis.

### Main actions

| Action | Effect | Window for the player |
|---|---|---|
| Launch model | raises a market's frontier | differentiate by niche, price or trust |
| Cut price | pressures margin | focus on premium, efficiency or bundle |
| Open weights | lowers the technical barrier | adapt, specialize or sell service |
| Sign partnership | strengthens distribution or data | seek another partner or verticalize |
| Announce benchmark | generates hype and expectation | challenge, ignore or release own evaluation |
| Hire key team | increases future capacity | counter-offer, alternative hiring or automation |
| Invest in datacenter | reduces future cost | lock cloud capacity before the change |
| Suffer incident | drops trust | offer a safe alternative and win contracts |
| Acquihire | strengthens a specialty | buy a smaller competitor or defend talent |
| Run campaign | increases demand in the sector | ride the traffic and compete on communication |

### Fairness rules

- A rival only reacts directly to the player when both operate in the same market.
- Response actions have a cooldown; a rival cannot "copy" every player innovation the next month.
- Companies have defined blind spots based on archetype.
- Big companies are slow in small markets; startups are fast but fragile.
- The player gets advance signals of big moves via rumors, job posts, TechPulse, contracts and reports.

---

## 8. Market frontier and reaction to the player

Each market has a `MarketFrontierScore`, computed from the best active products in that sector.

```text
MarketFrontierScore = best combination of capability, reliability,
cost, safety and distribution in the sector.
```

The reputation of a player launch depends on:

```text
Market reaction =
  segment fit
  + difference to the frontier
  + marketing promise
  + launch stability
  + price
  + prior reputation
  + TechPulse coverage
```

### Example

The player launches a legal chatbot with less capability than NeuraForge, but:
- 40% lower cost;
- licensed legal data;
- high compliance;
- on-premise deployment;
- excellent support.

Result: the product can lead the **legal enterprise** segment, even without winning the general benchmark.

---

## 9. TechPulse

TechPulse is a fictional network of news, posts and reactions of the ecosystem. Its goal is to tell stories and signal changes, not to replace the simulation.

### Publication types

| Source | Role |
|---|---|
| Company | launch, hiring, partnership, results and crisis |
| Press | context, investigation, ranking and analysis |
| Client | satisfaction, complaint, use case, migration |
| Employee | culture, rumor, burnout, pride, leak |
| Researcher | paper, benchmark, technical critique |
| Creator | reaction to a creative product or trend |
| Investor | round, confidence, pressure for returns |
| Regulator | investigation, consultation, fine, requirement |
| Community | memes, praise, questions and light discussion |

### Generation rules

- A publication must be tied to a real simulator event.
- The tone depends on author, segment, quality, hype and trust.
- Rumors must be clearly labeled as rumor and have a chance of being debunked.
- Competitor posts must anticipate consequences: "new cloud region", "research lead opening", "model teaser", "API price change".
- User posts must vary between technical reaction, business reaction and casual comment. They must not be repetitive, nor always positive/negative.

---

## 10. Difficulty progression

| Mode | Rivalry | Economy | Recovery |
|---|---|---|---|
| Startup | less aggressive rivals, clear leads | safer margin | contracts and investors more available |
| Founder | standard | standard | limited recovery |
| Frontier | rivals react earlier and execute better | high costs | bad decisions cost more |
| Simulation | more volatile market and incomplete information | expensive capital | no protection against bankruptcy |

Difficulty must not give rivals an "invisible quality bonus". It should alter planning, cash, reaction speed, available information and market tolerance.

---

## 11. Implementation scope

### Initial prototype — required

Use only four active companies:
- HaploWorks;
- Vectoria;
- NeuraForge;
- Cloudharbor.

Active markets:
- automation/support;
- enterprise;
- infrastructure.

Playable period of the prototype:
- 2016 to 2020.

Minimum systems:
- monthly calendar;
- player project;
- one metric per product;
- budget and cloud cost;
- three rival actions;
- basic TechPulse;
- one launch response;
- save/load.

### Phase 2

Add:
- Prismflow;
- MiniCore;
- image/audio generation;
- hiring and retention;
- data and licensing;
- market reports;
- smaller dynamic competitors.

### Phase 3

Add:
- Aegis Research;
- Pulseframe;
- foundation models;
- open weights;
- evaluation, safety and incidents;
- agents;
- partnerships and acquisitions;
- bigger public crises.

---

## 12. Recommended Unity data model

```csharp
public enum CompetitorArchetype
{
    Frontier,
    SafetyEnterprise,
    ConsumerViral,
    VerticalB2B,
    Creative,
    Marketplace,
    EfficientEdge,
    Infrastructure
}

public sealed class CompetitorDefinition
{
    public string id;
    public string displayName;
    public string logoKey;
    public CompetitorArchetype archetype;

    public int startYear;
    public string[] markets;

    public float startingCapital;
    public float startingResearch;
    public float startingCompute;
    public float startingDistribution;
    public float startingTrust;

    public string[] strengths;
    public string[] weaknesses;
}
```

```csharp
public sealed class CompetitorRuntimeState
{
    public float capital;
    public float revenue;
    public float runwayMonths;

    public float research;
    public float compute;
    public float dataAccess;
    public float operations;
    public float distribution;
    public float brandTrust;
    public float safetyMaturity;

    public Dictionary<string, float> marketCapability;
    public List<string> activeProjects;
    public List<string> activeCooldowns;
    public List<string> recentIncidents;
}
```

### Logo asset pipeline

```text
Assets/
  Art/
    Brand/
      Rivals/
        neuraforge_logo.png
        aegis_logo.png
        pulseframe_logo.png
        haploworks_logo.png
        prismflow_logo.png
        vectoria_logo.png
        minicore_logo.png
        cloudharbor_logo.png

Reference/
  DoNotShip/
    third_party_logo_references/
```

The game should only load the final logos, named by `logoKey`, from `Assets/Art/Brand/Rivals/`.

---

## 13. System approval criteria

Before considering the system done:

- [ ] No real brand logo or name appears in the public build.
- [ ] Each rival has at least two playable weaknesses.
- [ ] A rival action always creates a possible reaction for the player.
- [ ] Product comparison is specific to the market.
- [ ] The player can lead at least one segment before Era 4.
- [ ] TechPulse only publishes facts, rumors or reactions connected to the simulation.
- [ ] The prototype works with four rivals before expanding to eight.
- [ ] Tier is informative; the real variables determine the outcome.
- [ ] The campaign remains possible even if the player does not compete on foundation models.
- [ ] Every brand and logo asset has a recorded origin and license.

---

## 14. Decisions made in this version

1. The campaign moved from 2016–2026 to 2016–2032, with a later sandbox.
2. Competitors are no longer direct imitations of real companies.
3. The eight current files are temporary references; they do not enter the launch.
4. Rival strength is no longer fixed and now depends on attributes, cash, market and decisions.
5. Cloudharbor is infrastructure; it affects everyone but does not compete as a lab on every product.
6. Quality is no longer global and is now evaluated per market.
7. TechPulse now communicates real events, rumors and reactions, instead of generating random posts.
8. The MVP starts with four companies and three markets to keep scope controlled.