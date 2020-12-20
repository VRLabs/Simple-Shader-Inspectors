using System.Collections.Generic;
using UnityEditor;
using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface that defines an object that contains a list of SimpleControls to draw.
    /// </summary>
    public interface IControlContainer
    {
        /// <summary>
        /// List of controls the class implementing this interface will have.
        /// </summary>
        List<SimpleControl> Controls { get; set; }
    }
}
