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
        /// Style of the header label.
        /// </summary>
        public GUIStyle LabelStyle { get; set; }

        /// <summary>
        /// Style of the header background.
        /// </summary>
        public GUIStyle BackgroundStyle { get; set; }

        /// <summary>
        /// Boolean indicating if child controls will be inside the header background.
        /// </summary>
        public bool AreControlsInside { get; set; }

        /// <summary>
        /// Boolean indicating if the folding state material property needs to be updated
        /// </summary>
        public bool NonAnimatablePropertyChanged { get; set; }

        /// <summary>
        /// Boolean indicating if the folding state material property is animatable or not.
        /// </summary>
        /// <value></value>
        public bool IsPropertyAnimatable { get; set; }

        /// <summary>
        /// Boolean indicating if the foldout arrow is enabled or not.
        /// </summary>
        /// <value></value>
        public bool ShowFoldoutArrow { get; set; }

        /// <summary>
        /// Bool indicating if the section is folded out or not.
        /// </summary>
        public bool Show { get; protected set; }

        /// <summary>
        /// Background color of the header.
        /// </summary>
        public Color BackgroundColor { get; set; }
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
            AreControlsInside = false;
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

            if (!AreControlsInside)
            {
                EditorGUILayout.EndVertical();
            }
            if (Show)
            {
                DrawControls(materialEditor);
            }
            if (AreControlsInside)
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
    }
    public static partial class SectionControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="Section"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="alias">Alias of the section.</param>
        /// <returns>The <see cref="Section"/> object that has been added.</returns>
        public static Section AddSection(this IControlContainer container, string alias)
        {
            Section control = new Section().Alias(alias);
            container.Controls.Add(control);
            return control;
        }

        /// <summary>
        /// Creates a new control of type <see cref="Section"/> that is driven by a property and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property that will drive the section open state</param>
        /// <param name="hideValue">Float value that the material property will have if the section is collapsed, optional (default: 0).</param>
        /// <param name="showValue">Float value that the material property will have if the section is visible, optional (default: 1).</param>
        /// <returns>The <see cref="Section"/> object that has been added.</returns>
        public static Section AddPDSection(this IControlContainer container, string propertyName, float hideValue = 0, float showValue = 1)
        {
            Section control = new Section(propertyName, hideValue, showValue);
            container.Controls.Add(control);
            return control;
        }

        /// <summary>
        /// Set the background color.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The modified section.</returns>
        public static T SetBackgroundColor<T>(this T section, Color backgroundColor) where T : Section
        {
            section.BackgroundColor = backgroundColor;
            return section;
        }

        /// <summary>
        /// Set the style of the label.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="labelStyle">Style of the label.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The modified section.</returns>
        public static T SetLabelStyle<T>(this T section, GUIStyle labelStyle) where T : Section
        {
            section.LabelStyle = labelStyle;
            return section;
        }

        /// <summary>
        /// Set the style of the background of the section.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="backgroundStyle">Style of the background.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The modified section.</returns>
        public static T SetBackGroundStyle<T>(this T section, GUIStyle backgroundStyle) where T : Section
        {
            section.BackgroundStyle = backgroundStyle;
            return section;
        }

        /// <summary>
        /// Set if the background of the header will be extended to the child controls.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="areControlsInside"></param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The modified section.</returns>
        public static T IncludeControlsInHeader<T>(this T section, bool areControlsInside) where T : Section
        {
            section.AreControlsInside = areControlsInside;
            return section;
        }

        /// <summary>
        /// Set if the property driving the fold in and out of the section can be animated by the animation window.
        /// Does nothing if the section uses a dictionary for driving it's folding state.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="isPropertyAnimatable">If the property is animatable.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The modified section.</returns>
        public static T SetPropertyAnimatable<T>(this T section, bool isPropertyAnimatable) where T : Section
        {
            section.IsPropertyAnimatable = isPropertyAnimatable;
            return section;
        }

        /// <summary>
        /// Set if the foldout arrow is visible.
        /// </summary>
        /// <param name="section">Section this method extends to.</param>
        /// <param name="showFoldoutArrow">If the foldout arrow is visible.</param>
        /// <typeparam name="T">The type of this class.</typeparam>
        /// <returns>The modified section.</returns>
        public static T ShowFoldoutArrow<T>(this T section, bool showFoldoutArrow) where T : Section
        {
            section.ShowFoldoutArrow = showFoldoutArrow;
            return section;
        }
    }
}