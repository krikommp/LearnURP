Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
        }

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Off
            Blend One OneMinusSrcAlpha
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            Texture2D _ColorMask;
            SamplerState sampler_ColorMask;
            Texture2D _ShadowMap;
            SamplerState sampler_ShadowMap;
            float4 _Color;

            inline float4 UnityFlipSprite(in float3 pos, in half2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f vert (appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = TransformObjectToHClip(IN.vertex.xyz);
                OUT.texcoord = IN.texcoord;
                return OUT;
            }

            half4 frag (v2f IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
                c.rgb *= c.a;
                
                half4 c2 = SAMPLE_TEXTURE2D(_ColorMask, sampler_ColorMask, IN.texcoord);
                half4 c3 = SAMPLE_TEXTURE2D(_ShadowMap, sampler_ShadowMap, IN.texcoord);
                
                half r = c.r * c2.r *c3.r;
                c.r = r * _Color.r;
                
                return c;
            }
            ENDHLSL
        }
    }
}
