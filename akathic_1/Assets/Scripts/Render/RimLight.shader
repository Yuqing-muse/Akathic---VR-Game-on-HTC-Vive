// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "ApcShader/RimLight"  
{  
    Properties{  
        _Diffuse("Diffuse", Color) = (1,1,1,1)  
        _RimColor("RimColor", Color) = (1,1,1,1)  
        _RimPower("RimPower", Range(0.000001, 20.0)) = 0.1  
        _RimMask("RimMask", 2D) = "white"{}  
        _MainTex("Base 2D", 2D) = "white"{}  
        _RimSpeed("RimSpeed", Range(-10, 10)) = 1.0 
        _kd("KD", Range(0.000001, 1.0)) = 0.1  
        _ks("KS", Range(0.000001, 1.0)) = 0.1
        _frne("Fresnel", Range(0.000001, 1.0)) = 0.1
        _rough("rough", Range(0.000001, 1.0)) = 0.1
    }  
  
 
    SubShader  
    {  
        Pass  
        {  

            Tags{ "RenderType" = "Opaque" }  
  
            CGPROGRAM  

            #include "Lighting.cginc"  

            fixed4 _Diffuse;  
            sampler2D _MainTex;  

            float4 _MainTex_ST;  
            sampler2D _RimMask;  
            fixed4 _RimColor;  
            float _RimPower;  
  			float _RimSpeed;
  			float _kd;
  			float _ks;
  			float _frne;
  			float _rough;
  			//float4 _LightColor0;
            struct v2f  
            {  
                float4 pos : SV_POSITION;  
                float3 worldNormal : TEXCOORD0;  
                float2 uv : TEXCOORD1;  
         
                float3 worldViewDir : TEXCOORD2;  
            };  
  
  			float F(fixed3 V,fixed3 H,float _Fresnel)
			{
				float _f = _Fresnel + (1-_Fresnel)* pow(1-dot(V,H),5.0f);
				return _f;
			}

			float D(fixed3 N,fixed3 H,float _Rough,float E)
			{
				float NdotH = dot(N,H);
				float M2 = pow(_Rough,2.0f);
				float fir = 1.0f/(M2*pow(NdotH,4.0f));
				float secFir = (pow(NdotH,2.0f)-1.0f)/(pow(_Rough,2.0f)*(pow(NdotH,2.0f)));
				float sec = pow(E,secFir);
				return fir*sec;
			}

			float G(fixed3 N,fixed3 H,fixed3  L,fixed3 V)
			{
				float NdotH = dot(N,H);
				float NdotL = dot(N,L);
				float VdotH = dot(V,H);
				float NdotV = dot(N,V);
				
				float G1 = 2*NdotH*NdotL/(VdotH);
				float G2 = 2*NdotH*NdotV/(VdotH);

				if(G1<=G2)
				{
					if(G1<=1)
					{
						return G1;
					}
					else
					{
						return 1.0f;
					}	
				}
				else
				{
					if(G2<=1)
					{
						return G2;
					}
					else
					{
						return 1.0f;
					}
				}

			}
            v2f vert(appdata_base v)  
            {  
                v2f o;  
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);  

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);  
                o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);  

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  
 
                o.worldViewDir = _WorldSpaceCameraPos.xyz - worldPos;  
                return o;  
            }  
  
 
            fixed4 frag(v2f i) : SV_Target  
            {  

                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * _Diffuse.xyz;  

                fixed3 worldNormal = normalize(i.worldNormal);  

                fixed3 worldLightDir = normalize(_WorldSpaceLightPos0.xyz);  
    			fixed3 worldView = normalize(i.worldViewDir);
    			fixed3 H = normalize((worldLightDir+worldView)/2);

              //  fixed3 lambert = 0.5 * dot(worldNormal, worldLightDir) + 0.5;  

            //    fixed3 diffuse = lambert * _Diffuse.xyz * _LightColor0.xyz + ambient;  
            	float VdotN = dot(worldView,worldNormal);
				float NdotL = dot(worldNormal,worldLightDir);
				float VdotH = dot(worldView,H);

				float _F = F(worldView,H,_frne);
				float _D = D(worldNormal,H,_rough,2.71828f);
				float _G = G(worldNormal,H,worldLightDir,worldView);

                fixed4 color = tex2D(_MainTex, i.uv);  
                color.rgb+=ambient.rgb;
 				color+=(NdotL*_kd+_ks*_F*_D*_G/VdotH)*_LightColor0;
 
                float3 worldViewDir = normalize(i.worldViewDir);  

                float rim = 1 - max(0, dot(worldViewDir, worldNormal));  

                fixed3 rimColor = _RimColor * pow(rim, 1 / _RimPower);  

                fixed rimMask = tex2D(_RimMask, i.uv + float2(0 , _Time.y * _RimSpeed)).r; 

                color.rgb = color.rgb + rimColor * (1 - rimMask);  
                // color.rgb = color.rgb + rimColor ;
                return fixed4(color);  
            }  
  

            #pragma vertex vert  
            #pragma fragment frag     
  
            ENDCG  
        }  
    }  

    FallBack "Diffuse"  
} 
