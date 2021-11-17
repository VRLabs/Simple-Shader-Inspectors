using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

namespace VRLabs.SimpleShaderInspectors
{
    /// <summary>
    /// Static class containing utility functions for Controls and Inspectors.
    /// </summary>
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
                    foreach (var t in add.AdditionalProperties)
                        t.FetchProperty(properties);

                if (control is IControlContainer con) 
                    con.GetControlList().FetchProperties(properties);
            }
        }

        /// <summary>
        /// Fetches properties for all the given controls.
        /// </summary>
        /// <param name="controls">Controls needing to fetch properties.</param>
        /// <param name="properties">Property array to fetch properties from.</param>
        /// <param name="missingProperties">(Out) Properties defined in the inspector that are missing in the shader</param>
        public static void FetchProperties(this IEnumerable<SimpleControl> controls, MaterialProperty[] properties, out List<string> missingProperties)
        {
            var errs = new List<string>();
            foreach (var control in controls)
            {
                if (control is PropertyControl pr)
                {
                    pr.FetchProperty(properties);
                    if(pr.Property == null && !pr.PropertyName.Equals("SSI_UNUSED_PROP"))
                        errs.Add(pr.PropertyName);
                }

                if (control is IAdditionalProperties add)
                {
                    foreach (var t in add.AdditionalProperties)
                    {
                        t.FetchProperty(properties);
                        if(t.Property == null && t.IsPropertyMandatory)
                            errs.Add(t.PropertyName);
                    }
                }

                if (control is IControlContainer con)
                {
                    con.GetControlList().FetchProperties(properties, out List<string> ms);
                    errs.AddRange(ms);
                }
            }
            missingProperties = errs;
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
                    con.GetControlList().SetInspector(inspector);
            }
        }

        /// <summary>
        /// Find a material property from its name.
        /// </summary>
        /// <param name="propertyName">Name of the material property.</param>
        /// <param name="properties">Array of material properties to search from.</param>
        /// <param name="propertyIsMandatory">Boolean indicating if it's mandatory to find the requested material property</param>
        /// <returns>The material property with the wanted name.</returns>
        internal static int FindPropertyIndex(string propertyName, MaterialProperty[] properties, bool propertyIsMandatory = false)
        {
            if (!string.IsNullOrWhiteSpace(propertyName) && propertyName.Equals("SSI_UNUSED_PROP")) return -1;
            
            for (int i = 0; i < properties.Length; i++)
                if (properties[i] != null && properties[i].name == propertyName)
                    return i;

            // We assume all required properties can be found, otherwise something is broken.
            if (propertyIsMandatory)
                throw new ArgumentException("Could not find MaterialProperty: '" + propertyName + "', Num properties: " + properties.Length);

            return -1;
        }

        /// <summary>
        /// Finds all controls that implement the INonAnimatableProperty interface.
        /// </summary>
        /// <param name="controls">Controls to search from</param>
        /// <returns>An enumerable containing all INonAnimatableProperty instances found</returns>
        public static IEnumerable<INonAnimatableProperty> FindNonAnimatablePropertyControls(this IEnumerable<SimpleControl> controls)
        {
            List<INonAnimatableProperty> nonAnimatablePropertyControls = new List<INonAnimatableProperty>();
            foreach(var control in controls)
            {
                if(control is INonAnimatableProperty c)
                    nonAnimatablePropertyControls.Add(c);

                if(control is IControlContainer container)
                    nonAnimatablePropertyControls.AddRange(container.GetControlList().FindNonAnimatablePropertyControls());
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
                if (control.NonAnimatablePropertyChanged)
                    propertiesNeedingUpdate.Add(control);

            if (propertiesNeedingUpdate.Count == 0) return;

            if (updateOutsideAnimation)
            {
                // Reflection bs to get which animation window is recording
                var editorAssembly = typeof(Editor).Assembly;
                var windowType = editorAssembly.GetType("UnityEditorInternal.AnimationWindowState");

                var isRecordingProp = windowType.GetProperty
                    ("recording", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

                UnityEngine.Object[] windowInstances = Resources.FindObjectsOfTypeAll(windowType);
                UnityEngine.Object recordingInstance = null;

                foreach (var t in windowInstances)
                {
                    bool isRecording = isRecordingProp != null && (bool)isRecordingProp.GetValue
                        (t, null);

                    if (!isRecording) continue;
                    recordingInstance = t;
                    break;
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
                SetNonAnimatableProperties(materialEditor, propertiesNeedingUpdate);
            }
        }
        
        // TODO: set method private
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
            path = Directory.GetParent(path)?.FullName;
            string pathParent = Directory.GetParent(path)?.FullName;

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
        /// <param name="linear">If the texture should be saved as linear.</param>
        public static void SaveTexture(Texture2D texture, string path, TextureWrapMode mode = TextureWrapMode.Repeat, bool linear = false)
        {
            byte[] bytes = texture.EncodeToPNG();

            File.WriteAllBytes(path, bytes);
            AssetDatabase.Refresh();
            path = path.Substring(path.LastIndexOf("Assets", StringComparison.Ordinal));
            var t = AssetImporter.GetAtPath(path) as TextureImporter;
            if (t != null)
            {
                t.wrapMode = mode;
                t.isReadable = true;
                t.sRGBTexture = !linear;
            }

            AssetDatabase.ImportAsset(path);
        }

        /// <summary>
        /// Saves a texture to a specified path, and returns a reference of the new asset.
        /// </summary>
        /// <param name="texture">Texture to save.</param>
        /// <param name="path">path where you want to save the texture.</param>
        /// <param name="mode">Texture wrap mode (default: Repeat).</param>
        /// <returns>A Texture2D that references the newly created asset.</returns>
        /// <param name="linear">If the texture should be saved as linear</param>
        public static Texture2D SaveAndGetTexture(Texture2D texture, string path, TextureWrapMode mode = TextureWrapMode.Repeat, bool linear = false)
        {
            SaveTexture(texture, path, mode, linear);
            path = path.Substring(path.LastIndexOf("Assets", StringComparison.Ordinal));
            return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        }

        /// <summary>
        /// Set the texture readable state.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="isReadable">Does the texture need to be readable.</param>
        public static void SetTextureImporterReadable(Texture2D texture, bool isReadable)
        {
            if (texture is null) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter is null) return;
            
            tImporter.textureType = TextureImporterType.Default;
            tImporter.isReadable = isReadable;
            AssetDatabase.ImportAsset(assetPath);
            //AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// Check if the texture is in srgb.
        /// </summary>
        /// <param name="texture">Texture.</param>
        public static bool IsSrgb(this Texture2D texture)
        {
            switch (texture.graphicsFormat)
            {
                case GraphicsFormat.R8_SRGB:
                case GraphicsFormat.R8G8_SRGB:
                case GraphicsFormat.R8G8B8_SRGB:
                case GraphicsFormat.R8G8B8A8_SRGB:
                case GraphicsFormat.B8G8R8_SRGB:
                case GraphicsFormat.B8G8R8A8_SRGB:
                case GraphicsFormat.RGBA_DXT3_SRGB:
                case GraphicsFormat.RGBA_DXT5_SRGB:
                case GraphicsFormat.RGBA_BC7_SRGB:
                case GraphicsFormat.RGB_PVRTC_2Bpp_SRGB:
                case GraphicsFormat.RGB_PVRTC_4Bpp_SRGB:
                case GraphicsFormat.RGBA_PVRTC_2Bpp_SRGB:
                case GraphicsFormat.RGBA_PVRTC_4Bpp_SRGB:
                case GraphicsFormat.RGB_ETC2_SRGB:
                case GraphicsFormat.RGB_A1_ETC2_SRGB:
                case GraphicsFormat.RGBA_ETC2_SRGB:
                case GraphicsFormat.RGBA_ASTC4X4_SRGB:
                case GraphicsFormat.RGBA_ASTC5X5_SRGB:
                case GraphicsFormat.RGBA_ASTC6X6_SRGB:
                case GraphicsFormat.RGBA_ASTC8X8_SRGB:
                case GraphicsFormat.RGBA_ASTC10X10_SRGB:
                case GraphicsFormat.RGBA_ASTC12X12_SRGB:
                    return true;
                default:
                    return false;
            }
        }
        
        /// <summary>
        /// Set the texture alphaIsTransparency value.
        /// </summary>
        /// <param name="texture">Texture.</param>
        /// <param name="alphaIsTransparency">alphaIsTransparency option to set.</param>
        public static void SetTextureImporterAlpha(Texture2D texture, bool alphaIsTransparency)
        {
            if (texture is null) return;

            string assetPath = AssetDatabase.GetAssetPath(texture);
            var tImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (tImporter is  null) return;
            
            tImporter.textureType = TextureImporterType.Default;
            tImporter.alphaIsTransparency = alphaIsTransparency;
            AssetDatabase.ImportAsset(assetPath);
        }
    }
}