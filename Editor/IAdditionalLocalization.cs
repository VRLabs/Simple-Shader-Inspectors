using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Interface used to define the need for a control to use additional localization content
    /// </summary>
    /// <remarks>
    /// If a control needs more localized content than the one given by default, it can implement this interface to let the inspector know that it needs
    /// more than one localized content.
    /// </remarks>
    public interface IAdditionalLocalization
    {
        /// <summary>
        /// Array containing the additional localization elements needed by the control.
        /// </summary>
        /// <remarks>
        /// Is up to the control to set the array size and the unique names for each content string based
        /// on the control needs
        /// </remarks>
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

    /// <summary>
    /// Static class containing extension methods that manipulate <see cref="AdditionalLocalization"/> or <see cref="AdditionalLocalization"/> objects or arrays.
    /// </summary>
    public static class AdditionalContentExtensions
    {
        /// <summary>
        /// Initialize the <see cref="AdditionalLocalization"/> array of an <see cref="IAdditionalLocalization"/> object with the provided array of names.
        /// </summary>
        /// <param name="obj"><see cref="IAdditionalLocalization"/>Object to use.</param>
        /// <param name="contentNames">Array of names.</param>
        /// <remarks>
        /// Sometimes there's a need of a lot of localized content, but having to manually declare each item of the array manually can bloat the control's constructor
        /// fairly quick, and it becomes even worse when multiple constructors are needed. It's instead easier to declare an array of strings in a private field, and call this method instead.
        /// </remarks>
        public static void InitializeLocalizationWithNames(this IAdditionalLocalization obj, string[] contentNames)
        {
            obj.AdditionalContent = new AdditionalLocalization[contentNames.Length];
            for (int i = 0; i < contentNames.Length; i++)
                obj.AdditionalContent[i] = new AdditionalLocalization { Name = contentNames[i] };

        }
        /// <summary>
        /// Creates an <see cref="AdditionalLocalization"/> array from an array of names.
        /// </summary>
        /// <param name="contentNames">Array of names.</param>
        /// <returns>An array of <see cref="AdditionalLocalization"/> objects.</returns>
        /// <remarks>
        /// Functionally is the same as <see cref="InitializeLocalizationWithNames"/>, but instead of automatically assigning the resulting <see cref="AdditionalLocalization"/> array to the
        /// calling object, it returns the generated array, so that more things can be done with it later. A possible example is generating 2 sets of contents, and then exposing a combination
        /// of the 2 dynamically based on some conditional logic.
        /// </remarks>
        public static AdditionalLocalization[] CreateLocalizationArrayFromNames(string[] contentNames)
        {
            AdditionalLocalization[] obj = new AdditionalLocalization[contentNames.Length];
            for (int i = 0; i < contentNames.Length; i++)
                obj[i] = new AdditionalLocalization { Name = contentNames[i] };
            
            return obj;
        }
    }
}