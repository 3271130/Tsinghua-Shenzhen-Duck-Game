Shader "Nicrom/LPW/ASE/Low Poly Vegetation Gradient"
{
    Properties
    {
        [Header(Surface)][Space]_Color("Color", Color) = (1,1,1,1)
        [NoScaleOffset]_MainTex("Main Texture", 2D) = "white" {}
        [Space]_Metallic("Metallic", Range(0, 1)) = 0
        _Smoothness("Smoothness", Range(0, 1)) = 0
        [Header(Main Bending)][Space]_MBDefaultBending("MB Default Bending", Float) = 0
        [Space]_MBAmplitude("MB Amplitude", Float) = 1.5
        _MBAmplitudeOffset("MB Amplitude Offset", Float) = 2
        [Space]_MBFrequency("MB Frequency", Float) = 1.11
        _MBFrequencyOffset("MB Frequency Offset", Float) = 0
        [Space]_MBPhase("MB Phase", Float) = 1
        [Space]_MBWindDir("MB Wind Dir", Range(0, 360)) = 0
        _MBWindDirOffset("MB Wind Dir Offset", Range(0, 180)) = 20
        [Space]_MBMaxHeight("MB Max Height", Float) = 10
        [NoScaleOffset][Header(World Space Noise)][Space]_NoiseTexture("Noise Texture", 2D) = "bump" {}
        _NoiseTextureTilling("Noise Tilling - Static (XY), Animated (ZW)", Vector) = (1, 1, 1, 1)
        _NoisePannerSpeed("Noise Panner Speed", Vector) = (0.05, 0.03, 0, 0)
        [Header(Gradient)][Space]_GradientColorStart("Gradient Start Color", Color) = (0,1,0,1)
        _GradientColorEnd("Gradient End Color", Color) = (0,0,1,1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float gradientFactor : TEXCOORD1; // Y轴渐变系数
            };

            // Uniform variables
            float _MBWindDir;
            float _MBWindDirOffset;
            float _MBAmplitude;
            float _MBFrequency;
            float _MBPhase;
            float _MBMaxHeight;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float4 _GradientColorStart;
            float4 _GradientColorEnd;

            v2f vert(appdata_t v)
            {
                v2f o;
                float time = _Time.y;
                float windDirRadians = (_MBWindDir + _MBWindDirOffset) * 3.14159 / 180.0;

                // Wind calculations
                float waveX = sin(windDirRadians) * (_MBAmplitude * smoothstep(0.0, _MBMaxHeight, v.vertex.y));
                float waveZ = cos(windDirRadians) * (_MBAmplitude * smoothstep(0.0, _MBMaxHeight, v.vertex.y));
                float wave = sin((v.vertex.x * _MBFrequency + time + _MBPhase)) * waveX;
                v.vertex.x += wave;
                v.vertex.z += waveZ;

                // Apply gradient factor (normalized Y value)
                o.gradientFactor = saturate(v.vertex.y / _MBMaxHeight); // Clamps between 0 and 1

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Calculate gradient color
                half4 gradientColor = lerp(_GradientColorStart, _GradientColorEnd, i.gradientFactor);

                // Sample the main texture
                half4 texColor = tex2D(_MainTex, i.uv) * _Color;

                // Combine texture color with gradient
                return texColor * gradientColor;
            }
            ENDHLSL
        }
    }
    FallBack "Diffuse"
}