using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a group of controls that is visible only on when a property has a specific float value.
    /// </summary>
    /// <remarks>
    /// <para>This control has no UI for itself, instead it just displays all controls it has inside itself.</para>
    /// <para>It can be useful whenever you need to enable, disable or, in general, manage an entire group of controls at once.</para>
    /// <para>Unlike <see cref="ControlContainer"/> it has embedded logic to automatically display or hide content based on the given property float value</para>
    /// </remarks>
    /// <example>
    /// Example Usage:
    /// <code>
    /// // Create control
    /// ConditionalControlContainer control = this.AddConditionalControlContainer();
    ///
    /// // Add controls inside of it
    /// control.AddPropertyControl("_ExampleProperty");
    /// control.AddColorControl("_ExampleColor");
    /// </code>
    /// </example>
    public class ConditionalControlContainer : PropertyControl, IControlContainer
    {
        /// <summary>
        /// List of controls under this control.
        /// </summary>
        /// <value>
        /// All controls that have been added by extension methods.
        /// </value>
        public List<SimpleControl> Controls { get; set; }
        
        /// <summary>
        /// If the controls inside get indented or not.
        /// </summary>
        /// <value>
        /// True if the controls inside will get indented, false otherwise.
        /// </value>
        [Chainable]
        public bool Indent { get; set; }
        
        /// <summary>
        /// Which value enables the control container
        /// </summary>
        /// <value>
        /// Float value of the property if the toggle is enabled
        /// </value>
        protected readonly float EnableValue;

        /// <summary>
        /// Default constructor of <see cref="ConditionalControlContainer"/>.
        /// </summary>
        /// <returns>A new <see cref="ConditionalControlContainer"/> object.</returns>
        public ConditionalControlContainer(string conditionalProperty, float enableValue) : base(conditionalProperty)
        {
            EnableValue = enableValue;
            Controls = new List<SimpleControl>();
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            if (!(Math.Abs(Property.floatValue - EnableValue) < 0.001)) return;
            if(Indent) EditorGUI.indentLevel++;
            foreach (var control in Controls)
                control.DrawControl(materialEditor);
            if (Indent) EditorGUI.indentLevel--;
        }
        
        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to add controls. All controls added are stored in <see cref="Controls"/>
        /// </summary>
        /// <param name="control">Control to add.</param>
        /// <param name="alias">Optional alias to say where a control is appended after.</param>
        public void AddControl(SimpleControl control, string alias = "") => Controls.AddControl(control, alias);

        /// <summary>
        /// Implementation needed by <see cref="IControlContainer"/> to get the object's controls list.
        /// </summary>
        /// <returns><see cref="Controls"/></returns>
        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}