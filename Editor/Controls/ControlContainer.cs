using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a group of conrols.
    /// </summary>
    public class ControlContainer : SimpleControl, IControlContainer
    {
        /// <summary>
        /// List of controls under this control.
        /// </summary>
        public List<SimpleControl> Controls { get; set; }

        /// <summary>
        /// Default constructor of <see cref="ControlContainer"/>.
        /// </summary>
        /// <returns>A new <see cref="ControlContainer"/> object.</returns>
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
            foreach (SimpleControl control in Controls)
            {
                control.DrawControl(materialEditor);
            }
        }
    }
    public static partial class ControlExtensions
    {
        /// <summary>
        /// Creates a new control of type <see cref="PropertyControl"/> and adds it to the current container.
        /// </summary>
        /// <param name="container">Container of controls this method extends to.</param>
        /// <returns>The <see cref="ControlContainer"/> object that has been added.</returns>
        public static ControlContainer AddControlContainer(this IControlContainer container)
        {
            ControlContainer control = new ControlContainer();
            container.Controls.Add(control);
            return control;
        }
    }
}