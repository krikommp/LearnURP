Varyings vert(Attributes IN)
{
    Varyings OUT;
    OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
    return OUT;
}

half4 fontfrag(Varyings IN) : SV_Target
{
    half4 col = tex2D(_MainTex, IN.uv);
    half4 finalColor = _Color * col;

    return finalColor;
}

half4 backfrag(Varyings IN, bool isFrontFace : SV_IsFrontFace) : SV_Target
{
    half4 col = tex2D(_MainTex, IN.uv);
    half4 finalColor = _Color * col;

    float2 uv = IN.positionHCS.xy / _ScaledScreenParams.xy;
  
    half4 alphaMask = tex2D(_AlphaMask, uv);
    half finalAlpha = 1.0 - alphaMask.a;;
    finalColor.a = lerp(finalAlpha * finalColor.a, finalColor.a, isFrontFace);

    return finalColor;
}
