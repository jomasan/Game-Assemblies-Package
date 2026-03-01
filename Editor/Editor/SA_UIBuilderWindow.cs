using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum UIBuilderWidgetType
{
    ResourceCounter,
    ResourceCounterWithImage,
    PlayerResourceList,
    PlayerScore
}

public class SA_UIBuilderWindow : EditorWindow
{
    private const string ResourceScorePath = "Samples/Prefabs/UI Prefabs/ResourceScore.prefab";
    private const string ResourceScoreWithImagePath = "Samples/Prefabs/UI Prefabs/ResourceScoreWithImage.prefab";
    private const string PlayerResourceListPath = "Samples/Prefabs/UI Prefabs/PlayerResourceList.prefab";
    private const string PlayerScorePath = "Samples/Prefabs/UI Prefabs/PlayerScore.prefab";

    private Transform canvasParent;
    private UIBuilderWidgetType widgetType = UIBuilderWidgetType.ResourceCounter;

    private int resourceIndex;
    private string[] resourceNames;
    private Resource[] resourceAssets;

    private ResourceCountDisplayMode displayMode = ResourceCountDisplayMode.AllOfType;
    private int playerSlot;
    private bool includeName = true;
    private bool showIcon = true;

    private int listPlayerSlot;
    private List<Resource> resourcesToDisplay = new List<Resource>();
    private Vector2 listScroll;

    private TeamScoreDisplayType scoreDisplayType = TeamScoreDisplayType.GlobalScore;
    private int scorePlayerSlot;
    private int scoreTeamSlot;
    private bool addName = true;
    private string scoreLabel = "Score: ";

    private Vector2 mainScroll;

    [MenuItem("Game Assemblies/UI/UI Builder")]
    public static void ShowWindow()
    {
        var w = GetWindow<SA_UIBuilderWindow>("UI Builder");
        w.minSize = new Vector2(320, 400);
    }

    private void OnEnable()
    {
        RefreshResourceList();
    }

    private void RefreshResourceList()
    {
        SA_DatabaseManager.Refresh(typeof(Resource));
        var entries = SA_DatabaseManager.GetEntries(typeof(Resource));
        int count = entries.Count;
        resourceNames = new string[count + 1];
        resourceAssets = new Resource[count + 1];
        resourceNames[0] = "(None)";
        resourceAssets[0] = null;
        for (int i = 0; i < count; i++)
        {
            var r = entries[i] as Resource;
            resourceAssets[i + 1] = r;
            resourceNames[i + 1] = r != null ? r.resourceName : "(Missing)";
        }
    }

    private void OnGUI()
    {
        mainScroll = EditorGUILayout.BeginScrollView(mainScroll);

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("UI Builder", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("Link a canvas element (e.g. a panel with Layout Group) as parent, choose a widget type, set options, then click Instantiate.", MessageType.Info);
        EditorGUILayout.Space(6);

        EditorGUILayout.LabelField("Canvas / Parent", EditorStyles.boldLabel);
        canvasParent = (Transform)EditorGUILayout.ObjectField("Parent", canvasParent, typeof(Transform), true);

        EditorGUILayout.Space(8);
        EditorGUILayout.LabelField("Widget type", EditorStyles.boldLabel);
        widgetType = (UIBuilderWidgetType)EditorGUILayout.EnumPopup("Type", widgetType);

        EditorGUILayout.Space(6);

        switch (widgetType)
        {
            case UIBuilderWidgetType.ResourceCounter:
            case UIBuilderWidgetType.ResourceCounterWithImage:
                DrawResourceCounterOptions();
                break;
            case UIBuilderWidgetType.PlayerResourceList:
                DrawPlayerResourceListOptions();
                break;
            case UIBuilderWidgetType.PlayerScore:
                DrawPlayerScoreOptions();
                break;
        }

        EditorGUILayout.Space(12);
        EditorGUI.BeginDisabledGroup(canvasParent == null);
        if (GUILayout.Button("Instantiate", GUILayout.Height(28)))
        {
            InstantiateWidget();
        }
        EditorGUI.EndDisabledGroup();
        if (canvasParent == null)
            EditorGUILayout.HelpBox("Assign a Parent (canvas element) to instantiate under.", MessageType.Warning);

        EditorGUILayout.EndScrollView();
    }

    private void DrawResourceCounterOptions()
    {
        if (resourceNames == null || resourceNames.Length <= 1) RefreshResourceList();

        EditorGUILayout.LabelField("Resource counter options", EditorStyles.boldLabel);
        resourceIndex = EditorGUILayout.Popup("Resource to track", resourceIndex, resourceNames);
        displayMode = (ResourceCountDisplayMode)EditorGUILayout.EnumPopup("Display mode", displayMode);
        if (displayMode == ResourceCountDisplayMode.OwnedByPlayer)
        {
            string[] players = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };
            playerSlot = EditorGUILayout.Popup("Player", Mathf.Clamp(playerSlot, 0, 3), players);
        }
        includeName = EditorGUILayout.Toggle("Include name", includeName);
        if (widgetType == UIBuilderWidgetType.ResourceCounterWithImage)
            showIcon = EditorGUILayout.Toggle("Show icon", showIcon);
    }

    private void DrawPlayerResourceListOptions()
    {
        EditorGUILayout.LabelField("Player resource list options", EditorStyles.boldLabel);
        string[] players = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };
        listPlayerSlot = EditorGUILayout.Popup("Player to track", Mathf.Clamp(listPlayerSlot, 0, 3), players);

