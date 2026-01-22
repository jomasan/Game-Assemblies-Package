using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SA_CreatePlayersWindow : EditorWindow
{
    private Sprite player_1_Sprite;
    private Sprite player_2_Sprite;
    private Sprite player_3_Sprite;
    private Sprite player_4_Sprite;

    /// <summary>
    /// Helper method to find a prefab by relative path, checking both package and Assets folders
    /// </summary>
    private static GameObject FindPrefab(string relativePath)
    {
        // Try package path first (for when installed as a package)
        string packagePath = $"Packages/com.gameassemblylab.gameassemblies/{relativePath}";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(packagePath);
        
        if (prefab != null)
            return prefab;
        
        // Try Assets path (for when imported directly or in samples)
        string assetsPath = $"Assets/{relativePath}";
        prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetsPath);
        
        if (prefab != null)
            return prefab;
        
        // Try finding by filename as fallback
        string fileName = System.IO.Path.GetFileName(relativePath);
        string[] guids = AssetDatabase.FindAssets($"{System.IO.Path.GetFileNameWithoutExtension(fileName)} t:Prefab");
        
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("Simulated Assemblies") || path.Contains("gameassemblies"))
            {
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null && path.EndsWith(fileName))
                    return prefab;
            }
        }
        
        return null;
    }

    // Adds a menu item under Tools -> Create Resource
    [MenuItem("Game Assemblies/Players/Create Local Multiplayer System")]
    public static void ShowWindow()
    {
        // Opens the window with the title "Create Resource"
        GetWindow<SA_CreatePlayersWindow>("Create Player System");
    }

    private void OnGUI()
    {
        
        GUILayout.Space(20);
        GUILayout.Label("Welcome to the Player Enviroment Creator. "); //TEXT LABEL
        GUILayout.Label("Define the Player Sprites and hit Create.");
        GUILayout.Space(20);

        player_1_Sprite = (Sprite)EditorGUILayout.ObjectField("Player 1 Sprite", player_1_Sprite, typeof(Sprite), false);
        player_2_Sprite = (Sprite)EditorGUILayout.ObjectField("Player 2 Sprite", player_2_Sprite, typeof(Sprite), false);
        player_3_Sprite = (Sprite)EditorGUILayout.ObjectField("Player 3 Sprite", player_3_Sprite, typeof(Sprite), false);
        player_4_Sprite = (Sprite)EditorGUILayout.ObjectField("Player 4 Sprite", player_4_Sprite, typeof(Sprite), false);


        // When the Create button is pressed, execute CreateResource()
        if (GUILayout.Button("Create Local Multiplayer Environment"))
        {
            CreateEnvironment();
        }
        if (GUILayout.Button("Update Character Icons"))
        {
            UpdateCharacterIcons();
        }
    }

    public void UpdateCharacterIcons()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Players/Player_Drawn.prefab";
        GameObject playerPrefab = FindPrefab(relativePath);

        if (playerPrefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        playerPrefab.GetComponent<playerController>().sprite1 = player_1_Sprite;
        playerPrefab.GetComponent<playerController>().sprite2 = player_2_Sprite;
        playerPrefab.GetComponent<playerController>().sprite3 = player_3_Sprite;
        playerPrefab.GetComponent<playerController>().sprite4 = player_4_Sprite;

        // Log success (optional)
        Debug.Log("Character Icons Updated");
    }

    public void CreateEnvironment()
    {
        string managerPath = "Simulated Assemblies/Prefabs/Managers/PlayerManager.prefab";
        string playerPath = "Simulated Assemblies/Prefabs/Players/Player_Drawn.prefab";

        GameObject prefab = FindPrefab(managerPath);
        GameObject playerPrefab = FindPrefab(playerPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {managerPath}");
            return;
        }

        if (playerPrefab == null)
        {
            Debug.LogError($"Prefab not found: {playerPath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        instance.GetComponent<PlayerInputManager>().playerPrefab = playerPrefab;

        playerPrefab.GetComponent<playerController>().sprite1 = player_1_Sprite;
        playerPrefab.GetComponent<playerController>().sprite2 = player_2_Sprite;
        playerPrefab.GetComponent<playerController>().sprite3 = player_3_Sprite;
        playerPrefab.GetComponent<playerController>().sprite4 = player_4_Sprite;

        // Optionally, you can select the newly created object automatically
        //Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("Local Multiplayer Environment Created");
    }

}
