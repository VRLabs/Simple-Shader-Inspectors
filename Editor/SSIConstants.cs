namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Constants used across the library.
    /// </summary>
    /// <remarks>
    /// These constants are the default one used, when you embed the library you have the option to change some of these defaults to whatever you want
    /// </remarks>
    public static class SSIConstants
    {
        /// <summary>
        /// Default name of the subfolder of the Resources folder containing all the resources needed for the library.
        /// </summary>
        /// <remarks>
        /// A custom folder name is needed to differentiate different installed versions of the libraries from multiple shaders, you HAVE to use a different name when embedding the library to your shader project,
        /// or else conflicts may arise whenever someone who already has the official library will also download your shader, especially when the 2 versions don't match up.
        /// </remarks>
        public const string RESOURCES_FOLDER = "SSI";
        
        /// <summary>
        /// Default path in the menu to place all windows menu options.
        /// </summary>
        public const string WINDOW_PATH = "VRLabs/Simple Shader Inspectors";
    }
}