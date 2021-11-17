using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using UnityEditorInternal;

namespace VRLabs.SimpleShaderInspectors.Tools
{
    /// <summary>
    /// Editor window that enables the user to modify json localization data.
    /// </summary>
    public class LocalizationEditorWindow : EditorWindow
    {
        [MenuItem(SSIConstants.WINDOW_PATH + "/Localization file editor")]
        private static LocalizationEditorWindow CreateWindow()
        {
            LocalizationEditorWindow window = EditorWindow.GetWindow<LocalizationEditorWindow>();
            window.titleContent = new GUIContent("Localization Editor");
            return window;
        }

        private PropertyInfo _activeProperty;
        private bool _enableNameEditing;
        private ReorderableList _list;
        private List<PropertyInfo> _properties;
        private GUIStyle _rightSubLabel;
        private string _selectedPath;
        private Vector2 _scroll;

        private void OnEnable()
        {
            _rightSubLabel = new GUIStyle
            {
                alignment = TextAnchor.MiddleRight,
                normal =
                {
                    textColor = Color.gray
                }
            };
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            if (!string.IsNullOrEmpty(_selectedPath))
                EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Select Localization file"))
            {
                string path = EditorUtility.OpenFilePanel("Select localization file to edit", "Assets", "json");
                LoadReorderableList(path);
            }

            if (!string.IsNullOrEmpty(_selectedPath))
            {
                if (GUILayout.Button("Save currently open file"))
                    File.WriteAllText(_selectedPath, JsonUtility.ToJson(new LocalizationFile { Properties = _properties.ToArray() }, true));

                if (GUILayout.Button("Save as new file"))
                {
                    string path = EditorUtility.SaveFilePanel("Save localization", Path.GetDirectoryName(_selectedPath), "New language", "json");
                    File.WriteAllText(path, JsonUtility.ToJson(new LocalizationFile { Properties = _properties.ToArray() }, true));
                    _selectedPath = path;
                    AssetDatabase.Refresh();
                }

                EditorGUILayout.EndHorizontal();
                GUILayout.Label("Selected localization file: " + _selectedPath, Styles.MultilineLabel);
                EditorGUILayout.Space();

                if (_activeProperty != null)
                {
                    EditorGUILayout.BeginVertical("box");

                    EditorGUILayout.LabelField(_activeProperty.Name, EditorStyles.boldLabel);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUI.BeginDisabledGroup(!_enableNameEditing);
                    _activeProperty.Name = EditorGUILayout.TextField("Name", _activeProperty.Name);
                    EditorGUI.EndDisabledGroup();

                    // Unity default lock icon style alignment sucks, but i also suck at making icons so here's some GUILayout bs to align unity's one.
                    EditorGUILayout.BeginVertical(GUILayout.Width(18));
                    GUILayout.Space(3);
                    _enableNameEditing = !GUILayout.Toggle(!_enableNameEditing, "", "IN LockButton", GUILayout.Width(14), GUILayout.Height(15));
                    EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();

                    _activeProperty.DisplayName = EditorGUILayout.TextField("Display Name", _activeProperty.DisplayName);

                    EditorGUILayout.BeginHorizontal();
                    GUILayout.Label("Tooltip", GUILayout.Width(146));
                    _activeProperty.Tooltip = EditorGUILayout.TextArea(_activeProperty.Tooltip);
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Space();
                }
                _scroll = EditorGUILayout.BeginScrollView(_scroll);
                if (_list is null)
                    LoadReorderableList(_selectedPath);

                _list?.DoLayoutList();
                GUILayout.Space(20);
                EditorGUILayout.EndScrollView();
            }
        }

        private void LoadReorderableList(string path)
        {
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
            {
                _selectedPath = path;
                var localization = JsonUtility.FromJson<LocalizationFile>(File.ReadAllText(_selectedPath));
                _properties = new List<PropertyInfo>(localization.Properties);

                _list = new ReorderableList(_properties, typeof(PropertyInfo), true, false, true, true)
                {
                    drawElementCallback = DrawListItems,
                    onSelectCallback = l =>
                    {
                        _activeProperty = _properties[l.index];
                        _enableNameEditing = false;
                    },
                    index = 0
                };
                _activeProperty = _properties[0];
            }
            else
            {
                _selectedPath = null;
            }
        }

        private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
        {
            var property = _properties[index];

            GUI.Label(new Rect(rect.x, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), property.DisplayName, EditorStyles.boldLabel);
            GUI.Label(new Rect(rect.x + rect.width / 2, rect.y, rect.width / 2, EditorGUIUtility.singleLineHeight), property.Name, _rightSubLabel);
        }
    }
}