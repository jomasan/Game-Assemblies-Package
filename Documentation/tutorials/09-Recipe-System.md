# Tutorial 09: Recipe System

This tutorial explains the **Recipe System** in Game Assemblies: how to create recipes (alternative methods of production), assign them to stations, and use the active recipe at runtime so a single station can offer multiple input→output options.

## Prerequisites

- Completed [Tutorial 02](02-Stations-and-Resources.md) (Stations and Resources)
- Completed [Tutorial 03](03-Resource-Manager-and-Goals.md) (Resource Manager and Goals)
- A scene with at least one station and the Resource Management System

## Overview

In this tutorial, you'll learn:

1. **What is a Recipe?** — RecipeSO and RecipeSlot: inputs and outputs with quantities
2. **Creating a Recipe** — Using the Create Recipe editor window
3. **Assigning Recipes to a Station** — Using the Station Builder or StationDataSO
4. **Active Recipe at Runtime** — How the station chooses which recipe runs and how to switch it
5. **When to Use Recipes vs Fixed Consumes/Produces** — Backward compatibility and single-recipe stations

For deeper design notes and success criteria, see the feature planning document: [Recipe-System.md](../features%20planning/Recipe-System.md).

---

## Part 1: What is a Recipe?

A **recipe** is one **alternative method of production** for a station. Instead of a single fixed list of inputs and outputs, a station can have **multiple recipes**; at any time, one recipe is **active** and defines what must be consumed and what is produced.

### RecipeSO and RecipeSlot

- **RecipeSO** (ScriptableObject) — The recipe asset. It has:
  - **recipeName** — Display name (e.g. for UI or the Station Builder).
  - **icon** (optional) — Sprite for the recipe.
  - **workDurationOverride** (optional) — If &gt; 0, this recipe uses its own work duration instead of the station default.
  - **inputs** — List of **RecipeSlot**: each slot is a `Resource` + **amount** (how many of that resource are consumed).
  - **outputs** — List of **RecipeSlot**: each slot is a `Resource` + **amount** (how many are produced).

- **RecipeSlot** — A single entry: `resource` (Resource), `amount` (int). For example, "2 Wood" and "1 Iron" as inputs, "1 Plank" as output.

When the station runs, it consumes the active recipe’s inputs (from the input area) and produces the active recipe’s outputs (spawns resources, updates goals, etc.).

---

## Part 2: Creating a Recipe

1. **Open the Create Recipe window**  
   In Unity’s top menu:
   ```
   Game Assemblies → Recipes → Create Recipe
   ```

2. **Set the recipe identity**
   - **Recipe Name** — Name for the asset (e.g. `"Wood to Plank"`).
   - **Icon (optional)** — Assign a sprite if you want one.
   - **Work Duration Override** — Leave `0` to use the station’s work duration, or set a value (e.g. `2`) to override for this recipe.

3. **Add inputs**
   - Under **Inputs**, use **+ Add slot** for each required resource.
   - For each slot: assign the **Resource** and set **amount** (e.g. 2 Wood, 1 Ore).

4. **Add outputs**
   - Under **Outputs**, use **+ Add slot** for each produced resource.
   - For each slot: assign the **Resource** and set **amount** (e.g. 1 Plank).

5. **Click Create**  
   The recipe is saved at:
   ```
   Assets/Game Assemblies/Databases/Recipes/{RecipeName}.asset
   ```
   You can edit it later in the Inspector or in the [Database Inspector](08-Database-Inspector-and-Data-Management.md) under **Recipes**.

---

## Part 3: Assigning Recipes to a Station

Stations use recipes when **Use Recipes** is enabled on their **StationDataSO**.

### Option A: Station Builder

1. Open **Game Assemblies → Stations → Station Builder**.
2. In the **INPUTS** section, enable **Use Recipes**.
3. Use **+ Add Recipe** and assign the recipe assets you created (e.g. "Wood to Plank", "Metal to Plank").
4. Build the station as usual. The resulting StationDataSO will have `useRecipes = true` and `recipes` filled.

### Option B: Inspector (existing station)

1. Select the **StationDataSO** asset (or a station instance and edit its `stationData` reference).
2. Enable **Use Recipes**.
3. Add elements to the **Recipes** list and assign your RecipeSO assets.

When **Use Recipes** is **off**, the station behaves as before: it uses the fixed **Consumes** and **Produces** lists only (backward compatible).

---

## Part 4: Active Recipe at Runtime

- Each **Station** has an **activeRecipeIndex** (integer). When the station uses recipes, the **active** recipe is `recipes[activeRecipeIndex]` (clamped to a valid index).
- The station’s **input area requirements** and **output production** come from the active recipe: it consumes that recipe’s inputs and produces that recipe’s outputs.
- **Info panel** — When the player inspects the station, the info panel shows the **current** recipe’s inputs and outputs (because the station uses `GetConsumes()` and `GetProduces()`, which read from the active recipe when recipes are enabled).

### Changing the active recipe

- **In the Inspector** — Select the station at runtime and set **Active Recipe Index** (0 = first recipe, 1 = second, etc.).
- **In code** — Set `station.activeRecipeIndex = index`, then call `station.RefreshFromRecipe()` so the input area requirements and info panel update:
  ```csharp
  station.activeRecipeIndex = 1;
  station.RefreshFromRecipe();
  ```

If you never change `activeRecipeIndex`, the station always uses the first recipe (index 0).

---

## Part 5: When to Use Recipes vs Fixed Consumes/Produces

| Scenario | Use |
|----------|-----|
| One fixed conversion (e.g. always 1 Wood → 1 Plank) | Leave **Use Recipes** off; use the fixed **Consumes** and **Produces** lists. |
| Multiple alternatives (e.g. 2 Wood → 1 Plank **or** 1 Metal → 1 Plank) | Enable **Use Recipes** and add multiple RecipeSO assets; switch at runtime with **activeRecipeIndex**. |
| Single recipe but you want to reuse it on many stations | Create one RecipeSO, enable **Use Recipes** on each station, and add that one recipe to the list. |

Stations with **Use Recipes** off are unchanged from pre-recipe behavior; the recipe system is additive.

---

## Summary

- **RecipeSO** = one alternative method of production (inputs + outputs with quantities).
- Create recipes with **Game Assemblies → Recipes → Create Recipe**; they are stored in **Game Assemblies/Databases/Recipes**.
- Enable **Use Recipes** on a station’s StationDataSO and assign a list of recipes; the station uses **activeRecipeIndex** to choose which recipe runs.
- Use **RefreshFromRecipe()** after changing **activeRecipeIndex** so the input area and info panel stay in sync.
- Keep **Use Recipes** off to use the classic fixed consumes/produces lists.
