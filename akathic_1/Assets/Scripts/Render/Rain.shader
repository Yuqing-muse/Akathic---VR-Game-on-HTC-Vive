Shader "Custom/blend" {
	Properties 
	{
		_MainTex ("BlendTexture", 2D) = "white" {}
	//	fadescale ("FadeScale", Range(0.0,0.8)) = 0.4
	}
	Category 
	{
	Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	SubShader {
		pass
		{		
			blend srcalpha oneminussrcalpha
			cull front
			zwrite off
			CGPROGRAM
			#pragma vertex ver
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"
			struct v2f
			{
				float4 pos:POSITION;
				float2 uv:TEXCOORD0;
			};
			sampler2D _MainTex;
			float fadescale;
			v2f ver(appdata_full v)
			{
				v2f o;
				o.pos=mul(UNITY_MATRIX_MVP,v.vertex);
				o.uv = v.texcoord;
				return o;
			}
			fixed4 frag(v2f IN):COLOR
			{
				fixed4 coloroftex=tex2D(_MainTex,IN.uv);
			//	coloroftex.a =fadescale;
				return coloroftex;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
	}
}
