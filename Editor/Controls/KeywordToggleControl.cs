using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with a checkbox for toggling a keyword on and off.
    /// </summary>
    /// <remarks>
    /// With this control, by passing the keyword name you can have a toggle that will enable and disable the keyword on the material.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddKeywordToggleControl("KEYWORD_TO_TOGGLE");
    /// </code>
    ///
    /// By default the keyword name will be used as Alias.
    /// </example>
    public class KeywordToggleControl : SimpleControl
    {
        /// <summary>
        /// Boolean indicating if the toggle is enabled or not.
        /// </summary>
        /// <value>
        /// True if the toggle is enabled, false otherwise.
        /// </value>
        public bool ToggleEnabled { get; protected set; }

        /// <summary>
        /// Boolean indicating if the keyword state has updated.
        /// </summary>
        /// <value>
        /// True if the keyword value has been updated, false otherwise.
        /// </value>
        public bool HasKeywordUpdated { get; protected set; }
        
        /// <summary>
        /// Keyword that this toggle sets on and off, once set by the constructor it cannot be changed.
        /// </summary>
        /// <value>
        /// String containing the keyword.
        /// </value>
        protected readonly string Keyword;

        private Material[] _materials;

        /// <summary>
        /// Default constructor of <see cref="KeywordToggleControl"/>
        /// </summary>
        /// <param name="keyword">Name of the keyword to toggle.</param>
        /// <returns>A new <see cref="KeywordToggleControl"/> object.</returns>
        public KeywordToggleControl(string keyword) : base(keyword)
        {
            this.Keyword = keyword;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            if (_materials == null)
                _materials = Array.ConvertAll(materialEditor.targets, item => (Material)item);
            
            EditorGUI.showMixedValue = _materials.IsKeywordMixedValue(Keyword);
            ToggleEnabled = _materials[0].IsKeywordEnabled(Keyword);

            EditorGUI.BeginChangeCheck();
            ToggleEnabled = EditorGUILayout.Toggle(Content, ToggleEnabled);
            HasKeywordUpdated = EditorGUI.EndChangeCheck();
            if (HasKeywordUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(Keyword);
                _materials.SetKeyword(Keyword, ToggleEnabled);
            }

            EditorGUI.showMixedValue = false;
        }
    }
}