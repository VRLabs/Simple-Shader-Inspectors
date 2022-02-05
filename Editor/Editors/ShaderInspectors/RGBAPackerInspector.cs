using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    // ReSharper disable once InconsistentNaming
    internal class RGBAPackerInspector : TextureGeneratorShaderInspector
    {
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_RedMultiplier").WithAlias("RGBAPacker_RedTexture");
            var line = this.AddHorizontalContainer();
            line.AddToggleControl("_RedInvert").WithAlias("RGBAPacker_Invert");
            line.AddRGBASelectorControl("_RedChannel").WithAlias("RGBAPacker_Channel");
            this.AddSpaceControl(10);
            
            this.AddTextureControl("_GreenTexture", "_GreenMultiplier").WithAlias("RGBAPacker_GreenTexture");
            line = this.AddHorizontalContainer();
            line.AddToggleControl("_GreenInvert").WithAlias("RGBAPacker_Invert");
            line.AddRGBASelectorControl("_GreenChannel").WithAlias("RGBAPacker_Channel");
            this.AddSpaceControl(10);
            
            this.AddTextureControl("_BlueTexture", "_BlueMultiplier").WithAlias("RGBAPacker_BlueTexture");
            line = this.AddHorizontalContainer();
            line.AddToggleControl("_BluInvert").WithAlias("RGBAPacker_Invert");
            line.AddRGBASelectorControl("_BlueChannel").WithAlias("RGBAPacker_Channel");
            this.AddSpaceControl(10);
            
            this.AddTextureControl("_AlphaTexture", "_AlphaMultiplier").WithAlias("RGBAPacker_AlphaTexture");
            line = this.AddHorizontalContainer();
            line.AddToggleControl("_AlphaInvert").WithAlias("RGBAPacker_Invert");
            line.AddRGBASelectorControl("_AlphaChannel").WithAlias("RGBAPacker_Channel");
        }
    }
}