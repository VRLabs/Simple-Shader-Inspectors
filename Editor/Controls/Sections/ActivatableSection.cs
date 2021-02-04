using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Section that can be activated or deactivated thanks to a checkbox inside the header.
    /// </summary>
    public class ActivatableSection : Section, IAdditionalProperties
    {
        /// <summary>
        /// Float value that the Show bool gets converted if true.
        /// </summary>
        protected float enableValue;

        /// <summary>
        /// Float value that the Show bool gets converted if false.
        /// </summary>
        protected float disableValue;

        /// <summary>
        /// Extra properties array.
        /// </summary>
        public AdditionalProperty[] AdditionalProperties { get; set; }

        /// <summary>
        /// Has the property been updated this cycle?
        /// </summary>
        public bool HasActivatePropertyUpdated { get; protected set; }

        /// <summary>
        /// Boolean indicating if the section is enabled or not.
        /// </summary>
        public bool Enabled { get; protected set; }
        /// <summary>
        /// Constructor of <see cref="ActivatableSection"/> used when creating a property driven ActivatableSection.
        /// </summary>
        /// <param name="activatePropertyName">Material property that will drive the section enable state</param>
        /// <param name="showPropertyName">Material property that will drive the section open state</param>
        /// <param name="enableValue">Float value that the material property will have if the section is disabled, optional (default: 0).</param>
        /// <param name="disableValue">Float value that the material property will have if the section is enabled, optional (default: 1).</param>
        /// <param name="hideValue">Float value that the material property will have if the section is collapsed, optional (default: 0).</param>
        /// <param name="showValue">Float value that the material property will have if the section is visible, optional (default: 1).</param>
        public ActivatableSection(string activatePropertyName, string showPropertyName, float enableValue = 1,
            float disableValue = 0, float hideValue = 0, float showValue = 1) : base(showPropertyName, hideValue, showValue)
        {
            AdditionalProperties = new AdditionalProperty[1];
            AdditionalProperties[0] = new AdditionalProperty(activatePropertyName);
            this.disableValue = disableValue;
            this.enableValue = enableValue;
        }

        /// <summary>
        /// Default constructor of <see cref="ActivatableSection"/>.
        /// </summary>
        /// <param name="activatePropertyName">Material property that will drive the section enable state</param>
        /// <param name="enableValue">Float value that the material property will have if the section is disabled, optional (default: 0).</param>
        /// <param name="disableValue">Float value that the material property will have if the section is enabled, optional (default: 1).</param>
        public ActivatableSection(string activatePropertyName, float enableValue = 1, float disableValue = 0) : base()
        {
            AdditionalProperties = new AdditionalProperty[1];
            AdditionalProperties[0] = new AdditionalProperty(activatePropertyName);
            ControlAlias = activatePropertyName;
            this.disableValue = disableValue;
            this.enableValue = enableValue;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            SetupEnabled(materialEditor);

            EditorGUILayout.Space();

            Enabled = Math.Abs(AdditionalProperties[0].Property.floatValue - enableValue) < 0.001;

            // Begin header
            GUI.backgroundColor = BackgroundColor;
            EditorGUILayout.BeginVertical(BackgroundStyle);
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;

            Rect r = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            if (ShowFoldoutArrow)
            {
                Show = EditorGUILayout.Toggle(Show, EditorStyles.foldout, GUILayout.MaxWidth(15.0f));
            }
            EditorGUI.BeginChangeCheck();
            Enabled = EditorGUILayout.Toggle(Enabled, GUILayout.MaxWidth(20.0f));
            HasActivatePropertyUpdated = EditorGUI.EndChangeCheck();
            EditorGUILayout.LabelField(Content, LabelStyle);

            Show = GUI.Toggle(r, Show, GUIContent.none, new GUIStyle());
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                UpdateEnabled(materialEditor);
            }

            if (HasActivatePropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(AdditionalProperties[0].Property.displayName);
                AdditionalProperties[0].Property.floatValue = Enabled ? enableValue : disableValue;
            }
            EditorGUILayout.EndHorizontal();

            if (!AreControlsInHeader)
            {
                EditorGUILayout.EndVertical();
            }
            if (Show)
            {
                EditorGUI.BeginDisabledGroup(!Enabled);
                DrawControls(materialEditor);
                EditorGUI.EndDisabledGroup();
            }
            if (AreControlsInHeader)
            {
                EditorGUILayout.EndVertical();
            }
        }
    }
}