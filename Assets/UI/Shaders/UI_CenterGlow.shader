
Shader "UI/Center Glow Fixed"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Image Tint", Color) = (1,1,1,1)

        _GlowColor ("Glow Color", Color) = (0,0.8,1,1)
        _GlowSize ("Glow Size", Range(1,250)) = 40
        _GlowSoftness ("Glow Softness", Range(0.2,5)) = 2
        _GlowIntensity ("Glow Intensity", Range(0,10)) = 2

        _GlowInnerRadius ("Inner Radius", Range(0,1.5)) = 0
        _GlowOuterRadius ("Outer Radius", Range(0.01,2)) = 1.2

        _PulseSpeed ("Pulse Speed", Range(0,10)) = 0
        _PulseAmount ("Pulse Amount", Range(0,1)) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255
        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;

            fixed4 _Color;
            fixed4 _GlowColor;

            float _GlowSize;
            float _GlowSoftness;
            float _GlowIntensity;

            float _GlowInnerRadius;
            float _GlowOuterRadius;

            float _PulseSpeed;
            float _PulseAmount;

            float4 _ClipRect;

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Original image
                fixed4 original = tex2D(_MainTex, i.uv) * i.color;

                // Soft silhouette expansion

                float2 offset = _MainTex_TexelSize.xy * _GlowSize;

                float expanded = 0;

                float2 directions[8] = {
                    float2(1,0),
                    float2(-1,0),
                    float2(0,1),
                    float2(0,-1),
                    float2(0.707,0.707),
                    float2(-0.707,0.707),
                    float2(0.707,-0.707),
                    float2(-0.707,-0.707)
                };

                float ring1 = 0;
                float ring2 = 0;
                float ring3 = 0;

                for (int n = 0; n < 8; n++)
                {
                    ring1 += tex2D(_MainTex, i.uv + directions[n] * offset * 0.33).a;
                    ring2 += tex2D(_MainTex, i.uv + directions[n] * offset * 0.66).a;
                    ring3 += tex2D(_MainTex, i.uv + directions[n] * offset).a;
                }

                ring1 /= 8.0;
                ring2 /= 8.0;
                ring3 /= 8.0;

                // Combine samples with different weights
                expanded = ring1 * 0.5 + ring2 * 0.3 + ring3 * 0.2;

                // Control gradient softness
                expanded = pow(
                    saturate(expanded),
                    1.0 / max(_GlowSoftness, 0.01)
                );

                // Only outside the original image
                float mask = expanded * (1.0 - original.a);

                // Radial glow from the center
                float2 centeredUV = (i.uv - 0.5) * 2.0;
                float distanceFromCenter = length(centeredUV);

                float radial = 1.0 - smoothstep(
                    _GlowInnerRadius,
                    max(_GlowOuterRadius, 0.001),
                    distanceFromCenter
                );

                // Pulse animation
                float pulse = 1.0 + sin(_Time.y * _PulseSpeed) * _PulseAmount;

                float glowAlpha = saturate(
                    mask * radial * _GlowIntensity * pulse * _GlowColor.a
                );

                glowAlpha *= i.color.a;

                // Composite glow behind the original image
                float finalGlowAlpha = glowAlpha * (1.0 - original.a);
                float finalAlpha = original.a + finalGlowAlpha;

                float3 finalRGB = (
                    original.rgb * original.a +
                    _GlowColor.rgb * finalGlowAlpha
                ) / max(finalAlpha, 0.0001);

                fixed4 result = fixed4(finalRGB, finalAlpha);

                #ifdef UNITY_UI_CLIP_RECT
                result.a *= UnityGet2DClipping(
                    i.worldPosition.xy,
                    _ClipRect
                );
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(result.a - 0.001);
                #endif

                return result;
            }

            ENDCG
        }
    }
}
