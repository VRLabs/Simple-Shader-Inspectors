using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Represents a vector control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// While using a <see cref="PropertyControl"/> for a vector property can work just fine, it will default to displaying all 4 float values, even if the shader property represents
    /// a float2 or float3. This is due to unity treating all of them as a Vector4 inside inspectors, resulting on having a 4 float display.
    /// </para>
    /// <para>With this control you have the ability to fine tune which of the 4 floats to show.</para>
    /// <para>
    /// This can also be useful if you pack different values into a single Vector4 for various reasons, cause you can just create different vector controls where each one access
    /// different parts of the Vector4, so you can manage them independently inside your editor.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // adds a vector control
    /// this.AddVectorControl("ExampleVector");
    /// // adds a vector control where only the y and w values are displayed
    /// this.AddVectorControl("ExampleVector", false, true, false, true); 
    /// </code>
    /// </example>
    public class VectorControl : PropertyControl
    {
        private readonly int _visibleCount;
        
        /// <summary>
        /// Visibility state of the x value
        /// </summary>
        /// <value>
        /// True if the x value is visible, false otherwise
        /// </value>
        public bool IsXVisible { get; protected set; }
        
        /// <summary>
        /// Visibility state of the y value
        /// </summary>
        /// <value>
        /// True if the y value is visible, false otherwise
        /// </value>
        public bool IsYVisible { get; protected set; }
        
        /// <summary>
        /// Visibility state of the z value
        /// </summary>
        /// <value>
        /// True if the z value is visible, false otherwise
        /// </value>
        public bool IsZVisible { get; protected set; }
        
        /// <summary>
        /// Visibility state of the w value
        /// </summary>
        /// <value>
        /// True if the w value is visible, false otherwise
        /// </value>
        public bool IsWVisible { get; protected set; }
        /// <summary>
        /// Default constructor of <see cref="VectorControl"/>
        /// </summary>
        /// <param name="propertyName">Material property name.</param>
        /// <param name="isXVisible">Shows the x component. Optional (Default true).</param>
        /// <param name="isYVisible">Shows the y component. Optional (Default true).</param>
        /// <param name="isZVisible">Shows the z component. Optional (Default true).</param>
        /// <param name="isWVisible">Shows the w component. Optional (Default true).</param>
        /// <returns>A new <see cref="VectorControl"/> object.</returns>
        public VectorControl(string propertyName, bool isXVisible = true, bool isYVisible = true,
             bool isZVisible = true, bool isWVisible = true) : base(propertyName)
        {
            IsXVisible = isXVisible;
            IsYVisible = isYVisible;
            IsZVisible = isZVisible;
            IsWVisible = isWVisible;

            _visibleCount = 0;
            if (IsXVisible) _visibleCount++;
            if (IsYVisible) _visibleCount++;
            if (IsZVisible) _visibleCount++;
            if (IsWVisible) _visibleCount++;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            Rect r = EditorGUILayout.GetControlRect(true);
            EditorGUI.LabelField(r, Content);
            EditorGUI.showMixedValue = Property.hasMixedValue;
            EditorGUI.BeginChangeCheck();

            r = new Rect(r.x + EditorGUIUtility.labelWidth, r.y, r.width - EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight);

            int i = 0;
            int oldIndentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            // I'll be honest, it looks somewhat retarded, but it's not worth to make an array and cycle it, it would waste more resources.
            Vector4 vector = new Vector4(0, 0, 0, 0);
            vector.x = IsXVisible ? DrawSingleField("X", Property.vectorValue.x, GetFragmentedRect(r, _visibleCount, i++)) : Property.vectorValue.x;
            vector.y = IsYVisible ? DrawSingleField("Y", Property.vectorValue.y, GetFragmentedRect(r, _visibleCount, i++)) : Property.vectorValue.y;
            vector.z = IsZVisible ? DrawSingleField("Z", Property.vectorValue.z, GetFragmentedRect(r, _visibleCount, i++)) : Property.vectorValue.z;
            vector.w = IsWVisible ? DrawSingleField("W", Property.vectorValue.w, GetFragmentedRect(r, _visibleCount, i++)) : Property.vectorValue.w;

            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(Property.displayName);
                Property.vectorValue = vector;
            }

            EditorGUI.showMixedValue = false;
            EditorGUI.indentLevel = oldIndentLevel;
        }

        private static Rect GetFragmentedRect(Rect r, int count, int current)
        {
            return new Rect(r.x + (r.width * current / count), r.y, r.width / count, r.height);
        }

        private static float DrawSingleField(string label, float value, Rect r)
        {
            Rect rt = new Rect(r.x, r.y, 15, r.height);
            GUI.Label(rt, label, Styles.CenterLabel);
            rt = new Rect(r.x + 15, r.y, r.width - 15, r.height);
            return EditorGUI.FloatField(rt, value);
        }
    }
}