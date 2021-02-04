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
        [Chainable] public bool ShowUvOptions { get; set; }

        /// <summary>
        /// Boolean that defines if the control needs to render the second material property as an hdr color field,
        ///  only works if there is only one extra property and it's a color property.
        /// </summary>
        [Chainable] public bool HasHDRColor { get; set; }

        /// <summary>
        /// GUIStyle for the uv options button.
        /// </summary>
        /// <value></value>
        [Chainable] public GUIStyle UVButtonStyle { get; set; }
        /// <summary>
        /// GUIStyle for the uv options button.
        /// </summary>
        [Chainable] public GUIStyle UVAreaStyle { get; set; }

        /// <summary>
        /// Background color for the uv button.
        /// </summary>
        [Chainable] public Color UVButtonColor { get; set; }
        /// <summary>
        /// Background color for the uv button.
        /// </summary>
        [Chainable] public Color UVAreaColor { get; set; }

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
}