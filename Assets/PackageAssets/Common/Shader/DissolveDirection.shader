Shader "DissolveDirection"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_NoiseTex ("Noise Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		
		
		_EdgeWidth("EdgeWidth", Range(0,1)) = 0.2 
		_Dissolve("Dissolve", Range(-1,1)) = 0.2 
		[HDR]_DissolveColor ("DissolveColor", Color) = (1,1,1,1)
		_DissolveDir("DissolveDir", Range(0, 6.4)) = 0
		
		
		_SpriteText_Rect("Sprite Texture Rect", Vector) = (0,0,1,1) 
		
 		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}
		
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
	    Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		Pass
		{
			Name "Default"

		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			
			#pragma multi_compile __ UNITY_UI_ALPHACLIP

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"
			struct appdata_t
			{
				float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _NoiseTex;
			float4 _NoiseTex_ST;

			float4 _SpriteText_Rect;
		
			
            fixed4 _DissolveColor;
			fixed _Dissolve;
			fixed _EdgeWidth;
			fixed _DissolveDir;
		 
			v2f vert(appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
				
       		    return o;
			}

			fixed4 frag(v2f i, float facing : VFACE) : SV_Target
			{
				float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _niuqutu_var = tex2D(_NoiseTex,TRANSFORM_TEX(i.uv0, _NoiseTex));
                float node_575_ang = _DissolveDir;
                float node_575_spd = 1.0;
                float node_575_cos = cos(node_575_spd*node_575_ang);
                float node_575_sin = sin(node_575_spd*node_575_ang);
                float2 node_575_piv = float2(0.5,0.5);
                float2 node_575 = (mul(i.uv1-node_575_piv,float2x2( node_575_cos, -node_575_sin, node_575_sin, node_575_cos))+node_575_piv);
               
                float node_9282 = (pow(((_niuqutu_var.r*_Dissolve)+_Dissolve),3.0)*pow((_Dissolve+(_Dissolve*node_575.r)),10.0));
                clip(node_9282 - 0.5);
////// Lighting:
////// Emissive:
                // float4 _tietu_var = tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex));
                // float node_8999_if_leA = smoothstep(_rongjiebianyuan, _rongjiebianyuan + _rongjiebianyuanbantou, node_9282);
                // float node_8999_if_leB = smoothstep(_rongjiebianyuan - _rongjiebianyuanbantou, _rongjiebianyuan, node_9282);
                // float node_3935 = 0.0;
                // float3 emissive = ((_DissolveColor.rgb*_tietu_var.rgb*_DissolveColor.a)+(lerp((node_8999_if_leA*node_3935)+(node_8999_if_leB*1.0),node_3935,node_8999_if_leA*node_8999_if_leB)*_DissolveColor.rgb)+(_node_3394.rgb*pow(1.0-max(0,dot(i.normalDir, viewDirection)),_fne)));
                // float3 finalColor = emissive;
                // fixed4 finalRGBA = fixed4(finalColor,1);
                // UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                // return finalRGBA;


				half4 color = (tex2D(_MainTex,TRANSFORM_TEX(i.uv0, _MainTex))+ _TextureSampleAdd);
				color.rgb += step(node_9282, (_Dissolve + _EdgeWidth)) * _DissolveColor;
				//color.rgb += step(IN.texcoord1.x,((noise.y + _Dissolve) + _EdgeWidth)) * _DissolveColor;
				//color.rgb = noise.b;
				return color;
			}
		ENDCG
		}
	}
}
