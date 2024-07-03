Shader "Unlit/MeshDynamicBatch"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _TextureId("TextureId", Float) = 0
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
        
        Pass
        {
            Tags
            {
                "LightMode" = "PreDepth"
            }
            
            Cull Off
            ZWrite On
            ColorMask 0

            HLSLPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            float _TextureId;
            half4 _RendererColor;
            half2 _Flip;
            float _EnableExternalAlpha;

            half4 _Color;

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

            inline float4 UnityFlipSprite(in float3 pos, in half2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = TransformObjectToHClip(IN.vertex.xyz);
                OUT.texcoord = IN.texcoord;
                return OUT;
            }

            Texture2D _MainTex;
            SamplerState sampler_MainTex;

            half4 SpriteFrag(v2f IN) : SV_Target
            {
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
                clip(c.a - 0.5);
                return half4(1,1,1,1);
            }
            ENDHLSL
        }
        

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Equal
            Blend One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            Texture2D _ColorMask;
            SamplerState sampler_ColorMask;
            Texture2D _ShadowMap;
            SamplerState sampler_ShadowMap;
            
            float _TextureId;
            half4 _RendererColor;
            half2 _Flip;
            float _EnableExternalAlpha;

            half4 _Color;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float4 color : TEXCOORD1;
            };

            inline float4 UnityFlipSprite(in float3 pos, in half2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = TransformObjectToHClip(IN.vertex.xyz);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color;
                return OUT;
            }

            half4 SpriteFrag(v2f IN) : SV_Target
            {
                // Now we sample texture from Texture2DArray
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
                c.rgb *= c.a;
                
                half4 c2 = SAMPLE_TEXTURE2D(_ColorMask, sampler_ColorMask, IN.texcoord);
                half4 c3 = SAMPLE_TEXTURE2D(_ShadowMap, sampler_ShadowMap, IN.texcoord);
                
                half r = c.r * c2.r *c3.r;
                c.r = r * _Color.r;
                
                return c;

                // return half4(IN.texcoord, 1.0f, 1.0f);
            }
            ENDHLSL
        }
    }
}