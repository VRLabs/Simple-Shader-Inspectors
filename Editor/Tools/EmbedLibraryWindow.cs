using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.UIElements;

namespace VRLabs.SimpleShaderInspectors.Tools
{
    /// <summary>
    /// Editor window that embeds the Simple Shader Inspectors library into any custom editor folder with customized namespace.
    /// </summary>
    public class EmbedLibraryEditor : EditorWindow
    {
        private const string PATH = "Assets/VRLabs/SimpleShaderInspectors/Editor";
        private const string NAMESPACE = "VRLabs.SimpleShaderInspectors";

        private static readonly Regex _namespaceRegex = new Regex("^[a-zA-Z0-9.]*$");

        [MenuItem(SSIConstants.WINDOW_PATH + "/Embed Library")]
        private static EmbedLibraryEditor CreateWindow()
        {
            var window = EditorWindow.GetWindow<EmbedLibraryEditor>();
            window.titleContent = new GUIContent("Embed");
            return window;
        }
        
        [Serializable]
        private class LibrarySettings
        {
            public string nmsc;
            public string rscfName;
            public string windowPath;
        }
        
        private bool _keepComments;
        private TextField _namespaceField;
        private TextField _resourceFolderField;
        private Button _saveButton;
        private TextField _windowPathField;
        private Label _namespaceLabel;
        private Button _embedButton;
        private Button _loadButton;
        private Toggle _commentsFiels;


        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;
            
            var visualTree =Resources.Load<VisualTreeAsset>(SSIConstants.RESOURCES_FOLDER +"/UIElements/EmbedLibraryWindow");
            VisualElement labelFromUxml = visualTree.CloneTree();
            root.Add(labelFromUxml);
        
            _namespaceField = root.Q<TextField>("NamespaceField");
            _resourceFolderField = root.Q<TextField>("ResourceFolderField");
            _windowPathField = root.Q<TextField>("WindowPathField");
            _namespaceLabel = root.Q<Label>("NamespacePreview");
            _commentsFiels = root.Q<Toggle>("CommentsField");
            _embedButton = root.Q<Button>("EmbedButton");
            _loadButton = root.Q<Button>("LoadButton");
            _saveButton = root.Q<Button>("SaveButton");

            _embedButton.clicked += EmbedButtonOnclick;
            _loadButton.clicked += LoadSettingsFromFile;
            _saveButton.clicked += SaveSettingsOnFile;

            _namespaceField.RegisterValueChangedCallback(x =>
            {
                if (_namespaceRegex.IsMatch(x.newValue))
                    _namespaceLabel.text = x.newValue + ".SimpleShaderInspectors";
                else
                    _namespaceField.value = x.previousValue;
            });
        }

        private void EmbedButtonOnclick()
        {
            if (!Directory.Exists(PATH))
                EditorUtility.DisplayDialog("Error", "Simple Shader Inspectors has not been found in its default location, consider deleting it and reinstalling it using the official UnityPackage.", "Ok");
        
            string path = EditorUtility.OpenFolderPanel("Select editor folder to use", "Assets", "Editor");
            if (path.Length == 0)
                return;

            if (!Path.GetFileName(path).Equals("Editor"))
            {
                EditorUtility.DisplayDialog("Error", "The folder must be an \"Editor\" folder", "Ok");
                return;
            }
            
            if (Directory.Exists(path + "/SimpleShaderInspectors"))
                Directory.Delete(path + "/SimpleShaderInspectors", true);
            
            CopyDirectory(PATH, path, "", _commentsFiels.value);
            
            string licencePath = PATH.Replace("Editor", "LICENSE");
            if (File.Exists(licencePath))
                File.Copy(licencePath, path + "/SimpleShaderInspectors/LICENSE");

            AssetDatabase.Refresh();
        }
        
        private void SaveSettingsOnFile()
        {
            string path = EditorUtility.SaveFilePanel("Save settings to file", "Assets", "embedSettings.json", "json");
            if (path.Length == 0)
                return;
        
            var settings = new LibrarySettings
            {
                nmsc = _namespaceField.value,
                rscfName = _resourceFolderField.value,
                windowPath = _windowPathField.value
            };
        
            File.WriteAllText(path,JsonUtility.ToJson(settings));
        }
    
        private void LoadSettingsFromFile()
        {
            string path = EditorUtility.OpenFilePanel("Load settings", "Assets", "json");
            if (!File.Exists(path))
            {
                EditorUtility.DisplayDialog("Error", "File does not exist", "Ok");
                return;
            }

            var settings = JsonUtility.FromJson<LibrarySettings>(File.ReadAllText(path));

            _namespaceField.value = settings.nmsc;
            _resourceFolderField.value = settings.rscfName;
            _windowPathField.value = settings.windowPath;
        }

        private void CopyDirectory(string oldPath, string newPath, string subpath, bool keepComments)
        {
            foreach (var file in Directory.GetFiles(oldPath).Where(x => !Path.GetExtension(x).Equals(".meta")))
            {
                if (Path.GetFileName(file).Contains("EmbedLibraryWindow")) continue;
                
                if (new [] {".cs", ".uxml", ".shader", ".asmdef"}.Contains(Path.GetExtension(file)))
                {
                    var lines = new List<string>(File.ReadAllLines(file));
                    var newLines = lines.Where(line => !line.Trim().StartsWith("//")).ToList();
                    string text = string.Join(System.Environment.NewLine, newLines);

                    text = text.Replace(NAMESPACE, _namespaceField.value + ".SimpleShaderInspectors");
                    if (Path.GetFileName(file).Equals("SSIConstants.cs"))
                    {
                        text = text.Replace($"\"{SSIConstants.WINDOW_PATH}\"", $"\"{_windowPathField.value}\"");
                        text = text.Replace($"\"{SSIConstants.RESOURCES_FOLDER}\"", $"\"{_resourceFolderField.value}\"");
                    }

                    string finalPath = file.Replace(oldPath, newPath + "/SimpleShaderInspectors" + subpath);
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath) ?? string.Empty);
                    File.WriteAllText(finalPath, text);
                }
                else if (Path.GetDirectoryName(file).Contains("Resources"))
                {
                    string finalPath = file.Replace(oldPath, newPath + "/SimpleShaderInspectors" + subpath);
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath) ?? string.Empty);
                    finalPath = finalPath.Substring(finalPath.IndexOf("Assets", StringComparison.Ordinal));
                    AssetDatabase.CopyAsset(file, finalPath);
                    
                    if (Path.GetExtension(finalPath).Equals(".uss"))
                    {
                        string text = File.ReadAllText(finalPath);
                        text = text.Replace($"resource(\"{SSIConstants.RESOURCES_FOLDER}/", $"resource(\"{_resourceFolderField.value}/");
                        File.WriteAllText(finalPath, text);
                    }
                }
                else if (Path.GetFileName(file).Equals("LICENSE"))
                {
                    string finalPath = file.Replace(oldPath, newPath + "/SimpleShaderInspectors" + subpath);
                    File.Copy(file, finalPath);
                }
            }

            foreach (string directory in Directory.GetDirectories(oldPath))
            {
                if (Path.GetFileName(directory).Equals("Tools")) continue;

                string newSubPath = subpath + "/" + Path.GetFileName(directory);
                if (Path.GetFileName(directory).Equals(SSIConstants.RESOURCES_FOLDER) && Path.GetFileName(Path.GetDirectoryName(directory)).Equals("Resources"))
                    newSubPath = subpath + "/" + _resourceFolderField.value;
                CopyDirectory(directory, newPath,  newSubPath, keepComments);
            }
        }
    }
}