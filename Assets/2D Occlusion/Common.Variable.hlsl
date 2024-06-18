struct Attributes
{
    float4 positionOS : POSITION;
    float2 uv : TEXCOORD0;
};

struct Varyings
{
    float4 positionHCS : SV_POSITION;
    float2 uv : TEXCOORD0;
};

float _Transparency;
float4 _Color;
sampler2D _MainTex;
float4 _MainTex_ST;
