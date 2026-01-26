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

### Learning Path

**New to Unity or C#?** Start with the **Programming Fundamentals** tutorials to build a solid foundation:
1. [GameObjects and Components](basic%20concepts/GameObjects-and-Components.md) — Understand Unity's fundamental building blocks
2. [Basic C# Syntax](basic%20concepts/Basic-CSharp-Syntax.md) — Learn essential C# programming concepts
3. [Prefabs](basic%20concepts/Prefabs.md) — Understand Unity's prefab system
4. [Common Data Structures](basic%20concepts/Common-Data-Structures.md) — Master Lists and Dictionaries
5. [Basic Vector Math](basic%20concepts/Basic-Vector-Math.md) — Learn vector operations for game development

**Familiar with Unity?** Jump to the **Core Concepts** tutorials:
1. [Scriptable Objects](basic%20concepts/Scriptable-Objects.md) — Understanding data assets used throughout Game Assemblies
2. [Static References](basic%20concepts/Static-References.md) — Learn about singletons and global manager access
3. [Editor Tools](basic%20concepts/Editor-Tools.md) — Create custom Unity editor windows and menu items

**Ready to build?** Follow the **Step-by-Step Tutorials** in order:
1. [Tutorial 01: Creating a Character and Canvas](tutorials/01-Creating-Character-and-Canvas.md) — Set up your scene
2. [Tutorial 02: Stations and Resources](tutorials/02-Stations-and-Resources.md) — Add resources and production
3. [Tutorial 03: Resource Manager and Goals](tutorials/03-Resource-Manager-and-Goals.md) — Set up resource tracking and UI
4. [Tutorial 04: Goals and Goal Tracker](tutorials/04-Goals-and-Goal-Tracker.md) — Implement the Goals system
5. [Tutorial 05: Levels and Level Editor](tutorials/05-Levels-and-Level-Editor.md) — Create structured gameplay experiences

---

## Basic Concepts

Foundational tutorials that explain core Unity and Game Assemblies concepts. These are recommended reading before diving into the practical tutorials.

### Programming Fundamentals

Essential programming concepts for Unity and C# development. Start here if you're new to programming or Unity.

| Concept | Description |
|---------|-------------|
| [GameObjects and Components](basic%20concepts/GameObjects-and-Components.md) | Understanding Unity's fundamental building blocks: what GameObjects are, how components add functionality, why Transform is mandatory, and common component patterns. Essential for working with Unity. |
| [Basic C# Syntax](basic%20concepts/Basic-CSharp-Syntax.md) | Essential C# programming concepts: variables, data types, methods, classes, control flow, operators, and common patterns used in Unity and Game Assemblies. |
| [Prefabs](basic%20concepts/Prefabs.md) | Understanding Unity prefabs: what they are, how to create and use them, prefab instances, variants, and working with prefabs in code. Essential for reusable game objects. |
| [Common Data Structures](basic%20concepts/Common-Data-Structures.md) | Working with Lists and Dictionaries: when to use each, common operations, iteration patterns, and practical examples from Game Assemblies code. |
| [Basic Vector Math](basic%20concepts/Basic-Vector-Math.md) | Vector operations for game development: Vector2 and Vector3, basic operations, magnitude, normalization, distance calculations, and movement patterns. |

### Core Concepts

Game Assemblies-specific concepts and Unity editor tools. Read these to understand how the library works.

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
