using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Represents a general purpose control for a property that will be drawn based on the property type.
    /// </summary>
    /// <remarks>
    /// You can see this control as the Simple Shader Inspectors equivalent of <c>MaterialEditor.ShaderProperty</c> since it draws all properties in a generic way.
    /// </remarks>
    /// <example>
    /// Initialize the control inside the inspector:
    /// <code>
    /// this.AddPropertyControl("_ExampleProperty");
    /// </code>
    /// </example>
    /// <seealso cref="SimpleControl"/>
    public class PropertyControl : SimpleControl
    {
        /// <summary>
        /// Integer containing the index of the property of this control.
        /// </summary>
        private int _propertyIndex = -2;

        /// <summary>
        /// Name of the property shown by this control.
        /// </summary>
        /// <value>
        /// string containing the name of the property.
        /// </value>
        public string PropertyName { get; protected set; }
        /// <summary>
        /// MaterialProperty related to the property shown by this control.
        /// </summary>
        /// <value>
        /// MaterialProperty fetched by the inspector based on <see cref="PropertyName"/>.
        /// </value>
        public MaterialProperty Property { get; protected set; }

        /// <summary>
        /// Boolean indicating if this control updated its property value.
        /// </summary>
        /// <value>True if the property value has been updated, false otherwise.</value>
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
            if (_propertyIndex == -2)
                SetPropertyIndex(properties);

            if (_propertyIndex != -1)
                Property = properties[_propertyIndex];
            else
                Property = null;
        }
    }
}