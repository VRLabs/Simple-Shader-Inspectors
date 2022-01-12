using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors;
using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectorsExamples
{
    public class SimpleShaderGUI : SimpleShaderInspector
    {
        private ToggleListControl _toggle;
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_Color").WithAlias("Main texture").WithShowTilingAndOffset(true);
            _toggle = this.AddToggleListControl("_EnableNormal").WithAlias("Normal toggle");
            
            _toggle.AddTextureControl("_BumpMap", "_BumpIntensity").WithAlias("Normal map").WithShowTilingAndOffset(true);
            _toggle.AddHelpBoxControl("HelpBox").WithBoxType(MessageType.Info).WithIsWideBox(false);
        }

        protected override void Header()
        {
            GUILayout.Label("Simple shader editor");
            EditorGUILayout.Space();
            GUILayout.Label("Features shown by this editor:");
            EditorGUI.indentLevel++;
            GUILayout.Label("- Header");
            GUILayout.Label("- Footer");
            GUILayout.Label("- Texture control");
            GUILayout.Label("- Toggle control with hideable content inside");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }

        protected override void Footer()
        {
            GUILayout.Label("This is a basic footer");
        }
    }
}