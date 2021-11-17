using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Default textures available natively in Simple Shader Inspectors.
    /// </summary>
    public static class Textures
    {
        private static Texture2D _colorIconBorder;
        /// <summary>
        /// Texture used for a color selector border.
        /// </summary>
        public static Texture2D ColorIconBorder
        {
            get
            {
                if (_colorIconBorder != null) return _colorIconBorder;
                _colorIconBorder = Resources.Load<Texture2D>($"{SSIConstants.RESOURCES_FOLDER}/Textures/ColorIconBorder");
                return _colorIconBorder;
            }
        }

        private static Texture2D _colorIconBorderSelected;
        /// <summary>
        /// Texture used for a color selector border when selected.
        /// </summary>
        public static Texture2D ColorIconBorderSelected
        {
            get
            {
                if (_colorIconBorderSelected != null) return _colorIconBorderSelected;
                _colorIconBorderSelected = Resources.Load<Texture2D>($"{SSIConstants.RESOURCES_FOLDER}/Textures/ColorIconBorderSelected");
                return _colorIconBorderSelected;
            }
        }

        private static Texture2D _colorIcon;
        /// <summary>
        /// Texture used for a color selector internal part.
        /// </summary>
        public static Texture2D ColorIcon
        {
            get
            {
                if (_colorIcon != null) return _colorIcon;
                _colorIcon = Resources.Load<Texture2D>($"{SSIConstants.RESOURCES_FOLDER}/Textures/ColorIcon");
                return _colorIcon;
            }
        }
    }

    /// <summary>
    /// Default compute shader assets and settings natively available in Simple Shader Inspectors.
    /// </summary>
    public static class ComputeShaders
    {
        private static ComputeShader _rgbaPacker;
        /// <summary>
        /// Compute shader that packs 4 texture channels into a single texture.
        /// </summary>
        public static ComputeShader RGBAPacker
        {
            get
            {
                if (_rgbaPacker != null) return _rgbaPacker;
                _rgbaPacker = Resources.Load<ComputeShader>($"{SSIConstants.RESOURCES_FOLDER}/ComputeShaders/RGBAPacker");
                return _rgbaPacker;
            }
        }
        /// <summary>
        /// default input settings for the RGBAPacker compute shader.
        /// </summary>
        public static string RGBAPackerSettings => Resources.Load<TextAsset>($"{SSIConstants.RESOURCES_FOLDER}/ComputeShaderSettings/RGBAPackerDefault").text;
    }

    /// <summary>
    /// Default styles available natively in SimpleShaderInspectors.
    /// </summary>
    public static class Styles
    {
        private static GUIStyle _bubble;
        /// <summary>
        /// Style that uses the same background of a button.
        /// </summary>
        public static GUIStyle Bubble
        {
            get
            {
                if (_bubble != null) return _bubble;
                _bubble = new GUIStyle("button");
                return _bubble;
            }
        }

        private static GUIStyle _box;

        /// <summary>
        /// Box-like Style.
        /// </summary>
        public static GUIStyle Box
        {
            get
            {
                if (_box != null) return _box;
                _box = new GUIStyle("box");
                return _box;
            }
        }

        private static GUIStyle _textureBoxLightBorder;
        /// <summary>
        /// Style for a box pointing to a texture upwards. The border is more transparent than background.
        /// </summary>
        /// <remarks>
        /// The border, being more transparent than the background, will result darker than the background on dark themes, lighter than the background on light themes,
        /// and with a more vivid color compared to the background when a custom background color is used.
        /// </remarks>
        public static GUIStyle TextureBoxLightBorder
        {
            get
            {
                if (_textureBoxLightBorder != null) return _textureBoxLightBorder;
                _textureBoxLightBorder = CreateStyleFromSprite(new RectOffset(4, 4, 11, 4), $"{SSIConstants.RESOURCES_FOLDER}/Textures/TextureBoxLight");
                return _textureBoxLightBorder;
            }
        }

        private static GUIStyle _textureBoxHeavyBorder;
        /// <summary>
        /// Style for a box pointing to a texture upwards. The border is less transparent than background.
        /// </summary>
        /// <remarks>
        /// The border, being less transparent than the background, will result lighter than the background on dark themes, darker than the background on light themes,
        /// and with a less vivid color compared to the background when a custom background color is used.
        /// </remarks>
        public static GUIStyle TextureBoxHeavyBorder
        {
            get
            {
                if (_textureBoxHeavyBorder != null) return _textureBoxHeavyBorder;
                _textureBoxHeavyBorder = CreateStyleFromSprite(new RectOffset(4, 4, 11, 4), $"{SSIConstants.RESOURCES_FOLDER}/Textures/TextureBoxHeavy");
                return _textureBoxHeavyBorder;
            }
        }

        private static GUIStyle _boxLightBorder;
        /// <summary>
        /// Style for a box. The border is more transparent than background.
        /// </summary>
        /// <remarks>
        /// The border, being more transparent than the background, will result darker than the background on dark themes, lighter than the background on light themes,
        /// and with a more vivid color compared to the background when a custom background color is used.
        /// </remarks>
        public static GUIStyle BoxLightBorder
        {
            get
            {
                if (_boxLightBorder != null) return _boxLightBorder;
                _boxLightBorder = CreateStyleFromSprite(new RectOffset(4, 4, 4, 4), $"{SSIConstants.RESOURCES_FOLDER}/Textures/BoxLight");
                _boxLightBorder.alignment = TextAnchor.MiddleCenter;
                _boxLightBorder.normal.textColor = EditorStyles.label.normal.textColor;
                _boxLightBorder.active.textColor = EditorStyles.label.active.textColor;
                _boxLightBorder.hover.textColor = EditorStyles.label.hover.textColor;
                _boxLightBorder.focused.textColor = EditorStyles.label.focused.textColor;
                return _boxLightBorder;
            }
        }

        private static GUIStyle _boxHeavyBorder;
        /// <summary>
        /// Style for a box. The border is less transparent than background.
        /// </summary>
        /// <remarks>
        /// The border, being less transparent than the background, will result lighter than the background on dark themes, darker than the background on light themes,
        /// and with a less vivid color compared to the background when a custom background color is used.
        /// </remarks>
        public static GUIStyle BoxHeavyBorder
        {
            get
            {
                if (_boxHeavyBorder != null) return _boxHeavyBorder;
                _boxHeavyBorder = CreateStyleFromSprite(new RectOffset(4, 4, 4, 4), $"{SSIConstants.RESOURCES_FOLDER}/Textures/BoxHeavy");
                _boxHeavyBorder.alignment = TextAnchor.MiddleCenter;
                _boxHeavyBorder.normal.textColor = EditorStyles.label.normal.textColor;
                _boxHeavyBorder.active.textColor = EditorStyles.label.active.textColor;
                _boxHeavyBorder.hover.textColor = EditorStyles.label.hover.textColor;
                _boxHeavyBorder.focused.textColor = EditorStyles.label.focused.textColor;
                return _boxHeavyBorder;
            }
        }

        private static Texture2D _ssiLogoLight;
        /// <summary>
        /// Simple Shader Inspectors logo for light theme.
        /// </summary>
        public static Texture2D SSILogoLight
        {
            get
            {
                if (_ssiLogoLight != null) return _ssiLogoLight;

                _ssiLogoLight = Resources.Load<Texture2D>($"{SSIConstants.RESOURCES_FOLDER}/Textures/Logo/LogoLight");
                return _ssiLogoLight;

            }
        }

        private static Texture2D _ssiLogoDark;
        /// <summary>
        /// Simple Shader Inspectors logo for dark theme.
        /// </summary>
        public static Texture2D SSILogoDark
        {
            get
            {
                if (_ssiLogoDark != null) return _ssiLogoDark;

                _ssiLogoDark = Resources.Load<Texture2D>($"{SSIConstants.RESOURCES_FOLDER}/Textures/Logo/LogoDark");
                return _ssiLogoDark;
            }
        }

        private static GUIStyle _deleteIcon;
        /// <summary>
        /// Style for a delete button.
        /// </summary>
        // TODO: fix the texture name.
        public static GUIStyle DeleteIcon
        {
            get
            {
                if (_deleteIcon != null) return _deleteIcon;
                _deleteIcon = CreateStyleFromSprite(normal: $"{SSIConstants.RESOURCES_FOLDER}/Textures/DeleteIcon",
                                                    active: $"{SSIConstants.RESOURCES_FOLDER}/Textures/DeleteIconPressed",
                                                     hover: $"{SSIConstants.RESOURCES_FOLDER}/Textures/DeleteIconHover"); //new GUIStyle("WinBtnClose");
                return _deleteIcon;
            }
        }

        private static GUIStyle _upIcon;

        /// <summary>
        /// Style for a up button.
        /// </summary>
        public static GUIStyle UpIcon
        {
            get
            {
                if (_upIcon != null) return _upIcon;
                _upIcon = CreateStyleFromSprite(normal: $"{SSIConstants.RESOURCES_FOLDER}/Textures/UpIcon",
                                                active: $"{SSIConstants.RESOURCES_FOLDER}/Textures/UpIconPressed",
                                                 hover: $"{SSIConstants.RESOURCES_FOLDER}/Textures/UpIconHover"); //new GUIStyle("ProfilerTimelineRollUpArrow");
                return _upIcon;
            }
        }

        private static GUIStyle _downIcon;

        /// <summary>
        /// Style for a down button.
        /// </summary>
        public static GUIStyle DownIcon
        {
            get
            {
                if (_downIcon != null) return _downIcon;
                _downIcon = CreateStyleFromSprite(normal: $"{SSIConstants.RESOURCES_FOLDER}/Textures/DownIcon",
                                                  active: $"{SSIConstants.RESOURCES_FOLDER}/Textures/DownIconPressed",
                                                   hover: $"{SSIConstants.RESOURCES_FOLDER}/Textures/DownIconHover"); //new GUIStyle("ProfilerTimelineDigDownArrow");
                return _downIcon;
            }
        }

        private static GUIStyle _gearIcon;

        /// <summary>
        /// Style for a gear icon.
        /// </summary>
        public static GUIStyle GearIcon
        {
            get
            {
                if (_gearIcon != null) return _gearIcon;
                _gearIcon = CreateStyleFromSprite(normal: $"{SSIConstants.RESOURCES_FOLDER}/Textures/GearIcon",
                                                  active: $"{SSIConstants.RESOURCES_FOLDER}/Textures/GearIconPressed",
                                                   hover: $"{SSIConstants.RESOURCES_FOLDER}/Textures/GearIconHover");
                return _gearIcon;
            }
        } 
 
        private static GUIStyle _boldCenter;
        /// <summary>
        /// Style of a bold label with a center anchor.
        /// </summary>
        public static GUIStyle BoldCenter
        {
            get
            {
                if (_boldCenter != null) return _boldCenter;
                _boldCenter = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleCenter };
                return _boldCenter;
            }
        }

        private static GUIStyle _boldLeft;
        /// <summary>
        /// Style of a bold label with a left anchor.
        /// </summary>
        public static GUIStyle BoldLeft
        {
            get
            {
                if (_boldLeft != null) return _boldLeft;
                _boldLeft = new GUIStyle(EditorStyles.boldLabel) { alignment = TextAnchor.MiddleLeft };
                return _boldLeft;
            }
        }

        private static GUIStyle _centerLabel;
        /// <summary>
        /// Style of a label with a center anchor.
        /// </summary>
        public static GUIStyle CenterLabel
        {
            get
            {
                if (_centerLabel != null) return _centerLabel;
                _centerLabel = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleCenter };
                return _centerLabel;
            }
        }
        
        private static GUIStyle _bottomCenterLabel;
        /// <summary>
        /// Style of a label with a center anchor.
        /// </summary>
        public static GUIStyle BottomCenterLabel
        {
            get
            {
                if (_bottomCenterLabel != null) return _bottomCenterLabel;
                _bottomCenterLabel = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.LowerCenter };
                return _bottomCenterLabel;
            }
        }

        private static GUIStyle _rightLabel;
        /// <summary>
        /// Style of a label with a right anchor.
        /// </summary>
        public static GUIStyle RightLabel
        {
            get
            {
                if (_rightLabel != null) return _rightLabel;
                _rightLabel = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleRight };
                return _rightLabel;
            }
        }

        private static GUIStyle _multilineLabel;
        /// <summary>
        /// Style of a label that can take multiple lines.
        /// </summary>
        public static GUIStyle MultilineLabel
        {
            get
            {
                if (_multilineLabel != null) return _multilineLabel;
                _multilineLabel = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true
                };
                return _multilineLabel;
            }
        }

        /// <summary>
        /// Creates a GUIStyle using the given sprite names.
        /// </summary>
        /// <param name="normal">Sprite to use on normal state.</param>
        /// <param name="active">Sprite to use on active state.</param>
        /// <param name="focused">Sprite to use on focused state.</param>
        /// <param name="hover">Sprite to use on hover state.</param>
        /// <returns>The generated GUIStyle</returns>
        public static GUIStyle CreateStyleFromSprite(string normal, string active = null, string focused = null, string hover = null)
        {
            return CreateStyleFromSprite(new RectOffset(4, 4, 4, 4), normal, active, focused, hover);
        }
        /// <summary>
        /// Creates a GUIStyle using the given sprite names and padding.
        /// </summary>
        /// <param name="padding">Padding of the GUIStyle.</param>
        /// <param name="normal">Sprite to use on normal state.</param>
        /// <param name="active">Sprite to use on active state.</param>
        /// <param name="focused">Sprite to use on focused state.</param>
        /// <param name="hover">Sprite to use on hover state.</param>
        /// <returns>The generated GUIStyle</returns>
        public static GUIStyle CreateStyleFromSprite(RectOffset padding, string normal, string active = null, string focused = null, string hover = null)
        {
            var style = new GUIStyle();
            Sprite sprite = Resources.Load<Sprite>(normal);
            style.padding = padding;
            if (sprite != null)
            {
                style.border.left = (int)sprite.border.x;
                style.border.bottom = (int)sprite.border.y;
                style.border.right = (int)sprite.border.z;
                style.border.top = (int)sprite.border.w;
            }

            if (!string.IsNullOrEmpty(normal)) style.normal.background = Resources.Load<Texture2D>(normal);
            if (!string.IsNullOrEmpty(active)) style.active.background = Resources.Load<Texture2D>(active);
            if (!string.IsNullOrEmpty(focused)) style.focused.background = Resources.Load<Texture2D>(focused);
            if (!string.IsNullOrEmpty(hover)) style.hover.background = Resources.Load<Texture2D>(hover);

            return style;
        }
    }
}