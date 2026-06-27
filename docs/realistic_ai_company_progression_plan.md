# Model Foundry - Realistic AI Company Progression Plan

Status: active direction for new gameplay systems.

This document supersedes `IA-TYCOON-PLAN.md` for progression, difficulty, benchmarks, infrastructure, economy and future multiplayer design. The older plan remains useful as historical concept, art direction and prototype scope reference, but new agents should use this document when changing gameplay systems.

## Design Goal

Model Foundry should become a hard but readable AI-company simulation. The player should not climb from garage startup to frontier lab by launching a few profitable products. A strong model should require scarce compute, better staff, data rights, evaluation, deployment capacity, cash discipline and luck in the market.

The core fantasy stays the same: build an AI company. The rules change from "project complete means product succeeds" to "product launch is one step in a long model, infrastructure and market race."

## Real-World Anchors

Use these as design anchors, not as exact legal or financial simulation:

- Arena-style public leaderboards: player models should compete through noisy human preference, task-specific boards and public reputation, inspired by Arena AI/LMArena leaderboards.
- MLPerf-style infrastructure benchmarks: hardware and training systems should be measured by time-to-target-quality, throughput, latency and cost per inference.
- Epoch AI-style scaling pressure: frontier progress should be tied to compute growth, data, algorithmic efficiency and capital intensity.
- Stanford AI Index-style macro constraints: data centers, chip supply chains, energy, regulation, investment and geography should shape strategic constraints.

Sources:

- Arena AI leaderboard: https://arena.ai/leaderboard
- Arena AI text leaderboard: https://arena.ai/leaderboard/text
- MLCommons MLPerf Training: https://mlcommons.org/benchmarks/training/
- MLCommons MLPerf Inference Datacenter: https://mlcommons.org/benchmarks/inference-datacenter/
- Epoch AI trends: https://epoch.ai/trends
- Epoch AI models/data: https://epoch.ai/data/ai-models
- Stanford HAI AI Index 2026: https://hai.stanford.edu/ai-index/2026-ai-index-report
- Stanford HAI AI Index 2025: https://hai.stanford.edu/ai-index/2025-ai-index-report

## New Progression Model

Replace the single `ModelQuality` mindset with a portfolio of capabilities.

Company progression should depend on eight pillars:

1. People
2. Data
3. Compute
4. Research
5. Deployment
6. Product
7. Distribution
8. Trust

Each model launch should produce multiple scores:

- Reasoning
- Coding
- Math
- Writing
- Vision
- Tool use
- Latency
- Cost per 1M tokens
- Safety
- Reliability
- Context handling
- Enterprise compliance

The final public score should be a weighted blend, and different markets should care about different weights.

Examples:

- Consumer chat rewards writing, latency and Arena score.
- Enterprise automation rewards compliance, uptime, privacy and cost.
- Coding agents reward coding, tool use, reliability and sandbox safety.
- Vision products reward vision score, latency and data rights.
- Frontier research rewards reasoning, math, coding and public benchmark wins.

## Full Company Journey

The game should simulate the full journey from one founder to a global frontier AI company. The path should not be linear or guaranteed. The player should be able to become a top company through different strategies, but every path should require tradeoffs.

### Stage 0 - Solo Founder

The player starts with:

- One founder
- One laptop/workstation
- Almost no reputation
- Small savings or pre-seed cash
- No proprietary data
- No serious compute
- A simple website landing page
- A tiny demo model or wrapper product

The early game should be about survival and focus, not instant frontier research.

Available actions:

- Read papers and gain research literacy
- Build small demos
- Launch a simple website
- Write documentation
- Post on TechPulse
- Apply for cloud credits
- Do consulting contracts
- Fine-tune small open models
- Build a waitlist
- Run cheap evals
- Decide company positioning

Early choices should define identity:

- Research-first lab
- Developer tools company
- Enterprise automation startup
- Open-model company
- Consumer AI app company
- Infrastructure/API provider
- Safety/compliance lab

### Stage 1 - First Product Startup

The player should usually not train a frontier model here. They should ship useful products around existing/small models.

Possible first products:

- Customer support bot
- Internal knowledge search
- Image classifier
- Email automation
- Code review helper
- Meeting summarizer
- Website chatbot widget
- Document extraction tool
- Small API endpoint
- Prompt workflow builder

Key systems unlocked:

- Website maintenance
- Product analytics
- User feedback
- Support queue
- Billing
- Basic cloud hosting
- First contracts
- First hires

