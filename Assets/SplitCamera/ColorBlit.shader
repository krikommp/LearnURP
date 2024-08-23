Shader "ColorBlit"
{
    Properties
    {
        _Split1CameraTexture("Split1 Camera Texture", 2D) = "white" {}
        _Split2CameraTexture("Split2 Camera Texture", 2D) = "white" {}
    }
    
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        ZWrite Off Cull Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_GrabbedTexture);
            SAMPLER(sampler_GrabbedTexture);
            TEXTURE2D_X(_Split1CameraTexture);
            SAMPLER(sampler_Split1CameraTexture);
            TEXTURE2D_X(_Split2CameraTexture);
            SAMPLER(sampler_Split2CameraTexture);
            

            float _Intensity;

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float4 color = SAMPLE_TEXTURE2D_X(_GrabbedTexture, sampler_GrabbedTexture, input.texcoord);
                // return color * float4(0, _Intensity, 0, 1);

                float4 split1Color = SAMPLE_TEXTURE2D_X(_Split1CameraTexture, sampler_Split1CameraTexture, input.texcoord);
                float4 split2Color = SAMPLE_TEXTURE2D_X(_Split2CameraTexture, sampler_Split2CameraTexture, input.texcoord);

                float4 splitColor = split1Color + split2Color;

                // return color;

                // if (color.a > 0)
                // {
                //     return float4(color.r * color.a + split1Color.r * (1 - color.a), color.g * color.a + splitColor.g * (1 - color.a), color.b * color.a + splitColor.b * (1 - color.a), 1);
                // }
                return splitColor;
            }
            ENDHLSL
        }
    }
}