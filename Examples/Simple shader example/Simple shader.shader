Shader "SimpleShaderInspectors Examples/Simple shader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        [Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
        _BumpIntensity ("Normal map intensity", float) = 1
        _EnableNormal ("Enable normal", float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert fullforwardshadows

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _BumpMap;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        half _BumpIntensity;
        half _EnableNormal;
        fixed4 _Color;


        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            if(_EnableNormal > 0)
            {
                fixed3 d = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap)) * _BumpIntensity;
                o.Normal = d;
            }
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
    CustomEditor "VRLabs.SimpleShaderInspectorsExamples.SimpleShaderGUI"
}
