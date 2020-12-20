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
        public GUIStyle LabelStyle { get; set; }

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
    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="PropertyControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="alias">Alias of the control.</param>
        /// <returns>The <see cref="LabelControl"/> object that has been added.</returns>
        public static LabelControl AddLabelControl(this IControlContainer container, string alias)
        {
            LabelControl control = new LabelControl(alias);
            container.Controls.Add(control);
            return control;
        }

        /// <summary>
        /// Sets up the color of the LabelStyle.
        /// </summary>
        /// <param name="labelStyle">Control this method extends to.</param>
        /// <typeparam name="T">Type of the control object this method extends to.</typeparam>
        /// <returns>The <see cref="LabelControl"/> Object that has been modified.</returns>
        public static T SetLabelStyle<T>(this T label, GUIStyle labelStyle) where T : LabelControl
        {
            label.LabelStyle = labelStyle;
            return label;
        }
    }
}