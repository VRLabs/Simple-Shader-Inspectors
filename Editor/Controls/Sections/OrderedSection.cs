using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Section that can be hidden and ordered when in groups.
    /// </summary>
    public class OrderedSection : Section, IAdditionalProperties
    {
        public int PushState;
        private bool isUp;
        private bool isDown;

        /// <summary>
        /// Extra properties array.
        /// </summary>
        public AdditionalProperty[] AdditionalProperties { get; set; }

        /// <summary>
        /// Boolean indicating if the activate property has been updated this cycle.
        /// </summary>
        public bool HasActivatePropertyUpdated { get; protected set; }

        /// <summary>
        /// Boolean indicating if the section has turned on this cycle.
        /// </summary>
        public bool HasSectionTurnedOn { get; set; }

        /// <summary>
        /// Boolean indicating if the section is enabled or not.
        /// </summary>
        public bool Enabled { get; protected set; }

        /// <summary>
        /// GUIStyle for the UpIcon.
        /// </summary>
        [Chainable] public GUIStyle UpIcon { get; set; }

        /// <summary>
        /// GUIStyle for the DownIcon
        /// </summary>
        [Chainable] public GUIStyle DownIcon { get; set; }

        /// <summary>
        /// GUIStyle for the DeleteIcon
        /// </summary>
        [Chainable] public GUIStyle DeleteIcon { get; set; }

        /// <summary>
        /// Color of UpIcon.
        /// </summary>
        [Chainable] public Color UpColor { get; set; }

        /// <summary>
        /// Color of Downicon.
        /// </summary>
        [Chainable] public Color DownColor { get; set; }

        /// <summary>
        /// Color of Deleteicon.
        /// </summary>
        [Chainable] public Color DeleteColor { get; set; }

        /// <summary>
        /// Constructor of <see cref="OrderedSection"/> used when creating a property driven ActivatableSection.
        /// </summary>
        /// <param name="activatePropertyName">Material property that will drive the section enable state</param>
        /// <param name="showPropertyName">Material property that will drive the section open state</param>
        /// <param name="hideValue">Float value that the material property will have if the section is collapsed, optional (default: 0).</param>
        /// <param name="showValue">Float value that the material property will have if the section is visible, optional (default: 1).</param>
        [LimitAccessScope(typeof(OrderedSectionGroup))]
        public OrderedSection(string activatePropertyName, string showPropertyName,
        float hideValue = 0, float showValue = 1) : base(showPropertyName, hideValue, showValue)
        {
            AdditionalProperties = new AdditionalProperty[1];
            AdditionalProperties[0] = new AdditionalProperty(activatePropertyName);

            UpIcon = Styles.UpIcon;
            DownIcon = Styles.DownIcon;
            DeleteIcon = Styles.DeleteIcon;
            UpColor = Color.white;
            DownColor = Color.white;
            DeleteColor = Color.white;
        }

        /// <summary>
        /// Default constructor of <see cref="OrderedSection"/>.
        /// </summary>
        /// <param name="activatePropertyName">Material property that will drive the section enable state</param>
        [LimitAccessScope(typeof(OrderedSectionGroup))]
        public OrderedSection(string activatePropertyName) : base()
        {
            AdditionalProperties = new AdditionalProperty[1];
            AdditionalProperties[0] = new AdditionalProperty(activatePropertyName);

            ControlAlias = activatePropertyName;

            UpIcon = Styles.UpIcon;
            DownIcon = Styles.DownIcon;
            DeleteIcon = Styles.DeleteIcon;
            UpColor = Color.white;
            DownColor = Color.white;
            DeleteColor = Color.white;
        }

        protected void DrawSideButtons()
        {
            Color bgcolor = GUI.backgroundColor;
            GUI.backgroundColor = UpColor;
            isUp = EditorGUILayout.Toggle(isUp, UpIcon, GUILayout.Width(14.0f), GUILayout.Height(14.0f));
            GUI.backgroundColor = DownColor;
            isDown = EditorGUILayout.Toggle(isDown, DownIcon, GUILayout.Width(14.0f), GUILayout.Height(14.0f));
            if (isUp)
            {
                PushState = -1;
                isUp = false;
            }
            else if (isDown)
            {
                PushState = 1;
                isDown = false;
            }

            EditorGUI.BeginChangeCheck();
            GUI.backgroundColor = DeleteColor;
            Enabled = EditorGUILayout.Toggle(Enabled, DeleteIcon, GUILayout.MaxWidth(14.0f), GUILayout.Height(14.0f));
            if (!Enabled)
            {
                AdditionalProperties[0].Property.floatValue = 0;
            }
            HasActivatePropertyUpdated = EditorGUI.EndChangeCheck();
            GUI.backgroundColor = bgcolor;
        }

        public void PredrawUpdate(MaterialEditor materialEditor)
        {
            SetupEnabled(materialEditor);
            Enabled = AdditionalProperties[0].Property.floatValue > 0;
            HasActivatePropertyUpdated = false;
        }

        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            //ActivateProperty = FindProperty(ActivatePropertyName, properties);
            EditorGUILayout.Space();
            // Begin header
            GUI.backgroundColor = BackgroundColor;
            EditorGUILayout.BeginVertical(BackgroundStyle);
            GUI.backgroundColor = SimpleShaderInspector.DefaultBgColor;

            Rect r = EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginChangeCheck();
            if (ShowFoldoutArrow)
                Show = EditorGUILayout.Toggle(Show, EditorStyles.foldout, GUILayout.MaxWidth(15.0f));
            
            Rect temp = GUILayoutUtility.GetLastRect();
            Rect r2 = new Rect(r.x + temp.width, r.y, r.width - (temp.width * 2), r.height);
            /*
            EditorGUI.BeginChangeCheck();
            Enabled = EditorGUILayout.Toggle(Enabled, GUILayout.MaxWidth(20.0f));
            HasActivatePropertyUpdated = EditorGUI.EndChangeCheck();
            */
            EditorGUI.LabelField(r2, Content, LabelStyle);
            GUILayout.FlexibleSpace();

            DrawSideButtons();

            if (HasSectionTurnedOn)
                HasActivatePropertyUpdated = true;
            
            HasSectionTurnedOn = false;

            Show = GUI.Toggle(r, Show, GUIContent.none, new GUIStyle());
            HasPropertyUpdated = EditorGUI.EndChangeCheck();
            if (HasPropertyUpdated)
                UpdateEnabled(materialEditor);

            /*if (HasActivatePropertyUpdated)
            {
                materialEditor.RegisterPropertyChangeUndo(ActivateProperty.displayName);
                ActivateProperty.floatValue = Enabled ? enableValue : disableValue;
            }*/
            EditorGUILayout.EndHorizontal();

            if (!AreControlsInHeader)
                EditorGUILayout.EndVertical();
            
            if (Show)
                DrawControls(materialEditor);
            
            if (AreControlsInHeader)
                EditorGUILayout.EndVertical();
        }
    }
}