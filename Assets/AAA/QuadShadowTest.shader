Shader "Unlit/QuadShadowTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

        _CloudTex ("ColorTexture", 2D) = "white" {}
        _SpeedX("Cloud SpeedX", Float) = 0.05
        _SpeedY("Cloud SpeedY", Float) = 0.05
        _CloudShadowParams("Cloud Shadow Params", Vector) = (0, 0, 1, 1)

        [Toggle] _CLOUD_SHADOW("Cloud Shadow", Float) = 0
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
            ZWrite On
            ZTest LEqual
            Blend One OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature_local_fragment _CLOUD_SHADOW_ON

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
                float3 worldPos : TEXCOORD1;
            };

            Texture2D _CloudTex;
            float4 _CloudTex_ST;
            SamplerState sampler_CloudTex;
            half _SpeedX;
            half _SpeedY;
            half4 _CloudShadowParams;
            
            #define _Coverage _CloudShadowParams.x
            #define _Softness _CloudShadowParams.y
            #define _Opacity _CloudShadowParams.z
            #define _Scale _CloudShadowParams.w
            
            Texture2D _MainTex;
            float4 _MainTex_ST;
            SamplerState sampler_MainTex;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = TransformObjectToHClip(IN.vertex.xyz);
                OUT.worldPos = TransformObjectToWorld(IN.vertex.xyz);
                OUT.texcoord = IN.texcoord;
                return OUT;
            }

            // 优化指令
            float CalculateAlpha(float2 cloudUV)
            {
                float stepValue = -_Softness + (1 - _Coverage) * (1 + (_Softness * 2));

                float coverageMin = stepValue - _Softness;
                float coverageMax = stepValue + _Softness;

                float alpha = SAMPLE_TEXTURE2D(_CloudTex, sampler_CloudTex, cloudUV).g;
                alpha = smoothstep(coverageMin, coverageMax, alpha);

                return alpha;
            }

            #define BlendSubtract(base, blend)		(blend - base)
            #define BlendOpacity(base, blend, function, opacity)	(function(base, blend) * opacity + blend * (1.0 - opacity))

            half4 frag(v2f IN) : SV_Target
            {
                float4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.texcoord);
                #if _CLOUD_SHADOW_ON
                float2 worldUV = float2(IN.worldPos.x, IN.worldPos.z) + _CloudTex_ST.zw;
                float2 cloudUV = worldUV * _Scale * 0.005f;
                cloudUV = (cloudUV + _Time.y * float2(_SpeedX, _SpeedY)) * _CloudTex_ST.xy;

                half cloud = CalculateAlpha(cloudUV);
                cloud = BlendOpacity(cloud, 1.0f, BlendSubtract, _Opacity);
                
                float3 col = cloud * mainTex.rgb;

                return float4(col, mainTex.a);
                #else
                return mainTex;
                #endif
            }
            ENDHLSL
        }
    }
}