using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TeamScoreDisplay))]
public class SA_TeamScoreDisplayEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        TeamScoreDisplay display = (TeamScoreDisplay)target;

        EditorGUILayout.Space(4);

        SerializedProperty displayTypeProp = serializedObject.FindProperty("displayType");
        EditorGUILayout.PropertyField(displayTypeProp);

        TeamScoreDisplayType displayType = (TeamScoreDisplayType)displayTypeProp.enumValueIndex;

        if (displayType == TeamScoreDisplayType.PlayerScore)
        {
            string[] playerOptions = new[] { "Player 1", "Player 2", "Player 3", "Player 4" };
            SerializedProperty playerSlotProp = serializedObject.FindProperty("playerSlot");
            int slot = Mathf.Clamp(playerSlotProp.intValue, 0, 3);
            int newSlot = EditorGUILayout.Popup("Player", slot, playerOptions);
            if (newSlot != slot)
                playerSlotProp.intValue = newSlot;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("playerList"));
        }
        else if (displayType == TeamScoreDisplayType.TeamScore)
        {
            string[] teamOptions = new[] { "Team 1", "Team 2", "Team 3" };
            SerializedProperty teamSlotProp = serializedObject.FindProperty("teamSlot");
            int slot = Mathf.Clamp(teamSlotProp.intValue, 0, 2);
            int newSlot = EditorGUILayout.Popup("Team", slot, teamOptions);
            if (newSlot != slot)
                teamSlotProp.intValue = newSlot;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("addName"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("scoreLabel"));

        if (TeamManager.Instance == null)
            EditorGUILayout.HelpBox("No TeamManager in scene. Add a TeamManager for the score to display.", MessageType.Info);

        serializedObject.ApplyModifiedProperties();
    }
}