Main challenge:

- The product can get users before the company can afford inference.
- High usage can be dangerous if gross margin is negative.

### Stage 2 - Product-Market Fit

The company now needs repeatable revenue, not just demos.

Systems unlocked:

- Pricing plans
- Churn and retention
- Customer success
- Sales pipeline
- SOC2/compliance preparation
- Incident response
- Product roadmap
- Design system
- Documentation portal
- API keys and usage limits
- Rate limiting and abuse prevention

The player should choose what kind of company they are building:

- SaaS product company
- API company
- Agent platform
- Coding tools company
- Enterprise platform
- Research lab
- Infrastructure provider

### Stage 3 - Model Company

The company begins building differentiated models.

New requirements:

- Data strategy
- Evals team
- Training pipelines
- Experiment tracking
- Model registry
- Safety pipeline
- Model serving stack
- Benchmark submissions
- Dedicated infra budget

This is where the player first seriously competes on AI Arena.

### Stage 4 - Platform Company

The company becomes a platform, not just a model vendor.

Products can include:

- Web app
- Public API
- SDK
- CLI
- Code editor extension
- Agent builder
- Workflow builder
- Model playground
- Fine-tuning dashboard
- Evaluation dashboard
- Team/admin console
- Marketplace for tools/agents/templates

The challenge becomes coordination. More products increase revenue potential but also create more maintenance, support load, security risk and roadmap complexity.

### Stage 5 - Frontier Lab

The company competes with top labs.

Requirements:

- Large research team
- Compute reservations or owned supercluster
- Data licensing at scale
- Safety governance
- Government/regulatory relationships
- Global enterprise sales
- Multi-region inference
- Public benchmark credibility
- Strong talent brand
- Huge fundraising or profit engine

At this point, the player can attempt 85+ benchmark scores and top leaderboard positions.

### Stage 6 - Top 1 Company

Becoming top 1 should be rare, expensive and unstable.

To be top 1, the player needs:

- Best model or best product ecosystem
- Strongest distribution channel
- Enough inference capacity to serve demand
- Sustainable gross margin
- Trust from users, enterprises and regulators
- Talent retention
- Safety record
- Ability to respond when rivals overtake a benchmark

Top 1 is not permanent. Rivals should catch up, benchmarks should saturate, costs should rise, and regulators/customers should create pressure.

## Product Surface System

Model Foundry should treat "products" as first-class buildable surfaces. A model without product surfaces is research; a product surface turns capability into users and revenue.

Product surfaces:

- Website
- Web app
- API
- SDK
- CLI
- Desktop app
- Mobile app
- Browser extension
- IDE/editor extension
- Chat app
- Agent runtime
- Builder/no-code studio
- Enterprise admin panel
- Documentation portal
- Status page
- Billing portal
- Community/forum

Each product surface should have stats:

- Build progress
- UX quality
- Reliability
- Security
- Documentation quality
- Maintenance load
- Support load
- Distribution reach
- Conversion rate
- Retention impact
- Enterprise readiness

Important rule: a strong model can fail if the product surface is weak.

Example:

```text
Great coding model + bad CLI + poor docs = low adoption
Medium model + excellent workflow builder + good onboarding = profitable niche product
```

## Website and Brand System

The company website should matter mechanically.

Website modules:

- Landing page
- Pricing page
- Docs
- Blog/research posts
- Playground
- API dashboard
- Careers page
- Trust/compliance page
- Customer stories
- Status page

Website metrics:

- Traffic
- Conversion
- Documentation quality
- SEO/discoverability
- Trust
- Support deflection
- Enterprise credibility

Roles:

- Designer improves brand, onboarding and conversion.
- Frontend Engineer improves web app quality and velocity.
- Growth Marketer improves traffic and conversion.
- Developer Advocate improves docs, SDK adoption and community trust.
- Support Engineer reduces churn and incident reputation loss.

Bad website outcomes:

- Users do not understand the product.
- Developers abandon API signup.
- Enterprise customers distrust the company.
- Support load explodes because docs are weak.
- TechPulse mocks confusing positioning.

## Developer Tools Product Line

The game should support a developer-tools path similar to coding agents and AI development platforms.

Developer products:

- Code assistant
- CLI coding agent
- IDE extension
- Repo analysis bot
- Pull request reviewer
- Test generator
- Migration assistant
- Internal codebase Q&A
- Terminal agent
- Cloud coding workspace
- Agent SDK

