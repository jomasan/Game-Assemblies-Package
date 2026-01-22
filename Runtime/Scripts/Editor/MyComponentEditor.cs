using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MyComponent))]
public class MyComponentEditor : Editor
{
    // For foldouts or toggles, you can store states here
    private bool showAdvancedOptions;

    public override void OnInspectorGUI()
    {
        // Get a reference to the script
        MyComponent myComponent = (MyComponent)target;

        // Update the serialized object (required for custom Editors)
        serializedObject.Update();

        // 1. Draw some fields normally
        EditorGUILayout.PropertyField(serializedObject.FindProperty("alwaysVisible"));

        // 2. Draw a toggle to decide if we show advanced stuff
        //    (Alternatively, you could use a foldout, see below)
        myComponent.showAdvanced = EditorGUILayout.Toggle("Show Advanced?", myComponent.showAdvanced);

        // 3. Conditionally show or hide advanced fields
        if (myComponent.showAdvanced)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("advancedOption1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("advancedOption2"));
        }

        // Apply modifications
        serializedObject.ApplyModifiedProperties();
    }
}
