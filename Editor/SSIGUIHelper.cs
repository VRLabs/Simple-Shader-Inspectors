using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    public static class SSIGUIHelper
    {
        public static Rect GetControlRectForSingleLine()
        {
            const float CONTENT_EXTRA_SPACING = 2f;
            return EditorGUILayout.GetControlRect(true, CONTENT_EXTRA_SPACING + 16f /* EditorGUI.kSingleLineHeight*/, EditorStyles.layerMaskField); // Unity give use public non internal getters for indentation space thanks
        }

        /// <summary>
        /// Draw a texture property with an HDR color field, it's the same as MaterialEditor.TexturePropertyWithHDRColor but it adds a fix for usage under indentation that would normally break it.
        /// </summary>
        /// <param name="editor">material editor</param>
        /// <param name="label">label to show</param>
        /// <param name="textureProp">texture material property</param>
        /// <param name="colorProperty">color material property</param>
        /// <param name="showAlpha">show alpha channel</param>
        /// <returns></returns>
        public static Rect TexturePropertyWithHDRColorFixed(this MaterialEditor editor, GUIContent label, MaterialProperty textureProp, MaterialProperty colorProperty, bool showAlpha)
        {
            Rect rect = GetControlRectForSingleLine();
            editor.TexturePropertyMiniThumbnail(rect, textureProp, label.text, label.tooltip);

            if (colorProperty.type != MaterialProperty.PropType.Color)
            {
                Debug.LogError("Assuming MaterialProperty.PropType.Color (was " + colorProperty.type + ")");
                return rect;
            }

            editor.BeginAnimatedCheck(rect, colorProperty);

            // Temporarily reset the indent level as it was already used earlier to compute the positions of the layout items. same fix officially applied to MaterialEditor.TexturePropertySingleLine, why it hasn't been done to this one idk.
            int oldIndentLevel = EditorGUI.indentLevel;

            EditorGUI.indentLevel = 0;

            Rect leftRect = MaterialEditor.GetLeftAlignedFieldRect(rect);
            EditorGUI.BeginChangeCheck();
            EditorGUI.showMixedValue = colorProperty.hasMixedValue;
            Color newValue = EditorGUI.ColorField(leftRect, GUIContent.none, colorProperty.colorValue, true, showAlpha, true);
            EditorGUI.showMixedValue = false;
            if (EditorGUI.EndChangeCheck())
                colorProperty.colorValue = newValue;

            editor.EndAnimatedCheck();

            // Restore the indent level
            EditorGUI.indentLevel = oldIndentLevel;

            return rect;
        }
    }
}