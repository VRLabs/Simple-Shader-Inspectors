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
            this.AddTextureControl("_MainTex", "_Color").WithAlias("Main texture").WithShowTilingAndOffset(true);
            this.AddGradientTextureControl("_Ramp", "_RampColor").WithAlias("Ramp");
            this.AddPropertyControl("_ShadowIntensity").WithAlias("Shadow intensity");
            this.AddHelpBoxControl("InfoBox", "Main texture").WithBoxType(MessageType.Info);
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
            GUILayout.Label("- Info box control");
            GUILayout.Label("- Control places after a specific alias");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}