using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// A didactic Station Builder window that guides users through configuring a station.
/// Centers the station graphic and offers options for key variables with explanatory tooltips.
/// </summary>
public class SA_StationBuilderWindow : EditorWindow
{
    private const string TemplatePrefabPath = "Samples/Prefabs/Tutorial Objects/Convert_Resources_On_Work.prefab";

    // Station identity
    private string stationName = "New Station";
    private Sprite stationGraphic;

    // Resource consumption (list)
    private bool consumeResource;
    private List<Resource> consumeResources = new List<Resource>();

    // Resource production (list)
    private bool produceResource;
    private List<Resource> produceResources = new List<Resource>();

    // Lifespan
    private bool isSingleUse;

    // Capital
    private bool produceCapital;
    private int capitalOutputAmount = 1;
    private bool consumeCapital;
    private int capitalInputAmount = 1;

    // Goals
    private bool consumptionCompletesGoals;
    private bool productionCompletesGoals;

    // Timing & interaction
    private float productionInterval = 5f;
    private float workDuration = 5f;
    private Station.interactionType typeOfProduction = Station.interactionType.whenWorked;
    private Station.interactionType typeOfConsumption = Station.interactionType.whenWorked;
    private bool canBeWorked = true;

    // Preview
    private Rect previewRect;
    private Vector2 scrollPosition;

    // Output mode: ScriptableObject only or full prefab in scene
    private enum CreateMode { ScriptableObject, PrefabInScene }
    private CreateMode createMode = CreateMode.PrefabInScene;

    // Tooltips
    private static readonly GUIContent ConsumeResourceTip = new GUIContent(
        "Consume Resources",
        "When enabled, this station requires physical resources (e.g., wood, ore) to be placed in its input area before it can operate. " +
        "Players must bring the required resource(s) into the station's input zone. This creates a conversion or crafting flow.");
    private static readonly GUIContent ConsumeResourceSlotTip = new GUIContent(
        "Resource",
        "A resource this station accepts as input. Add multiple resources for stations that require several inputs.");
    private static readonly GUIContent ProduceResourceTip = new GUIContent(
        "Produce Resources",
        "When enabled, this station creates and outputs physical resources (e.g., planks, ingots). " +
        "The produced resource spawns in the output area and can be picked up by players or used by other stations.");
    private static readonly GUIContent ProduceResourceSlotTip = new GUIContent(
        "Resource",
        "A resource this station creates. Add multiple resources for stations that produce several outputs.");
    private static readonly GUIContent SingleUseTip = new GUIContent(
        "Single-Use Station",
        "When enabled, the station operates only once and then becomes inactive (or is destroyed). " +
        "Useful for one-time conversions, extractors that deplete, or consumable stations.");
    private static readonly GUIContent ProduceCapitalTip = new GUIContent(
        "Produces Capital",
        "When enabled, this station adds points (capital) to the global score when it completes a cycle. " +
        "Capital represents economic value and can drive goals, purchases, or win conditions.");
    private static readonly GUIContent CapitalOutputAmountTip = new GUIContent(
        "Capital Amount",
        "How many points are added to the global score each time this station completes a production cycle.");
    private static readonly GUIContent ConsumeCapitalTip = new GUIContent(
        "Consumes Capital",
        "When enabled, this station requires capital (points) to be spent from the global pool or worker when it operates. " +
        "Use for paid services, upgrades, or gated stations.");
    private static readonly GUIContent CapitalInputAmountTip = new GUIContent(
        "Capital Cost",
        "How many points are deducted when this station completes a consumption/production cycle.");
    private static readonly GUIContent ConsumptionCompletesGoalsTip = new GUIContent(
        "Consumption Completes Goals",
        "When enabled, consuming the required resource in this station counts toward active goals. " +
        "Use when players must deliver specific resources to stations to meet level objectives.");
    private static readonly GUIContent ProductionCompletesGoalsTip = new GUIContent(
        "Production Completes Goals",
        "When enabled, producing the output resource counts toward active goals. " +
        "Use when players must create specific resources to meet level objectives.");
    private static readonly GUIContent ProductionIntervalTip = new GUIContent(
        "Production Interval (seconds)",
        "For automatic stations: time between production cycles. For worked stations: not used.");
    private static readonly GUIContent WorkDurationTip = new GUIContent(
        "Work Duration (seconds)",
        "How long a player must work at this station (hold/interact) before the cycle completes. " +
        "Longer durations create more strategic timing and coordination.");
    private static readonly GUIContent CanBeWorkedTip = new GUIContent(
        "Requires Player Work",
        "When enabled, a player must interact with the station to trigger consumption/production. " +
        "When disabled, the station operates automatically (if configured for automatic mode).");

