Shader "Unlit/Sprite/Stencil"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(-1,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }

        Pass
        {
            Tags
            {
                "LightMode" = "PreStencil"
            }

            ZWrite Off
            ZTest Off
            ColorMask 0
            Cull Off

            Stencil
            {
                Ref 1
                WriteMask 1
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Assets/2D Occlusion/ScreenDoor.hlsl"
            // #define _AlphaTest_Threshold (0.0)
            #define _AlphaTest_ScreenDoor (0.0)

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Transparency;
            float4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half alpha = tex2D(_MainTex, IN.uv).a;

                #ifdef  _AlphaTest_Threshold
                clip(alpha - _Transparency);
                #endif

                #ifdef _AlphaTest_ScreenDoor
                if (alpha <= 0.01)
                {
                    clip(-1); // 直接丢弃当前片元
                }
                else if (alpha >= 1.0)
                {
                    // 不做任何处理，直接返回
                    return 0;
                }
                else
                {
                    DitheredAlpha(alpha + _Transparency, IN.positionHCS);
                }
                #endif

                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Off
            Blend SrcAlpha OneMinusSrcAlpha

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/2D Occlusion/Common.Variable.hlsl"
            #include "Assets/2D Occlusion/Common.Function.hlsl"
            ENDHLSL
        }
    }
}