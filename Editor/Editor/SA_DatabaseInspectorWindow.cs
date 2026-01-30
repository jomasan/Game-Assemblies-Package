using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Editor window that lists all ScriptableObject assets of a selected database type.
/// Use the dropdown to switch between Resources, Loot Tables, Levels, etc.
/// </summary>
public class SA_DatabaseInspectorWindow : EditorWindow
{
    private int _selectedDatabaseIndex;
    private Vector2 _scrollPosition;
    private string _searchFilter = "";
    private ScriptableObject _selectedAsset;

    [MenuItem("Game Assemblies/Databases/Database Inspector")]
    public static void ShowWindow()
    {
        var window = GetWindow<SA_DatabaseInspectorWindow>("Database Inspector");
        window.minSize = new Vector2(280, 200);
    }

    private void OnEnable()
    {
        SA_DatabaseManager.ClearCache();
    }

    private void OnGUI()
    {
        EditorGUILayout.Space(8);

        // Database type dropdown
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Database Type", GUILayout.Width(80));
        int newIndex = EditorGUILayout.Popup(_selectedDatabaseIndex, GetDisplayNames());
        if (newIndex != _selectedDatabaseIndex)
        {
            _selectedDatabaseIndex = newIndex;
            _selectedAsset = null;
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(4);

        // Search filter
        var type = SA_DatabaseManager.DatabaseTypes[_selectedDatabaseIndex].Type;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Search", GUILayout.Width(80));
        _searchFilter = EditorGUILayout.TextField(_searchFilter);
        if (GUILayout.Button("Refresh", GUILayout.Width(60)))
        {
            SA_DatabaseManager.Refresh(type);
            Repaint();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(8);

        // List view
        var entries = SA_DatabaseManager.GetEntries(type);

        string filter = _searchFilter?.Trim().ToLowerInvariant() ?? "";
        var filtered = new List<ScriptableObject>();
        foreach (var entry in entries)
        {
            if (string.IsNullOrEmpty(filter) || (entry != null && entry.name.ToLowerInvariant().Contains(filter)))
                filtered.Add(entry);
        }

        EditorGUILayout.LabelField($"{filtered.Count} entries", EditorStyles.miniLabel);
        EditorGUILayout.Space(4);

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        for (int i = 0; i < filtered.Count; i++)
        {
            var asset = filtered[i];
            if (asset == null) continue;

            bool isSelected = asset == _selectedAsset;
            var bgColor = isSelected ? new Color(0.3f, 0.5f, 0.8f, 0.3f) : (i % 2 == 0 ? new Color(1, 1, 1, 0.02f) : Color.clear);
            var prevBg = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, GUILayout.Height(22));

            if (GUILayout.Button(asset.name, EditorStyles.label, GUILayout.ExpandWidth(true), GUILayout.Height(20)))
            {
                _selectedAsset = asset;
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }

            var pingContent = EditorGUIUtility.IconContent("d_Project");
            if (pingContent == null) pingContent = new GUIContent("→", "Select and ping in Project");
            else pingContent.tooltip = "Select and ping in Project";
            if (GUILayout.Button(pingContent, GUILayout.Width(24), GUILayout.Height(20)))
            {
                _selectedAsset = asset;
                Selection.activeObject = asset;
                EditorGUIUtility.PingObject(asset);
            }

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = prevBg;
        }

        EditorGUILayout.EndScrollView();
    }

    private string[] GetDisplayNames()
    {
        var types = SA_DatabaseManager.DatabaseTypes;
        var names = new string[types.Length];
        for (int i = 0; i < types.Length; i++)
            names[i] = types[i].DisplayName;
        return names;
    }
}
