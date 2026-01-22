# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.5] - 2026-01-22

### Added
- **SA_AssetPathHelper** – Shared helper for resolving prefabs and assets in both package and Assets (Samples) layouts
- `FindPrefab(relativePath)` – Resolves prefabs from `Packages/...` or `Assets/...` with filename fallback
- `FindAsset<T>(relativePath)` – Same for textures and other assets (e.g. tutorial images)
- `GetAssetSearchFolders(relativeFolder)` – Returns `[Assets/..., Packages/...]` for `FindAssets`
- `EnsureAssetPathDirectories(relativePathUnderAssets)` – Creates `Assets/Simulated Assemblies/...` before `CreateAsset`/`CopyAsset`

### Changed
- **Editor scripts – path resolution**: Replaced hardcoded `Assets/Simulated Assemblies/...` paths with flexible lookup
- **SA_Menu.cs** – Uses `SA_AssetPathHelper.FindPrefab` for all prefab-based menu items
- **SA_CreatePlayersWindow.cs** – Uses `SA_AssetPathHelper.FindPrefab` for PlayerManager and Player_Drawn
- **SA_CreateResourceWindow.cs** – Template prefab and tutorial image via helper; `EnsureAssetPathDirectories` for Prefabs/Resources and Databases/Resources
- **SA_CreateLootTableWindow.cs** – Tutorial image and `FindAssets` over package + Assets; `EnsureAssetPathDirectories` for LootTables
- **SA_CreateLevelWindow.cs** – Tutorial image via helper; `EnsureAssetPathDirectories` for Databases/Levels
- **SA_CreateGoalWindow.cs** – `EnsureAssetPathDirectories` for Databases/Goals before `CreateAsset`

### Fixed
- Editor menu items and create windows now work when the package is installed as a UPM package (prefabs in `Packages/`) or when Samples are imported into `Assets/`
- "Prefab not found" and similar errors when using Game Assemblies menus in package-based projects

## [1.0.4] - 2026-01-22

### Fixed
- Fixed sample folder configuration and GUID conflicts
- Updated sample folder path to point to the complete Samples directory
- Resolved GUID conflicts in sample files to prevent import errors
- Improved sample folder structure and organization

## [1.0.3] - 2026-01-22

### Removed
- Removed legacy prefabs that are no longer needed

### Fixed
- Various bug fixes and improvements

## [1.0.2] - 2026-01-22

### Added
- Included sample content in the package
- Sample assets including 2D sprites, audio files, prefabs, materials, shaders, and example scenes
- Example games demonstrating different use cases (Murals, Salvage, Cars, Music)
- Tutorial scenes and example setups for getting started with the library

## [1.0.1] - 2026-01-21

### Fixed
- Fixed invalid GUIDs in all .meta files by generating unique, properly formatted GUIDs
- Added missing .meta files for all package assets (CHANGELOG.md, LICENSE, README.md, package.json, .gitignore, .gitattributes, and sample files)
- Resolved GUID conflicts with Unity's internal assets
- Ensured all .meta files have exactly 32-character hexadecimal GUIDs

### Changed
- Updated package structure to use GameAssemblyLab.TestPackage namespace
- Renamed all classes and files from PackageName to TestPackage
- Updated package metadata with correct company and author information

## [1.0.0] - 2026-01-21

### Added
- Initial release of Unity Package Manager template
- Complete package structure with Runtime, Editor, Tests, and Samples folders
- Assembly definition files (asmdef) for all assemblies
- Runtime script example (TestPackage.cs)
- Editor script example (TestPackageEditor.cs)
- Test examples for both runtime and editor code
- Sample folder structure with example sample
- Package manifest (package.json) with proper configuration
- Documentation (README.md) with installation instructions
- MIT License
- Changelog file
- .gitignore for Unity projects
