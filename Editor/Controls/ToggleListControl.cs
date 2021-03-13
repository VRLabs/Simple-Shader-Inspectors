using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for setting a float property to 2 defined values.
    /// Also shows and hides a list of controls based on its state.
    /// </summary>
    /// <remarks>
    /// It's effectively a combination of <see cref="ToggleListControl"/> and <see cref="ControlContainer"/>, where the controls list is displayed only when the toggle is enabled.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // adds a toggle that toggles between 0 and 1
    /// this.AddToggleListControl("_ExampleProperty");
    /// // adds a toggle that toggles between 3 and 7
    /// this.AddToggleListControl("_ExampleProperty", 3, 7); 
    /// </code>
    /// </example>
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
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to add controls. All controls added are stored in <see cref="Controls"/>
        /// </summary>
        /// <param name="control">Control to add.</param>
        public void AddControl(SimpleControl control) => Controls.Add(control);

        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to get the object's controls list.
        /// </summary>
        /// <returns><see cref="Controls"/></returns>
        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}