Shader "Unlit/Common/Ring"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Transparent+1"
        }

        Pass
        {
            ZWrite Off
            ZTest On
            Blend SrcAlpha OneMinusSrcAlpha

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 positionNDC : TEXCOORD1;
            };

            float _Transparency;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AlphaMask;
            float4 _AlphaMask_ST;
            float4 _Color;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                float4 ndc = OUT.positionHCS * 0.5f;
                OUT.positionNDC.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
                OUT.positionNDC.zw = OUT.positionHCS.zw;
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 col = tex2D(_MainTex, IN.uv);
                half4 finalColor = col * _Color;

                float2 uv = IN.positionHCS.xy / _ScaledScreenParams.xy;
                half4 alphaMask = tex2D(_AlphaMask, uv);

                float depth = (IN.positionNDC.z) / IN.positionNDC.w;
                depth = 1 / depth;
                // float depth = (IN.positionHCS.z / IN.positionHCS.w) * 0.5 + 0.5;
                
                half maskDepth = alphaMask.g;

                if (depth <= maskDepth)
                {
                    // finalColor = half4(1, 0, 0, 1);
                }else
                {
                    finalColor.a = finalColor.a + finalColor.a * (-alphaMask.r);
                }

                return finalColor;
            }
            ENDHLSL
        }
    }
}