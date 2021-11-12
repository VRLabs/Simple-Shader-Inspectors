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
        private OrderedSectionGroup _group;
        private OrderedSection _ordered1;
        private OrderedSection _ordered2;
        private OrderedSection _ordered3;
        private OrderedSection _ordered4;
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_Color").Alias("Main texture").SetShowTilingAndOffset(true);

            _layersSection = this.AddActivatableSection("_AdditionalMasksEnable").Alias("Layers header")
                .SetBackgroundColor(Color.cyan).SetAreControlsInHeader(true);
            _layersSection.AddTextureGeneratorControl("_AdditionalMasks").Alias("Layers mask").SetShowTilingAndOffset(true);
            _layersSection.AddSpaceControl();

            _rChannel = _layersSection.AddSection().Alias("Red channel");
            _rChannel.AddColorControl("_RColor").Alias("Red color");
            _rChannel.AddToggleControl("_REmissive").Alias("Red emissive");

            _gChannel = _layersSection.AddSection().Alias("Green channel").SetShowFoldoutArrow(false);
            _gChannel.AddColorControl("_GColor").Alias("Green color");
            _gChannel.AddToggleControl("_GEmissive").Alias("Green emissive");

            _bChannel = _layersSection.AddSection().Alias("Blue channel").SetBackgroundStyle(Styles.Bubble);
            _bChannel.AddColorControl("_BColor").Alias("Blue color");
            _bChannel.AddToggleControl("_BEmissive").Alias("Blue emissive");

            _aChannel = _layersSection.AddSection().Alias("Alpha channel").SetLabelStyle(Styles.RightLabel);
            _aChannel.AddColorControl("_AColor").Alias("Alpha color");
            _aChannel.AddToggleControl("_AEmissive").Alias("Alpha emissive");

            _layersSection.AddLabelControl("OrderedLabel");
            _group = _layersSection.AddOrderedSectionGroup("Section group");
            _ordered1 = _group.AddOrderedSection("_OrderedSection1").Alias("Ordered1");
            _ordered1.AddColorControl("_RColor").Alias("Red color");
            _ordered1.AddToggleControl("_REmissive").Alias("Red emissive");

            _ordered2 = _group.AddOrderedSection("_OrderedSection2").Alias("Ordered2");
            _ordered2.AddColorControl("_GColor").Alias("Green color");
            _ordered2.AddToggleControl("_GEmissive").Alias("Green emissive");

            _ordered3 = _group.AddOrderedSection("_OrderedSection3").Alias("Ordered3");
            _ordered3.AddColorControl("_BColor").Alias("Blue color");
            _ordered3.AddToggleControl("_BEmissive").Alias("Blue emissive");

            _ordered4 = _group.AddOrderedSection("_OrderedSection4").Alias("Ordered4");
            _ordered4.AddColorControl("_AColor").Alias("Alpha color");
            _ordered4.AddToggleControl("_AEmissive").Alias("Alpha emissive");

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
            GUILayout.Label("- Ordered Sections");
            GUILayout.Label("- Look customizations in sections");
            EditorGUI.indentLevel--;
            EditorGUILayout.Space();
        }
    }
}