Developer-tool dimensions:

- Code quality
- Repository understanding
- Tool use
- Shell safety
- Patch quality
- Test reliability
- Latency
- Context length
- Permission model
- Rollback/recovery
- IDE integration
- CLI ergonomics
- Enterprise security

Failure modes:

- Agent deletes important files.
- Agent hallucinates APIs.
- CLI is powerful but unsafe.
- Context window is too small for real repos.
- Enterprise refuses adoption due to data controls.
- Generated code creates security issues.

Developer products should require more than model quality:

- Strong coding benchmark
- Good tool-use benchmark
- Secure sandbox
- Excellent UX
- Clear permissions
- Strong docs
- Fast support
- Integrations with Git providers and CI

## Builder and Agent Platform

Add a product category where players build agent platforms, not just single agents.

Builder features:

- Agent templates
- Tool connectors
- Workflow graph
- Memory configuration
- Evaluation suites
- Deployment environments
- Human approval steps
- Logs and traces
- Cost controls
- Permission scopes
- Marketplace

Agent capabilities:

- Planning
- Tool use
- Web browsing
- Code execution
- File editing
- Data analysis
- Multi-step task handling
- Long-context retrieval
- Memory
- Human handoff

Agent risks:

- Prompt injection
- Tool misuse
- Cost runaway
- Bad autonomous decisions
- Data exfiltration
- Unclear accountability
- Long-running task failures

Agent products should be high reward, high risk. They can win enterprise contracts, but a single bad incident can damage trust.

## Design, UX and Creative Hiring

Add design as a real discipline, not a cosmetic upgrade.

Design roles:

- Product Designer
- UX Researcher
- Brand Designer
- Motion Designer
- Technical Writer
- Developer Experience Designer

Design affects:

- Product onboarding
- Conversion
- Retention
- Enterprise trust
- App usability
- Documentation clarity
- Demo quality
- TechPulse reception
- Accessibility

Design workstreams:

- Brand identity
- Website design
- Product UI
- Design system
- Documentation design
- Launch assets
- Demo videos
- Enterprise pitch decks
- Accessibility pass

Design should be especially important for:

- Consumer chat products
- Builders/no-code tools
- Developer tools
- Enterprise dashboards
- Public launch pages

Bad design should create realistic pain:

- Users cannot find features.
- API signup conversion is low.
- Enterprise buyers perceive the company as immature.
- Support tickets increase.
- Reviews complain about complexity.

## Research and Training Loop

The research loop should be iterative and uncertain.

Research phases:

1. Paper study
2. Hypothesis
3. Small experiment
4. Ablation
5. Data improvement
6. Scaling run
7. Internal eval
8. Safety eval
9. Product integration
10. Public benchmark

Research outputs:

- Algorithmic efficiency
- New architecture
- Better data recipe
- Better post-training
- Better agent scaffold
- Lower inference cost
- Better safety behavior
- Longer context
- Better retrieval

Most experiments should not create major breakthroughs. Failed experiments should still produce learning.

Suggested outcome distribution:

- 45% small learning
- 30% no meaningful gain
- 15% moderate improvement
- 7% costly failure/incident
- 3% breakthrough

Senior researchers and strong infra should improve the distribution, not guarantee success.

## Model Lifecycle

Every model should have lineage.

Model record:

- Model id
- Family
- Parent model
- Architecture
- Parameter tier
- Dataset recipe
- Training compute
- Post-training method
- Eval suite
- Safety profile
- Serving cost
- Latency profile
- Release channel
- Product surfaces using it
- Benchmark history

Release types:

- Internal only
- Private beta
- API preview
- Consumer launch
- Enterprise launch
- Open weights
- Research paper only

Each release type has tradeoffs:

- Open weights increase community trust but reduce moat.
- API preview generates developer adoption but increases support.
- Enterprise launch needs compliance but creates revenue.
- Consumer launch creates public pressure and high serving costs.

## Moat System

Top AI companies are not only models. The game should represent moats.

Moat types:

- Proprietary data
- Distribution
- Talent density
- Compute contracts
- Product ecosystem
- Developer ecosystem
- Enterprise contracts
- Safety reputation
- Brand trust
- Research credibility
- Cost efficiency
- Hardware partnership
- Regulatory approval

Each moat should decay if ignored.

Example:

