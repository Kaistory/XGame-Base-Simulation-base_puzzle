Shader "UI/DoubleOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        // Bo góc nhẹ
        _Radius ("Corner Radius", Range(0.015,0.035)) = 0.025

        // Độ dày viền ngoài (đen)
        _OuterBorder ("Outer Border Width", Range(0.002,0.005)) = 0.0045

        // Độ dày viền trong (trắng)
        _InnerBorder ("Inner Border Width", Range(0.002,0.005)) = 0.0028

        // Màu
        _OuterColor ("Outer Border Color", Color) = (0,0,0,1)
        _InnerColor ("Inner Border Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Radius;
            float _OuterBorder;
            float _InnerBorder;
            float4 _OuterColor;
            float4 _InnerColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float alpha : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.alpha = v.color.a;
                return o;
            }

            float RoundedSDF(float2 uv01, float r)
            {
                float2 p = uv01 - 0.5;
                float2 b = 0.5 - r;
                float2 d = abs(p) - b;
                return length(max(d, 0.0)) - r;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv01 = (i.uv - _MainTex_ST.zw) / _MainTex_ST.xy;
                uv01 = saturate(uv01);

                // Giới hạn min để tránh mất biên
                float outer = max(_OuterBorder, 0.0008);
                float inner = max(_InnerBorder, 0.0006);
                float total = outer + inner;

                float d = RoundedSDF(uv01, _Radius);
                float aa = fwidth(d) * 1.2;

                if (d > aa)
                    discard;

                fixed4 texCol = tex2D(_MainTex, i.uv);
                texCol.a *= i.alpha;

                // Trung tâm (ảnh)
                float centerMask = 1.0 - smoothstep(-total - aa, -total + aa, d);

                // Viền trong (trắng)
                float innerMask =
                    smoothstep(-total - aa, -total + aa, d) *
                    (1.0 - smoothstep(-inner - aa, -inner + aa, d));

                // Viền ngoài (đen)
                float outerMask = smoothstep(-inner - aa, -inner + aa, d) * (1.0 - smoothstep(0.0 - aa, 0.0 + aa, d));

                fixed4 col = 0;
                col += texCol * centerMask;
                col += _InnerColor * innerMask;
                col += _OuterColor * outerMask;

                col.a = max(max(texCol.a * centerMask, _InnerColor.a * innerMask), _OuterColor.a * outerMask);

                return col;
            }
            ENDCG
        }
    }
    FallBack "UI/Default"
}