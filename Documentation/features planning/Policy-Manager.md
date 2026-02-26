# Feature Planning: Policy Manager

## Overview

The **Policy Manager** is the system that represents “the current law” in the simulation. It determines what is possible for players and how the economy and ownership systems behave (e.g., who can own what, how score is attributed, whether stealing is allowed, how tax is applied). It is intended to integrate with the [Resource Ownership](Resource-Ownership.md) system so that ownership rules and consequences are configurable rather than hard-coded.

As the game progresses, **players may be able to change these laws** (e.g., through votes, purchases, or level progression), making the Policy Manager the single place to read and update the active rules.

This document defines the core policies, what has been **implemented** so far (base implementation, no taxation), and a **to-do list** of next steps.

---

## 1. Implemented: Base Policy Manager (No Taxation)

### 1.1 Components

- **PolicyManager** (MonoBehaviour, singleton) — Add to scene via the **Create Policy Manager** editor tool (see §5); assign a `PolicyDataSO` (or one is created and assigned by the tool). If `policy` is null, defaults are used (e.g. PrivateIndividual, Disallowed stealing, ResourceOwner attribution, Anyone station use). Exposes: `GetOwnershipModel()`, `CanTakeResource(actor, currentOwner)`, `IsTakingStealing()`, `GetGoalAttribution()`, `GetGoalAttributionOwnerShare()`, `CanUseStation(actor, stationOwner)`, `IsSharingAllowed()`, `GetResourceVisibility()`, `SetPolicy(PolicyDataSO)`.
- **PolicyDataSO** (ScriptableObject) — Created via **Game Assemblies > Systems > Create Policy Manager** (see §5). Holds: ownership model, stealing policy, goal attribution (and owner share for Split), station use policy, sharing allowed, resource visibility. **Taxation is not included** in this base implementation.

### 1.2 When No Policy Manager Is Present

If there is **no PolicyManager** in the scene, the game behaves as **communal**: all resources are common (no owner set on production or grab), no policy checks on grab/absorb, anyone can use any station, and **ResourceManager** counts all resources when `GetResourceCount(type, null)` is used. All policy checks are guarded by `PolicyManager.Instance != null`.

### 1.3 Ownership and Station Use Integration

- **Production:** Station sets resource owner only when Policy Manager exists, ownership model ≠ Communal, and station has an owner.
- **Grab / absorb:** playerController and grabRegion use `CanTakeResource(actor, resourceObj.owner)` to allow or block taking; when allowed and model is private, ownership is transferred to the acting player.
- **Station use:** Station calls `CanUseStation(worker, owner)` before worker-based consume/produce (ConsumeOnWork, ProduceOnWork, ConsumedProduction, ConsumeOnCycle); when not allowed, the action is skipped.
- **Ownership-aware counting:** ResourceManager `GetResourceCount(Resource, playerController ownerOrNull)` and `GetAllResourceCounts(ownerOrNull)` use policy when owner is null (Communal or no manager → count all; otherwise → count unowned only). See [Resource-Ownership.md](Resource-Ownership.md).

Goal attribution (ResourceOwner, Deliverer, StationOwner, Split) is **defined** in policy and exposed on PolicyManager but is **not yet used** in GoalManager when distributing rewards (see to-do below).

---

## 2. Core Policies to Implement (Reference)

### 2.1 Ownership Model

Defines how property is understood in the simulation.

| Value / mode | Description | Effect on simulation |
|--------------|-------------|------------------------|
| **Communal** | No private ownership; all resources and stations are shared. | Resource counts and score are global; no per-player attribution. Stealing is N/A. |
| **Private (individual)** | Each player (or team) owns their own resources and optionally stations. | Counting and scoring can be per-player; only “your” resources count for your goals/score when configured. |
| **Private (team)** | Ownership is by team; resources/stations belong to a team. | Same as private but keyed by team; score and goals can be team-based. |
| **Hybrid** | Some resources or zones are communal (e.g., “common pool”), others private. | Policy can define per-resource-type or per-zone rules (e.g., “gold is private, wood is common”). |

**Implementation note:** The Policy Manager should expose the current ownership model (enum or scriptable config) so that ResourceManager, goals, and stations query it before counting, attributing score, or allowing transfers.

---

### 2.2 Taxation Model

Defines **when** and **on what** tax is applied.

