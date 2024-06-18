Shader "Unlit/Ring/Stencil"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Transparency ("Transparency", Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry+1"
        }

        Pass
        {
            ZTest Off
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil
            {
                Ref 1
                ReadMask 1
                CompBack NotEqual
            }

            Tags
            {
                "LightMode" = "UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Assets/2D Occlusion/Common.Variable.hlsl"
            #include "Assets/2D Occlusion/Common.Function.hlsl" 
            ENDHLSL
        }
    }
}