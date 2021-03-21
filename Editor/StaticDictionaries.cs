using System.Collections.Generic;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Static class containing dictionaries that can be used by controls for various needs.
    /// </summary>
    public static class StaticDictionaries
    {
        /// <summary>
        /// Dictionary containing boolean values.
        /// </summary>
        public static Dictionary<string, bool> BoolDictionary { get; set; } = new Dictionary<string, bool>();
    }
}