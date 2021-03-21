using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for setting a float property to 2 defined values.
    /// </summary>
    /// <remarks>
    /// <para>This control can be used to set a float property between 2 values, which can come useful when there's a need to toggle on and off features using a float value in shader.</para>
    /// <para>Both the enabled and disabled state can be set to give the material property a specific float value. If not set in the constructor they will be set
    /// to 0 for disabled and 1 for enabled.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // adds a toggle that toggles between 0 and 1
    /// this.AddToggleControl("_ExampleProperty");
    /// // adds a toggle that toggles between 3 and 7
    /// this.AddToggleControl("_ExampleProperty", 3, 7); 
    /// </code>
    /// </example>
    public class ToggleControl : PropertyControl
    {
        /// <summary>
        /// Float value that the Enabled bool gets converted if false.
        /// </summary>
        /// <value>
        /// Float value of the property if the toggle is disabled.
        /// </value>
        protected readonly float falseValue;

        /// <summary>
        /// Float value that the Enabled bool gets converted if true.
        /// </summary>
        /// <value>
        /// Float value of the proeprty if the toggle is enabled
        /// </value>
        protected readonly float trueValue;

        /// <summary>
        /// Boolean indicating if the toggle is enabled or not.
        /// </summary>
        /// <value>
        /// True if the toggle is enabled, false otherwise.
        /// </value>
        public bool ToggleEnabled => Math.Abs((Property?.floatValue ?? 0) - trueValue) < 0.001;

        /// <summary>
        /// Default constructor of <see cref="ToggleControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="falseValue">Float value that the material property will have if the checkbox is not checked. Optional (default: 0).</param>
        /// <param name="trueValue">Float value that the material property will have if the checkbox is checked. Optional (default: 1).</param>
        /// <returns>A new <see cref="ToggleControl"/> object.</returns>
        public ToggleControl(string propertyName, float falseValue = 0, float trueValue = 1) : base(propertyName)
        {
            this.falseValue = falseValue;
            this.trueValue = trueValue;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUI.showMixedValue = Property.hasMixedValue;

            EditorGUI.BeginChangeCheck();
            bool toggle = EditorGUILayout.Toggle(Content, ToggleEnabled);
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(Property.displayName);
                Property.floatValue = toggle ? trueValue : falseValue;
            }

            EditorGUI.showMixedValue = false;
        }
    }
}