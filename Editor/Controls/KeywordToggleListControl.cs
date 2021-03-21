using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// <para>Represents a control with a checkbox for setting a keyword.</para>
    /// <para>Also shows and hides a list of controls based on its state.</para>
    /// </summary>
    /// <remarks>
    /// it's effectively a combination of <see cref="KeywordToggleControl"/> and <see cref="ControlContainer"/>, where the controls list is enabled based on the keyword
    /// enable state.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddKeywordToggleListControl("KEYWORD_TO_TOGGLE");
    /// </code>
    /// </example>
    public class KeywordToggleListControl : KeywordToggleControl, IControlContainer
    {
        /// <summary>
        /// List of controls under this control.
        /// </summary>
        /// <value>
        /// All controls that have been added by extension methods.
        /// </value>
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
                foreach (var control in Controls)
                    control.DrawControl(materialEditor);
                
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