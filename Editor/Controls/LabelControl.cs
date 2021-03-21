using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a label without any property.
    /// </summary>
    /// <remarks>
    /// <para>It can be seen ad the Simple Shader Inspectors version of <c>EditorGUILayout.LabelField</c>, with the advantage that it uses Simple Shader Inspectors' localization system for the
    /// label string.</para>
    /// <para>It is required to give an alias to the control, since it doesn't have anything that it could use as a default.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddLabelControl("AliasToUse");
    /// </code>
    /// </example>
    public class LabelControl : SimpleControl
    {
        /// <summary>
        /// Style used for the label control.
        /// </summary>
        /// <value>GUIStyle for the label.</value>
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