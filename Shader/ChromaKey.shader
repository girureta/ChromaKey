Shader "Hidden/Custom/ChromaKey"
{
    HLSLINCLUDE

        #include "Assets/PostProcessing/Shaders/StdLib.hlsl"
		#include "Assets/PostProcessing/Shaders/Colors.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
		TEXTURE2D_SAMPLER2D(_ReplaceTex, sampler_ReplaceTex);
        float _Distance;
		float4 _ScreenColor;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
			float4 colorReplaceTex = SAMPLE_TEXTURE2D(_ReplaceTex, sampler_ReplaceTex, i.texcoord);

			float4 colorHSV = float4(RgbToHsv(color),1);
			float4 colorReplaceTexHSV = float4(RgbToHsv(_ScreenColor), 1);

			float d = distance(colorHSV[0], colorReplaceTexHSV[0]);
            if(d<_Distance)
			{
				color.rgb = colorReplaceTex;
			}
            return color;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}