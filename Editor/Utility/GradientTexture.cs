using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
namespace VRLabs.SimpleShaderInspectors.Utility
{
    /// <summary>
    /// Enum indicating the blend mode of the gradient texture.
    /// </summary>
    public enum GradientBlendMode
    {
        Linear,
        Fixed
    }

    /// <summary>
    /// Utility class used when creating gradient editors.
    /// </summary>
    public class GradientTexture
    {
        /// <summary>
        /// List of gradient keys.
        /// </summary>
        [SerializeField]
        public List<ColorKey> Keys = new List<ColorKey>();

        /// <summary>
        /// Blend mode between keys.
        /// </summary>
        public GradientBlendMode BlendMode;

        // Texture generated.
        private Texture2D _texture;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="width">With of the result gradient texture.</param>
        public GradientTexture(int width)
        {
            BlendMode = GradientBlendMode.Linear;
            Keys.Add(new ColorKey(Color.black, 0));
            Keys.Add(new ColorKey(Color.white, 1));

            _texture = new Texture2D(width, 1, TextureFormat.RGB24, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            UpdateTexture();
        }

        /// <summary>
        /// Returns a color at the specified time.
        /// </summary>
        /// <param name="time">Time of the color to sample.</param>
        /// <returns>Color at selected time.</returns>
        public Color Evaluate(float time)
        {
            ColorKey keyLeft = Keys[0];
            ColorKey keyRight = Keys[Keys.Count - 1];

            for (int i = 0; i < Keys.Count; i++)
            {
                if (Keys[i].Time <= time)
                {
                    keyLeft = Keys[i];
                }
                if (Keys[i].Time >= time)
                {
                    keyRight = Keys[i];
                    break;
                }
            }

            if (BlendMode == GradientBlendMode.Fixed) return keyRight.Color;
            
            float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
            return Color.Lerp(keyLeft.Color, keyRight.Color, blendTime);

        }

        /// <summary>
        /// Adds a new key, and removes any key that is in the same time.
        /// </summary>
        /// <param name="color">Color of the key</param>
        /// <param name="time">Time of the key</param>
        /// <returns>The key index</returns>
        public int AddKey(Color color, float time)
        {
            return AddKey(color, time, true);
        }
        // Internal version that has an additional skippable check for deleting a key that is in the same time of the new one.
        private int AddKey(Color color, float time, bool shouldDelete)
        {
            for (int i = 0; i < Keys.Count; i++)
            {
                if (time < Keys[i].Time)
                {
                    Keys.Insert(i, new ColorKey(color, time));
                    UpdateTexture();
                    return i;
                }

                if (time == Keys[i].Time && shouldDelete)
                {
                    Keys[i] = new ColorKey(color, time);
                    UpdateTexture();
                    return -1;
                }
            }
            Keys.Add(new ColorKey(color, time));
            UpdateTexture();
            return Keys.Count - 1;
        }

        /// <summary>
        /// Removes a key at the selected index.
        /// </summary>
        /// <param name="index">Index of the key to remove.</param>
        public void RemoveKey(int index)
        {
            RemoveKey(index, true);
        }

        // Private version with an additional check to decide if removing it with only one key left, is just for updating key time
        // correctly when there's only one key in the list.
        private void RemoveKey(int index, bool checkMin)
        {
            if (Keys.Count > 1 && checkMin)
            {
                Keys.RemoveAt(index);
            }
            else if (!checkMin)
            {
                Keys.RemoveAt(index);
            }
            UpdateTexture();
        }

        /// <summary>
        /// Updates the key time position.
        /// </summary>
        /// <param name="index">Index of the key to update.</param>
        /// <param name="time">New time.</param>
        /// <returns>The new index of the key.</returns>
        public int UpdateKeyTime(int index, float time)
        {
            if (time < 0)
            {
                time = 0;
            }
            else if (time > 1)
            {
                time = 1;
            }

            if (index < 0) index = 0;

            Color col = Keys[index].Color;
            RemoveKey(index, false);
            return AddKey(col, time, false);
        }

        /// <summary>
        /// Updates the key color.
        /// </summary>
        /// <param name="index">Index of the key.</param>
        /// <param name="col">Color of the key.</param>
        public void UpdateKeyColor(int index, Color col)
        {
            Keys[index] = new ColorKey(col, Keys[index].Time);
            UpdateTexture();
        }

        /// <summary>
        /// Get gradient texture.
        /// </summary>
        /// <returns>Texture of the gradient.</returns>
        public Texture2D GetTexture()
        {
            return _texture;
        }

        /// <summary>
        /// Updates the width of the result texture.
        /// </summary>
        /// <param name="width">Width.</param>
        public void UpdateTextureWidth(int width)
        {
            _texture = new Texture2D(width, 1, TextureFormat.RGB24, false)
            {
                wrapMode = TextureWrapMode.Clamp
            };
            UpdateTexture();
        }

        /// <summary>
        /// Updates the internal gradient Texture.
        /// </summary>
        public void UpdateTexture()
        {
            Color[] colors = new Color[_texture.width];
            for (int i = 0; i < _texture.width; i++)
            {
                colors[i] = Evaluate((float)i / (_texture.width - 1));
            }
            _texture.SetPixels(colors);
            //texture.Apply();
            _texture.Apply(true);
        }

        /// <summary>
        /// Structure containing a color and a float indicating at which time the color is.
        /// </summary>
        [System.Serializable]
        public struct ColorKey
        {
            /// <summary>
            /// Color of the key.
            /// </summary>
            [SerializeField]
            public Color Color;
            
            /// <summary>
            /// Time of the key.
            /// </summary>
            [SerializeField]
            public float Time;

            /// <summary>
            /// Default constructor of <see cref="ColorKey"/>
            /// </summary>
            /// <param name="color">Color of the key.</param>
            /// <param name="time">Time of the key.</param>
            public ColorKey(Color color, float time)
            {
                Color = color;
                Time = time;
            }
        }
    }
}