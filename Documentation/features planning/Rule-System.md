# Feature Planning: Rule System

## Overview

The **Rule System** was sketched as a separate layer from the **Policy Manager**: rules represent *governing structures* (property, economy, governance, security, incentives, social) that can be **activated over time** (e.g., roguelike-style), have **dependencies** on other rules, and **parameters** (e.g., tax rate, enforcement level). Policy, by contrast, defines *the current law in force*—a single active configuration for ownership, stealing, attribution, station use, and visibility.

This document analyzes what a Rule system could be **in relation to** Policy, Events, and the rest of the game, and how it might complement what we have. The existing **Rules** editor menu entries and Database Inspector types are **hidden** so that the Rules system is not surfaced until design and integration are clear; the runtime types (`RuleSO`, `RulesSessionSO`, `RulesManager`) remain in the codebase for reference and future use.

---

## 2. What Was Sketched (Current Codebase)

The following exist but are not exposed in the editor:

- **RuleSO** — ScriptableObject: `ruleName`, `description`, `icon`, `RuleCategory` (Property, Economy, Governance, Security, Incentives, Social, Custom), `displayOrder`, `requiredRules` (dependencies), `parameters` (key/value: float, int, bool, string), and runtime `isActive`. Helpers: `GetFloat(key)`, `GetInt(key)`, etc.
- **RulesSessionSO** — ScriptableObject: `sessionName`, `startingRules`, `availableRulePool` (rules that can be unlocked during play), `maxActiveRules`, `oneTimeUnlock`.
- **RulesManager** — MonoBehaviour (singleton): holds `RulesSessionSO`, maintains `activeRules`, `onRuleActivated` / `onRuleDeactivated` events. Methods: `InitializeFromSession`, `IsRuleActive`, `CanActivateRule` (dependency check), `ActivateRule` / `DeactivateRule`, `TryUnlockFromPool`, `GetUnlockableRules`, `GetActiveRulesByCategory`, `HasRuleInCategory`, `ClearAllRules`.

So the sketch is: **session-based, unlockable “laws” with dependencies and parameters**, not yet wired to Policy, Events, or gameplay.

---

## 3. Policy vs Rules: Distinct Roles

| Aspect | Policy (PolicyManager + PolicyDataSO) | Rules (RulesManager + RuleSO) |
|--------|--------------------------------------|-------------------------------|
| **Role** | “The current law” — one active configuration. | “Which governing structures are in effect” — a set of active rules that can grow (or shrink) over a session. |
| **Cardinality** | One policy at a time (can be swapped). | Many rules can be active; they can be turned on/off or unlocked over time. |
| **Content** | Ownership model, stealing, goal attribution, station use, sharing, visibility (and future: tax triggers/rates). | Categories (property, economy, governance, etc.) and **parameters** (e.g., tax rate, enforcement level) plus dependencies. |
| **Usage** | Systems read PolicyManager for “can I take this?”, “who gets credit?”, “can I use this station?”. | Rules could **drive** policy (e.g., “when Rule X is active, use policy Y”) or **expose parameters** that other systems read (e.g., tax rate from a rule). |
| **Progression** | Policy can be changed by game logic (e.g., vote, level, event). | Rules are designed for **progressive activation** (starting set + unlock pool, dependencies, optional cap). |

So: **Policy** answers “what is the law right now?”; **Rules** can answer “which laws are in effect this run, and what are their parameters?” and can sit **above** or **alongside** Policy.

---

## 4. How a Rule System Could Complement the Current Systems

### 4.1 Rules as a Session / Run Configuration

- A **RulesSessionSO** defines “this run’s starting rules” and “pool of unlockable rules.” RulesManager initializes from it and tracks active rules.
- Other systems (menu, level loader, game mode) can assign a session per run; no direct coupling to Policy yet.

### 4.2 Rules Driving or Overriding Policy

