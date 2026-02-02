# Game Assemblies - Local Multiplayer Game Library

A Unity-based game development library for creating local multiplayer games centered around resource conversion and goal achievement mechanics. Inspired by games like **Overcooked**, this library provides a comprehensive set of systems for building cooperative and competitive multiplayer experiences where players work together (or compete) to convert resources and complete objectives.

## Overview

Game Assemblies is designed to streamline the creation of local multiplayer games with a focus on:
- **Resource Management**: Convert, produce, and consume resources through interactive stations
- **Goal-Oriented Gameplay**: Create time-based or resource-based objectives for players to complete
- **Local Multiplayer**: Support for multiple players using gamepads or keyboard input
- **Modular Design**: Mix and match systems to create unique game experiences

## Core Philosophy

The library is built around the concept of **resource conversion chains** — players gather base resources, process them through stations, and deliver final products to achieve goals. This creates engaging gameplay loops where coordination, planning, and efficiency are key to success.

---

## Documentation

**[📚 Full Documentation](Documentation/README.md)** — Tutorials, basic concepts, and step-by-step guides.

### Quick Links

| Topic | Description |
|-------|-------------|
| [Basic Concepts (01–10)](Documentation/README.md#basic-concepts) | GameObjects, Prefabs, C# Syntax, Vector Math, Data Structures, Static References, Scriptable Objects, Compressed Syntax, Input System, Editor Tools |
| [Step-by-Step Tutorials (01–05)](Documentation/README.md#tutorials) | Creating a Character and Canvas, Stations and Resources, Resource Manager and Goals, Goals and Goal Tracker, Levels and Level Editor |
| [Learning Path](Documentation/README.md#learning-path) | Recommended order for new users |

---

## Editor Tools

The library includes **21 dedicated editor tools** accessible from the **Game Assemblies** menu for easy setup and content creation:

| Category | Tools |
|----------|-------|
| **Systems** | Create Resource Management System, Create Levels System and Menu |
| **Environment** | Create White Canvas, Create Stage Background, Create Ground Tile, Create Bush, Procedural Level Builder |
| **Camera** | Create Pixel Perfect Camera |
| **Players** | Create Local Multiplayer System |
| **Resources** | Create Resource, Resource Builder, Create Loot Table |
| **Stations** | Station Builder |
| **Goals** | Create Goal |
| **Levels** | Create Level |
| **Rules** | Create Rule, Create Rules Session |
| **Databases** | Database Inspector |
| **Asset Tools** | Crop Image, Resample Colors, Pixel Art Converter |

---

## System Index

### 🎮 Player Systems
- **Player Controller** — Core player movement, interaction, and input handling
- **Player Info Manager** — Manages player data, colors, and identification
- **Multi-Input Support** — Gamepad and keyboard input via Unity's Input System

### 📦 Resource Management System
- **Resource Manager** — Central hub for tracking all resources in the game
- **Resource Objects** — Physical items that players can grab, carry, and deliver
- **Resource Nodes** — Base class for resource-producing entities
- **Resource Pools** — Object pooling for efficient resource spawning
- **Resource UI Binding** — Connect resources to UI displays

### 🏭 Station & Production System
- **Station** — Interactive workstations that convert resources
- **Station Manager** — Manages station states and interactions
- **Resource Producer** — Automated resource generation
- **Resource Sink** — Consumes resources to produce outputs
- **Consume Area** — Input zones for resource delivery
- **Production Modes** — Resource, Station, or LootTable outputs

### 🎯 Goal System
- **Goal Manager** — Tracks and manages active goals
- **Resource Goals** — Time-based or resource-count objectives
- **Goal Tracker UI** — Visual display of goal progress
- **Goal ScriptableObjects** — Data-driven goal configuration

### 📊 Level Management
- **Level Manager** — Controls level progression and timing
- **Level Data** — ScriptableObject-based level configuration
- **Sequential & Random Goals** — Different goal spawning patterns
- **Score Brackets** — Star rating system based on performance

### 🎨 Game Management
- **Game Manager** — State machine for game flow (Menu, Playing, Paused, Results)
- **Creation Manager** — Handles object spawning and creation
- **Soundtrack Manager** — Manages background music and audio

### 🗺️ Area & Region System
- **Area** — Spatial zones for triggering events
- **Grab Region** — Detection zones for object interaction
- **Region Events** — Event-driven area interactions

### 🎲 Loot & Randomization
- **Loot Tables** — Weighted random resource generation
- **Random Populate** — Spawn objects randomly within bounds

### 🎨 Visual & UI Systems
- **Progress Bar Controller** — Visual feedback for work progress
- **Info Window** — Display information panels
- **Dynamic Sorting Order** — 2D depth sorting for sprites
- **Color Palette System** — Themed color management

### 🔧 Utility Systems
- **Tag System** — MultiTag component for flexible object tagging
- **Tween System** — Animation and easing functions
- **Camera Shake** — Screen effects
- **Countdown Timer** — Time-based mechanics
- **Object Spawner** — Generic spawning system

---

## Quick Start

1. **Import the Library**: Add the package to your Unity project (or import the `Assets/Game-Assemblies-Package` folder)
2. **Set Up Players**: Use **Game Assemblies → Players → Create Local Multiplayer System**
3. **Create Resources**: Use **Game Assemblies → Resources → Create Resource** or **Resource Builder**
4. **Build Stations**: Use **Game Assemblies → Stations → Station Builder**
5. **Define Goals**: Use **Game Assemblies → Goals → Create Goal**
6. **Configure Levels**: Use **Game Assemblies → Levels → Create Level**

For detailed setup instructions, see the [Documentation](Documentation/README.md).

---

## Samples

The package includes sample content in the **Samples** folder:
- **Prefabs** — Players, stations, managers, UI elements
- **Examples** — Tutorial scenes (Automatic Resource Production, Converting Resources), Cooking Game Template, Foresting Example, 2D Drawing setups
- **2D Assets** — Sprites, tiles, icons for prototyping
- **Player Controls** — Input Actions asset for keyboard and gamepad

---

## Key Features

✅ **Local Multiplayer Ready** — Built-in support for multiple players with gamepad/keyboard  
✅ **21 Editor Tools** — Dedicated windows for creating resources, stations, goals, levels, loot tables, and more  
✅ **Modular Architecture** — Use only the systems you need  
✅ **ScriptableObject Driven** — Data-driven design for easy iteration  
✅ **Extensible** — Easy to add custom behaviors and systems  
✅ **UI Integration** — Pre-built UI components for common needs  
✅ **Event-Driven** — UnityEvents for flexible system communication  

---

## Requirements

- **Unity Version**: 2021.3 LTS or later
- **Required Packages**:
  - Input System (com.unity.inputsystem)
  - TextMeshPro (com.unity.textmeshpro)
- **Optional Packages**:
  - 2D Sprite (for sprite-based games)
  - Audio (for sound management)

---

## Contributing

This library is designed to be extended. When adding new systems:
1. Follow the existing naming conventions
2. Use ScriptableObjects for data configuration
3. Implement singleton pattern for managers
4. Add null checks for optional dependencies
5. Document your system in the [Documentation](Documentation/README.md) folder

---

## License

[Specify your license here]

---

## Support

For questions, issues, or contributions, please [add your contact/support information here].

---

**Happy Game Making! 🎮**
