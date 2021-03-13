using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Represents a grouping of controls with an header that can be folded in and out.
    /// </summary>
    /// <remarks>
    /// <para>
    /// On bigger shaders it may be needed to organize options in categories to make easier for the user to find the option they want to modify. This control makes you able to
    /// organize it with ease, by grouping controls under categories with a dedicated title.
    /// </para>
    /// <para>
    /// The folding state can be controlled either by the control alone, or driven by a material property.
    /// In this last case the folding state will stay saved between editor sessions.
    /// </para>
    /// <para>
    /// In case a material property isn't used, a default alias is used.
    /// This alias is shared between al sections, so you need to override it if you want to use a different string for each section.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds a section and sets its alias
    /// this.AddSection().Alias("ExampleAlias"); 
    ///
    /// // Adds a section that uses a material property for its folding state.
    /// this.AddSection("_ExampleProperty"); 
    ///
    /// // Adds a section that uses a material property for its folding state.
    /// // When not folded out the property value will be 2, when folded out 6
    /// this.AddSection("_ExampleProperty", 2, 6); 
    /// </code>
    /// </example>
    public class Section : PropertyControl, IControlContainer, INonAnimatableProperty
    {
        /// <summary>
        /// Float value that the Show bool gets converted if true.
        /// </summary>
        /// <value>
        /// Value of the material property when the section is open.
        /// </value>
        protected readonly float showValue;

        /// <summary>
        /// Float value that the Show bool gets converted if false.
        /// </summary>
        /// <value>
        /// Value of the material property when the section is closed.
        /// </value>
        protected readonly float hideValue;

        /// <summary>
        /// Boolean indicating if the section folding state is driven by an internal dictionary or not. It will be true in case you don't use a material property.
        /// </summary>
        /// <value>
        /// True if it uses the internal dictionary, false when it uses a material property.
        /// </value>
        protected readonly bool useDictionary;

        /// <summary>
        /// String containing the key value that the section will use for the dictionary.
        /// </summary>
        /// <value>
        /// The key used for the dictionary value.
        /// </value>
        protected string dictionaryKey = null;

        /// <summary>
        /// Boolean indicating if it's the first ui update.
        /// </summary>
        /// <value>
        /// True if it's the first UI update, false otherwise.
        /// </value>
        protected bool firstCycle = true;

        /// <summary>
        /// List of controls inside this section.
        /// </summary>
        /// <value>
        /// List of controls that this section shows/hides.
        /// </value>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// Boolean indicating if the folding state material property needs to be updated. Implementation needed by <see cref="INonAnimatableProperty"/>.
        /// </summary>
        /// <code>
        /// True if the non animatable property changed, false otherwise.
        /// </code>
        public bool NonAnimatablePropertyChanged { get; set; }

        /// <summary>
        /// Bool indicating if the section is folded out or not.
        /// </summary>
        /// <value>
        /// True if the section is folded out, false otherwise.
        /// </value>
        public bool Show { get; protected set; }

        /// <summary>
        /// Style of the header label.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the header label.
        /// </value>
        [Chainable] public GUIStyle LabelStyle { get; set; }

        /// <summary>
        /// Style of the header background.
        /// </summary>
        /// <value>
        /// GUIStyle used when displaying the header background.
        /// </value>
        [Chainable] public GUIStyle BackgroundStyle { get; set; }

        /// <summary>
        /// Boolean indicating if child controls will be inside the header background.
        /// </summary>
        /// <value>
        /// True if the child controls are displayed inside the background style of the header, false otherwise.
        /// </value>
        [Chainable] public bool AreControlsInHeader { get; set; }

        /// <summary>
        /// Boolean indicating if the folding state material property is animatable or not.
        /// </summary>
        /// <value>
        /// True if the property can be animated, false otherwise.
        /// </value>
        [Chainable] public bool IsPropertyAnimatable { get; set; }

        /// <summary>
        /// Boolean indicating if the foldout arrow is enabled or not.
        /// </summary>
        /// <value>
        /// True if the foldout arrow in the header is visible, false otherwise.
        /// </value>
        [Chainable] public bool ShowFoldoutArrow { get; set; }

        /// <summary>
        /// Background color of the header.
        /// </summary>
        /// <value>
        /// Color used when displaying the header background.
        /// </value>
        [Chainable] public Color BackgroundColor { get; set; }

        /// <summary>
        /// Constructor of <see cref="Section"/> used when creating a property driven section
        /// </summary>
        /// <param name="propertyName">Material property that will drive the section open state</param>
        /// <param name="hideValue">Float value that the material property will have if the section is collapsed, optional (default: 0).</param>
        /// <param name="showValue">Float value that the material property will have if the section is visible, optional (default: 1).</param>
        public Section(string propertyName, float hideValue = 0, float showValue = 1) : base(propertyName)
        {
            InitSection();
            useDictionary = false;
            this.hideValue = hideValue;
            this.showValue = showValue;
        }

        /// <summary>
        /// Default constructor of <see cref="Section"/>.
        /// </summary>
        public Section() : base("")
        {
            InitSection();
            useDictionary = true;
            ControlAlias = "Section";
            hideValue = 0;
            showValue = 1;
        }

        private void InitSection()
        {
            Controls = new List<SimpleControl>();
            BackgroundColor = new Color(1, 1, 1, 1);
            LabelStyle = Styles.BoldCenter;
            BackgroundStyle = Styles.BoxHeavyBorder;
            AreControlsInHeader = false;
            IsPropertyAnimatable = false;
            ShowFoldoutArrow = true;
        }

        /// <summary>
        /// Sets the Enabled boolean to the value currently in the source based on the settings of this control.
        /// Should only be used inside <see cref="ControlGUI"/>.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        /// <remarks>
        /// This method is available for all section controls inheriting this class and will automatically fetch the current value of the <see cref="Show"/> boolean based on if
        /// the section folding state is handled by a material property or a dictionary value. You should call it inside your override of <see cref="ControlGUI"/>, possibly at the start of it.
        /// </remarks>
        protected void SetupEnabled(MaterialEditor materialEditor)
        {
            //bool previousShow = Show;
            if (useDictionary)
            {
                if (!firstCycle) return;
                
                if (dictionaryKey == null)
                    dictionaryKey = ControlAlias + ((Material)materialEditor.target).GetInstanceID();
                
                if (StaticDictionaries.BoolDictionary.TryGetValue(dictionaryKey, out bool show))
                {
                    Show = show;
                }
                else
                {
                    Show = false;
                    StaticDictionaries.BoolDictionary[dictionaryKey] = Show;
                }
                firstCycle = false;
            }
            else
            {
                Show = Math.Abs(Property.floatValue - showValue) < 0.001;
            }
        }

        /// <summary>
        /// Updates the source of the Enabled boolean base on the settings of this control.
        /// Should only be used inside <see cref="ControlGUI"/>.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        /// <remarks>
        /// This method is available for all section controls inheriting this class and will automatically update the current value of the <see cref="Show"/> boolean based on if
        /// the section folding state is handled by a material property or a dictionary value. You should call it inside your override of <see cref="ControlGUI"/>, possibly at the end of it.
        /// </remarks>
        protected void UpdateEnabled(MaterialEditor materialEditor)
        {
            if (useDictionary)
            {
                StaticDictionaries.BoolDictionary[dictionaryKey] = Show;
            }
            else
            {
                if (IsPropertyAnimatable)
                {
                    materialEditor.RegisterPropertyChangeUndo(Property.displayName);
                    Property.floatValue = Show ? showValue : hideValue;
                }
                else
                {
                    NonAnimatablePropertyChanged = true;
                }
            }
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            SetupEnabled(materialEditor);
            EditorGUILayout.Space();

            // Save previous color and set header color.
            GUI.backgroundColor = BackgroundColor;
            // Begin header
            EditorGUILayout.BeginVertical(BackgroundStyle);
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;
            Rect r = EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            if (ShowFoldoutArrow)
                Show = EditorGUILayout.Toggle(Show, EditorStyles.foldout, GUILayout.MaxWidth(15.0f));
            
            EditorGUILayout.LabelField(Content, LabelStyle);
            //DrawUpDownButtons();
            //isEnabled = EditorGUILayout.Toggle(isEnabled, TSConstants.Styles.deleteStyle, GUILayout.MaxWidth(15.0f));

            Show = GUI.Toggle(r, Show, GUIContent.none, new GUIStyle());
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
                UpdateEnabled(materialEditor);

            EditorGUILayout.EndHorizontal();

            if (!AreControlsInHeader)
                EditorGUILayout.EndVertical();
            
            if (Show)
                DrawControls(materialEditor);
            
            if (AreControlsInHeader)
                EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws the list of controls that can be hidden by this control.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected void DrawControls(MaterialEditor materialEditor)
        {
            //EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            foreach (var control in Controls)
                control.DrawControl(materialEditor);
            
            EditorGUILayout.Space();
            //EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Updates properties that should not be animated, implementation needed by <see cref="INonAnimatableProperty"/>
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        public virtual void UpdateNonAnimatableProperty(MaterialEditor materialEditor)
        {
            if (useDictionary || !HasPropertyUpdated) return;
            materialEditor.RegisterPropertyChangeUndo(Property.displayName);
            Property.floatValue = Show ? showValue : hideValue;
        }
        
        public void AddControl(SimpleControl control) => Controls.Add(control);

        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}