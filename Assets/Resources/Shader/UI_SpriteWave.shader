Shader "UI/SpriteWave" {
	Properties {
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Vector) = (1,1,1,1)
		[Header(Wave Settings)] _WaveAmplitude ("Wave Amplitude", Range(0, 0.1)) = 0.02
		_WaveFrequency ("Wave Frequency", Range(0, 20)) = 5
		_WaveSpeed ("Wave Speed", Range(0, 5)) = 1
		_WaveDirection ("Wave Direction", Vector) = (1,0,0,0)
		[Header(Secondary Wave)] _SecondWaveAmplitude ("Second Wave Amplitude", Range(0, 0.05)) = 0.01
		_SecondWaveFrequency ("Second Wave Frequency", Range(0, 30)) = 8
		_SecondWaveSpeed ("Second Wave Speed", Range(0, 5)) = 1.5
		[Header(UI Settings)] _StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255
		_ColorMask ("Color Mask", Float) = 15
		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}