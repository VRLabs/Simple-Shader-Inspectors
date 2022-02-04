using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

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
    /// since it can load custom <c>shaders</c>, enabling you to create your own generator, and uses the shader's gui for setting inputs for the generator.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds a new texture generator control with a texture and color field, using the default merger for the generator
    /// this.AddTextureGeneratorControl("_TextureProperty", "_ColorProperty"); 
    ///
    /// // Adds a new texture generator control with a texture and color field, using a custom generator
    /// this.AddTextureGeneratorControl(myShader, "_TextureProperty", "_ColorProperty"); 
    /// </code>
    /// </example>
    public class TextureGeneratorControl : TextureControl, IAdditionalLocalization
    {
        private readonly AdditionalLocalization[] _baseContent;
        private AdditionalLocalization[] _namesContent;
        private bool _isGeneratorOpen;
        private readonly Shader _shader;
        private Resolution _resolution;
        private bool _linearSpace;
        
        private CustomRenderTexture _crt;
        private Material _crtMaterial;
        private MaterialEditor _crtMaterialEditor;
        private DefaultGeneratorGUI _crtEditorGUI;
        private PropertyInfo[] _propertyInfos;
        
        private Texture2D[] _previousTextures;
        
        private float _width;
        private float _windowWidth;

        private List<Shader> generatorChildShaders;

        private static readonly string[] _baseNames =
        {
            "GeneratorOpen",        //[0]
            "GeneratorSaveButton",  //[1]
            "GeneratorCancelButton",//[2]
            "GeneratorTextureSize", //[3]
            "GeneratorPreview",     //[4]
            "GeneratorLinearSpace"  //[5]
        };

        private bool _isCircularReference;
        
        private ISimpleShaderInspector _inspector;

        public override ISimpleShaderInspector Inspector
        {
            get => _inspector;
            set
            {
                _inspector = value;
                _crtMaterial = new Material(_shader);
                _crtMaterialEditor = Editor.CreateEditor(_crtMaterial) as MaterialEditor;
                generatorChildShaders = new List<Shader>();
                if (Inspector is TextureGeneratorShaderInspector generatorInspector)
                    generatorChildShaders.AddRange(generatorInspector.shaderStack);
                generatorChildShaders.Add(Inspector.Shader);
            
                if (_crtMaterialEditor.customShaderGUI != null && _crtMaterialEditor.customShaderGUI is TextureGeneratorShaderInspector compliantInspector)
                {
                    if (generatorChildShaders.Contains(((Material)_crtMaterialEditor.target).shader))
                    {
                        _namesContent = Array.Empty<AdditionalLocalization>();
                        _isCircularReference = true;
                        
                    }
                    else
                    {
                        //generatorChildShaders.Add(((Material)_crtMaterialEditor.target).shader);
                        compliantInspector.shaderStack.AddRange(generatorChildShaders);
                        compliantInspector.Setup(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
                        _namesContent = compliantInspector.GetRequiredLocalization().Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToArray();
                    }
                }
                else
                {
                    _namesContent = Array.Empty<AdditionalLocalization>();
                }

                for (int i = 0; i < _namesContent.Length; i++)
                    _namesContent[i].Name = "Input_" + _namesContent[i].Name;
            
                Object.DestroyImmediate(_crtMaterial);
                Object.DestroyImmediate(_crtMaterialEditor);
            }
        }

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
        public TextureGeneratorControl(string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : this(Shaders.RGBAPacker, propertyName, extraPropertyName1, extraPropertyName2)
        {
        }

        /// <summary>
        /// Default constructor of <see cref="TextureGeneratorControl"/>
        /// </summary>
        /// <param name="shader">Shader used</param>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="extraPropertyName1">First additional material property name. Optional.</param>
        /// <param name="extraPropertyName2">Second additional material property name. Optional.</param>
        /// <returns>A new <see cref="TextureGeneratorControl"/> object.</returns>
        public TextureGeneratorControl(Shader shader, string propertyName, string extraPropertyName1 = null, string extraPropertyName2 = null) : base(propertyName, extraPropertyName1, extraPropertyName2)
        {
            HasCustomInlineContent = true;
            _resolution = Resolution.M_512x512;
            _shader = shader;

            GeneratorButtonStyle = Styles.Bubble;
            GeneratorStyle = Styles.TextureBoxHeavyBorder;
            GeneratorInputStyle = Styles.Box;
            GeneratorSaveButtonStyle = Styles.Bubble;

            GeneratorButtonColor = Color.white;
            GeneratorColor = Color.white;
            GeneratorInputColor = Color.white;
            GeneratorSaveButtonColor = Color.white;

            _baseContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_baseNames);
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
                DrawGenerator(materialEditor);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = previousIndent;
                EditorGUILayout.EndHorizontal();
            }
        }
        
        protected override void DrawSideContent(MaterialEditor materialEditor)
        {
            if (_isGeneratorOpen) return;
            
            GUI.backgroundColor = GeneratorButtonColor;
            if (GUILayout.Button(_baseContent[0].Content, GeneratorButtonStyle))
            {
                _isGeneratorOpen = true;
                _previousTextures = new Texture2D[materialEditor.targets.Length];
                for (int i = 0; i < Inspector.Materials.Length; i++)
                    _previousTextures[i] = (Texture2D)Inspector.Materials[i].GetTexture(PropertyName);
                Selection.selectionChanged += GeneratorCloseCleanup;
            }
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
        }

        private void DrawGenerator(MaterialEditor materialEditor)
        {
            if (_crtMaterialEditor == null)
            {
                _crt = new CustomRenderTexture(
                    (int)_resolution, (int)_resolution, RenderTextureFormat.ARGB32, 
                    _linearSpace ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
                Property.textureValue = _crt;
                _crtMaterial = new Material(_shader);
                _crt.material = _crtMaterial;
                _crtMaterialEditor = Editor.CreateEditor(_crtMaterial) as MaterialEditor;
                switch (_crtMaterialEditor.customShaderGUI)
                {
                    case null:
                        _crtEditorGUI = new DefaultGeneratorGUI();
                        break;
                    case TextureGeneratorShaderInspector compliantInspector:
                        SetupInspector(compliantInspector);
                        break;
                }
                UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(_crtMaterial, true);
                
                
            }
            
            // Draw generator shader inspector
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            if (_crtEditorGUI != null)
                _crtEditorGUI.OnGUI(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
            else
                _crtMaterialEditor.OnInspectorGUI();
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;

            EditorGUILayout.Space(20);
            
            if(Event.current.type != EventType.Repaint)
            {
                float oldWindowWidth = _windowWidth;
                _windowWidth = EditorGUIUtility.currentViewWidth;
                float difference = _windowWidth - oldWindowWidth;
                _width += difference;
            }
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.BeginVertical();
            if (_width <= 400)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
            }
            GUILayout.Label(_baseContent[4].Content,GUILayout.MinWidth(60));
            Rect windowRect = GUILayoutUtility.GetRect(10, 126, 10, 132);
            float squareSize = Mathf.Min(windowRect.width - 6, windowRect.height - 12);
            var previewRect = new Rect(windowRect.x + 7, windowRect.y + 9, squareSize, squareSize);
            GUI.DrawTexture(previewRect, _crt, ScaleMode.StretchToFill, true);
            
            if (_width <= 400)
            {
                EditorGUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical();
                GUILayout.FlexibleSpace();
            }
            
            EditorGUI.BeginChangeCheck();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_baseContent[5].Content,GUILayout.MinWidth(20));
            GUILayout.FlexibleSpace();
            _linearSpace = EditorGUILayout.Toggle( _linearSpace, GUILayout.Width(16));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(_baseContent[3].Content, GUILayout.MinWidth(20));
            GUILayout.FlexibleSpace();
            _resolution = (Resolution)EditorGUILayout.EnumPopup(_resolution,GUILayout.Width(130));
            EditorGUILayout.EndHorizontal();
            
            if (EditorGUI.EndChangeCheck())
            {
                _crt.Release();
                Object.DestroyImmediate(_crt);
                _crt = new CustomRenderTexture(
                    (int)_resolution, (int)_resolution, RenderTextureFormat.ARGB32, 
                    _linearSpace ? RenderTextureReadWrite.Linear : RenderTextureReadWrite.sRGB);
                _crt.material = _crtMaterial;
                Property.textureValue = _crt;
            }
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = GeneratorSaveButtonColor;
            
            if (GUILayout.Button(_baseContent[1].Content, GeneratorSaveButtonStyle,GUILayout.MinWidth(60)))
            {
                GenerateTexture();
                _previousTextures = null;
                _isGeneratorOpen = false;
                GeneratorCloseCleanup();
                
            }

            if (GUILayout.Button(_baseContent[2].Content, GeneratorSaveButtonStyle,GUILayout.MinWidth(60)))
            {
                _isGeneratorOpen = false;
                GeneratorCloseCleanup();
                
            }
            
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
            
            if (Event.current.type == EventType.Repaint)
            {
                _windowWidth = EditorGUIUtility.currentViewWidth;
                _width = GUILayoutUtility.GetLastRect().width;
            }


        }

        private void GeneratorCloseCleanup()
        {
            if (_crt != null)
            {
                _crt.Release();
                Object.DestroyImmediate(_crt);
            }

            Object.DestroyImmediate(_crtMaterial);
            Object.DestroyImmediate(_crtMaterialEditor);
            
            if (_previousTextures != null)
            {
                for (int i = 0; i < Inspector.Materials.Length; i++)
                {
                    if (Inspector.Materials[i] == null) continue;
                    Inspector.Materials[i].SetTexture(PropertyName, _previousTextures[i]);
                }
                    

                _previousTextures = null;
            }
            
            Selection.selectionChanged += GeneratorCloseCleanup;
        }

        private void SetupInspector(TextureGeneratorShaderInspector compliantInspector)
        {
            compliantInspector.isFromGenerator = true;
            if (_propertyInfos == null && !_isCircularReference)
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
            else
            {
                if (Inspector is TextureGeneratorShaderInspector ins)
                    _propertyInfos = ins.stackedInfo;
            }
            compliantInspector.Setup(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
            compliantInspector.SetShaderLocalizationFromGenerator(_propertyInfos);
        }

        // Generate the result texture form the generator.
        private void GenerateTexture()
        {
            string path = SSIHelper.GetTextureDestinationPath((Material)Property.targets[0], PropertyName + ".png");
            Property.textureValue = SSIHelper.SaveAndGetTexture(_crt, path, TextureWrapMode.Repeat, _linearSpace);
        }
    }

    public enum Resolution
    {
        // ReSharper disable InconsistentNaming
        XXS_64x64 = 64,
        XS_128x128 = 128,
        S_256x256 = 256,
        M_512x512 = 512,
        L_1024x1024 = 1024,
        XL_2048x2048 = 2048,
        XXL_4096x4096 = 4096
        // ReSharper restore InconsistentNaming
    }
}