- Developer ecosystem decays if SDK/CLI/docs are neglected.
- Safety reputation decays after incidents.
- Compute advantage decays when rivals secure new chips.
- Product ecosystem decays if maintenance debt grows.

## Maintenance and Technical Debt

Every product surface and model release should add maintenance.

Maintenance dimensions:

- Site bugs
- API uptime
- SDK compatibility
- CLI regressions
- Model regressions
- Billing issues
- Security patches
- Documentation drift
- Customer support
- Infrastructure upgrades

Technical debt should accumulate when the player ships too fast.

Debt consequences:

- Slower development
- Higher incident risk
- Worse retention
- More support cost
- Lower staff morale
- Failed enterprise audits

Roles that reduce debt:

- Staff Engineer
- Infrastructure Engineer
- QA Engineer
- Security Engineer
- Technical Writer
- Support Engineer
- Engineering Manager

## Realistic Business Models

Support multiple viable business models:

- Subscription SaaS
- API usage billing
- Enterprise contracts
- Seat-based developer tools
- Usage-based coding agents
- Private deployment
- Fine-tuning services
- Consulting/services
- Marketplace revenue share
- Infrastructure resale
- Open-source plus enterprise support

Each has tradeoffs:

- API billing scales but can have thin margins.
- Enterprise is slower but high revenue.
- Consumer SaaS needs brand and UX.
- Developer tools need docs, trust and integrations.
- Open source grows community but weakens pricing power.
- Private deployment needs compliance and support.

## What Was Missing From the Previous Plan

The first version of this document was correct at the high level, but incomplete for the full experience. It had:

- Good benchmark direction
- Good infrastructure direction
- Good difficulty direction
- Good multiplayer caution

It was missing or under-specified:

- Website and brand as mechanical systems
- Design and UX hiring
- Developer products such as CLI, SDK, IDE extension and coding agent
- Builder/no-code agent platform
- Product maintenance and technical debt
- Model lineage
- Business model differentiation
- Full path from solo founder to top 1 lab
- Distribution and developer ecosystem as moats
- Support, docs, billing and compliance as gameplay

## AI Arena System

Create an in-game benchmark network called AI Arena.

AI Arena should not be one percentage bar. It should be a set of boards:

- Arena General
- Arena Coding
- Arena Math
- Arena Vision
- Arena Agents
- Arena Enterprise
- Arena Safety
- MLPerf Training
- MLPerf Inference

Each benchmark should have:

- A public leaderboard rank
- A hidden uncertainty band
- A number of evaluation samples/votes
- A contamination risk
- A benchmark saturation value
- A cost to run official evaluation
- A cooldown before the next submission

Important rule: moving from 20 to 50 should be achievable. Moving from 70 to 80 should be expensive. Moving from 80 to 85 should require a real company transformation. Moving from 85 to 90 should be frontier-lab territory.

Suggested score curve:

- 0-30: toy prototype
- 31-50: useful narrow model
- 51-65: credible startup product
- 66-75: strong commercial system
- 76-82: top-tier specialist
- 83-88: frontier contender
- 89+: global leader, rare and temporary

Score gain should follow diminishing returns:

```text
effective_gain = base_gain * (1 - current_score / 100)^2
```

This makes late-game gains brutally expensive without hard caps.

## Product Launch Rework

Current prototype launches are too generous. A launch should have phases:

1. Prototype
2. Internal eval
3. Red-team
4. Private beta
5. Public launch
6. Post-launch operations
7. Benchmark submission

Skipping phases should be allowed, but risky.

Failure modes:

- Hallucination incident
- Latency collapse
- Inference bill spike
- Model leak
- Bad benchmark result
- Data lawsuit
- Enterprise churn
- Safety recall
- GPU quota exhaustion
- User backlash on TechPulse

Launch revenue should depend on retention and serving capacity, not just quality.

Suggested formula:

```text
revenue = users * conversion_rate * price
gross_margin = revenue - inference_cost - support_cost - incident_cost
```

Models can be popular and still bankrupt the company if inference is too expensive.

## People System

Replace binary hires with staff profiles.

Every employee should have:

- Role
- Level
- Salary
- Burnout
- Domain skills
- Personality
- Hiring market reputation requirement
- Ramp-up time
- Retention risk

Core skills:

- Research
- Engineering
- Data
- Infrastructure
- Product
- Safety
- Security
- Sales
- Finance
- Operations

Employee impact should be multiplicative with systems, not flat bonuses.

