Shader "Unlit/SplitColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex1 ("Mask Texture 1", 2D) = "white" {}
        _MaskTex2 ("Mask Texture 2", 2D) = "white" {}

        _MaskValue0 ("Mask Value 0", Vector) = (3.36, 1.39, 6.96, 1.90)
        _MaskValue1 ("Mask Value 1", Vector) = (0, -3.90, 0.10, 0.07)

        _Rotate("Rotate",float) = 0
        _Scale("Scale", float) = 0.0075
        _Offset("Offset",float) =(0,0,0,0)
        
        _BlurredRange("BlurredRange", Vector) = (0.2, 0.5, 0.1, 0.1)
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
            float4 _MaskValue0;
            float4 _MaskValue1;
            float _Rotate;
            float _Scale;
            float4 _Offset;
            int _StencilRef;
            float4 _BlurredRange;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 positionOS = v.vertex;
                float4 positionWS = positionOS;
                o.uv = v.uv;

                float4x4 scaleMatrix = float4x4(
                    _Scale, 0.0f, 0.0f, 0.0f,
                    0.0f, _Scale, 0.0f, 0.0f,
                    0.0f, 0.0f, _Scale, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );

                float4 t1 = mul(scaleMatrix, positionWS);

                float2 t2 = (float2(0.4, 0.0) * positionWS.ww) + t1.xy;
                t1 = float4(t2.x, t2.y, t1.z, t1.w);

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

                float step = 0.0;
                if (maskUV.x <= _BlurredRange.x)
                {
                    step = 1.0;
                }else if (maskUV.x > _BlurredRange.x && maskUV.x < _BlurredRange.y)
                {
                    float normalizedValue = (maskUV.x - _BlurredRange.x) / (_BlurredRange.y - _BlurredRange.x);
                    step = lerp(1, 0, normalizedValue) * _MaskValue0.z;
                }
                else
                {
                    step = 0.0;
                }
                
                float mask = maskTexValue1 * step;
                // mask = clamp(mask, 0, 1);
                // mask *= 0.5f;
                finalColor.xyz = mask;

                float2 mask2UV = (i.maskPositions.zw * _MaskTex2_ST.xy) + _MaskTex2_ST.zw;
                mask2UV.xy = 1.0 - mask2UV.xy;
                float maskTexValue2 = tex2D(_MaskTex2, mask2UV).r;

                // float mask2 = (maskTexValue2 * _MaskValue0.w) + mask;
                float mask2 =  maskTexValue2 * _MaskValue0.w +  mask;
                mask2 -= _MaskValue1.x;
                // mask2 = clamp(mask2, 0, 1);
                finalColor.xyz = mask2;

                float finalMask = mask2;
             
                
                return finalColor;
            }
            ENDCG
        }
    }
}