using System.Collections.Generic;
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
    public class TextureControl : PropertyControl, IAdditionalProperties, IControlContainer
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
        /// Indicates if the options button is pressed and the option area is visible.
        /// </summary>
        /// <value>
        /// True if the button is pressed, false otherwise.
        /// </value>
        protected bool IsOptionsButtonPressed;
        
        /// <summary>
        /// List of controls under this control's options.
        /// </summary>
        /// <value>
        /// All controls that have been added by extension methods.
        /// </value>
        public List<SimpleControl> Controls { get; set; }

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
        /// Boolean that defines if the control will show up the texture tiling and offset options in the options area.
        /// </summary>
        /// <value>
        /// True if the control has to show the uv tiling and offset, false otherwise.
        /// </value>
        [Chainable] public bool ShowTilingAndOffset { get; set; }

        /// <summary>
        /// Boolean that defines if the control needs to render the second material property as an hdr color field,
        /// only works if there is only one extra property and it's a color property.
        /// </summary>
        /// <value>
        /// True if the color property should have an HDR color field, false otherwise.
        /// </value>
        [Chainable] public bool HasHDRColor { get; set; }

        /// <summary>
        /// Style for the options button.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the button.
        /// </value>
        [Chainable] public GUIStyle OptionsButtonStyle { get; set; }
        
        /// <summary>
        /// Style for the options background area.
        /// </summary>
        /// <value>
        /// GUIStyle used for the background of the options area.
        /// </value>
        [Chainable] public GUIStyle OptionsAreaStyle { get; set; }

        /// <summary>
        /// Color for the options button.
        /// </summary>
        /// <value>
        /// Color used when displaying the options button.
        /// </value>
        [Chainable] public Color OptionsButtonColor { get; set; }
        
        /// <summary>
        /// Background color for the options area.
        /// </summary>
        /// <value>
        /// Color used when displaying the background for options area.
        /// </value>
        [Chainable] public Color OptionsAreaColor { get; set; }

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
            
            Controls = new List<SimpleControl>();

            OptionsButtonStyle = Styles.GearIcon;
            OptionsAreaStyle = Styles.TextureBoxHeavyBorder;
            OptionsButtonColor = Color.white;
            OptionsAreaColor = Color.white;

            ShowTilingAndOffset = false;
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
            if (ShowTilingAndOffset || Controls.Count > 0 || HasCustomInlineContent)
                EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical();
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
            EditorGUILayout.EndVertical();
            
            if (HasCustomInlineContent)
                DrawSideContent(materialEditor);
            
            if (ShowTilingAndOffset || Controls.Count > 0)
            {
                GUI.backgroundColor = OptionsButtonColor;
                IsOptionsButtonPressed = EditorGUILayout.Toggle(IsOptionsButtonPressed, OptionsButtonStyle, GUILayout.Width(19.0f), GUILayout.Height(19.0f));
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                
            }
            if (ShowTilingAndOffset || Controls.Count > 0 || HasCustomInlineContent)
                EditorGUILayout.EndHorizontal();
            
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            
            if (IsOptionsButtonPressed)
            {
                GUI.backgroundColor = OptionsAreaColor;
                EditorGUILayout.BeginHorizontal();
                int previousIndent = EditorGUI.indentLevel;
                GUILayout.Space(EditorGUI.indentLevel * 15);
                EditorGUI.indentLevel = 0;
                EditorGUILayout.BeginVertical(OptionsAreaStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                if (ShowTilingAndOffset)
                    materialEditor.TextureScaleOffsetProperty(Property);
                foreach (var control in Controls)
                    control.DrawControl(materialEditor);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = previousIndent;
                EditorGUILayout.EndHorizontal();
            }
        }
        
        protected virtual void DrawSideContent(MaterialEditor materialEditor)
        {
        }
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to add controls. All controls added are stored in <see cref="Controls"/>.
        ///
        /// These controls are going to be displayed inside the options area, after the tiling and offset option (it enabled).
        /// </summary>
        /// <param name="control">Control to add.</param>
        public void AddControl(SimpleControl control) => Controls.Add(control);


        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to get the object's controls list.
        /// </summary>
        /// <returns><see cref="Controls"/></returns>
        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}