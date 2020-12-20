using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for setting a float property to 2 defined values.
    /// </summary>
    public class KeywordToggleControl : SimpleControl
    {
        /// <summary>
        /// Boolean indicating if the toggle is enabled or not.
        /// </summary>
        public bool ToggleEnabled { get; protected set; }

        /// <summary>
        /// Boolean indicating if the keyword state has updated.
        /// </summary>
        public bool HasKeywordUpdated { get; protected set; }

        protected readonly string keyword;

        private Material[] _materials;

        /// <summary>
        /// Default constructor of <see cref="KeywordToggleControl"/>
        /// </summary>
        /// <param name="keyword">Name of the keyword to toggle.</param>
        /// <returns>A new <see cref="KeywordToggleControl"/> object.</returns>
        public KeywordToggleControl(string keyword) : base(keyword)
        {
            this.keyword = keyword;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            if (_materials == null)
            {
                _materials = Array.ConvertAll(materialEditor.targets, item => (Material)item);
            }
            EditorGUI.showMixedValue = _materials.IsKeywordMixedValue(keyword);
            ToggleEnabled = _materials[0].IsKeywordEnabled(keyword);

            EditorGUI.BeginChangeCheck();
            ToggleEnabled = EditorGUILayout.Toggle(Content, ToggleEnabled);
            HasKeywordUpdated = EditorGUI.EndChangeCheck();
            if (HasKeywordUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(keyword);
                _materials.SetKeyword(keyword, ToggleEnabled);
            }

            EditorGUI.showMixedValue = false;
        }
    }
    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="KeywordToggleControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="keyword">Keyword name.</param>
        /// <returns>The <see cref="KeywordToggleControl"/> object that has been added.</returns>
        public static KeywordToggleControl AddKeywordToggleControl(this IControlContainer container, string keyword)
        {
            KeywordToggleControl control = new KeywordToggleControl(keyword);
            container.Controls.Add(control);
            return control;
        }
    }
}