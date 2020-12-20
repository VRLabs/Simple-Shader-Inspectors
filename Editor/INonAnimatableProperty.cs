using UnityEditor;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface that defines an object that has a property that shouldn't be animated by the animation window
    /// </summary>
    public interface INonAnimatableProperty
    {
        /// <summary>
        /// Boolean indicating if a non animatable material property need to be updated.
        /// </summary>
        bool NonAnimatablePropertyChanged { get; set; }

        /// <summary>
        /// Updates the value of the non animatable property
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        void UpdateNonAnimatableProperty(MaterialEditor materialEditor);
    }
}