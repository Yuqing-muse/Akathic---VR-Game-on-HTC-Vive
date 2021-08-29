Shader "Custom/GrayScale" {
	Properties 
	{
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_SecondTex ("Add (RGB)", 2D) = "white" {}
		 _NoiseTex("Noise (RGB)", 2D) = "black" {}
		 _Mask("Noise (RGB)", 2D) = "black" {}
		 _Distortion("_Distortion", Range(0.0, 1.0)) = 0.3  
        _ScreenResolution("_ScreenResolution", Vector) = (0., 0., 0., 0.)  
        _ResolutionValue("_ResolutionValue", Range(0.0, 5.0)) = 1.0  
        _Radius("_Radius", Range(0.0, 5.0)) = 2.0  
	}
	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			struct v2f
			{
				float4 pos:SV_POSITION;
				half2 uv:TEXCOORD0;

			};
			v2f vert(appdata_img v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP,v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			float v2Dis(float2 v1,float2 v2)
			{
				return pow((v1-v2).x*(v1-v2).x+(v1-v2).y*(v1-v2).y,0.5f);
			}
			sampler2D _MainTex;
			sampler2D _SecondTex;
			sampler2D _NoiseTex;
			sampler2D _Mask;
			uniform float _DistortTimeFactor;  
    		uniform float _DistortStrength;  
    		uniform float centerX;
    		uniform float centerY;
    		uniform float _ResolutionValue;
    		uniform float2 _ScreenResolution;
    		uniform float _Radius;
			fixed4 frag(v2f i):SV_Target
			{
                float4 noise = tex2D(_NoiseTex, i.uv - _Time.xy * _DistortTimeFactor);  
       			float2 offset = noise.xy * _DistortStrength;  
       			float2 uv = offset + i.uv;  
                float4 color1= tex2D(_MainTex,uv);   
                float dis = v2Dis(i.uv,float2(centerX,centerY));
       			float4 noiseblue = tex2D(_NoiseTex,i.uv);
       			float4 mask = tex2D(_Mask,i.uv);
       			color1.b+=mask.r*1.2f+dis*0.15f+noiseblue.b*0.2f;
       			color1.g+=dis*0.45f;
       			//color1.r+=noiseblue.b*0.3f;
       			return color1;



			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
