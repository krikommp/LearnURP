Shader "Unlit/MeshSRPBatch"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _MainTex2 ("Sprite Texture2", 2D) = "white" {}
        _MainTex3 ("Sprite Texture3", 2D) = "white" {}
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
            Cull Off
            Lighting Off
            ZWrite Off
            ZTest Off
            Blend One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            Texture2D _MainTex2;
            SamplerState sampler_MainTex2;
            Texture2D _MainTex3;
            SamplerState sampler_MainTex3;
            

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
                half4 c = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
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