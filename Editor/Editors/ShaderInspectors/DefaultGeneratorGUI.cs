using UnityEditor;

namespace VRLabs.SimpleShaderInspectors
{
    public class DefaultGeneratorGUI : ShaderGUI
    {
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            foreach (MaterialProperty property in properties)
            {
                materialEditor.ShaderProperty(property, property.displayName);
            }
        }
    }
}