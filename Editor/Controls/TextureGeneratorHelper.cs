using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors.Controls
{
    /// <summary>
    /// Helper class for TextureGeneratorControl.
    /// </summary>
    public static class TextureGeneratorHelper
    {
        // Get the kernel and relative inputs from a json string.
        internal static (string, List<ComputeInputBase>) GetInputs(string settingsJson)
        {
            TextureGeneratorSettings settings = JsonUtility.FromJson<TextureGeneratorSettings>(settingsJson);
            var inputs = new List<ComputeInputBase>();
            foreach (var input in settings.Inputs)
            {
                switch (input.Type)
                {
                    case InputType.Texture:
                        inputs.Add(new ComputeTextureInput
                        {
                            InputName = input.InputName,
                            UseSpecificChannel = input.Settings[0] == 1,
                            UseInvert = input.Settings[1] == 1
                        });
                        break;
                    case InputType.Color:
                        inputs.Add(new ComputeColorInput
                        {
                            InputName = input.InputName,
                            UseColorspace = input.Settings[0] == 1
                        });
                        break;
                }
            }

            return (settings.KernelName, inputs);
        }
    }
    /// <summary>
    /// Type of input for the texture generator.
    /// </summary>
    public enum InputType
    {
        Texture,
        Color
    }
#pragma warning disable 0649 //disabled warning here since the object is serializable and will be serialized by loading json files. 
    /// <summary>
    /// Json settings file structure.
    /// </summary>
    [Serializable]
    internal class TextureGeneratorSettings
    {
        public string KernelName;
        public TextureGeneratorInput[] Inputs;
    }
    /// <summary>
    /// Substructure of the Json settings file indicating a single input for the generator.
    /// </summary>
    [Serializable]
    internal class TextureGeneratorInput
    {
        public InputType Type;
        public string InputName;
        public int[] Settings;
    }
#pragma warning restore 0649

    internal class ComputeInputs
    {
        internal struct TextureData
        {
            public string Name;
            public Texture2D Texture;
        }
        internal struct TextureMetadata
        {
            public float Width;
            public float Height;
            public float SelectedChannel;
            public float Reverse;
            public float Gamma;
        }

        public List<TextureData> Textures;
        public List<TextureMetadata> TexturesMeta;
        public List<Vector4> Colors;

        public ComputeInputs()
        {
            Textures = new List<TextureData>();
            TexturesMeta = new List<TextureMetadata>();
            Colors = new List<Vector4>();
        }
    }

    internal abstract class ComputeInputBase
    {
        public string InputName;
        internal abstract void AssignInputsToCompute(ComputeInputs inputs, int kernel);

        public abstract void InputGUI(GUIContent name, AdditionalLocalization[] extraContent);
    }

    internal class ComputeTextureInput : ComputeInputBase
    {
        public Texture2D Texture;
        internal DefaultTexture Default;

        public int Channel;
        public bool UseSpecificChannel;

        public bool Invert;
        public bool UseInvert;

        public ComputeTextureInput()
        {
            UseSpecificChannel = false;
            UseInvert = false;
        }

        internal override void AssignInputsToCompute(ComputeInputs inputs, int kernel)
        {
            if (!UseSpecificChannel) Channel = -1;
            if (!UseInvert) Invert = false;

            Texture2D def = null;
            if (Texture == null)
            {
                switch (Default)
                {
                    case DefaultTexture.DefaultWhite:
                        def = Texture2D.whiteTexture;
                        break;
                    case DefaultTexture.DefaultBlack:
                        def = Texture2D.blackTexture;
                        break;
                }
            }
            ComputeInputs.TextureMetadata textureMetadata = new ComputeInputs.TextureMetadata
            {
                Width = def?.width ?? Texture.width,
                Height = def?.height ?? Texture.height,
                SelectedChannel = Channel,
                Reverse = Invert ? 1 : 0,
                Gamma = IsGamma()
            };

            ComputeInputs.TextureData textureData = new ComputeInputs.TextureData
            {
                Name = InputName,
                Texture = def ?? Texture
            };

            inputs.Textures.Add(textureData);
            inputs.TexturesMeta.Add(textureMetadata);
        }

        public override void InputGUI(GUIContent name, AdditionalLocalization[] extraContent)
        {
            EditorGUI.LabelField(GUILayoutUtility.GetRect(100, 16), name);
            var rect = GUILayoutUtility.GetRect(80, 80);
            rect.x += 10; rect.y += 5;
            rect.width -= 20; rect.height -= 10;
            Texture = (Texture2D)EditorGUI.ObjectField(rect, Texture, typeof(Texture2D), false);
            if (UseSpecificChannel) Channel = GUILayout.Toolbar(Channel, new string[] { "R", "G", "B", "A" }); //extraContent[0].Content,
            if (UseInvert)
            {
                rect = GUILayoutUtility.GetRect(100, 16);
                GUI.Label(rect, extraContent[0].Content);
                rect.x += 84;
                Invert = EditorGUI.Toggle(rect, Invert);
                GUILayout.Space(4);
            }
            Default = (DefaultTexture)EditorGUI.EnumPopup(GUILayoutUtility.GetRect(100, 16), Default);
        }
        private float IsGamma()
        {
            string assetPath = AssetDatabase.GetAssetPath(Texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                return tImporter.sRGBTexture ? 1 : 0;
            }
            return 0;
        }
    }

    internal class ComputeColorInput : ComputeInputBase
    {
        public Color Color;
        public int Colorspace;
        public bool UseColorspace;

        public void ComputeTextureInput()
        {
            UseColorspace = false;
        }

        internal override void AssignInputsToCompute(ComputeInputs inputs, int kernel)
        {
            //compute.SetTexture(kernel, channelName, Texture);
            //compute.SetFloat(TextureWidthInputName, Texture.width);
            if (UseColorspace)
            {
                inputs.Colors.Add(Colorspace == 0 ? Color : Color.gamma);
            }
            else
            {
                inputs.Colors.Add(Color.gamma);
            }
        }

        public override void InputGUI(GUIContent name, AdditionalLocalization[] extraContent)
        {
            GUILayout.Label(name);
            EditorGUILayout.ColorField(Color);
            if (UseColorspace) Colorspace = GUILayout.Toolbar(Colorspace, new string[] { "Linear", "Gamma" }, EditorStyles.miniLabel); //extraContent[1].Content
        }
    }

    internal enum DefaultTexture
    {
        DefaultWhite,
        DefaultBlack
    }
}