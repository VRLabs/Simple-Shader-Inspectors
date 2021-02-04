using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    public class VertexStreamsControl : SimpleControl, IAdditionalLocalization
    {
        public AdditionalLocalization[] AdditionalContent { get; set; }

        private List<ParticleSystemRenderer> _renderers;
        private List<ParticleSystemVertexStream> _streams = new List<ParticleSystemVertexStream>();

        public VertexStreamsControl(string alias) : base(alias)
        {
            AdditionalContent = new AdditionalLocalization[1];
            AdditionalContent[0] = new AdditionalLocalization { Name = "ApplyVertexStreamsButton" };
        }

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

                foreach (ParticleSystemRenderer renderer in _renderers)
                    renderer?.SetActiveVertexStreams(_streams);
            }
        }

        public VertexStreamsControl AddVertexStream(ParticleSystemVertexStream stream)
        {
            if (!_streams.Contains(stream))
            {
                _streams.Add(stream);
                _streams = _streams.OrderBy(x => x).ToList();
            }
            return this;
        }

        public VertexStreamsControl RemoveVertexStream(ParticleSystemVertexStream stream)
        {
            _streams.Remove(stream);
            return this;
        }

        private void CacheRenderers(Material material)
        {
            ParticleSystemRenderer[] renderers = Resources.FindObjectsOfTypeAll(typeof(ParticleSystemRenderer)) as ParticleSystemRenderer[];
            foreach (ParticleSystemRenderer renderer in renderers)
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