using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Section that can be activated or deactivated thanks to a checkbox inside the header.
    /// </summary>
    /// <remarks>
    /// This more advanced version of <see cref="Section"/> has in addition one checkbox to enable/disable the content of the section.
    /// When disabled the content is still visible, but cannot be edited.
    ///
    /// This section requires at least one material property to drive the enabled/disabled state, and it will use this material property name as alias.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Add an activatable section
    /// this.AddActivatableSection("_EnableProperty"); 
    ///
    /// // Add an activatable section, the property will have the values of 2 and 4 when off and on respectively
    /// this.AddActivatableSection("_EnableProperty", 2, 4); 
    ///
    /// // Add an activatable section with the show state being driven by a property
    /// this.AddActivatableSection("_EnableProperty", "_ShowProperty");
    ///
    /// // Add an activatable section, the properties will have set values when on and off, 2/4 for the enable one and 0/1 of the show one.
    /// this.AddActivatableSection("_EnableProperty", "_ShowProperty", 2, 4, 0, 1); 
    /// </code>
    /// </example>
    public class ActivatableSection : Section, IAdditionalProperties
    {
        /// <summary>
        /// Float value that the Show bool gets converted if true.
        /// </summary>
        /// <value>
        /// Value of the activate material property when the section is enabled.
        /// </value>
        protected float enableValue;

        /// <summary>
        /// Float value that the Show bool gets converted if false.
        /// </summary>
        /// <value>
        /// Value of the activate material property when the section is disabled.
        /// </value>
        protected float disableValue;

        /// <summary>
        /// Extra properties array. Implementation needed by <see cref="IAdditionalProperties"/>.
        /// </summary>
        /// <value>
        /// Array of <see cref="AdditionalProperties"/>.
        /// </value>
        /// <remarks>
        /// The Array will contain the following material properties:
        /// <list type="bullet">
        /// <item>
        /// <term>[0]: </term>
        /// <description>Property used for the enabled state</description>
        /// </item>
        /// </list>
        /// </remarks>
        public AdditionalProperty[] AdditionalProperties { get; set; }

        /// <summary>
        /// Has the property been updated this cycle?
        /// </summary>
        /// <value>
        /// True if the activate property has been updated, false otherwise.
        /// </value>
        public bool HasActivatePropertyUpdated { get; protected set; }

        /// <summary>
        /// Boolean indicating if the section is enabled or not.
        /// </summary>
        /// <value>
        /// True if the section is active, false otherwise.
        /// </value>
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
                Show = EditorGUILayout.Toggle(Show, EditorStyles.foldout, GUILayout.MaxWidth(15.0f));
            
            EditorGUI.BeginChangeCheck();
            Enabled = EditorGUILayout.Toggle(Enabled, GUILayout.MaxWidth(20.0f));
            HasActivatePropertyUpdated = EditorGUI.EndChangeCheck();
            EditorGUILayout.LabelField(Content, LabelStyle);

            Show = GUI.Toggle(r, Show, GUIContent.none, new GUIStyle());
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
                UpdateEnabled(materialEditor);

            if (HasActivatePropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(AdditionalProperties[0].Property.displayName);
                AdditionalProperties[0].Property.floatValue = Enabled ? enableValue : disableValue;
            }
            EditorGUILayout.EndHorizontal();

            if (!AreControlsInHeader)
                EditorGUILayout.EndVertical();
            
            if (Show)
            {
                EditorGUI.BeginDisabledGroup(!Enabled);
                DrawControls(materialEditor);
                EditorGUI.EndDisabledGroup();
            }
            
            if (AreControlsInHeader)
                EditorGUILayout.EndVertical();
        }
    }
}