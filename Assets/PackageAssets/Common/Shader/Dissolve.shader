Shader "Dissolve"
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
		
		_SpriteText_Rect("Sprite Texture Rect", Vector) = (0,0,1,1) 
		_EdgeWidth("EdgeWidth", Range(0,1)) = 0.2 
		_Dissolve("Dissolve", Range(-1,1)) = 0.2 
		_DissolveColor ("DissolveColor", Color) = (1,1,1,1)
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
				float4 vertex   : POSITION;
				float4 color	: COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID

				float2 uv1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color	: COLOR;
				float2 texcoord  : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			sampler2D _NoiseTex;
			float4 _NoiseTex_TexelSize;

			float4 _SpriteText_Rect;
			fixed4 _DissolveColor;

			fixed _Dissolve;
			fixed _EdgeWidth;
		 
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityObjectToClipPos(IN.vertex);

				OUT.texcoord = IN.texcoord;
				OUT.texcoord1 = (IN.texcoord - _SpriteText_Rect.xy)/ _SpriteText_Rect.zw;;

				OUT.color = IN.color * _Color;
       		    return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
				half4 noise = (tex2D(_NoiseTex, IN.texcoord1) + _TextureSampleAdd);
				fixed4 originAlpha = color.a;

				clip(noise.g - _Dissolve);
			//	clip(IN.texcoord1.x - (noise.y + _Dissolve));

				//noise.g 
				color.rgb += step(noise.g, (_Dissolve + _EdgeWidth)) * _DissolveColor;
				//color.rgb += step(IN.texcoord1.x,((noise.y + _Dissolve) + _EdgeWidth)) * _DissolveColor;
				//color.rgb = noise.b;
				color *= IN.color;
				
				return color;
			}
		ENDCG
		}
	}
}
