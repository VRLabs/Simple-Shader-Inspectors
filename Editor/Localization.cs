using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Static helper class used for managing localizations.
    /// </summary>
    public static class Localization
    {
        /// <summary>
        /// Apply Localization strings to a list of controls.
        /// </summary>
        /// <param name="controls">Controls to apply a localization</param>
        /// <param name="localizationFilePath">Path of the localization file</param>
        /// <param name="writeIfNotFound">Generate empty fields / new localization file if the provided one is missing or incomplete.</param>
        public static void ApplyLocalization(this IEnumerable<SimpleControl> controls, string localizationFilePath, bool writeIfNotFound = false)
        {
            LocalizationFile localizationFile;

            if (File.Exists(localizationFilePath))
                localizationFile = JsonUtility.FromJson<LocalizationFile>(File.ReadAllText(localizationFilePath));
            else
                localizationFile = new LocalizationFile();

            List<PropertyInfo> missingInfo = SetPropertiesLocalization(controls, localizationFile.Properties).ToList();

            if (missingInfo.Count > 0 && writeIfNotFound)
            {
                missingInfo.AddRange(localizationFile.Properties);
                localizationFile.Properties = missingInfo.ToArray();
                File.WriteAllText(localizationFilePath, JsonUtility.ToJson(localizationFile, true));
            }
        }

        private static IEnumerable<PropertyInfo> SetPropertiesLocalization(IEnumerable<SimpleControl> controls, PropertyInfo[] propertyInfos)
        {
            List<PropertyInfo> missingInfo = new List<PropertyInfo>();
            foreach (var control in controls)
            {
                // Find localization of the control content. 
                var selectedInfo = propertyInfos.FindPropertyByName(control.ControlAlias);
                if (selectedInfo == null)
                {
                    selectedInfo = new PropertyInfo
                    {
                        Name = control.ControlAlias,
                        DisplayName = control.ControlAlias,
                        Tooltip = ""
                    };

                    if (!string.IsNullOrWhiteSpace(selectedInfo.Name))
                        missingInfo.Add(selectedInfo);
                }
                control.Content = new GUIContent(selectedInfo.DisplayName, selectedInfo.Tooltip);

                switch (control)
                {
                    // Find additional content in case it implements the IAdditionalLocalization interface.
                    case IAdditionalLocalization additional:
                        foreach (var content in additional.AdditionalContent)
                        {
                            string fullName = control.ControlAlias + "_" + content.Name;
                            var extraInfo = propertyInfos.FindPropertyByName(fullName);
                            if (extraInfo == null)
                            {
                                extraInfo = new PropertyInfo
                                {
                                    Name = fullName,
                                    DisplayName = fullName,
                                    Tooltip = ""
                                };
                                if (!string.IsNullOrWhiteSpace(extraInfo.Name))
                                    missingInfo.Add(extraInfo);
                            }

                            content.Content = new GUIContent(extraInfo.DisplayName, extraInfo.Tooltip);
                        }
                        break;

                    // Recursively set property localization for all properties inside this control if it has the IControlContainer interface.
                    case IControlContainer container:
                        missingInfo.AddRange(SetPropertiesLocalization(container.GetControlList(), propertyInfos));
                        break;
                }
            }
            return missingInfo;
        }
    }

    public static class LocalizationSearchers
    {
        public static PropertyInfo FindPropertyByName(this IEnumerable<PropertyInfo> properties, string name)
        {
            return properties?.FirstOrDefault(property => property.Name.Equals(name));
        }
        public static string FindPropertyByName(this (string, string)[] properties, string name)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].Item1.Equals(name))
                {
                    return properties[i].Item2;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class PropertyInfo
    {
        public string Name;
        public string DisplayName;
        public string Tooltip;
    }
    [Serializable]
    public class LocalizationFile
    {
        public PropertyInfo[] Properties;
        
        public LocalizationFile()
        {
            Properties = Array.Empty<PropertyInfo>();
        }
    }
    [Serializable]
    public class SettingsFile
    {
        public string SelectedLanguage;
    }
}
