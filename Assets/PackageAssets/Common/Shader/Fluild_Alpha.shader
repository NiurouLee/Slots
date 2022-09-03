// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "ProjectQ/GFX/Complex/GFX_Fluild_Alpha" {
    Properties {
        _Color ("Color", Color) = (0.5,0.5,0.5,1)
        _power ("power", Range(0, 10)) = 0
        _fliuld ("fliuld", Range(-1, 1)) = 0
        _Diffuse ("Diffuse", 2D) = "white" {}
        _mask ("mask", 2D) = "white" {}
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _ColorMask ("Color Mask", Float) = 15.000000    
     }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        LOD 200

        Stencil{
            Ref [_Stencil]
            Pass[_StencilOp]
            Comp[_StencilComp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }

        Pass {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma target 2.0

			#include "UnityCG.cginc"
			#include "CGInclude.cginc"

            uniform fixed4 _Color;
            uniform fixed _power;
            uniform fixed _fliuld;
            uniform sampler2D _Diffuse; uniform half4 _Diffuse_ST;
            uniform sampler2D _mask; uniform half4 _mask_ST;

            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
            };

            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 uv0 : TEXCOORD0;
				fixed4 vertexColor : COLOR;
				MY_FOG_LIGHTMAP_COORDS(1)
				MY_FOGWAR_COORDS(2)
            };

            VertexOutput vert (VertexInput v) {
				VertexOutput o = (VertexOutput)0;
				o.uv0.xy = v.texcoord0;// TRANSFORM_TEX(v.texcoord0, _Diffuse);
				o.uv0.zw = TRANSFORM_TEX(v.texcoord0, _mask);
				o.vertexColor = v.vertexColor;
				o.pos = UnityObjectToClipPos(v.vertex);

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				MY_TRANSFER_FOG(o, worldPos, o.pos.z);
                return o;
            }

            float4 frag(VertexOutput i) : COLOR 
			{
				fixed4 _DiffuseTex = tex2D(_Diffuse, TRANSFORM_TEX(i.uv0, _Diffuse));
				half2 duv = (i.uv0 + (_Time.g * _fliuld) * fixed2(0, 1));
				fixed4 _DiffuseTex_Dlta = tex2D(_Diffuse, TRANSFORM_TEX(duv, _Diffuse));
				fixed4 _maskTex = tex2D(_mask, i.uv0.zw);

				fixed alpha = i.vertexColor.a * _maskTex.r * _Color.a;
				fixed3 finalColor = _Color.rgb * _DiffuseTex.rgb+_DiffuseTex_Dlta.rgb*_power*_Color.rgb;

				fixed4 finalRGBA = fixed4(finalColor, alpha);

				MY_APPLY_FOG_COLOR(i, finalRGBA, fixed4(0, 0, 0, 0), fixed4(0, 0, 0, 0));

				return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
