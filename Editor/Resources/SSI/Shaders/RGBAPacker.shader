Shader "Hidden/VRLabs/GeneratorShaders/RGBAPacking"
{
    Properties
    {
        _MainTex ("Red", 2D) = "white" {}
        _GreenTexture ("Green", 2D) = "white" {}
        _BlueTexture ("Blue", 2D) = "white" {}
        _AlphaTexture ("Alpha", 2D) = "white" {}
        
        _RedChannel("Red Channel", Float) = 0
        _GreenChannel("Green Channel", Float) = 0
        _BlueChannel("Blue Channel", Float) = 0
        _AlphaChannel("Alpha Channel", Float) = 0
        
        _RedMultiplier("Red Multiplier", Range(0,1)) = 1.0
        _GreenMultiplier("Green Multiplier", Range(0,1)) = 1.0
        _BlueMultiplier("Blue Multiplier", Range(0,1)) = 1.0
        _AlphaMultiplier("Alpha Multiplier", Range(0,1)) = 1.0
        
        [Toggle]_RedInvert("Red Invert", Float) = 0
        [Toggle]_GreenInvert("Green Invert", Float) = 0
        [Toggle]_BluInvert("Blue Invert", Float) = 0
        [Toggle]_AlphaInvert("Alpha Invert", Float) = 0
    }
    SubShader
    {
        Lighting Off
        Blend One Zero

        Pass
        {
            CGPROGRAM

            #include "UnityCustomRenderTexture.cginc"
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            #pragma target 3.0

            UNITY_DECLARE_TEX2D(_MainTex);
            UNITY_DECLARE_TEX2D_NOSAMPLER(_GreenTexture);
            UNITY_DECLARE_TEX2D_NOSAMPLER(_BlueTexture);
            UNITY_DECLARE_TEX2D_NOSAMPLER(_AlphaTexture);
            float4 _MainTex_ST;

            float _RedMultiplier, _GreenMultiplier, _BlueMultiplier, _AlphaMultiplier;
            float _RedChannel, _GreenChannel, _BlueChannel, _AlphaChannel;
            float _RedInvert, _GreenInvert, _BlueInvert, _AlphaInvert;

            inline float4 conditionalInvert(float4 value, float4 invert)
            {
                return (1 - value) * invert + value * (1 - invert);
            }

            fixed4 frag (v2f_customrendertexture  i) : SV_Target
            {
                float r = UNITY_SAMPLE_TEX2D(_MainTex, i.localTexcoord.xy)[_RedChannel] * _RedMultiplier;
                float g = UNITY_SAMPLE_TEX2D_SAMPLER(_GreenTexture, _MainTex, i.localTexcoord.xy)[_GreenChannel] * _GreenMultiplier;
                float b = UNITY_SAMPLE_TEX2D_SAMPLER(_BlueTexture, _MainTex, i.localTexcoord.xy)[_BlueChannel] * _BlueMultiplier;
                float a = UNITY_SAMPLE_TEX2D_SAMPLER(_AlphaTexture, _MainTex, i.localTexcoord.xy)[_AlphaChannel] * _AlphaMultiplier;
                float4 rgba = float4(r, g, b, a);
                rgba = conditionalInvert(rgba, float4(_RedInvert, _GreenInvert, _BlueInvert, _AlphaInvert));
                return rgba;
            }
            ENDCG
        }
    }
    CustomEditor "VRLabs.SimpleShaderInspectors.RGBAPackerInspector"
}
