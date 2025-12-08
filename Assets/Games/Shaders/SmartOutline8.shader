Shader "Custom/DoubleOutlineRoundedSmart"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _BorderColorBlack ("Outer Border (Black)", Color) = (0,0,0,1)
        _BorderColorWhite ("Inner Border (White)", Color) = (1,1,1,1)

        _OuterSize ("Outer Border Size", Range(0,0.2)) = 0.05
        _InnerSize ("Inner Border Size", Range(0,0.2)) = 0.025
        _CornerRadius ("Corner Radius", Range(0,0.5)) = 0.1
        _Smoothness ("Smooth Edge", Range(0,0.02)) = 0.005

        _EnableTop ("Enable Top", Float) = 1
        _EnableBottom ("Enable Bottom", Float) = 1
        _EnableLeft ("Enable Left", Float) = 1
        _EnableRight ("Enable Right", Float) = 1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _BorderColorBlack;
            float4 _BorderColorWhite;
            float _OuterSize;
            float _InnerSize;
            float _CornerRadius;
            float _Smoothness;

            float _EnableTop;
            float _EnableBottom;
            float _EnableLeft;
            float _EnableRight;

            float4 _UVMinMax;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float inRoundedRectSmart(float2 uv, float radius, float smoothness,
                                     float enableTop, float enableBottom, float enableLeft, float enableRight)
            {
                float2 corner = float2(0,0);
                float2 diff;
                float dist;
                float mask = 1.0;

                if (enableTop > 0 && enableLeft > 0 && uv.x < radius && uv.y > 1.0 - radius)
                {
                    corner = float2(radius, 1.0 - radius);
                    diff = uv - corner;
                    dist = length(diff);
                    mask = smoothstep(radius, radius - smoothness, dist);
                }

                if (enableTop > 0 && enableRight > 0 && uv.x > 1.0 - radius && uv.y > 1.0 - radius)
                {
                    corner = float2(1.0 - radius, 1.0 - radius);
                    diff = uv - corner;
                    dist = length(diff);
                    mask = smoothstep(radius, radius - smoothness, dist);
                }

                if (enableBottom > 0 && enableLeft > 0 && uv.x < radius && uv.y < radius)
                {
                    corner = float2(radius, radius);
                    diff = uv - corner;
                    dist = length(diff);
                    mask = smoothstep(radius, radius - smoothness, dist);
                }

                if (enableBottom > 0 && enableRight > 0 && uv.x > 1.0 - radius && uv.y < radius)
                {
                    corner = float2(1.0 - radius, radius);
                    diff = uv - corner;
                    dist = length(diff);
                    mask = smoothstep(radius, radius - smoothness, dist);
                }

                return mask;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uvLocal = (i.uv - _UVMinMax.xy) / (_UVMinMax.zw - _UVMinMax.xy);

                fixed4 texCol = tex2D(_MainTex, i.uv);

                float borderBlack = 0.0;
                float borderWhite = 0.0;

                if (_EnableTop > 0)    borderBlack = max(borderBlack, smoothstep(1.0 - _OuterSize, 1.0 - _OuterSize + _Smoothness, uvLocal.y));
                if (_EnableBottom > 0) borderBlack = max(borderBlack, smoothstep(_OuterSize, _OuterSize - _Smoothness, uvLocal.y));
                if (_EnableLeft > 0)   borderBlack = max(borderBlack, smoothstep(_OuterSize, _OuterSize - _Smoothness, uvLocal.x));
                if (_EnableRight > 0)  borderBlack = max(borderBlack, smoothstep(1.0 - _OuterSize, 1.0 - _OuterSize + _Smoothness, uvLocal.x));

                if (_EnableTop > 0)    borderWhite = max(borderWhite, smoothstep(1.0 - _InnerSize, 1.0 - _InnerSize + _Smoothness, uvLocal.y));
                if (_EnableBottom > 0) borderWhite = max(borderWhite, smoothstep(_InnerSize, _InnerSize - _Smoothness, uvLocal.y));
                if (_EnableLeft > 0)   borderWhite = max(borderWhite, smoothstep(_InnerSize, _InnerSize - _Smoothness, uvLocal.x));
                if (_EnableRight > 0)  borderWhite = max(borderWhite, smoothstep(1.0 - _InnerSize, 1.0 - _InnerSize + _Smoothness, uvLocal.x));

                float roundedMask = inRoundedRectSmart(uvLocal, _CornerRadius, _Smoothness,
                                                       _EnableTop, _EnableBottom, _EnableLeft, _EnableRight);

                if (roundedMask < 0.01)
                    discard;

                float4 color = texCol;
                color = lerp(color, _BorderColorWhite, borderWhite);
                color = lerp(color, _BorderColorBlack, borderBlack);

                if (borderBlack > 0.5)
                    color.a = 1.0;

                return color;
            }
            ENDCG
        }
    }
}
