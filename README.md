# Game Assemblies

Brief description of your Unity package.

## Installation

### Via Git URL (Unity 2019.3+)

1. Open Unity Package Manager (Window > Package Manager)
2. Click the **+** button
3. Select **Add package from git URL...**
4. Enter the Git URL: `https://github.com/yourusername/your-repo.git`

### Via Local Path

1. Open Unity Package Manager (Window > Package Manager)
2. Click the **+** button
3. Select **Add package from disk...**
4. Navigate to the package folder and select it

### Via UPM Registry

If you have a private registry, add it to your project's `Packages/manifest.json`:

```json
{
  "scopedRegistries": [
    {
      "name": "Your Registry",
      "url": "https://your-registry-url.com",
      "scopes": ["com.gameassemblylab"]
    }
  ],
  "dependencies": {
    "com.gameassemblylab.testpackage": "1.0.0"
  }
}
```

## Features

- Feature 1
- Feature 2
- Feature 3

## Usage

### Basic Usage

```csharp
// Example code snippet
```

### Advanced Usage

```csharp
// More complex example
```

## Requirements

- Unity 2021.3 or later
- (Add any other requirements)

## Documentation

For detailed documentation, see [Documentation URL]

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## Support

For support, email jsanchez@cornell.edu or open an issue in the repository.
