using System;
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
        /// GuiContent set by the inspector.
        /// </summary>
        public GUIContent Content { get; set; }

        /// <summary>
        /// Name used for localization.
        /// </summary>
        public string ControlAlias { get; set; }

        /// <summary>
        /// Boolean that defines if the control is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Boolean that defines if the control is enabled.
        /// </summary>
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
        /// Content of tha control that is drawn.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected abstract void ControlGUI(MaterialEditor materialEditor);

        /// <summary>
        /// Content of tha control that is drawn.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        public void DrawControl(MaterialEditor materialEditor)
        {
            if (IsVisible)
            {
                // We could directly use EditorGUI.BeginDisabledGroup(!IsEnabled) instead of this check,
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
    }

    /// <summary>
    /// Static class containing all control extension methods in the base namespace of Simple Shader Inspectors.
    /// </summary>
    public static partial class BaseControlExtensions
    {
        /// <summary>
        /// Set a custom alias for control localization.
        /// </summary>
        /// <param name="control">The control this extension method is used on.</param>
        /// <param name="alias">Name used for localization.</param>
        /// <typeparam name="T">Type of the control. Has to be child of SimpleControl.</typeparam>
        /// <returns>The control modified.</returns>
        public static T Alias<T>(this T control, string alias) where T : SimpleControl
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
        public static T SetVisibility<T>(this T control, bool visible) where T : SimpleControl
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
        public static T SetEnabled<T>(this T control, bool enabled) where T : SimpleControl
        {
            control.IsEnabled = enabled;
            return control;
        }
    }
}