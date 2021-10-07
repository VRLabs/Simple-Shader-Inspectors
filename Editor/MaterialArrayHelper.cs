using System.Collections.Generic;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Helper class for handling Material arrays.
    /// </summary>
    /// <remarks>
    /// Managing multiple materials inside the inspector could become slightly annoying, these extension functions will make this process less annoying by automatically
    /// looping the materials array to and do the operations you want
    /// </remarks>
    public static class MaterialArrayHelper
    {
        /// <summary>
        /// Sets a keyword state to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="keyword">The keyword that is being toggled.</param>
        /// <param name="state">Toggle value.</param>
        public static void SetKeyword(this IEnumerable<Material> materials, string keyword, bool state)
        {
            foreach (var m in materials)
            {
                if (state)
                    m.EnableKeyword(keyword);
                else
                    m.DisableKeyword(keyword);
            }
        }

        /// <summary>
        /// Gets the mixed value state of a keyword on the materials array
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="keyword">The keyword to check against.</param>
        /// <returns>True if the keyword has mixed values, false otherwise.</returns>
        public static bool IsKeywordMixedValue(this Material[] materials, string keyword)
        {
            bool reference = materials[0].IsKeywordEnabled(keyword);
            foreach (var m in materials)
                if (m.IsKeywordEnabled(keyword) != reference)
                    return true;
            return false;
        }

        /// <summary>
        /// Set override tag to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="value">Value of the tag.</param>
        public static void SetOverrideTag(this IEnumerable<Material> materials, string tagName, string value)
        {
            foreach (var m in materials)
                m.SetOverrideTag(tagName, value);
        }

        /// <summary>
        /// Set int to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the int.</param>
        /// <param name="value">Value of the int.</param>
        public static void SetInt(this IEnumerable<Material> materials, string name, int value)
        {
            foreach (var m in materials)
                m.SetInt(name, value);
        }
        
        /// <summary>
        /// Set float to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the float.</param>
        /// <param name="value">Value of the float.</param>
        public static void SetFloat(this IEnumerable<Material> materials, string name, float value)
        {
            foreach (var m in materials)
                m.SetFloat(name, value);
        }

        /// <summary>
        /// Set vector to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the Vector4.</param>
        /// <param name="value">Value of the Vector4.</param>
        public static void SetVector(this IEnumerable<Material> materials, string name, Vector4 value)
        {
            foreach (var m in materials)
                m.SetVector(name, value);
        }
        
        /// <summary>
        /// Set color to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the Color.</param>
        /// <param name="value">Value of the Color.</param>
        public static void SetColor(this IEnumerable<Material> materials, string name, Color value)
        {
            foreach (var m in materials)
                m.SetColor(name, value);
        }
        
        /// <summary>
        /// Set texture to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="name">Name of the texture.</param>
        /// <param name="value">Value of the texture.</param>
        public static void SetTexture(this IEnumerable<Material> materials, string name, Texture value)
        {
            foreach (var m in materials)
                m.SetTexture(name, value);
        }

        /// <summary>
        /// Set render queue to all materials in the array.
        /// </summary>
        /// <param name="materials">Material array this method extends to.</param>
        /// <param name="queue">Render queue value.</param>
        public static void SetRenderQueue(this IEnumerable<Material> materials, int queue)
        {
            foreach (var m in materials)
                m.renderQueue = queue;
        }
    }
}