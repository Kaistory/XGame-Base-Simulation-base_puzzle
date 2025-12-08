Shader "Custom/RoundedSprite_Pixel_Outline_UV01"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color  ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0

        _CornerRadiusPx ("Corner Radius (px)", Float) = 16

        _OuterWidthPx      ("Outer (Black) Width px", Float) = 2
        _GapPx             ("Gap Black->White px",    Float) = 1
        _InnerWidthPx      ("Inner (White) Width px", Float) = 2
        _InnerBackWidthPx  ("Inner (Back) Width px",  Float) = 2

        _OuterColor     ("Outer Color", Color)     = (0,0,0,1)
        _InnerColor     ("Inner Color", Color)     = (1,1,1,1)
        _InnerBackColor ("Inner Back",  Color)     = (0,0,0,1)

        _FillOpacity    ("Fill Opacity (Inside Only)", Range(0,1)) = 1

        [PerRendererData] _UVMinMax ("UV Min(xy) Max(zw)", Vector) = (0,0,1,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off Lighting Off ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma target 2.0
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex:POSITION; float4 color:COLOR; float2 texcoord:TEXCOORD0; };
            struct v2f       { float4 vertex:SV_POSITION; fixed4 color:COLOR; float2 uv: TEXCOORD0; };

            sampler2D _MainTex; float4 _MainTex_ST; float4 _MainTex_TexelSize; 
            fixed4 _Color; 
            float  _CornerRadiusPx;
            float  _OuterWidthPx, _GapPx, _InnerWidthPx, _InnerBackWidthPx;
            fixed4 _OuterColor, _InnerColor, _InnerBackColor;
            float4 _UVMinMax; 
            float  _FillOpacity; 

            v2f vert (appdata_t v){
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = TRANSFORM_TEX(v.texcoord,_MainTex);
                o.color  = v.color * _Color; 
                #ifdef PIXELSNAP_ON
                o.vertex = UnityPixelSnap(o.vertex);
                #endif
                return o;
            }

            float sdRoundedBox01(float2 uv01, float r){
                float2 p = uv01 - 0.5;
                float2 d = abs(p) - (0.5 - r);
                return min(max(d.x,d.y),0.0) + length(max(d,0.0)) - r;
            }

            fixed4 frag (v2f i):SV_Target
            {
                float2 uvMin = _UVMinMax.xy;
                float2 uvMax = _UVMinMax.zw;
                float2 uv01  = (i.uv - uvMin) / max(uvMax - uvMin, float2(1e-6,1e-6));
                uv01 = saturate(uv01);

                fixed4 texCol  = tex2D(_MainTex, i.uv);
                float  baseA   = texCol.a;
                fixed3 baseRGB = texCol.rgb * i.color.rgb;

                float texW = max(_MainTex_TexelSize.z, 1.0);
                float texH = max(_MainTex_TexelSize.w, 1.0);
                float pieceWpx = texW * max(uvMax.x - uvMin.x, 1e-6);
                float pieceHpx = texH * max(uvMax.y - uvMin.y, 1e-6);
                float minPx    = max(min(pieceWpx, pieceHpx), 1.0);

                float rN   = saturate(_CornerRadiusPx / minPx);
                float bwN  = _OuterWidthPx     / minPx;
                float gapN = _GapPx            / minPx;
                float wwN  = _InnerWidthPx     / minPx;
                float b2N  = _InnerBackWidthPx / minPx;

                float d  = sdRoundedBox01(uv01, rN);
                float aa = max(fwidth(d), 1e-5) * 1.25;
                if (d > bwN + aa) discard;

                float inside = 1.0 - smoothstep(-aa, aa, d);

                float outerBegin = smoothstep(-aa, +aa, d);
                float outerEnd   = 1.0 - smoothstep(bwN - aa, bwN + aa, d);
                float outerMask  = saturate(outerBegin * outerEnd);

                float innerBeginW = smoothstep(gapN - aa,            gapN + aa,            -d);
                float innerEndW   = 1.0 - smoothstep(gapN + wwN - aa, gapN + wwN + aa,     -d);
                float innerWhiteMask = saturate(innerBeginW * innerEndW);

                float startB2 = gapN + wwN;
                float innerBeginB = smoothstep(startB2 - aa,              startB2 + aa,              -d);
                float innerEndB   = 1.0 - smoothstep(startB2 + b2N - aa,  startB2 + b2N + aa,        -d);
                float innerBlackMask = saturate(innerBeginB * innerEndB);

                fixed3 rgb = baseRGB;
                rgb = lerp(rgb, _OuterColor.rgb,     outerMask      * _OuterColor.a);
                rgb = lerp(rgb, _InnerColor.rgb,     innerWhiteMask * _InnerColor.a);
                rgb = lerp(rgb, _InnerBackColor.rgb, innerBlackMask * _InnerBackColor.a);

                float aInside = baseA * inside * _FillOpacity;
                float aBorder = saturate(
                    outerMask      * _OuterColor.a +
                    innerWhiteMask * _InnerColor.a +
                    innerBlackMask * _InnerBackColor.a
                );
                float aFinal = max(aInside, aBorder);

                if (aFinal < 0.001) discard;
                return fixed4(rgb, aFinal);
            }
            ENDCG
        }
    }
}
