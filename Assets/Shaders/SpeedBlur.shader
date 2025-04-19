Shader "PostProcessing/SpeedBlur"
{
    Properties
    {
        _BlurMap ("BlurMap", 2D) = "black" {}
        _Width ("Width", int) = 230
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
            float _Width;
            float _Level;

            float lvl(int x) {
                if (x <= _Width * _Level) return x / _Width * _Level;
                return 1;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                int m = 1;
                int M = 3;

                float red = tex2D(_BlurMap, input.texcoord);
                int size = round(red * (M - m)) + m;

                if (red <= 1 - _Level) return SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord);

                half4 col = 0;

                if (red <= 1.25 - _Level) {
                    for (int i = -1; i <= 1; i++) {
                        for (int j = -1; j <= 1; j++) {
                            col = col + SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + float4(i/_ScreenParams.x,j/_ScreenParams.y,0,0));
                        }
                    }
                    return col / 9;
                }
                else if (red <= 1.50 - _Level) {
                    for (int i = -3; i <= 3; i++) {
                        for (int j = -3; j <= 3; j++) {
                            col = col + SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + float4(i/_ScreenParams.x,j/_ScreenParams.y,0,0));
                        }
                    }
                    return col / 49; // UWAGA!!!!111!1!!!!!1!
                    // TA MAGICZNA LICZBA OZNACZA -> -> ->::::::
                    // œredni¹ arytmetyczn¹ wszytskich kana³ów koloru zmiennej 'col' (box blur), tak jak inne magiczne liczby przy return
                }
                else {
                    for (int i = -5; i <= 5; i++) {
                        for (int j = -5; j <= 5; j++) {
                            col = col + SAMPLE_TEXTURE2D(_BlitTexture, sampler_BlitTexture, input.texcoord + float4(i/_ScreenParams.x,j/_ScreenParams.y,0,0));
                        }
                    }
                    return col / 121;
                }
            }

            ENDHLSL
        }
    }
}