- **Option A — Rules select Policy:** Each rule (or rule category) could reference a `PolicyDataSO` or a policy “preset”; when the rule is active, PolicyManager could apply that policy or merge it with a base policy. So “Rule: Private Property” might imply a policy with `ownershipModel = PrivateIndividual`, etc.
- **Option B — Rules as policy parameters:** Rule parameters (e.g., `taxRate`, `stealingPenalty`) could be read by PolicyManager or by a future “effective policy” layer that combines base policy + rule-derived overrides. Policy stays the single source of truth for *what* is configurable; rules supply *values* for some of those options.
- **Option C — Rules and Policy independent:** Rules affect other systems (e.g., rewards, difficulty, events); Policy remains the only place for ownership/stealing/attribution. Clear separation but no reuse of rule parameters for “law” behavior.

### 4.3 Rules and Events

- **Events** already have effects (change policy, multiply speed, scale recipe/decay, etc.). A Rule could be a **persistent** “law” that stays until deactivated, while an Event is **time-bound** or one-off. So:
  - Rules = “structural” (this run we have private property, taxes on delivery).
  - Events = “momentary or timed” (this round double output, next 30 seconds slow motion).
- Rules could **trigger or be triggered by** events (e.g., “Event: Revolution” deactivates a rule and activates another; or “when Rule X is unlocked, fire Event Y for UI”).

### 4.4 Rules and Goals / Progression

- Unlock conditions for rules could depend on goals, score, or resources (e.g., "unlock Rule: Taxation when score > 100"). RulesManager already has `GetUnlockableRules()` and dependency checks; the missing piece is **who** calls `TryUnlockFromPool` and with what criteria (goal completion, threshold, etc.).

### 4.5 Parameters and Who Reads Them

- **RuleParameter** (key + float/int/bool/string) is generic. For a Rule system to complement Policy/Events:
  - Either **conventions** are defined (e.g., key `"taxRate"` is read by a tax system; key `"policyOverride"` references a PolicyDataSO).
  - Or **RuleSO** (or a bridge) maps rule type/category to a known set of parameters that PolicyManager, EventManager, or scoring code know how to read.
- Without integration, rules remain data-only; with integration, they become a **governance layer** that feeds Policy, Events, or other system parameters.

---

## 5. Possible Directions (To Be Decided)

1. **Rules as “policy presets” over time** — Each rule (or category) maps to a policy fragment or full PolicyDataSO; RulesManager + PolicyManager together resolve “effective policy” from active rules (e.g., merge or override). Unlock order and dependencies shape which “laws” are in effect.
2. **Rules as parameter bags** — Rules only hold named parameters; PolicyManager (or a new “EffectivePolicy” layer) reads those parameters when resolving tax rate, penalties, etc. PolicyDataSO stays the schema; rules supply values per run.
3. **Rules as pure progression / narrative** — Rules are unlocked for UI or narrative (e.g., “you adopted Democracy”); they don’t change Policy or parameters. Events and Policy remain the only mechanical levers.
4. **Hybrid** — Some rules drive policy or parameters; others are cosmetic or narrative. Rule category or a “rule type” field could distinguish.

Until we pick a direction, the **Rules system stays hidden in the editor** and is not part of the default workflow. The existing **RuleSO**, **RulesSessionSO**, and **RulesManager** can be extended or refactored when we define how Rules complement Policy and Events.

---

## 6. To-Do / Next Steps

- [ ] **Decide relationship to Policy** — Do rules select/override policy, supply parameters, or stay independent?
- [ ] **Decide relationship to Events** — Are rules “persistent events” or a separate axis? Can events activate/deactivate rules?
- [ ] **Define parameter conventions** — Which keys (e.g., tax rate, enforcement) are standard, and which systems read them?
- [ ] **Unlock integration** — When does `TryUnlockFromPool` get called? Goals? Score? Resources? Level?
- [ ] **Re-enable editor surface** — Once the design is fixed: restore **Game Assemblies > Rules** menu items and Database Inspector entries for Rules / Rules Sessions, and document the workflow.

---

## 7. Summary

- **Policy** = current law (one active PolicyDataSO); **Rules** = optional set of governing structures with dependencies and parameters, activatable over a session.
- The codebase already has **RuleSO**, **RulesSessionSO**, and **RulesManager**; the **Rules editor menu and Database Inspector entries are hidden** so the system is not visible until we decide how Rules complement Policy and Events.
- A Rule system can complement by: driving or parameterizing Policy, coexisting with Events (structural vs timed effects), and tying unlocks to goals/score/resources. This doc is the placeholder for that design.
