using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ResourceCounterDisplay))]
public class SA_ResourceCounterDisplayEditor : Editor
{
    private string[] _resourceNames;
    private Resource[] _resourceAssets;
    private int _selectedIndex = -1;

    private void OnEnable()
    {
        RefreshResourceList();
    }

    private void RefreshResourceList()
    {
        SA_DatabaseManager.Refresh(typeof(Resource));
        var entries = SA_DatabaseManager.GetEntries(typeof(Resource));
        int count = entries.Count;
        _resourceNames = new string[count + 1];
        _resourceAssets = new Resource[count + 1];
        _resourceNames[0] = "(None)";
        _resourceAssets[0] = null;
        for (int i = 0; i < count; i++)
        {
            var r = entries[i] as Resource;
            _resourceAssets[i + 1] = r;
            _resourceNames[i + 1] = r != null ? r.resourceName : "(Missing)";
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        ResourceCounterDisplay display = (ResourceCounterDisplay)target;

        if (_resourceNames == null || _resourceNames.Length <= 1)
            RefreshResourceList();

        EditorGUILayout.Space(4);

        int currentIndex = 0;
        if (display.resourceToTrack != null && _resourceAssets != null)
        {
            for (int i = 1; i < _resourceAssets.Length; i++)
            {
                if (_resourceAssets[i] == display.resourceToTrack)
                {
                    currentIndex = i;
                    break;
                }
            }
        }

        SerializedProperty resourceToTrackProp = serializedObject.FindProperty("resourceToTrack");
        EditorGUI.BeginChangeCheck();
        int newIndex = EditorGUILayout.Popup("Resource to track", currentIndex, _resourceNames);
        if (EditorGUI.EndChangeCheck() && _resourceAssets != null && newIndex >= 0 && newIndex < _resourceAssets.Length)
        {
            Undo.RecordObject(serializedObject.targetObject, "Set Resource to track");
            resourceToTrackProp.objectReferenceValue = _resourceAssets[newIndex];
        }

        SerializedProperty displayModeProp = serializedObject.FindProperty("displayMode");
        EditorGUILayout.PropertyField(displayModeProp);

        if ((ResourceCountDisplayMode)displayModeProp.enumValueIndex == ResourceCountDisplayMode.OwnedByPlayer)
        {
            string[] playerOptions = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };
            SerializedProperty playerSlotProp = serializedObject.FindProperty("playerSlot");
            int slot = Mathf.Clamp(playerSlotProp.intValue, 0, 3);
            int newSlot = EditorGUILayout.Popup("Player", slot, playerOptions);
            if (newSlot != slot)
                playerSlotProp.intValue = newSlot;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerList"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("includeName"));

        SerializedProperty showIconProp = serializedObject.FindProperty("showIcon");
        EditorGUILayout.PropertyField(showIconProp);
        if (showIconProp.boolValue)
            EditorGUILayout.PropertyField(serializedObject.FindProperty("iconImage"));

        if (ResourceManager.Instance == null)
            EditorGUILayout.HelpBox("No ResourceManager in scene. Add a ResourceManager for the counter to display counts.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
