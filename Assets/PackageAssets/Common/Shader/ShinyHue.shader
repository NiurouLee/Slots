Shader "ShinyHue"
{
	Properties
	{
		[PerRendererData] _MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15
		
		_SpriteText_Rect("Sprite Texture Rect", Vector) = (0,0,1,1)
		_HueRangeMin ("Hue Range Min", Range(0, 1)) = 0
       	_HueRangeMax ("Hue Range Max", Range(0, 1)) = 1
  		[HideInInspector] _ShinyWidth("ShinyWidth", Range(0,1)) = 0.2 
		[HideInInspector] _ShinyLocation("ShinyLocation", Range(0,1)) = 0.2 
		[HideInInspector] _ShinyRotation("ShinyRotation", Range(0,360)) = 0.2 
		[HideInInspector] _ShinySoftness("ShinySoftness", Range(0,1)) = 0.2 
		[HideInInspector] _ShinyBrightness("ShinyBrightness", Range(0,1)) = 0.2 
		[HideInInspector] _ShinyHighlight("ShinyHighlight", Range(0,1)) = 0.2 
		[HideInInspector] _ShinyColor ("ShinyColor", Color) = (1,1,1,1)
		[HideInInspector] _HighlightThreshold ("HighlightThreshold", Range(0,3)) = 0
		[HideInInspector] _HighlightSoftFactor ("HighlightSoftFactor", Range(0,1)) = 0
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
				float4 worldPosition : TEXCOORD2;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;
			fixed4 _TextureSampleAdd;
			float4 _ClipRect;
			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			float4 _SpriteText_Rect;
			fixed _ShinyWidth;
			fixed _ShinyLocation;
			fixed _ShinyRotation;
			fixed _ShinySoftness;
			fixed _ShinyBrightness;
			fixed _ShinyHighlight;
			fixed4 _ShinyColor;
			fixed _HighlightThreshold;
			fixed _HighlightSoftFactor;
			fixed _HueRangeMin;
			fixed _HueRangeMax;

			float getHue(float3 c) 
			{
              float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
              float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
              float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

              float d = q.x - min(q.w, q.y);
              float e = 1.0e-10;
              return abs(q.z + (q.w - q.y) / (6.0 * d + e));
            }
 
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.worldPosition = IN.vertex;
				OUT.texcoord = IN.texcoord;
				float2 localUv = (IN.texcoord - _SpriteText_Rect.xy)/ _SpriteText_Rect.zw;;

				const float Deg2Rad = (UNITY_PI * 2.0) / 360.0;
				float rotationRadians = _ShinyRotation * Deg2Rad; // convert degrees to radians

				float s = sin(rotationRadians); // sin and cos take radians, not degrees
				float c = cos(rotationRadians);
 
                float2x2 rotationMatrix = float2x2( c, -s, s, c); // construct simple rotation matrix
				localUv -= 0.5; // offset UV so we rotate around 0.5 and not 0.0
                localUv = mul(rotationMatrix, localUv); // apply rotation matrix
                localUv += 0.5;
				OUT.texcoord1 = localUv;

				OUT.color = IN.color * _Color;
       		    return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd);
				fixed4 originAlpha = color.a;
				
				color *= IN.color;

				#ifdef UNITY_UI_ALPHACLIP
					color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				 	clip (color.a - 0.001);
				#endif

				half pos = IN.texcoord1.x -  ((_ShinyLocation - 0.5)*(1.41421356237 + _ShinyWidth) + 0.5);

				half normalized = 1 - saturate(abs(pos / _ShinyWidth));
				half shinePower = smoothstep(0, _ShinySoftness * 2, normalized);
				half3 reflectColor = lerp(_ShinyColor.rgb, color.rgb * 10, _ShinyHighlight);

				fixed sumColor = color.r + color.g + color.b;

				sumColor = smoothstep(_HighlightThreshold - _HighlightSoftFactor,_HighlightThreshold + _HighlightSoftFactor, sumColor);
			 	float hue = getHue(color.rgb);	
				
			    float affectMult = sumColor * step(_HueRangeMin, hue) * step(hue, _HueRangeMax);

				// color.rgb = sumColor;
			 	color.rgb += originAlpha * shinePower * _ShinyBrightness * reflectColor * affectMult;
				return color;
			}
		ENDCG
		}
	}
}
