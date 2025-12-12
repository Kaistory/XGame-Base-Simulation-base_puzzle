// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XGame/Diffuse"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _MainTex ("Albedo", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        // And generate the shadow pass with instancing support
        #pragma surface surf Lambert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        struct Input
        {
            float4 color: Color;
            float2 uv_MainTex;
            float2 uv_BumpMap;
        };

        sampler2D _MainTex;

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = albedo.rgb;
            o.Alpha = albedo.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}