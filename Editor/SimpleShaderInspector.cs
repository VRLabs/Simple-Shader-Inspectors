using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System;
using Object = UnityEngine.Object;


namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Default base class for creating new inspectors.
    /// </summary>
    /// <remarks>
    /// If you want to make an inspector with Simple Shader Inspectors you need to either derive from this class, or create your own implementing <see cref="ISimpleShaderInspector"/>,
    /// but in this second case you need to implement a lot of stuff manually, so it isn't advised unless you need really custom inspector behaviours.
    /// </remarks>
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
        /// <value>
        /// Default gui background color.
        /// </value>
        /// <remarks>This static property makes easy to reset the background color inside a control without the need to locally store the default background color before changing it.</remarks>
        public static Color DefaultBgColor { get; set; } = GUI.backgroundColor;

        /// <summary>
        /// Array of selected materials
        /// </summary>
        /// <value>
        ///Array containing the materials currently selected by the inspector.
        /// </value>
        public Material[] Materials { get; private set; }

        /// <summary>
        /// Shader currently used
        /// </summary>
        /// <value>
        /// Contains the shader this inspector is viewing at the moment.
        /// </value>
        public Shader Shader { get; private set; }

        /// <summary>
        /// List of controls.
        /// </summary>
        /// <value>
        /// List of control that the inspector had to draw.
        /// </value>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// String containing a custom folder name for localization of shaders.
        /// </summary>
        /// <value>
        /// Path relative to the folder containing localization files
        /// </value>
        /// <remarks>
        /// This string does NOT contain the entire path string, just a sub path from <c>ShaderFolderName/Localization/</c>.
        /// </remarks>
        protected string CustomLocalizationShaderName { get; set; }

        /// <summary>
        /// Boolean value that defines if the inspector should check for non animatable properties.
        /// </summary>
        /// <value>True if non animatable properties should be granted their need to update outside of the animation recording, false otherwise. (default: false)</value>
        /// <remarks>
        /// Due to the cost of Reflection needed to get non animatable properties to be updated without animation recording the change, by default this behaviour is not enabled.
        /// </remarks>
        protected bool NeedsNonAnimatableUpdate { get; set; }

        /// <summary>
        /// Initialization method where all the controls are instanced. You need to override it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is called only once at the beginning of the inspector lifecycle, and it should be used to create all controls that are needed, and set properties like
        /// <see cref="NeedsNonAnimatableUpdate"/> or <see cref="CustomLocalizationShaderName"/> if a value different than the default one is needed.
        /// </para>
        /// <para>Keep in mind that <c>MaterialProperties</c> inside controls at this stage have not been filled in yet, so any attempt to use them would result in a <c>NullReferenceException</c></para>
        /// </remarks>
        protected abstract void Start();

        /// <summary>
        /// Draw method that is used before drawing controls in the inspector.
        /// </summary>
        /// <remarks>
        /// This method is called before drawing any control (but after the language selector) and can be used to draw any kind of static stuff, like a title or a logo visualization.
        /// </remarks>
        protected virtual void Header() { }

        /// <summary>
        /// Draw method that is used after drawing controls in the inspector.
        /// </summary>
        /// <remarks>
        /// This method is called at the end of the inspector, either at the bottom (in unity 2018.4) or right after the last control (in unity 2019.2+).
        /// </remarks>
        protected virtual void Footer() { }

        /// <summary>
        /// Checks done on the first cycle before UI is drawn.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        /// <remarks>
        /// This method is called after the <see cref="Start"/> function, and at this stage material properties have been fetched and can be used for whatever is needed.
        /// </remarks>
        protected virtual void StartChecks(MaterialEditor materialEditor) { }

        /// <summary>
        /// Check changes happened to properties.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        /// <remarks>
        /// This method is called after drawing all controls, and can be used to check if anything important has been changed by the user and some operations are needed to be done
        /// (for example the user has changed a property to a certain value and therefore some controls need to be enabled or disables)
        /// </remarks>
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
            DrawFooter(materialEditor);

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

                if (!Directory.Exists(_path))
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
        
        // Draws the footer
        private void DrawFooter(MaterialEditor materialEditor)
        {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            
            EditorGUILayout.BeginVertical();
            Footer();
            EditorGUILayout.EndVertical();
            
            GUILayout.FlexibleSpace();
            // BTW i hate laying out stuff at the bottom right with unity's bs gui
            EditorGUILayout.BeginVertical(GUILayout.MinHeight(42));
            SSILogo(32);
            EditorGUILayout.EndVertical();
            
            EditorGUILayout.EndHorizontal();
        }

        // Draws the SSI logo
        private void SSILogo(float logoHeight)
        {
            if (GUILayout.Button(new GUIContent(_logo, "Check out Simple Shader Inspectors!"), Styles.BottomCenterLabel, 
                GUILayout.Width(logoHeight), GUILayout.MaxHeight(logoHeight+10), GUILayout.ExpandHeight(true)))
                Application.OpenURL("https://github.com/VRLabs/SimpleShaderInspectors");
        }
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to add controls.
        /// </summary>
        /// <param name="control">Control to add.</param>
        public void AddControl(SimpleControl control) => Controls.Add(control);
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to get the object's controls list.
        /// </summary>
        /// <returns>This inspector's controls list</returns>
        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}
