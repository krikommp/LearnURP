Shader "Unlit/Common/AlphaMask"
{
    Properties
    {
         _MainTex ("Texture", 2D) = "white" {}
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
            // blend srcAlpha oneMinusSrcAlpha = srcColor * srcAlpha + dstColor * (1 - srcAlpha)
            Blend OneMinusSrcAlpha DstAlpha
            Tags
            {
                "LightMode" = "AlphaMask"
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/2D Occlusion/Common.Variable.hlsl"
            #include "AlphaMask.Function.hlsl"
            ENDHLSL
        }
    }
}