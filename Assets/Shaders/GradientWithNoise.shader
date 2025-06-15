Shader "Custom/GradientWithNoise"
{
    Properties
    {
        _GradientColorTop ("Gradient Color Top", Color) = (0.5, 0, 0.5, 1) // 深紫色
        _GradientColorBottom ("Gradient Color Bottom", Color) = (0.8, 0.4, 0.8, 1) // 浅紫色
        _GradientTopPosition ("Gradient Top Position", Float) = 10.0 // 控制渐变的世界空间顶点位置
        _GradientBottomPosition ("Gradient Bottom Position", Float) = 0.0 // 控制渐变的世界空间底部位置
        _TransparencyStart ("Transparency Start", Range(0,1)) = 0.0
        _TransparencyEnd ("Transparency End", Range(0,1)) = 1.0
        _NoiseTex ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 10.0
        _NoiseColor ("Noise Color", Color) = (1.0, 0.5, 0.0, 1.0) // 橙色
        _NoiseIntensity ("Noise Intensity", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _GradientColorTop;
            fixed4 _GradientColorBottom;
            float _GradientTopPosition;
            float _GradientBottomPosition;
            float _TransparencyStart;
            float _TransparencyEnd;
            sampler2D _NoiseTex;
            float _NoiseScale;
            fixed4 _NoiseColor;
            float _NoiseIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0; // 世界空间位置
                float3 normal : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 获取顶点在世界空间的坐标
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 基于世界空间的 Y 坐标进行渐变计算
                float height = saturate((i.worldPos.y - _GradientBottomPosition) / (_GradientTopPosition - _GradientBottomPosition));

                // 颜色渐变
                fixed4 baseColor = lerp(_GradientColorBottom, _GradientColorTop, height);

                // 透明度渐变
                float alpha = lerp(_TransparencyStart, _TransparencyEnd, height);
                baseColor.a = alpha;

                // 采样噪声纹理
                float2 noiseUV = i.worldPos.xz * _NoiseScale;
                float noise = tex2D(_NoiseTex, noiseUV).r;

                // 噪点颜色
                fixed4 noiseColor = lerp(fixed4(0,0,0,0), _NoiseColor, noise) * _NoiseIntensity;

                // 结合基色和噪点
                fixed4 finalColor = lerp(baseColor, noiseColor, noise);

                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}