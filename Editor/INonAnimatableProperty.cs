using UnityEditor;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface that defines an object that has a property that shouldn't be animated by the animation window
    /// </summary>
    /// <remarks>
    /// <para>In some cases you don't want to record changes of a property inside an animation. This will let the inspector know that.</para>
    /// <para>Bare in mind that this will not assure that the property will not be recorded since is up to the inspector to satisfy the condition,
    /// and the user could still edit the animation manually.</para>
    /// </remarks>
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