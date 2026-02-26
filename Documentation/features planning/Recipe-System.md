# Feature Planning: Recipe System

## Overview

The **Recipe System** allows a station to support **multiple alternative methods of production**. Instead of a single fixed input→output mapping, a station can have several **recipes**, each defining a list of paired inputs and outputs. Players or the simulation can then choose which recipe to run (e.g., “craft A from wood” vs “craft A from metal”), enabling variety and strategy (different resources, efficiencies, or unlock paths).

A **recipe** is essentially:

- A list of **inputs** (resource type + quantity) required to be present or consumed.
- A list of **outputs** (resource type + quantity) produced when the recipe runs.
- Optional metadata (name, icon, duration, unlock conditions) for UI and game logic.

All stations can have alternative methods of production by referencing one or more recipes rather than a single consume/produce list.

---

## 2. Scope and Design

### 2.1 Recipe as a first-class asset

- **Recipe** (e.g. `RecipeSO` or similar) — ScriptableObject or data asset that defines:
  - **Inputs:** List of `(Resource, amount)` pairs. When the recipe runs, these are consumed (or checked, depending on station behavior).
  - **Outputs:** List of `(Resource, amount)` pairs. When the recipe runs, these are produced.
  - Optional: display name, icon, description, work duration, category/tags for filtering.

### 2.2 Station integration

- **Station** currently has fixed `consumes` and `produces` lists (from `StationDataSO`). The recipe system would allow:
  - A station to reference a **list of recipes** instead of (or in addition to) a single consume/produce pair.
  - At runtime, the station (or the player) **selects which recipe** is active; input requirements and outputs come from that recipe.
  - Existing “single recipe” stations can be represented as a station with one recipe, or remain as the current fixed consume/produce for backward compatibility.

### 2.3 Alternative methods of production

- **Alternative methods** — Same station type can offer different recipes (e.g., “Furnace: Iron from Ore” vs “Furnace: Steel from Iron + Coal”). Requirements and outputs are driven by the active recipe.
- **Selection** — How the active recipe is chosen (e.g., first available, player choice, automatic by resource availability, or event-driven) is a design choice and may integrate with an [Event System](Event-System.md) or UI.

### 2.4 Relation to existing systems

- **ResourceManager** — Still tracks all resources; consumption and production from a recipe would call the same add/remove or station produce/consume logic.
- **Goals** — Goal progress can count resources produced or delivered; recipe outputs are still resources, so no conceptual change. Optional: goals that require “produced via recipe X.”
- **Policy / Ownership** — Ownership and policy (who can use the station, who gets credit) apply to the station and its actions; the recipe only defines what is consumed and produced.

---

## 3. Current State

- **Implemented.** `RecipeSO` and `RecipeSlot` define recipe data; `StationDataSO` has `useRecipes` and `recipes`; `Station` uses `GetConsumes()`/`GetProduces()` and `activeRecipeIndex`. Create Recipe window and Station Builder recipe assignment are in place. When `useRecipes` is false, behavior is unchanged (fixed consumes/produces).

---

## 4. To-Do / Next Steps

- [x] **Define Recipe data model** — ScriptableObject or asset type for Recipe (inputs list, outputs list, optional name/icon/duration).
- [x] **StationDataSO / Station** — Add support for “recipe list” mode: station holds or references multiple recipes; one is “active” or selectable.
- [x] **Station logic** — When using recipes, resolve input requirements and output products from the active recipe instead of from fixed consumes/produces.
- [x] **Editor tools** — Recipe creator/editor (e.g. Game Assemblies > Recipes > Create Recipe), store recipes in a Databases or similar folder; optional Station Builder integration to assign recipes to stations.
- [x] **Backward compatibility** — Ensure stations that only need one “recipe” can keep using current consume/produce or be represented as a single-recipe station.
- [ ] **Integration with Event System** — Optional: events that unlock/disable or modify recipes (see [Event-System.md](Event-System.md)).

---

## 5. Success Criteria

- A station can define multiple recipes (alternative methods of production).
- Each recipe is a clear input→output mapping (list of input pairs, list of output pairs).
- At runtime, the station (or player) can use one of its recipes to consume inputs and produce outputs.
- Recipe data is reusable and stored as assets (e.g. in a Databases/Recipes folder); stations reference recipes rather than duplicating data.
