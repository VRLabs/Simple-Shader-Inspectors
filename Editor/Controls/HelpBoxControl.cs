using UnityEditor;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    public class HelpBoxControl : SimpleControl
    {
        
        [Chainable] public MessageType BoxType { get; set; }
        [Chainable] public bool IsWideBox { get; set; }
        
        public HelpBoxControl(string alias) : base(alias)
        {
            BoxType = MessageType.None;
            IsWideBox = true;
        }

        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUILayout.HelpBox(Content.text, BoxType, IsWideBox);
        }
    }
}