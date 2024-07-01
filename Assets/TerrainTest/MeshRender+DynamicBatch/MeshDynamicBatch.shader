Shader "Unlit/MeshDynamicBatch"
{
    Properties
    {
        _Textures("Textures", 2DArray) = "" {}
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
            #pragma require 2darray
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2DArray _Textures;
            SamplerState sampler_Textures;

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
                return half4(1,1,1,1);
            }
            ENDHLSL
        }
        

        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Off
            Blend One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma require 2darray
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2DArray _Textures;
            SamplerState sampler_Textures;

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
                half4 c = _Textures.Sample(sampler_Textures,
                                           float3(IN.texcoord, IN.color.r * 5));
                
                return c;

                // return half4(IN.color.xyz, 1.0f);
            }
            ENDHLSL
        }
    }
}