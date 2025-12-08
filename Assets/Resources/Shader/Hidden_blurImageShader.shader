Shader "Hidden/blurImageShader" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_BlurSize ("Blur Size", Range(0, 10)) = 1
		_Darkness ("Darkness", Range(0, 2)) = 1
		_Brightness ("Brightness", Range(0, 2)) = 1
		_Contrast ("Contrast", Range(0, 3)) = 1
		_EffectHeight ("Effect Height", Range(0, 1)) = 0.8
		_BlendSoftness ("Blend Softness", Range(0, 0.5)) = 0.1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}