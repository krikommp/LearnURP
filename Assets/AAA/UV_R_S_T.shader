Shader "Unlit/UV_R_S_T"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle(OpenScale)]_OpenScale("OpenScale",float)=0
        _Scale("Scale",Range(0,10)) = 1
        [Toggle(OpenRotate)]_OpenRotate("OpenScale",float)=0
        _Rotate("Rotate",float) = 0
        [Toggle(OpenOffset)]_OpenOffset("OpenOffset",float)=0
        _Offset("Offset",float) =(0,0,0,0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _OpenScale;
            float _Scale;
            float _OpenRotate;
            float _Rotate;
            float _OpenOffset;
            float4 _Offset;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 newUV = i.uv;
                // if (_OpenOffset)newUV += _Offset.xy;
                // if (_OpenScale)
                // {
                //     newUV -= float2(0.5, 0.5);
                //     newUV *= _Scale;
                //     newUV += float2(0.5, 0.5);
                // }
                if (_OpenRotate)
                {
                    float angle = _Rotate * 0.017453292519943295;
                    newUV -= float2(0.5, 0.5);
                    newUV = float2(newUV.x * cos(angle) - newUV.y * sin(angle),
                                   newUV.y * cos(angle) + newUV.x * sin(angle));
                    newUV += float2(0.5, 0.5);
                }

                fixed4 col = tex2D(_MainTex, newUV);
                return col;
            }
            ENDCG
        }
    }
}