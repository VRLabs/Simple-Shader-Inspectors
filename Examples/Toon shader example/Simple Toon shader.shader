Shader "SimpleShaderInspectors Examples/Simple toon shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Ramp ("Ramp", 2D) = "white" {}
        _RampColor ("Ramp color", Color) = (1,1,1,1)
        _ShadowIntensity ("Shadow intensity", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Ramp

        #pragma target 3.0

        sampler2D _Ramp;

        struct SurfaceOutputRamp
        {
            fixed3 Albedo; 
            fixed3 Normal;
            fixed3 Emission;
            fixed3 RampColor;
            fixed ShadowIntensity;   
            fixed Alpha;
        };

        half4 LightingRamp (SurfaceOutputRamp s, half3 lightDir, half atten) {
            half NdotL = dot (s.Normal, lightDir);
            half diff = NdotL * 0.5 + 0.5;
            half3 ramp = tex2D(_Ramp, float2(diff,diff)).rgb * s.RampColor;
            ramp = ramp * s.ShadowIntensity + (1 - s.ShadowIntensity);
            half4 c;
            c.rgb = s.Albedo * _LightColor0.rgb * ramp;
            c.a = s.Alpha;
            return c;
        }

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _ShadowIntensity;
        fixed4 _Color, _RampColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputRamp o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.RampColor = _RampColor.rgb;
            o.ShadowIntensity = _ShadowIntensity;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "VRLabs.SimpleShaderInspectorsExamples.SimpleToonShaderGUI"
}
