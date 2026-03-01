# UI Composer — Editor Window

## Overview

A Unity Editor window that lets designers and developers compose in-game UI by placing prebuilt, design-system-compliant prefabs (for example: text score, icon+counter, progress bar, info panel) onto a canvas. The tool streamlines creating HUD and in-world UI by providing a palette of components, WYSIWYG placement, and automatic wiring to game systems (score, inventory, stats, etc.).

## Goals

- Enable non-programmers to assemble UI quickly using an established design system.
- Ensure generated UI respects visual tokens (spacing, colors, typography).
- Auto-wire created elements to runtime systems via configurable connectors.
- Produce reusable prefabs and serialized layout assets.

## Core Concepts

- Design System: A set of visual tokens and prefab variants (sizes, color modes, accessibility variants).
- Component Prefabs: Small, focused prefabs that represent a UI primitive or pattern:
  - Text Score (bindable numeric text with prefix/suffix)
  - Icon + Counter (icon sprite with numeric badge)
  - Progress Bar (value, min/max, label)
  - Info Panel (title, body, optional icon)
- Composer Canvas: Editor-only layout surface for placement and preview of elements.
- Connectors: Editor-side adapters that record what runtime system and property an element binds to (e.g., `ScoreSystem/Player/TotalScore`).

## Editor Window UI & Workflow

- Palette (left): list of available prefabs, categorized by purpose. Each shows design-system variant previews.
- Canvas (center): drag-and-drop placement, snap-to-grid and anchor presets (anchored-to-corners, screen-relative positions).
- Properties (right): selected element inspector exposing design tokens, binding configuration, and preview toggles.
- Top Bar: create new layout, save layout asset, load layout, play-preview toggle.

Workflow:
1. Open `UI Composer` window.
2. Choose a prefab from Palette and drag it onto Canvas.
3. Configure visual variant and binding in Properties.
4. Save layout as a ScriptableObject or prefab in a selected folder.
5. At runtime, a loader component instantiates and wires elements to the appropriate systems.

## Automatic Wiring

- Each prefab exposes a small interface (`IUIBindable`) and an Editor connector that records the binding path and optional transformation (format strings, unit conversions).
- The runtime loader reads binding metadata from the saved layout and subscribes to the target system's events or properties.
- Provide a set of built-in connector adapters (ScoreConnector, InventoryConnector, StatConnector) and a fallback: `CustomMethodConnector` (executes a named method on a known GameObject).

## Persistence & Prefab Management

- Layouts saved as `UIComposition` ScriptableObjects that reference prefab instances and binding metadata.
- Option to bake compositions into a single prefab for easy runtime instantiation.

## Implementation Notes

- Editor code should live under `Editor/` and use `Handles`/IMGUI or UIElements for the window UI.
- Keep runtime binding code separate and lightweight; Editor only writes metadata (strings, GUIDs, enums).
- Support schema versioning on saved `UIComposition` assets for future migration.

## API Surface (sketch)

- Runtime:
  - `IUIBindable` interface with `Bind(IBindingContext)`
  - `UICompositionLoader` component to instantiate and bind compositions
- Editor:
  - `UIComposerWindow` EditorWindow
  - `PrefabPalette` scriptable to declare available prefab variants
  - `BindingConnector` Editor utility to pick targets and serialize binding info

## Example: Icon + Counter binding

- Designer drags `Icon+Counter` prefab onto Canvas.
- In Properties, chooses `Inventory/Ammo/Primary` from a connector picker.
- The saved composition notes: prefab GUID, local transform, connector: `InventoryConnector`, path `Ammo.Primary.Count`.
- At runtime, `UICompositionLoader` instantiates the prefab, finds `InventoryConnector` on the runtime adapter, and subscribes the counter to value changes.

## Risks & Considerations

- Binding by string paths is fragile—provide validation and a runtime-safe adapter registry.
- Keep performance in mind for HUDs that subscribe to frequent events; offer throttling or value-diffing.
- Accessibility: ensure variants include large-text and high-contrast tokens.

## Next Steps (implementation-first)

- Create `PrefabPalette` ScriptableObject and example prefabs for `Text Score` and `Icon+Counter`.
- Scaffold `UIComposerWindow` with a simple Palette and Canvas (drag & drop placement, static preview).
- Implement basic `BindingConnector` editor UI and `UIComposition` asset saving.

---

