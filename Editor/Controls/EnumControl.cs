using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control with options based on an enum
    /// </summary>
    /// <remarks>
    /// With this control you can make selectors that are based on an enum, without the need to have an array of strings for options, since the enum name will be used instead.
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// //...
    /// public enum ExampleEnum
    /// {
    ///     Option1,
    ///     Option2,
    ///     Option3
    /// }
    /// //...
    /// // Inside the controls declarations
    /// this.AddEnumControl&lt;ExampleEnum&gt;("_ExampleProperty");
    /// </code>
    ///
    /// The value of the material property will be the index of the enum.
    /// </example>
    /// <typeparam name="TEnum">Type of enum used by the control</typeparam>
    public class EnumControl<TEnum> : PropertyControl where TEnum : Enum
    {
        private readonly GUIContent[] _options;
        
        /// <summary>
        /// Option currently selected.
        /// </summary>
        /// <value>
        /// Currently selected option of type <typeparamref name="TEnum"/>
        /// </value>
        public TEnum SelectedOption => (TEnum)Enum.ToObject(typeof(TEnum) , Property.floatValue);

        /// <summary>
        /// Default constructor of <see cref="EnumControl{TEnum}"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <returns>A new <see cref="EnumControl{TEnum}"/> object.</returns>
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
}