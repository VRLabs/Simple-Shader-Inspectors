Shader "SimpleShaderInspectors Examples/Simple standard shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpIntensity ("Normal map intensity", float) = 1
        _MultiMask ("MultiMask", 2D) = "white" {}
        _Metallic ("Metallic", Range(0, 1)) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 1
        _Occlusion ("Occlusion", Range(0, 1)) = 1
        _EmissionColor ("Emission color", Color) = (0, 0, 0, 0)
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _MultiMask;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_MultiMask;
        };

        half _BumpIntensity;
        half _Metallic;
        half _Smoothness;
        half _Occlusion;
        fixed4 _Color;
        fixed4 _EmissionColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            fixed3 d = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * _BumpIntensity;
            o.Normal = d;
            o.Albedo = c.rgb;
            o.Alpha = c.a;

            c = tex2D(_MultiMask, IN.uv_MultiMask);
            o.Metallic = c.r * _Metallic;
            o.Smoothness = c.g * _Smoothness;
            o.Occlusion = c.b * _Occlusion;
            o.Emission = c.a * _EmissionColor;
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "VRLabs.SimpleShaderInspectorsExamples.SimpleStandardShaderGUI"
}
