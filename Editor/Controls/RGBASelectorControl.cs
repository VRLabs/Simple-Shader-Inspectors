using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control that provides a selector for a texture channel.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control can be used to filter out a mask texture into a the channel that you want. It returns to the property the index of the channel selected, with <c>Red = 0</c>, <c>Green = 1</c>, <c>Blue = 2</c> and <c>Alpha = 3</c>.
    /// </para>
    /// <para>
    /// This makes the usage of the value fairly easy in shader since to sample the channel you want you just need to do <c>sampledTexture[_SelectedChannel]</c>
    /// where <c>sampledTexture</c> if the float4 of your texture after sampling it, and <c>_SelectedChannel</c> is the property given to this control.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds a new RGBASelectorControl
    /// this.AddRGBASelectorControl("_TextureProperty"); 
    /// </code>
    /// </example>
    // ReSharper disable once InconsistentNaming
    public class RGBASelectorControl : PropertyControl
    {

        public RGBASelectorControl(string propertyName) : base(propertyName)
        {}

        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            bool hasContent = Content != null && !string.IsNullOrWhiteSpace(Content.text);
            float channel = Property.floatValue;
            EditorGUI.BeginChangeCheck();
            if (hasContent)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(Content);
                channel = GUILayout.Toolbar((int)channel, new[] { "R", "G", "B", "A" }); 
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                channel = GUILayout.Toolbar((int)channel, new[] { "R", "G", "B", "A" });
            }
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                Property.floatValue = channel;
            }
            
        }
    }
}