using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Object = UnityEngine.Object;


namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Base class for creating new inspectors.
    /// </summary>
    public abstract class SimpleShaderInspector : ShaderGUI, ISimpleShaderInspector
    {
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

        private bool ContainsNonAnimatableProperties => _nonAnimatablePropertyControls.Count > 0;

        private List<INonAnimatableProperty> _nonAnimatablePropertyControls;

        private Texture2D _logo;

        /// <summary>
        /// Default gui background color.
        /// </summary>
        public static Color DefaultBgColor { get; set; } = GUI.backgroundColor;

        /// <summary>
        /// Array of selected materials
        /// </summary>
        public Material[] Materials { get; private set; }

        // Reference to the shader used
        public Shader Shader { get; private set; }

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
        protected bool NeedsNonAnimatableUpdate { get; set; }

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
        /// Check changes happened to properties.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        protected virtual void CheckChanges(MaterialEditor materialEditor) { }

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
                NeedsNonAnimatableUpdate = false;
                Controls = new List<SimpleControl>();
                Materials = Array.ConvertAll(materialEditor.targets, item => (Material)item);
                Shader = Materials[0].shader;
                Start();
                LoadLocalizations();
                Controls.SetInspector(this);
                _nonAnimatablePropertyControls = (List<INonAnimatableProperty>)Controls.FindNonAnimatablePropertyControls();
                Controls.FetchProperties(properties);
                StartChecks(materialEditor);
                _isFirstLoop = false;
                if (Controls == null || Controls.Count == 0)
                    _doesContainControls = false;
            }
            else
            {
                Controls.FetchProperties(properties);
            }

            Header();
            DrawGUI(materialEditor, properties);
            if (ContainsNonAnimatableProperties)
                SSIHelper.UpdateNonAnimatableProperties(_nonAnimatablePropertyControls, materialEditor, NeedsNonAnimatableUpdate);

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
        }

        /// <summary>
        /// Load/reload the currently selected localization.
        /// </summary>
        private void LoadLocalizations()
        {
            // Initializes path if it hasn't been initialized.
            if (string.IsNullOrWhiteSpace(_path))
            {
                _path = AssetDatabase.GetAssetPath(Shader);
                if (string.IsNullOrWhiteSpace(CustomLocalizationShaderName))
                    CustomLocalizationShaderName = Path.GetFileNameWithoutExtension(_path);

                _path = $"{Path.GetDirectoryName(_path)}/Localization/{CustomLocalizationShaderName}";

                if (Directory.Exists(_path))
                    Directory.CreateDirectory(_path);
            }

            // Get Localization.  
            string settingsPath = $"{_path}/Settings.json";
            if (File.Exists(settingsPath))
            {
                _selectedLanguage = JsonUtility.FromJson<SettingsFile>(File.ReadAllText(settingsPath)).SelectedLanguage;
            }
            else
            {
                File.WriteAllText(settingsPath, JsonUtility.ToJson(new SettingsFile { SelectedLanguage = "English" }));
                _selectedLanguage = "English";
            }

            Controls.ApplyLocalization($"{_path}/{_selectedLanguage}.json", true);

            // Generate language options array based on available localizations.
            List<string> names = new List<string>();
            string[] localizations = Directory.GetFiles(_path);
            for (int i = 0; i < localizations.Length; i++)
            {
                if (localizations[i].EndsWith(".json", StringComparison.OrdinalIgnoreCase) && !localizations[i].EndsWith("Settings.json"))
                {
                    string name = Path.GetFileNameWithoutExtension(localizations[i]);
                    names.Add(name);
                    if (name.Equals(_selectedLanguage))
                    {
                        _selectedLanguageIndex = names.Count - 1;
                    }
                }
            }
            _languages = names.ToArray();
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
                        Controls.ApplyLocalization($"{_path}/{_selectedLanguage}.json", true);
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                }

                // Draw controls
                foreach (var control in Controls)
                    control.DrawControl(materialEditor);
            }
            else
            {
                // In a working inspector it should never appear.
                EditorGUILayout.HelpBox("No controls have been passed to the Start() method, therefore a default inspector has been drawn, if you are an end user of the shader try to reinstall the shader or contact the creator.", MessageType.Error);
                base.OnGUI(materialEditor, properties);
            }
        }

        // Draws the SSI logo
        private void SSILogo()
        {
            if (GUILayout.Button(new GUIContent(_logo, "Check out Simple Shader Inspectors!"), "label", GUILayout.Width(32), GUILayout.Height(32)))
                Application.OpenURL("https://github.com/VRLabs/SimpleShaderInspectors");
        }

        public void AddControl(SimpleControl control) => Controls.Add(control);

        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}
