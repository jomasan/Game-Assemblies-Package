# Tutorial 11: Ownership Model

This tutorial explains the **Resource Ownership** model in Game Assemblies: how resources and stations can have owners, how the **Policy Manager** controls who can take resources and use stations, and how **ownership-aware counting** works for goals and the UI. When no Policy Manager is present, everything behaves as **communal** (no ownership).

## Prerequisites

- Completed [Tutorial 02](02-Stations-and-Resources.md) (Stations and Resources)
- Completed [Tutorial 03](03-Resource-Manager-and-Goals.md) (Resource Manager and Goals)
- Completed [Tutorial 10](10-Policy-System.md) (Policy System) — policies define ownership and stealing rules

## Overview

In this tutorial, you'll learn:

1. **What is the ownership model?** — Owners on resources and stations; communal vs private
2. **Default: communal** — Behavior when there is no Policy Manager
3. **With Policy Manager** — How production, grab, absorb, and station use respect policy
4. **Ownership-aware counting** — How ResourceManager counts by owner (all vs unowned) for goals and UI
5. **Editor tools** — Create Policy Manager and Database Inspector for policies

For implementation status and remaining gaps (e.g. goal attribution from policy), see [Resource-Ownership.md](../features%20planning/Resource-Ownership.md).

---

## Part 1: What is the Ownership Model?

The **ownership model** determines whether resources and stations are **shared** (communal) or **owned** by a player (or team). It enables:

- **Ownership-aware scoring** — Score or goals can (when configured) consider who owns the resources.
- **Controlled taking** — Policies can allow or disallow taking another player’s resources (stealing).
- **Station access** — Policies can restrict who can use a station (owner only, same team, or anyone).

Key components:

- **ResourceObject** — Has an `owner` (playerController). When a resource is produced or grabbed, the owner can be set according to policy.
- **Station** — Has an `owner` and a `worker`. Produced resources can be assigned to the station owner when policy is not communal.
- **Policy Manager** — Defines the **ownership model** (Communal, Private Individual, Private Team, Hybrid), **stealing policy**, and **station use policy**. See [Tutorial 10](10-Policy-System.md).

---

## Part 2: Default — Communal (No Policy Manager)

When there is **no PolicyManager** in the scene:

- **Production** — Stations do **not** set an owner on produced resources; they remain unowned (common).
- **Grab / absorb** — Players can grab or absorb any resource; no check against another owner. No ownership is assigned on grab/absorb.
- **Stations** — Anyone can use any station (work, consume, produce).
- **Counting** — **ResourceManager** counts **all** resources of a type when you call `GetResourceCount(resourceType)` (or with owner = null). Goals and UI see the full count.

This is the default "everyone shares" behavior and requires no Policy Manager.

---

## Part 3: With Policy Manager — How Behavior Changes

When a **Policy Manager** is present and a **PolicyDataSO** is assigned, the following apply.

### Production (Station)

- The station sets the **owner** of a produced resource only when:
  - The ownership model is **not** Communal, **and**
  - The station has an **owner** assigned.
- Otherwise the produced resource stays **unowned** (common).

### Grab and absorb (playerController)

- **Before grab** — The game checks `PolicyManager.CanTakeResource(actor, resourceObj.owner)`. If the resource has another owner and policy disallows taking (e.g. Stealing = Disallowed), the grab is **blocked**.
- **After grab** — If taking is allowed and the model is not Communal, the resource’s **owner** is set to the grabbing player (ownership transfer).
- **Absorb** — The same rule applies when absorbing resources into the player’s multigrab: only allowed if `CanTakeResource` permits; then ownership is assigned to the absorbing player when not communal.

### Grabbable list (grabRegion)

- A resource is only added to the player’s grabbable list if there is no Policy Manager **or** `CanTakeResource(player, resourceObj.owner)` is true. So players cannot even **target** resources they are not allowed to take.

### Station use

- Before a **worker-based** consume or produce (e.g. ConsumeOnWork, ProduceOnWork), the station checks `CanUseStation(worker, station.owner)`. If the policy is e.g. Owner Only and the worker is not the owner, the action is **skipped**.

---

## Part 4: Ownership-Aware Counting

**ResourceManager** can count resources **by owner** or by **policy when owner is null**.

- **GetResourceCount(Resource type, playerController ownerOrNull)**
  - If **ownerOrNull** is set — Counts only resources of that type whose `owner` equals that player.
  - If **ownerOrNull** is **null**:
    - **No Policy Manager** or **Communal** → Counts **all** resources of that type.
    - **Policy Manager** and **not Communal** → Counts only **unowned** (common) resources of that type.

- **GetResourceCount(Resource type)** (no owner) uses the rule above for null, so **goals** and **resource UI** automatically get either "all" or "unowned only" depending on policy.

- **GetAllResourceCounts(ownerOrNull)** — Same rules applied per resource type for a full count dictionary.

So: in a private ownership game, the global resource panel and goals that use `GetResourceCount(type)` see only the **common pool** (unowned resources) unless you explicitly pass an owner to count that player’s resources.

---

## Part 5: Editor Tools and Summary

- **Create Policy Manager** — **Game Assemblies → Systems → Create Policy Manager** creates a policy asset and adds or updates the Policy Manager in the scene. Configure ownership model, stealing, station use, etc. there.
- **Database Inspector** — **Game Assemblies → Databases → Database Inspector** → **Policies** lets you browse and edit policy assets.

**Summary**

- No Policy Manager = **communal**: no owners, anyone can grab and use anything; counting is "all."
- With Policy Manager and non-Communal model: production can set owner; grab/absorb are checked and can transfer ownership; station use can be restricted; counting with owner null = "unowned only."
- Use **GetResourceCount(type, owner)** when you need per-player or unowned-only counts; goals and UI that use the no-owner overload follow policy automatically.
