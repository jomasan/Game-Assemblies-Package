# Game Assemblies - Local Multiplayer Game Library

A Unity-based game development library for creating local multiplayer games centered around resource conversion and goal achievement mechanics. Inspired by games like **Overcooked**, this library provides a comprehensive set of systems for building cooperative and competitive multiplayer experiences where players work together (or compete) to convert resources and complete objectives.

## Overview

Game Assemblies is designed to streamline the creation of local multiplayer games with a focus on:
- **Resource Management**: Convert, produce, and consume resources through interactive stations
- **Goal-Oriented Gameplay**: Create time-based or resource-based objectives for players to complete
- **Local Multiplayer**: Support for multiple players using gamepads or keyboard input
- **Modular Design**: Mix and match systems to create unique game experiences

## Core Philosophy

The library is built around the concept of **resource conversion chains** - players gather base resources, process them through stations, and deliver final products to achieve goals. This creates engaging gameplay loops where coordination, planning, and efficiency are key to success.

---

## System Index

### 🎮 [Player Systems](./docs/PlayerSystems.md)
- **Player Controller** - Core player movement, interaction, and input handling
- **Player Info Manager** - Manages player data, colors, and identification
- **Multi-Input Support** - Gamepad and keyboard input via Unity's Input System

### 📦 [Resource Management System](./docs/ResourceManagement.md)
- **Resource Manager** - Central hub for tracking all resources in the game
- **Resource Objects** - Physical items that players can grab, carry, and deliver
- **Resource Nodes** - Base class for resource-producing entities
- **Resource Pools** - Object pooling for efficient resource spawning
- **Resource UI Binding** - Connect resources to UI displays

### 🏭 [Station & Production System](./docs/StationSystem.md)
- **Station** - Interactive workstations that convert resources
- **Station Manager** - Manages station states and interactions
- **Resource Producer** - Automated resource generation
- **Resource Sink** - Consumes resources to produce outputs
- **Consume Area** - Input zones for resource delivery
- **Production Modes** - Resource, Station, or LootTable outputs

### 🎯 [Goal System](./docs/GoalSystem.md)
- **Goal Manager** - Tracks and manages active goals
- **Resource Goals** - Time-based or resource-count objectives
- **Goal Tracker UI** - Visual display of goal progress
- **Goal ScriptableObjects** - Data-driven goal configuration

### 📊 [Level Management](./docs/LevelManagement.md)
- **Level Manager** - Controls level progression and timing
- **Level Data** - ScriptableObject-based level configuration
- **Sequential & Random Goals** - Different goal spawning patterns
- **Score Brackets** - Star rating system based on performance

### 🎨 [Game Management](./docs/GameManagement.md)
- **Game Manager** - State machine for game flow (Menu, Playing, Paused, Results)
- **Creation Manager** - Handles object spawning and creation
- **Soundtrack Manager** - Manages background music and audio

### 🗺️ [Area & Region System](./docs/AreaSystem.md)
- **Area** - Spatial zones for triggering events
- **Grab Region** - Detection zones for object interaction
- **Region Events** - Event-driven area interactions

### 🎲 [Loot & Randomization](./docs/LootSystem.md)
- **Loot Tables** - Weighted random resource generation
- **Random Populate** - Spawn objects randomly within bounds

### 🎨 [Visual & UI Systems](./docs/VisualSystems.md)
- **Progress Bar Controller** - Visual feedback for work progress
- **Info Window** - Display information panels
- **Dynamic Sorting Order** - 2D depth sorting for sprites
- **Color Palette System** - Themed color management

### 🔧 [Utility Systems](./docs/UtilitySystems.md)
- **Tag System** - MultiTag component for flexible object tagging
- **Tween System** - Animation and easing functions
- **Camera Shake** - Screen effects
- **Countdown Timer** - Time-based mechanics
- **Object Spawner** - Generic spawning system

---

## Quick Start

1. **Import the Library**: Add the `Assets/Simulated Assemblies` folder to your Unity project
2. **Set Up Players**: Add `playerController` components to your player GameObjects
3. **Create Resources**: Define resources in the ResourceManager ScriptableObject
4. **Build Stations**: Place Station prefabs and configure their input/output requirements
5. **Define Goals**: Create ResourceGoal ScriptableObjects for your objectives
6. **Configure Levels**: Set up LevelData ScriptableObjects with your goal sequences

For detailed setup instructions, see the [Getting Started Guide](./docs/GettingStarted.md).

---

## Example Games

The library includes several example games demonstrating different use cases:

- **Murals** - Art creation game
- **Salvage** - Resource collection and processing
- **Cars** - Vehicle assembly mechanics
- **Music** - Audio production gameplay

These examples can be found in `Assets/Simulated Assemblies/Detroit At Play Games/`.

---

## Key Features

✅ **Local Multiplayer Ready** - Built-in support for multiple players with gamepad/keyboard  
✅ **Modular Architecture** - Use only the systems you need  
✅ **ScriptableObject Driven** - Data-driven design for easy iteration  
✅ **Extensible** - Easy to add custom behaviors and systems  
✅ **UI Integration** - Pre-built UI components for common needs  
✅ **Event-Driven** - UnityEvents for flexible system communication  

---

## Documentation Structure

Each system has detailed documentation covering:
- **Overview** - What the system does and when to use it
- **Components** - Key scripts and their purposes
- **Setup Guide** - Step-by-step configuration
- **API Reference** - Important methods and properties
- **Examples** - Common use cases and patterns

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
5. Document your system in the appropriate docs folder

---

## License

[Specify your license here]

---

## Support

For questions, issues, or contributions, please [add your contact/support information here].

---

**Happy Game Making! 🎮**
