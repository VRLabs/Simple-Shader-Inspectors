using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents an empty space.
    /// </summary>
    /// <remarks>
    /// <para>It can be considered the Simple Shader Inspectors equivalent of <c>GUILayout.Space</c>.</para>
    /// <para>It does not need an alias in order to work.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds a space of 1 line worth of pixels
    /// this.AddSpaceControl();
    /// // adds a space of 15 pixels
    /// this.AddSpaceControl(15); 
    /// </code>
    /// </example>
    public class SpaceControl : SimpleControl
    {
        /// <summary>
        /// Amount of space in pixels this control has.
        /// </summary>
        /// <value>
        /// Pixels of space.
        /// </value>
        public int Space { get; set; }

        /// <summary>
        /// Default constructor of <see cref="LabelControl"/>.
        /// </summary>
        /// <param name="space">amount of space to use, if set to 0 it defaults to 1 line worth of space.</param>
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
                EditorGUILayout.Space();
            else
                GUILayout.Space(Space);
        }
    }
}