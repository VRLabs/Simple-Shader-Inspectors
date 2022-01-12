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
            this.AddTextureControl("_MainTex", "_Color").WithAlias("Main texture").WithShowTilingAndOffset(true);

            _layersSection = this.AddActivatableSection("_AdditionalMasksEnable").WithAlias("Layers header")
                .WithBackgroundColor(Color.cyan).WithAreControlsInHeader(true);
            _layersSection.AddTextureGeneratorControl("_AdditionalMasks").WithAlias("Layers mask").WithShowTilingAndOffset(true);
            _layersSection.AddSpaceControl();

            _rChannel = _layersSection.AddSection().WithAlias("Red channel");
            _rChannel.AddColorControl("_RColor").WithAlias("Red color");
            _rChannel.AddToggleControl("_REmissive").WithAlias("Red emissive");

            _gChannel = _layersSection.AddSection().WithAlias("Green channel").WithShowFoldoutArrow(false);
            _gChannel.AddColorControl("_GColor").WithAlias("Green color");
            _gChannel.AddToggleControl("_GEmissive").WithAlias("Green emissive");

            _bChannel = _layersSection.AddSection().WithAlias("Blue channel").WithBackgroundStyle(Styles.Bubble);
            _bChannel.AddColorControl("_BColor").WithAlias("Blue color");
            _bChannel.AddToggleControl("_BEmissive").WithAlias("Blue emissive");

            _aChannel = _layersSection.AddSection().WithAlias("Alpha channel").WithLabelStyle(Styles.RightLabel);
            _aChannel.AddColorControl("_AColor").WithAlias("Alpha color");
            _aChannel.AddToggleControl("_AEmissive").WithAlias("Alpha emissive");

            _layersSection.AddLabelControl("OrderedLabel");
            _group = _layersSection.AddOrderedSectionGroup("Section group");
            _ordered1 = _group.AddOrderedSection("_OrderedSection1").WithAlias("Ordered1");
            _ordered1.AddColorControl("_RColor").WithAlias("Red color");
            _ordered1.AddToggleControl("_REmissive").WithAlias("Red emissive");

            _ordered2 = _group.AddOrderedSection("_OrderedSection2").WithAlias("Ordered2");
            _ordered2.AddColorControl("_GColor").WithAlias("Green color");
            _ordered2.AddToggleControl("_GEmissive").WithAlias("Green emissive");

            _ordered3 = _group.AddOrderedSection("_OrderedSection3").WithAlias("Ordered3");
            _ordered3.AddColorControl("_BColor").WithAlias("Blue color");
            _ordered3.AddToggleControl("_BEmissive").WithAlias("Blue emissive");

            _ordered4 = _group.AddOrderedSection("_OrderedSection4").WithAlias("Ordered4");
            _ordered4.AddColorControl("_AColor").WithAlias("Alpha color");
            _ordered4.AddToggleControl("_AEmissive").WithAlias("Alpha emissive");

            _rimSection = this.AddActivatableSection("_RimLightEnable").WithAlias("Rim light header")
                .WithBackgroundColor(Color.yellow);
            _rimSection.AddColorControl("_RimLightColor").WithAlias("Rim color");
            _rimSection.AddPropertyControl("_RimLightPower").WithAlias("Rim power");
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