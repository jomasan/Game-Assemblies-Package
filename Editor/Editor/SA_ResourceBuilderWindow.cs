using UnityEngine;
using UnityEditor;

/// <summary>
/// Resource Builder window that guides users through creating resources with templates.
/// Resources define what flows through the chain: Source (gather) → Transform (craft) → Sink (deliver).
/// </summary>
public class SA_ResourceBuilderWindow : EditorWindow
{
    private const string TemplatePrefabPath = "Samples/Prefabs/Resources/resource_obj_template.prefab";

    private string resourceName = "New Resource";
    private Sprite resourceIcon;
    private Resource.ResourceBehavior typeOfBehavior = Resource.ResourceBehavior.Static;
    private float lifespan = 10f;
    private Vector2 scrollPosition;

    private enum ResourceTemplate
    {
        None,
        Static,
        Decays,
        Consumable
    }
    private ResourceTemplate selectedTemplate = ResourceTemplate.None;

    private static readonly string[] TemplateDisplayNames = new[]
    {
        "None",
        "Static (permanent, grab & move)",
        "Decays (loses value over time)",
        "Consumable (instant collect on contact)"
    };

    private static readonly string[] TemplateDescriptions = new[]
    {
        "",
        "A permanent store of value. Players grab and move these into station input areas. They persist until consumed or delivered. Use for wood, ore, planks, ingots—anything that flows through the chain.",
        "A resource that loses value over time. Players must grab and move it into stations before it expires. Use for food that spoils, fuel that burns off, or time-sensitive materials.",
        "Instantly collected upon contact. No grab-and-carry—players absorb these on touch. Use for coins, power-ups, or resources that are consumed immediately."
    };

    [MenuItem("Game Assemblies/Resources/Resource Builder")]
    public static void ShowWindow()
    {
        var window = GetWindow<SA_ResourceBuilderWindow>("Resource Builder");
        window.minSize = new Vector2(450, 420);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        DrawTemplates();
        DrawFields();
        DrawCreateButton();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(8);
        GUILayout.Label("Resource Builder", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Resources flow through the chain: players gather from stations, carry to other stations, and deliver to complete goals. " +
            "Behavior (Static, Decays, Consumable) defines how the resource is picked up and whether it persists over time.",
            MessageType.Info);
        EditorGUILayout.Space(4);
    }

