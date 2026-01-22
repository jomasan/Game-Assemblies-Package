# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
