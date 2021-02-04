using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors;
using VRLabs.SimpleShaderInspectors.Controls;
using VRLabs.SimpleShaderInspectors.Controls.Sections;

namespace VRLabs.SimpleShaderInspectorsExamples
{
    public class SimpleSectionedShaderGUI : SimpleShaderInspector
    {
        private ActivatableSection _layersSection;
        private ActivatableSection _rimSection;
        private Section _rChannel;
        private Section _gChannel;
        private Section _bChannel;
        private Section _aChannel;
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_Color").Alias("Main texture").SetShowUvOptions(true);

            _layersSection = this.AddActivatableSection("_AdditionalMasksEnable").Alias("Layers header")
                .SetBackgroundColor(Color.cyan).SetAreControlsInHeader(true);
            _layersSection.AddTextureGeneratorControl("_AdditionalMasks").Alias("Layers mask").SetShowUvOptions(true);
            _layersSection.AddSpaceControl();

            _rChannel = _layersSection.AddSection("Red channel");
            _rChannel.AddColorControl("_RColor").Alias("Red color");
            _rChannel.AddToggleControl("_REmissive").Alias("Red emissive");

            _gChannel = _layersSection.AddSection("Green channel").SetShowFoldoutArrow(false);
            _gChannel.AddColorControl("_GColor").Alias("Green color");
            _gChannel.AddToggleControl("_GEmissive").Alias("Green emissive");

            _bChannel = _layersSection.AddSection("Blue channel").SetBackgroundStyle(Styles.Bubble);
            _bChannel.AddColorControl("_BColor").Alias("Blue color");
            _bChannel.AddToggleControl("_BEmissive").Alias("Blue emissive");

            _aChannel = _layersSection.AddSection("Alpha channel").SetLabelStyle(Styles.RightLabel);
            _aChannel.AddColorControl("_AColor").Alias("Alpha color");
            _aChannel.AddToggleControl("_AEmissive").Alias("Alpha emissive");

            _rimSection = this.AddActivatableSection("_RimLightEnable").Alias("Rim light header")
                .SetBackgroundColor(Color.yellow);
            _rimSection.AddColorControl("_RimLightColor").Alias("Rim color");
            _rimSection.AddPropertyControl("_RimLightPower").Alias("Rim power");
        }

        protected override void Header()
        {
            GUILayout.Label("Simple sectioned shader editor");
            EditorGUILayout.Space();
            GUILayout.Label("Features shown by this editor:");
            EditorGUI.indentLevel++;
            GUILayout.Label("- Header");
            GUILayout.Label("- Texture control");
            GUILayout.Label("- Texture generator control");
            GUILayout.Label("- Default property control");
            GUILayout.Label("- Color control");
            GUILayout.Label("- Toggle control");
            GUILayout.Label("- Space control");
            GUILayout.Label("- Sections");
            GUILayout.Label("- Activatable Sections");
            GUILayout.Label("- Look customizations in sections");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}