using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Control for viewing and setting particle vertex streams used
    /// </summary>
    /// <remarks>
    /// <para>
    /// When making particle shaders there may be a need to set the particles vertex streams so that the shader can access the data it needs.
    /// Using this control you can easily view which vertex streams are needed and set them in the particle systems that have a material using this shader, in a similar way the
    /// standard unity particle shader inspector does.
    /// </para>
    /// <para>Since the control does not use any material property, it requires an alias in order to work.</para>
    /// </remarks>
    /// <example>
    /// Example usage:
    /// <code>
    /// this.AddVertexStreamsControl("AliasToUse");
    /// </code>
    /// </example>
    public class VertexStreamsControl : SimpleControl, IAdditionalLocalization
    {
        /// <summary>
        /// Additional localization needed by the control, Implementation required by the <see cref="IAdditionalLocalization"/> interface.
        /// </summary>
        /// <value>
        /// An array of <see cref="AdditionalLocalization"/>
        /// </value>
        /// <remarks>
        /// The array has the following values:
        /// - [0] : ApplyVertexStreamsButton
        /// </remarks>
        public AdditionalLocalization[] AdditionalContent { get; set; }

        private List<ParticleSystemRenderer> _renderers;
        private List<ParticleSystemVertexStream> _streams = new List<ParticleSystemVertexStream>();
        
        /// <summary>
        /// Default constructor of <see cref="VertexStreamsControl"/>.
        /// </summary>
        /// <param name="alias">Alias needed by the control</param>
        public VertexStreamsControl(string alias) : base(alias)
        {
            AdditionalContent = new AdditionalLocalization[1];
            AdditionalContent[0] = new AdditionalLocalization { Name = "ApplyVertexStreamsButton" };
        }
        
        /// <summary>
        /// Draws the control represented by this object.
        /// </summary>
        /// <param name="materialEditor">Material editor.</param>
        protected override void ControlGUI(MaterialEditor materialEditor)
        {
            if (_renderers == null)
            {
                _renderers = new List<ParticleSystemRenderer>();
                CacheRenderers(materialEditor.target as Material);
            }

            GUILayout.Label(Content, EditorStyles.boldLabel);

            foreach (var stream in _streams)
            {
                GUILayout.Label(Enum.GetName(typeof(ParticleSystemVertexStream), stream));
            }
            if (materialEditor.targets.Length > 1) return;
            if (GUILayout.Button(AdditionalContent[0].Content, EditorStyles.miniButton, GUILayout.ExpandWidth(false)))
            {
                Undo.RecordObjects(_renderers.Where(r => r != null).ToArray(), AdditionalContent[0].Content.text);

                foreach (var renderer in _renderers)
                    if(renderer != null)
                        renderer.SetActiveVertexStreams(_streams);
            }
        }
        
        /// <summary>
        /// Adds a vertex stream to the list of used streams.
        /// </summary>
        /// <param name="stream">Vertex stream to add.</param>
        /// <returns>This control.</returns>
        public VertexStreamsControl AddVertexStream(ParticleSystemVertexStream stream)
        {
            if (_streams.Contains(stream)) return this;
            _streams.Add(stream);
            _streams = _streams.OrderBy(x => x).ToList();
            return this;
        }
        
        /// <summary>
        /// Removes a vertex stream from the list of used streams.
        /// </summary>
        /// <param name="stream">Vertex stream to remove.</param>
        /// <returns>This control.</returns>
        public VertexStreamsControl RemoveVertexStream(ParticleSystemVertexStream stream)
        {
            _streams.Remove(stream);
            return this;
        }

        private void CacheRenderers(Material material)
        {
            ParticleSystemRenderer[] renderers = Resources.FindObjectsOfTypeAll(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
            foreach (var renderer in renderers)
            {
                var go = renderer.gameObject;
                if (go.hideFlags == HideFlags.NotEditable || go.hideFlags == HideFlags.HideAndDontSave)
                    continue;

                if (renderer.sharedMaterial == material)
                    _renderers.Add(renderer);
            }
        }
    }
}