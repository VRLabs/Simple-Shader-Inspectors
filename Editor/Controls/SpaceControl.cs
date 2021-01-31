using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a label without any property.
    /// </summary>
    public class SpaceControl : SimpleControl
    {
        /// <summary>
        /// Amount of space in pixels this control has.
        /// </summary>
        public int Space { get; set; }

        /// <summary>
        /// Default constructor of <see cref="LabelControl"/>.
        /// </summary>
        /// <param name="alias">Alias of the control.</param>
        /// <returns>A new <see cref="LabelControl"/> object.</returns>
        public SpaceControl(int space = 0) : base("")
        {
            Space = space;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            if (Space == 0)
            {
                EditorGUILayout.Space();
            }
            else
            {
                GUILayout.Space(Space);
            }
        }
    }
}