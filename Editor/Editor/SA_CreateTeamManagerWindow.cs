using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Editor window to create a Team Manager in the active scene from the TeamManager prefab.
/// Enforces a single TeamManager instance per scene.
/// </summary>
public class SA_CreateTeamManagerWindow : EditorWindow
{
    private const string TeamManagerPrefabPath = "Samples/Prefabs/Managers/TeamManager.prefab";
    private TeamManager.Mode mode = TeamManager.Mode.EveryoneOneTeam;
    private TeamManager.LevelScoreAggregate levelScoreAggregate = TeamManager.LevelScoreAggregate.SumAll;
    private playersInfo playerList;
    private bool debug;

    [MenuItem("Game Assemblies/Systems/Create Team Manager")]
    public static void ShowWindow()
    {
        GetWindow<SA_CreateTeamManagerWindow>("Create Team Manager");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);
        GUILayout.Label("Create Team Manager", EditorStyles.boldLabel);
        GUILayout.Space(6);

        EditorGUILayout.HelpBox(
            "Instantiates TeamManager.prefab into the active scene. If a TeamManager already exists, this tool applies these settings to the existing manager instead of creating another one.",
            MessageType.Info);

        GUILayout.Space(8);
        DrawConfigurationUI();

        GUILayout.Space(14);
        if (GUILayout.Button("Create / Configure Team Manager", GUILayout.Height(30)))
            CreateTeamManager();
    }

    private void DrawConfigurationUI()
    {
        EditorGUILayout.LabelField("Team Manager Settings", EditorStyles.boldLabel);

        mode = (TeamManager.Mode)EditorGUILayout.EnumPopup(
            new GUIContent("Mode", "EveryoneOneTeam: one shared score. Teams: per-team scores. Solo: per-player scores for joined players. CompetitiveSolo: fixed 4 player score slots."),
            mode);

        levelScoreAggregate = (TeamManager.LevelScoreAggregate)EditorGUILayout.EnumPopup(
            new GUIContent("Level Score Aggregate", "How to compute level result score in Teams mode. SumAll adds all team scores; Max uses the highest team score."),
            levelScoreAggregate);

        playerList = (playersInfo)EditorGUILayout.ObjectField(
            new GUIContent("Player List", "Optional playersInfo reference used to register players and populate score labels/slots."),
            playerList,
            typeof(playersInfo),
            true);

        debug = EditorGUILayout.Toggle(
            new GUIContent("Debug Logs", "When enabled, TeamManager writes score-routing/debug messages to the Console."),
            debug);

        GUILayout.Space(6);
        EditorGUILayout.HelpBox(
            "These values are applied to the TeamManager component on creation. If one already exists, these values are applied to that existing component.",
            MessageType.None);
    }

    private void CreateTeamManager()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        if (!activeScene.IsValid() || !activeScene.isLoaded)
        {
            Debug.LogError("Create Team Manager: No valid active scene.");
            return;
        }

        TeamManager existing = Object.FindObjectOfType<TeamManager>();
        if (existing != null)
        {
            ApplyConfiguration(existing);
            Selection.activeGameObject = existing.gameObject;
            EditorGUIUtility.PingObject(existing.gameObject);
            Debug.Log("Create Team Manager: TeamManager already exists in scene. Applied settings to existing instance.");
            return;
        }

        GameObject prefab = SA_AssetPathHelper.FindPrefab(TeamManagerPrefabPath);
        if (prefab == null)
        {
            Debug.LogError("Create Team Manager: TeamManager prefab not found at " + TeamManagerPrefabPath);
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, activeScene);
        if (instance == null)
        {
            Debug.LogError("Create Team Manager: Failed to instantiate TeamManager prefab.");
            return;
        }

        instance.transform.position = Vector3.zero;
        TeamManager createdManager = instance.GetComponent<TeamManager>();
        if (createdManager == null)
            createdManager = instance.AddComponent<TeamManager>();
        ApplyConfiguration(createdManager);

        Undo.RegisterCreatedObjectUndo(instance, "Create Team Manager");
        Selection.activeGameObject = instance;
        EditorGUIUtility.PingObject(instance);
        Debug.Log("Team Manager created in scene and configured.");
    }

    private void ApplyConfiguration(TeamManager manager)
    {
        if (manager == null) return;

        Undo.RecordObject(manager, "Configure Team Manager");
        manager.mode = mode;
        manager.levelScoreAggregate = levelScoreAggregate;
        manager.playerList = playerList;
        manager.debug = debug;
        EditorUtility.SetDirty(manager);
    }
}
