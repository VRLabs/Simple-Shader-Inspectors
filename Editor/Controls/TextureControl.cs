using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a texture property with possibility to inline 2 extra properties.
    /// </summary>
    public class TextureControl : PropertyControl, IAdditionalProperties
    {
        // Cached boolean that says if the control received the first extra property.
        protected bool _hasExtra1 = false;

        // Cached boolean that says if the control received the second extra property.
        protected bool _hasExtra2 = false;

        protected bool _isUVButtonPressed = false;

        /// <summary>
        /// <para>Extra properties array.</para>
        /// <para>In this control the properties instanced in the array are as foolows:</para>
        /// <para>[0] = first extra property.</para>
        /// <para>[1] = second extra property.</para>
        /// </summary>
        public AdditionalProperty[] AdditionalProperties { get; set; }

        /// <summary>
        /// Boolean that defines if the control will show up an additional button to have access to the texture tiling and offset options.
        /// </summary>
        public bool ShowUvOptions { get; set; }

        /// <summary>
        /// Boolean that defines if the control needs to render the second material property as an hdr color field,
        ///  only works if there is only one extra property and it's a color property.
        /// </summary>
        public bool HasHDRColor { get; set; }

        /// <summary>
        /// GUIStyle for the uv options button.
        /// </summary>
        /// <value></value>
        public GUIStyle UVButtonStyle { get; set; }
        /// <summary>
        /// GUIStyle for the uv options button.
        /// </summary>
        public GUIStyle UVAreaStyle { get; set; }

        /// <summary>
        /// Background color for the uv button.
        /// </summary>
        public Color UVButtonColor { get; set; }
        /// <summary>
        /// Background color for the uv button.
        /// </summary>
        public Color UVAreaColor { get; set; }

        /// <summary>
        /// Default constructor of <see cref="TextureControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>A new <see cref="TextureControl"/> object.</returns>
        public TextureControl(string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : base(propertyName)
        {
            AdditionalProperties = new AdditionalProperty[2];
            AdditionalProperties[0] = new AdditionalProperty(extraPropertyName1);
            if (!string.IsNullOrWhiteSpace(extraPropertyName1))
            {
                _hasExtra1 = true;
            }
            AdditionalProperties[1] = new AdditionalProperty(extraPropertyName2);
            if (!string.IsNullOrWhiteSpace(extraPropertyName2))
            {
                _hasExtra2 = true;
            }

            UVButtonStyle = Styles.GearIcon;
            UVAreaStyle = Styles.TextureBoxHeavyBorder;
            UVButtonColor = Color.white;
            UVAreaColor = Color.white;

            ShowUvOptions = false;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            DrawTextureSingleLine(materialEditor);
        }

        protected void DrawTextureSingleLine(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            if (ShowUvOptions)
            {
                EditorGUILayout.BeginHorizontal();
            }
            if (_hasExtra2)
            {
                materialEditor.TexturePropertySingleLine(Content, Property, AdditionalProperties[0].Property, AdditionalProperties[1].Property);
            }
            else if (_hasExtra1)
            {
                if (AdditionalProperties[0].Property.type == MaterialProperty.PropType.Color && HasHDRColor)
                    materialEditor.TexturePropertyWithHDRColorFixed(Content, Property, AdditionalProperties[0].Property, true);
                else
                    materialEditor.TexturePropertySingleLine(Content, Property, AdditionalProperties[0].Property);
            }
            else
            {
                materialEditor.TexturePropertySingleLine(Content, Property);
            }
            if (ShowUvOptions)
            {
                GUI.backgroundColor = UVButtonColor;
                _isUVButtonPressed = EditorGUILayout.Toggle(_isUVButtonPressed, UVButtonStyle, GUILayout.Width(16.0f), GUILayout.Height(16.0f));
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                EditorGUILayout.EndHorizontal();
                if (_isUVButtonPressed)
                {
                    GUI.backgroundColor = UVAreaColor;
                    EditorGUILayout.BeginVertical(UVAreaStyle);
                    GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                    EditorGUI.indentLevel++;
                    materialEditor.TextureScaleOffsetProperty(Property);
                    EditorGUI.indentLevel--;
                    EditorGUILayout.EndVertical();
                }
            }
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
        }
    }

    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="TextureControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>The <see cref="TextureControl"/> object that has been added.</returns>
        public static TextureControl AddTextureControl(this IControlContainer container, string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null)
        {
            TextureControl control = new TextureControl(propertyName, extraPropertyName1, extraPropertyName2);
            container.Controls.Add(control);
            return control;
        }

        /// <summary>
        /// Set if the additional tiling and offset button is visible
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="showUvOptions">Boolean value</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureControl"/> Object that has been modified.</returns>
        public static T SetUvOptions<T>(this T control, bool showUvOptions) where T : TextureControl
        {
            control.ShowUvOptions = showUvOptions;
            return control;
        }

        /// <summary>
        /// Sets up the HasHDRColor bool.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="hasHDRColor">bool.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureControl"/> Object that has been modified.</returns>
        public static T SetHasHDRColor<T>(this T control, bool hasHDRColor) where T : TextureControl
        {
            control.HasHDRColor = hasHDRColor;
            return control;
        }

        /// <summary>
        /// Sets up the additional uv button style.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="style">Style to use.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureControl"/> Object that has been modified.</returns>
        public static T SetUVButtonStyle<T>(this T control, GUIStyle style) where T : TextureControl
        {
            control.UVButtonStyle = style;
            return control;
        }

        /// <summary>
        /// Sets up the additional uv tiling and offset area style.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="style">Style to use.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureControl"/> Object that has been modified.</returns>
        public static T SetUVAreaStyle<T>(this T control, GUIStyle style) where T : TextureControl
        {
            control.UVAreaStyle = style;
            return control;
        }

        /// <summary>
        /// Sets up the uv button color.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="color">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="TextureControl"/> Object that has been modified.</returns>
        public static T SetUVButtonColor<T>(this T control, Color color) where T : TextureGeneratorControl
        {
            control.UVButtonColor = color;
            return control;
        }

        /// <summary>
        /// Sets up  the additional uv tiling and offset area background color.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="color">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="TextureControl"/> Object that has been modified.</returns>
        public static T SetUVAreaColor<T>(this T control, Color color) where T : TextureGeneratorControl
        {
            control.UVButtonColor = color;
            return control;
        }
    }
}