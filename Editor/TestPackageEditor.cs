using UnityEngine;
using UnityEditor;
using GameAssemblyLab.TestPackage;

namespace GameAssemblyLab.TestPackage.Editor
{
    /// <summary>
    /// Custom editor for the package.
    /// </summary>
    [CustomEditor(typeof(TestPackage))]
    public class TestPackageEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            // Add custom editor GUI here
        }
    }
}
