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

        public void AddControl(SimpleControl control)
        {
            Controls.Add(control);
        }

        public IEnumerable<SimpleControl> GetControlList()
        {
            return Controls;
        }
    }
}