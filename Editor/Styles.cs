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
                _colorIconBorder = Resources.Load<Texture2D>("Textures/SSIColorIconBorder");
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
                _colorIconBorderSelected = Resources.Load<Texture2D>("Textures/SSIColorIconBorderSelected");
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
                _colorIcon = Resources.Load<Texture2D>("Textures/SSIColorIcon");
                return _colorIcon;
            }
        }
    }

    /// <summary>
    /// Default compute shader assets and settings natively available in Simple Shader Inspectors.
    /// </summary>
    public static class ComputeShaders
    {
        private static ComputeShader _RGBAPacker;
        /// <summary>
        /// Compute shader that packs 4 texture channels into a single texture.
        /// </summary>
        public static ComputeShader RGBAPacker
        {
            get
            {
                if (_RGBAPacker != null) return _RGBAPacker;
                _RGBAPacker = Resources.Load<ComputeShader>("ComputeShaders/SSIRGBAPacker");
                return _RGBAPacker;
            }
        }
        /// <summary>
        /// default input settings for the RGBAPAcker compute shader.
        /// </summary>
        public static string RGBAPackerSettings
        {
            get
            {
                return Resources.Load<TextAsset>("ComputeShaderSettings/SSIRGBAPackerDefault").text;
            }
        }
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
        public static GUIStyle TextureBoxLightBorder
        {
            get
            {
                if (_textureBoxLightBorder != null) return _textureBoxLightBorder;
                _textureBoxLightBorder = CreateStyleFromSprite(new RectOffset(4, 4, 11, 4), "Textures/SSITextureBoxLight");
                return _textureBoxLightBorder;
            }
        }

        private static GUIStyle _textureBoxHeavyBorder;
        /// <summary>
        /// Style for a box pointing to a texture upwards. The border is less transparent than background.
        /// </summary>
        public static GUIStyle TextureBoxHeavyBorder
        {
            get
            {
                if (_textureBoxHeavyBorder != null) return _textureBoxHeavyBorder;
                _textureBoxHeavyBorder = CreateStyleFromSprite(new RectOffset(4, 4, 11, 4), "Textures/SSITextureBoxHeavy");
                return _textureBoxHeavyBorder;
            }
        }

        private static GUIStyle _boxLightBorder;
        /// <summary>
        /// Style for a box. The border is more transparent than background.
        /// </summary>
        public static GUIStyle BoxLightBorder
        {
            get
            {
                if (_boxLightBorder != null) return _boxLightBorder;
                _boxLightBorder = CreateStyleFromSprite(new RectOffset(4, 4, 4, 4), "Textures/SSIBoxLight");
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
        public static GUIStyle BoxHeavyBorder
        {
            get
            {
                if (_boxHeavyBorder != null) return _boxHeavyBorder;
                _boxHeavyBorder = CreateStyleFromSprite(new RectOffset(4, 4, 4, 4), "Textures/SSIBoxHeavy");
                _boxHeavyBorder.alignment = TextAnchor.MiddleCenter;
                _boxHeavyBorder.normal.textColor = EditorStyles.label.normal.textColor;
                _boxHeavyBorder.active.textColor = EditorStyles.label.active.textColor;
                _boxHeavyBorder.hover.textColor = EditorStyles.label.hover.textColor;
                _boxHeavyBorder.focused.textColor = EditorStyles.label.focused.textColor;
                return _boxHeavyBorder;
            }
        }

        private static Texture2D _SSILogoLight;
        /// <summary>
        /// Simple Shader Inspectors logo for light theme.
        /// </summary>
        public static Texture2D SSILogoLight
        {
            get
            {
                if (_SSILogoLight != null) return _SSILogoLight;

                _SSILogoLight = Resources.Load<Texture2D>("Textures/Logo/SSILogoLight");
                return _SSILogoLight;

            }
        }

        private static Texture2D _SSILogoDark;
        /// <summary>
        /// Simple Shader Inspectors logo for dark theme.
        /// </summary>
        public static Texture2D SSILogoDark
        {
            get
            {
                if (_SSILogoDark != null) return _SSILogoDark;

                _SSILogoDark = Resources.Load<Texture2D>("Textures/Logo/SSILogoDark");
                return _SSILogoDark;
            }
        }

        private static GUIStyle _deleteIcon;
        /// <summary>
        /// Style for a delete button.
        /// </summary>
        public static GUIStyle DeleteIcon
        {
            get
            {
                if (_deleteIcon != null) return _deleteIcon;
                _deleteIcon = CreateStyleFromSprite(normal: "Textures/SSIDelecteIcon",
                                                    active: "Textures/SSIDeleteIconPressed",
                                                     hover: "Textures/SSIDeleteIconHover"); //new GUIStyle("WinBtnClose");
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
                _upIcon = CreateStyleFromSprite(normal: "Textures/SSIUpIcon",
                                                active: "Textures/SSIUpIconPressed",
                                                 hover: "Textures/SSIUpIconHover"); //new GUIStyle("ProfilerTimelineRollUpArrow");
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
                _downIcon = CreateStyleFromSprite(normal: "Textures/SSIDownIcon",
                                                  active: "Textures/SSIDownIconPressed",
                                                   hover: "Textures/SSIDownIconHover"); //new GUIStyle("ProfilerTimelineDigDownArrow");
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
                _gearIcon = CreateStyleFromSprite(normal: "Textures/SSIGearIcon",
                                                  active: "Textures/SSIGearIconPressed",
                                                   hover: "Textures/SSIGearIconHover");
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
            GUIStyle style = new GUIStyle();
            Sprite sprite = Resources.Load<Sprite>(normal);
            style.padding = padding;
            style.border.left = (int)sprite.border.x;
            style.border.bottom = (int)sprite.border.y;
            style.border.right = (int)sprite.border.z;
            style.border.top = (int)sprite.border.w;

            style.normal.background = Resources.Load<Texture2D>(normal);
            if (!string.IsNullOrEmpty(active)) style.active.background = Resources.Load<Texture2D>(active);
            if (!string.IsNullOrEmpty(focused)) style.focused.background = Resources.Load<Texture2D>(focused);
            if (!string.IsNullOrEmpty(hover)) style.hover.background = Resources.Load<Texture2D>(hover);

            return style;
        }
    }
}