Shader "Hidden/DoomColorLimiter"
{
    Properties
    {
        _Tint ("Tint", Color) = (1.0, 1.0, 1.0, 1)
        _Saturation ("Saturation", Range(0, 2)) = 1.0
        _Brightness ("Brightness", Range(-1, 1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "ColorLimiter"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float4 _Tint;
            float _Saturation;
            float _Brightness;

            // 4x4 Bayer Matrix
            static const float4x4 bayer4x4 = float4x4(
                0.0, 8.0, 2.0, 10.0,
                12.0, 4.0, 14.0, 6.0,
                3.0, 11.0, 1.0, 9.0,
                15.0, 7.0, 13.0, 5.0
            ) / 16.0;

            half4 Frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // Evitar procesar si la cámara es nula o inválida
                if (any(isnan(color.rgb)) || any(isinf(color.rgb))) return half4(0,0,0,1);

                // 1. Ajustes base
                color.rgb += _Brightness;
                
                // 2. Saturación
                float luminance = dot(color.rgb, float3(0.2126, 0.7152, 0.0722));
                color.rgb = lerp((half3)luminance, color.rgb, _Saturation);

                // 3. Tinte
                color.rgb *= _Tint.rgb;

                // 4. Dithering (alineado a 320x240)
                float2 screenPos = floor(uv * float2(320, 240));
                int x = (int)fmod(screenPos.x, 4);
                int y = (int)fmod(screenPos.y, 4);
                float dither = bayer4x4[x][y];
                
                // Cuantización a 5 bits (32 niveles)
                float levels = 31.0;
                color.rgb = saturate(color.rgb); // Importante: Limitar antes de cuantizar
                color.rgb = floor(color.rgb * levels + (dither - 0.5)) / levels;

                return half4(color.rgb, 1.0);
            }
            ENDHLSL
        }
    }
}