| Value / mode | Description | Typical use |
|--------------|-------------|-------------|
| **None** | No tax. | Default; no deductions. |
| **On delivery** | Tax applied when resources are delivered (e.g., to a goal or deposit). | “The house takes a cut when you score.” |
| **On transfer** | Tax applied when ownership of a resource or capital changes hands. | Simulates trade or theft penalties. |
| **On production** | Tax applied when a station produces capital or resources. | “Production tax” or “value-added.” |
| **On income (score)** | Tax applied when score/capital is awarded (e.g., goal reward). | Flat tax on “income.” |
| **Composite** | Multiple of the above can be active (e.g., on delivery + on transfer). | Policy holds a set of triggers; each can have its own rate or share a global rate. |

**Implementation note:** The Policy Manager stores which triggers are enabled; the Resource Ownership and scoring systems invoke “ApplyTax(amount, trigger)” and use the returned net amount (or record the tax separately for UI/feedback).

---

### 2.3 Taxation Rate

The **magnitude** of tax (e.g., a fraction of the value or a flat amount).

- **Format:** Could be a float in `[0, 1]` (e.g., `0.1` = 10%) or a fixed amount per transaction. Supporting both (e.g., “10% or 1 point, whichever is greater”) allows flexible design.
- **Scope:** Can be global (one rate for all tax triggers) or per-trigger (e.g., 5% on delivery, 15% on transfer). Recommendation: start with one global rate, then add per-trigger overrides.
- **Recipient:** Who receives the collected tax (e.g., global pool, “the house,” a specific team, or redistributed to all players). This can be a separate policy (see **Tax recipient** below).

**Implementation note:** Policy Manager exposes `GetTaxRate(trigger)` and optionally `GetTaxRecipient()`. Scoring and transfer code apply tax and then credit the recipient.

---

### Suggested additional policies

These policies extend the “current law” and support richer simulation and player-driven rule changes.

### 2.4 Stealing / Taking

- **Allowed / Disallowed / Penalized**  
  - Whether a player can take a resource owned by another (or by a team). If “Penalized,” taking is allowed but triggers a cost or a tax (e.g., “theft tax” or loss of score).
- **Linking to Resource Ownership:** When a player attempts to pick up or absorb a resource, the grab/area logic asks the Policy Manager “Can actor X take resource owned by Y?” and “What happens if they do?” (ownership transfer, penalty, etc.).

### 2.5 Tax Recipient

- **Where tax goes:** Global pool, house (removed from play), specific team, or split among all players. Enables “redistributive” or “competitive” economies and supports level design (e.g., “tax goes to team 2 this round”).

### 2.6 Goal Attribution

- **Who gets credit** when a goal is completed: the owner of the resource delivered, the player who delivered it, the station owner, or a split (e.g., 70% owner, 30% deliverer). Policy Manager can define this per goal type or globally, so the same goal logic works under different “laws.”

### 2.7 Resource Visibility (optional)

- **Public / Private**  
  - Whether other players can see how many resources or how much capital a player (or team) has. Supports bluffing, negotiation, or full-information modes.

### 2.8 Sharing / Gifting

- **Allowed / Disallowed**  
  - Whether players can voluntarily transfer resources or capital to another (gift, trade). Complements “stealing” by allowing cooperative transfers when policy permits.

### 2.9 Starting Allocation

- **Initial resources or capital per player/team**  
  - Policy can define starting amounts or “handicaps” so level or game mode can change starting conditions without hard-coding.

### 2.10 Penalties and Rewards

- **Failure penalty** (e.g., goal failed: lose points, lose resources, or no effect).  
- **Bonus rules** (e.g., “first to N goals gets a multiplier”).  
  - Stored as policy so “law changes” can tweak risk/reward (e.g., “this round, failures are punished more”).

### 2.11 Station Ownership Rules

- **Who can use a station:** Only owner, only same team, anyone, or “anyone but pay a fee.”  
  - Enables “private workshop” vs. “public station” without duplicating station logic; Policy Manager answers “Can actor X use station owned by Y?”

### 2.12 Time- or Phase-Based Policies

- **Policy schedules** (e.g., “first 2 minutes: no tax; after that: 10% on delivery”).  
  - Supports narrative or difficulty curves (e.g., “laws change at halftime”).

---

## 3. To-Do: Next Steps

