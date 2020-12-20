using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for setting a float property to 2 defined values.
    /// </summary>
    public class ToggleControl : PropertyControl
    {
        /// <summary>
        /// Float value that the Enabled bool gets converted if false.
        /// </summary>
        protected readonly float falseValue;

        /// <summary>
        /// Float value that the Enabled bool gets converted if true.
        /// </summary>
        protected readonly float trueValue;

        /// <summary>
        /// Boolean indicating if the toggle is enabled or not.
        /// </summary>
        public bool ToggleEnabled
        {
            get
            {
                return Math.Abs((Property?.floatValue ?? 0) - trueValue) < 0.001;
            }
            /*set
            {
                if (Property != null)
                    Property.floatValue = ToggleEnabled ? trueValue : falseValue;
            }*/
        }

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
    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="ToggleControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="falseValue">Float value that the material property will have if the checkbox is not checked. Optional (default: 0).</param>
        /// <param name="trueValue">Float value that the material property will have if the checkbox is checked. Optional (default: 1).</param>
        /// <returns>The <see cref="ToggleControl"/> object that has been added.</returns>
        public static ToggleControl AddToggleControl(this IControlContainer container, string propertyName, float falseValue = 0, float trueValue = 1)
        {
            ToggleControl control = new ToggleControl(propertyName, falseValue, trueValue);
            container.Controls.Add(control);
            return control;
        }
    }
}