Shader "Unlit/SplitColor2"
{
    Properties
    {
        _MaskTex1 ("Mask Texture 1", 2D) = "white" {}
        _MaskTex2 ("Mask Texture 2", 2D) = "white" {}

        _SplitRotate("Split Rotate",float) = 0
        _SplitScreenRatio("Split Screen Ratio", Range(0, 1)) = 0.5
        _MaskScale("Mask Scale", float) = 0
        _MaskAttenuation("Mask Attenuation", Range(0, 1)) = 0
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
                float2 uvMinMax : TEXCOORD3;
            };

            // _child2 1.00, 1.00, 0.00, 0.00 32 float4
            sampler2D _MaskTex1;
            float4 _MaskTex1_ST;
            // _child3 2.00, 2.00, 0.42, 1.06 48 float4
            sampler2D _MaskTex2;
            float4 _MaskTex2_ST;
            int _StencilRef;
            float _MaskScale;
            float _MaskAttenuation;
            float _MaskCoverage;
            float _Mask1Strength;
            float _Mask2Strength;
            float _SplitScreenRatio;
            float _SplitRotate;

            float2 RotatePoint(float2 uv, float angle)
            {
                float2 result;
                const float2 center = float2(0.5, 0.5);
                float2 uvCentered = uv - center;
                result.x = uvCentered.x * cos(angle) - uvCentered.y * sin(angle);
                result.y = uvCentered.x * sin(angle) + uvCentered.y * cos(angle);
                result += center;
                return result;
            }

            float2 CalculateUVMinMax(float angle)
            {
                // 定义原始四个角点
                float2 points[4] = {
                    float2(0.0, 0.0),
                    float2(1.0, 0.0),
                    float2(0.0, 1.0),
                    float2(1.0, 1.0)
                };

                // 初始化最小值和最大值
                float minX = 1.0;
                float maxX = 0.0;

                // 旋转每个点，并计算最小和最大x值
                for (int i = 0; i < 4; i++)
                {
                    float2 rotated = RotatePoint(points[i], angle);
                    minX = min(minX, rotated.x);
                    maxX = max(maxX, rotated.x);
                }

                return float2(minX, maxX);
            }

            float2 CalculateMaskRange(float2 uvMinMax, float maskCoverage, float mask1Strength, float mask2Strength)
            {
                // uvmax * _MaskCoverage - _MaskAttenuation = 1.0
                // uvmin * _MaskCoverage + (1 * _Mask1Strength) + (1 * _Mask2Strength) - _MaskAttenuation = 1.0

                float maskMax = uvMinMax.y * maskCoverage;
                float maskMin = uvMinMax.x * maskCoverage + mask1Strength + mask2Strength;

                return float2(maskMin, maskMax);
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 positionOS = v.vertex;
                o.uv = v.uv;

                float4x4 scaleMatrix = float4x4(
                    _MaskScale, 0.0f, 0.0f, 0.0f,
                    0.0f, _MaskScale, 0.0f, 0.0f,
                    0.0f, 0.0f, _MaskScale, 0.0f,
                    0.0f, 0.0f, 0.0f, 1.0f
                );

                float4 t1 = mul(scaleMatrix, positionOS);
                float2 t3 = t1.xy * float2(1.0, 0.5);

                o.maskPositions = float4(positionOS.x, positionOS.y, t3.x, t3.y);
                o.uvMinMax = CalculateUVMinMax(radians(_SplitRotate));

                return o;
            }

            float SplitScreen(float2 uv, float2 uvMinMax)
            {
                float angle = radians(_SplitRotate);

                float2 rotatedUV = RotatePoint(uv, angle);

                float threshold = lerp(uvMinMax.x, uvMinMax.y, _SplitScreenRatio);

                return 1.0 - step(threshold, rotatedUV.x);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float4 finalColor = float4(1.0, 1.0, 1.0, 1.0);

                float2 maskUV = RotatePoint(i.uv, radians(_SplitRotate));
                float2 mask1UV = (i.maskPositions.zw * _MaskTex1_ST.xy) + _MaskTex1_ST.zw;
                float maskTexValue1 = tex2D(_MaskTex1, mask1UV).r;
                float mask1 = (maskTexValue1 * _Mask1Strength);

                mask1 *= 0.5f;
                // [_MaskValue0.y * 0.5, _MaskValue0.x * 0.5]

                float2 mask2UV = (i.maskPositions.zw * _MaskTex2_ST.xy) + _MaskTex2_ST.zw;
                float maskTexValue2 = tex2D(_MaskTex2, mask2UV).r;
                float mask2 = (maskTexValue2 * _Mask2Strength);

                // uvmax * _MaskCoverage - _MaskAttenuation = 1.0
                // uvmin * _MaskCoverage + (1 * _Mask1Strength) + (1 * _Mask2Strength) - _MaskAttenuation = 1.0

                // 
                float mask = (maskUV.x * _MaskCoverage) + mask1 + mask2;
                float2 maskRange = CalculateMaskRange(i.uvMinMax, _MaskCoverage, _Mask1Strength, _Mask2Strength);
                mask = mask - lerp(maskRange.x, maskRange.y, _MaskAttenuation);
                mask = clamp(mask, 0.0, 1.0);
                finalColor.xyz = mask + SplitScreen(i.uv, i.uvMinMax);

                return finalColor;
            }
            ENDCG
        }
    }

    // CustomEditor "SplitScreenEditor"
}