using UnityEditor;

using VRLabs.SimpleShaderInspectors;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a texture tiling and offset field.
    /// </summary>
    public class TilingAndOffsetControl : PropertyControl
    {
        /// <summary>
        /// Default constructor of <see cref="TilingAndOffsetControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        public TilingAndOffsetControl(string propertyName) : base(propertyName)
        {
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            // EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            materialEditor.TextureScaleOffsetProperty(Property);
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
        }
    }
}