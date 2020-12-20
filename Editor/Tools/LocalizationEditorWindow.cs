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
    /// Editor window that enables the user to modify json localization data.
    /// </summary>
    public class LocalizationEditorWindow : EditorWindow
    {
        [MenuItem("VRLabs/Simple Shader Inspectors/Localization file editor")]
        private static LocalizationEditorWindow CreateWindow()
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("Localization Editor");
            return window;
        }

        private string _selectedPath = null;
        private LocalizationFile _localization;
        private Vector2 _scroll;

        void OnGUI()
        {
            if (_selectedPath != null)
            {
                EditorGUILayout.BeginHorizontal();
            }
            if (GUILayout.Button("Select Localization file"))
            {
                string path = EditorUtility.OpenFilePanel("Select localization file to edit", "Assets", "json");
                if (path.Length != 0)
                {
                    _selectedPath = path;
                    _localization = JsonUtility.FromJson<LocalizationFile>(File.ReadAllText(_selectedPath));
                }
            }
            if (_selectedPath != null)
            {
                if (GUILayout.Button("Save currently open file"))
                {
                    File.WriteAllText(_selectedPath, JsonUtility.ToJson(_localization, true));
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Selected localization file: " + _selectedPath, Styles.MultilineLabel);
                EditorGUILayout.Space();

                _scroll = EditorGUILayout.BeginScrollView(_scroll);

                foreach (var property in _localization.Properties)
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField(property.Name, EditorStyles.boldLabel);

                    //We should let the inspector automatically generate the names
                    //property.Name = EditorGUILayout.TextField("Name", property.Name);
                    property.DisplayName = EditorGUILayout.TextField("Display Name", property.DisplayName);
                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Tooltip", GUILayout.Width(146));
                    property.Tooltip = EditorGUILayout.TextArea(property.Tooltip);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                EditorGUILayout.EndScrollView();
            }
        }
    }
}