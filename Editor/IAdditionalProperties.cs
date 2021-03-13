using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface used to define the need for additional material properties to handle.
    /// </summary>
    /// <remarks>
    /// When a control needs more than a single property to do its job, implementing this interface will let the inspector know this need.
    /// </remarks>
    public interface IAdditionalProperties
    {
        /// <summary>
        /// Array containing all additional properties the control needs.
        /// </summary>
        /// <remarks>
        /// Is up to the control to initialize the array and assign the property names it needs to fetch.
        /// </remarks>
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
        private int _propertyIndex = -2;

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
            _propertyIndex = SSIHelper.FindPropertyIndex(PropertyName, properties);
        }

        // Fetch the material property based on the index.
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
