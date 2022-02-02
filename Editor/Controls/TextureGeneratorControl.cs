using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control for a texture property with possibility to inline 2 extra properties. Also includes a texture generator.
    /// </summary>
    /// <remarks>
    /// <para>
    /// It is a more complex and specialized version of <see cref="TextureControl"/>, where on top of the base functionality of <see cref="TextureControl"/> it also has
    /// a full blown texture generator.
    /// </para>
    /// <para>
    /// When using it right away the texture generator will default to a simple 4 channel merger where for each channel you can select a texture, select which channel of
    /// the texture use, and use that as the channel for the final generated texture.
    /// </para>
    /// <para>
    /// While this is already a great use of the control and a fairly common one (like merging 4 monochrome texture masks) it is just one possible use,
    /// since it can load custom <c>compute shaders</c> and relative control input options to go along, enabling you to create your own generator that takes
    /// your own defined parameters to get a specific output.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds a new texture generator control with a texture and color field, using the default merger for the generator
    /// this.AddTextureGeneratorControl("_TextureProperty", "_ColorProperty"); 
    ///
    /// // Adds a new texture generator control with a texture and color field, using a custom generator
    /// this.AddTextureGeneratorControl(myCompute, computeInputJson, "_TextureProperty", "_ColorProperty"); 
    /// </code>
    /// </example>
    public class TextureGeneratorControl : TextureControl, IAdditionalLocalization
    {
        private readonly AdditionalLocalization[] _baseContent;
        private readonly AdditionalLocalization[] _namesContent;
        private bool _isGeneratorOpen;
        private readonly Shader _shader;
        private readonly string _kernelName;
        private Resolution _resolution;
        private readonly List<ComputeInputBase> _inputs;
        private RenderTexture _result;
        private CustomRenderTexture _crt;
        private Material _crtMaterial;
        private MaterialEditor _crtMaterialEditor;
        private DefaultGeneratorGUI _crtEditorGUI;
        private Texture2D _resultTex;
        private PropertyInfo[] _propertyInfos;

        private static readonly string[] _baseNames =
                        {
            "GeneratorOpen",        //[0]
            "GeneratorSaveButton",  //[1]
            "GeneratorCancelButton",//[2]
            "GeneratorTextureSize"  //[3]
        };

        /// <summary>
        /// Style for the texture generator button.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the generator "open" button.
        /// </value>
        [FluentSet] public GUIStyle GeneratorButtonStyle { get; set; }
        
        /// <summary>
        /// Style for the texture generator save button.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the generator "save" button.
        /// </value>
        [FluentSet] public GUIStyle GeneratorSaveButtonStyle { get; set; }

        /// <summary>
        /// Style for the texture generator background.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the generator background.
        /// </value>
        [FluentSet] public GUIStyle GeneratorStyle { get; set; }

        /// <summary>
        /// Style for the generator input background.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the background of a generator input field.
        /// </value>
        [FluentSet] public GUIStyle GeneratorInputStyle { get; set; }

        /// <summary>
        /// Background color for the texture generator button.
        /// </summary>
        /// <value>
        /// Color used when displaying the generator "open" button.
        /// </value>
        [FluentSet] public Color GeneratorButtonColor { get; set; }

        /// <summary>
        /// Background color for the generator save button.
        /// </summary>
        /// <value>
        /// Color used when displaying the generator "save" button.
        /// </value>
        [FluentSet] public Color GeneratorSaveButtonColor { get; set; }

        /// <summary>
        /// Background color for the generator background.
        /// </summary>
        /// <value>
        /// Color used when displaying the generator background.
        /// </value>
        [FluentSet] public Color GeneratorColor { get; set; }

        /// <summary>
        /// Background color for the generator input background.
        /// </summary>
        /// <value>
        /// Color used when displaying the background of a generator input field.
        /// </value>
        [FluentSet] public Color GeneratorInputColor { get; set; }

        /// <summary>
        /// Additional localization strings. Implementation for <see cref="IAdditionalLocalization"/>.
        /// </summary>
        /// <value>
        /// A list of <see cref="AdditionalLocalization"/> used by the control.
        /// </value>
        /// <remarks>
        /// <para>For this specific control this variable should only be used for reading values and not adding more due to the fact that unlike a usual case scenario,
        /// here the additional content is stored in multiple variables and put in a single list only when requested by this property.</para>
        /// </remarks>
        public AdditionalLocalization[] AdditionalContent
        {
            get
            {
                List<AdditionalLocalization> content = new List<AdditionalLocalization>();
                content.AddRange(_baseContent);
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
        public TextureGeneratorControl(string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : this(Shaders.RGBAPacker, Shaders.RGBAPackerSettings, propertyName, extraPropertyName1, extraPropertyName2)
        {
        }

        /// <summary>
        /// Default constructor of <see cref="TextureGeneratorControl"/>
        /// </summary>
        /// <param name="compute">Compute shader used</param>
        /// <param name="computeOptionsJson">Settings Json used for the compute shader</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>A new <see cref="TextureGeneratorControl"/> object.</returns>
        public TextureGeneratorControl(Shader shader, string computeOptionsJson, string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : base(propertyName, extraPropertyName1, extraPropertyName2)
        {
            HasCustomInlineContent = true;
            _resolution = Resolution.M_512x512;
            _result = new RenderTexture((int)_resolution, (int)_resolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
            {
                enableRandomWrite = true
            };
            _result.Create();

            _shader = shader;
            /*(_kernelName, _inputs) = TextureGeneratorHelper.GetInputs(computeOptionsJson);

            foreach (var input in _inputs)
            {
                switch (input)
                {
                    case ComputeTextureInput _:
                        _containsTextures = true;
                        break;
                    case ComputeColorInput _:
                        _containsColors = true;
                        break;
                }

                if (_containsColors && _containsTextures)
                {
                    break;
                }
            }*/

            GeneratorButtonStyle = Styles.Bubble;
            GeneratorStyle = Styles.TextureBoxHeavyBorder;
            GeneratorInputStyle = Styles.Box;
            GeneratorSaveButtonStyle = Styles.Bubble;

            GeneratorButtonColor = Color.white;
            GeneratorColor = Color.white;
            GeneratorInputColor = Color.white;
            GeneratorSaveButtonColor = Color.white;

            _baseContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_baseNames);
/*
            // Texture exclusive content
            if (_containsTextures)
                _textureContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_textureNames);

            // Color exclusive content
            if (_containsTextures)
                _colorContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_colorNames);
*/
            _crtMaterial = new Material(_shader);
            _crtMaterialEditor = Editor.CreateEditor(_crtMaterial) as MaterialEditor;
            if (_crtMaterialEditor.customShaderGUI != null && _crtMaterialEditor.customShaderGUI is TextureGeneratorShaderInspector compliantInspector)
            {
                compliantInspector.Setup(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
                _namesContent = compliantInspector.GetRequiredLocalization().ToArray();
            }

            for (int i = 0; i < _namesContent.Length; i++)
                _namesContent[i].Name = "Input_" + _namesContent[i].Name;
            
            Object.DestroyImmediate(_crtMaterial);
            Object.DestroyImmediate(_crtMaterialEditor);
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            DrawTextureSingleLine(materialEditor);

            if (_isGeneratorOpen)
            {
                GUI.backgroundColor = GeneratorColor;
                EditorGUILayout.BeginHorizontal();
                int previousIndent = EditorGUI.indentLevel;
                GUILayout.Space(EditorGUI.indentLevel * 15);
                EditorGUI.indentLevel = 0;
                EditorGUILayout.BeginVertical(GeneratorStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                DrawGenerator();
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = previousIndent;
                EditorGUILayout.EndHorizontal();
            }
        }
        
        protected override void DrawSideContent(MaterialEditor materialEditor)
        {
            if (!_isGeneratorOpen)
            {
                GUI.backgroundColor = GeneratorButtonColor;
                if (GUILayout.Button(_baseContent[0].Content, GeneratorButtonStyle))
                {
                    _isGeneratorOpen = true;
                }
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            }
        }

        private void DrawGenerator()
        {
            if (_crtMaterialEditor == null)
            {
                _crt = new CustomRenderTexture((int)_resolution, (int)_resolution, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
                _crtMaterial = new Material(_shader);
                _crt.material = _crtMaterial;
                _crtMaterialEditor = Editor.CreateEditor(_crtMaterial) as MaterialEditor;
                if (_crtMaterialEditor.customShaderGUI == null)
                {
                    _crtEditorGUI = new DefaultGeneratorGUI();
                }
                else
                {
                    if (_crtMaterialEditor.customShaderGUI is TextureGeneratorShaderInspector compliantInspector)
                    {
                        SetupInspector(compliantInspector);
                    }
                }
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(_crtMaterial, true);
            }
            
            // Draw generator shader inspector
            EditorGUILayout.BeginVertical();
            if (_crtEditorGUI != null)
                _crtEditorGUI.OnGUI(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
            else
                _crtMaterialEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
            
            //_crtMaterialEditor.PropertiesGUI();
            //crtMaterialEditor.serializedObject.FindProperty("isExpanded").boolValue = true;
            /*
            int columns = (int)((EditorGUIUtility.currentViewWidth - 20) / 90) - 1;
            if (columns == 0) columns = 1;
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

                AdditionalLocalization[] selectedArray = _inputs[i] is ComputeTextureInput ? _textureContent : _colorContent;
                
                GUI.backgroundColor = GeneratorInputColor;
                EditorGUILayout.BeginVertical(GeneratorInputStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                _inputs[i].InputGUI(_namesContent[i].Content, selectedArray);
                EditorGUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            _resolution = (Resolution)EditorGUILayout.EnumPopup(_baseContent[3].Content, _resolution); //_baseContent[3]

            */
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = GeneratorSaveButtonColor;
            
            if (GUILayout.Button(_baseContent[1].Content, GeneratorSaveButtonStyle))
            {
                GenerateTexture();
                _isGeneratorOpen = false;
                if (_crtMaterialEditor != null)
                {
                    Object.DestroyImmediate(_crt);
                    Object.DestroyImmediate(_crtMaterial);
                    Object.DestroyImmediate(_crtMaterialEditor);
                }
            }

            if (GUILayout.Button(_baseContent[2].Content, GeneratorSaveButtonStyle))
            {
                _isGeneratorOpen = false;
                if (_crtMaterialEditor != null)
                {
                    Object.DestroyImmediate(_crt);
                    Object.DestroyImmediate(_crtMaterial);
                    Object.DestroyImmediate(_crtMaterialEditor);
                }
            }
            
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            EditorGUILayout.EndHorizontal();
            
        }

        private void SetupInspector(TextureGeneratorShaderInspector compliantInspector)
        {
            compliantInspector.isFromGenerator = true;
            if (_propertyInfos == null)
            {
                _propertyInfos = new PropertyInfo[_namesContent.Length];
                for (int i = 0; i < _propertyInfos.Length; i++)
                {
                    _propertyInfos[i] = new PropertyInfo
                    {
                        Name = _namesContent[i].Name.Substring(6),
                        DisplayName = _namesContent[i].Content.text,
                        Tooltip = _namesContent[i].Content.tooltip
                    };
                }
            }
            compliantInspector.Setup(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
            compliantInspector.SetShaderLocalizationFromGenerator(_propertyInfos);
        }

        // Generate the result texture form the generator.
        private void GenerateTexture()
        {
            /*if (_result.width != (int)_resolution || _result.height != (int)_resolution)
            {
                _result = new RenderTexture((int)_resolution, (int)_resolution, 32, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB)
                {
                    enableRandomWrite = true
                };
                _result.Create();
                // 
            }

            int kernel = _shader.FindKernel(_kernelName);
            _shader.SetTexture(kernel, "Result", _result);
            _shader.SetFloat("width", (float)_resolution);
            _shader.SetFloat("height", (float)_resolution);

            var computeData = new ComputeInputs();
            foreach (var input in _inputs)
                input.AssignInputsToCompute(computeData, kernel);
            
            ComputeBuffer textureParamsBuffer = null;
            ComputeBuffer colorParamsBuffer = null;
            if (computeData.TexturesMeta.Count > 0)
            {
                textureParamsBuffer = new ComputeBuffer(computeData.TexturesMeta.Count, 20);
                textureParamsBuffer.SetData(computeData.TexturesMeta.ToArray());
                _shader.SetBuffer(kernel, "TexturesMeta", textureParamsBuffer);
            }
            if (computeData.Colors.Count > 0)
            {
                colorParamsBuffer = new ComputeBuffer(computeData.Colors.Count, 16);
                colorParamsBuffer.SetData(computeData.Colors.ToArray());
                _shader.SetBuffer(kernel, "Colors", colorParamsBuffer);
            }

            foreach (var texture in computeData.Textures)
                _shader.SetTexture(kernel, texture.Name, texture.Texture);
            
            _shader.Dispatch(kernel, (int)_resolution / 16, (int)_resolution / 16, 1);

            textureParamsBuffer?.Release();
            colorParamsBuffer?.Release();

            RenderTexture.active = _result;
            _resultTex = new Texture2D(_result.width, _result.height, TextureFormat.RGBA32, false);
            _resultTex.ReadPixels(new Rect(0, 0, _result.width, _result.height), 0, 0);
            RenderTexture.active = null;
            _resultTex.Apply(true);
            Property.textureValue = SSIHelper.SaveAndGetTexture(_resultTex, SSIHelper.GetTextureDestinationPath((Material)Property.targets[0], PropertyName + ".png"));*/
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