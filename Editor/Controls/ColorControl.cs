using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a color property.
    /// </summary>
    public class ColorControl : PropertyControl
    {
        /// <summary>
        /// Boolan that determines if the color picker and the color field should show the alpha value.
        /// </summary>
        public bool ShowAlphaValue { get; set; }

        /// <summary>
        /// Default constructor of <see cref="ColorControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="showAlphaValue">Show alpha value in the color picker, Optional (default: true).</param>
        /// <returns>A new <see cref="ColorControl"/> object.</returns>
        public ColorControl(string propertyName, bool showAlphaValue = true) : base(propertyName)
        {
            ShowAlphaValue = showAlphaValue;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            Color boxColor = Property.colorValue;
            EditorGUI.BeginChangeCheck();
            bool hdr = Property.flags == MaterialProperty.PropFlags.HDR;

            Rect colorPropertyRect = EditorGUILayout.GetControlRect();
            colorPropertyRect.width = EditorGUIUtility.labelWidth + EditorGUIUtility.fieldWidth;
            EditorGUI.showMixedValue = Property.hasMixedValue;
            boxColor = EditorGUI.ColorField(colorPropertyRect, Content, boxColor, true, ShowAlphaValue, hdr);

            EditorGUI.showMixedValue = false;
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(Property.displayName);
                Property.colorValue = boxColor;
            }
        }
    }

    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="ColorControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="showAlphaValue">Show alpha value in the color picker, Optional (default: true).</param>
        /// <returns>The <see cref="ColorControl"/> object that has been added.</returns>
        public static ColorControl AddColorControl(this IControlContainer container, string propertyName, bool showAlphaValue = true)
        {
            ColorControl control = new ColorControl(propertyName, showAlphaValue);
            container.Controls.Add(control);
            return control;
        }

        /// <summary>
        /// Sets up the generator background color.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="showAlpha">Boolean indicating if you want to show the alpha channel in the color selector.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="ColorControl"/> Object that has been modified.</returns>
        public static T SetShowAlphaValue<T>(this T control, bool showAlpha) where T : ColorControl
        {
            control.ShowAlphaValue = showAlpha;
            return control;
        }
    }
}