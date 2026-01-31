using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Procedural Level Builder window for generating levels from prefabs and rules.
/// </summary>
public class SA_ProceduralLevelBuilderWindow : EditorWindow
{
    private Vector2 scrollPosition;
    private int _selectedToolIndex;

    // Scatter Prefab Tool
    private List<ScatterEntry> _scatterEntries = new List<ScatterEntry>();
    private int _scatterTotalInstances = 50;
    private AxisPlane _scatterAxisPlane = AxisPlane.XY;
    private float _scatterWidth = 10f;
    private float _scatterHeight = 10f;
    private Vector3 _scatterCenter = Vector3.zero;

    private enum AxisPlane { XY, XZ, YZ }

    [System.Serializable]
    private class ScatterEntry
    {
        public GameObject prefab;
        public float percent = 50f;
    }

    [MenuItem("Game Assemblies/Environment/Procedural Level Builder")]
    public static void ShowWindow()
    {
        var window = GetWindow<SA_ProceduralLevelBuilderWindow>("Procedural Level Builder");
        window.minSize = new Vector2(400, 500);
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

        // Tool selector
        EditorGUILayout.LabelField("Tools", EditorStyles.boldLabel);
        string[] toolNames = { "Scatter Prefab", "Grid of Prefabs (planned)", "Room / Wave (planned)", "Tilemap from Noise (planned)", "Path / Walkway (planned)", "Spawn Points (planned)", "Boundary / Fence (planned)", "Layered Placement (planned)" };
        _selectedToolIndex = EditorGUILayout.Popup("Active Tool", _selectedToolIndex, toolNames);
        EditorGUILayout.Space(8);

        if (_selectedToolIndex == 0)
        {
            DrawScatterPrefabTool();
        }
        else
        {
            DrawPlannedPlaceholder();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawScatterPrefabTool()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.LabelField("Scatter Prefab Tool", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Place prefabs across an area with configurable probability for each. Use for rocks, bushes, trees, or debris.",
            MessageType.None);
        EditorGUILayout.Space(4);

        // Prefabs to scatter (List) with % for each
        EditorGUILayout.LabelField("Prefabs to Scatter", EditorStyles.boldLabel);
        for (int i = 0; i < _scatterEntries.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            _scatterEntries[i].prefab = (GameObject)EditorGUILayout.ObjectField(_scatterEntries[i].prefab, typeof(GameObject), false, GUILayout.Width(180));
            EditorGUILayout.LabelField("%", GUILayout.Width(14));
            _scatterEntries[i].percent = Mathf.Max(0f, EditorGUILayout.FloatField(_scatterEntries[i].percent, GUILayout.Width(50)));
            if (GUILayout.Button("−", GUILayout.Width(22)))
            {
                _scatterEntries.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+ Add Prefab"))
        {
            _scatterEntries.Add(new ScatterEntry { prefab = null, percent = 50f });
        }
        EditorGUILayout.Space(4);

        // Total number of instances
        _scatterTotalInstances = Mathf.Max(1, EditorGUILayout.IntField("Total Instances", _scatterTotalInstances));
        EditorGUILayout.Space(4);

        // Axis plane
        _scatterAxisPlane = (AxisPlane)EditorGUILayout.EnumPopup("Axis Plane", _scatterAxisPlane);
        EditorGUILayout.Space(4);

        // Bounds (-width, width, -height, height)
        EditorGUILayout.LabelField("Bounds", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Range: X from -Width to +Width, Y/Z from -Height to +Height (based on plane).", MessageType.None);
        EditorGUILayout.BeginHorizontal();
        _scatterWidth = Mathf.Max(0.01f, EditorGUILayout.FloatField("Width (±)", _scatterWidth));
        _scatterHeight = Mathf.Max(0.01f, EditorGUILayout.FloatField("Height (±)", _scatterHeight));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);

        // Center
        _scatterCenter = EditorGUILayout.Vector3Field("Center", _scatterCenter);
        EditorGUILayout.Space(8);

        // Scatter button
        bool canScatter = _scatterEntries.Count > 0 && _scatterEntries.Exists(e => e.prefab != null);
        EditorGUI.BeginDisabledGroup(!canScatter);
        if (GUILayout.Button("Scatter Prefabs", GUILayout.Height(28)))
        {
            ExecuteScatter();
        }
        EditorGUI.EndDisabledGroup();
        if (!canScatter)
        {
            EditorGUILayout.HelpBox("Add at least one prefab to scatter.", MessageType.Warning);
        }

        EditorGUILayout.EndVertical();
    }

    private void ExecuteScatter()
    {
        var validEntries = new List<ScatterEntry>();
        float totalPercent = 0f;
        foreach (var e in _scatterEntries)
        {
            if (e.prefab != null && e.percent > 0f)
            {
                validEntries.Add(e);
                totalPercent += e.percent;
            }
        }
        if (validEntries.Count == 0)
        {
            Debug.LogWarning("Procedural Level Builder: No valid prefabs to scatter.");
            return;
        }
        if (totalPercent <= 0f)
        {
            Debug.LogWarning("Procedural Level Builder: Total percent must be > 0.");
            return;
        }

        var parent = new GameObject("Scattered Prefabs");
        Undo.RegisterCreatedObjectUndo(parent, "Scatter Prefabs");

        for (int i = 0; i < _scatterTotalInstances; i++)
        {
            float r = Random.Range(0f, totalPercent);
            GameObject chosenPrefab = null;
            foreach (var e in validEntries)
            {
                r -= e.percent;
                if (r <= 0f)
                {
                    chosenPrefab = e.prefab;
                    break;
                }
            }
            if (chosenPrefab == null) chosenPrefab = validEntries[validEntries.Count - 1].prefab;

            float a = Random.Range(-_scatterWidth, _scatterWidth);
            float b = Random.Range(-_scatterHeight, _scatterHeight);
            Vector3 pos = _scatterCenter;
            switch (_scatterAxisPlane)
            {
                case AxisPlane.XY: pos.x += a; pos.y += b; break;
                case AxisPlane.XZ: pos.x += a; pos.z += b; break;
                case AxisPlane.YZ: pos.y += a; pos.z += b; break;
            }

            var instance = (GameObject)PrefabUtility.InstantiatePrefab(chosenPrefab);
            instance.transform.SetParent(parent.transform);
            instance.transform.position = pos;
            Undo.RegisterCreatedObjectUndo(instance, "Scatter Prefabs");
        }

        Selection.activeGameObject = parent;
        EditorGUIUtility.PingObject(parent);
        Debug.Log($"Procedural Level Builder: Scattered {_scatterTotalInstances} prefabs.");
    }

    private void DrawPlannedPlaceholder()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        EditorGUILayout.HelpBox("This tool is planned for future implementation.", MessageType.Info);
        EditorGUILayout.EndVertical();
    }
}
