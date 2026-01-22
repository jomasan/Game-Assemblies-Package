using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;

public static class SA_Menu
{
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
    [MenuItem("Game Assemblies/Systems/Create Resource Management System")]
    public static void CreateResourceManagementSystem()
    {
        GameObject rm_prefab = FindPrefab("Simulated Assemblies/Prefabs/Managers/ResourceManager.prefab");
        GameObject gm_prefab = FindPrefab("Simulated Assemblies/Prefabs/Managers/GoalManager.prefab");
        GameObject rmc_prefab = FindPrefab("Simulated Assemblies/Prefabs/UI Prefabs/ResourceManager_Canvas.prefab");

        if (rm_prefab == null)
        {
            Debug.LogError($"Prefab not found: ResourceManager.prefab");
            return;
        }

        if (gm_prefab == null)
        {
            Debug.LogError($"Prefab not found: GoalManager.prefab");
            return;
        }

        if (rmc_prefab == null)
        {
            Debug.LogError($"Prefab not found: ResourceManager_Canvas.prefab");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject gm_instance = (GameObject)PrefabUtility.InstantiatePrefab(gm_prefab, SceneManager.GetActiveScene());
        GameObject rm_instance = (GameObject)PrefabUtility.InstantiatePrefab(rm_prefab, SceneManager.GetActiveScene());
        GameObject rmc_instance = (GameObject)PrefabUtility.InstantiatePrefab(rmc_prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        gm_instance.transform.position = Vector3.zero;
        rm_instance.transform.position = Vector3.zero;
        rmc_instance.transform.position = Vector3.zero;

        GameObject gtg = rmc_instance.GetComponent<resourceManagerCanvas>().goalTrackerModule;
        TMP_Text gs = rmc_instance.GetComponent<resourceManagerCanvas>().globalScoreModule;

        gm_instance.GetComponent<GoalManager>().goalTrackerGrid = gtg;
        gm_instance.GetComponent<GoalManager>().scoreText = gs;

        // Log success (optional)
        Debug.Log("Resource Management System Created");

    }

    [MenuItem("Game Assemblies/Systems/Create Levels System and Menu")]
    public static void CreateLevelGameSystem()
    {
        GameObject lm_prefab = FindPrefab("Simulated Assemblies/Prefabs/Managers/LevelManager.prefab");
        GameObject gsmc_prefab = FindPrefab("Simulated Assemblies/Prefabs/Managers/GameStateManagerAndCanvas.prefab");

        if (lm_prefab == null)
        {
            Debug.LogError($"Prefab not found: LevelManager.prefab");
            return;
        }

        if (gsmc_prefab == null)
        {
            Debug.LogError($"Prefab not found: GameStateManagerAndCanvas.prefab");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject lm_instance = (GameObject)PrefabUtility.InstantiatePrefab(lm_prefab, SceneManager.GetActiveScene());
        GameObject gsmc_instance = (GameObject)PrefabUtility.InstantiatePrefab(gsmc_prefab, SceneManager.GetActiveScene());


        // Set its position to (0,0,0)
        lm_instance.transform.position = Vector3.zero;
        gsmc_instance.transform.position = Vector3.zero;

        //GameObject gtg = rmc_instance.GetComponent<resourceManagerCanvas>().goalTrackerModule;
        //TMP_Text gs = rmc_instance.GetComponent<resourceManagerCanvas>().globalScoreModule;

        //gm_instance.GetComponent<GoalManager>().goalTrackerGrid = gtg;
        //gm_instance.GetComponent<GoalManager>().scoreText = gs;

        // Log success (optional)
        Debug.Log("Levels System & Menu System Created");

    }

    [MenuItem("Game Assemblies/Environment/Create White Canvas")]
    public static void CreateEmptyEnvironment()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Canvas/BackgroundPlane.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}. Searched in both package and Assets folders.");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("White Canvas created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Environment/Create Stage Background")]
    public static void CreateBackgroundEnvironment()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Canvas/ScreenBackground.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("Stage Background created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Environment/Create Ground Tile")]
    public static void CreateGroundTile()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tile_template_01.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Check for a GameObject called "Tiles" in the scene
        GameObject stations = GameObject.Find("Tiles");
        if (stations == null)
        {
            // Create a new "Tiles" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Tiles");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Tiles" object
        instance.transform.SetParent(stations.transform);

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("Ground Tile created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Environment/Create Bush")]
    public static void CreateBush()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Bush.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Check for a GameObject called "Environment" in the scene
        GameObject stations = GameObject.Find("Environment");
        if (stations == null)
        {
            // Create a new "Environment" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Environment");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Environment" object
        instance.transform.SetParent(stations.transform);

        // Log success (optional)
        Debug.Log("Ground Tile created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Stations/Create Automatic Station")]
    public static void CreateAutomaticStation()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tutorial Objects/Automatic_Resources.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Check for a GameObject called "Stations" in the scene
        GameObject stations = GameObject.Find("Stations");
        if (stations == null)
        {
            // Create a new "Stations" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Stations");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Stations" object
        instance.transform.SetParent(stations.transform);

        // Log success (optional)
        Debug.Log("Automatic Station created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Stations/Create Resources When Worked Station")]
    public static void CreateResourcesWhenWorkedStation()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tutorial Objects/Resources_when_Worked.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Check for a GameObject called "Stations" in the scene
        GameObject stations = GameObject.Find("Stations");
        if (stations == null)
        {
            // Create a new "Stations" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Stations");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Stations" object
        instance.transform.SetParent(stations.transform);

        // Log success (optional)
        Debug.Log("Resources when Worked Station created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Stations/Create Random Resources When Worked Station")]
    public static void CreateRandomResourcesWhenWorkedStation()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tutorial Objects/Random_Resources_when_Worked.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Check for a GameObject called "Stations" in the scene
        GameObject stations = GameObject.Find("Stations");
        if (stations == null)
        {
            // Create a new "Stations" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Stations");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Stations" object
        instance.transform.SetParent(stations.transform);

        // Log success (optional)
        Debug.Log("Random Resources when Worked Station created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Stations/Create Convert Resources on Work Station")]
    public static void CreateConvertOnWorkStation()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tutorial Objects/Convert_Resources_On_Work.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Check for a GameObject called "Stations" in the scene
        GameObject stations = GameObject.Find("Stations");
        if (stations == null)
        {
            // Create a new "Stations" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Stations");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Stations" object
        instance.transform.SetParent(stations.transform);

        // Log success (optional)
        Debug.Log("Automatic Station created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Stations/Create Output Box")]
    public static void CreateOutputBox()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tutorial Objects/Convert_Resources_To_Capital.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Check for a GameObject called "Stations" in the scene
        GameObject stations = GameObject.Find("Stations");
        if (stations == null)
        {
            // Create a new "Stations" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Stations");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Stations" object
        instance.transform.SetParent(stations.transform);

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("Output Box created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Stations/Create Single Extract Station")]
    public static void CreateSingleExtractStation()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Tutorial Objects/Single_Use_Resource.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = Vector3.zero;

        // Check for a GameObject called "Stations" in the scene
        GameObject stations = GameObject.Find("Stations");
        if (stations == null)
        {
            // Create a new "Stations" GameObject at (0,0,0) if it doesn't exist
            stations = new GameObject("Stations");
            stations.transform.position = Vector3.zero;
        }

        // Parent the instantiated object under the "Stations" object
        instance.transform.SetParent(stations.transform);

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("Automatic Station created at (0,0,0).");
    }

    [MenuItem("Game Assemblies/Camera/Create Pixel Perfect Camera")]
    public static void CreateCameraSetup()
    {
        string relativePath = "Simulated Assemblies/Prefabs/Cameras/Pixel Perfect Camera.prefab";
        GameObject prefab = FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}");
            return;
        }

        // Check for a GameObject called "Camera" in the scene and remove it
        GameObject cameraObj = GameObject.FindAnyObjectByType<Camera>().gameObject;
        if (cameraObj == null)
        {
            // Remove the object immediately in the Editor.
            //Object.DestroyImmediate(cameraObj);
            cameraObj.SetActive(false);
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,0)
        instance.transform.position = new Vector3(0,0,-5);

       

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("Pixel Perfect Camera created at (0,0,-5).");
    }
}
