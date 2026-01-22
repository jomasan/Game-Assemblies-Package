using UnityEngine;
using UnityEditor;

public class SA_CreateResourceWindow : EditorWindow
{
    private string resourceName = "NewResource";
    private Sprite objectSprite;

    // Tutorial image
    private Texture2D tutorialImage;

    // Adds a menu item under Tools -> Create Resource
    [MenuItem("Game Assemblies/Resources/Create Resource")]
    public static void ShowWindow()
    {
        // Opens the window with the title "Create Resource"
        GetWindow<SA_CreateResourceWindow>("Create Resource");
    }

    private void OnGUI()
    {

        /*
        // Display the tutorial image if available
        if (tutorialImage != null)
        {
            GUILayout.Label(tutorialImage, GUILayout.Width(300), GUILayout.Height(150));
        } else
        {
            //EditorGUILayout.HelpBox("Tutorial image not found at Assets/Editor/TutorialImage.png", MessageType.Warning);
        }
        */

        GUILayout.Label("Create a New Resource", EditorStyles.boldLabel);
        GUILayout.Space(20);
        // Text field for the name of the new resource
        resourceName = EditorGUILayout.TextField("Resource Name", resourceName);
        // Sprite field for assigning a sprite to the ScriptableObject's field
        objectSprite = (Sprite)EditorGUILayout.ObjectField("Resource Sprite", objectSprite, typeof(Sprite), false);



        GUILayout.Space(20);
        GUILayout.Label("Welcome to the resource creator. Input the name of your resource. "); //TEXT LABEL
        GUILayout.Label("This panel will create a resource prefab as well as a scriptable object.");
        GUILayout.Label("Prefab Path: Simulated Assemblies/Prefabs/Resources");
        GUILayout.Label("Scriptable Object Path: Simulated Assemblies/Databases/Resources/");
        //"Assets/Simulated Assemblies/Databases/Resources/Apples_data.asset"
        GUILayout.Space(20);


        // When the Create button is pressed, execute CreateResource()
        if (GUILayout.Button("Create"))
        {
            CreateResource();
        }
    }

    private void OnEnable()
    {
        // Load a tutorial image from your assets.
        // Make sure the image exists at the given path.
        tutorialImage = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Simulated Assemblies/2d Assets/Asset02b.png");
    }

    private void CreateResource()
    {

        // --- 1. Instantiate the prefab ---
        // Specify the path to your prefab template asset (update this path as needed)
        string templatePrefabPath = "Assets/Simulated Assemblies/Prefabs/Resources/resource_obj_template.prefab";
        // Specify the destination path for the new prefab (using the provided resource name)
        string newPrefabPath = $"Assets/Simulated Assemblies/Prefabs/Resources/{resourceName}.prefab";

        // Check if a prefab with this name already exists
        if (AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath) != null)
        {
            Debug.LogError("A prefab with this name already exists: " + newPrefabPath);
            return;
        }

        // Copy the template prefab to create a new prefab asset
        bool copySuccess = AssetDatabase.CopyAsset(templatePrefabPath, newPrefabPath);
        if (!copySuccess)
        {
            Debug.LogError("Failed to copy prefab template to " + newPrefabPath);
            return;
        } else
        {
            Debug.Log("Prefab created at: " + newPrefabPath);
        }

        // --- 2. Create the ScriptableObject asset ---

        // Replace MyScriptableObject with your actual ScriptableObject class
        Resource newAsset = ScriptableObject.CreateInstance<Resource>();
        newAsset.name = resourceName;
        newAsset.resourceName = resourceName;
        newAsset.icon = objectSprite;

        // Determine where to save the new asset (you can adjust the folder if desired)

        string assetPath = $"Assets/Simulated Assemblies/Databases/Resources/{resourceName}.asset";
        AssetDatabase.CreateAsset(newAsset, assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log("Resource created: " + resourceName);

        // -------- 3. WIRE the relationship between prefab and scriptable object

        GameObject newPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath) as GameObject;
        if (newPrefab != null)
        {
            newPrefab.GetComponent<ResourceObject>().resourceType = newAsset;
            newPrefab.GetComponent<SpriteRenderer>().sprite = objectSprite;
            newAsset.resourcePrefab = newPrefab;
            newPrefab.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        }
        Debug.Log("Resource wired correctly: " + resourceName);

    }
}

