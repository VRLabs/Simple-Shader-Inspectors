using UnityEditor;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a control containing an helpbox to display information
    /// </summary>
    /// <remarks>
    /// Whenever you want to display some important information of some kind, this is the control that will help you make it more visible.
    /// </remarks>
    /// <example>
    /// Initialize the control inside the inspector:
    /// <code>
    /// // initialize an HelpBoxControl
    /// this.AddHelpBoxControl("ExampleHelpBox");
    /// // initialize an HelpBoxControl and specifies the type of message shown (by default is set to MessageType.None).
    /// this.AddHelpBoxControl("ExampleHelpBox2").SetBoxType(MessageType.Info);
    /// // initialize an HelpBoxControl and specifies that the HelpBox should not be wide (by default is set to True).
    /// this.AddHelpBoxControl("ExampleHelpBox3").SetIsWideBox(false);
    /// </code>
    /// </example>
    public class HelpBoxControl : SimpleControl
    {
        /// <summary>
        /// Indicates the type of message that is contained in the box.
        /// </summary>
        [Chainable] public MessageType BoxType { get; set; }
        
        /// <summary>
        /// Indicates if the HelpBox is a wide box (spans to the entire length of the window) or not (is only as large as the area where input fields are usually placed)
        /// </summary>
        [Chainable] public bool IsWideBox { get; set; }
        
        /// <summary>
        /// Default constructor of <see cref="HelpBoxControl"/>
        /// </summary>
        /// <param name="alias">Alias of the control, used for localization (required)</param>
        public HelpBoxControl(string alias) : base(alias)
        {
            BoxType = MessageType.None;
            IsWideBox = true;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            EditorGUILayout.HelpBox(Content.text, BoxType, IsWideBox);
        }
    }
}