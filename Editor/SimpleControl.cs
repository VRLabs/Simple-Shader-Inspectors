using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Base class of all controls used by Simple Shader Inspectors.
    /// </summary>
    public abstract class SimpleControl
    {
        /// <summary>
        /// Inspector that contains this control.
        /// </summary>
        public virtual ISimpleShaderInspector Inspector { get; set; }
        /// <summary>
        /// Localized GuiContent set by the inspector.
        /// </summary>
        /// <value>
        /// A GUIContent that is fetched by the inspector using the control's alias containing the localized string that will be displayed.
        /// </value>
        public GUIContent Content { get; set; }

        /// <summary>
        /// Name used for localization.
        /// </summary>
        /// <value>
        /// String containing the alias used for localization.
        /// </value>
        /// <remarks>
        /// <para>By default the alias should be set by the constructor and modified with the chainable <c><see cref="Chainables.WithAlias{T}"/></c> method.</para>
        /// <para>
        /// Is up to the controls derived from this class to decide what should be used as default value, just keep in mind that using hardcoded strings means that all
        /// instances of that class would share the same localization string unless <c><see cref="Chainables.WithAlias{T}"/></c> is used in the inspector.
        /// </para>
        /// <para>
        /// Controls using a <c>MaterialProperty</c> should use the material property name as alias (which should be done by default if you inherit from <see cref="PropertyControl"/>
        /// and pass the property name to the base constructor), meanwhile controls that do not use a <c>MaterialProperty</c> should include a string in the constructor to pass as alias.
        /// </para>
        /// </remarks>
        public string ControlAlias { get; set; }

        /// <summary>
        /// Boolean that defines if the control is visible.
        /// </summary>
        /// <value>
        /// True if the control should be visible, false otherwise (default: true).
        /// </value>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Boolean that defines if the control is enabled.
        /// </summary>
        /// <value>True if the control should be enabled, false otherwise (default: true).</value>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Default constructor of <see cref="SimpleControl"/>
        /// </summary>
        /// <param name="alias">Material property name.</param>
        protected SimpleControl(string alias)
        {
            ControlAlias = alias;
            IsEnabled = true;
            IsVisible = true;
        }

        /// <summary>
        /// Content of the control that is drawn.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected abstract void ControlGUI(MaterialEditor materialEditor);

        /// <summary>
        /// Content of the control that is drawn.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        public void DrawControl(MaterialEditor materialEditor)
        {
            if (!IsVisible) return;
            // We could directly use EditorGUI.BeginDisabledGroup(!IsEnabled) instead of doing this check,
            // but i don't trust adding too many groups when not necessary
            if (!IsEnabled)
            {
                EditorGUI.BeginDisabledGroup(true);
                ControlGUI(materialEditor);
                EditorGUI.EndDisabledGroup();
            }
            else
            {
                ControlGUI(materialEditor);
            }
        }
    }

    /// <summary>
    /// Static class containing all control extension methods in the base namespace of Simple Shader Inspectors.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Set a custom alias for control localization.
        /// </summary>
        /// <param name="control">The control this extension method is used on.</param>
        /// <param name="alias">Name used for localization.</param>
        /// <typeparam name="T">Type of the control. Has to be child of SimpleControl.</typeparam>
        /// <returns>The control modified.</returns>
        public static T WithAlias<T>(this T control, string alias) where T : SimpleControl
        {
            control.ControlAlias = alias;
            return control;
        }

        /// <summary>
        /// Set if the control is visible.
        /// </summary>
        /// <param name="control">The control this extension method is used on.</param>
        /// <param name="visible">If the control is visible.</param>
        /// <typeparam name="T">Type of the control. Has to be child of SimpleControl.</typeparam>
        /// <returns>The control modified.</returns>
        public static T WithVisibility<T>(this T control, bool visible) where T : SimpleControl
        {
            control.IsVisible = visible;
            return control;
        }

        /// <summary>
        /// Set if the control is enabled.
        /// </summary>
        /// <param name="control">The control this extension method is used on.</param>
        /// <param name="enabled">If the control is enabled.</param>
        /// <typeparam name="T">Type of the control. Has to be child of SimpleControl.</typeparam>
        /// <returns>The control modified.</returns>
        public static T WithEnabled<T>(this T control, bool enabled) where T : SimpleControl
        {
            control.IsEnabled = enabled;
            return control;
        }
    }
}