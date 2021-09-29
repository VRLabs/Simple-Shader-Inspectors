using System.Collections.Generic;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface that defines an object that contains a list of SimpleControls of a specified type to draw.
    /// </summary>
    /// <remarks>
    /// This generic version of <see cref="IControlContainer"/> makes possible to define a stricter set of control types to be contained. 
    /// </remarks>
    public interface IControlContainer<T> : IControlContainer where T : SimpleControl
    {
        /// <summary>
        /// Generic version of <see cref="IControlContainer.AddControl"/>.
        /// </summary>
        /// <param name="control">Control to add.</param>
        void AddControl(T control);
        
        /// <summary>
        /// Generic version of <see cref="IControlContainer.GetControlList"/>.
        /// </summary>
        /// <returns>An IEnumerable of the controls stored in this object.</returns>
        new IEnumerable<T> GetControlList();
    }

    /// <summary>
    /// Interface that defines an object that contains a list of SimpleControls of type SimpleControl to draw.
    /// </summary>
    public interface IControlContainer
    {
        /// <summary>
        /// Method used to add a control to the list of controls that are under this object
        /// </summary>
        /// <param name="control">Control to add</param>
        /// <remarks>
        /// <para>Simple Shader Inspectors needs an endpoint to add controls inside <see cref="IControlContainer"/> objects using Chainable methods, and this is the one it uses.</para>
        /// <para>This approach is preferable compared to defining a list because it will leave the control the freedom to handle how to store ad manage controls passed by the inspector.</para>
        /// </remarks>
        void AddControl(SimpleControl control);
        
        /// <summary>
        /// Method used to get the list of stored controls.
        /// </summary>
        /// <returns>An IEnumerable of the controls stored in this object.</returns>
        /// <remarks>
        /// Just like <see cref="AddControl"/> this is used by Simple Shader Inspector to have a way to retrieve controls stored inside this object, to then do operations with it
        /// (for example, fetching the current localization content needed by the controls).
        /// </remarks>
        IEnumerable<SimpleControl> GetControlList();
    }
}
