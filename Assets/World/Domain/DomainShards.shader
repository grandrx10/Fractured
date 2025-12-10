Shader "Custom/CubemapProjectWithOrig"
{
    Properties
    {
        _Cube ("Cubemap", CUBE) = "" {}
        _CamPos ("Camera Pos", Vector) = (0,0,0,0)
        _UseOrigPos("Use Original Position", Float) = 0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Opaque" }
        Cull Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            samplerCUBE _Cube;
            float3 _CamPos;
            float _UseOrigPos;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv2 : TEXCOORD1; // x,y of original world pos
                float2 uv3 : TEXCOORD2; // z,0 of original world pos
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

                if (_UseOrigPos > 0.5)
                {
                    worldPos = float3(v.uv2.x, v.uv2.y, v.uv3.x);
                }

                o.worldPos = worldPos;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.worldPos - _CamPos);
                fixed4 col = texCUBE(_Cube, dir);
                return col;
            }
            ENDCG
        }
    }
}
