# Game Assemblies Documentation

Welcome to the Game Assemblies documentation. This folder contains tutorials, guides, and API reference for using the Game Assemblies package.

---

## Outline

1. [Getting Started](#getting-started)
2. [Tutorials](#tutorials)
3. [Documentation Structure](#documentation-structure)
4. [Contributing](#contributing)

---

## Getting Started

Start with the main [README.md](../README.md) in the root directory for installation and basic usage instructions.

Then follow the tutorials in order: begin with [Tutorial 01: Creating a Character and Canvas](Tutorials/01-Creating-Character-and-Canvas.md) to set up your scene, [Tutorial 02: Stations and Resources](Tutorials/02-Stations-and-Resources.md) to add resources and production, [Tutorial 03: Resource Manager and Goals](Tutorials/03-Resource-Manager-and-Goals.md) to set up resource tracking and the UI, and [Tutorial 04: Goals and Goal Tracker](Tutorials/04-Goals-and-Goal-Tracker.md) for the Goals system and goal-completing stations.

---

## Tutorials

Step-by-step guides for common workflows. Work through them in order for the best experience.

| # | Tutorial | Description |
|---|----------|-------------|
| 01 | [Creating a Character and Canvas](Tutorials/01-Creating-Character-and-Canvas.md) | Set up a basic scene with a playable character and background canvas using the Game Assemblies editor tools. Covers creating an empty white canvas and a local multiplayer player system with customizable character sprites. |
| 02 | [Stations and Resources](Tutorials/02-Stations-and-Resources.md) | Learn the core concepts of **Stations** and **Resources**: what they are, how to create resource types and stations, and how to build conversion chains for gathering, producing, and transforming resources. |
| 03 | [Resource Manager and Goals](Tutorials/03-Resource-Manager-and-Goals.md) | Use the **Resource Manager** system: add the Resource Management System to your scene, configure resource tracking and UI panels, and understand **global capital** and how it connects to the Goals system. |
| 04 | [Goals and Goal Tracker](Tutorials/04-Goals-and-Goal-Tracker.md) | Create **ResourceGoalSO** goals, configure the **GoalManager** and **Goal Tracker** UI, and use **stations as goal-completing modules** with `completesGoals_consumption` and `completesGoals_production`. |

---

## Documentation Structure

- **`README.md`** — This file; overview and navigation
- **`Tutorials/`** — Step-by-step tutorials (see [Tutorials](#tutorials) above)
- **`API/`** — API reference documentation
- **`Guides/`** — Detailed guides for specific features

---

## Contributing

When adding new documentation:

- Use clear, descriptive filenames
- Include code examples where applicable
- Keep tutorials focused on specific tasks
- Update this README when adding new sections or tutorials
