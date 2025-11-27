Shader "Hidden/Distortion"
{
    Properties
    {
        _Intensity ("Displacement value", Range(0,1)) = 0.05        // 왜곡 강도
        _Texture   ("Displacement map (RGB)", 2D)        = "black" {}
        _ValueX    ("Noise value", Range(0,10))          = 4.5      // 노이즈 스케일
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        LOD 100

        ZWrite Off
        Cull Off
        ZTest Always

        Pass
        {
            Name "DistortionPass"

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // Vert, Varyings, Attributes 제공
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            // 카메라 컬러 텍스처 (공식 예제와 동일 패턴)
            TEXTURE2D_X(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            // 노이즈 패턴 텍스처
            TEXTURE2D(_Texture);
            SAMPLER(sampler_Texture);

            float _ValueX;
            float _Intensity;

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                // 패턴 텍스처에서 노이즈 값 샘플
                float3 n = SAMPLE_TEXTURE2D(_Texture, sampler_Texture, uv).rgb;
                float noise = dot(n, float3(0.299, 0.587, 0.114)); // 밝기 기반

                // X 방향으로 UV를 밀어주기
                uv.x += noise * _ValueX * _Intensity;

                // 카메라 컬러 텍스처 샘플
                half4 col = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, uv);
                return col;
            }

            ENDHLSL
        }
    }
}
