using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace VRLabs.SimpleShaderInspectors
{
    public static class SSIHelper
    {
        /// <summary>
        /// Fetches properties for all the given controls.
        /// </summary>
        /// <param name="controls">Controls needing to fetch properties.</param>
        /// <param name="properties">Property array to fetch properties from.</param>
        public static void FetchProperties(this IEnumerable<SimpleControl> controls, MaterialProperty[] properties)
        {
            foreach (var control in controls)
            {
                if (control is PropertyControl pr)
                    pr.FetchProperty(properties);

                if (control is IAdditionalProperties add)
                {
                    for (int j = 0; j < add.AdditionalProperties.Length; j++)
                        add.AdditionalProperties[j].FetchProperty(properties);
                }

                if (control is IControlContainer con)
                    con.Controls.FetchProperties(properties);
            }
        }

        /// <summary>
        /// Set the inspector of each control of the list.
        /// </summary>
        /// <param name="controls">Controls this method extends from.</param>
        /// <param name="inspector">Inspector to set</param>
        /// <param name="recursive">Is the set recursive to child controls</param>
        public static void SetInspector(this IEnumerable<SimpleControl> controls, ISimpleShaderInspector inspector, bool recursive = true)
        {
            foreach (var control in controls)
            {
                control.Inspector = inspector;
                if (recursive && control is IControlContainer con)
                    con.Controls.SetInspector(inspector);
            }
        }

        /// <summary>
        /// Find a material property from its name.
        /// </summary>
        /// <param name="propertyName">Name of the material proeperty.</param>
        /// <param name="properties">Array of material properties to search from.</param>
        /// <param name="propertyIsMandatory">Boolean indicating if it's mandatory to find the requested material property</param>
        /// <returns>The material property with the wanted name.</returns>
        internal static int FindPropertyIndex(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory = false)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i] != null && properties[i].name == propertyName)
                    return i;
            }

            // We assume all required properties can be found, otherwise something is broken.
            if (propertyIsMandatory)
                throw new ArgumentException("Could not find MaterialProperty: '" + propertyName + "', Num properties: " + properties.Length);

            return -1;
        }

        /// <summary>
        /// Finds all controls that implement the INonAnimatableProperty interface.
        /// </summary>
        /// <param name="controls">Controls to search from</param>
        /// <returns>An enumerable containing all INonAnimarableProperty instances found</returns>
        public static IEnumerable<INonAnimatableProperty> FindNonAnimatablePropertyControls(this IEnumerable<SimpleControl> controls)
        {
            List<INonAnimatableProperty> nonAnimatablePropertyControls = new List<INonAnimatableProperty>();
            foreach(var control in controls)
            {
                if(control is INonAnimatableProperty c)
                    nonAnimatablePropertyControls.Add(c);

                if(control is IControlContainer container)
                    nonAnimatablePropertyControls.AddRange(container.Controls.FindNonAnimatablePropertyControls());
            }
            return nonAnimatablePropertyControls;
        }

        /// <summary>
        /// Updates properties that are set to not be recorded during animation recording.
        /// </summary>
        /// <param name="controls">Controls to check.</param>
        /// <param name="materialEditor">Material editor.</param>
        /// <param name="updateOutsideAnimation">If the animations will actually be changed outside of animation recording</param>
        public static void UpdateNonAnimatableProperties(IEnumerable<INonAnimatableProperty> controls, MaterialEditor materialEditor, bool updateOutsideAnimation = true)
        {
            List<INonAnimatableProperty> propertiesNeedingUpdate = new List<INonAnimatableProperty>();
            foreach(var control in controls)
            {
                if (control.NonAnimatablePropertyChanged)
                    propertiesNeedingUpdate.Add(control);
            }

            if (propertiesNeedingUpdate.Count == 0) return;

            if (updateOutsideAnimation)
            {
                // Reflection bs to get which animation window is recording
                System.Reflection.Assembly editorAssembly = typeof(Editor).Assembly;
                Type windowType = editorAssembly.GetType("UnityEditorInternal.AnimationWindowState");

                System.Reflection.PropertyInfo isRecordingProp = windowType.GetProperty
                    ("recording", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                UnityEngine.Object[] windowInstances = Resources.FindObjectsOfTypeAll(windowType);
                UnityEngine.Object recordingInstance = null;

                for (int i = 0; i < windowInstances.Length; i++)
                {
                    bool isRecording = (bool)isRecordingProp.GetValue
                        (windowInstances[i], null);

                    if (isRecording)
                    {
                        recordingInstance = windowInstances[i];
                        break;
                    }
                }
                if (recordingInstance != null)
                {
                    System.Reflection.MethodBase stopRecording = windowType.GetMethod
                        ("StopRecording", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
                    System.Reflection.MethodBase startRecording = windowType.GetMethod
                        ("StartRecording", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                    stopRecording?.Invoke(recordingInstance, null);
                    SetNonAnimatableProperties(materialEditor, propertiesNeedingUpdate);
                    startRecording?.Invoke(recordingInstance, null);
                }
                else
                {
                    SetNonAnimatableProperties(materialEditor, propertiesNeedingUpdate);
                }
            }
            else
            {
                for (int i = 0; i < propertiesNeedingUpdate.Count; i++)
                    SetNonAnimatableProperties(materialEditor, propertiesNeedingUpdate);
            }
        }

        public static void SetNonAnimatableProperties(MaterialEditor materialEditor, IEnumerable<INonAnimatableProperty> nonAnimatableProperties)
        {
            foreach(var nonAnimatableProperty in nonAnimatableProperties)
            {
                nonAnimatableProperty.UpdateNonAnimatableProperty(materialEditor);
                nonAnimatableProperty.NonAnimatablePropertyChanged = false;
            }
        }

        /// <summary>
        /// Get a path to save a texture relative to the material.
        /// </summary>
        /// <param name="mat">Material.</param>
        /// <param name="name">Name of the texture.</param>
        /// <returns>A path for the texture to save.</returns>
        public static string GetTextureDestinationPath(Material mat, string name)
        {
            string path = AssetDatabase.GetAssetPath(mat);
            path = Directory.GetParent(path).FullName;
            string pathParent = Directory.GetParent(path).FullName;

            if (Directory.Exists(pathParent + "/Textures/"))
                return pathParent + "/Textures/" + mat.name + name;
            else
                return path + "/" + mat.name + name;
        }

        /// <summary>
        /// Saves a texture to a specified path.
        /// </summary>
        /// <param name="texture">Texture to save.</param>
        /// <param name="path">path where you want to save the texture.</param>
        /// <param name="mode">Texture wrap mode (default: Repeat).</param>
        public static void SaveTexture(Texture2D texture, string path, TextureWrapMode mode = TextureWrapMode.Repeat)
        {
            byte[] bytes = texture.EncodeToPNG();

            System.IO.File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            path = path.Substring(path.LastIndexOf("Assets"));
            TextureImporter t = AssetImporter.GetAtPath(path) as TextureImporter;
            t.wrapMode = mode;
            t.isReadable = true;
            AssetDatabase.ImportAsset(path);
        }

        /// <summary>
        /// Saves a texture to a specified path, and returns a reference of the new asset.
        /// </summary>
        /// <param name="texture">Texture to save.</param>
        /// <param name="path">path where you want to save the texture.</param>
        /// <param name="mode">Texture wrap mode (default: Repeat).</param>
        /// <returns>A Texture2D that references the newly created asset.</returns>
        public static Texture2D SaveAndGetTexture(Texture2D texture, string path, TextureWrapMode mode = TextureWrapMode.Repeat)
        {
            SaveTexture(texture, path, mode);
            path = path.Substring(path.LastIndexOf("Assets"));
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>
        /// Set the texture readable state.
        /// </summary>
        /// <param name="texture">Texture</param>
        /// <param name="isReadable">Does the texture need to be readable.</param>
        public static void SetTextureImporterReadable(Texture2D texture, bool isReadable)
        {
            if (texture == null) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.Default;
                tImporter.isReadable = isReadable;
                AssetDatabase.ImportAsset(assetPath);
                //AssetDatabase.Refresh();
            }
        }

        public static void SetTextureImporterAlpha(Texture2D texture, bool alphaIsTransparency)
        {
            if (texture == null) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter != null)
            {
                tImporter.textureType = TextureImporterType.Default;
                tImporter.alphaIsTransparency = alphaIsTransparency;
                AssetDatabase.ImportAsset(assetPath);
            }
        }
    }
}