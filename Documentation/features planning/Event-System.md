# Feature Planning: Event System

## Overview

The **Event System** allows **dynamic events** to enter the game and affect a wide range of variables and behaviors. Events can change policies, player speed, recipe requirements or outputs, decay speed, and other parameters, enabling time-based or condition-based variety (e.g., “double decay this round,” “everyone moves slower,” “this station now produces 2x output”).

Events have:

- **Trigger** — When the event starts: either an **explicit condition** (e.g., time elapsed, score threshold, goal completed) or **triggered by another system** (e.g., an Event Manager that fires events on a schedule or in response to game state).
- **Modality** — **Permanent** (effect lasts until explicitly removed or overwritten) or **with duration** (effect lasts for a set time, then ends).
- **Presentation** — **Name** and **icon** for UI; eventually a **card system** for a graphic presentation of events.

This document outlines the desired behavior, scope, and integration points. It is intended to work with the [Policy Manager](Policy-Manager.md), [Resource Ownership](Resource-Ownership.md), stations, recipes (see [Recipe System](Recipe-System.md)), and other systems that expose modifiable parameters.

---

## 2. Scope and Design

### 2.1 What events can affect

Events should be able to influence (directly or via modifiers) variables such as:

- **Policies** — Switch or override the active policy (e.g., “this round: communal ownership” or “stealing allowed”).
- **Player speed** — Global or per-player movement speed multiplier.
- **Recipe inputs/outputs** — Resources required or produced by recipes (e.g., “half inputs” or “double output” for a recipe or station).
- **Decay speed** — Rate at which decayable resources age or expire.
- **Other** — Capital rewards, station work duration, spawn rates, etc., as the codebase exposes them.

Implementation can be modifier-based (e.g., “current speed = base speed × event multiplier”) or direct override (e.g., “event sets policy to X for its duration”).

### 2.2 Triggering

- **Explicit conditions** — Event defines when it triggers: e.g., “after 60 seconds,” “when score reaches 100,” “when goal N is completed,” “when resource count of type R exceeds K.” A runtime component evaluates these conditions and starts the event.
- **Triggered by other systems** — An **Event Manager** (or similar) can fire events on a schedule, from a list, or in response to signals (e.g., level start, wave start, narrative beat). Events may also be triggered by other events (chaining).

### 2.3 Modalities

- **Permanent** — Once triggered, the event’s effect stays until something removes it (e.g., another event, level end, or explicit “clear event” call). Useful for “law change” or “permanent modifier” style events.
- **With duration** — Event has a duration (e.g., 30 seconds); when the duration ends, the effect is reverted (or stacked with other events depending on design). Useful for temporary buffs/debuffs.

### 2.4 Presentation

- **Name** — Short title for the event (e.g., “Double output,” “Slow motion”).
- **Icon** — Sprite or image for UI and cards.
- **Card system (future)** — A graphic presentation layer: events are shown as cards (hand, queue, or active stack) with name, icon, and optional description or duration. The event system itself can remain data/logic-only; the card system is a separate presentation layer that reads active events and displays them.

### 2.5 Relation to other systems

- **Policy Manager** — Events may push a temporary or permanent policy override; Policy Manager (or a wrapper) can resolve “current effective policy” from active events + base policy.
- **Stations / Recipe System** — Events may apply modifiers to recipe inputs/outputs or work speed so that the same station behaves differently under different events.
- **Resources / decay** — Events may scale decay rate or other resource parameters.
- **Goals / scoring** — Events may modify rewards, penalties, or what counts toward goals.

---

## 3. Current State

- **Event data model (implemented)** — `EventSO` ScriptableObject with name, icon, description, trigger type and parameters (TriggeredByManager, TimeElapsed, ScoreThreshold, GoalCompleted, ResourceCountExceeds), modality (Permanent / WithDuration + durationSeconds), and a list of `EventEffect` entries.
- **Effect types (implemented)** — `EventEffect` with `EventEffectType` enum: ChangePolicy, MultiplySpeed, ScaleRecipeInputs, ScaleRecipeOutputs, ScaleDecay, ScaleWorkDuration, Other; parameters include effectParamFloat, optional effectPolicy / effectRecipe / effectStation for targeting.
- **Event Manager (implemented)** — `EventManager` runtime component (singleton): optional `conditionEvents` list, `FireEvent(EventSO)` / `RemoveEvent(EventSO)` / `GetActiveEvents()` / `IsActive(EventSO)`; applies and removes events by modality (duration-based events auto-expire in Update); condition evaluation implemented only for **TimeElapsed** (Score/Goal/ResourceCount triggers are stubs for later integration).
- **Editor tools (implemented)** — **Game Assemblies > Events > Create Event** window creates `EventSO` assets and saves to `Game Assemblies/Databases/Events`; **Database Inspector** includes an "Events" database type so events can be listed, viewed, and edited in the inspector.
- **Integration points (not yet implemented)** — Policy Manager, playerController (speed), stations/recipes, and decay logic do not yet read from `EventManager.GetActiveEvents()` or apply modifiers; the data and manager are in place for that wiring.
- **Card system** — Not implemented; can be built once integration is done.

---

## 4. To-Do / Next Steps

- [x] **Define Event data model** — ScriptableObject or asset type for an Event: name, icon, trigger (condition or “triggered by manager”), modality (permanent vs duration), and a list or structure of “effects” (what variables to change and how). *(Done: `EventSO`, `EventEffect`, trigger/modality enums.)*
- [x] **Define effect types** — How events express “change policy,” “multiply speed,” “scale recipe inputs/outputs,” “scale decay,” etc. *(Done: `EventEffectType` enum + `EventEffect` with params and optional policy/recipe/station refs.)*
- [x] **Event Manager** — Runtime component that holds or references events, evaluates explicit conditions, and/or fires events on a schedule or when signaled; applies and removes effects based on modality (duration vs permanent). *(Done: `EventManager`; only TimeElapsed condition is evaluated; others are stubs.)*
- [ ] **Integration points** — Policy Manager, playerController (speed), stations/recipes, decay logic, etc., read from “current active events” or “effective modifiers” and apply the combined result.
- [x] **Editor tools** — Event creator/editor (e.g. Game Assemblies > Events > Create Event), store events in a Databases or similar folder; optional list/view of events for level or game design. *(Done: Create Event window; Events in Database Inspector.)*
- [ ] **Card system (later)** — UI layer that displays active (or available) events as cards with name, icon, and optional description/duration; can be built once the event data and manager exist.

---

## 5. Success Criteria

- Events are first-class assets with name, icon, trigger, and modality (permanent or duration).
- Events can be triggered by explicit conditions or by an Event Manager (or other systems).
- Events can affect at least: policies, player speed, recipe-related inputs/outputs, and decay speed (with room to add more).
- Active events are visible to the rest of the game (e.g., via Event Manager or a central “active events” list) so systems can apply modifiers or overrides.
- A path exists to a future card-based presentation of events without changing the core event data or trigger logic.
