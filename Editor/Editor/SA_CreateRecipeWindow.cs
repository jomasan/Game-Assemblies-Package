using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SA_CreateRecipeWindow : EditorWindow
{
    private string recipeName = "New Recipe";
    private Sprite icon;
    private float workDurationOverride;
    private List<RecipeSlot> inputs = new List<RecipeSlot>();
    private List<RecipeOutputSlot> outputs = new List<RecipeOutputSlot>();
    private Vector2 _scrollPosition;

    [MenuItem("Game Assemblies/Recipes/Create Recipe")]
    public static void ShowWindow()
    {
        GetWindow<SA_CreateRecipeWindow>("Create Recipe");
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        GUILayout.Label("Create a New Recipe", EditorStyles.boldLabel);
        GUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Inputs are always resources. Each output line is either a resource (with amount) or a station (spawns from Station Data prefab when the recipe runs).",
            MessageType.Info);
        GUILayout.Space(12);

        recipeName = EditorGUILayout.TextField("Recipe Name", recipeName);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon (optional)", icon, typeof(Sprite), false);
        workDurationOverride = EditorGUILayout.FloatField("Work Duration Override (0 = use station default)", Mathf.Max(0f, workDurationOverride));
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Inputs", EditorStyles.boldLabel);
        DrawInputSlotList(inputs);
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Outputs", EditorStyles.boldLabel);
        DrawOutputSlotList(outputs);
        GUILayout.Space(20);

        if (GUILayout.Button("Create"))
        {
            CreateRecipe();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawInputSlotList(List<RecipeSlot> list)
    {
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) list[i] = new RecipeSlot();
            EditorGUILayout.BeginHorizontal();
            list[i].resource = (Resource)EditorGUILayout.ObjectField(list[i].resource, typeof(Resource), false, GUILayout.Width(180));
            list[i].amount = Mathf.Max(1, EditorGUILayout.IntField(list[i].amount, GUILayout.Width(40)));
            if (GUILayout.Button("−", GUILayout.Width(22)))
            {
                list.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        if (GUILayout.Button("+ Add input"))
        {
            list.Add(new RecipeSlot { amount = 1 });
        }
    }

    private void DrawOutputSlotList(List<RecipeOutputSlot> list)
    {
        if (list == null) return;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == null) list[i] = new RecipeOutputSlot();
            RecipeOutputSlot s = list[i];

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            s.kind = (RecipeOutputKind)EditorGUILayout.EnumPopup("Output type", s.kind);
            s.amount = Mathf.Max(1, EditorGUILayout.IntField("Amount", s.amount));

            if (s.kind == RecipeOutputKind.Resource)
                s.resource = (Resource)EditorGUILayout.ObjectField("Resource", s.resource, typeof(Resource), false);
            else
                s.stationData = (StationDataSO)EditorGUILayout.ObjectField("Station data", s.stationData, typeof(StationDataSO), false);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                list.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }
        if (GUILayout.Button("+ Add output"))
        {
            list.Add(new RecipeOutputSlot { kind = RecipeOutputKind.Resource, amount = 1 });
        }
    }

    private void CreateRecipe()
    {
        if (string.IsNullOrWhiteSpace(recipeName))
        {
            Debug.LogWarning("Create Recipe: Enter a recipe name.");
            return;
        }

        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Databases/Recipes");

        RecipeSO asset = ScriptableObject.CreateInstance<RecipeSO>();
        asset.recipeName = recipeName;
        asset.icon = icon;
        asset.workDurationOverride = workDurationOverride;
        asset.inputs = new List<RecipeSlot>();
        asset.outputs = new List<RecipeOutputSlot>();
        foreach (var s in inputs)
        {
            if (s?.resource != null)
                asset.inputs.Add(new RecipeSlot { resource = s.resource, amount = Mathf.Max(1, s.amount) });
        }
        foreach (var s in outputs)
        {
            if (s == null) continue;
            if (s.kind == RecipeOutputKind.Resource && s.resource != null)
                asset.outputs.Add(new RecipeOutputSlot { kind = RecipeOutputKind.Resource, resource = s.resource, amount = Mathf.Max(1, s.amount), stationData = null });
            else if (s.kind == RecipeOutputKind.Station && s.stationData != null)
                asset.outputs.Add(new RecipeOutputSlot { kind = RecipeOutputKind.Station, resource = null, amount = Mathf.Max(1, s.amount), stationData = s.stationData });
        }

        string assetPath = $"Assets/Game Assemblies/Databases/Recipes/{recipeName}.asset";
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);

        Debug.Log("Recipe created: " + recipeName);
    }
}
