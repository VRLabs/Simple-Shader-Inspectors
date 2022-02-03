using VRLabs.SimpleShaderInspectors.Controls;

namespace VRLabs.SimpleShaderInspectors
{
    // ReSharper disable once InconsistentNaming
    internal class RGBAPackerInspector : TextureGeneratorShaderInspector
    {
        protected override void Start()
        {
            this.AddTextureControl("_MainTex", "_RedMultiplier").WithAlias("RedTexture");
            this.AddToggleControl("_RedInvert").WithAlias("Invert");
            this.AddSpaceControl(20);
            this.AddTextureControl("_GreenTexture", "_GreenMultiplier").WithAlias("GreenTexture");
            this.AddToggleControl("_GreenInvert").WithAlias("Invert");
            this.AddSpaceControl(20);
            this.AddTextureControl("_BlueTexture", "_BlueMultiplier").WithAlias("BlueTexture");
            this.AddToggleControl("_BluInvert").WithAlias("Invert");
            this.AddSpaceControl(20);
            this.AddTextureControl("_AlphaTexture", "_AlphaMultiplier").WithAlias("AlphaTexture");
            this.AddToggleControl("_AlphaInvert").WithAlias("Invert");
        }
    }
}