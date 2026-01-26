# Game Assemblies Documentation

Welcome to the Game Assemblies documentation. This folder contains tutorials, guides, and API reference for using the Game Assemblies package.

---

## Outline

1. [Getting Started](#getting-started)
2. [Basic Concepts](#basic-concepts)
3. [Tutorials](#tutorials)
4. [Documentation Structure](#documentation-structure)
5. [Contributing](#contributing)

---

## Getting Started

Start with the main [README.md](../README.md) in the root directory for installation and basic usage instructions.

### For Beginners

If you're new to Unity or Game Assemblies concepts, we recommend starting with the **Basic Concepts** tutorials to understand foundational topics:

- **[Scriptable Objects](basic%20concepts/Scriptable-Objects.md)** — Understanding data assets and how they're used throughout Game Assemblies
- **[Static References](basic%20concepts/Static-References.md)** — Learning about singletons and how managers are accessed globally
- **[Editor Tools](basic%20concepts/Editor-Tools.md)** — Creating custom Unity editor windows and menu items

### Step-by-Step Tutorials

Then follow the practical tutorials in order: begin with [Tutorial 01: Creating a Character and Canvas](tutorials/01-Creating-Character-and-Canvas.md) to set up your scene, [Tutorial 02: Stations and Resources](tutorials/02-Stations-and-Resources.md) to add resources and production, [Tutorial 03: Resource Manager and Goals](tutorials/03-Resource-Manager-and-Goals.md) to set up resource tracking and the UI, [Tutorial 04: Goals and Goal Tracker](tutorials/04-Goals-and-Goal-Tracker.md) for the Goals system and goal-completing stations, and [Tutorial 05: Levels and Level Editor](tutorials/05-Levels-and-Level-Editor.md) for creating structured gameplay experiences with levels.

---

## Basic Concepts

Foundational tutorials that explain core Unity and Game Assemblies concepts. These are recommended reading before diving into the practical tutorials.

| Concept | Description |
|---------|-------------|
| [Scriptable Objects](basic%20concepts/Scriptable-Objects.md) | Learn what ScriptableObjects are, why they're used in Game Assemblies, and how to create and use them. Essential for understanding resources, goals, levels, and other data assets. |
| [Static References](basic%20concepts/Static-References.md) | Understand the singleton pattern and how static references provide global access to managers like ResourceManager, GoalManager, and LevelManager. |
| [Editor Tools](basic%20concepts/Editor-Tools.md) | Learn how to create custom Unity editor windows and menu items. Covers EditorWindow, menu items, GUI elements, and creating prefabs and ScriptableObjects programmatically. |

---

## Tutorials

Step-by-step guides for common workflows. Work through them in order for the best experience.

| # | Tutorial | Description |
|---|----------|-------------|
| 01 | [Creating a Character and Canvas](tutorials/01-Creating-Character-and-Canvas.md) | Set up a basic scene with a playable character and background canvas using the Game Assemblies editor tools. Covers creating an empty white canvas and a local multiplayer player system with customizable character sprites. |
| 02 | [Stations and Resources](tutorials/02-Stations-and-Resources.md) | Learn the core concepts of **Stations** and **Resources**: what they are, how to create resource types and stations, and how to build conversion chains for gathering, producing, and transforming resources. |
| 03 | [Resource Manager and Goals](tutorials/03-Resource-Manager-and-Goals.md) | Use the **Resource Manager** system: add the Resource Management System to your scene, configure resource tracking and UI panels, and understand **global capital** and how it connects to the Goals system. |
| 04 | [Goals and Goal Tracker](tutorials/04-Goals-and-Goal-Tracker.md) | Create **ResourceGoalSO** goals, configure the **GoalManager** and **Goal Tracker** UI, and use **stations as goal-completing modules** with `completesGoals_consumption` and `completesGoals_production`. |
| 05 | [Levels and Level Editor](tutorials/05-Levels-and-Level-Editor.md) | Create **LevelDataSO** levels using the Create Level editor window. Learn about Sequential and Random Interval modes, configure level settings, and understand how LevelManager uses level data to spawn goals dynamically. |

---

## Documentation Structure

- **`README.md`** — This file; overview and navigation
- **`basic concepts/`** — Foundational tutorials explaining core concepts (see [Basic Concepts](#basic-concepts) above)
- **`tutorials/`** — Step-by-step tutorials (see [Tutorials](#tutorials) above)
- **`API/`** — API reference documentation
- **`Guides/`** — Detailed guides for specific features

---

## Contributing

When adding new documentation:

- Use clear, descriptive filenames
- Include code examples where applicable
- Keep tutorials focused on specific tasks
- Update this README when adding new sections or tutorials