    private void DrawTemplates()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Templates", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Selecting a template updates Behavior, Lifespan, and suggested name. You can customize all values after.",
            MessageType.None);
        EditorGUILayout.BeginHorizontal();
        int templateIndex = EditorGUILayout.Popup(
            new GUIContent("Template", "Predefined resource types. Selecting a template updates Behavior and Lifespan automatically."),
            (int)selectedTemplate,
            TemplateDisplayNames);
        ResourceTemplate newSelection = (ResourceTemplate)templateIndex;
        if (newSelection != selectedTemplate)
        {
            selectedTemplate = newSelection;
            ApplyTemplate(selectedTemplate);
        }
        else
        {
            selectedTemplate = newSelection;
        }
        if (GUILayout.Button("Apply Template", GUILayout.Width(120)))
        {
            ApplyTemplate(selectedTemplate);
        }
        EditorGUILayout.EndHorizontal();
        if (selectedTemplate != ResourceTemplate.None && (int)selectedTemplate < TemplateDescriptions.Length)
        {
            EditorGUILayout.Space(2);
            EditorGUILayout.HelpBox(TemplateDescriptions[(int)selectedTemplate], MessageType.None);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(4);
    }

    private void ApplyTemplate(ResourceTemplate template)
    {
        if (template == ResourceTemplate.None) return;

        switch (template)
        {
            case ResourceTemplate.Static:
                resourceName = "Wood";
                typeOfBehavior = Resource.ResourceBehavior.Static;
                lifespan = 10f;
                break;
            case ResourceTemplate.Decays:
                resourceName = "Fresh Food";
                typeOfBehavior = Resource.ResourceBehavior.Decays;
                lifespan = 30f;
                break;
            case ResourceTemplate.Consumable:
                resourceName = "Coins";
                typeOfBehavior = Resource.ResourceBehavior.Consumable;
                lifespan = 10f;
                break;
        }
    }

    private void DrawFields()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        GUILayout.Label("Resource Details", EditorStyles.boldLabel);
        resourceName = EditorGUILayout.TextField(
            new GUIContent("Resource Name", "Display name used in stations, goals, and UI."),
            resourceName);
        resourceIcon = (Sprite)EditorGUILayout.ObjectField(
            new GUIContent("Icon", "Sprite shown in UI, info panels, and goal trackers."),
            resourceIcon,
            typeof(Sprite),
            false);
        typeOfBehavior = (Resource.ResourceBehavior)EditorGUILayout.EnumPopup(
            new GUIContent("Behavior", "Static: permanent, grab & move. Decays: expires after lifespan. Consumable: instant collect on contact."),
            typeOfBehavior);
        EditorGUI.BeginDisabledGroup(typeOfBehavior != Resource.ResourceBehavior.Decays);
        lifespan = EditorGUILayout.FloatField(
            new GUIContent("Lifespan (seconds)", "How long before this resource decays and is destroyed. Only used when Behavior is Decays."),
            Mathf.Max(0.1f, lifespan));
        EditorGUI.EndDisabledGroup();
        EditorGUILayout.HelpBox(
            "A prefab will be created from the resource template and linked automatically. " +
            "Assign the icon; the prefab's sprite will match.",
            MessageType.None);
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(4);
    }

    private void DrawCreateButton()
    {
        EditorGUILayout.Space(4);
        var createRect = GUILayoutUtility.GetRect(0, 36);
        createRect.x += 20;
        createRect.width -= 40;

        bool canCreate = !string.IsNullOrWhiteSpace(resourceName);

        EditorGUI.BeginDisabledGroup(!canCreate);
        if (GUI.Button(createRect, "Create Resource"))
        {
            CreateResource();
        }
        EditorGUI.EndDisabledGroup();

        if (!canCreate)
        {
            EditorGUILayout.HelpBox("Enter a resource name to create.", MessageType.Warning);
        }

        EditorGUILayout.Space(4);
    }

    private void CreateResource()
    {
        GameObject templatePrefab = SA_AssetPathHelper.FindPrefab(TemplatePrefabPath);
        if (templatePrefab == null)
        {
            Debug.LogError($"Resource Builder: Template prefab not found at {TemplatePrefabPath}");
            return;
        }

        string templatePrefabPath = AssetDatabase.GetAssetPath(templatePrefab);
        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Prefabs/Resources");
        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Databases/Resources");

        string newPrefabPath = $"Assets/Game Assemblies/Prefabs/Resources/{resourceName}.prefab";
        if (AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath) != null)
        {
            Debug.LogError($"Resource Builder: A prefab with this name already exists: {newPrefabPath}");
            return;
        }

        bool copySuccess = AssetDatabase.CopyAsset(templatePrefabPath, newPrefabPath);
        if (!copySuccess)
        {
            Debug.LogError($"Resource Builder: Failed to copy prefab template to {newPrefabPath}");
            return;
        }

        Resource newAsset = ScriptableObject.CreateInstance<Resource>();
        newAsset.name = resourceName;
        newAsset.resourceName = resourceName;
        newAsset.icon = resourceIcon;
        newAsset.typeOfBehavior = typeOfBehavior;
        newAsset.lifespan = lifespan;

        string assetPath = $"Assets/Game Assemblies/Databases/Resources/{resourceName}.asset";
        AssetDatabase.CreateAsset(newAsset, assetPath);
        AssetDatabase.SaveAssets();

        GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
        if (newPrefab != null)
        {
            var resourceObj = newPrefab.GetComponent<ResourceObject>();
            if (resourceObj != null) resourceObj.resourceType = newAsset;
            var sr = newPrefab.GetComponent<SpriteRenderer>();
            if (sr != null && resourceIcon != null) sr.sprite = resourceIcon;
            newAsset.resourcePrefab = newPrefab;
            EditorUtility.SetDirty(newAsset);
            AssetDatabase.SaveAssets();
        }

        Selection.activeObject = newAsset;
        EditorGUIUtility.PingObject(newAsset);
        Debug.Log($"Resource Builder: Created '{resourceName}' - SO at {assetPath}, prefab at {newPrefabPath}");
    }
}
