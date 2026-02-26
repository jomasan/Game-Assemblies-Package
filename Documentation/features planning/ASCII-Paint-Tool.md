# Feature Planning: ASCII Paint Tool (Editor Window)

## Overview

The **ASCII Paint Tool** is an editor window that works as a **character-based painting tool** for creating graphic assets. The user selects characters from a character list and a font (e.g. **Unifont** or other fixed-width bitmap fonts), then “paints” characters onto a fixed-size canvas. The result can be exported as a **sprite** with a user-defined name, suitable for use as icons, tiles, or pixel-art-style graphics in the game.

Key capabilities:

- **Character list** — User can choose which character(s) to paint with (e.g. pick from a predefined set or full Unicode range, filtered by font support).
- **Font selection** — Support for fixed-width / bitmap fonts such as Unifont so that each character occupies a consistent cell size and the canvas aligns to a grid.
- **Configurable canvas** — User defines **rows** and **columns** (e.g. 8×8, 16×16, 32×16). Canvas size is adaptable to the chosen dimensions and font metrics.
- **Painting** — Click or drag to place the selected character in a cell; optional erase/clear, fill, or secondary color for foreground/background.
- **Export** — When painting is done, the user can **create a sprite** from the canvas and give it a **name**; the asset is saved (e.g. to a designated folder such as `Game Assemblies/Sprites` or a user-chosen path).

---

## 2. Scope and Design

### 2.1 Editor window

- **Menu entry** — e.g. **Game Assemblies > Asset Tools > ASCII Paint** (or under a dedicated “ASCII Paint” submenu).
- **Layout** — Toolbar (font, character picker, rows/columns, export); canvas area (grid of cells); optional side panel for character list or palette.

### 2.2 Character list and font

- **Font** — User selects a font asset (e.g. Unifont, or any Unity Font). The tool uses the font’s character info and texture to render characters in the canvas. **Fixed-width / monospace** fonts are recommended so that each character fits one cell.
- **Character list** — A set of characters the user can paint with. Options:
  - **Predefined set** — e.g. block/symbol set (█ ░ ▒ ▓ · etc.) or alphanumeric.
  - **Font-supported subset** — Characters that the selected font actually contains (from font’s character table).
  - **Unicode range** — Optional: allow picking from a range (e.g. U+2580–U+259F for block elements) with a preview.
- **Current brush** — One (or more) selected character used when painting; optionally foreground/background color if the font or rendering supports it.

### 2.3 Canvas

- **Rows and columns** — User-defined (e.g. 8×8, 16×16, 32×16). The canvas displays a grid of that many cells; each cell holds one character (and optionally color).
- **Adaptable size** — The canvas area size (in pixels) can scale to fit the window or be fixed; cell size is derived from font metrics (e.g. character width × columns, line height × rows) so that the final sprite dimensions are predictable.
- **Painting** — Click/drag to set a cell to the current brush character; optional tools: erase (clear cell or set to space), fill, undo/redo if needed.
- **Data model** — Canvas state is a 2D array (or grid) of character + optional color per cell; this is what gets rendered and then converted to a texture/sprite.

### 2.4 Export: create a sprite with a name

- **Export action** — Button or menu: “Create Sprite” / “Export Sprite.”
- **Name** — User enters a name for the sprite asset; used as the filename (e.g. `MyIcon.png` or `MyIcon.asset` depending on output type).
- **Output** — Render the canvas to a **Texture2D** (character glyphs drawn into the texture using the selected font), then create a **Sprite** (e.g. via `Sprite.Create`) and save it as an asset. Save path could be configurable (e.g. `Game Assemblies/Sprites/{name}.png` and corresponding .meta, or a single texture asset with sprite sub-asset).
- **Result** — The user gets a named sprite asset they can assign to UI, resources, tiles, etc.

### 2.5 Relation to existing tools

- Fits alongside **Pixel Art Converter**, **Crop Image**, **Resample Colors** under **Game Assemblies > Asset Tools** as another way to produce or edit graphic assets.
- Output sprites can be used by **Resource Builder**, **Station Builder**, **Goal** icons, and other systems that reference sprites.

---

## 3. Current State

- **Not implemented.** There is no ASCII paint editor window, no character-grid canvas, and no export-to-sprite-from-canvas flow in the package.

---

## 4. To-Do / Next Steps

- [ ] **Editor window shell** — Create `SA_ASCIIPaintWindow` (or similar) with menu item, toolbar, and a resizable canvas area.
- [ ] **Font selection** — Dropdown or object field to select a Unity Font; load font texture and character metrics for rendering.
- [ ] **Character list / picker** — UI to choose which character(s) to paint with (e.g. list of block/symbol chars, or subset of font’s supported characters). Optional: Unicode range selector.
- [ ] **Canvas model** — 2D grid (rows × columns) storing character (and optionally color) per cell; initialize with spaces or default character.
- [ ] **Canvas UI** — Draw the grid in the editor window; handle mouse input to paint (and optionally erase, fill). Scale or scroll so the canvas fits the window.
- [ ] **Rendering to texture** — Given the grid and font, render each cell’s character into a Texture2D using the font’s material/texture (e.g. character lookup + blit or Unity’s text rendering). Ensure pixel-perfect or crisp result for sprite export.
- [ ] **Create sprite with name** — “Create Sprite” flow: user enters name, choose save path (or use default), generate texture from canvas, create Sprite, save asset (e.g. PNG + import as sprite, or Texture2D asset with sprite). Ensure correct pivot and pixels-per-unit for the sprite.
- [ ] **Optional** — Foreground/background color per cell; undo/redo; load existing image/sprite and convert to character grid (reverse path) for editing.

---

## 5. Success Criteria

- User can open an ASCII Paint editor window from the menu.
- User can select a font (e.g. Unifont) and a character list (or brush character).
- User can set canvas rows and columns; the canvas is displayed as a grid and adapts to the chosen size.
- User can paint characters onto the canvas by clicking/dragging.
- User can create a named sprite from the current canvas and save it as an asset for use in the project.
