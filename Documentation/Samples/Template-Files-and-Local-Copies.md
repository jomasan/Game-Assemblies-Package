# Sample Template Files and Local Copies

This document describes the main **template prefabs** shipped in the **Samples** folder of the Game Assemblies package, and how the editor tools create **local copies** in your project so you can build your own variants without modifying package content.

---

## Why templates and local copies?

- **Templates** live inside the package (under `Samples/`) and are read-only for the library. They define the required components and structure (e.g. `Station`, `ResourceObject`, colliders, UI).
- **Local copies** are created under **`Assets/Game Assemblies/`** in your project. All new prefabs and database assets (ScriptableObjects) are written there. You can edit these copies freely to create your own stations, resources, players, and environment objects.

The package uses **`SA_AssetPathHelper`** to resolve template paths whether the package is installed as a UPM package or opened from `Assets/`. New assets are always created under `Assets/Game Assemblies/...` so they stay in your project.

---

## Main template prefabs (Samples)

All paths below are relative to the package root (e.g. `Samples/Prefabs/...`).

### Stations

| Template | Path | Used by |
|----------|------|--------|
| **station_template** | `Samples/Prefabs/Stations/station_template.prefab` | **Station Builder** (default) |
| station_template_small | `Samples/Prefabs/Stations/station_template_small.prefab` | Optional alternative in Station Builder |

- Must have a **Station** component.
- Station Builder can use a different prefab via the **Station Prefab Template** field; if empty, the default above is used.

### Resources

| Template | Path | Used by |
|----------|------|--------|
| **resource_obj_template** | `Samples/Prefabs/Resources/resource_obj_template.prefab` | **Resource Builder**, **Create Resource** |

- Must have **ResourceObject** and a **SpriteRenderer** (or in children).
- Used as the base prefab for new resource types; the tool copies it and then assigns the new Resource ScriptableObject, icon, scale, and behavior tags.

### Players

| Template | Path | Used by |
|----------|------|--------|
| **Player_Drawn_Small** | `Samples/Prefabs/Players/Player_Drawn_Small.prefab` | **Create Local Multiplayer System** (Create Players) |

- Used when creating a new player prefab; you can assign a different template in the window. The new prefab is a **copy** of the template so you can customize sprite, collider, and input without touching the sample.

### Environment objects

| Template | Path | Used by |
|----------|------|--------|
| **Obstacle_Template** | `Samples/Prefabs/Obstacle_Template.prefab` | **Create Environment Object** (Obstacle) |
| **Groundtile_Template** | `Samples/Prefabs/Groundtile_Template.prefab` | **Create Environment Object** (Ground Tile) |
| **Folliage_Template** | `Samples/Prefabs/Folliage_Template.prefab` | **Create Environment Object** (Foliage) |

- Each environment type has its own template; the Create Environment Object window lets you pick a different prefab per type. New prefabs are created as **copies** of the selected template.

### Managers and UI (reference prefabs)

These prefabs are used when **creating systems** (e.g. Level Manager, Game State Manager, Resource Management System). They are typically **instantiated** or **referenced** from the package rather than copied into `Game Assemblies/`:

- `Samples/Prefabs/Managers/LevelManager.prefab`
- `Samples/Prefabs/Managers/GameStateManagerAndCanvas.prefab`
- `Samples/Prefabs/Managers/ResourceManager.prefab`
- `Samples/Prefabs/Managers/GoalManager.prefab`
- `Samples/Prefabs/Managers/PlayerManager.prefab`
- `Samples/Prefabs/UI Prefabs/ResourceManager_Canvas.prefab`
- `Samples/Prefabs/Canvas/BackgroundPlane.prefab`
- `Samples/Prefabs/Canvas/ScreenBackground.prefab`
- `Samples/Prefabs/Cameras/Pixel Perfect Camera.prefab`
- `Samples/Prefabs/UI Prefabs/InfoWindow.prefab` (used by station prefabs)

