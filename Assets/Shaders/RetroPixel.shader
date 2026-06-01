Shader "Hidden/RetroPixel"
{
    Properties
    {
        _PixelSize ("Pixel Size", Float) = 4.0
        _PosterizeSteps ("Posterize Steps", Float) = 16.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "Pixelation"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl includes the necessary functions to sample the source color buffer.
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _PixelSize;
            float _PosterizeSteps;

            half4 Frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                
                // Pixelation
                float2 res = _ScreenParams.xy / _PixelSize;
                float2 pixelatedUV = floor(uv * res) / res;

                half4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, pixelatedUV);

                // Posterization (Color Depth reduction)
                if (_PosterizeSteps > 0)
                {
                    color.rgb = floor(color.rgb * _PosterizeSteps) / _PosterizeSteps;
                }

                return color;
            }
            ENDHLSL
        }
    }
}
