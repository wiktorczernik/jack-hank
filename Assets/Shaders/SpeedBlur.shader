Shader "PostProcessing/SpeedBlur"
{
    Properties
    {
        _BlurMap ("Blur Map", 2D) = "black" {}
        _BlurThreshold ("Blur Threshold", float) = 0.2
        _BlurSize ("Blur Size", float) = 5
        _Level ("Level", float) = 0
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "Blit"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Debug/DebuggingFullscreen.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            SAMPLER(sampler_BlitTexture);

            sampler2D _BlurMap;
            // Poziom intensywnoœci bluru zadawany przez skrypty
            half _Level;
            // Minimalna wymagana intensywnoœæ do osi¹gniêcia bluru
            half _BlurThreshold;
            half _BlurSize;


            half4 Frag(Varyings input) : SV_Target
            {
                // Intensywnoœæ bluru wedle tekstury
                half intensity = tex2D(_BlurMap, input.texcoord);
                // Intensywnoœæ bluru w danym punkcie pomno¿one przez zadany poziom bluru
                half leveledIntensity = intensity * _Level;

                
                if (leveledIntensity <= _BlurThreshold) return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                half screenXfrag = 1 / _ScreenParams.x;
                screenXfrag *= _BlurSize * leveledIntensity;
                
                half4 outputColor = 0;
                outputColor += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + half4(-screenXfrag, 0, 0,0));
                outputColor += SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + half4(screenXfrag, 0, 0,0));
                outputColor /= 2;

                return outputColor;
            }

            ENDHLSL
        }
    }
}