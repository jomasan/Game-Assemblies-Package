using UnityEditor;
using UnityEngine;

public class SA_CreateStationGoalWindow : EditorWindow
{
    private string goalName = "NewStationGoal";
    private StationDataSO stationType;
    private float timeLimit = 20f;
    private int requiredCount = 1;
    private int rewardPoints = 0;
    private int penalty = 1;

    [MenuItem("Game Assemblies/Goals/Create Station Goal")]
    public static void ShowWindow()
    {
        GetWindow<SA_CreateStationGoalWindow>("Create Station Goal");
    }

    private void OnGUI()
    {
        GUILayout.Label("Create a New Station Goal", EditorStyles.boldLabel);
        GUILayout.Space(12);

        goalName = EditorGUILayout.TextField("Goal Name", goalName);
        stationType = EditorGUILayout.ObjectField("Station to Create", stationType, typeof(StationDataSO), false) as StationDataSO;
        requiredCount = EditorGUILayout.IntField("Required Count", requiredCount);
        timeLimit = EditorGUILayout.FloatField("Time Limit (seconds)", timeLimit);

        GUILayout.Space(8);
        GUILayout.Label("Scoring", EditorStyles.boldLabel);
        rewardPoints = EditorGUILayout.IntField("Reward Points", rewardPoints);
        penalty = EditorGUILayout.IntField("Penalty", penalty);

        GUILayout.Space(16);
        EditorGUILayout.HelpBox(
            "Creates a StationGoalSO asset that completes when the selected station type is created enough times before the timer expires.",
            MessageType.Info);

        if (GUILayout.Button("Create"))
            CreateGoal();
    }

    private void CreateGoal()
    {
        if (stationType == null)
        {
            Debug.LogWarning("Station Goal creation canceled: assign a StationDataSO.");
            return;
        }

        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Databases/Goals");

        StationGoalSO newAsset = ScriptableObject.CreateInstance<StationGoalSO>();
        newAsset.name = goalName;
        newAsset.stationType = stationType;
        newAsset.requiredCount = Mathf.Max(1, requiredCount);
        newAsset.timeLimit = Mathf.Max(0f, timeLimit);
        newAsset.rewardPoints = rewardPoints;
        newAsset.penalty = penalty;

        string assetPath = $"Assets/Game Assemblies/Databases/Goals/{goalName}.asset";
        AssetDatabase.CreateAsset(newAsset, assetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = newAsset;
        EditorGUIUtility.PingObject(newAsset);

        Debug.Log("Station goal created: " + goalName);
    }
}
