// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GFX/UIIconTrail"
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
		_ColorOverlay("Color Overlay", Color) = (1,1,1,1)
		_GlobalAlphaIntensity("Global Alpha Intensity", Range( 0 , 5)) = 2
		_GlobalAlphaFalloff("Global Alpha Falloff", Range( 1 , 5)) = 1
		_TrailLengthFalloff("Trail Length Falloff", Range( 1 , 10)) = 1.5
		_HighLightIntensity("High Light Intensity", Range( 0 , 3)) = 0.1
		_HighLightFalloff("High Light Falloff", Range( 1 , 5)) = 1
		_TimeSpeed("Time Speed", Range( 0 , 5)) = 1
		[IntRange]_TrailNumber("Trail Number", Range( 1 , 4)) = 2
		[Toggle(_USECLOCKWISE_ON)] _UseClockwise("Use Clockwise ?", Float) = 0
		[Toggle(_ENABLESECONDALPHASHADE_ON)] _EnableSecondAlphaShade("Enable Second AlphaShade", Float) = 0
		[Toggle(_INVERTSECONDALPHASHADE_ON)] _InvertSecondAlphaShade("Invert Second AlphaShade?", Float) = 0
		[Toggle(_USEDISTORTION_ON)] _UseDistortion("Use Distortion?", Float) = 0
		_DistortionIntensity("Distortion Intensity", Range( 0 , 0.25)) = 0
		_BaseTexture("Base Texture", 2D) = "white" {}
		_RotatingTexture("Rotating Texture", 2D) = "white" {}
		_SecondIconAlphaTexture("Second Icon Alpha Texture", 2D) = "white" {}
		[HideInInspector]_T_CloudNoise("T_CloudNoise", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
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
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha
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
			#pragma shader_feature _USEDISTORTION_ON
			#pragma shader_feature _USECLOCKWISE_ON
			#pragma shader_feature _ENABLESECONDALPHASHADE_ON
			#pragma shader_feature _INVERTSECONDALPHASHADE_ON

			
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
			uniform float4 _ColorOverlay;
			uniform sampler2D _BaseTexture;
			uniform sampler2D _T_CloudNoise;
			uniform float _DistortionIntensity;
			uniform float _HighLightIntensity;
			uniform float _HighLightFalloff;
			uniform float _TrailNumber;
			uniform sampler2D _RotatingTexture;
			uniform float _TimeSpeed;
			uniform float _TrailLengthFalloff;
			uniform float _GlobalAlphaIntensity;
			uniform float _GlobalAlphaFalloff;
			uniform sampler2D _SecondIconAlphaTexture;
			uniform float4 _SecondIconAlphaTexture_ST;
			
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
				float3 appendResult139 = (float3(_ColorOverlay.r , _ColorOverlay.g , _ColorOverlay.b));
				float2 uv0127 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 uv0123 = IN.texcoord.xy * float2( 1.5,1.5 ) + float2( 0,0 );
				float2 panner122 = ( 1.0 * _Time.y * float2( 0.1,0.1 ) + uv0123);
				float lerpResult162 = lerp( -0.25 , 0.15 , tex2D( _T_CloudNoise, panner122 ).r);
				float2 appendResult125 = (float2(lerpResult162 , lerpResult162));
				#ifdef _USEDISTORTION_ON
				float2 staticSwitch114 = ( uv0127 + ( appendResult125 * _DistortionIntensity ) );
				#else
				float2 staticSwitch114 = uv0127;
				#endif
				float4 tex2DNode84 = tex2D( _BaseTexture, staticSwitch114 );
				float temp_output_135_0 = pow( ( tex2DNode84.r * _HighLightIntensity ) , _HighLightFalloff );
				float3 appendResult138 = (float3(temp_output_135_0 , temp_output_135_0 , temp_output_135_0));
				float2 uv016 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float4 appendResult20 = (float4(( 1.0 - uv016.x ) , uv016.y , 0.0 , 0.0));
				#ifdef _USECLOCKWISE_ON
				float4 staticSwitch21 = appendResult20;
				#else
				float4 staticSwitch21 = float4( uv016, 0.0 , 0.0 );
				#endif
				float mulTime57 = _Time.y * ( _TimeSpeed * 3.0 );
				float cos13 = cos( mulTime57 );
				float sin13 = sin( mulTime57 );
				float2 rotator13 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos13 , -sin13 , sin13 , cos13 )) + float2( 0.5,0.5 );
				float4 tex2DNode12 = tex2D( _RotatingTexture, rotator13 );
				float cos55 = cos( ( mulTime57 + 3.15 ) );
				float sin55 = sin( ( mulTime57 + 3.15 ) );
				float2 rotator55 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos55 , -sin55 , sin55 , cos55 )) + float2( 0.5,0.5 );
				float4 tex2DNode56 = tex2D( _RotatingTexture, rotator55 );
				float cos75 = cos( ( mulTime57 + 1.575 ) );
				float sin75 = sin( ( mulTime57 + 1.575 ) );
				float2 rotator75 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos75 , -sin75 , sin75 , cos75 )) + float2( 0.5,0.5 );
				float cos77 = cos( ( mulTime57 + 4.725 ) );
				float sin77 = sin( ( mulTime57 + 4.725 ) );
				float2 rotator77 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos77 , -sin77 , sin77 , cos77 )) + float2( 0.5,0.5 );
				float temp_output_79_0 = ( tex2DNode12.r + tex2DNode56.r + tex2D( _RotatingTexture, rotator75 ).r + tex2D( _RotatingTexture, rotator77 ).r );
				float cos62 = cos( ( mulTime57 + 2.1 ) );
				float sin62 = sin( ( mulTime57 + 2.1 ) );
				float2 rotator62 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos62 , -sin62 , sin62 , cos62 )) + float2( 0.5,0.5 );
				float cos64 = cos( ( mulTime57 + 4.2 ) );
				float sin64 = sin( ( mulTime57 + 4.2 ) );
				float2 rotator64 = mul( staticSwitch21.xy - float2( 0.5,0.5 ) , float2x2( cos64 , -sin64 , sin64 , cos64 )) + float2( 0.5,0.5 );
				float temp_output_70_0 = ( tex2DNode12.r + tex2D( _RotatingTexture, rotator62 ).r + tex2D( _RotatingTexture, rotator64 ).r );
				float ifLocalVar53 = 0;
				if( _TrailNumber >= 4.0 )
				ifLocalVar53 = temp_output_79_0;
				else
				ifLocalVar53 = temp_output_70_0;
				float temp_output_61_0 = ( tex2DNode12.r + tex2DNode56.r );
				float ifLocalVar51 = 0;
				if( _TrailNumber > 3.0 )
				ifLocalVar51 = ifLocalVar53;
				else if( _TrailNumber == 3.0 )
				ifLocalVar51 = temp_output_70_0;
				else if( _TrailNumber < 3.0 )
				ifLocalVar51 = temp_output_61_0;
				float ifLocalVar49 = 0;
				if( _TrailNumber > 2.0 )
				ifLocalVar49 = ifLocalVar51;
				else if( _TrailNumber == 2.0 )
				ifLocalVar49 = temp_output_61_0;
				else if( _TrailNumber < 2.0 )
				ifLocalVar49 = tex2DNode12.r;
				float ifLocalVar25 = 0;
				if( _TrailNumber <= 1.0 )
				ifLocalVar25 = tex2DNode12.r;
				else
				ifLocalVar25 = ifLocalVar49;
				float clampResult113 = clamp( pow( pow( ifLocalVar25 , 1.1 ) , _TrailLengthFalloff ) , 0.0 , 1.0 );
				float clampResult145 = clamp( pow( ( ( ( tex2DNode84.r * clampResult113 ) * 1.5 ) * _GlobalAlphaIntensity ) , _GlobalAlphaFalloff ) , 0.0 , 1.0 );
				float2 uv_SecondIconAlphaTexture = IN.texcoord.xy * _SecondIconAlphaTexture_ST.xy + _SecondIconAlphaTexture_ST.zw;
				float4 tex2DNode91 = tex2D( _SecondIconAlphaTexture, uv_SecondIconAlphaTexture );
				#ifdef _INVERTSECONDALPHASHADE_ON
				float staticSwitch87 = ( 1.0 - tex2DNode91.r );
				#else
				float staticSwitch87 = tex2DNode91.r;
				#endif
				#ifdef _ENABLESECONDALPHASHADE_ON
				float staticSwitch88 = staticSwitch87;
				#else
				float staticSwitch88 = 1.0;
				#endif
				float4 appendResult141 = (float4(( appendResult139 + appendResult138 ) , ( clampResult145 * staticSwitch88 )));
				
				half4 color = appendResult141;
				
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
