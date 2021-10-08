using System.Collections.Generic;
using UnityEditor;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a group of controls.
    /// </summary>
    /// <remarks>
    /// <para>This control has no UI for itself, instead it just displays all controls it has inside itself.</para>
    /// <para>It can be useful whenever you need to enable, disable or, in general, manage an entire group of controls at once.</para>
    /// <para>Since it has no UI, it does not need an alias, and setting one would not do much.</para>
    /// </remarks>
    /// <example>
    /// Example Usage:
    /// <code>
    /// // Create control
    /// ControlContainer control = this.AddControlContainer();
    ///
    /// // Add controls inside of it
    /// control.AddPropertyControl("_ExampleProperty");
    /// control.AddColorControl("_ExampleColor");
    /// </code>
    /// </example>
    public class ControlContainer : SimpleControl, IControlContainer
    {
        /// <summary>
        /// List of controls under this control.
        /// </summary>
        /// <value>
        /// All controls that have been added by extension methods.
        /// </value>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// Default constructor of <see cref="ControlContainer"/>.
        /// </summary>
        /// <returns>A new <see cref="ControlContainer"/> object.</returns>
        /// <remarks>
        /// Since this control does not need an alias, no alias strings are needed, and the alias will be set as "".
        /// </remarks>
        public ControlContainer() : base("")
        {
            Controls = new List<SimpleControl>();
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            foreach (var control in Controls)
                control.DrawControl(materialEditor);
        }
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to add controls. All controls added are stored in <see cref="Controls"/>
        /// </summary>
        /// <param name="control">Control to add.</param>
        public void AddControl(SimpleControl control)
        {
            Controls.Add(control);
        }
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to get the object's controls list.
        /// </summary>
        /// <returns><see cref="Controls"/></returns>
        public IEnumerable<SimpleControl> GetControlList()
        {
            return Controls;
        }
    }
}