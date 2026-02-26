using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Single editor tool: create a Policy Data asset from the form, and either instantiate
/// a Policy Manager in the scene (with that policy) or assign the new policy to the existing manager.
/// Uses a copy of the template prefab saved under Game Assemblies/Prefabs/Managers. Only one Policy Manager per scene.
/// </summary>
public class SA_CreatePolicyManagerWindow : EditorWindow
{
    private const string PolicyManagerPrefabPath = "Samples/Prefabs/Managers/PolicyManager.prefab";
    private const string PrefabOutputFolder = "Game Assemblies/Prefabs/Managers";
    private const string PrefabOutputName = "PolicyManager";

    [Tooltip("Template prefab to copy into Game Assemblies/Prefabs/Managers. The copy is used for instantiation; the template is not modified.")]
    public GameObject prefabTemplate;

    private string policyName = "New Policy";
    private OwnershipModel ownershipModel = OwnershipModel.PrivateIndividual;
    private StealingPolicy stealingPolicy = StealingPolicy.Disallowed;
    private GoalAttribution goalAttribution = GoalAttribution.ResourceOwner;
    private float goalAttributionOwnerShare = 0.7f;
    private StationUsePolicy stationUsePolicy = StationUsePolicy.Anyone;
    private bool sharingAllowed = true;
    private ResourceVisibility resourceVisibility = ResourceVisibility.Public;

    private Vector2 scrollPosition;

    [MenuItem("Game Assemblies/Systems/Create Policy Manager")]
    public static void ShowWindow()
    {
        GetWindow<SA_CreatePolicyManagerWindow>("Create Policy Manager");
    }

    private void OnEnable()
    {
        if (prefabTemplate == null)
            prefabTemplate = SA_AssetPathHelper.FindPrefab(PolicyManagerPrefabPath);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Label("Create Policy Manager", EditorStyles.boldLabel);
        GUILayout.Space(4);
        EditorGUILayout.HelpBox(
            "Creates a Policy Data asset and connects it to the Policy Manager. If a Policy Manager already exists in the scene, the new policy replaces its assignment. Otherwise, a copy of the template prefab is saved to " + PrefabOutputFolder + " and instantiated with the new policy assigned. Only one Policy Manager per scene.",
            MessageType.Info);
        GUILayout.Space(12);

        EditorGUILayout.LabelField("Prefab Template", EditorStyles.boldLabel);
        prefabTemplate = (GameObject)EditorGUILayout.ObjectField(
            "Policy Manager Prefab (template)",
            prefabTemplate,
            typeof(GameObject),
            false);
        if (prefabTemplate != null && PrefabUtility.GetPrefabAssetType(prefabTemplate) == PrefabAssetType.NotAPrefab)
            EditorGUILayout.HelpBox("Assign a prefab asset. The tool will copy it to " + PrefabOutputFolder + "/" + PrefabOutputName + ".prefab and instantiate from that copy.", MessageType.Warning);
        else if (prefabTemplate != null)
            EditorGUILayout.LabelField("Copy will be saved to:", $"Assets/{PrefabOutputFolder}/{PrefabOutputName}.prefab", EditorStyles.miniLabel);
        GUILayout.Space(12);

        EditorGUILayout.LabelField("Policy Name & Data", EditorStyles.boldLabel);
        policyName = EditorGUILayout.TextField("Policy Name", policyName);
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Ownership", EditorStyles.boldLabel);
        ownershipModel = (OwnershipModel)EditorGUILayout.EnumPopup("Ownership Model", ownershipModel);
        GUILayout.Space(6);

        EditorGUILayout.LabelField("Stealing / Taking", EditorStyles.boldLabel);
        stealingPolicy = (StealingPolicy)EditorGUILayout.EnumPopup("Stealing Policy", stealingPolicy);
        GUILayout.Space(6);

        EditorGUILayout.LabelField("Goal Attribution", EditorStyles.boldLabel);
        goalAttribution = (GoalAttribution)EditorGUILayout.EnumPopup("Goal Attribution", goalAttribution);
        goalAttributionOwnerShare = EditorGUILayout.Slider("Owner Share (when Split)", goalAttributionOwnerShare, 0f, 1f);
        GUILayout.Space(6);

        EditorGUILayout.LabelField("Station Use", EditorStyles.boldLabel);
        stationUsePolicy = (StationUsePolicy)EditorGUILayout.EnumPopup("Station Use Policy", stationUsePolicy);
        GUILayout.Space(6);

        EditorGUILayout.LabelField("Sharing & Visibility", EditorStyles.boldLabel);
        sharingAllowed = EditorGUILayout.Toggle("Sharing Allowed", sharingAllowed);
        resourceVisibility = (ResourceVisibility)EditorGUILayout.EnumPopup("Resource Visibility", resourceVisibility);
        GUILayout.Space(20);

        if (GUILayout.Button("Create Policy Manager", GUILayout.Height(32)))
        {
            CreatePolicyAndManager();
        }

        EditorGUILayout.EndScrollView();
    }

