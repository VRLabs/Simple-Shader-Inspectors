using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Represents a grouping of controls with an header that can be folded in and out.
    /// </summary>
    public class Section : PropertyControl, IControlContainer, INonAnimatableProperty
    {
        /// <summary>
        /// Float value that the Show bool gets converted if true.
        /// </summary>
        protected readonly float showValue;

        /// <summary>
        /// Float value that the Show bool gets converted if false.
        /// </summary>
        protected readonly float hideValue;

        /// <summary>
        /// Boolean indicating if the section folding state is driven by an internal dictionary or not. It will be true in case you don't use a material property.
        /// </summary>
        protected readonly bool useDictionary;

        /// <summary>
        /// String containing the key value that the section will use for the dictionary.
        /// </summary>
        protected string dictionaryKey = null;

        /// <summary>
        /// Boolean indicating if it's the first ui update.
        /// </summary>
        protected bool firstCycle = true;

        /// <summary>
        /// List of controls inside this section.
        /// </summary>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// Boolean indicating if the folding state material property needs to be updated
        /// </summary>
        public bool NonAnimatablePropertyChanged { get; set; }

        /// <summary>
        /// Bool indicating if the section is folded out or not.
        /// </summary>
        public bool Show { get; protected set; }

        /// <summary>
        /// Style of the header label.
        /// </summary>
        [Chainable] public GUIStyle LabelStyle { get; set; }

        /// <summary>
        /// Style of the header background.
        /// </summary>
        [Chainable] public GUIStyle BackgroundStyle { get; set; }

        /// <summary>
        /// Boolean indicating if child controls will be inside the header background.
        /// </summary>
        [Chainable] public bool AreControlsInHeader { get; set; }

        /// <summary>
        /// Boolean indicating if the folding state material property is animatable or not.
        /// </summary>
        /// <value></value>
        [Chainable] public bool IsPropertyAnimatable { get; set; }

        /// <summary>
        /// Boolean indicating if the foldout arrow is enabled or not.
        /// </summary>
        /// <value></value>
        [Chainable] public bool ShowFoldoutArrow { get; set; }

        /// <summary>
        /// Background color of the header.
        /// </summary>
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
        /// Should only be used inside <see cref="DrawControl"/>.
        /// </summary>
        protected void SetupEnabled(MaterialEditor materialEditor)
        {
            //bool previousShow = Show;
            if (useDictionary)
            {
                if (firstCycle)
                {
                    if (dictionaryKey == null)
                    {
                        dictionaryKey = ControlAlias + ((Material)materialEditor.target).GetInstanceID();
                    }
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
            }
            else
            {
                Show = Math.Abs(Property.floatValue - showValue) < 0.001;
            }
        }

        /// <summary>
        /// Updates the source of the Enabled boolean base on the settings of this control.
        /// </summary>
        /// Should only be used inside <see cref="DrawControl"/>.
        /// <param name="materialEditor">Material editor.</param>
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
            {
                Show = EditorGUILayout.Toggle(Show, EditorStyles.foldout, GUILayout.MaxWidth(15.0f));
                //GUILayout.Space(38.0f);
            }
            EditorGUILayout.LabelField(Content, LabelStyle);
            //DrawUpDownButtons();
            //isEnabled = EditorGUILayout.Toggle(isEnabled, TSConstants.Styles.deleteStyle, GUILayout.MaxWidth(15.0f));

            Show = GUI.Toggle(r, Show, GUIContent.none, new GUIStyle());
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                UpdateEnabled(materialEditor);
            }

            EditorGUILayout.EndHorizontal();

            if (!AreControlsInHeader)
            {
                EditorGUILayout.EndVertical();
            }
            if (Show)
            {
                DrawControls(materialEditor);
            }
            if (AreControlsInHeader)
            {
                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// Draws the list of controls that can be hidden by this control.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected void DrawControls(MaterialEditor materialEditor)
        {
            //EditorGUI.indentLevel++;
            EditorGUILayout.Space();
            foreach (SimpleControl control in Controls)
            {
                control.DrawControl(materialEditor);
            }
            EditorGUILayout.Space();
            //EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Updates properties that should not be animated
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        public virtual void UpdateNonAnimatableProperty(MaterialEditor materialEditor)
        {
            if (!useDictionary && HasPropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(Property.displayName);
                Property.floatValue = Show ? showValue : hideValue;
            }
        }
        
        public void AddControl(SimpleControl control)
        {
            Controls.Add(control);
        }

        public IEnumerable<SimpleControl> GetControlList()
        {
            return Controls;
        }
    }
}