Creating a “local copy” for these is usually done by the specific menu (e.g. “Create Level Manager”) which instantiates or places the manager in the scene; the prefab itself may remain in the package.

---

## How local copies are created

### 1. Copy template prefab (`AssetDatabase.CopyAsset`)

Used for **Resources**, **Players**, and **Environment** prefabs:

1. The editor finds the template prefab (from `Samples/` or the path you chose).
2. It ensures the target folder under `Assets/Game Assemblies/` exists (`EnsureAssetPathDirectories`).
3. It calls **`AssetDatabase.CopyAsset(templatePath, newPrefabPath)`** so the new prefab is a full copy in your project.
4. The new prefab is then loaded and modified (e.g. assign Resource SO, set sprite, scale, tags).

**Result:** A new prefab at e.g. `Assets/Game Assemblies/Prefabs/Resources/MyResource.prefab` that you can edit. The template in `Samples/` is unchanged.

### 2. Instantiate then save as prefab (Stations)

Used for **Stations**:

1. The **Station Builder** loads the station template prefab (e.g. `station_template.prefab`).
2. It creates a **StationDataSO** and saves it under `Assets/Game Assemblies/Databases/Stations/{name}.asset`.
3. It **instantiates** the template in the active scene, applies the station data (sprite, scale, inputs/outputs, etc.), and optionally resizes colliders to the station graphic.
4. It saves the modified instance as a **new prefab** with **`PrefabUtility.SaveAsPrefabAssetAndConnect`** to `Assets/Game Assemblies/Prefabs/Stations/{name}.prefab`.
5. The StationDataSO’s `stationPrefab` field is set to this new prefab.

**Result:** A new station prefab and a matching StationDataSO in your project; the template is only used as the initial structure, not copied byte-for-byte.

### 3. ScriptableObjects (no prefab template)

**Goals**, **Levels**, **Loot Tables**, **Rules**, **Rules Sessions**, and **Resources** (the data asset) are **ScriptableObjects**. They are created with **`ScriptableObject.CreateInstance<>()`** and **`AssetDatabase.CreateAsset(...)`**. There is no prefab template; the tool just creates a new asset and saves it under `Assets/Game Assemblies/Databases/...` (e.g. `Databases/Goals/`, `Databases/Levels/`, `Databases/Resources/`).

---

## Where your files go (Game Assemblies)

| Content type | Prefab path (local copy) | Database asset path (if any) |
|--------------|--------------------------|------------------------------|
| Stations | `Game Assemblies/Prefabs/Stations/{name}.prefab` | `Game Assemblies/Databases/Stations/{name}.asset` |
| Resources | `Game Assemblies/Prefabs/Resources/{name}.prefab` | `Game Assemblies/Databases/Resources/{name}.asset` |
| Players | `Game Assemblies/Prefabs/Players/{name}.prefab` | — |
| Environment (obstacles/tiles/foliage) | `Game Assemblies/Prefabs/Environment/Obstacles\|Tiles\|Foliage/{name}.prefab` | — |
| Goals | — | `Game Assemblies/Databases/Goals/{name}.asset` |
| Levels | — | `Game Assemblies/Databases/Levels/{name}.asset` |
| Loot tables | — | `Game Assemblies/Databases/LootTables/{name}.asset` |
| Rules / Rules sessions | — | `Game Assemblies/Databases/Rules\|Rules Sessions/{name}.asset` |

All of these paths are under **`Assets/`**, so they live in your project and are safe to edit and version control.

---

## Summary

- **Templates** = prefabs in **Samples/** (package). Used as the source structure for stations, resources, players, and environment objects.
- **Local copies** = new prefabs and assets under **`Assets/Game Assemblies/`**. Created by **CopyAsset** (resources, players, environment) or **Instantiate + SaveAsPrefabAsset** (stations); database assets are created with **CreateAsset**.
- You can always point the editor windows at a **different template** (e.g. your own base station or resource prefab) when the UI exposes a “Template” or “Prefab Template” field, and the new variant will still be written into `Game Assemblies/` so you keep your own variants separate from the package samples.
