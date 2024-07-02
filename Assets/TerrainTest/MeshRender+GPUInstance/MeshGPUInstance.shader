Shader "Unlit/MeshGPUInstance"
{
    Properties
    {
        _Textures("Textures", 2DArray) = "" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _MainTex2 ("Sprite Texture2", 2D) = "white" {}
        _MainTex3 ("Sprite Texture3", 2D) = "white" {}
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
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
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2DArray _Textures;
            SamplerState sampler_Textures;
            Texture2D _MainTex2;
            SamplerState sampler_MainTex2;
            Texture2D _MainTex3;
            SamplerState sampler_MainTex3;
            
            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, _TextureIndex)
            UNITY_INSTANCING_BUFFER_END(Props)

            half4 _RendererColor;
            half2 _Flip;
            float _EnableExternalAlpha;


            half4 _Color;


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float3 normal   : NORMAL;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                // UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in half2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex.xyz, _Flip);

                OUT.vertex = TransformWorldToHClip(TransformObjectToWorld(OUT.vertex.xyz));
                OUT.texcoord = IN.texcoord;

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
            #pragma multi_compile_instancing
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2DArray _Textures;
            SamplerState sampler_Textures;

            UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, _TextureIndex)
            UNITY_INSTANCING_BUFFER_END(Props)

            half4 _RendererColor;
            half2 _Flip;
            float _EnableExternalAlpha;


            half4 _Color;


            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float3 normal   : NORMAL;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                // UNITY_VERTEX_OUTPUT_STEREO
            };

            inline float4 UnityFlipSprite(in float3 pos, in half2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }

            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_TRANSFER_INSTANCE_ID(IN, OUT);

                OUT.vertex = UnityFlipSprite(IN.vertex.xyz, _Flip);

                OUT.vertex = TransformWorldToHClip(TransformObjectToWorld(OUT.vertex.xyz));
                OUT.texcoord = IN.texcoord;

                return OUT;
            }

            Texture2D _MainTex2;
            SamplerState sampler_MainTex2;
            Texture2D _MainTex3;
            SamplerState sampler_MainTex3;
            
            half4 SpriteFrag(v2f IN) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(IN);
                // Now we sample texture from Texture2DArray
                half4 c = _Textures.Sample(sampler_Textures, float3(IN.texcoord, UNITY_ACCESS_INSTANCED_PROP(Props, _TextureIndex)));
                // c = half4(IN.texcoord, 1.0, 1.0);
                half4 c2 = SAMPLE_TEXTURE2D(_MainTex2, sampler_MainTex2, IN.texcoord);
                half4 c3 = SAMPLE_TEXTURE2D(_MainTex3, sampler_MainTex3, IN.texcoord);
                half r = c.r * c2.r *c3.r;
                c.r = r;
                return c;
            }
            
            ENDHLSL
        }
    }
}
