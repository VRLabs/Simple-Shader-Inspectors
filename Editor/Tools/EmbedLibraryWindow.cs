using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace VRLabs.SimpleShaderInspectors.Tools
{
    /// <summary>
    /// Editor window that embeds the Simple Shader Inspectors library into any custom editor folder with customized namespace.
    /// </summary>
    public class EmbedLibraryEditor : EditorWindow
    {
        private const string PATH = "Assets/VRLabs/SimpleShaderInspectors/Editor";
        private const string NAMESPACE = "VRLabs.SimpleShaderInspectors";
        private const string IDENTIFIER = "SSI";

        private static readonly Regex _namespaceRegex = new Regex("^[a-zA-Z0-9.]*$");

        [MenuItem("VRLabs/Simple Shader Inspectors/Embed to shader editor folder")]
        private static EmbedLibraryEditor CreateWindow()
        {
            EmbedLibraryEditor window = EditorWindow.GetWindow<EmbedLibraryEditor>();
            window.titleContent = new GUIContent("Embred");
            window.minSize = new Vector2(400, 280);
            window.maxSize = new Vector2(400, 280);
            return window;
        }

        private string _selectedPath = null;
        private string _customNamespace = null;
        private string _acronym = null;
        private bool _keepComments;

        void OnGUI()
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Select folder"))
            {
                string path = EditorUtility.OpenFolderPanel("Select editor folder to use", "Assets", "Editor");
                if (path.Length == 0 || !Path.GetFileName(path).Equals("Editor"))
                {
                    _selectedPath = null;
                }
                else
                {
                    _selectedPath = path;
                }
            }
            GUILayout.Label("Selected folder: " + _selectedPath, Styles.MultilineLabel);
            if (_selectedPath == null)
            {
                EditorGUILayout.HelpBox("You need to select a valid Editor folder.", MessageType.Warning);
            }
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            string cn = EditorGUILayout.TextField("Custom Namespace", _customNamespace);
            if (EditorGUI.EndChangeCheck() && _namespaceRegex.IsMatch(cn))
            {
                _customNamespace = cn;
            }

            GUILayout.Label("The namespace will be: " + _customNamespace + ".SimpleShaderInspectors", Styles.MultilineLabel);
            EditorGUILayout.HelpBox("Remember to change the namespace on your shader editor as well.", MessageType.Info);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();
            cn = EditorGUILayout.TextField("Assets acronym", _acronym);
            if (EditorGUI.EndChangeCheck() && !string.IsNullOrWhiteSpace(cn))
            {
                _acronym = cn;
            }

            GUILayout.Label("The used acronym will be: " + _acronym, Styles.MultilineLabel);
            EditorGUILayout.HelpBox("Some assets in the resource folder contain an acronym, multiple copies of those assets are not allowed to have the same acronym, so choose an original one.", MessageType.Info);
            EditorGUILayout.Space();

            _keepComments = EditorGUILayout.Toggle("Keep comments", _keepComments);
            EditorGUILayout.Space();

            EditorGUI.BeginDisabledGroup(string.IsNullOrWhiteSpace(_selectedPath) || string.IsNullOrWhiteSpace(_customNamespace) || string.IsNullOrWhiteSpace(_acronym));
            if (GUILayout.Button("Embed SimpleShaderInspectors"))
            {
                CopyLibrary(_selectedPath, _customNamespace, _acronym, _keepComments);
                this.Close();
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void CopyLibrary(string path, string customNamespace, string assetIdentifier, bool keepComments = false)
        {
            if (!Directory.Exists(PATH))
            {
                throw new DirectoryNotFoundException("Simple Shader Inspectors has not been found in its default location, consider deleting it and reinstalling it using the official UnityPackage.");
            }
            if (Directory.Exists(path + "/SimpleShaderInspectors"))
            {
                Directory.Delete(path + "/SimpleShaderInspectors", true);
            }
            CopyDirectory(PATH, path, customNamespace, assetIdentifier, "", keepComments);

            AssetDatabase.Refresh();
        }

        private static void CopyDirectory(string oldPath, string newPath, string customNamespace, string assetIdentifier, string subpath, bool keepComments)
        {
            foreach (var file in Directory.GetFiles(oldPath).Where(x => !Path.GetExtension(x).Equals(".meta")))
            {
                if (Path.GetExtension(file).Equals(".cs"))
                {
                    List<string> lines = new List<string>();
                    lines.AddRange(File.ReadAllLines(file));
                    int index = 0;
                    int i = 0;
                    while (i < lines.Count && !keepComments)
                    {
                        index = lines[i].IndexOf("//");
                        if (index != -1)
                        {
                            if (!string.IsNullOrEmpty(lines[i].Substring(0, index).Trim()))
                            {
                                lines[i] = lines[i].Substring(0, index);
                                i++;
                            }
                            else
                            {
                                lines.RemoveAt(i);
                            }
                        }
                        else
                        {
                            i++;
                        }
                    }

                    string text = string.Join(System.Environment.NewLine, lines);

                    text = text.Replace(NAMESPACE, customNamespace + ".SimpleShaderInspectors");

                    if (Path.GetFileName(file).Equals("Styles.cs"))
                    {
                        text = text.Replace(IDENTIFIER, assetIdentifier);
                    }
                    string finalPath = file.Replace(oldPath, newPath + "/SimpleShaderInspectors" + subpath);
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath));
                    File.WriteAllText(finalPath, text);
                }
                else if (Path.GetDirectoryName(file).Contains("Resources"))
                {
                    string finalPath = file.Replace(oldPath, newPath + "/SimpleShaderInspectors" + subpath);
                    finalPath = finalPath.Replace(IDENTIFIER, assetIdentifier);
                    Directory.CreateDirectory(Path.GetDirectoryName(finalPath));
                    finalPath = finalPath.Substring(finalPath.IndexOf("Assets"));
                    AssetDatabase.CopyAsset(file, finalPath);
                }
            }

            foreach (var directory in Directory.GetDirectories(oldPath))
            {
                if (!Path.GetFileName(directory).Equals("Tools"))
                {
                    string newSubPath = subpath + directory.Replace(PATH + subpath, "");
                    CopyDirectory(directory, newPath, customNamespace, assetIdentifier, newSubPath, keepComments);
                }
            }
        }
    }
}