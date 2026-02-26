# Feature Planning: Resource Ownership

## Overview

The **Resource Ownership** model is a flexible system that allows the simulation to understand ownership and private property for players or teams. It enables:

- **Ownership-aware scoring** — Count or award score based on who owns the resources that are being scored (e.g., only your delivered items count toward your score).
- **Simulated actions** — Support mechanics such as stealing (taking another's resources), shared vs. private resources, and rules that depend on "who owns what."
- **Policy-driven behavior** — Ownership rules and consequences are driven by a **Policy Manager** (see [Policy-Manager.md](./Policy-Manager.md)), so the same systems can represent different "laws" (e.g., communal vs. private property, taxation).

This document reviews the **current state** of the resource ownership implementation, what is **working** in the project today, what is **missing**, and a **to-do list** of next steps.

---

## 1. What Is Working (Current Project)

| Component | Status |
|-----------|--------|
| **ResourceObject** | Has `owner` (`playerController`), `hasOwner` (bool), and `setOwner(playerController)`. Owner can be set when resources are produced or transferred. |
| **Station** | Has `owner` and `worker`. When producing resources, **sets the produced resource's owner only when Policy Manager exists, model ≠ Communal, and station has an owner**; otherwise communal. **Station use:** before worker-based consume/produce, checks `CanUseStation(worker, owner)` when Policy Manager present; no manager = anyone can use. |
| **Station capital** | `ProduceCapital(playerController)` and `ConsumeCapital(playerController)` update the given player's `capital` and route score through **TeamManager** (per-player/per-team or global) or `ResourceManager.globalCapital`. |
| **playerController** | Has `capital` (int) and `properties` (list of GameObjects). Can be associated with a team via **TeamManager** (team ID, per-player/team score). |
| **TeamManager** | **Implemented.** Supports EveryoneOneTeam, Teams, and Solo modes. Tracks per-player and per-team scores; goal rewards/penalties are attributed to a **contributor** (player who triggered the goal completion) via `AddScore(amount, contributor)`. Resource Manager Canvas can show up to 4 score items based on TeamManager. |
| **PolicyManager** | **Implemented and wired.** Full API; **Station**, **playerController**, and **grabRegion** use it: production owner, grab/absorb (CanTakeResource + ownership transfer), grabbable filtering, and CanUseStation before worker actions. No Policy Manager = communal default (see §2.1). |
| **PolicyDataSO** | ScriptableObject for ownership model, stealing policy, station use policy, goal attribution, sharing, visibility. |
| **ResourceManager** | Tracks `allResources`. **Ownership-aware counting:** `GetResourceCount(Resource, playerController ownerOrNull)` — when owner is non-null, counts only that owner's resources; when null, behavior depends on policy: if Policy Manager exists and model ≠ Communal, counts **unowned (common) only**, otherwise counts **all**. `GetResourceCount(Resource)` and `GetResourceCount2(Resource)` call the overload with null. `GetAllResourceCounts(ownerOrNull)` supports the same filter. |
| **Goals & scoring** | `ResourceGoalSO` / `GoalManager` use `ResourceManager.GetResourceCount(resourceType)` (owner = null), so goal progress uses policy-based counting (all vs unowned only). When a goal completes, **reward goes to the contributor**; TeamManager assigns score. **Policy-based attribution** (ResourceOwner, Deliverer, StationOwner, Split) is not used yet. |

---

## 2. What Is Missing (Gaps)

### 2.1 Policy integration — implemented

When **no Policy Manager** is in the scene, the default is **communal**: resources have no owner set on production, anyone can grab/absorb them, anyone can use any station, and score is global (or per-player/per-team via TeamManager as configured).

When a **Policy Manager** is present:

- **Station production** — Produced resource's owner is set only when ownership model ≠ Communal and station has an owner; otherwise resource stays unowned (communal).
- **Grab (playerController)** — Before grab, `CanTakeResource(this, resourceObj.owner)` is checked; if not allowed, grab is aborted. On successful grab, if model is not Communal, resource owner is set to the grabbing player.
- **Absorb (sortObsorbedObject)** — Same: absorb is allowed only if `CanTakeResource` allows; on add, ownership is set to this player when model is not Communal.
- **grabRegion** — A Grabbable with `ResourceObject` is added to the grabbable list only when there is no Policy Manager or `CanTakeResource(pController, resourceObj.owner)` is true.
- **Station use** — Before worker-based consume/produce (ConsumeOnWork, ProduceOnWork, ConsumedProduction, ConsumeOnCycle), Station checks `CanUseStation(worker, owner)`; if not allowed, the action is skipped.

### 2.2 Ownership-aware counting — implemented

- **ResourceManager** exposes `GetResourceCount(Resource resourceType, playerController ownerOrNull)`. When `ownerOrNull` is non-null, only that owner's resources of that type are counted. When null: if Policy Manager exists and ownership model is not Communal, only **unowned** (common) resources are counted; otherwise **all** resources of that type are counted. `GetAllResourceCounts(ownerOrNull)` uses the same rule. Goals and UI that call `GetResourceCount(resourceType)` (no owner) therefore get policy-based behavior (all vs unowned only).

### 2.3 Remaining gaps (goals and attribution)

- **Goal progress by owner** — No option yet to count only resources owned by a **given** player/team toward a goal (e.g. "player 2 must deliver 5 apples"); current progress uses the null-owner count (all or unowned by policy).
- **Goal attribution from policy** — Reward is always attributed to the **contributor** (deliverer/worker). Policy Manager's `GetGoalAttribution()` (ResourceOwner, Deliverer, StationOwner, Split) and `GetGoalAttributionOwnerShare()` are not used to decide who gets the points.

### 2.4 Other gaps

- **Areas** — Input/output/absorb areas do not check ownership when consuming or absorbing; no attribution to resource owner or depositing player per policy.
- **Team as ownership identity** — TeamManager has team IDs and per-team score, but there is no first-class "team" as resource owner (ownership is only `playerController`); team-based counting/attribution is not implemented.
- **`hasOwner` semantics** — Not wired into counting or policy; "common" vs. owned could be clarified and used consistently.
- **Events** — No `ResourceOwnershipChanged` or similar for UI/achievements/taxation.

---

## 3. To-Do: Next Steps

- [x] **Wire Policy Manager into grab/absorb/production** — Done. No Policy Manager = communal (no owner set, anyone can grab/use). With Policy Manager: production sets owner only when model ≠ Communal; grab/absorb check `CanTakeResource` and set owner on take when private; grabRegion filters grabbables by `CanTakeResource`.
- [x] **Enforce station use policy** — Done. Station checks `CanUseStation(worker, owner)` before worker-based consume/produce; no Policy Manager = anyone can use.
- [x] **Ownership-aware resource counting** — Done. `GetResourceCount(Resource, playerController ownerOrNull)` and `GetAllResourceCounts(ownerOrNull)`; when owner is null, policy decides (Communal or no Policy Manager → count all; otherwise → count unowned only).
- [ ] **Goal progress by owner** — In ResourceGoalSO / GoalManager, add an option to count only resources owned by a given player/team toward a goal, and wire goal completion to that rule.
- [ ] **Policy-based goal attribution** — TeamManager already provides per-player/per-team score and attributes rewards to the contributor. Wire Policy Manager's goal attribution (ResourceOwner, Deliverer, StationOwner, Split) and `GetGoalAttributionOwnerShare()` so reward distribution follows the active policy (e.g. credit to resource owner vs. deliverer vs. station owner).
- [ ] **Areas: ownership checks and attribution** — When an Area consumes or absorbs a resource, optionally check ownership (e.g. only owner can deposit) and attribute goal progress or score to the resource's owner (or depositing player) per policy.
- [ ] **Ownership identity / teams** — Introduce an ownership identity type (e.g. "no owner," player, team) and optional Team entity so goals and score can be team-based; extend counting and attribution to "by team."
- [ ] **Clarify "common" and `hasOwner`** — Define and use `ResourceObject.hasOwner` (or an explicit common vs. owned flag) consistently so counting and policy can treat "common" resources explicitly.
- [ ] **Events (optional)** — Consider events such as `ResourceOwnershipChanged` for UI, achievements, or future taxation on transfer.

---

## 4. Success Criteria (Summary)

- **Counting** — Goals and UI can count resources by owner/team and distinguish "common" vs. owned.
- **Scoring** — Score can be attributed per-player or per-team based on who owns the resources that contributed (with policy deciding exact rules).
- **Stealing** — Taking another's resource is possible when policy allows, with ownership transfer and optional events.
- **Policy-driven** — Ownership behavior is configurable via the Policy Manager (ownership model, taxation, and other policies).
- **Extensible** — Adding teams or new ownership types does not require rewriting core resource/station logic.

Once these are in place, the Resource Ownership feature will be in a complete enough state to support "current law" and player-driven policy changes (e.g., changing laws as the game progresses) as described in the Policy Manager document.

---

## 5. Editor tools

- **Game Assemblies > Systems > Create Policy Manager** — Single editor window to create a Policy Data asset (name and all policy fields), save it under `Game Assemblies/Databases/Policies`, and either assign it to an existing Policy Manager in the scene or instantiate a Policy Manager from a template prefab (copy saved to `Game Assemblies/Prefabs/Managers/PolicyManager.prefab`). The prefab template is exposed in the window; only one Policy Manager per scene.
