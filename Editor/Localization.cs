using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    internal static class Localization
    {
        /// <summary>
        /// Gets informations about available localizations on selected path.
        /// </summary>
        /// <param name="path">Path where the localization data is stored</param>
        /// <returns>
        ///     A string array with all available localizations,
        ///     A string containing the selected localization,
        ///     An integer containing the index of the selected localization.
        /// </returns>
        public static (string[], string, int) GetLocalization(string path)
        {
            List<string> names = new List<string>();
            if (Directory.Exists(path))
            {
                string selected = "";
                if (File.Exists(path + "/Settings.json"))
                {
                    selected = JsonUtility.FromJson<SettingsFile>(File.ReadAllText(path + "/Settings.json")).SelectedLanguage;
                }

                int selectedIndex = -1;
                string[] namesPlusSettings = Directory.GetFiles(path);
                for (int i = 0; i < namesPlusSettings.Length; i++)
                {
                    if (!namesPlusSettings[i].EndsWith("Settings.json") && !namesPlusSettings[i].EndsWith(".meta"))
                    {
                        string name = Path.GetFileNameWithoutExtension(namesPlusSettings[i]);
                        names.Add(name);
                        if (name.Equals(selected))
                        {
                            selectedIndex = names.Count - 1;
                        }
                    }
                }

                if (selectedIndex != -1) return (names.ToArray(), selected, selectedIndex);
                SaveSettings(names[0], path);
                selected = names[0];

                return (names.ToArray(), selected, selectedIndex);
            }
            else
            {
                return (null, null, -1);
            }
        }

        /// <summary>
        /// Gets a LocalizationFile object containing a localization based on the passed string.
        /// </summary>
        /// <param name="path">Path to look for the localization.</param>
        /// <param name="name">Name of the localization.</param>
        /// <returns>A LocalizationFile object containing the localizaion, or null if the localization was not found.</returns>
        public static LocalizationFile GetSelectedLocalization(string path, string name)
        {
            if (File.Exists(path + "/" + name + ".json"))
            {
                return JsonUtility.FromJson<LocalizationFile>(File.ReadAllText(path + "/" + name + ".json"));
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Generates a default English localization file.
        /// </summary>
        /// <param name="controls">Controls that need a localization.</param>
        /// <param name="shader">Shader that the inspector references</param>
        /// <param name="path">Save path</param>
        /// <returns>
        ///     A string array with only one element with the localization name,
        ///     A string containing the selected localization,
        ///     The generated localization.
        /// </returns>
        public static (string[], string, LocalizationFile) GenerateDefaultLocalization(List<SimpleControl> controls, Shader shader, string path)
        {
            List<PropertyInfo> localizationProperties = new List<PropertyInfo>();

            GenerateLocalizationObjectStructure(controls, shader, localizationProperties);

            LocalizationFile file = new LocalizationFile { Properties = localizationProperties.ToArray() };
            Directory.CreateDirectory(path);
            File.WriteAllText(path + "/English.json", JsonUtility.ToJson(file, true));
            SaveSettings("English", path);
            return (new string[] { "English" }, "English", file);
        }

        // Generates the list of PropertyInfo based on the controls that need a localization.
        private static void GenerateLocalizationObjectStructure(List<SimpleControl> controls, Shader shader, List<PropertyInfo> localizations)
        {
            (string, string)[] properties = new (string, string)[ShaderUtil.GetPropertyCount(shader)];
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = (ShaderUtil.GetPropertyName(shader, i), ShaderUtil.GetPropertyDescription(shader, i));
            }
            foreach (SimpleControl control in controls)
            {
                if (localizations.FindPropertyByName(control.ControlAlias) == null)
                {
                    string displayName;
                    if (control is PropertyControl pr)
                    {
                        displayName = properties.FindPropertyByName(pr.PropertyName);
                    }
                    else
                    {
                        displayName = properties.FindPropertyByName(control.ControlAlias);
                    }

                    if (string.IsNullOrEmpty(displayName))
                    {
                        displayName = "";
                    }
                    PropertyInfo info = new PropertyInfo
                    {
                        Name = control.ControlAlias,
                        DisplayName = displayName,
                        Tooltip = displayName
                    };
                    localizations.Add(info);
                }

                switch (control)
                {
                    case IAdditionalLocalization additional:
                        {
                            foreach (AdditionalLocalization content in additional.AdditionalContent)
                            {
                                string fullName = control.ControlAlias + "_" + content.Name;
                                if (localizations.FindPropertyByName(fullName) == null)
                                {
                                    PropertyInfo info = new PropertyInfo
                                    {
                                        Name = fullName,
                                        DisplayName = "",
                                        Tooltip = ""
                                    };
                                    localizations.Add(info);
                                }
                            }

                            break;
                        }
                    case IControlContainer container:
                        GenerateLocalizationObjectStructure(container.Controls, shader, localizations);
                        break;
                }
            }
        }

        /// <summary>
        /// Save current localization settings
        /// </summary>
        /// <param name="selectedLanguage">Selected language name.</param>
        /// <param name="path">Path where the settings file is saved.</param>
        public static void SaveSettings(string selectedLanguage, string path)
        {
            SettingsFile file = new SettingsFile { SelectedLanguage = selectedLanguage };
            Directory.CreateDirectory(path);
            string save = JsonUtility.ToJson(file, true);
            File.WriteAllText(path + "/Settings.json", save);
        }

        /// <summary>
        /// Add missing properties into the selected localization file.
        /// </summary>
        /// <param name="missingInfo">List of missing properties.</param>
        /// <param name="selectedLanguage">Selected language.</param>
        /// <param name="path">path to save.</param>
        public static void AddDefaultsForMissingProperties(List<PropertyInfo> missingInfo, string selectedLanguage, string path)
        {
            if (File.Exists(path + "/" + selectedLanguage + ".json"))
            {
                LocalizationFile file = JsonUtility.FromJson<LocalizationFile>(File.ReadAllText(path + "/" + selectedLanguage + ".json"));
                missingInfo.AddRange(file.Properties);
                file.Properties = missingInfo.ToArray();
                File.WriteAllText(path + "/" + selectedLanguage + ".json", JsonUtility.ToJson(file, true));
            }
        }
        /// <summary>
        /// Gets the property description from the property name.
        /// </summary>
        /// <param name="name">Name of the property</param>
        /// <param name="shader">Shader to seatch from</param>
        /// <returns>A string containing the property description.</returns>
        public static string GetPropertyDescription(string name, Shader shader)
        {
            for (int i = 0; i < ShaderUtil.GetPropertyCount(shader); i++)
            {
                if (ShaderUtil.GetPropertyName(shader, i).Equals(name))
                {
                    return ShaderUtil.GetPropertyDescription(shader, i);
                }
            }

            return "";
        }
    }

    internal static class LocalizationSearchers
    {
        public static PropertyInfo FindPropertyByName(this List<PropertyInfo> properties, string name)
        {
            for (int i = 0; i < properties.Count; i++)
            {
                if (properties[i].Name.Equals(name))
                {
                    return properties[i];
                }
            }
            return null;
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
    internal class PropertyInfo
    {
        public string Name;
        public string DisplayName;
        public string Tooltip;
    }
    [Serializable]
    internal class LocalizationFile
    {
        public PropertyInfo[] Properties;
    }
    [Serializable]
    internal class SettingsFile
    {
        public string SelectedLanguage;
    }
}
