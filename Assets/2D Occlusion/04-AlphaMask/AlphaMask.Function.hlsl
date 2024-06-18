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
    half alpha = col.a;
    return half4(alpha, alpha, alpha, alpha);
}
              