- [ ] **Taxation** — Add taxation model, rate(s), and recipient to PolicyDataSO and PolicyManager (e.g. On delivery, On transfer, On production, On income). Have scoring and transfer code call into PolicyManager to apply tax and credit recipient.
- [x] **Enforce station use policy** — Done. Station calls `CanUseStation(worker, owner)` before worker-based consume/produce; no Policy Manager = anyone can use.
- [ ] **Use goal attribution in GoalManager** — When a goal is completed, determine which owner(s) get the reward using `GetGoalAttribution()` and `GetGoalAttributionOwnerShare()` (ResourceOwner, Deliverer, StationOwner, Split) and credit per-player or per-team score accordingly.
- [ ] **Stealing penalty (Penalized)** — When stealing policy is Penalized, apply a cost or penalty on take (e.g. deduct score, or hook for future “theft tax”).
- [ ] **Resource visibility** — Use `GetResourceVisibility()` in UI so other players’ resources/capital are hidden when policy is Private.
- [ ] **Sharing / gifting** — If voluntary transfer of resources or capital is added, gate it with `IsSharingAllowed()`.
- [ ] **Policy events** — Optionally raise events when policy changes so UI and gameplay can refresh (e.g. labels, permissions).
- [ ] **Time- or phase-based policies** — Support policy schedules (e.g. different rules after a timer or phase change).

---

## 4. Implementation Outline (Reference)

- **Single source of truth:** One Policy Manager (or scriptable asset) holds the current set of policies. All systems that need “the law” (ResourceManager, goals, stations, areas, UI) read from it.
- **Scriptable or runtime:** Policies can be defined in ScriptableObjects per level or game mode, and optionally modified at runtime when “players change the laws.”
- **Events:** When a policy changes (e.g., tax rate or ownership model), the Policy Manager can raise events so UI and gameplay systems refresh (e.g., update labels, re-check permissions).
- **Defaults:** Ship with a default policy (e.g., Private ownership, No tax, Stealing disallowed) so the game runs without extra setup; advanced modes override.

---

## 5. Summary Table

| Policy | Purpose | Example values |
|--------|---------|----------------|
| **Ownership model** | Who can own what; communal vs. private | Communal, Private (individual), Private (team), Hybrid |
| **Taxation model** | When tax is applied | None, On delivery, On transfer, On production, On income, Composite |
| **Taxation rate** | How much is taken | 0–1 (percentage) or fixed amount; global or per-trigger |
| **Tax recipient** | Where tax goes | Global pool, House, Team, Redistribute |
| **Stealing / taking** | Can players take others’ resources? | Allowed, Disallowed, Penalized |
| **Goal attribution** | Who gets credit for a completed goal | Owner, Deliverer, Station owner, Split |
| **Sharing / gifting** | Voluntary transfers allowed? | Allowed, Disallowed |
| **Resource visibility** | Can others see your resources/capital? | Public, Private |
| **Station use** | Who can use a station | Owner only, Team, Anyone, Anyone (with fee) |
| **Starting allocation** | Initial resources/capital | Per-player or per-team amounts |
| **Penalties / rewards** | Failure penalty, bonus rules | Configurable penalties and multipliers |
| **Policy schedule** | Change policies over time | Time- or phase-based overrides |

Implementing the **core three** (ownership model, taxation model, taxation rate) plus **tax recipient** and **stealing** will give a strong “current law” layer that the Resource Ownership system can depend on; the rest can be added incrementally as needed.

---

## 6. Editor tools

- **Game Assemblies > Systems > Create Policy Manager** — Single window that:
  1. **Exposes the Policy Manager prefab template** — Object field (default: package template `Samples/Prefabs/Managers/PolicyManager.prefab`). The tool copies this prefab to **`Game Assemblies/Prefabs/Managers/PolicyManager.prefab`** if that copy does not exist, then instantiates from the copy (not from the package).
  2. **Policy data form** — Policy name and all policy fields (ownership model, stealing policy, goal attribution, owner share, station use policy, sharing allowed, resource visibility). On **Create Policy Manager**, a new **PolicyDataSO** is created and saved to **`Game Assemblies/Databases/Policies/{policyName}.asset`**.
  3. **Scene connection** — If a Policy Manager already exists in the scene, the new policy asset is assigned to it (replacing the previous one). If none exists, the template copy is instantiated in the scene and the new policy is assigned. Only one Policy Manager per scene.
