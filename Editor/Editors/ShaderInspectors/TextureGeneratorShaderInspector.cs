using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Base class for creating new inspectors for shaders meant to be used in the texture generator control
    /// </summary>
    /// <remarks>
    /// The <see cref=" VRLabs.SimpleShaderInspectors.Controls.TextureGeneratorControl"/> uses shaders outputs to create textures,
    /// and uses their inspectors as options ui.
    /// Using inspectors inherited from this one will let the control get and give some needed data, as well as letting the user know that the shader is not supposed to be used for a material.
    /// </remarks>
    public abstract class TextureGeneratorShaderInspector : ShaderGUI, ISimpleShaderInspector
    {
        private bool _isFirstLoop = true;
        private bool _doesContainControls = true;
        private List<INonAnimatableProperty> _nonAnimatablePropertyControls;
        private bool ContainsNonAnimatableProperties => _nonAnimatablePropertyControls.Count > 0;
        internal bool isFromGenerator = false;

        internal readonly List<Shader> shaderStack = new List<Shader>();
        internal PropertyInfo[] stackedInfo;

        /// <summary>
        /// List of controls.
        /// </summary>
        /// <value>
        /// List of control that the inspector had to draw.
        /// </value>
        public List<SimpleControl> Controls { get; set; }
        
        /// <summary>
        /// Array of selected materials
        /// </summary>
        /// <value>
        ///Array containing the materials currently selected by the inspector.
        /// </value>
        public Material[] Materials { get; private set; }
        
        /// <summary>
        /// Shader currently used
        /// </summary>
        /// <value>
        /// Contains the shader this inspector is viewing at the moment.
        /// </value>
        public Shader Shader { get; private set; }
        
        /// <summary>
        /// Initialization method where all the controls are instanced. You need to override it.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method is used the same way as <see cref="SimpleShaderInspector.Start"/>.
        /// </para>
        /// </remarks>
        protected abstract void Start();
        
        /// <summary>
        /// Checks done on the first cycle before UI is drawn.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        /// <remarks>
        /// This method is used the same way as <see cref="SimpleShaderInspector.StartChecks"/>.
        /// </remarks>
        protected virtual void StartChecks(MaterialEditor materialEditor) { }

        /// <summary>
        /// Check changes happened to properties.
        /// </summary>
        /// <param name="materialEditor">material editor that uses this GUI.</param>
        /// <remarks>
        /// This method is used the same way as <see cref="SimpleShaderInspector.CheckChanges"/>.
        /// </remarks>
        protected virtual void CheckChanges(MaterialEditor materialEditor) { }
        
        
        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (!isFromGenerator)
            {
                EditorGUILayout.HelpBox("This shader is meant to be used inside a Texture Generator in the Simple Shader Inspectors library, and not be used for a material, please select another shader", MessageType.Error);
                return;
            }
            
            if (_isFirstLoop)
            {
                Setup(materialEditor, properties);
            }
            else
            {
                Controls.FetchProperties(properties);
            }
            
            DrawGUI(materialEditor, properties);
            if (ContainsNonAnimatableProperties)
                SSIHelper.UpdateNonAnimatableProperties(_nonAnimatablePropertyControls, materialEditor, false);
            
            CheckChanges(materialEditor);
        }

        internal void Setup(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            Controls = new List<SimpleControl>();
            Materials = Array.ConvertAll(materialEditor.targets, item => (Material)item);
            Shader = Materials[0].shader;
            Start();
            //LoadLocalizations();
            Controls.SetInspector(this);
            Controls.Initialize();
            _nonAnimatablePropertyControls = (List<INonAnimatableProperty>)Controls.FindNonAnimatablePropertyControls();
            Controls.FetchProperties(properties);
            StartChecks(materialEditor);
            _isFirstLoop = false;
            if (Controls == null || Controls.Count == 0)
                _doesContainControls = false;
        }

        private void DrawGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
        {
            if (_doesContainControls)
            {
                // Draw controls
                foreach (var control in Controls)
                    control.DrawControl(materialEditor);
            }
            else
            {
                // In a working inspector it should never appear.
                EditorGUILayout.HelpBox("No controls have been passed to the Start() method, therefore a default inspector has been drawn, if you are an end user of the shader try to reinstall the shader or contact the creator.", MessageType.Error);
                base.OnGUI(materialEditor, properties);
            }
        }

        internal void SetShaderLocalizationFromGenerator(PropertyInfo[] propertyInfos)
        {
            stackedInfo = propertyInfos;
            Localization.SetPropertiesLocalization(Controls, propertyInfos, new List<PropertyInfo>());
        }

        internal List<AdditionalLocalization> GetRequiredLocalization()
        {
            var elements = new List<AdditionalLocalization>();
            foreach (var control in Controls)
            {
                elements.AddRange(GetControlLocalization(control));
            }

            return elements;
        }

        private static List<AdditionalLocalization> GetControlLocalization(SimpleControl control)
        {
            var elements = new List<AdditionalLocalization>();
            var element = new AdditionalLocalization();
            element.Name = control.ControlAlias;
            
            elements.Add(element);
                
            // Find additional content in case it implements the IAdditionalLocalization interface.
            if (control is IAdditionalLocalization additional)
            {
                if (additional is TextureGeneratorControl texture)
                {
                    //if(!texture._isCircularReference)
                        elements.AddRange(texture.namesContent.Select(x => new AdditionalLocalization{Name = $"{x.Name}", Content = null}));
                        elements.AddRange(texture.baseContent.Select(x => new AdditionalLocalization { Name = $"{control.ControlAlias}_{x.Name}", Content = null }));
                }
                else
                {
                    elements.AddRange(additional.AdditionalContent.Select(x => new AdditionalLocalization { Name = $"{control.ControlAlias}_{x.Name}", Content = null }));
                }
            }
                

            // Recursively set property localization for all properties inside this control if it has the IControlContainer interface.
            if(control is IControlContainer container)
                foreach (var childControl in container.GetControlList())
                    elements.AddRange(GetControlLocalization(childControl));

            return elements;
        }
        
        public void AddControl(SimpleControl control, string alias = "") => Controls.AddControl(control, alias);

        public IEnumerable<SimpleControl> GetControlList() => Controls;
    }
}