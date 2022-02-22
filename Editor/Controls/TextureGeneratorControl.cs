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
        // Static content for the generator
        internal readonly AdditionalLocalization[] baseContent;
        // Dynamic content based on che generator shader, empty if is in a circular reference
        internal AdditionalLocalization[] namesContent;
        
        private bool _isGeneratorOpen;
        private readonly Shader _shader;
        private Resolution _resolution;
        private bool _linearSpace;
        
        // CRT used for preview and save, null when generator is not open
        private CustomRenderTexture _crt;
        // Material used by the CRT, null when generator is not open
        private Material _crtMaterial;
        // Material editor of the CRT material, null when generator is not open
        private MaterialEditor _crtMaterialEditor;
        // Generator specific to show a generator shader, null when generator is not open OR when the shader used doesn't use this type of ShaderGUI
        private DefaultGeneratorGUI _crtEditorGUI;
        // Localization for generator inspectors
        private PropertyInfo[] _propertyInfos;
        // Textures in the materials before the generator opened, they are restored to the material if the generator is closed or the selection has changed
        private Texture2D[] _previousTextures;
        // Width of the generator area
        private float _width;
        // Width of the window, used to calculate differentials of the generator area during the layout phase
        private float _windowWidth;
        // List of all shaders visited in a possible generator tree, used to detect circular references of shaders with generators
        private List<Shader> _shaderStack;
        // Array of names used for base generator localization ids
        private static readonly string[] _baseNames =
        {
            "GeneratorOpen",        //[0]
            "GeneratorSaveButton",  //[1]
            "GeneratorCancelButton",//[2]
            "GeneratorTextureSize", //[3]
            "GeneratorPreview",     //[4]
            "GeneratorLinearSpace"  //[5]
        };
        
        private ISimpleShaderInspector _inspector;

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
        /// Style for the texture generator close button.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the generator "close" button.
        /// </value>
        [FluentSet] public GUIStyle GeneratorCloseButtonStyle { get; set; }

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
        /// Background color for the generator close button.
        /// </summary>
        /// <value>
        /// Color used when displaying the generator "close" button.
        /// </value>
        [FluentSet] public Color GeneratorCloseButtonColor { get; set; }

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
                content.AddRange(baseContent);
                content.AddRange(namesContent);

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
            GeneratorCloseButtonStyle = Styles.Bubble;

            GeneratorButtonColor = Color.white;
            GeneratorColor = Color.white;
            GeneratorInputColor = Color.white;
            GeneratorSaveButtonColor = Color.white;
            GeneratorCloseButtonColor = Color.white;

            baseContent = AdditionalContentExtensions.CreateLocalizationArrayFromNames(_baseNames);
        }

        public override void Initialization()
        {
            _crtMaterial = new Material(_shader);
            _crtMaterialEditor = Editor.CreateEditor(_crtMaterial) as MaterialEditor;
            _shaderStack = new List<Shader>();
            bool isRoot = true;
            if (Inspector is TextureGeneratorShaderInspector generatorInspector)
            {
                _shaderStack.AddRange(generatorInspector.shaderStack);
                isRoot = false;
            }
            _shaderStack.Add(Inspector.Shader);
            
            // ReSharper disable once PossibleNullReferenceException
            if (_crtMaterialEditor.customShaderGUI != null && _crtMaterialEditor.customShaderGUI is TextureGeneratorShaderInspector compliantInspector)
            {
                if (_shaderStack.Contains(((Material)_crtMaterialEditor.target).shader))
                {
                    namesContent = Array.Empty<AdditionalLocalization>();
                }
                else
                {
                    compliantInspector.shaderStack.AddRange(_shaderStack);
                    compliantInspector.Setup(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
                    namesContent = compliantInspector.GetRequiredLocalization().Where(x => !string.IsNullOrWhiteSpace(x.Name)).Distinct().ToArray();
                }
            }
            else
            {
                namesContent = Array.Empty<AdditionalLocalization>();
            }

            if(isRoot)
                foreach (AdditionalLocalization t in namesContent)
                    t.Name = "Input_" + t.Name;

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
            if (_isGeneratorOpen) return;
            
            GUI.backgroundColor = GeneratorButtonColor;
            if (GUILayout.Button(baseContent[0].Content, GeneratorButtonStyle))
            {
                _isGeneratorOpen = true;
                _previousTextures = new Texture2D[materialEditor.targets.Length];
                for (int i = 0; i < Inspector.Materials.Length; i++)
                    _previousTextures[i] = (Texture2D)Inspector.Materials[i].GetTexture(PropertyName);
                Selection.selectionChanged += GeneratorCloseCleanup;
            }
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
        }

        private void DrawGenerator()
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
                // ReSharper disable once PossibleNullReferenceException
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

            GUI.backgroundColor = GeneratorInputColor;
            EditorGUILayout.BeginHorizontal(GeneratorInputStyle);
            EditorGUILayout.BeginVertical();
            if (_width <= 400)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                EditorGUILayout.BeginVertical();
            }
            GUILayout.Label(baseContent[4].Content,GUILayout.MinWidth(60));
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
            GUILayout.Label(baseContent[5].Content,GUILayout.MinWidth(20));
            GUILayout.FlexibleSpace();
            _linearSpace = EditorGUILayout.Toggle( _linearSpace, GUILayout.Width(16));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(baseContent[3].Content, GUILayout.MinWidth(20));
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
            
            if (GUILayout.Button(baseContent[1].Content, GeneratorSaveButtonStyle,GUILayout.MinWidth(60)))
            {
                GenerateTexture();
                _previousTextures = null;
                _isGeneratorOpen = false;
                GeneratorCloseCleanup();
                
            }
            GUI.backgroundColor = GeneratorCloseButtonColor;
            if (GUILayout.Button(baseContent[2].Content, GeneratorCloseButtonStyle,GUILayout.MinWidth(60)))
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
                _crt = null;
            }

            Object.DestroyImmediate(_crtMaterial);
            Object.DestroyImmediate(_crtMaterialEditor);
            _crtMaterial = null;
            _crtMaterialEditor = null;
            _crtEditorGUI = null;
            
            if (_previousTextures != null)
            {
                for (int i = 0; i < Inspector.Materials.Length; i++)
                {
                    if (Inspector.Materials[i] == null) continue;
                    Inspector.Materials[i].SetTexture(PropertyName, _previousTextures[i]);
                }
                    

                _previousTextures = null;
            }
            
            Selection.selectionChanged -= GeneratorCloseCleanup;
        }

        private void SetupInspector(TextureGeneratorShaderInspector compliantInspector)
        {
            compliantInspector.isFromGenerator = true;
            compliantInspector.shaderStack.AddRange(_shaderStack);
            compliantInspector.Setup(_crtMaterialEditor, MaterialEditor.GetMaterialProperties(_crtMaterialEditor.targets));
            
            if (Inspector is TextureGeneratorShaderInspector ins)
                _propertyInfos = ins.stackedInfo;
            else
            {
                _propertyInfos = new PropertyInfo[namesContent.Length];
                for (int i = 0; i < _propertyInfos.Length; i++)
                {
                    _propertyInfos[i] = new PropertyInfo
                    {
                        Name = namesContent[i].Name.Substring(6),
                        DisplayName = namesContent[i].Content.text,
                        Tooltip = namesContent[i].Content.tooltip
                    };
                    
                }
            }
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