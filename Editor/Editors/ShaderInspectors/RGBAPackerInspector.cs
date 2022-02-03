using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    // ReSharper disable once InconsistentNaming
    internal class RGBAPackerInspector : TextureGeneratorShaderInspector
    {
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_RedMultiplier").WithAlias("RedTexture");
            var line = this.AddHorizontalContainer();
            line.AddToggleControl("_RedInvert").WithAlias("Invert");
            line.AddRGBASelectorControl("_RedChannel").WithAlias("Channel");
            this.AddSpaceControl(10);
            
            this.AddTextureControl("_GreenTexture", "_GreenMultiplier").WithAlias("GreenTexture");
            line = this.AddHorizontalContainer();
            line.AddToggleControl("_GreenInvert").WithAlias("Invert");
            line.AddRGBASelectorControl("_GreenChannel").WithAlias("Channel");
            this.AddSpaceControl(10);
            
            this.AddTextureControl("_BlueTexture", "_BlueMultiplier").WithAlias("BlueTexture");
            line = this.AddHorizontalContainer();
            line.AddToggleControl("_BluInvert").WithAlias("Invert");
            line.AddRGBASelectorControl("_BlueChannel").WithAlias("Channel");
            this.AddSpaceControl(10);
            
            this.AddTextureControl("_AlphaTexture", "_AlphaMultiplier").WithAlias("AlphaTexture");
            line = this.AddHorizontalContainer();
            line.AddToggleControl("_AlphaInvert").WithAlias("Invert");
            line.AddRGBASelectorControl("_AlphaChannel").WithAlias("Channel");
        }
    }
}