    private void CreatePolicyAndManager()
    {
        if (string.IsNullOrWhiteSpace(policyName))
        {
            Debug.LogWarning("Create Policy Manager: Enter a policy name.");
            return;
        }

        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid() || !activeScene.isLoaded)
        {
            Debug.LogError("Create Policy Manager: No valid active scene.");
            return;
        }

        // 1. Create Policy Data asset and save to Databases/Policies
        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Databases/Policies");
        PolicyDataSO policyAsset = ScriptableObject.CreateInstance<PolicyDataSO>();
        policyAsset.ownershipModel = ownershipModel;
        policyAsset.stealingPolicy = stealingPolicy;
        policyAsset.goalAttribution = goalAttribution;
        policyAsset.goalAttributionOwnerShare = goalAttributionOwnerShare;
        policyAsset.stationUsePolicy = stationUsePolicy;
        policyAsset.sharingAllowed = sharingAllowed;
        policyAsset.resourceVisibility = resourceVisibility;

        string assetPath = $"Assets/Game Assemblies/Databases/Policies/{policyName}.asset";
        AssetDatabase.CreateAsset(policyAsset, assetPath);
        AssetDatabase.SaveAssets();

        // 2. Find existing PolicyManager in scene or instantiate from template
        PolicyManager existingManager = Object.FindObjectOfType<PolicyManager>();
        if (existingManager != null)
        {
            existingManager.policy = policyAsset;
            Undo.RecordObject(existingManager, "Assign Policy");
            EditorUtility.SetDirty(existingManager);
            Selection.activeGameObject = existingManager.gameObject;
            EditorGUIUtility.PingObject(policyAsset);
            Debug.Log("Create Policy Manager: New policy assigned to existing Policy Manager in scene.");
        }
        else
        {
            if (prefabTemplate == null)
            {
                Debug.LogError("Create Policy Manager: Assign a Policy Manager prefab template (or ensure " + PolicyManagerPrefabPath + " exists).");
                Selection.activeObject = policyAsset;
                EditorGUIUtility.PingObject(policyAsset);
                return;
            }
            if (PrefabUtility.GetPrefabAssetType(prefabTemplate) == PrefabAssetType.NotAPrefab)
            {
                Debug.LogError("Create Policy Manager: Prefab Template must be a prefab asset.");
                Selection.activeObject = policyAsset;
                return;
            }

            SA_AssetPathHelper.EnsureAssetPathDirectories(PrefabOutputFolder);
            string newPrefabPath = $"Assets/{PrefabOutputFolder}/{PrefabOutputName}.prefab";
            string templatePath = AssetDatabase.GetAssetPath(prefabTemplate);

            if (AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath) == null)
            {
                bool copySuccess = AssetDatabase.CopyAsset(templatePath, newPrefabPath);
                if (!copySuccess)
                {
                    Debug.LogError("Create Policy Manager: Failed to copy prefab to " + newPrefabPath);
                    Selection.activeObject = policyAsset;
                    return;
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            GameObject prefabCopy = AssetDatabase.LoadAssetAtPath<GameObject>(newPrefabPath);
            if (prefabCopy == null)
            {
                Debug.LogError("Create Policy Manager: Failed to load prefab copy at " + newPrefabPath);
                Selection.activeObject = policyAsset;
                return;
            }

            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefabCopy, activeScene);
            if (instance == null)
            {
                Debug.LogError("Create Policy Manager: Failed to instantiate prefab.");
                Selection.activeObject = policyAsset;
                return;
            }

            instance.transform.position = Vector3.zero;
            instance.name = prefabCopy.name;

            PolicyManager pm = instance.GetComponent<PolicyManager>();
            if (pm == null)
            {
                pm = instance.AddComponent<PolicyManager>();
            }

            pm.policy = policyAsset;
            Undo.RecordObject(pm, "Assign Policy");
            EditorUtility.SetDirty(pm);
            Undo.RegisterCreatedObjectUndo(instance, "Create Policy Manager");

            Selection.activeGameObject = instance;
            EditorGUIUtility.PingObject(policyAsset);
            Debug.Log("Policy Manager created in scene (from " + newPrefabPath + ") with policy: " + policyName);
        }
    }
}
