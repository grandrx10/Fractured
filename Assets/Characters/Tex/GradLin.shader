Shader "Custom/CelGradients"
{
    Properties
    {
        _Color_1("Base Color 1", Color) = (1,1,1,1)
        _Color_2("Base Color 2", Color) = (0,0,0,1)
        _Range_Min("Range Min", Float) = 0
        _Range_Max("Range Max", Float) = 1
        _Normal("Normal/Center", Vector) = (0,1,0,0)
        _Shadow("Shadows", Range(0,1)) = 0.75
        [Toggle()] _Rad("Toggle (Linear/Radial)", float) = 0
        _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline Width", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

        // First pass: outline
        Pass
        {
            Name "Outline"
            Tags {}
            Cull Front // Render backfaces
            ZWrite On
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vertOutline
            #pragma fragment fragOutline
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : TEXCOORD1;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float c : SV;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vertOutline(Attributes v)
            {
                Varyings o;
                float3 normal = v.normalOS.xyz;
                float3 posOffset = v.positionOS.xyz + normal * _OutlineWidth;
                o.positionCS = TransformObjectToHClip(posOffset);
                return o;
            }

            half4 fragOutline(Varyings i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        // Second pass: cel shading with lighting
        Pass
        {
            Name "CelShading"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 color      : COLOR;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 positionLS : POS;
            };

            float4 _Color_1;
            float4 _Color_2;
            float _Range_Min;
            float _Range_Max;
            float _Rad;
            float _Shadow;
            float3 _Normal;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionLS = v.color.xyz;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Main directional light
                Light mainLight = GetMainLight(float4(i.positionWS, 0));

                float t;
                if (!_Rad)
                {
                    float dist = dot(normalize(_Normal.xyz), i.positionLS.xzy);
                    t = Remap(_Range_Min, _Range_Max, 0, 1, dist);
                }
                else
                {
                    float dist = distance(_Normal, i.positionLS.xzy);
                    t = Remap(_Range_Min, _Range_Max, 0, 1, dist);
                }
                
                // Diffuse N·L
                float NdotL = dot(normalize(i.normalWS), mainLight.direction);

                // Sample ramp texture for toon bands
                float ramp = 1-step(NdotL, _Shadow * 2 - 1)*0.7;

                // Multiply by main light attenuation (shadows)
                ramp *= mainLight.shadowAttenuation;

                float3 color = lerp(_Color_1, _Color_2, 1-saturate(t)) * ramp;

                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}
