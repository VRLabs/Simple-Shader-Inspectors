using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a texture property with possibility to inline 2 extra properties.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It can be considered the Simple Shader Inspector equivalent of <c>MaterialEditor.TexturePropertySingleLine</c> where you can pass a texture property followed
    /// by up to 2 additional properties
    /// </para>
    /// <para>
    /// Unlike <c>MaterialEditor.TexturePropertySingleLine</c> it does add the possibility to edit texture tiling and offset, in the form of an optional gear icon on the side
    /// (the icon can be changed)
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds a texture control with an additional color and float properties
    /// this.AddTextureControl("_TextureProperty", "_ColorProperty", "_FloatProperty"); 
    ///
    /// // Adds a texture control with an additional color property and editable tiling and offset
    /// this.AddTextureControl("_TextureProperty", "_ColorProperty").SetShowUvOptions(true);
    /// </code>
    /// </example>
    public class TextureControl : PropertyControl, IAdditionalProperties
    {
        /// <summary>
        /// Indicates if the control has the first extra property.
        /// </summary>
        /// <value>
        /// True if the control had the first extra property, false otherwise.
        /// </value>
        protected bool HasExtra1;

        /// <summary>
        /// Indicates if the control has the second extra property.
        /// </summary>
        /// <value>
        /// True if the control had the second extra property, false otherwise.
        /// </value>
        protected bool HasExtra2;
        
        /// <summary>
        /// Indicates if controls inheriting from this one have something to display inlined with the texture.
        /// </summary>
        /// <value>
        /// True if the control has something to inline to the texture, false otherwise.
        /// </value>
        protected bool HasCustomInlineContent = false;

        /// <summary>
        /// Indicates if the uv button is pressed and the tiling and offset area is visible.
        /// </summary>
        /// <value>
        /// True if the button is pressed, false otherwise.
        /// </value>
        protected bool IsUVButtonPressed;

        /// <summary>
        /// Extra properties array. Implementation of <see cref="IAdditionalProperties"/>.
        /// </summary>
        /// <value>List of <see cref="AdditionalProperty"/> used by this control.</value>
        /// <remarks>
        /// The list will store the following properties:
        /// <list type="bullet">
        /// <item> <term>[0]: </term> <description>first extra property.</description></item>
        /// <item> <term>[1]: </term> <description>second extra property.</description></item>
        /// </list>
        /// </remarks>
        public AdditionalProperty[] AdditionalProperties { get; set; }

        /// <summary>
        /// Boolean that defines if the control will show up an additional button to have access to the texture tiling and offset options.
        /// </summary>
        /// <value>
        /// True if the control has to show the button for uv tiling and offset, false otherwise.
        /// </value>
        [Chainable] public bool ShowUvOptions { get; set; }

        /// <summary>
        /// Boolean that defines if the control needs to render the second material property as an hdr color field,
        /// only works if there is only one extra property and it's a color property.
        /// </summary>
        /// <value>
        /// True if the color property should have an HDR color field, false otherwise.
        /// </value>
        [Chainable] public bool HasHDRColor { get; set; }

        /// <summary>
        /// Style for the tiling and offset options button.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the tiling and offset button.
        /// </value>
        [Chainable] public GUIStyle UVButtonStyle { get; set; }
        
        /// <summary>
        /// Style for the tiling and offset background area.
        /// </summary>
        /// <value>
        /// GUIStyle used for the background of the tiling and offset area.
        /// </value>
        [Chainable] public GUIStyle UVAreaStyle { get; set; }

        /// <summary>
        /// Color for the tiling and offset button.
        /// </summary>
        /// <value>
        /// Color used when displaying the tiling and offset button.
        /// </value>
        [Chainable] public Color UVButtonColor { get; set; }
        
        /// <summary>
        /// Background color for the uv button.
        /// </summary>
        /// <value>
        /// Color used when displaying the background for the tiling and offset area.
        /// </value>
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
            AdditionalProperties[0] = new AdditionalProperty(extraPropertyName1, false);
            if (!string.IsNullOrWhiteSpace(extraPropertyName1))
                HasExtra1 = true;
            
            AdditionalProperties[1] = new AdditionalProperty(extraPropertyName2, false);
            if (!string.IsNullOrWhiteSpace(extraPropertyName2))
                HasExtra2 = true;

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
            if (ShowUvOptions|| HasCustomInlineContent)
                EditorGUILayout.BeginHorizontal();
            
            if (HasExtra2)
            {
                materialEditor.TexturePropertySingleLine(Content, Property, AdditionalProperties[0].Property, AdditionalProperties[1].Property);
            }
            else if (HasExtra1)
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
            
            if (HasCustomInlineContent)
                DrawSideContent(materialEditor);
            
            if (ShowUvOptions)
            {
                GUI.backgroundColor = UVButtonColor;
                IsUVButtonPressed = EditorGUILayout.Toggle(IsUVButtonPressed, UVButtonStyle, GUILayout.Width(19.0f), GUILayout.Height(19.0f));
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                
            }
            if (ShowUvOptions || HasCustomInlineContent)
                EditorGUILayout.EndHorizontal();
            
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            
            if (IsUVButtonPressed)
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
        
        protected virtual void DrawSideContent(MaterialEditor materialEditor)
        {
        }
    }
}