using UnityEditor;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Control that handles the instancing option.
    /// </summary>
    /// <remarks>
    /// <para>Is the Simple Shader Inspectors equivalent of <c>MaterialEditor.EnableInstancingField</c>.</para>
    /// <para>It does not need an alias in order to work.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddEnableInstancingControl();
    /// </code>
    /// </example>
    public class EnableInstancingControl : SimpleControl
    {
        /// <summary>
        /// Default constructor of <see cref="EnableInstancingControl"/>.
        /// </summary>
        public EnableInstancingControl() : base("") { }

        /// <summary>
        /// Boolean indicating if the enable instancing control has updated.
        /// </summary>
        /// <value>
        /// True if the enable instancing control has been updated, false otherwise.
        /// </value>
        public bool HasInstancingUpdated { get; protected set; }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            materialEditor.EnableInstancingField();
            HasInstancingUpdated = EditorGUI.EndChangeCheck();
        }
    }
}