using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors.Utility;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a texture property with possibility to inline 2 extra properties. Also includes a texture generator.
    /// </summary>
    public class TextureGeneratorControl : TextureControl, IAdditionalLocalization
    {
        private readonly AdditionalLocalization[] _baseContent;
        private readonly AdditionalLocalization[] _textureContent;
        private readonly AdditionalLocalization[] _colorContent;
        private readonly AdditionalLocalization[] _namesContent;
        private bool _isGeneratorOpen = false;
        private readonly ComputeShader _compute;
        private readonly string _kernelName;
        private Resolution _resolution;
        private readonly List<ComputeInputBase> _inputs;
        private RenderTexture _result;
        private Texture2D _resultTex;
        private readonly bool _containsTextures = false;

        private static readonly string[] _baseNames =
                        {
            "GeneratorOpen",        //[0]
            "GeneratorSaveButton",  //[1]
            "GeneratorCancelButton",//[2]
            "GeneratorTextureSize"  //[3]
        };
        private static readonly string[] _textureNames =
                {
            "TextureInvert"         //[0]
        };

        private static readonly string[] _colorNames =
                        {
            "ColorSpace"            //[0]
        };

        private readonly bool _containsColors = false;

        /// <summary>
        /// GUIStyle for the texture generator button.
        /// </summary>
        public GUIStyle GeneratorButtonStyle { get; set; }
        /// <summary>
        /// GUIStyle for the texture generator save button.
        /// </summary>
        public GUIStyle GeneratorSaveButtonStyle { get; set; }

        /// <summary>
        /// GUIStyle for the texture generator background.
        /// </summary>
        public GUIStyle GeneratorStyle { get; set; }

        /// <summary>
        /// GUIStyle for the input background.
        /// </summary>
        public GUIStyle GeneratorInputStyle { get; set; }

        /// <summary>
        /// Background color for the gradient editor button.
        /// </summary>
        public Color GeneratorButtonColor { get; set; }

        /// <summary>
        /// Background color for the gradient editor button.
        /// </summary>
        public Color GeneratorSaveButtonColor { get; set; }

        /// <summary>
        /// Background color for the gradient editor button.
        /// </summary>
        public Color GeneratorColor { get; set; }

        /// <summary>
        /// Background color for the gradient editor button.
        /// </summary>
        public Color GeneratorInputColor { get; set; }

        /// <summary>
        /// Additional localization strings.
        /// </summary>
        public AdditionalLocalization[] AdditionalContent
        {
            get
            {
                List<AdditionalLocalization> content = new List<AdditionalLocalization>();
                content.AddRange(_baseContent);
                if (_containsTextures) content.AddRange(_textureContent);
                if (_containsColors) content.AddRange(_colorContent);
                content.AddRange(_namesContent);

                return content.ToArray();
            }
            set { }
        }
        /// <summary>
        /// Default constructor of <see cref="TextureGeneratorControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>A new <see cref="TextureGeneratorControl"/> object.</returns>
        public TextureGeneratorControl(string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : this(ComputeShaders.RGBAPacker, ComputeShaders.RGBAPackerSettings, propertyName, extraPropertyName1, extraPropertyName2)
        {
        }

        /// <summary>
        /// Default constructor of <see cref="TextureGeneratorControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>A new <see cref="TextureGeneratorControl"/> object.</returns>
        public TextureGeneratorControl(ComputeShader compute, string computeOptionsJson, string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : base(propertyName, extraPropertyName1, extraPropertyName2)
        {
            _resolution = Resolution.M_512x512;
            _result = new RenderTexture((int)_resolution, (int)_resolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
            {
                enableRandomWrite = true
            };
            _result.Create();

            _compute = compute;
            (_kernelName, _inputs) = TextureGeneratorHelper.GetInputs(computeOptionsJson);

            foreach (var input in _inputs)
            {
                if (input is ComputeTextureInput)
                {
                    _containsTextures = true;
                }
                else if (input is ComputeColorInput)
                {
                    _containsColors = true;
                }

                if (_containsColors && _containsTextures)
                {
                    break;
                }
            }

            GeneratorButtonStyle = Styles.Bubble;
            GeneratorStyle = Styles.TextureBoxHeavyBorder;
            GeneratorInputStyle = Styles.Box;
            GeneratorSaveButtonStyle = Styles.Bubble;

            GeneratorButtonColor = Color.white;
            GeneratorColor = Color.white;
            GeneratorInputColor = Color.white;
            GeneratorSaveButtonColor = Color.white;

            _baseContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_baseNames);

            // Texture exclusive content
            if (_containsTextures)
            {
                _textureContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_textureNames);
            }

            // Color exclusive content
            if (_containsTextures)
            {
                _colorContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_colorNames);
            }

            _namesContent = new AdditionalLocalization[_inputs.Count];
            for (int i = 0; i < _inputs.Count; i++)
            {
                _namesContent[i] = new AdditionalLocalization { Name = "Input" + (i + 1) };
            }
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            DrawTextureSingleLine(materialEditor);
            if (!_isGeneratorOpen)
            {
                GUI.backgroundColor = GeneratorButtonColor;
                if (GUILayout.Button(_baseContent[0].Content, GeneratorButtonStyle))
                {
                    _isGeneratorOpen = true;
                }
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            }

            if (_isGeneratorOpen)
            {
                GUI.backgroundColor = GeneratorColor;
                EditorGUILayout.BeginVertical(GeneratorStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                DrawGenerator();
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawGenerator()
        {
            int columns = (int)((EditorGUIUtility.currentViewWidth - 20) / 90) - 1;
            if (columns == 0) columns = 1;
            AdditionalLocalization[] selectedArray;
            for (int i = 0; i < _inputs.Count; i++)
            {
                if (i % columns == 0)
                {
                    if (i == 0)
                    {
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                    else
                    {
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                    }
                }
                if (_inputs[i] is ComputeTextureInput)
                {
                    selectedArray = _textureContent;
                }
                else
                {
                    selectedArray = _colorContent;
                }
                GUI.backgroundColor = GeneratorInputColor;
                EditorGUILayout.BeginVertical(GeneratorInputStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                _inputs[i].InputGUI(_namesContent[i].Content, selectedArray);
                EditorGUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            _resolution = (Resolution)EditorGUILayout.EnumPopup(_baseContent[3].Content, _resolution); //_baseContent[3]

            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = GeneratorSaveButtonColor;
            if (GUILayout.Button(_baseContent[1].Content, GeneratorSaveButtonStyle))
            {
                GenerateTexture();
                _isGeneratorOpen = false;
            }
            if (GUILayout.Button(_baseContent[2].Content, GeneratorSaveButtonStyle))
            {
                _isGeneratorOpen = false;
            }
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            EditorGUILayout.EndHorizontal();
        }

        // Generate the result texture form the generator.
        private void GenerateTexture()
        {
            if (_result.width != (int)_resolution || _result.height != (int)_resolution)
            {
                _result = new RenderTexture((int)_resolution, (int)_resolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true
                };
                _result.Create();
                // 
            }

            int kernel = _compute.FindKernel(_kernelName);
            _compute.SetTexture(kernel, "Result", _result);
            _compute.SetFloat("width", (float)_resolution);
            _compute.SetFloat("height", (float)_resolution);

            ComputeInputs computeData = new ComputeInputs();
            foreach (var input in _inputs)
            {
                input.AssignInputsToCompute(computeData, kernel);
            }
            ComputeBuffer textureParamsBuffer = null;
            ComputeBuffer colorParamsBuffer = null;
            if (computeData.TexturesMeta.Count > 0)
            {
                textureParamsBuffer = new ComputeBuffer(computeData.TexturesMeta.Count, 20);
                textureParamsBuffer.SetData(computeData.TexturesMeta.ToArray());
                _compute.SetBuffer(kernel, "TexturesMeta", textureParamsBuffer);
            }
            if (computeData.Colors.Count > 0)
            {
                colorParamsBuffer = new ComputeBuffer(computeData.Colors.Count, 16);
                colorParamsBuffer.SetData(computeData.Colors.ToArray());
                _compute.SetBuffer(kernel, "Colors", colorParamsBuffer);
            }

            foreach (var texture in computeData.Textures)
            {
                _compute.SetTexture(kernel, texture.Name, texture.Texture);
            }
            _compute.Dispatch(kernel, (int)_resolution / 16, (int)_resolution / 16, 1);

            textureParamsBuffer?.Release();
            colorParamsBuffer?.Release();

            RenderTexture.active = _result;
            _resultTex = new Texture2D(_result.width, _result.height, TextureFormat.RGBA32, false);
            _resultTex.ReadPixels(new Rect(0, 0, _result.width, _result.height), 0, 0);
            RenderTexture.active = null;
            _resultTex.Apply(true);
            Property.textureValue = SSIHelper.SaveAndGetTexture(_resultTex, SSIHelper.GetTextureDestinationPath((Material)Property.targets[0], PropertyName + ".png"));
        }
    }

    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="TextureGeneratorControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>The <see cref="TextureGeneratorControl"/> object that has been added.</returns>
        public static TextureGeneratorControl AddTextureGeneratorControl(this IControlContainer container, string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null)
        {
            TextureGeneratorControl control = new TextureGeneratorControl(propertyName, extraPropertyName1, extraPropertyName2);
            container.Controls.Add(control);
            return control;
        }

        /// <summary>
        /// Sets up the generator toggle button style.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="style">Style to use.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorButtonStyle<T>(this T control, GUIStyle style) where T : TextureGeneratorControl
        {
            control.GeneratorButtonStyle = style;
            return control;
        }

        /// <summary>
        /// Sets up the generator save button style.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="style">Style to use.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorSaveButtonStyle<T>(this T control, GUIStyle style) where T : TextureGeneratorControl
        {
            control.GeneratorSaveButtonStyle = style;
            return control;
        }

        /// <summary>
        /// Sets up the generator background style.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="style">Style to use.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorStyle<T>(this T control, GUIStyle style) where T : TextureGeneratorControl
        {
            control.GeneratorStyle = style;
            return control;
        }

        /// <summary>
        /// Sets up the generator's input background style.
        /// </summary>
        /// <param name="control">Control this method extends to.</param>
        /// <param name="style">Style to use.</param>
        /// <typeparam name="T">Type of the control it extends to</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorInputStyle<T>(this T control, GUIStyle style) where T : TextureGeneratorControl
        {
            control.GeneratorInputStyle = style;
            return control;
        }
        /// <summary>
        /// Sets up the generator toggle button color.
        /// </summary>
        /// <param name="control">Section this method extends to.</param>
        /// <param name="color">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorButtonColor<T>(this T control, Color color) where T : TextureGeneratorControl
        {
            control.GeneratorButtonColor = color;
            return control;
        }

        /// <summary>
        /// Sets up the generator texture save button color.
        /// </summary>
        /// <param name="control">Section this method extends to.</param>
        /// <param name="color">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorSaveButtonColor<T>(this T control, Color color) where T : TextureGeneratorControl
        {
            control.GeneratorSaveButtonColor = color;
            return control;
        }

        /// <summary>
        /// Sets up the style of the generator background color.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="color">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorColor<T>(this T control, Color color) where T : TextureGeneratorControl
        {
            control.GeneratorColor = color;
            return control;
        }

        /// <summary>
        /// Sets up the generator background color.
        /// </summary>
        /// <param name="control">Section this method extends to.</param>
        /// <param name="color">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The <see cref="TextureGeneratorControl"/> Object that has been modified.</returns>
        public static T SetGeneratorInputColor<T>(this T control, Color color) where T : TextureGeneratorControl
        {
            control.GeneratorInputColor = color;
            return control;
        }
    }

    public enum Resolution
    {
        XS_128x128 = 128,
        S_256x256 = 256,
        M_512x512 = 512,
        L_1024x1024 = 1024,
        XL_2048x2048 = 2048,
        XXL_4096x4096 = 4096
    }
}