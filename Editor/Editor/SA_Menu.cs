using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;

public static class SA_Menu
{
    [MenuItem("Game Assemblies/Systems/Create Resource Management System")]
    public static void CreateResourceManagementSystem()
    {
        GameObject rm_prefab = SA_AssetPathHelper.FindPrefab("Samples/Prefabs/Managers/ResourceManager.prefab");
        GameObject gm_prefab = SA_AssetPathHelper.FindPrefab("Samples/Prefabs/Managers/GoalManager.prefab");
        GameObject rmc_prefab = SA_AssetPathHelper.FindPrefab("Samples/Prefabs/UI Prefabs/ResourceManager_Canvas.prefab");

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
        GameObject lm_prefab = SA_AssetPathHelper.FindPrefab("Samples/Prefabs/Managers/LevelManager.prefab");
        GameObject gsmc_prefab = SA_AssetPathHelper.FindPrefab("Samples/Prefabs/Managers/GameStateManagerAndCanvas.prefab");

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
        string relativePath = "Samples/Prefabs/Canvas/BackgroundPlane.prefab";
        GameObject prefab = SA_AssetPathHelper.FindPrefab(relativePath);

        if (prefab == null)
        {
            Debug.LogError($"Prefab not found: {relativePath}. Searched in both package and Assets folders.");
            return;
        }

        // Use PrefabUtility.InstantiatePrefab to properly instantiate in Editor/Scene
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, SceneManager.GetActiveScene());

        // Set its position to (0,0,1) to avoid collision conflicts with sprites at z=0
        instance.transform.position = new Vector3(0, 0, 1);

        // Optionally, you can select the newly created object automatically
        Selection.activeObject = instance;

        // Log success (optional)
        Debug.Log("White Canvas created at (0,0,1).");
    }

    [MenuItem("Game Assemblies/Environment/Create Stage Background")]
    public static void CreateBackgroundEnvironment()
    {
        string relativePath = "Samples/Prefabs/Canvas/ScreenBackground.prefab";
        GameObject prefab = SA_AssetPathHelper.FindPrefab(relativePath);

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

    [MenuItem("Game Assemblies/Camera/Create Pixel Perfect Camera")]
    public static void CreateCameraSetup()
    {
        string relativePath = "Samples/Prefabs/Cameras/Pixel Perfect Camera.prefab";
        GameObject prefab = SA_AssetPathHelper.FindPrefab(relativePath);

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
