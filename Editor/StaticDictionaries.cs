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
        /// <typeparam name="string">Type of the Key.</typeparam>
        /// <typeparam name="bool">Type of the value.</typeparam>
        public static Dictionary<string, bool> BoolDictionary { get; set; } = new Dictionary<string, bool>();
    }
}