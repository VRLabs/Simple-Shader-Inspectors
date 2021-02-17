using System.Collections.Generic;
using UnityEditor;
using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface that defines an object that contains a list of SimpleControls of a specified type to draw.
    /// </summary>
    public interface IControlContainer<T> : IControlContainer where T : SimpleControl
    {
        void AddControl(T control);
        
        new IEnumerable<T> GetControlList();
    }

    /// <summary>
    /// Interface that defines an object that contains a list of SimpleControls of type SimpleControl to draw.
    /// </summary>
    public interface IControlContainer
    {
        void AddControl(SimpleControl control);

        IEnumerable<SimpleControl> GetControlList();
    }
}
