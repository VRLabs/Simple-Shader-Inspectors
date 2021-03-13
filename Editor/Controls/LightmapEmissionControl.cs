using UnityEditor;
using UnityEngine;
namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Control that handles the lightmap Emission Property of the material.
    /// </summary>
    /// <remarks>
    /// <para>Is the Simple Shader Inspectors equivalent of <c>MaterialEditor.LightmapEmissionProperty</c>.</para>
    /// <para>It does not need an alias in order to work.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddLightmapEmissionControl();
    /// </code>
    /// </example>
    public class LightmapEmissionControl : SimpleControl
    {
        /// <summary>
        /// Default constructor of <see cref="LightmapEmissionControl"/>.
        /// </summary>
        public LightmapEmissionControl() : base("") { }

        /// <summary>
        /// Boolean indicating if the lightmap emission property has updated.
        /// </summary>
        /// <value>
        /// True if the lightmap emission value has been updated, false otherwise.
        /// </value>
        public bool HasLightmapEmissionUpdated { get; protected set; }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUI.BeginChangeCheck();
            materialEditor.LightmapEmissionProperty();
            HasLightmapEmissionUpdated = EditorGUI.EndChangeCheck();
            if (HasLightmapEmissionUpdated)
            {
                foreach (Material mat in materialEditor.targets)
                {
                    MaterialEditor.FixupEmissiveFlag(mat);
                    bool shouldEmissionBeEnabled = (mat.globalIlluminationFlags & MaterialGlobalIlluminationFlags.EmissiveIsBlack) == 0;
                    mat.SetOverrideTag("IsEmissive", shouldEmissionBeEnabled ? "true" : "false");
                }
            }
        }
    }
}