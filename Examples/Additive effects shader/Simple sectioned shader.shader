Shader "SimpleShaderInspectors Examples/Simple sectioned shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _AdditionalMasksEnable ("Additional Masks", float) = 0
        _AdditionalMasks ("Masks)", 2D) = "black" {}
        _RColor ("Color", Color) = (1,1,1,1)
        _REmissive ("Emissive", float) = 0
        _GColor ("Color", Color) = (1,1,1,1)
        _GEmissive ("Emissive", float) = 0
        _BColor ("Color", Color) = (1,1,1,1)
        _BEmissive ("Emissive", float) = 0
        _AColor ("Color", Color) = (1,1,1,1)
        _AEmissive ("Emissive", float) = 0
        _RimLightEnable("Rim light", float) = 0
        _RimLightColor ("Color", Color) = (1,1,1,1)
        _RimLightPower("Power", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _AdditionalMasks;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_AdditionalMasks;
            float3 viewDir;
        };

        half _AdditionalMasksEnable, _RimLightEnable, _RimLightPower;
        fixed4 _Color, _RimLightColor, _RColor, _GColor, _BColor, _AColor;
        float _REmissive, _GEmissive, _BEmissive, _AEmissive;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed3 emission = 0;

            if(_AdditionalMasksEnable > 0)
            {
                fixed4 masks = tex2D(_AdditionalMasks, IN.uv_AdditionalMasks);
                albedo = lerp(albedo, _RColor, masks.r * (1 - _REmissive));
                albedo = lerp(albedo, _GColor, masks.g * (1 - _GEmissive));
                albedo = lerp(albedo, _BColor, masks.b * (1 - _BEmissive));
                albedo = lerp(albedo, _AColor, masks.a * (1 - _AEmissive));

                emission = lerp(emission, _RColor, masks.r * _REmissive);
                emission = lerp(emission, _GColor, masks.g * _GEmissive);
                emission = lerp(emission, _BColor, masks.b * _BEmissive);
                emission = lerp(emission, _AColor, masks.a * _AEmissive);
            }
            if(_RimLightEnable > 0)
            {
                half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
                emission += pow(rim,max((1 - _RimLightPower) * 10, 0.001)) * _RimLightColor;
            }

            o.Albedo = albedo.rgb;
            o.Alpha = albedo.a;
            o.Emission = emission;
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "VRLabs.SimpleShaderInspectorsExamples.SimpleSectionedShaderGUI"
}
