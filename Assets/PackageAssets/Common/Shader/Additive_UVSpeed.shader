// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UI/Additive_UVSpeed"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_Texture_A("Texture_A", 2D) = "white" {}
		_TextureSpeed_A("TextureSpeed_A", Vector) = (0,0,0,0)
		_Color_A("Color_A", Color) = (1,1,1,1)
		_ColorPower_A("ColorPower_A", Float) = 1
		_Texture_B("Texture_B", 2D) = "white" {}
		_TextureSpeed_B("TextureSpeed_B", Vector) = (0,0,0,0)
		_Color_B("Color_B", Color) = (1,1,1,1)
		_ColorPower_B("ColorPower_B", Float) = 1
		_TextureMask("TextureMask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend One One
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform sampler2D _Texture_A;
			uniform float4 _Texture_A_ST;
			uniform float2 _TextureSpeed_A;
			uniform float4 _Color_A;
			uniform float _ColorPower_A;
			uniform sampler2D _Texture_B;
			uniform float4 _Texture_B_ST;
			uniform float2 _TextureSpeed_B;
			uniform float4 _Color_B;
			uniform float _ColorPower_B;
			uniform sampler2D _TextureMask;
			uniform float4 _TextureMask_ST;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				float2 uv_Texture_A = IN.texcoord.xy * _Texture_A_ST.xy + _Texture_A_ST.zw;
				float2 uv_Texture_B = IN.texcoord.xy * _Texture_B_ST.xy + _Texture_B_ST.zw;
				float2 uv_TextureMask = IN.texcoord.xy * _TextureMask_ST.xy + _TextureMask_ST.zw;
				
				half4 color = ( ( ( tex2D( _Texture_A, ( uv_Texture_A + ( _Time.y * _TextureSpeed_A ) ) ) * _Color_A * _ColorPower_A ) * ( tex2D( _Texture_B, ( uv_Texture_B + ( _Time.y * _TextureSpeed_B ) ) ) * _Color_B * _ColorPower_B ) ) * tex2D( _TextureMask, uv_TextureMask ) * IN.color * IN.color.a );
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	FallBack "Diffuse"
}