Last updated: 2026-03-01
# UI Composer — Editor Window

## Overview

A Unity Editor window that lets designers and developers compose in-game UI by placing prebuilt, design-system-compliant prefabs (for example: text score, icon+counter, progress bar, info panel) onto a canvas. The tool streamlines creating HUD and in-world UI by providing a palette of components, WYSIWYG placement, and automatic wiring to game systems (score, inventory, stats, etc.).

## Goals

- Enable non-programmers to assemble UI quickly using an established design system.
- Ensure generated UI respects visual tokens (spacing, colors, typography).
- Auto-wire created elements to runtime systems via configurable connectors.
- Produce reusable prefabs and serialized layout assets.

## Core Concepts

- Design System: A set of visual tokens and prefab variants (sizes, color modes, accessibility variants).
- Component Prefabs: Small, focused prefabs that represent a UI primitive or pattern:
  - Text Score (bindable numeric text with prefix/suffix)
  - Icon + Counter (icon sprite with numeric badge)
  - Progress Bar (value, min/max, label)
  - Info Panel (title, body, optional icon)
- Composer Canvas: Editor-only layout surface for placement and preview of elements.
- Connectors: Editor-side adapters that record what runtime system and property an element binds to (e.g., `ScoreSystem/Player/TotalScore`).

## Editor Window UI & Workflow

- Palette (left): list of available prefabs, categorized by purpose. Each shows design-system variant previews.
- Canvas (center): drag-and-drop placement, snap-to-grid and anchor presets (anchored-to-corners, screen-relative positions).
- Properties (right): selected element inspector exposing design tokens, binding configuration, and preview toggles.
- Top Bar: create new layout, save layout asset, load layout, play-preview toggle.

Workflow:
1. Open `UI Composer` window.
2. Choose a prefab from Palette and drag it onto Canvas.
3. Configure visual variant and binding in Properties.
4. Save layout as a ScriptableObject or prefab in a selected folder.
5. At runtime, a loader component instantiates and wires elements to the appropriate systems.

## Automatic Wiring

- Each prefab exposes a small interface (`IUIBindable`) and an Editor connector that records the binding path and optional transformation (format strings, unit conversions).
- The runtime loader reads binding metadata from the saved layout and subscribes to the target system's events or properties.
- Provide a set of built-in connector adapters (ScoreConnector, InventoryConnector, StatConnector) and a fallback: `CustomMethodConnector` (executes a named method on a known GameObject).

## Persistence & Prefab Management

- Layouts saved as `UIComposition` ScriptableObjects that reference prefab instances and binding metadata.
- Option to bake compositions into a single prefab for easy runtime instantiation.

## Implementation Notes

- Editor code should live under `Editor/` and use `Handles`/IMGUI or UIElements for the window UI.
- Keep runtime binding code separate and lightweight; Editor only writes metadata (strings, GUIDs, enums).
- Support schema versioning on saved `UIComposition` assets for future migration.

## API Surface (sketch)

- Runtime:
  - `IUIBindable` interface with `Bind(IBindingContext)`
  - `UICompositionLoader` component to instantiate and bind compositions
- Editor:
  - `UIComposerWindow` EditorWindow
  - `PrefabPalette` scriptable to declare available prefab variants
  - `BindingConnector` Editor utility to pick targets and serialize binding info

## Example: Icon + Counter binding

- Designer drags `Icon+Counter` prefab onto Canvas.
- In Properties, chooses `Inventory/Ammo/Primary` from a connector picker.
- The saved composition notes: prefab GUID, local transform, connector: `InventoryConnector`, path `Ammo.Primary.Count`.
- At runtime, `UICompositionLoader` instantiates the prefab, finds `InventoryConnector` on the runtime adapter, and subscribes the counter to value changes.

## Risks & Considerations

- Binding by string paths is fragile—provide validation and a runtime-safe adapter registry.
- Keep performance in mind for HUDs that subscribe to frequent events; offer throttling or value-diffing.
- Accessibility: ensure variants include large-text and high-contrast tokens.

## Next Steps (implementation-first)

- Create `PrefabPalette` ScriptableObject and example prefabs for `Text Score` and `Icon+Counter`.
- Scaffold `UIComposerWindow` with a simple Palette and Canvas (drag & drop placement, static preview).
- Implement basic `BindingConnector` editor UI and `UIComposition` asset saving.

---

Last updated: 2026-03-01