    private const float FlowColumnWidth = 300f;
    private const float StationPreviewSize = 100f;
    private const float ArrowWidth = 36f;

    [MenuItem("Game Assemblies/Stations/Station Builder")]
    public static void ShowWindow()
    {
        var window = GetWindow<SA_StationBuilderWindow>("Station Builder");
        window.minSize = new Vector2(700, 520);
    }

    private void OnGUI()
    {
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        DrawHeader();
        DrawFlowDiagram();
        DrawGeneralSettings();
        DrawCreateButton();

        EditorGUILayout.EndScrollView();
    }

    private void DrawHeader()
    {
        EditorGUILayout.Space(4);
        GUILayout.Label("Station Builder", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Configure your station as a flow: inputs on the left, station in the center, outputs on the right. " +
            "Hover over labels for detailed explanations.",
            MessageType.Info);
        EditorGUILayout.Space(2);
        stationName = EditorGUILayout.TextField("Station Name", stationName);
        EditorGUILayout.Space(4);
    }

    private void DrawFlowDiagram()
    {
        float arrowIconSize = 24f;
        float centerBandHeight = StationPreviewSize + 24;
        var arrowStyle = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, fontSize = 16 };

        EditorGUILayout.BeginHorizontal();

        // === LEFT: INPUTS ===
        EditorGUILayout.BeginVertical(GUILayout.Width(FlowColumnWidth), GUILayout.MinHeight(centerBandHeight));
        DrawInputsPanel();
        EditorGUILayout.EndVertical();

        // === CENTER BAND: Arrow | Station | Arrow (fixed height for alignment) ===
        EditorGUILayout.BeginHorizontal(GUILayout.Height(centerBandHeight), GUILayout.ExpandHeight(false));

