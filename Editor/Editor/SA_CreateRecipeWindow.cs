using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SA_CreateRecipeWindow : EditorWindow
{
    private string recipeName = "New Recipe";
    private Sprite icon;
    private float workDurationOverride;
    private List<RecipeSlot> inputs = new List<RecipeSlot>();
    private List<RecipeSlot> outputs = new List<RecipeSlot>();
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
            "A recipe defines one alternative method of production: a list of inputs (resources consumed) and outputs (resources produced). Stations can reference multiple recipes and choose one as active.",
            MessageType.Info);
        GUILayout.Space(12);

        recipeName = EditorGUILayout.TextField("Recipe Name", recipeName);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon (optional)", icon, typeof(Sprite), false);
        workDurationOverride = EditorGUILayout.FloatField("Work Duration Override (0 = use station default)", Mathf.Max(0f, workDurationOverride));
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Inputs", EditorStyles.boldLabel);
        DrawSlotList(inputs);
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Outputs", EditorStyles.boldLabel);
        DrawSlotList(outputs);
        GUILayout.Space(20);

        if (GUILayout.Button("Create"))
        {
            CreateRecipe();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawSlotList(List<RecipeSlot> list)
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
        if (GUILayout.Button("+ Add slot"))
        {
            list.Add(new RecipeSlot { amount = 1 });
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
        asset.outputs = new List<RecipeSlot>();
        foreach (var s in inputs)
        {
            if (s?.resource != null)
                asset.inputs.Add(new RecipeSlot { resource = s.resource, amount = Mathf.Max(1, s.amount) });
        }
        foreach (var s in outputs)
        {
            if (s?.resource != null)
                asset.outputs.Add(new RecipeSlot { resource = s.resource, amount = Mathf.Max(1, s.amount) });
        }

        string assetPath = $"Assets/Game Assemblies/Databases/Recipes/{recipeName}.asset";
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);

        Debug.Log("Recipe created: " + recipeName);
    }
}
