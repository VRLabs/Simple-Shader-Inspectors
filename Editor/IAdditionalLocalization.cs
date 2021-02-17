using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface used to define the need for a control to use additional localization content
    /// </summary>
    public interface IAdditionalLocalization
    {
        /// <summary>
        /// Array containing the additional localization elements needed by the control.
        /// </summary>
        AdditionalLocalization[] AdditionalContent { get; set; }
    }

    /// <summary>
    /// This class is a simple class containing a Name used for finding the localization content and a GUIContent object containing said content.
    /// </summary>
    public class AdditionalLocalization
    {
        /// <summary>
        /// Name of the additional localized content.
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// GUIContent containing the localized content.
        /// </summary>
        public GUIContent Content { get; set; }
    }

    public static class AdditionalContentExtensions
    {
        public static void InitializeLocalizationWithNames(this IAdditionalLocalization obj, string[] contentNames)
        {
            obj.AdditionalContent = new AdditionalLocalization[contentNames.Length];
            for (int i = 0; i < contentNames.Length; i++)
                obj.AdditionalContent[i] = new AdditionalLocalization { Name = contentNames[i] };

        }

        public static AdditionalLocalization[] CreateLocalizationArrayFromNames(string[] contentNames)
        {
            AdditionalLocalization[] obj = new AdditionalLocalization[contentNames.Length];
            for (int i = 0; i < contentNames.Length; i++)
                obj[i] = new AdditionalLocalization { Name = contentNames[i] };
            
            return obj;
        }
    }
}