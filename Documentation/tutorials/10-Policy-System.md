# Tutorial 10: Policy System

This tutorial explains the **Policy Manager** and **Policy Data** in Game Assemblies: how to create a policy asset, add a Policy Manager to the scene, and how policies control ownership, stealing, station use, and related behavior. When no Policy Manager is present, the game runs in a **communal** default (everyone shares everything).

## Prerequisites

- Completed [Tutorial 02](02-Stations-and-Resources.md) (Stations and Resources)
- Completed [Tutorial 03](03-Resource-Manager-and-Goals.md) (Resource Manager and Goals)
- Optional: [Tutorial 11](11-Ownership-Model.md) (Ownership Model) for how ownership and policies interact

## Overview

In this tutorial, you'll learn:

1. **What is the Policy Manager?** — The single source of "current law" for ownership and economy
2. **Creating Policy Data and the Policy Manager** — One editor window: create a policy asset and add or update the Policy Manager in the scene
3. **Policy options** — Ownership model, stealing, goal attribution, station use, sharing, visibility
4. **When no Policy Manager is present** — Communal default behavior
5. **Editing policies** — Database Inspector and changing policy at runtime

For full policy reference and to-dos (e.g. taxation), see [Policy-Manager.md](../features%20planning/Policy-Manager.md).

---

## Part 1: What is the Policy Manager?

The **Policy Manager** is a singleton component that holds the **current set of rules** for your game: who can own resources, whether players can take each other’s resources (stealing), who gets credit when a goal is completed, who can use which stations, and whether sharing or resource visibility is allowed. These rules are stored in a **PolicyDataSO** asset; the **PolicyManager** in the scene references one such asset.

- **PolicyManager** (MonoBehaviour) — One per scene. Exposes methods like `GetOwnershipModel()`, `CanTakeResource(actor, owner)`, `CanUseStation(actor, stationOwner)`, `GetGoalAttribution()`, etc. Other systems (Station, playerController, grabRegion, ResourceManager) call these when they need to enforce or query the rules.
- **PolicyDataSO** (ScriptableObject) — The actual policy: ownership model, stealing policy, goal attribution, station use policy, sharing allowed, resource visibility. No Policy Manager in the scene = **communal** behavior (no ownership checks, anyone can grab and use anything).

---

## Part 2: Creating Policy Data and the Policy Manager

One editor window creates both the policy asset and the scene object.

1. **Open the Create Policy Manager window**  
   In Unity’s top menu:
   ```
   Game Assemblies → Systems → Create Policy Manager
   ```

2. **Prefab template**  
   The window shows a **Policy Manager Prefab (template)** field, usually pre-filled with the package template. A copy is saved to `Game Assemblies/Prefabs/Managers/PolicyManager.prefab` if it doesn’t exist; the scene instance is created from that copy.

3. **Set policy name and options**
   - **Policy Name** — Name for the new policy asset (e.g. `"Private Ownership"`).
   - **Ownership Model** — Communal, Private Individual, Private Team, or Hybrid.
   - **Stealing Policy** — Allowed, Disallowed, or Penalized.
   - **Goal Attribution** — Resource Owner, Deliverer, Station Owner, or Split (with **Owner Share** when Split).
   - **Station Use Policy** — Owner Only, Same Team, Anyone, or Anyone With Fee.
   - **Sharing Allowed** — Whether voluntary transfers are allowed.
   - **Resource Visibility** — Public or Private.

4. **Click Create Policy Manager**
   - A new **PolicyDataSO** is created and saved at:
     ```
     Assets/Game Assemblies/Databases/Policies/{PolicyName}.asset
     ```
   - If a **Policy Manager** already exists in the scene, the new policy is **assigned** to it (replacing the previous one).
   - If no Policy Manager exists, one is **instantiated** from the prefab copy and the new policy is assigned. Only one Policy Manager per scene.

---

## Part 3: Policy Options (Quick Reference)

| Option | Effect |
|--------|--------|
| **Ownership Model** | Communal = no ownership; Private Individual/Team = resources and stations can be owned; Hybrid = mixed (future). |
| **Stealing Policy** | Whether a player can take a resource owned by another (Allowed / Disallowed / Penalized). |
| **Goal Attribution** | Who gets credit when a goal is completed: resource owner, deliverer, station owner, or split. (Currently reward still goes to contributor; full attribution is planned.) |
| **Station Use Policy** | Who can work/use a station: Owner Only, Same Team, Anyone, or Anyone With Fee. |
| **Sharing Allowed** | Whether voluntary transfer of resources/capital is allowed. |
| **Resource Visibility** | Whether other players can see a player’s resources/capital (Public / Private). |

When **no Policy Manager** is in the scene, the game behaves as **communal**: no ownership is set, anyone can grab and use anything, and ResourceManager counts all resources.

---

## Part 4: When No Policy Manager Is Present

If there is **no PolicyManager** in the scene:

- All resources are treated as **common** (no owner set on production or grab).
- No policy checks on grab or absorb; anyone can take any resource.
- Anyone can use any station.
- **ResourceManager** counts all resources when `GetResourceCount(type, null)` is used.

All code that uses the Policy Manager checks `PolicyManager.Instance != null` first, so the game runs normally without a Policy Manager.

---

## Part 5: Editing Policies

- **Database Inspector** — Open **Game Assemblies → Databases → Database Inspector**, choose **Policies**, and select a policy asset to view and edit its fields.
- **Runtime** — You can change the active policy in code with `PolicyManager.Instance.SetPolicy(newPolicyDataSO);` so "laws" can change during play (e.g. after a vote or level transition).

---

## Summary

- **Policy Manager** = single source of "current law"; **PolicyDataSO** = the asset holding the rules.
- Use **Game Assemblies → Systems → Create Policy Manager** to create a policy and add or update the Policy Manager in the scene.
- Policies control ownership model, stealing, goal attribution, station use, sharing, and visibility.
- No Policy Manager = communal default (everyone shares; no ownership checks).
- Edit policies in the Database Inspector or assign a different PolicyDataSO at runtime with `SetPolicy()`.
