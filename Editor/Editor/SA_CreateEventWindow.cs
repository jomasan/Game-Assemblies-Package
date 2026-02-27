using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

///
/// Editor window to create a new Event asset. Events define triggers, modality (permanent or duration), and a list of effects.
/// Stored under Game Assemblies/Databases/Events. Editable in Database Inspector (Game Assemblies > Databases > Database Inspector > Events).
public class SA_CreateEventWindow : EditorWindow
{
    private string eventName = "New Event";
    private Sprite icon;
    private string description = "";
    private EventTriggerType triggerType = EventTriggerType.TriggeredByManager;
    private float triggerParamFloat;
    private int triggerParamInt;
    private Resource triggerResource;
    private ResourceGoalSO triggerGoal;
    private EventModality modality = EventModality.WithDuration;
    private float durationSeconds = 30f;
    private List<EventEffect> effects = new List<EventEffect>();
    private Vector2 _scrollPosition;

    [MenuItem("Game Assemblies/Events/Create Event")]
    public static void ShowWindow()
    {
        GetWindow<SA_CreateEventWindow>("Create Event");
    }

    private void OnGUI()
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

        GUILayout.Label("Create a New Event", EditorStyles.boldLabel);
        GUILayout.Space(8);
        EditorGUILayout.HelpBox(
            "Events can change policies, player speed, recipe inputs/outputs, decay, and more. They are triggered by the Event Manager or by conditions (time, score, goal, resource count). Effects can be permanent or last for a duration.",
            MessageType.Info);
        GUILayout.Space(12);

        EditorGUILayout.LabelField("Identity", EditorStyles.boldLabel);
        eventName = EditorGUILayout.TextField("Event Name", eventName);
        icon = (Sprite)EditorGUILayout.ObjectField("Icon (optional)", icon, typeof(Sprite), false);
        description = EditorGUILayout.TextArea(description, GUILayout.Height(40));
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Trigger", EditorStyles.boldLabel);
        triggerType = (EventTriggerType)EditorGUILayout.EnumPopup("Trigger Type", triggerType);
        if (triggerType == EventTriggerType.TimeElapsed || triggerType == EventTriggerType.ScoreThreshold)
            triggerParamFloat = EditorGUILayout.FloatField(triggerType == EventTriggerType.TimeElapsed ? "Seconds" : "Score threshold", Mathf.Max(0f, triggerParamFloat));
        if (triggerType == EventTriggerType.ResourceCountExceeds)
        {
            triggerResource = (Resource)EditorGUILayout.ObjectField("Resource", triggerResource, typeof(Resource), false);
            triggerParamInt = EditorGUILayout.IntField("Count threshold", Mathf.Max(0, triggerParamInt));
        }
        if (triggerType == EventTriggerType.GoalCompleted)
            triggerGoal = (ResourceGoalSO)EditorGUILayout.ObjectField("Goal", triggerGoal, typeof(ResourceGoalSO), false);
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Modality", EditorStyles.boldLabel);
        modality = (EventModality)EditorGUILayout.EnumPopup("Modality", modality);
        if (modality == EventModality.WithDuration)
            durationSeconds = EditorGUILayout.FloatField("Duration (seconds)", Mathf.Max(0.1f, durationSeconds));
        GUILayout.Space(8);

        EditorGUILayout.LabelField("Effects", EditorStyles.boldLabel);
        DrawEffectsList();
        GUILayout.Space(20);

        if (GUILayout.Button("Create"))
        {
            CreateEvent();
        }

        EditorGUILayout.EndScrollView();
    }

    private void DrawEffectsList()
    {
        if (effects == null) effects = new List<EventEffect>();

        for (int i = 0; i < effects.Count; i++)
        {
            if (effects[i] == null) effects[i] = new EventEffect();

            EventEffect e = effects[i];
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            e.effectType = (EventEffectType)EditorGUILayout.EnumPopup("Effect Type", e.effectType);
            e.effectParamFloat = EditorGUILayout.FloatField("Multiplier / Param", e.effectParamFloat);

            if (e.effectType == EventEffectType.ChangePolicy)
                e.effectPolicy = (PolicyDataSO)EditorGUILayout.ObjectField("Policy", e.effectPolicy, typeof(PolicyDataSO), false);
            if (e.effectType == EventEffectType.ScaleRecipeInputs || e.effectType == EventEffectType.ScaleRecipeOutputs)
            {
                e.effectRecipe = (RecipeSO)EditorGUILayout.ObjectField("Recipe (optional)", e.effectRecipe, typeof(RecipeSO), false);
                e.effectStation = (StationDataSO)EditorGUILayout.ObjectField("Station (optional)", e.effectStation, typeof(StationDataSO), false);
            }
            if (e.effectType == EventEffectType.ScaleWorkDuration)
                e.effectStation = (StationDataSO)EditorGUILayout.ObjectField("Station (optional)", e.effectStation, typeof(StationDataSO), false);

            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                effects.RemoveAt(i);
                i--;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            GUILayout.Space(4);
        }

        if (GUILayout.Button("+ Add effect"))
        {
            effects.Add(new EventEffect { effectType = EventEffectType.MultiplySpeed, effectParamFloat = 1f });
        }
    }

    private void CreateEvent()
    {
        if (string.IsNullOrWhiteSpace(eventName))
        {
            Debug.LogWarning("Create Event: Enter an event name.");
            return;
        }

        SA_AssetPathHelper.EnsureAssetPathDirectories("Game Assemblies/Databases/Events");

        EventSO asset = ScriptableObject.CreateInstance<EventSO>();
        asset.eventName = eventName;
        asset.icon = icon;
        asset.description = description ?? "";
        asset.triggerType = triggerType;
        asset.triggerParamFloat = triggerParamFloat;
        asset.triggerParamInt = triggerParamInt;
        asset.triggerResource = triggerResource;
        asset.triggerGoal = triggerGoal;
        asset.modality = modality;
        asset.durationSeconds = modality == EventModality.WithDuration ? Mathf.Max(0.1f, durationSeconds) : 0f;
        asset.effects = new List<EventEffect>();
        if (effects != null)
        {
            foreach (var e in effects)
            {
                if (e != null)
                    asset.effects.Add(new EventEffect
                    {
                        effectType = e.effectType,
                        effectParamFloat = e.effectParamFloat,
                        effectPolicy = e.effectPolicy,
                        effectRecipe = e.effectRecipe,
                        effectStation = e.effectStation
                    });
            }
        }

        string safeName = eventName.Replace('/', '-').Replace('\\', '-');
        string assetPath = $"Assets/Game Assemblies/Databases/Events/{safeName}.asset";
        AssetDatabase.CreateAsset(asset, assetPath);
        AssetDatabase.SaveAssets();

        Selection.activeObject = asset;
        EditorGUIUtility.PingObject(asset);

        Debug.Log("Event created: " + eventName);
    }
}
