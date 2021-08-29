Shader "Custom/Melt" {
	Properties 
	{
		_NoiseTex ("NoiseTexture", 2D) = "white" {}
		_MainTex ("MainTexture", 2D) = "white" {}
		_MeltScale ("MeltScale", Range(0,1)) = 0.0
		_outline("outline",Range(0,0.2))=0.05
		_DissColor ("DissColor", Color) = (1,0,0,1)
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
        	#pragma fragment frag
        	#include "UnityCG.cginc"

        	struct appdata_t
        	{
        		 float4 vertex : POSITION;
                 fixed4 color : COLOR;
                 float2 texcoord: TEXCOORD0;
        	};
        	struct v2f
        	{
                 float4 vertex : POSITION;
                 float2 uv : TEXCOORD0;
            };
            sampler2D _NoiseTex;
           	sampler2D _MainTex;
           	float _MeltScale;
           	float _outline;
           	fixed4 _DissColor;
        	v2f vert(appdata_t o)
        	{
        		v2f v;
        		v.vertex = mul(UNITY_MATRIX_MVP, o.vertex);
        		v.uv = o.texcoord;
        		return v;
        	}

        	fixed4 frag(v2f i):COLOR
        	{
        		fixed4 resultColor;
        		fixed4 col = tex2D(_MainTex, i.uv); 

    			float clipAmount = tex2D(_NoiseTex,i.uv).r;
    			if(clipAmount-_MeltScale<col.r-_outline/2)
    			{
    				clip(-0.1);
    			}
    			else if(clipAmount-_MeltScale>col.r+_outline/2)
    			{
    				resultColor = col;
    			}
    			else
    			{
    				resultColor = lerp(fixed4(0.0f,0.0f,0.0f,0.0f),_DissColor,(clipAmount-_MeltScale-col.r+_outline/2)/_outline);
    			}
        		return resultColor;
        	}
        	ENDCG
        }

	}
	FallBack "Diffuse"
}
