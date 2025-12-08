Shader "Custom/URP_AutoUV" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Color ("Color", Vector) = (1,1,1,1)
		_Tile ("UV Tiling", Vector) = (1,1,0,0)
		_Offset ("UV Offset", Vector) = (0,0,0,0)
		_UVMode ("UV Mode (0=Local,1=World)", Range(0, 1)) = 0
		_Projection ("Projection Axis (0=XY,1=XZ,2=YZ)", Range(0, 2)) = 1
		_KeepMove ("Keep Move (UV Scroll)", Float) = 0
		_ScrollSpeed ("Scroll Speed (U,V)", Vector) = (0,0,0,0)
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
	Fallback "Hidden/InternalErrorShader"
}