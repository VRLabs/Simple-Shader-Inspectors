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

        public void AddControl(SimpleControl control)
        {
            Controls.Add(control);
        }

        public IEnumerable<SimpleControl> GetControlList()
        {
            return Controls;
        }
    }
}