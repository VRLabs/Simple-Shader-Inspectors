using UnityEditor;
using UnityEngine;
namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Control that handles THe lightmap Emission Property of the material.
    /// </summary>
    public class LightmapEmissionControl : SimpleControl
    {
        /// <summary>
        /// Default constructor of <see cref="LightmapEmissionControl"/>.
        /// </summary>
        public LightmapEmissionControl() : base("") { }

        /// <summary>
        /// Boolean indicating if the lightmap emission property has updated.
        /// </summary>
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