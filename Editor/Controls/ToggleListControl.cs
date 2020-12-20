using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for setting a float property to 2 defined values.
    /// Also shows and hides a list of controls based on its state.
    /// </summary>
    public class ToggleListControl : ToggleControl, IControlContainer
    {
        /// <summary>
        /// List of controls that can be hidden by this control.
        /// </summary>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// Default constructor of <see cref="ToggleListControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="falseValue">Float value that the material property will have if the checkbox is not checked. Optional (default: 0).</param>
        /// <param name="trueValue">Float value that the material property will have if the checkbox is checked. Optional (default: 1).</param>
        /// <returns>A new <see cref="ToggleListControl"/> object.</returns>
        public ToggleListControl(string propertyName, float falseValue = 0, float trueValue = 1) : base(propertyName, falseValue, trueValue)
        {
            Controls = new List<SimpleControl>();
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            base.ControlGUI(materialEditor);
            if (ToggleEnabled)
            {
                EditorGUI.indentLevel++;
                foreach (SimpleControl control in Controls)
                {
                    control.DrawControl(materialEditor);
                }
                EditorGUI.indentLevel--;
            }
        }
    }
    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="ToggleListControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="falseValue">Float value that the material property will have if the checkbox is not checked. Optional (default: 0).</param>
        /// <param name="trueValue">Float value that the material property will have if the checkbox is checked. Optional (default: 1).</param>
        /// <returns>The <see cref="ToggleListControl"/> object that has been added.</returns>
        public static ToggleListControl AddToggleListControl(this IControlContainer container, string propertyName, float falseValue = 0, float trueValue = 1)
        {
            ToggleListControl control = new ToggleListControl(propertyName, falseValue, trueValue);
            container.Controls.Add(control);
            return control;
        }
    }
}