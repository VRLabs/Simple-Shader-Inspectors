using UnityEditor;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Control that handles the material double sided GI option.
    /// </summary>
    /// <remarks>
    /// <para>Is the Simple Shader Inspectors equivalent of <c>MaterialEditor.DoubleSidedGIField</c>.</para>
    /// <para>It does not need an alias in order to work.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddDoubleSidedGIControl();
    /// </code>
    /// </example>
    public class DoubleSidedGIControl : SimpleControl
    {
        /// <summary>
        /// Default constructor of <see cref="DoubleSidedGIControl"/>.
        /// </summary>
        public DoubleSidedGIControl() : base("") { }

        /// <summary>
        /// Boolean indicating if the double sided GI has updated.
        /// </summary>
        /// <value>
        /// True if the double sided GI has been updated, false otherwise.
        /// </value>
        public bool HasDoubleSidedGIUpdated { get; protected set; }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            materialEditor.DoubleSidedGIField();
            HasDoubleSidedGIUpdated = EditorGUI.EndChangeCheck();
        }
    }
}