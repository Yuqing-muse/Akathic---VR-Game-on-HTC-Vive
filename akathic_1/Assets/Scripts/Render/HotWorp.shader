Shader "Custom/SimpleHeatDistortion" {

    Properties {
        _NoiseTex ("Noise Texture (RG)", 2D) = "white" {}
        _MainTex ("Alpha (A)", 2D) = "white" {}
        _HeatTime  ("Heat Time", range (0,1.5)) = 1
        _HeatForce  ("Heat Force", range (0,1)) = 1
    }

    Category {
        Tags { "Queue"="Transparent+1" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater .01
        Cull Off Lighting Off ZWrite Off

        SubShader {
            GrabPass { Name "BASE" Tags { "LightMode" = "Always" } }

            Pass {
                Name "BASE"
                Tags { "LightMode" = "Always" }

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                struct appdata_t {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 texcoord: TEXCOORD0;
                };

                struct v2f {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                    float2 uvmain : TEXCOORD1;
                };

                float _HeatForce;
                float _HeatTime;
                float4 _MainTex_ST;
                float4 _NoiseTex_ST;
                sampler2D _NoiseTex;
                sampler2D _MainTex;

                v2f vert (appdata_t v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    #if UNITY_UV_STARTS_AT_TOP
                        float scale = -1.0;
                    #else
                        float scale = 1.0;
                    #endif
                    o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y*scale) + o.vertex.w) * 0.5;
                    o.uvgrab.zw = o.vertex.zw;
                    o.uvmain = TRANSFORM_TEX( v.texcoord, _MainTex );
                    return o;
                }

                sampler2D _GrabTexture;

                half4 frag( v2f i ) : COLOR
                {
                    half4 offsetColor2 = tex2D(_NoiseTex, i.uvmain - _Time.yx*_HeatTime);
                    i.uvgrab.xy=offsetColor2.rg*i.uvgrab.z*_HeatForce+i.uvgrab.xy;
                    half4 col = tex2Dproj( _GrabTexture, UNITY_PROJ_COORD(i.uvgrab));
                    col.a = 1.0f;
                    half4 tint = tex2D( _MainTex, i.uvmain);

                    return col*tint;
                }

                ENDCG

            }// End Pass

        }// End SubShader

        // Fallback for older cards and Unity non-Pro
        SubShader 
        {
            Blend DstColor Zero
            Pass {
                Name "BASE"
                SetTexture [_MainTex] { combine texture }
            }
        }
    }
}
