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
        /// <remarks>
        /// The optional alias string should be used to place the control after the control with said alias, and if the alias is null or empty it should be placed at the end.
        /// </remarks>
        /// <param name="control">Control to add.</param>
        /// <param name="alias">Alias to append after.</param>
        void AddControl(T control, string alias = "");
        
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
        /// <param name="alias">Alias to append after.</param>
        /// <remarks>
        /// <para>Simple Shader Inspectors needs an endpoint to add controls inside <see cref="IControlContainer"/> objects using Chainable methods, and this is the one it uses.</para>
        /// <para>This approach is preferable compared to defining a list because it will leave the control the freedom to handle how to store ad manage controls passed by the inspector.</para>
        /// <para>The optional alias string should be used to place the control after the control with said alias, and if the alias is null or empty it should be placed at the end.</para>
        /// </remarks>
        void AddControl(SimpleControl control, string alias = "");
        
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

    /// <summary>
    /// Extension methods for dealing with IControlContainer type of objects
    /// </summary>
    public static class ContainerExtensions
    {
        /// <summary>
        /// Default behaviour that should be used for adding new controls into a list of controls.
        /// </summary>
        /// <remarks>
        /// <para>If you're using an IList derivative list object to contain the list of controls in your <see cref="IControlContainer"/> object you can just use this method to add a new control.</para>
        /// <para>If you're dealing with controls differently you should implement your own method with the following sequence of actions:</para>
        /// <list type="bullet">
        /// <item>Check if the alias is valid by calling "string.IsNullOrWhiteSpace(alias)", in case it's not valid just append the new control at the end.</item>
        /// <item>Find the position of the control with the given alias and insert the new control after it.</item>
        /// <item>If the control with the given alias is not found, append the new control at the end.</item>
        /// </list>
        /// </remarks>
        /// <param name="items">List of items to add the new control to.</param>
        /// <param name="control">Control to add.</param>
        /// <param name="alias">alias to append after to, default value is an empty string.</param>
        public static void AddControl<T>(this IList<T> items, T control, string alias = "") where T : SimpleControl
        {
            if (string.IsNullOrWhiteSpace(alias))
            {
                items.Add(control);
                return;
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].ControlAlias != alias) continue;
                items.Insert(i+1, control);
                return;
            }
            
            items.Add(control);
        }
    }
    
}
