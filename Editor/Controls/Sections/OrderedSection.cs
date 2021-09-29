using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls.Sections
{
    /// <summary>
    /// Section that can be hidden and ordered when in groups.
    /// </summary>
    /// <remarks>
    /// <para>It can be considered an additional evolution of <see cref="ActivatableSection"/>, but it actually inherits directly from <see cref="Section"/>.</para>
    /// <para>
    /// Functionality wise it acts pretty much the same as <see cref="ActivatableSection"/>, the main difference is in how it does that. Namely instead of having a disabled state,
    /// it gets disabled it completely disappears from the ui, resulting in an overall cleaner ui.
    /// </para>
    /// <para>
    /// Unlike other types of sections, this one cannot be added directly to the inspector, instead it has to be added to an <see cref="OrderedSectionGroup"/> in order to work properly.
    /// In fact the only way to enable a previously disabled <see cref="OrderedSection"/> in the ui is by using the add button provided by <see cref="OrderedSectionGroup"/>
    /// </para>
    /// <para>
    /// Another quirk of this type of section is that it can be moved up or down relative to the sections of the same group, letting the user order them in whichever way they see fit best.
    /// </para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// // Adds an OrderedSection to the current inspector
    /// OrderedSectionGroup group = this.AddOrderedSectionGroup("GroupAlias"); 
    ///
    /// // Adds an OrderedSection using the specified property as activation property
    /// group.AddOrderedSection("_ActivateProperty");  
    /// 
    ///  // Adds an OrderedSection using the specified property as activation property,
    ///  // the values for activation are set to 2-3 respectively when disabled and enabled
    /// group.AddOrderedSection("_ActivateProperty", 2, 3); 
    ///
    /// // Adds an OrderedSection using the specified properties for activation and folding state
    /// group.AddOrderedSection("_ActivateProperty", "_ShowProperty"); 
    ///
    /// // Adds an OrderedSection using the specified properties for activation and folding state,
    /// // the values for activation are set to 2-3 respectively when disabled and enabled
    /// // the values for folding are set to 4-6 respectively when folded in and out
    /// group.AddOrderedSection("_ActivateProperty", "_ShowProperty", 2, 3, 4, 6); 
    /// </code>
    /// </example>
    public class OrderedSection : Section, IAdditionalProperties
    {
        /// <summary>
        /// Indicates if the section should be pushed up or down relative to its neighbour sections.
        /// </summary>
        /// <value>0 when not moving, -1 when needs to go up, 1 when needs to go down.</value>
        public int PushState;
        private bool _isUp;
        private bool _isDown;

        /// <summary>
        /// Extra properties array. Implementation needed by <see cref="IAdditionalProperties"/>.
        /// </summary>
        /// <value>
        /// Array of <see cref="AdditionalProperties"/>.
        /// </value>
        /// <remarks>
        /// The Array will contain the following material properties:
        /// <list type="bullet">
        /// <item>
        /// <term>[0]: </term>
        /// <description>Property used for the enabled state</description>
        /// </item>
        /// </list>
        /// </remarks>
        public AdditionalProperty[] AdditionalProperties { get; set; }

        /// <summary>
        /// Boolean indicating if the activate property has been updated this cycle.
        /// </summary>
        /// <value>
        /// True if the activate property has been updated, false otherwise.
        /// </value>
        public bool HasActivatePropertyUpdated { get; protected set; }

        /// <summary>
        /// Boolean indicating if the section has turned on this cycle.
        /// </summary>
        /// <value>
        /// True if the section has just been turned on, false otherwise.
        /// </value>
        public bool HasSectionTurnedOn { get; set; }

        /// <summary>
        /// Boolean indicating if the section is enabled or not.
        /// </summary>
        /// <value>
        /// True if the section is enabled, false otherwise.
        /// </value>
        public bool Enabled { get; protected set; }


        private int _sectionPosition;
        /// <summary>
        /// integer indicating the position of the section in the group.
        /// </summary>
        /// <value></value>
        public int SectionPosition 
        {
            get => _sectionPosition;
            set
            {
                _sectionPosition = value;
                if (string.IsNullOrWhiteSpace(_positionDictionaryKey))
                    _positionDictionaryKey = $"{ControlAlias}_{AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Inspector.Materials[0]))}_SectionPosition";
                StaticDictionaries.IntDictionary.SetValue(_positionDictionaryKey, _sectionPosition);
            }
        }

        /// <summary>
        /// Style for the up icon.
        /// </summary>
        /// <value>
        /// GUIStyle used for the up icon display.
        /// </value>
        [Chainable] public GUIStyle UpIcon { get; set; }

        /// <summary>
        /// Style for the down icon.
        /// </summary>
        /// <value>
        /// GUIStyle used for the down icon display.
        /// </value>
        [Chainable] public GUIStyle DownIcon { get; set; }

        /// <summary>
        /// Style for the delete icon.
        /// </summary>
        /// <value>
        /// GUIStyle used for the delete icon display.
        /// </value>
        [Chainable] public GUIStyle DeleteIcon { get; set; }

        /// <summary>
        /// Color of the up icon.
        /// </summary>
        /// <value>
        /// Color used to display the up icon.
        /// </value>
        [Chainable] public Color UpColor { get; set; }

        /// <summary>
        /// Color of the down icon.
        /// </summary>
        /// <value>
        /// Color used to display the down icon.
        /// </value>
        [Chainable] public Color DownColor { get; set; }

        /// <summary>
        /// Color of the delete icon.
        /// </summary>
        /// <value>
        /// Color used to display the down icon.
        /// </value>
        [Chainable] public Color DeleteColor { get; set; }

        /// <summary>
        /// Float value that the Show bool gets converted if true.
        /// </summary>
        /// <value>
        /// Value of the activate material property when the section is enabled.
        /// </value>
        protected float enableValue;

        /// <summary>
        /// Float value that the Show bool gets converted if false.
        /// </summary>
        /// <value>
        /// Value of the activate material property when the section is disabled.
        /// </value>
        protected float disableValue;

        private string _positionDictionaryKey;

        /// <summary>
        /// Constructor of <see cref="OrderedSection"/> used when creating a property driven OrderedSection.
        /// </summary>
        /// <param name="activatePropertyName">Material property that will drive the section enable state</param>
        /// <param name="showPropertyName">Material property that will drive the section open state</param>
        /// <param name="showValue">Float value that the material property will have if the section is visible, optional (default: 1).</param>
        /// <param name="hideValue">Float value that the material property will have if the section is collapsed, optional (default: 0).</param>
        /// <param name="enableValue">Float value that the material property will have if the section is enabled, optional (default: 1).</param>
        /// <param name="disableValue">Float value that the material property will have if the section is disabled, optional (default: 0).</param>
        [LimitAccessScope(typeof(OrderedSectionGroup))]
        public OrderedSection(string activatePropertyName, string showPropertyName, float enableValue = 1,
            float disableValue = 0, float showValue = 1, float hideValue = 0) : base(showPropertyName, hideValue, showValue)
        {
            AdditionalProperties = new AdditionalProperty[1];
            AdditionalProperties[0] = new AdditionalProperty(activatePropertyName);

            UpIcon = Styles.UpIcon;
            DownIcon = Styles.DownIcon;
            DeleteIcon = Styles.DeleteIcon;
            UpColor = Color.white;
            DownColor = Color.white;
            DeleteColor = Color.white;

            this.disableValue = disableValue;
            this.enableValue = enableValue;
        }

        /// <summary>
        /// Default constructor of <see cref="OrderedSection"/>.
        /// </summary>
        /// <param name="activatePropertyName">Material property that will drive the section enable state</param>
        /// <param name="enableValue">Float value that the material property will have if the section is enabled, optional (default: 1).</param>
        /// <param name="disableValue">Float value that the material property will have if the section is disabled, optional (default: 0).</param>
        [LimitAccessScope(typeof(OrderedSectionGroup))]
        public OrderedSection(string activatePropertyName, float enableValue = 1, float disableValue = 0)
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

            this.disableValue = disableValue;
            this.enableValue = enableValue;
        }

        /// <summary>
        /// Draws and handles the up, down and delete icon on the side.
        /// </summary>
        protected void DrawSideButtons()
        {
            Color bgColor = GUI.backgroundColor;
            GUI.backgroundColor = UpColor;
            _isUp = EditorGUILayout.Toggle(_isUp, UpIcon, GUILayout.Width(14.0f), GUILayout.Height(14.0f));
            GUI.backgroundColor = DownColor;
            _isDown = EditorGUILayout.Toggle(_isDown, DownIcon, GUILayout.Width(14.0f), GUILayout.Height(14.0f));
            if (_isUp)
            {
                PushState = -1;
                _isUp = false;
            }
            else if (_isDown)
            {
                PushState = 1;
                _isDown = false;
            }

            EditorGUI.BeginChangeCheck();
            GUI.backgroundColor = DeleteColor;
            Enabled = EditorGUILayout.Toggle(Enabled, DeleteIcon, GUILayout.MaxWidth(14.0f), GUILayout.Height(14.0f));
            if (!Enabled)
            {
                AdditionalProperties[0].Property.floatValue = 0;
                SectionPosition = 0;
            }
            HasActivatePropertyUpdated = EditorGUI.EndChangeCheck();
            GUI.backgroundColor = bgColor;
        }

        public void PredrawUpdate(MaterialEditor materialEditor)
        {
            SetupEnabled(materialEditor);

            if (string.IsNullOrWhiteSpace(_positionDictionaryKey))
            {
                _positionDictionaryKey = $"{ControlAlias}_{AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(Inspector.Materials[0]))}_SectionPosition";

                if (StaticDictionaries.IntDictionary.TryGetValue(_positionDictionaryKey, out int position))
                {
                    _sectionPosition = position;
                }
                else
                {
                    _sectionPosition = 0;
                    StaticDictionaries.IntDictionary.SetValue(_positionDictionaryKey, _sectionPosition);
                }
                
                if(Math.Abs(AdditionalProperties[0].Property.floatValue - disableValue) > 0.001 && _sectionPosition == 0)
                    _sectionPosition = 753;

                else if(Math.Abs(AdditionalProperties[0].Property.floatValue - disableValue) < 0.001 && _sectionPosition > 0)
                    _sectionPosition = 0;
            }

            Enabled = SectionPosition > 0;
            AdditionalProperties[0].Property.floatValue = Enabled ? enableValue : disableValue;
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