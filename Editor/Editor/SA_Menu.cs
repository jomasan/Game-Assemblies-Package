using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro; 

public static class SA_Menu
{
    [MenuItem("Game Assemblies/Systems/Create Resource Management System")]
    public static void CreateResourceManagementSystem()
    {
        string resourceManagerprefabPath = "Assets/Simulated Assemblies/Prefabs/Managers/ResourceManager.prefab";
        GameObject rm_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(resourceManagerprefabPath);

        string goalManagerPrefabPath = "Assets/Simulated Assemblies/Prefabs/Managers/GoalManager.prefab";
        GameObject gm_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(goalManagerPrefabPath);

        string rmCanvasPrefabPath = "Assets/Simulated Assemblies/Prefabs/UI Prefabs/ResourceManager_Canvas.prefab";
        GameObject rmc_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(rmCanvasPrefabPath);

        if (rm_prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {resourceManagerprefabPath}");
            return;
        }

        if (gm_prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {goalManagerPrefabPath}");
            return;
        }

        if (rmc_prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {rmCanvasPrefabPath}");
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
        string levelManagerprefabPath = "Assets/Simulated Assemblies/Prefabs/Managers/LevelManager.prefab";
        GameObject lm_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(levelManagerprefabPath);

        string gameStateManagerCanvas = "Assets/Simulated Assemblies/Prefabs/Managers/GameStateManagerAndCanvas.prefab";
        GameObject gsmc_prefab = AssetDatabase.LoadAssetAtPath<GameObject>(gameStateManagerCanvas);

        if (lm_prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {levelManagerprefabPath}");
            return;
        }

        if (gsmc_prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {gameStateManagerCanvas}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Canvas/BackgroundPlane.prefab";


        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Canvas/ScreenBackground.prefab";


        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tile_template_01.prefab";

        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Bush.prefab";

        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tutorial Objects/Automatic_Resources.prefab";
        

        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tutorial Objects/Resources_when_Worked.prefab";


        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tutorial Objects/Random_Resources_when_Worked.prefab";


        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tutorial Objects/Convert_Resources_On_Work.prefab";


        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tutorial Objects/Convert_Resources_To_Capital.prefab";

        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Tutorial Objects/Single_Use_Resource.prefab";


        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
        // Replace the path below with the actual path to your prefab
        // e.g., "Assets/Prefabs/AutomaticStation.prefab"
        string prefabPath = "Assets/Simulated Assemblies/Prefabs/Cameras/Pixel Perfect Camera.prefab";

        // Load the prefab from the specified path
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found at path: {prefabPath}");
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
