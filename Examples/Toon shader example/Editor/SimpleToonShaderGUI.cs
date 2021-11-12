using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors;
using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectorsExamples
{
    public class SimpleToonShaderGUI : SimpleShaderInspector
    {
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_Color").Alias("Main texture").SetShowTilingAndOffset(true);
            this.AddGradientTextureControl("_Ramp", "_RampColor").Alias("Ramp");
            this.AddPropertyControl("_ShadowIntensity").Alias("Shadow intensity");
        }

        protected override void Header()
        {
            GUILayout.Label("Basic toon shader editor");
            EditorGUILayout.Space();
            GUILayout.Label("Features shown by this editor:");
            EditorGUI.indentLevel++;
            GUILayout.Label("- Header");
            GUILayout.Label("- Texture control");
            GUILayout.Label("- Gradient texture control");
            GUILayout.Label("- Default property control");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}