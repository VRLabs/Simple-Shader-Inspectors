using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with options based on an enum
    /// </summary>
    public class EnumControl<TEnum> : PropertyControl where TEnum : Enum
    {
        private readonly GUIContent[] _options;

        public TEnum SelectedOption => (TEnum)Enum.ToObject(typeof(TEnum) , Property.floatValue);

        /// <summary>
        /// Default constructor of <see cref="EnumControl<TEnum>"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <returns>A new <see cref="EnumControl<TEnum>"/> object.</returns>
        public EnumControl(string propertyName) : base(propertyName)
        {
            string[] op = Enum.GetNames(typeof(TEnum));
            _options = new GUIContent[op.Length];
            for (int i = 0; i < op.Length; i++)
            {
                _options[i] = new GUIContent(op[i]);
            }
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            int selected = (int)Property.floatValue;
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = Property.hasMixedValue;
            selected = EditorGUILayout.Popup(Content, selected, _options);
            EditorGUI.showMixedValue = false;
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(Content.text);
                Property.floatValue = selected;
            }
        }
    }
    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="EnumControl<TEnum>"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <returns>The <see cref="EnumControl<TEnum>"/> object that has been added.</returns>
        public static EnumControl<TEnum> AddEnumControl<TEnum>(this IControlContainer container, string propertyName) where TEnum : Enum
        {
            EnumControl<TEnum> control = new EnumControl<TEnum>(propertyName);
            container.Controls.Add(control);
            return control;
        }
    }
}