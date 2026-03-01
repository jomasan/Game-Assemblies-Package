using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerResourceListDisplay))]
public class SA_PlayerResourceListDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        PlayerResourceListDisplay listDisplay = (PlayerResourceListDisplay)target;

        EditorGUILayout.Space(4);

        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerNameText"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rowPrefab"));

        string[] playerOptions = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };
        SerializedProperty playerSlotProp = serializedObject.FindProperty("playerSlot");
        int slot = Mathf.Clamp(playerSlotProp.intValue, 0, 3);
        int newSlot = EditorGUILayout.Popup("Player to track", slot, playerOptions);
        if (newSlot != slot)
            playerSlotProp.intValue = newSlot;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerList"));

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Backgrounds", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerBackgroundImage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerBackgroundColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showPlayerBackground"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("listBackgroundImage"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("listBackgroundColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showListBackground"));

        EditorGUILayout.Space(4);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("rowScaleFactor"));

        EditorGUILayout.Space(6);
        EditorGUILayout.LabelField("Resources to display", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("resourcesToDisplay"), true);

        if (GUILayout.Button("Populate from project (all Resources)"))
        {
            SA_DatabaseManager.Refresh(typeof(Resource));
            var entries = SA_DatabaseManager.GetEntries(typeof(Resource));
            SerializedProperty listProp = serializedObject.FindProperty("resourcesToDisplay");
            listProp.ClearArray();
            int index = 0;
            foreach (var entry in entries)
            {
                if (entry is Resource r && r != null)
                {
                    listProp.InsertArrayElementAtIndex(index);
                    listProp.GetArrayElementAtIndex(index).objectReferenceValue = r;
                    index++;
                }
            }
        }

        if (listDisplay.rowPrefab != null && listDisplay.rowPrefab.GetComponentInChildren<ResourceCounterDisplay>() == null)
            EditorGUILayout.HelpBox("Row prefab should contain a ResourceCounterDisplay (on it or a child).", MessageType.Warning);

        serializedObject.ApplyModifiedProperties();
    }
}
