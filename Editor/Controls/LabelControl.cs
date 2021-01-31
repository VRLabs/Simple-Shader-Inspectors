using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a label without any property.
    /// </summary>
    public class LabelControl : SimpleControl
    {
        /// <summary>
        /// GUIStyle for the LabelStyle
        /// </summary>
        [Chainable] public GUIStyle LabelStyle { get; set; }

        /// <summary>
        /// Default constructor of <see cref="LabelControl"/>.
        /// </summary>
        /// <param name="alias">Alias of the control.</param>
        /// <returns>A new <see cref="LabelControl"/> object.</returns>
        public LabelControl(string alias) : base(alias)
        {
            LabelStyle = EditorStyles.label;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUILayout.LabelField(Content, LabelStyle);
        }
    }
}