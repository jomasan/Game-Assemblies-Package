using UnityEngine;
using UnityEditor;

/// <summary>
/// Procedural Level Builder window for generating levels from prefabs and rules.
/// Placeholder for future procedural generation tools.
/// </summary>
public class SA_ProceduralLevelBuilderWindow : EditorWindow
{
    private Vector2 scrollPosition;

    [MenuItem("Game Assemblies/Environment/Procedural Level Builder")]
    public static void ShowWindow()
    {
        var window = GetWindow<SA_ProceduralLevelBuilderWindow>("Procedural Level Builder");
        window.minSize = new Vector2(400, 400);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        EditorGUILayout.Space(8);
        GUILayout.Label("Procedural Level Builder", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Tools for procedurally generating level content. Select a tool below to place prefabs, tiles, or structures in the scene.",
            MessageType.Info);
        EditorGUILayout.Space(8);

        GUILayout.Label("Planned Features", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        DrawIdea("Scatter Prefab Tool",
            "Place one or many prefab types across an area with configurable probability for each. " +
            "Use for rocks, bushes, trees, or debris scattered naturally.");

        DrawIdea("Grid of Prefabs Tool",
            "Fill a grid with prefabs. Support probability of gaps (empty cells) or random offset from the rigid grid for a less uniform look. " +
            "Use for tile floors, walls, or structured layouts.");

        DrawIdea("Room / Wave Generator",
            "Generate connected rooms or wave-based arenas with configurable size, count, and connectivity. " +
            "Use for dungeons, arenas, or multi-room levels.");

        DrawIdea("Tilemap from Noise",
            "Generate a 2D tilemap from Perlin or Simplex noise with thresholds for different tile types. " +
            "Use for terrain, caves, or organic-looking floors.");

        DrawIdea("Path / Walkway Generator",
            "Create paths or walkways between points with optional width variation and edge decoration. " +
            "Use for roads, rivers, or connecting corridors.");

        DrawIdea("Spawn Point Distribution",
            "Distribute spawn points (players, enemies, pickups) across an area with spacing rules and exclusion zones. " +
            "Use for fair player placement or balanced encounter setup.");

        DrawIdea("Boundary / Fence Generator",
            "Place prefabs along a boundary or polygon outline with optional corners and gates. " +
            "Use for walls, fences, or level boundaries.");

        DrawIdea("Layered Placement",
            "Combine multiple tools: e.g. scatter ground clutter on top of a grid floor, or place props inside generated rooms. " +
            "Use for richer, multi-pass level generation.");

        EditorGUILayout.Space(12);
        EditorGUILayout.HelpBox(
            "These features are planned for future implementation. Use the Environment menu for manual placement in the meantime.",
            MessageType.None);

        EditorGUILayout.EndScrollView();
    }

    private void DrawIdea(string title, string description)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        EditorGUILayout.LabelField(description, EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(2);
    }
}
