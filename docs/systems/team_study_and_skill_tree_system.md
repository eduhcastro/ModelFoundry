# Team, Study and Skill Tree System

Status: first gameplay system for the new realistic progression direction.

Primary reference: `docs/realistic_ai_company_progression_plan.md`.

This system is the new starting point for Model Foundry progression. Older prototype systems can remain as compatibility data, but new gameplay should route through staff profiles, assigned roles, study tracks, skill points and project allocation.

## Design Intent

The first goal is no longer a direct button sequence. "Study Papers, Improve the Website, Build a Prototype" should become a set of decisions performed by people.

The player starts with a founder. The founder has skills, work history, education, role focus, study history and skill points. Later employees use the same structure.

Every meaningful action should ask:

- Who is doing the work?
- What role are they currently assigned to?
- What skill does the work require?
- How long does it take?
- What does it improve?
- What opportunity cost does it create?

## Research Notes

Skill and progression systems work best when they create durable choices instead of pure linear upgrades. Diminishing returns prevent one dominant strategy and make specialization meaningful. For Model Foundry, this matches the realistic plan: a small startup can learn fast at the beginning, but late gains require specialized staff, better infrastructure, data and process.

References used for this first pass:

- GDKeys, "Keys to Meaningful Skill Trees": https://gdkeys.com/keys-to-meaningful-skill-trees/
- GameDev.net discussion on level progression and diminishing returns: https://gamedev.net/forums/topic/505262-level-progression/

Practical rules:

- Early study should feel useful.
- Repeating the same study should become less efficient.
- Past a threshold, broad generalist study should give small gains.
- Specialization should unlock stronger work, but narrow the person's best role.
- Skill points should get more expensive as the person advances.
- A person assigned to study should not also be fully productive on product work.

## Employee Profile

Each staff profile should track:

- Display name
- Role title
- Level
- Salary
- Burnout
- Education
- Work history
- Current assignment
- Skill points
- Skill point cost
- Study history
- Attributes

Initial attributes:

- Research
- Engineering
- Product
- Design
- Infrastructure
- Safety
- Communication
- Focus
- Leadership

## Assignment Flow

Assignments define what the person is currently best used for:

- Research Study
- Website
- Prototype
- Infrastructure
- AI Arena
- Management
- Rest

Changing assignment should be cheap early, but specialized staff should have stronger bonuses in their specialty and weaker output outside it.

## Study Flow

The player opens Team, selects a person, then chooses Study.

Each study option shows:

- Duration in days
- Cash cost
- Required role or recommended role
- Affected attributes
- Expected gain
- Diminishing return warning

Study options for the first implementation:

- AI Paper Reading: Research, Focus
- Systems Design Notes: Engineering, Infrastructure
- Product UX Review: Product, Design, Communication
- Safety Case Studies: Safety, Research
- Founder Operating Cadence: Leadership, Focus, Communication

Formula:

```text
effective_gain = base_gain * (1 - current_attribute / 100)^2 * repetition_penalty
repetition_penalty = 1 / (1 + completed_studies_in_same_track * 0.35)
```

This gives meaningful early growth and slow late growth.

## Skill Tree

Skill points are earned from completed study and later from project milestones.

Point cost:

```text
next_point_cost = 100 + earned_points * 75
```

First perks:

- Lean Reader: Study costs 10% less.
- Deep Work: Study produces 15% more Focus and Research.
- Practical Builder: Prototype work gains Engineering output.
- Clear Communicator: Website and documentation work gain Product output.
- Cost Discipline: Salary and study costs are slightly easier to manage.

Perks should not be free stat inflation. They should alter role fit, cost, time or risk.

## UI Rules

Use a light translucent modal style for management windows:

- White or near-white panel
- Subtle transparency
- Dark readable text
- Small icon buttons
- No full-height side menu
- Goal checklist on the left
- Modal appears only when the player opens a workflow

Right-click context menus are removed from this new flow.

## First Implementation Scope

Implement now:

- Founder profile
- Team button
- Team modal
- Employee profile view
- Role assignment buttons
- Study option list
- Active study progress by day
- Diminishing returns
- Skill points and first perk unlocks
- Goal checklist on the left
- Remove right-click legacy context menu

Not implemented yet:

- Hiring market generation
- 3D head render thumbnails
- Multiple employees from real hiring pipeline
- Full project allocation calendar
- Save/load migration for team profiles
- Online-ready deterministic server model

## Integration With Realistic Progression

This system maps to Phase C - Staff Refactor in the realistic plan and becomes the foundation for:

- Research loop
- Product surfaces
- Infrastructure specialization
- AI Arena readiness
- Hiring and poaching
- Burnout and retention
- Multiplayer-ready company state
