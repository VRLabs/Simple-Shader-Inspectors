using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface used to define the need for additional material properties to handle.
    /// </summary>
    public interface IAdditionalProperties
    {
        /// <summary>
        /// Array containing all additional properties the control needs.
        /// </summary>
        AdditionalProperty[] AdditionalProperties { get; set; }
    }

    /// <summary>
    /// This class defines an additional property.
    /// </summary>
    public class AdditionalProperty
    {
        /// <summary>
        /// String containing the additional property name.
        /// </summary>
        public string PropertyName { get; }

        /// <summary>
        /// Array containing the index needed to fetch.
        /// </summary>
        private int _propertyIndex;

        /// <summary>
        /// MaterialProperty containing the additional property needed by the control.
        /// </summary>
        public MaterialProperty Property { get; private set; }

        /// <summary>
        /// Default constructor of AdditionalProperty.
        /// </summary>
        /// <param name="propertyName">Name of the materialProperty to fetch.</param>
        public AdditionalProperty(string propertyName)
        {
            PropertyName = propertyName;
        }

        // Fetch and store the index of the needed material property.
        internal void SetPropertyIndex(MaterialProperty[] properties)
        {
            _propertyIndex = SimpleShaderInspector.FindPropertyIndex(PropertyName, properties);
        }

        // Fetch the material property based on the index.
        internal void FetchProperty(MaterialProperty[] properties)
        {
            if (_propertyIndex != -1)
            {
                Property = properties[_propertyIndex];
            }
            else
            {
                Property = null;
            }
        }
    }
}