        EditorGUILayout.LabelField("Resources to display");
        listScroll = EditorGUILayout.BeginScrollView(listScroll, GUILayout.MaxHeight(120));
        for (int i = 0; i < resourcesToDisplay.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            resourcesToDisplay[i] = (Resource)EditorGUILayout.ObjectField(resourcesToDisplay[i], typeof(Resource), false);
            if (GUILayout.Button("−", GUILayout.Width(22)))
            {
                resourcesToDisplay.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
        if (GUILayout.Button("+ Add resource"))
            resourcesToDisplay.Add(null);
        if (GUILayout.Button("Populate from project (all Resources)"))
        {
            SA_DatabaseManager.Refresh(typeof(Resource));
            var entries = SA_DatabaseManager.GetEntries(typeof(Resource));
            resourcesToDisplay.Clear();
            foreach (var entry in entries)
            {
                if (entry is Resource r && r != null)
                    resourcesToDisplay.Add(r);
            }
        }
    }

    private void DrawPlayerScoreOptions()
    {
        EditorGUILayout.LabelField("Player score options", EditorStyles.boldLabel);
        scoreDisplayType = (TeamScoreDisplayType)EditorGUILayout.EnumPopup("Score type", scoreDisplayType);
        if (scoreDisplayType == TeamScoreDisplayType.PlayerScore)
        {
            string[] players = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };
            scorePlayerSlot = EditorGUILayout.Popup("Player", Mathf.Clamp(scorePlayerSlot, 0, 3), players);
        }
        else if (scoreDisplayType == TeamScoreDisplayType.TeamScore)
        {
            string[] teams = new[] { "Team 1", "Team 2", "Team 3" };
            scoreTeamSlot = EditorGUILayout.Popup("Team", Mathf.Clamp(scoreTeamSlot, 0, 2), teams);
        }
        addName = EditorGUILayout.Toggle("Add name", addName);
        scoreLabel = EditorGUILayout.TextField("Score label", scoreLabel);
    }

    private GameObject GetPrefab()
    {
        string path = widgetType switch
        {
            UIBuilderWidgetType.ResourceCounter => ResourceScorePath,
            UIBuilderWidgetType.ResourceCounterWithImage => ResourceScoreWithImagePath,
            UIBuilderWidgetType.PlayerResourceList => PlayerResourceListPath,
            UIBuilderWidgetType.PlayerScore => PlayerScorePath,
            _ => ResourceScorePath
        };
        return SA_AssetPathHelper.FindPrefab(path);
    }

    private void InstantiateWidget()
    {
        if (canvasParent == null)
        {
            Debug.LogWarning("UI Builder: Assign a Parent first.");
            return;
        }

        GameObject prefab = GetPrefab();
        if (prefab == null)
        {
            Debug.LogError($"UI Builder: Prefab not found for {widgetType}. Check Samples/Prefabs/UI Prefabs/.");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab, canvasParent);
        Undo.RegisterCreatedObjectUndo(instance, "UI Builder Instantiate");

        switch (widgetType)
        {
            case UIBuilderWidgetType.ResourceCounter:
            case UIBuilderWidgetType.ResourceCounterWithImage:
                ApplyResourceCounter(instance);
                break;
            case UIBuilderWidgetType.PlayerResourceList:
                ApplyPlayerResourceList(instance);
                break;
            case UIBuilderWidgetType.PlayerScore:
                ApplyPlayerScore(instance);
                break;
        }

        Selection.activeGameObject = instance;
        EditorGUIUtility.PingObject(instance);
        Debug.Log($"UI Builder: Instantiated {widgetType} under {canvasParent.name}.");
    }

    private void ApplyResourceCounter(GameObject instance)
    {
        var display = instance.GetComponentInChildren<ResourceCounterDisplay>();
        if (display == null) return;

        display.resourceToTrack = resourceIndex > 0 && resourceAssets != null && resourceIndex < resourceAssets.Length ? resourceAssets[resourceIndex] : null;
        display.displayMode = displayMode;
        display.playerSlot = playerSlot;
        display.includeName = includeName;
        display.showIcon = widgetType == UIBuilderWidgetType.ResourceCounterWithImage && showIcon;
        EditorUtility.SetDirty(display);
    }

    private void ApplyPlayerResourceList(GameObject instance)
    {
        var listDisplay = instance.GetComponentInChildren<PlayerResourceListDisplay>();
        if (listDisplay == null) return;

        listDisplay.playerSlot = listPlayerSlot;
        listDisplay.resourcesToDisplay.Clear();
        foreach (var r in resourcesToDisplay)
        {
            if (r != null)
                listDisplay.resourcesToDisplay.Add(r);
        }
        EditorUtility.SetDirty(listDisplay);
    }

    private void ApplyPlayerScore(GameObject instance)
    {
        var scoreDisplay = instance.GetComponentInChildren<TeamScoreDisplay>();
        if (scoreDisplay == null) return;

        scoreDisplay.displayType = scoreDisplayType;
        scoreDisplay.playerSlot = scorePlayerSlot;
        scoreDisplay.teamSlot = scoreTeamSlot;
        scoreDisplay.addName = addName;
        scoreDisplay.scoreLabel = scoreLabel ?? "Score: ";
        EditorUtility.SetDirty(scoreDisplay);
    }
}
