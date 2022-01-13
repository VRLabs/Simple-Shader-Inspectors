using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors.Utility;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Control that handles a gradient texture with a gradient editor included.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This advanced control is a <see cref="TextureControl"/> specialized for gradient textures. It embeds an editor for generating or modifying gradients into textures,
    /// enabling the user to create and previewing gradients in real time without leaving the inspector.
    /// </para>
    /// <para>It can also include a color property.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddGradientTextureControl("_exampleGradientTexture", "_ExampleColor");
    /// </code>
    /// </example>
    public class GradientTextureControl : TextureControl, IAdditionalLocalization
    {
        private GradientTexture _gradient;
        private GradientBlendMode _blendMode;

        private GradientWidth _rampWidth;
        private Texture2D[] _previousTextures;
        private bool _isGradientEditorOpen;
        private Rect _gradientPreviewRect;
        private Rect _keySelectionAreaRect;
        private Rect[] _keyRects;

        private int _selectedKeyIndex;

        private bool _mouseIsDownOverKey;
        private bool _hasMinValue;
        private bool _hasMaxValue;

        private static readonly string[] _contentNames = {
            "GradientOpenEditor",
            "GradientColorLabel",
            "GradientTimeLabel",
            "GradientRampSizeLabel",
            "GradientBlendModeLabel",
            "GradientSaveButton",
            "GradientCancelButton"
        };

        /// <summary>
        /// Style used to display the gradient editor button.
        /// </summary>
        /// <value>
        /// GUIStyle used.
        /// </value>
        [FluentSet] public GUIStyle GradientButtonStyle { get; set; }
        
        /// <summary>
        /// Style used to display the gradient editor save button.
        /// </summary>
        /// <value>
        /// GUIStyle used.
        /// </value>
        [FluentSet] public GUIStyle GradientSaveButtonStyle { get; set; }
        
        /// <summary>
        /// Style used to display the gradient editor background.
        /// </summary>
        /// <value>
        /// GUIStyle used.
        /// </value>
        [FluentSet] public GUIStyle GradientEditorStyle { get; set; }

        /// <summary>
        /// Background color used to display the gradient editor button.
        /// </summary>
        /// <value>
        /// Color used.
        /// </value>
        [FluentSet] public Color GradientButtonColor { get; set; }
        
        /// <summary>
        /// Background color used to display the gradient editor save button.
        /// </summary>
        /// <value>
        /// Color used.
        /// </value>
        [FluentSet] public Color GradientSaveButtonColor { get; set; }
        
        /// <summary>
        /// Background color used to display the gradient editor background.
        /// </summary>
        /// <value>
        /// Color used.
        /// </value>
        [FluentSet] public Color GradientEditorColor { get; set; }

        /// <summary>
        /// Implementation of <see cref="IAdditionalLocalization"/> for the additional localization strings.
        /// </summary>
        /// <value>
        /// Array of <see cref="AdditionalLocalization"/> objects. 
        /// </value>
        /// <remarks>
        /// The localized content array will have the following object names:
        /// <list type="bullet">
        /// <item> <term>[0]: </term> <description>GradientOpenEditor</description> </item>
        /// <item> <term>[1]: </term> <description>GradientColorLabel</description> </item>
        /// <item> <term>[2]: </term> <description>GradientTimeLabel</description> </item>
        /// <item> <term>[3]: </term> <description>GradientRampSizeLabel</description> </item>
        /// <item> <term>[4]: </term> <description>GradientBlendModeLabel</description> </item>
        /// <item> <term>[5]: </term> <description>GradientSaveButton</description> </item>
        /// <item> <term>[6]: </term> <description>GradientCancelButton</description> </item>
        /// </list>
        /// </remarks>
        public AdditionalLocalization[] AdditionalContent { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="propertyName">Name of the gradient texture property.</param>
        /// <param name="colorPropertyName">Name of the relative color property (default: null).</param>
        public GradientTextureControl(string propertyName, string colorPropertyName = null) : this(propertyName, null, null, colorPropertyName)
        {
        }
        
        /// <summary>
        /// Constructor with additional min and max texture color properties.
        /// </summary>
        /// <param name="propertyName">Name of the gradient texture property.</param>
        /// <param name="minColorPropertyName">Minimum color of the gradient.</param>
        /// <param name="maxColorPropertyName">Maximum color of the gradient.</param>
        /// <param name="colorPropertyName">Name of the relative color property (default: null).</param>
        public GradientTextureControl(string propertyName, string minColorPropertyName, string maxColorPropertyName, string colorPropertyName = null) : base(propertyName)
        {
            AdditionalProperties = new AdditionalProperty[3];
            AdditionalProperties[0] = new AdditionalProperty(colorPropertyName);
            AdditionalProperties[1] = new AdditionalProperty(minColorPropertyName);
            AdditionalProperties[2] = new AdditionalProperty(maxColorPropertyName);
            if (!string.IsNullOrWhiteSpace(colorPropertyName))
                HasExtra1 = true;
            if (!string.IsNullOrWhiteSpace(minColorPropertyName))
                _hasMinValue = true;
            if (!string.IsNullOrWhiteSpace(maxColorPropertyName))
                _hasMaxValue = true;
            
            Controls = new List<SimpleControl>();
            
            GradientButtonStyle = Styles.Bubble;
            GradientEditorStyle = Styles.TextureBoxHeavyBorder;
            GradientSaveButtonStyle = Styles.Bubble;
            ShowTilingAndOffset = false;

            GradientButtonColor = Color.white;
            GradientEditorColor = Color.white;
            GradientSaveButtonColor = Color.white;

            //Gradient editor default settings.
            _gradient = new GradientTexture(1024);
            _rampWidth = GradientWidth.L_1024;
            _blendMode = GradientBlendMode.Linear;
            _previousTextures = null;

            this.InitializeLocalizationWithNames(_contentNames);
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.BeginHorizontal();
            
            if (HasExtra1)
                materialEditor.TexturePropertySingleLine(Content, Property, AdditionalProperties[0].Property);
            else
                materialEditor.TexturePropertySingleLine(Content, Property);
            
            if (ShowTilingAndOffset || Controls.Count > 0)
            {
                GUI.backgroundColor = OptionsButtonColor;
                IsOptionsButtonPressed = EditorGUILayout.Toggle(IsOptionsButtonPressed, OptionsButtonStyle, GUILayout.Width(19.0f), GUILayout.Height(19.0f));
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            }
            
            if (!_isGradientEditorOpen)
            {
                GUI.backgroundColor = GradientButtonColor;
                if (GUILayout.Button(AdditionalContent[0].Content, GradientButtonStyle))
                {
                    _isGradientEditorOpen = true;
                    _previousTextures = new Texture2D[materialEditor.targets.Length];
                    for (int i = 0; i < Inspector.Materials.Length; i++)
                        _previousTextures[i] = (Texture2D)Inspector.Materials[i].GetTexture(PropertyName);
                    
                    Selection.selectionChanged += ResetGradientTexture;
                    if (_previousTextures != null)
                        TranslateTextureToGradient(_previousTextures[0]);

                    Property.textureValue = _gradient.GetTexture();
                }
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            }
            
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            
            EditorGUILayout.EndHorizontal();
            
            if (IsOptionsButtonPressed)
            {
                GUI.backgroundColor = OptionsAreaColor;
                EditorGUILayout.BeginHorizontal();
                int previousIndent = EditorGUI.indentLevel;
                GUILayout.Space(EditorGUI.indentLevel * 15);
                EditorGUI.indentLevel = 0;
                EditorGUILayout.BeginVertical(OptionsAreaStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                EditorGUI.indentLevel++;
                if (ShowTilingAndOffset)
                    materialEditor.TextureScaleOffsetProperty(Property);
                foreach (var control in Controls)
                    control.DrawControl(materialEditor);
                EditorGUI.indentLevel--;
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = previousIndent;
                EditorGUILayout.EndHorizontal();
            }
            
            if (HasPropertyUpdated && (_hasMinValue || _hasMaxValue))
                UpdateMinMaxProperties();

            if (_isGradientEditorOpen)
            {
                
                GUI.backgroundColor = GradientEditorColor;
                EditorGUILayout.BeginHorizontal();
                int previousIndent = EditorGUI.indentLevel;
                GUILayout.Space(EditorGUI.indentLevel * 15);
                EditorGUI.indentLevel = 0;
                EditorGUILayout.BeginVertical(GradientEditorStyle);
                GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
                DrawGradientEditor(materialEditor);
                EditorGUILayout.EndVertical();
                EditorGUI.indentLevel = previousIndent;
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private void UpdateMinMaxProperties()
        {
            Texture2D ramp = (Texture2D)Property.textureValue;
            Color min = new Color(100, 100, 100, 0);
            Color max = new Color(0, 0, 0, 1);
            if (ramp != null)
            {
                if (!ramp.isReadable)
                {
                    SSIHelper.SetTextureImporterReadable(ramp, true);
                }
                foreach (Color c in ramp.GetPixels())
                {
                    if (min.r > c.r) min.r = c.r; 
                    if (min.g > c.g) min.g = c.g;
                    if (min.b > c.b) min.b = c.b;
                    if (max.r < c.r) max.r = c.r;
                    if (max.g < c.g) max.g = c.g;
                    if (max.b < c.b) max.b = c.b;
                }
            }
            else
            {
                min = new Color(0.9f, 0.9f, 0.9f, 1);
                max = Color.white;
            }

            bool textureGamma = ramp.IsSrgb();
            if (HasExtra1)
            {
                bool colorGamma = AdditionalProperties[0].Property.flags != MaterialProperty.PropFlags.HDR;
                if ( colorGamma && !textureGamma)
                {
                    min *= AdditionalProperties[0].Property.colorValue.linear;
                    max *= AdditionalProperties[0].Property.colorValue.linear;
                }
                else if ( !colorGamma && textureGamma)
                {
                    min *= AdditionalProperties[0].Property.colorValue.gamma;
                    max *= AdditionalProperties[0].Property.colorValue.gamma;
                }
                else
                {
                    min *= AdditionalProperties[0].Property.colorValue;
                    max *= AdditionalProperties[0].Property.colorValue;
                }
                
            }

            if (_hasMinValue)
            {
                bool minColorGamma = AdditionalProperties[1].Property.flags != MaterialProperty.PropFlags.HDR;
                if (minColorGamma && !textureGamma)
                    AdditionalProperties[1].Property.colorValue = min.gamma;
                else if (!minColorGamma && textureGamma)
                    AdditionalProperties[1].Property.colorValue = min.linear;
                else
                    AdditionalProperties[1].Property.colorValue = min;
            }

            if (_hasMaxValue)
            {
                bool maxColorGamma = AdditionalProperties[2].Property.flags != MaterialProperty.PropFlags.HDR;
                if (maxColorGamma && !textureGamma)
                    AdditionalProperties[2].Property.colorValue = max.gamma;
                else if (!maxColorGamma && textureGamma)
                    AdditionalProperties[2].Property.colorValue = max.linear;
                else
                    AdditionalProperties[2].Property.colorValue = max;
            }

        }

        private void DrawGradientEditor(MaterialEditor materialEditor)
        {
            Rect windowRect = GUILayoutUtility.GetRect(100, 1000, 60, 60);
            _gradientPreviewRect = new Rect(windowRect.x + 9, windowRect.y + 9, windowRect.width - 18, 18);
            _keySelectionAreaRect = new Rect(_gradientPreviewRect.x - 9, _gradientPreviewRect.yMax, _gradientPreviewRect.width + 18, 18);
            GUI.DrawTexture(_gradientPreviewRect, _gradient.GetTexture());
            if (_keyRects == null || _keyRects.Length != _gradient.Keys.Count)
            {
                _keyRects = new Rect[_gradient.Keys.Count];
            }
            Rect selectedRect = Rect.zero;
            for (int i = 0; i < _gradient.Keys.Count; i++)
            {
                Rect keyRect = new Rect(_gradientPreviewRect.x + (_gradientPreviewRect.width * _gradient.Keys[i].Time) - 9, _gradientPreviewRect.yMax, 18, 18);
                _keyRects[i] = keyRect;
                if (i == _selectedKeyIndex)
                {
                    selectedRect = keyRect;
                }
                else
                {
                    GUI.DrawTexture(keyRect, Textures.ColorIconBorder);
                    GUI.DrawTexture(keyRect, Textures.ColorIcon, ScaleMode.ScaleToFit, true, 0, _gradient.Keys[i].Color, 0, 0);
                }
            }
            if (selectedRect != Rect.zero)
            {
                GUI.DrawTexture(selectedRect, Textures.ColorIconBorderSelected);
                GUI.DrawTexture(selectedRect, Textures.ColorIcon, ScaleMode.ScaleToFit, true, 0, _gradient.Keys[_selectedKeyIndex].Color, 0, 0);
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(AdditionalContent[1].Content);
            GUILayout.Label(AdditionalContent[2].Content);
            GUILayout.Label(AdditionalContent[3].Content);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            Color col = EditorGUILayout.ColorField(_gradient.Keys[_selectedKeyIndex].Color);
            if (!col.Equals(_gradient.Keys[_selectedKeyIndex].Color))
                _gradient.UpdateKeyColor(_selectedKeyIndex, col);

            float time = (float)Math.Round(EditorGUILayout.FloatField(_gradient.Keys[_selectedKeyIndex].Time), 3);
            if (Math.Abs(time - _gradient.Keys[_selectedKeyIndex].Time) > 0.0001)
                _selectedKeyIndex = _gradient.UpdateKeyTime(_selectedKeyIndex, time);

            _rampWidth = (GradientWidth)EditorGUILayout.EnumPopup(_rampWidth);
            if ((int)_rampWidth != _gradient.GetTexture().width)
            {
                _gradient.UpdateTextureWidth((int)_rampWidth);
                Property.textureValue = _gradient.GetTexture();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            _blendMode = (GradientBlendMode)EditorGUILayout.EnumPopup(AdditionalContent[4].Content, _blendMode);
            if (_blendMode != _gradient.BlendMode)
            {
                _gradient.BlendMode = _blendMode;
                _gradient.UpdateTexture();
            }

            HandleEvents(materialEditor);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = GradientSaveButtonColor;
            if (GUILayout.Button(AdditionalContent[5].Content, GradientSaveButtonStyle))
            {
                string path = SSIHelper.GetTextureDestinationPath((Material)Property.targets[0], PropertyName + "_gradient.png");
                Property.textureValue = SSIHelper.SaveAndGetTexture(_gradient.GetTexture(), path, TextureWrapMode.Clamp);
                HasPropertyUpdated = true;
                _previousTextures = null;
                Selection.selectionChanged -= ResetGradientTexture;
                _isGradientEditorOpen = false;
            }
            if (GUILayout.Button(AdditionalContent[6].Content, GradientSaveButtonStyle))
            {
                _isGradientEditorOpen = false;
                ResetGradientTexture();
            }
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Handles mouse and keyboard events
        /// </summary>
        private void HandleEvents(MaterialEditor materialEditor)
        {
            bool repaint = false;

            Event guiEvent = Event.current;
            // Check when left mouse down
            if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
            {
                // Check if selecting a keyframe
                for (int i = 0; i < _keyRects.Length; i++)
                {
                    if (_keyRects[i].Contains(guiEvent.mousePosition))
                    {
                        _mouseIsDownOverKey = true;
                        _selectedKeyIndex = i;
                        repaint = true;
                        GUI.FocusControl(null);
                        break;
                    }
                }
                // Creates a new keyframe if not selected one
                if (!_mouseIsDownOverKey && _keySelectionAreaRect.Contains(guiEvent.mousePosition))
                {
                    float keytime = Mathf.InverseLerp(_gradientPreviewRect.x, _gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                    _selectedKeyIndex = _gradient.AddKey(_gradient.Evaluate(keytime), keytime);
                    _mouseIsDownOverKey = true;
                    repaint = true;
                }
            }

            // Check left mouse up
            if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
            {
                _mouseIsDownOverKey = false;
            }

            // Check if mouse is dragging
            if (_mouseIsDownOverKey && guiEvent.type == EventType.MouseDrag && guiEvent.button == 0)
            {
                float keytime = Mathf.InverseLerp(_gradientPreviewRect.x, _gradientPreviewRect.xMax, guiEvent.mousePosition.x);
                _selectedKeyIndex = _gradient.UpdateKeyTime(_selectedKeyIndex, keytime);
                repaint = true;
            }
            // Check if using the delete key
            if (guiEvent.keyCode == KeyCode.Delete && guiEvent.type == EventType.KeyDown)
            {
                _gradient.RemoveKey(_selectedKeyIndex);
                if (_selectedKeyIndex >= _gradient.Keys.Count)
                {
                    _selectedKeyIndex = _gradient.Keys.Count - 1;
                }
                repaint = true;
            }

            if (repaint)
                materialEditor.Repaint();
        }

        private void TranslateTextureToGradient(Texture2D texture)
        {
            if (texture == null) return;
            if (!texture.isReadable)
                SSIHelper.SetTextureImporterReadable(texture, true);
            
            _gradient = new GradientTexture((int)_rampWidth);
            _blendMode = GradientBlendMode.Linear;
            int sampleWidth = Mathf.Max(texture.width, texture.height);
            Color[] colors = new Color[sampleWidth];

            for (int i = 0; i < sampleWidth; i++)
                colors[i] = texture.GetPixel(texture.width * i / sampleWidth, texture.height * i / sampleWidth);
            
            Color[] delta = GetDelta(colors);
            int deltaVariance = 0;
            
            for (int i = 0; i < delta.Length; i++)
                if (delta[i].r != 0 || delta[i].g != 0 || delta[i].b != 0)
                    deltaVariance++;

            if (deltaVariance < Mathf.Max(texture.width, texture.height) * 0.1)
                _blendMode = GradientBlendMode.Fixed;
            
            _gradient.BlendMode = _blendMode;
            delta[0] = delta[1];

            Color[] deltaSquared = GetDelta(delta);
            List<GradientTexture.ColorKey> keyChanged = new List<GradientTexture.ColorKey>();
            List<Color> deltaChanged = new List<Color>();
            keyChanged.Add(new GradientTexture.ColorKey(colors[0], 0));
            deltaChanged.Add(deltaSquared[0]);
            for (int i = 1; i < sampleWidth - 1; i++)
            {
                if (Mathf.Abs(deltaSquared[i].r) > 0.002 || Mathf.Abs(deltaSquared[i].g) > 0.002 || Mathf.Abs(deltaSquared[i].b) > 0.002)
                {
                    if (_blendMode == GradientBlendMode.Fixed)
                    {
                        keyChanged.Add(new GradientTexture.ColorKey(colors[i - 1], i / ((float)sampleWidth - 1)));
                        deltaChanged.Add(deltaSquared[i]);
                    }
                    else
                    {
                        keyChanged.Add(new GradientTexture.ColorKey(colors[i], i / ((float)sampleWidth - 1)));
                        deltaChanged.Add(deltaSquared[i]);
                    }
                }
            }
            keyChanged.Add(new GradientTexture.ColorKey(colors[sampleWidth - 1], 1));
            deltaChanged.Add(deltaSquared[sampleWidth - 1]);
            List<GradientTexture.ColorKey> finalKeys = new List<GradientTexture.ColorKey>
            {
                keyChanged[0]
            };
            Color lastDelta = new Color(0, 0, 0, 0);
            if (_blendMode == GradientBlendMode.Fixed)
            {
                for (int i = 1; i < keyChanged.Count; i++)
                {
                    if ((keyChanged[i].Time - finalKeys[finalKeys.Count - 1].Time) > 0.05)
                    {
                        finalKeys.Add(keyChanged[i]);
                    }
                }
            }
            else
            {
                for (int i = 1; i < keyChanged.Count; i++)
                {
                    float deltaVarianceSum = Mathf.Abs(GetDeltaVariance(deltaChanged[i]) - GetDeltaVariance(lastDelta));
                    if (keyChanged[i].Time - finalKeys[finalKeys.Count - 1].Time + deltaVarianceSum > 0.1 && deltaVarianceSum > 0.002)
                    {
                        finalKeys.Add(keyChanged[i]);
                        lastDelta = deltaChanged[i];
                    }
                }
                if (finalKeys[finalKeys.Count - 1].Time < 1)
                {
                    finalKeys.Add(keyChanged[keyChanged.Count - 1]);
                }
            }

            _gradient.Keys = finalKeys;
            _gradient.UpdateTexture();
        }

        private static Color[] GetDelta(Color[] colors)
        {
            Color[] delta = new Color[colors.Length];
            delta[0] = new Color(0, 0, 0);
            for (int i = 1; i < colors.Length; i++)
            {
                delta[i].r = colors[i - 1].r - colors[i].r;
                delta[i].g = colors[i - 1].g - colors[i].g;
                delta[i].b = colors[i - 1].b - colors[i].b;
            }

            return delta;
        }

        private static float GetDeltaVariance(Color delta)
        {
            return Mathf.Max(new[] { Mathf.Abs(delta.r), Mathf.Abs(delta.g), Mathf.Abs(delta.b) });
        }

        private void ResetGradientTexture()
        {
            if (_previousTextures != null)
            {
                for (int i = 0; i < Inspector.Materials.Length; i++)
                    Inspector.Materials[i].SetTexture(PropertyName, _previousTextures[i]);

                _previousTextures = null;
            }
            Selection.selectionChanged -= ResetGradientTexture;
        }
    }

    /// <summary>
    /// Supported gradient widths
    /// </summary>
    public enum GradientWidth
    {
        XS_128 = 128,
        S_256 = 256,
        M_512 = 512,
        L_1024 = 1024,
        XL_2048 = 2048,
        XXL_4096 = 4096
    }
}