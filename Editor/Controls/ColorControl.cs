using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a color property.
    /// </summary>
    /// <remarks>
    /// It is the control to use whenever there's a need to display a color material property.
    /// </remarks>
    /// <example>
    /// Initialize the control inside the inspector:
    /// <code>
    /// // initialize a color control that does not show the alpha value in the field.
    /// this.AddColorControl("_ExampleColorProperty", false);
    /// </code>
    /// </example>
    public class ColorControl : PropertyControl
    {
        /// <summary>
        /// Boolean that determines if the color picker and the color field should show the alpha value.
        /// </summary>
        /// <value>
        /// True if the control should show the alpha value, false otherwise (default: true)
        /// </value>
        [FluentSet] public bool ShowAlphaValue { get; set; }

        /// <summary>
        /// Selected color of the property stored in this control.
        /// </summary>
        /// <value>
        /// The currently selected color.
        /// </value>
        public Color SelectedColor => Property.colorValue;

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
}