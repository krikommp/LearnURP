Shader "Unlit/SplitColor2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex1 ("Mask Texture 1", 2D) = "white" {}
        _MaskTex2 ("Mask Texture 2", 2D) = "white" {}

        _Rotate("Rotate",float) = 0
        _Offset("Offset",float) =(0,0,0,0)

        _MaskScale("Mask Scale", float) = 0
        _MaskAttenuation("Mask Attenuation", float) = 0
        _MaskCoverage("Mask Coverage", float) = 0
        _Mask1Strength("Mask1 Strength", float) = 0.5
        _Mask2Strength("Mask2 Strength", float) = 0.5
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque-1"
        }

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
                float4 vertex : SV_POSITION;
                float4 maskPositions : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            // _child2 1.00, 1.00, 0.00, 0.00 32 float4
            sampler2D _MaskTex1;
            float4 _MaskTex1_ST;
            // _child3 2.00, 2.00, 0.42, 1.06 48 float4
            sampler2D _MaskTex2;
            float4 _MaskTex2_ST;
            float _Rotate;

            float4 _Offset;
            int _StencilRef;
            float _MaskScale;
            float _MaskAttenuation;
            float _MaskCoverage;
            float _Mask1Strength;
            float _Mask2Strength;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 positionOS = v.vertex;
                float4 positionWS = positionOS;
                o.uv = v.uv;

                float4x4 scaleMatrix = float4x4(
                    _MaskScale, 0.0f, 0.0f, 0.0f,
                    0.0f, _MaskScale, 0.0f, 0.0f,
                    0.0f, 0.0f, _MaskScale, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );

                float4 t1 = mul(scaleMatrix, positionWS);
                float2 t3 = t1.xy * float2(1.0, 0.5);

                o.maskPositions = float4(positionWS.x, positionWS.y, t3.x, t3.y);

                return o;
            }

            void Mask(v2f i)
            {
            }

            float2 RotateUV(float rotate, float2 uv)
            {
                float2 newUV = uv;
                float angle = rotate * 0.017453292519943295;
                newUV -= float2(0.5, 0.5);
                newUV = float2(newUV.x * cos(angle) - newUV.y * sin(angle),
                               newUV.y * cos(angle) + newUV.x * sin(angle));
                newUV += float2(0.5, 0.5);
                return newUV;
            }

            float2 OffsetUV(float2 offset, float2 uv)
            {
                float2 newUV = uv;
                newUV += offset.xy;
                return newUV;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 finalColor = float4(1.0, 1.0, 1.0, 1.0);

                float2 mainUV = i.uv;
                mainUV = OffsetUV(_Offset, mainUV);
                mainUV = RotateUV(_Rotate + 90, mainUV);
                float4 mainTex = tex2D(_MainTex, mainUV);

                float2 maskUV = RotateUV(_Rotate, i.uv);
                float2 mask1UV = (i.maskPositions.zw * _MaskTex1_ST.xy) + _MaskTex1_ST.zw;
                float maskTexValue1 = tex2D(_MaskTex1, mask1UV).r;
                float mask1 = (maskTexValue1 * _Mask1Strength);

                // 除非第一张 mask 图是全白的
                // 否则都拿 0 作为最小值来计算
                // 完全白, 那么 maskUV = 1, maskTexValue1 = 0, ( 1 * _MaskValue0.x ) + ( 0 * _MaskValue0.y) = _MaskValue0.x
                // 完全黑, 那么 maskUV = 0 maskTexValue1 = 1, ( 0 * _MaskValue0.x ) + (1 * _MaskValue0.y) = _MaskValue0.y

                // float mask = (maskUV.x *_MaskCoverage) + (maskTexValue1 * _Mask1Strength);
                // float mask = maskUV.x + maskTexValue1;
                // finalColor.x = (maskUV.x * _MaskValue0.x) + _MaskValue0.y;
                mask1 *= 0.5f;
                // [_MaskValue0.y * 0.5, _MaskValue0.x * 0.5]

                float2 mask2UV = (i.maskPositions.zw * _MaskTex2_ST.xy) + _MaskTex2_ST.zw;
                float maskTexValue2 = tex2D(_MaskTex2, mask2UV).r;
                float mask2 = (maskTexValue2 * _Mask2Strength);

                float mask = (maskUV.x * _MaskCoverage) + mask1 + mask2;

                mask = mask - _MaskAttenuation;
                mask2 = clamp(mask2, 0.0, 1.0);
                finalColor.xyz = mask;


                float finalMask = mask2 + mainTex.r;
                return finalColor;
            }
            ENDCG
        }
    }
}