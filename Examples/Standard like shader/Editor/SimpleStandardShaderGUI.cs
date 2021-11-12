using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors;
using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectorsExamples
{
    public class SimpleStandardShaderGUI : SimpleShaderInspector
    {
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_Color").Alias("Main texture").SetShowTilingAndOffset(true);
            this.AddTextureControl("_BumpMap", "_BumpIntensity").Alias("Normal map").SetShowTilingAndOffset(true);
            this.AddTextureGeneratorControl("_MultiMask").Alias("Multi mask").SetShowTilingAndOffset(true);
            this.AddSpaceControl();
            this.AddLabelControl("MaskControls");
            this.AddPropertyControl("_Metallic");
            this.AddPropertyControl("_Smoothness");
            this.AddPropertyControl("_Occlusion");
            this.AddColorControl("_EmissionColor", false);
        }

        protected override void Header()
        {
            GUILayout.Label("Simple standard shader editor");
            EditorGUILayout.Space();
            GUILayout.Label("Features shown by this editor:");
            EditorGUI.indentLevel++;
            GUILayout.Label("- Header");
            GUILayout.Label("- Texture control");
            GUILayout.Label("- Texture generator control with default packer");
            GUILayout.Label("- Property control");
            GUILayout.Label("- Space control");
            GUILayout.Label("- Label control");
            GUILayout.Label("- Color control");
            GUILayout.Label("- Default aliases when a custom one is not set");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}