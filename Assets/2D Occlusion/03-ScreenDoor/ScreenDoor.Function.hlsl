half DitheredAlpha(half inputAlpha, half transparency, float4 positionCS)
{
    // #if !defined(_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A) && !defined(_GLOSSINESS_FROM_BASE_ALPHA)
    // half alpha = albedoAlpha * transparency;
    // #else
    half alpha = transparency;
    // #endif
 
    float DITHER_THRESHOLDS[16] =
    {
        1.0 / 17.0,  9.0 / 17.0,  3.0 / 17.0, 11.0 / 17.0,
        13.0 / 17.0,  5.0 / 17.0, 15.0 / 17.0,  7.0 / 17.0,
        4.0 / 17.0, 12.0 / 17.0,  2.0 / 17.0, 10.0 / 17.0,
        16.0 / 17.0,  8.0 / 17.0, 14.0 / 17.0,  6.0 / 17.0
    };
   
    float2 uv = positionCS.xy / _ScaledScreenParams.xy;
    uv *= _ScreenParams.xy;  
    uint index = (uint(uv.x) % 4) * 4 + uint(uv.y) % 4;
    // Returns > 0 if not clipped, < 0 if clipped based
    // on the dither
    clip(alpha - DITHER_THRESHOLDS[index]);
 
    return inputAlpha;
}

Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
    return OUT;
}


half4 frag(Varyings IN) : SV_Target
{
    half4 col = tex2D(_MainTex, IN.uv);
    half4 finalColor = _Color * col;

    finalColor.a = DitheredAlpha(finalColor, _Transparency, IN.positionHCS);
    
    return finalColor;
}