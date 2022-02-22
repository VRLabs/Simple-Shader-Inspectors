using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface indicating that the object is a valid Simple Shader Inspector and can use its features.
    /// </summary>
    /// <remarks>
    /// <para>By implementing this interface you can create your own inspector that uses the Simple Shader Inspectors library.</para>
    /// <para>
    /// It can be useful when you need some really custom behaviour, at the cost oh having to manage the controls initialization, property fetching etc.
    /// It can be a good starting point to see the source code of <see cref="SimpleShaderInspector"/>.
    /// </para>
    /// </remarks>
    public interface ISimpleShaderInspector : IControlContainer
    {
        /// <summary>
        /// Array of the materials selected.
        /// </summary>
        Material[] Materials { get; }

        /// <summary>
        /// Shader this inspector is viewing.
        /// </summary>
        Shader Shader { get; }
    }
}