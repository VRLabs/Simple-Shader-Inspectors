using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    public class RGBAPackerInspector : TextureGeneratorShaderInspector
    {
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_RedMultiplier");
            this.AddToggleControl("_RedInvert");
            this.AddTextureControl("_GreenTexture", "_GreenMultiplier");
            this.AddToggleControl("_GreenInvert");
            this.AddTextureControl("_BlueTexture", "_BlueMultiplier");
            this.AddToggleControl("_BluInvert");
            this.AddTextureControl("_AlphaTexture", "_AlphaMultiplier");
            this.AddToggleControl("_AlphaInvert");
        }
    }
}