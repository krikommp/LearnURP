Shader "Unlit/MeshSRPBatch"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _TextureId("TextureId", Float) = 0
        [HideInInspector] _NewUV("NewUV", Vector) = (0,0,0,0)
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

            CBUFFER_START(UnityPerMaterial)
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float4 _NewUV;
            CBUFFER_END
            

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

            half4 SpriteFrag(v2f IN) : SV_Target
            {
                float4 rect = _NewUV;
                float2 newUV = IN.texcoord;
                newUV.x = lerp(rect.x, rect.x + rect.z, newUV.x);
                newUV.y = lerp(rect.y, rect.y + rect.w, newUV.y);
                
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, newUV);
                clip(c.a - 0.5);
                
                return half4(1.0, 1.0, 1.0, 1.0);
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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2D _ColorMask;
            SamplerState sampler_ColorMask;
            Texture2D _ShadowMap;
            SamplerState sampler_ShadowMap;

            CBUFFER_START(UnityPerMaterial)
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            float4 _NewUV;
            CBUFFER_END
            
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

            half4 SpriteFrag(v2f IN) : SV_Target
            {
                float4 rect = _NewUV;
                float2 newUV = IN.texcoord;
                newUV.x = lerp(rect.x, rect.x + rect.z, newUV.x);
                newUV.y = lerp(rect.y, rect.y + rect.w, newUV.y);
                
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, newUV);
                c.rgb *= c.a;
                
                half4 c2 = SAMPLE_TEXTURE2D(_ColorMask, sampler_ColorMask, newUV);
                half4 c3 = SAMPLE_TEXTURE2D(_ShadowMap, sampler_ShadowMap, newUV);
                
                half r = c.r * c2.r *c3.r;
                c.r = r;
                
                return c;
            }
            
            ENDHLSL
        }
    }
}