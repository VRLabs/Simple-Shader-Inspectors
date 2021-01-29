using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Represents a general purpose control for a property that will be drawn based on the property type.
    /// </summary>
    public class PropertyControl : SimpleControl
    {
        /// <summary>
        /// Integer containing the index of the property of this control.
        /// </summary>
        private int _propertyIndex = -2;

        /// <summary>
        /// Name of the property shown by this control.
        /// </summary>
        public string PropertyName { get; protected set; }
        /// <summary>
        /// MaterialProperty related to the property shown by this control.
        /// </summary>
        public MaterialProperty Property { get; protected set; }

        /// <summary>
        /// Boolean indicating if this control updated its property value
        /// </summary>
        public bool HasPropertyUpdated { get; protected set; }

        /// <summary>
        /// Default constructor of <see cref="PropertyControl"/>.
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <returns>A new <see cref="PropertyControl"/> object.</returns>
        public PropertyControl(string propertyName) : base(propertyName)
        {
            PropertyName = propertyName;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            materialEditor.ShaderProperty(Property, Content);
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
        }

        internal void SetPropertyIndex(MaterialProperty[] properties)
        {
            _propertyIndex = SSIHelper.FindPropertyIndex(PropertyName, properties);
        }

        internal void FetchProperty(MaterialProperty[] properties)
        {
            if(_propertyIndex == -2)
                SetPropertyIndex(properties);

            if (_propertyIndex != -1)
                Property = properties[_propertyIndex];
            else
                Property = null;
        }
    }
    public static partial class BaseControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="PropertyControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <param name="propertyName">Material property name.</param>
        /// <returns>The <see cref="PropertyControl"/> object that has been added.</returns>
        public static PropertyControl AddPropertyControl(this IControlContainer container, string propertyName)
        {
            PropertyControl control = new PropertyControl(propertyName);
            container.Controls.Add(control);
            return control;
        }
    }
}