Examples:

- A strong Research Scientist improves experiment quality but needs compute.
- A Data Engineer reduces contamination and increases data pipeline throughput.
- An Infrastructure Engineer lowers downtime and inference cost.
- A Safety Researcher reduces catastrophic launch risk.
- A Product Manager improves retention and market fit.
- A Finance Lead improves fundraising terms and runway planning.

Hiring should become difficult:

- Senior staff require reputation, equity packages or signing bonuses.
- Bad culture and burnout increase attrition.
- Big rivals can poach employees.
- Layoffs harm trust but reduce burn.

## Infrastructure and Hosting

Add hosting decisions before serious product launches.

Infrastructure tiers:

1. Laptop / local workstation
2. Cloud credits
3. Shared GPU instances
4. Reserved GPU cluster
5. Colocation racks
6. Owned data center module
7. Multi-region inference platform
8. Frontier training supercluster

Each tier should expose tradeoffs:

- CapEx
- OpEx
- Training throughput
- Inference throughput
- Latency
- Uptime
- Energy load
- Cooling load
- Security
- Vendor lock-in
- Chip supply risk

Hosting contracts:

- Spot GPUs: cheap, unstable, can interrupt training.
- Reserved cloud GPUs: reliable, expensive, quota-limited.
- Enterprise cloud partnership: lower cost, exclusivity constraints.
- Own datacenter: high CapEx, energy and cooling complexity.
- Sovereign cloud: required for regulated/government contracts.

Chips should not be a simple upgrade button. They should be inventory:

- Consumer GPUs
- Used datacenter GPUs
- H100-class accelerators
- B200/next-gen accelerators
- Custom ASIC prototypes
- Inference-optimized accelerators

Chip shortages should affect delivery dates, not just price.

## Data System

Data should be a first-class resource.

Data sources:

- Public web crawl
- Licensed text corpus
- Code repositories
- Enterprise private data
- Synthetic data
- Human preference data
- Multimodal datasets
- Domain expert annotations
- Customer feedback loops

Data dimensions:

- Volume
- Quality
- Freshness
- Legality
- Domain fit
- Contamination
- Bias/safety risk
- Privacy risk

Bad data should quietly poison the model until an evaluation finds it.

Data contracts should create real strategy:

- Licensed data improves compliance but costs cash.
- Scraped data is cheap but creates audit and lawsuit risk.
- Enterprise data improves domain models but cannot always be reused.
- Synthetic data improves scale but can reduce robustness if overused.

## Budget and Runway

Make budget the central constraint.

Add budgets:

- Payroll
- Training compute
- Inference compute
- Data acquisition
- Evaluation
- Safety
- Legal/compliance
- Sales/customer success
- Marketing
- Infrastructure maintenance

A company can fail in multiple ways:

- Cash reaches zero
- Board trust reaches zero
- Cloud provider cuts quota
- Regulatory shutdown
- Repeated benchmark fraud
- Severe data leak
- Talent collapse

Fundraising should not be free money. Each round should add:

- Board pressure
- Growth targets
- Hiring expectations
- Revenue targets
- Strategic constraints
- Equity dilution

## Market and Rival Pressure

Rivals should have strategies, not random posts.

Rival archetypes:

- Frontier lab: high research, high burn, strong benchmarks
- Cloud giant: huge infrastructure, distribution advantage
- Open model collective: low price, fast community adoption
- Enterprise vendor: high sales, compliance, retention
- Hardware-backed lab: cheaper compute, slow product
- Vertical AI startup: narrow high-quality products

Rivals should compete for:

- Talent
- Chips
- Data licenses
- Enterprise contracts
- Arena rank
- Cloud partnerships
- Media narrative

Player actions should create reactions:

- If player dominates a benchmark, rivals copy or price-cut.
- If player has high burn, investors demand revenue.
- If player uses risky data, regulators and journalists investigate.
- If player hires aggressively, poaching wars begin.

## Difficulty Targets

Early game should teach, not punish. Midgame should punish shallow strategy. Late game should punish weak infrastructure.

Target experience:

- First useful model: achievable in 10-20 minutes.
- First profitable model: requires controlling inference cost.
- First 70+ benchmark: requires specialized staff and data.
- First 80+ benchmark: requires dedicated infra, evaluation and research loop.
- First 85+ benchmark: requires frontier-scale organization.
- 90+ benchmark: endgame achievement, not normal progression.