        // Left arrow
        EditorGUILayout.BeginVertical(GUILayout.Width(ArrowWidth));
        GUILayout.FlexibleSpace();
        var arrowRect = GUILayoutUtility.GetRect(arrowIconSize, arrowIconSize);
        GUI.Label(arrowRect, "→", arrowStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        // Station icon
        EditorGUILayout.BeginVertical(GUILayout.Width(StationPreviewSize + 24));
        GUILayout.FlexibleSpace();
        var centerReserved = GUILayoutUtility.GetRect(StationPreviewSize, StationPreviewSize);
        var centerBgRect = new Rect(centerReserved.x - 6, centerReserved.y - 6, centerReserved.width + 12, centerReserved.height + 12);
        EditorGUI.DrawRect(centerBgRect, new Color(0.25f, 0.25f, 0.28f, 0.8f));
        stationGraphic = (Sprite)EditorGUI.ObjectField(centerReserved, stationGraphic, typeof(Sprite), false);
        previewRect = centerReserved;

        if (stationGraphic != null)
        {
            Texture previewTex = AssetPreview.GetAssetPreview(stationGraphic);
            if (previewTex != null)
            {
                GUI.DrawTexture(previewRect, previewTex, ScaleMode.ScaleToFit, true, 0, Color.white, 0, 0);
            }
            else if (stationGraphic.texture != null)
            {
                Texture tex = stationGraphic.texture;
                Rect texCoords = new Rect(
                    stationGraphic.textureRect.x / tex.width,
                    (tex.height - stationGraphic.textureRect.y - stationGraphic.textureRect.height) / tex.height,
                    stationGraphic.textureRect.width / tex.width,
                    stationGraphic.textureRect.height / tex.height);
                GUI.DrawTextureWithTexCoords(previewRect, tex, texCoords, true);
                Repaint();
            }
            else
            {
                var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
                GUI.Label(previewRect, "Loading...", style);
                Repaint();
            }
        }
        else
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontSize = 10 };
            GUI.Label(previewRect, "Drop sprite\nhere", style);
        }

        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        // Right arrow
        EditorGUILayout.BeginVertical(GUILayout.Width(ArrowWidth));
        GUILayout.FlexibleSpace();
        var arrowOutRect = GUILayoutUtility.GetRect(arrowIconSize, arrowIconSize);
        GUI.Label(arrowOutRect, "→", arrowStyle);
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();

        // === RIGHT: OUTPUTS ===
        EditorGUILayout.BeginVertical(GUILayout.Width(FlowColumnWidth), GUILayout.MinHeight(centerBandHeight));
        DrawOutputsPanel();
        EditorGUILayout.EndVertical();

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);
    }

    private void DrawResourceIcon(Resource resource, float size)
    {
        if (resource == null || resource.icon == null) return;

        var iconRect = GUILayoutUtility.GetRect(size, size);
        Texture previewTex = AssetPreview.GetAssetPreview(resource.icon);
        if (previewTex != null)
        {
            GUI.DrawTexture(iconRect, previewTex, ScaleMode.ScaleToFit, true);
        }
        else if (resource.icon.texture != null)
        {
            Texture tex = resource.icon.texture;
            Rect texCoords = new Rect(
                resource.icon.textureRect.x / tex.width,
                (tex.height - resource.icon.textureRect.y - resource.icon.textureRect.height) / tex.height,
                resource.icon.textureRect.width / tex.width,
                resource.icon.textureRect.height / tex.height);
            GUI.DrawTextureWithTexCoords(iconRect, tex, texCoords, true);
            Repaint();
        }
    }

    private void DrawInputsPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(FlowColumnWidth - 12));
        int savedIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        GUILayout.Label("INPUTS", EditorStyles.miniBoldLabel);
        EditorGUILayout.Space(2);

        var r1 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
        consumeResource = EditorGUI.ToggleLeft(r1, ConsumeResourceTip, consumeResource);
        if (consumeResource)
        {
            if (consumeResources.Count == 0)
                consumeResources.Add(null);
            EditorGUI.indentLevel = 1;
            for (int i = 0; i < consumeResources.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                consumeResources[i] = (Resource)EditorGUILayout.ObjectField(ConsumeResourceSlotTip, consumeResources[i], typeof(Resource), false);
                DrawResourceIcon(consumeResources[i], 24f);
                if (GUILayout.Button("−", GUILayout.Width(20)))
                {
                    consumeResources.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+ Add Input Resource"))
            {
                consumeResources.Add(null);
            }
        }
        EditorGUILayout.Space(2);

        var r2 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
        consumeCapital = EditorGUI.ToggleLeft(r2, ConsumeCapitalTip, consumeCapital);
        if (consumeCapital)
        {
            EditorGUI.indentLevel = 1;
            capitalInputAmount = EditorGUILayout.IntField(CapitalInputAmountTip, Mathf.Max(1, capitalInputAmount));
        }
        EditorGUILayout.Space(2);

        var r3 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
        consumptionCompletesGoals = EditorGUI.ToggleLeft(r3, ConsumptionCompletesGoalsTip, consumptionCompletesGoals);

        EditorGUI.indentLevel = savedIndent;
        EditorGUILayout.EndVertical();
    }

    private void DrawOutputsPanel()
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox, GUILayout.Width(FlowColumnWidth - 12));
        int savedIndent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        GUILayout.Label("OUTPUTS", EditorStyles.miniBoldLabel);
        EditorGUILayout.Space(2);

        var r1 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
        produceResource = EditorGUI.ToggleLeft(r1, ProduceResourceTip, produceResource);
        if (produceResource)
        {
            if (produceResources.Count == 0)
                produceResources.Add(null);
            EditorGUI.indentLevel = 1;
            for (int i = 0; i < produceResources.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                produceResources[i] = (Resource)EditorGUILayout.ObjectField(ProduceResourceSlotTip, produceResources[i], typeof(Resource), false);
                DrawResourceIcon(produceResources[i], 24f);
                if (GUILayout.Button("−", GUILayout.Width(20)))
                {
                    produceResources.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("+ Add Output Resource"))
            {
                produceResources.Add(null);
            }
        }
        EditorGUILayout.Space(2);

        var r2 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
        produceCapital = EditorGUI.ToggleLeft(r2, ProduceCapitalTip, produceCapital);
        if (produceCapital)
        {
            EditorGUI.indentLevel = 1;
            capitalOutputAmount = EditorGUILayout.IntField(CapitalOutputAmountTip, Mathf.Max(1, capitalOutputAmount));
        }
        EditorGUILayout.Space(2);

        var r3 = EditorGUILayout.GetControlRect(GUILayout.ExpandWidth(true));
        productionCompletesGoals = EditorGUI.ToggleLeft(r3, ProductionCompletesGoalsTip, productionCompletesGoals);

        EditorGUI.indentLevel = savedIndent;
        EditorGUILayout.EndVertical();
    }

    private void DrawGeneralSettings()
    {
        GUILayout.Label("General Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space(2);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(FlowColumnWidth));
        isSingleUse = EditorGUILayout.Toggle(SingleUseTip, isSingleUse);
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        canBeWorked = EditorGUILayout.Toggle(CanBeWorkedTip, canBeWorked);
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space(2);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.BeginVertical(GUILayout.Width(FlowColumnWidth));
        if (canBeWorked)
        {
            workDuration = EditorGUILayout.FloatField(WorkDurationTip, Mathf.Max(0.1f, workDuration));
        }
        else
        {
            productionInterval = EditorGUILayout.FloatField(ProductionIntervalTip, Mathf.Max(0.1f, productionInterval));
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();
        if (canBeWorked)
        {
            typeOfProduction = (Station.interactionType)EditorGUILayout.EnumPopup(
                new GUIContent("Production Trigger", "whenWorked / whenResourcesConsumed / cycle / automatic"),
                typeOfProduction);
            typeOfConsumption = (Station.interactionType)EditorGUILayout.EnumPopup(
                new GUIContent("Consumption Trigger", "whenWorked / whenResourcesConsumed / cycle / automatic"),
                typeOfConsumption);
        }
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space(4);
    }

    private void DrawCreateButton()
    {
        EditorGUILayout.Space(4);
        createMode = (CreateMode)EditorGUILayout.EnumPopup(
            new GUIContent("Create As", "ScriptableObject: saves station data as an asset. Prefab in Scene: creates a full station GameObject in the scene."),
            createMode);

        EditorGUILayout.Space(2);
        var createRect = GUILayoutUtility.GetRect(0, 36);
        createRect.x += 20;
        createRect.width -= 40;

        bool hasConsumeResources = !consumeResource || consumeResources.Exists(r => r != null);
        bool hasProduceResources = !produceResource || produceResources.Exists(r => r != null);
        bool canCreate = hasConsumeResources && hasProduceResources;

        EditorGUI.BeginDisabledGroup(!canCreate);
        string buttonLabel = createMode == CreateMode.ScriptableObject ? "Create Station Data (SO)" : "Create Station Prefab in Scene";
        if (GUI.Button(createRect, buttonLabel))
        {
            if (createMode == CreateMode.ScriptableObject)
                CreateStationDataSO();
            else
                CreateStationPrefab();
        }
        EditorGUI.EndDisabledGroup();

        if (!canCreate)
        {
            EditorGUILayout.HelpBox(
                "When 'Consume Resources' or 'Produce Resources' is enabled, add at least one resource.",
                MessageType.Warning);
        }

        EditorGUILayout.Space(6);
    }

    private void CreateStationDataSO()
    {
        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Databases/Stations");

        StationDataSO data = ScriptableObject.CreateInstance<StationDataSO>();
        data.stationName = stationName;
        data.stationGraphic = stationGraphic;
        data.consumeResource = consumeResource;
        data.produceResource = produceResource;
        data.consumes = new List<Resource>();
        foreach (var r in consumeResources)
            if (r != null) data.consumes.Add(r);
        data.produces = new List<Resource>();
        foreach (var r in produceResources)
            if (r != null) data.produces.Add(r);
        data.isSingleUse = isSingleUse;
        data.destroyAfterSingleUse = isSingleUse;
        data.capitalInput = consumeCapital;
        data.capitalOutput = produceCapital;
        data.capitalInputAmount = capitalInputAmount;
        data.capitalOutputAmount = capitalOutputAmount;
        data.completesGoals_consumption = consumptionCompletesGoals;
        data.completesGoals_production = productionCompletesGoals;
        data.canBeWorked = canBeWorked;
        data.workDuration = workDuration;
        data.productionInterval = productionInterval;
        data.typeOfProduction = typeOfProduction;
        data.typeOfConsumption = typeOfConsumption;

        string assetPath = $"Assets/Game Assemblies/Databases/Stations/{stationName}.asset";
        AssetDatabase.CreateAsset(data, assetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = data;
        EditorGUIUtility.PingObject(data);
        Debug.Log($"Station Builder: Created Station Data SO '{stationName}' at {assetPath}");
    }

    private void CreateStationPrefab()
    {
        GameObject templatePrefab = SA_AssetPathHelper.FindPrefab(TemplatePrefabPath);
        if (templatePrefab == null)
        {
            Debug.LogError($"Station Builder: Template prefab not found at {TemplatePrefabPath}");
            return;
        }

        var scene = SceneManager.GetActiveScene();
        if (!scene.IsValid())
        {
            Debug.LogError("Station Builder: No active scene. Open a scene first.");
            return;
        }

        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(templatePrefab, scene);
        instance.name = stationName;

        // Parent under Stations
        GameObject stationsRoot = GameObject.Find("Stations");
        if (stationsRoot == null)
        {
            stationsRoot = new GameObject("Stations");
            stationsRoot.transform.position = Vector3.zero;
        }
        instance.transform.SetParent(stationsRoot.transform);
        instance.transform.position = Vector3.zero;

        Station station = instance.GetComponent<Station>();
        if (station == null)
        {
            Debug.LogError("Station Builder: Template prefab has no Station component.");
            return;
        }

        // Apply configuration
        station.consumeResource = consumeResource;
        station.produceResource = produceResource;
        station.consumes.Clear();
        if (consumeResource)
        {
            foreach (var r in consumeResources)
                if (r != null) station.consumes.Add(r);
        }
        station.produces.Clear();
        if (produceResource)
        {
            foreach (var r in produceResources)
                if (r != null) station.produces.Add(r);
        }

        station.isSingleUse = isSingleUse;
        station.destroyAfterSingleUse = isSingleUse;

        station.capitalOutput = produceCapital;
        station.capitalOutputAmount = produceCapital ? capitalOutputAmount : 0;
        station.capitalInput = consumeCapital;
        station.capitalInputAmount = consumeCapital ? capitalInputAmount : 0;

        station.completesGoals_consumption = consumptionCompletesGoals;
        station.completesGoals_production = productionCompletesGoals;

        station.canBeWorked = canBeWorked;
        station.workDuration = workDuration;
        station.productionInterval = productionInterval;
        station.typeOfProduction = canBeWorked ? typeOfProduction : Station.interactionType.automatic;
        station.typeOfConsumption = canBeWorked ? typeOfConsumption : Station.interactionType.automatic;

        if (!canBeWorked)
        {
            station.typeOfProduction = Station.interactionType.automatic;
            station.typeOfConsumption = Station.interactionType.automatic;
        }

        // Station graphic
        if (stationGraphic != null)
        {
            var renderers = instance.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var r in renderers)
            {
                if (r.GetComponent<Canvas>() == null && r.transform.GetComponentInParent<Canvas>() == null)
                {
                    r.sprite = stationGraphic;
                    station.normalSprite = stationGraphic;
                    break;
                }
            }
        }

        // Wire input/output areas
        if (station.inputArea != null)
        {
            station.inputArea.requirements.Clear();
            if (consumeResource)
            {
                foreach (var r in consumeResources)
                    if (r != null) station.inputArea.requirements.Add(r);
            }
            station.inputArea.gameObject.SetActive(consumeResource);
        }
        if (station.outputArea != null)
        {
            station.outputArea.gameObject.SetActive(produceResource);
        }

        Selection.activeGameObject = instance;
        EditorGUIUtility.PingObject(instance);
        SceneView.lastActiveSceneView?.FrameSelected();

        Debug.Log($"Station Builder: Created '{stationName}' in scene.");
    }
}
