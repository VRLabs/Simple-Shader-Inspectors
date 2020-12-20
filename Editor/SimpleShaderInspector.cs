using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Diagnostics;
using System;
using Object = UnityEngine.Object;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Base class for creating new inspectors.
    /// </summary>
    public abstract class SimpleShaderInspector : ShaderGUI, IControlContainer
    {
        // Reference to the shader used
        private Shader _shader;
        // Array containing all found languages for a specific GUI.
        private string[] _languages;
        // String containing the selected language.
        private string _selectedLanguage;
        // Integer containing the index of the selected language into the languages array.
        private int _selectedLanguageIndex;
        // Path of the shader.
        private string _path;
        // Bool that determines if the current OnGUI call is the first one or not.
        private bool _isFirstLoop = true;
        // Bool that determines if the child class added some controls or not.
        private bool _doesContainControls = true;

        private Texture2D _logo;

        /// <summary>
        /// Default gui background color.
        /// </summary>
        public static Color DefaultBgColor { get; set; } = GUI.backgroundColor;

        /// <summary>
        /// Array of selected materials
        /// </summary>
        public Material[] Materials { get; set; }

        /// <summary>
        /// List of controls.
        /// </summary>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// String containing a custom folder name for localization of shaders.
        /// </summary>
        protected string CustomLocalizationShaderName { get; set; }

        /// <summary>
        /// Boolean value that defines if the inspector should check for non animatable properties.
        /// </summary>
        protected bool HasNonAnimatableProperties { get; set; }

        /// <summary>
        /// Inizialization method where all the controls are instanced. You need to override it.
        /// </summary>
        protected abstract void Start();

        /// <summary>
        /// Draw method that is used before drawing controls in the inspector.
        /// </summary>
        protected virtual void Header() { }

        /// <summary>
        /// Draw method that is used after drawing controls in the inspector.
        /// </summary>
        protected virtual void Footer() { }

        /// <summary>
        /// Checks done on the first cycle before UI is drawn.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        protected virtual void StartChecks(MaterialEditor materialEditor) { }

        /// <summary>
        /// Method called when updating UI. Cannot be overridden in child classes, leave it alone.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        /// <param name="properties">List of MaterialProperties currently available on the selected shader.</param>
        public sealed override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (_isFirstLoop)
            {
                DefaultBgColor = GUI.backgroundColor;
                _logo = EditorGUIUtility.isProSkin ? Styles.SSILogoDark : Styles.SSILogoLight;
                //var stopWatch = new Stopwatch();
                //stopWatch.Start();
                HasNonAnimatableProperties = false;
                Controls = new List<SimpleControl>();
                Materials = Array.ConvertAll(materialEditor.targets, item => (Material)item);
                _shader = Materials[0].shader;
                Start();
                FetchOrGenerateLocalization();
                ApplyPropertiesIndex(Controls, properties);
                FetchProperties(Controls, properties);
                StartChecks(materialEditor);
                _isFirstLoop = false;
                if (Controls == null || Controls.Count == 0)
                {
                    _doesContainControls = false;
                }
                //stopWatch.Stop();
                //UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds);
            }
            else
            {
                FetchProperties(Controls, properties);
            }
            //var stopWatch = new Stopwatch();
            //stopWatch.Start();
            Header();
            DrawGUI(materialEditor, properties);
            UpdateNonAnimatableProperties(materialEditor);

            // Draw footer and inspector logo.
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            Footer();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            SSILogo();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            CheckChanges(materialEditor);
            //stopWatch.Stop();
            //UnityEngine.Debug.Log(stopWatch.ElapsedMilliseconds);
        }

        /// <summary>
        /// Check changes happened to properties.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        protected virtual void CheckChanges(MaterialEditor materialEditor) { }
        // Updates properties that should not be tracked while the animation tab is recording
        private void UpdateNonAnimatableProperties(MaterialEditor materialEditor)
        {
            List<INonAnimatableProperty> nonAnimatableProperties = new List<INonAnimatableProperty>();
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is INonAnimatableProperty control)
                {
                    if (control.NonAnimatablePropertyChanged)
                    {
                        nonAnimatableProperties.Add(control);
                    }
                }
            }

            if (nonAnimatableProperties.Count == 0) return;
            if (HasNonAnimatableProperties)
            {
                // Reflection bs to get which animation window is recording
                System.Reflection.Assembly editorAssembly = typeof(Editor).Assembly;
                System.Type windowType = editorAssembly.GetType("UnityEditorInternal.AnimationWindowState");

                System.Reflection.PropertyInfo isRecordingProp = windowType.GetProperty
                    ("recording", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                Object[] windowInstances = Resources.FindObjectsOfTypeAll(windowType);
                Object recordingInstance = null;

                for (int i = 0; i < windowInstances.Length; i++)
                {
                    bool isRecording = (bool)isRecordingProp.GetValue
                        (windowInstances[i], null);

                    if (isRecording)
                    {
                        recordingInstance = windowInstances[i];
                        break;
                    }
                }
                if (recordingInstance != null)
                {
                    System.Reflection.MethodBase stopRecording = windowType.GetMethod
                    ("StopRecording", System.Reflection.BindingFlags.Instance |
                                      System.Reflection.BindingFlags.Public);
                    System.Reflection.MethodBase startRecording = windowType.GetMethod
                    ("StartRecording", System.Reflection.BindingFlags.Instance |
                                       System.Reflection.BindingFlags.Public);

                    stopRecording?.Invoke(recordingInstance, null);
                    SetNonAnimatableProperties(materialEditor, nonAnimatableProperties);
                    startRecording?.Invoke(recordingInstance, null);
                }
                else
                {
                    SetNonAnimatableProperties(materialEditor, nonAnimatableProperties);
                }
            }
            else
            {
                for (int i = 0; i < nonAnimatableProperties.Count; i++)
                {
                    SetNonAnimatableProperties(materialEditor, nonAnimatableProperties);
                }
            }
        }
        // Retrieve localization information or creates a template one if it's missing.
        // I really need to improve this mess one day.
        private void FetchOrGenerateLocalization()
        {
            // Initializes path if it hasn't been initialized.
            if (string.IsNullOrWhiteSpace(_path))
            {
                _path = AssetDatabase.GetAssetPath(_shader);
                if (string.IsNullOrWhiteSpace(CustomLocalizationShaderName))
                {
                    CustomLocalizationShaderName = Path.GetFileNameWithoutExtension(_path);
                }
                _path = Path.GetDirectoryName(_path) + "/Localization/" + CustomLocalizationShaderName;
            }

            // Get Localization.
            (_languages, _selectedLanguage, _selectedLanguageIndex) = Localization.GetLocalization(_path);

            // Generates localization if null or retrieve it otherwise.
            if (_languages == null)
            {
                (_languages, _selectedLanguage, _) = Localization.GenerateDefaultLocalization(Controls, _shader, _path);
                _selectedLanguageIndex = 0;
            }
            SetLocalization();
        }

        // Retrieves the selected localization and applies it.
        private void SetLocalization()
        {
            LocalizationFile localization = Localization.GetSelectedLocalization(_path, _selectedLanguage);
            Localization.SaveSettings(_selectedLanguage, _path);
            List<PropertyInfo> missingInfo = new List<PropertyInfo>();
            SetPropertiesLocalization(Controls, localization, missingInfo);
            if (missingInfo.Count > 0)
            {
                Localization.AddDefaultsForMissingProperties(missingInfo, _selectedLanguage, _path);
            }
        }

        // Applies a given localization to all controls recursively.
        private void SetPropertiesLocalization(List<SimpleControl> controls, LocalizationFile localization, List<PropertyInfo> missingInfo)
        {
            foreach (SimpleControl control in controls)
            {
                control.Content = GetLocalizationContent(control, control.ControlAlias, localization, missingInfo);
                switch (control)
                {
                    // Check if control has additional localization GUIContent to fill.
                    case IAdditionalLocalization additional:
                        {
                            foreach (AdditionalLocalization content in additional.AdditionalContent)
                            {
                                string fullName = control.ControlAlias + "_" + content.Name;
                                content.Content = GetLocalizationContent(control, fullName, localization, missingInfo);
                            }

                            break;
                        }
                    // Check if control has additional child controls.
                    case IControlContainer container:
                        SetPropertiesLocalization(container.Controls, localization, missingInfo);
                        break;
                }
            }
        }

        // Draws the custom GUI.
        private void DrawGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (_doesContainControls)
            {
                // Draw the language selector only if there is more than 1 language available.
                if (_languages.Length > 1)
                {
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.FlexibleSpace();
                    int s = EditorGUILayout.Popup(_selectedLanguageIndex, _languages, GUILayout.Width(120));
                    if (s != _selectedLanguageIndex)
                    {
                        _selectedLanguageIndex = s;
                        _selectedLanguage = _languages[s];
                        SetLocalization();
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                // Draw controls
                foreach (SimpleControl control in Controls)
                {
                    control.DrawControl(materialEditor);
                }
            }
            else
            {
                // Literally the only non localizable thing in the entire inspector. not sure if i want to do something about it.
                // In a working inspector it should never appear to begin with.
                EditorGUILayout.HelpBox("No controls have been passed to the Start() method, therefore a default inspector has been drawn, if you are an end user of the shader try to reinstall the shader or contact the creator.", MessageType.Error);
                base.OnGUI(materialEditor, properties);
            }
        }

        // Gets the GUIContent of selected controls.
        private GUIContent GetLocalizationContent(SimpleControl control, string fullName, LocalizationFile localization, List<PropertyInfo> missingInfo)
        {
            PropertyInfo info = null;
            for (int i = 0; i < localization.Properties.Length; i++)
            {
                if (localization.Properties[i].Name.Equals(fullName))
                {
                    info = localization.Properties[i];
                    break;
                }
            }

            if (info != null) return new GUIContent(info.DisplayName, info.Tooltip);
            string displayName;
            if (control is PropertyControl pr)
            {
                displayName = Localization.GetPropertyDescription(pr.PropertyName, _shader);
            }
            else
            {
                displayName = Localization.GetPropertyDescription(control.ControlAlias, _shader);
            }
            info = new PropertyInfo
            {
                Name = fullName,
                DisplayName = displayName,
                Tooltip = displayName
            };
            if (!string.IsNullOrEmpty(fullName))
            {
                missingInfo.Add(info);
            }
            return new GUIContent(info.DisplayName, info.Tooltip);
        }

        // Draws the SSI logo
        private void SSILogo()
        {
            if (GUILayout.Button(new GUIContent(_logo, "Check out Simple Shader Inspectors!"), "label", GUILayout.Width(32), GUILayout.Height(32)))
            {
                Application.OpenURL("https://github.com/VRLabs/SimpleShaderInspectors");
            }
        }

        // Caches all properties indexes used in the shader.
        private static void ApplyPropertiesIndex(List<SimpleControl> controls, MaterialProperty[] properties)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is PropertyControl pr)
                {
                    pr.SetPropertyIndex(properties);
                }

                if (controls[i] is IAdditionalProperties add)
                {
                    for (int j = 0; j < add.AdditionalProperties.Length; j++)
                    {
                        add.AdditionalProperties[j].SetPropertyIndex(properties);
                    }
                }

                if (controls[i] is IControlContainer con)
                {
                    ApplyPropertiesIndex(con.Controls, properties);
                }
            }
        }

        // Fetches all properties from the currently given properties array.
        private static void FetchProperties(List<SimpleControl> controls, MaterialProperty[] properties)
        {
            for (int i = 0; i < controls.Count; i++)
            {
                if (controls[i] is PropertyControl pr)
                {
                    pr.FetchProperty(properties);
                }

                if (controls[i] is IAdditionalProperties add)
                {
                    for (int j = 0; j < add.AdditionalProperties.Length; j++)
                    {
                        add.AdditionalProperties[j].FetchProperty(properties);
                    }
                }

                if (controls[i] is IControlContainer con)
                {
                    FetchProperties(con.Controls, properties);
                }
            }
        }

        private static void SetNonAnimatableProperties(MaterialEditor materialEditor, List<INonAnimatableProperty> nonAnimatableProperties)
        {
            for (int i = 0; i < nonAnimatableProperties.Count; i++)
            {
                nonAnimatableProperties[i].UpdateNonAnimatableProperty(materialEditor);
                nonAnimatableProperties[i].NonAnimatablePropertyChanged = false;
            }
        }

        /// <summary>
        /// Find a material property from its name.
        /// </summary>
        /// <param name="propertyName">Name of the material proeperty.</param>
        /// <param name="properties">Array of material properties to search from.</param>
        /// <param name="propertyIsMandatory">Boolean indicating if it's mandatory to find the requested material property</param>
        /// <returns>The material property with the wanted name.</returns>
        internal static int FindPropertyIndex(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory = false)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] != null && properties[i].name == propertyName)
                    return i;
            }

            // We assume all required properties can be found, otherwise something is broken.
            if (propertyIsMandatory)
                throw new ArgumentException("Could not find MaterialProperty: '" + propertyName + "', Num properties: " + properties.Length);
            return -1;
        }

        /// <summary>
        /// Get a path to save a texture relative to the material.
        /// </summary>
        /// <param name="mat">Material.</param>
        /// <param name="name">Name of the texture.</param>
        /// <returns>A path for the texture to save.</returns>
        public static string GetTextureDestinationPath(Material mat, string name)
        {
            string path = AssetDatabase.GetAssetPath(mat);
            path = Directory.GetParent(path).FullName;
            string pathParent = Directory.GetParent(path).FullName;
            if (Directory.Exists(pathParent + "/Textures/"))
            {
                return pathParent + "/Textures/" + mat.name + name;
            }
            else
            {
                return path + "/" + mat.name + name;
            }
        }

        /// <summary>
        /// Saves a texture to a specified path.
        /// </summary>
        /// <param name="texture">Texture to save.</param>
        /// <param name="path">path where you want to save the texture.</param>
        /// <param name="mode">Texture wrap mode (default: Repeat).</param>
        public static void SaveTexture(Texture2D texture, string path, TextureWrapMode mode = TextureWrapMode.Repeat)
        {
            byte[] bytes = texture.EncodeToPNG();

            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            path = path.Substring(path.LastIndexOf("Assets"));
            TextureImporter t = AssetImporter.GetAtPath(path) as TextureImporter;
            t.wrapMode = mode;
            t.isReadable = true;
            AssetDatabase.ImportAsset(path);
        }

        /// <summary>
        /// Saves a texture to a specified path, and returns a reference of the new asset.
        /// </summary>
        /// <param name="texture">Texture to save.</param>
        /// <param name="path">path where you want to save the texture.</param>
        /// <param name="mode">Texture wrap mode (default: Repeat).</param>
        /// <returns>A Texture2D that references the newly created asset.</returns>
        public static Texture2D SaveAndGetTexture(Texture2D texture, string path, TextureWrapMode mode = TextureWrapMode.Repeat)
        {
            SaveTexture(texture, path, mode);
            path = path.Substring(path.LastIndexOf("Assets"));
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>
        /// Set the texture readable state.
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <param name="isReadable">Does the texture need to be readable.</param>
        public static void SetTextureImporterReadable(Texture2D texture, bool isReadable)
        {
            if (texture == null) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.Default;
                tImporter.isReadable = isReadable;
                AssetDatabase.ImportAsset(assetPath);
                //AssetDatabase.Refresh();
            }
        }

        public static void SetTextureImporterAlpha(Texture2D texture, bool alphaIsTransparency)
        {
            if (texture == null) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.Default;
                tImporter.alphaIsTransparency = alphaIsTransparency;
                AssetDatabase.ImportAsset(assetPath);
            }
        }
    }

    /// <summary>
    /// This class contains extension methods that are meant to be used inside a SimpleShaderInspector child class.
    /// </summary>
    public static class SSIExtensions
    {
        /// <summary>
        /// Sets a keyword state to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="keyword">The keyword that is being toggled.</param>
        /// <param name="state">Toggle value.</param>
        public static void SetKeyword(this Material[] materials, string keyword, bool state)
        {
            foreach (var m in materials)
            {
                if (state)
                    m.EnableKeyword(keyword);
                else
                    m.DisableKeyword(keyword);
            }
        }

        /// <summary>
        /// Gets the mixed value state of a keyword on the materials array
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="keyword">The keyword to check against.</param>
        /// <returns>True if the keyword has mixed values, false otherwise.</returns>
        public static bool IsKeywordMixedValue(this Material[] materials, string keyword)
        {
            bool reference = materials[0].IsKeywordEnabled(keyword);
            foreach (var m in materials)
            {
                if (m.IsKeywordEnabled(keyword) != reference)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set override tag to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="value">Value of the tag.</param>
        public static void SetOverrideTag(this Material[] materials, string tagName, string value)
        {
            foreach (var m in materials)
                m.SetOverrideTag(tagName, value);
        }

        /// <summary>
        /// Set int to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the int.</param>
        /// <param name="value">Value of the int.</param>
        public static void SetInt(this Material[] materials, string name, int value)
        {
            foreach (var m in materials)
                m.SetInt(name, value);
        }

        /// <summary>
        /// Set vector to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the Vector4.</param>
        /// <param name="value">Value of the Vector4.</param>
        public static void SetVector(this Material[] materials, string name, Vector4 value)
        {
            foreach (var m in materials)
                m.SetVector(name, value);
        }

        /// <summary>
        /// Set render queue to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="queue">Render queue value.</param>
        public static void SetRenderQueue(this Material[] materials, int queue)
        {
            foreach (var m in materials)
                m.renderQueue = queue;
        }
    }
}