Launches should often be mediocre. That is the point. The fun comes from diagnosing why:

- Data mismatch?
- Too little compute?
- Weak staff?
- Bad eval?
- Infrastructure bottleneck?
- High serving cost?
- Low market fit?

## Multiplayer Future

Design all new systems as if online competition will exist later.

Do not make the client authoritative for future competitive scores. For now, keep local simulation, but model data should be serializable and deterministic enough to migrate.

Future server-authoritative concepts:

- Company profile
- Model submissions
- Arena benchmark results
- Seasonal leaderboards
- Market events
- Anti-cheat validation
- Resource economy

Avoid future multiplayer blockers:

- Do not store final benchmark score as a simple editable number.
- Store inputs and seed used to calculate score.
- Keep model lineage: parent model, dataset, compute, staff, infra, eval history.
- Separate public score from hidden capability values.

## Implementation Roadmap

Phase A - Stabilize Current Prototype

- Keep current scenes and controllers working.
- Add company identity selection.
- Reduce TechPulse noise.
- Preserve save compatibility.
- Add this document as active design direction.

Phase B - Metrics Refactor

- Introduce `ModelCapabilityProfile`.
- Replace single `ModelQuality` as the only success metric.
- Keep legacy `ModelQuality` as a compatibility summary.
- Add benchmark categories and diminishing returns.

Phase C - Staff Refactor

- Introduce employee profiles with skills, salary and burnout.
- Convert existing binary hire flags into generated starter profiles.
- Keep UI labels simple while the model becomes deeper.

Phase D - Infrastructure Refactor

- Replace GPU count-only progression with compute inventory and hosting contracts.
- Add training throughput, inference capacity, latency, uptime and cost per request.
- Make overheating affect uptime and incident risk, not only training duration.

Phase E - Data and Compliance

- Add data assets/contracts.
- Add contamination, licensing and domain-fit.
- Make compliance required for high-value enterprise contracts.

Phase F - AI Arena

- Add benchmark submission flow.
- Add public leaderboard rank and hidden uncertainty.
- Add benchmark-specific rewards and penalties.
- Add periodic benchmark drift as rivals improve.

Phase G - Economy Rebalance

- Revenue based on users, retention, pricing, margin and infrastructure.
- Product launches can lose money.
- Contracts require reliable delivery and compliance.
- Fundraising adds constraints and investor governance.

Phase H - Multiplayer-Ready State

- Store deterministic model lineage.
- Separate simulation state from UI controllers.
- Define future server API contracts without implementing online mode yet.

Phase I - Product Surfaces

- Add product records separate from model records.
- Add website, API, CLI, SDK, builder and web app as unlockable surfaces.
- Track UX quality, maintenance load, support load, conversion and retention.
- Make products depend on models, infrastructure and staff.

Phase J - Design and Developer Experience

- Add Product Designer, Brand Designer, Technical Writer and Developer Advocate roles.
- Add documentation quality, onboarding quality and developer adoption metrics.
- Make CLI/SDK/API adoption depend on docs, stability, examples and community support.
- Add accessibility and design-system workstreams for mature products.

Phase K - Agent Platform

- Add agent runtime, tool connectors, memory, approvals and trace logs.
- Add agent-specific risks: prompt injection, tool misuse, cost runaway and data exfiltration.
- Add enterprise agent contracts with strict security and compliance requirements.

Phase L - Moats and Maintenance

- Add moat tracking for data, compute, distribution, ecosystem, trust and talent.
- Add technical debt per product surface.
- Add maintenance budget and staff allocation.
- Make neglected products degrade over time.

## Immediate Balancing Changes Recommended After This Refactor

- Increase project training cost by 2-4x after the first launch.
- Reduce base client gain by 50-70%.
- Add inference cost to every launched model.
- Require benchmark evaluation before major reputation gains.
- Make licensed data mandatory for some enterprise contracts.
- Make 75+ quality rare without at least three aligned systems: staff, data and compute.
- Make 80+ quality impossible without benchmark/eval infrastructure.

## Agent Guidance

When implementing future gameplay changes:

- Use this document first for progression and difficulty.
- Use `IA-TYCOON-PLAN.md` only for original concept, tone and art direction.
- Preserve existing scenes unless asked to rebuild.
- Prefer small migrations over deleting prototype systems.
- Keep save compatibility whenever possible.
- Add tests for economy, benchmark and progression formulas.
