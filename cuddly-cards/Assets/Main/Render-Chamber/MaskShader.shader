Shader "Custom/Portal"
{
    Properties
    {
        _InactiveColour("Inactive Colour", Color) = (1, 1, 1, 1)
        srpBatcherFix("srpBatcherFix", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
#pragma fragment frag
# include "UnityCG.cginc"

            float srpBatcherFix;

struct appdata
{
    float4 vertex : POSITION;
            };

struct v2f
{
    float4 vertex : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };

sampler2D _MainTex;
float4 _InactiveColour;


v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.screenPos = ComputeScreenPos(o.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target
{
    float2 uv = i.screenPos.xy / i.screenPos.w;
    fixed4 portalCol = tex2D(_MainTex, uv);
    // this is just used to make it not compatible with the srpbatcher. I hate that you can't turn it off manually.
    return portalCol * srpBatcherFix;
}
ENDCG
        }
    }
    Fallback "Standard" // for shadows
}
