using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for setting a keyword.
    /// Also shows and hides a list of controls based on its state.
    /// </summary>
    public class KeywordToggleListControl : KeywordToggleControl, IControlContainer
    {
        /// <summary>
        /// List of controls that can be hidden by this control.
        /// </summary>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// Default constructor of <see cref="KeywordToggleListControl"/>
        /// </summary>
        /// <param name="keyword">Keyword name.</param>
        /// <returns>A new <see cref="ToggleListControl"/> object.</returns>
        public KeywordToggleListControl(string keyword) : base(keyword)
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
        /// Creates a new control of type <see cref="KeywordToggleListControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="falseValue">Float value that the material property will have if the checkbox is not checked. Optional (default: 0).</param>
        /// <param name="trueValue">Float value that the material property will have if the checkbox is checked. Optional (default: 1).</param>
        /// <returns>The <see cref="KeywordToggleListControl"/> object that has been added.</returns>
        public static KeywordToggleListControl AddKeywordToggleListControl(this IControlContainer container, string keyword)
        {
            KeywordToggleListControl control = new KeywordToggleListControl(keyword);
            container.Controls.Add(control);
            return control;
        }
    }
}