Shader "Custom/CelTexture"
{
    Properties
    {
        _Color_1("Base Color", Color) = (1,1,1,1)
        _MainTex("Main Texture", 2D) = "white" {}
        _Range_Min("Range Min", Float) = 0
        _Range_Max("Range Max", Float) = 1
        _Shadow("Shadows", Range(0,1)) = 1
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
                float3 normal = v.normalOS.xzy * float3(1, 1, -1);
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
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 positionLS : POS;
                float2 uv : TEXCOORD2;
            };
            
            sampler2D _MainTex;
            float4 _Color_1;
            float _Shadow;

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.positionLS = v.color.xyz;
                o.normalWS = TransformObjectToWorldNormal(v.normalOS);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                // Main directional light
                Light mainLight = GetMainLight(float4(i.positionWS, 0));

                
                // Diffuse N·L
                float NdotL = dot(normalize(i.normalWS), mainLight.direction);

                // Sample ramp texture for toon bands
                float ramp = 1-step(NdotL, _Shadow*2-1)*0.7;

                // Multiply by main light attenuation (shadows)
                ramp *= mainLight.shadowAttenuation;

                float3 color = tex2D(_MainTex, i.uv) * _Color_1 * ramp;

                return half4(color, 1);
            }
            ENDHLSL
        }
    }
}
