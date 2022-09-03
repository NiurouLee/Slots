
Shader "Sprites/SupportTiling"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1, 1, 1, 1)
		_SpriteText_Rect("Sprite Texture Rect", Vector) = (0,0,1,1)
		_RepeatX("RepeatX", float) = 1
		_RepeatY("RepeatY", float) = 1
		[MaterialToggle] PixelSnap("Pixel snap", Float) = 0
	}
 
	SubShader
	{
		Tags
		{
		"Queue" = "Transparent"
		"IgnoreProjector" = "True"
		"RenderType" = "Transparent"
		"PreviewType" = "Plane"
		"CanUseSpriteAtlas" = "True"
	}
 
		Cull Off
			Lighting Off
			ZWrite Off
			Blend One OneMinusSrcAlpha
 
			Pass
		{
			CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma target 2.0
#pragma multi_compile _ PIXELSNAP_ON
#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
#include "UnityCG.cginc"
 
			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
 
			fixed4 _Color;
 			float4 _SpriteText_Rect;
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = (IN.texcoord - _SpriteText_Rect.xy)/ _SpriteText_Rect.zw;
				OUT.color = IN.color * _Color;
#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap(OUT.vertex);
#endif
 
				return OUT;
			}
 
			sampler2D _MainTex;
			sampler2D _AlphaTex;
			fixed _RepeatX;
			fixed _RepeatY;
		

			fixed4 SampleSpriteTexture(float2 uv)
			{
				uv.x = (uv.x - (int)(uv.x / (1 / _RepeatX)) * (1 / _RepeatX)) * _RepeatX;
				uv.y = (uv.y - (int)(uv.y / (1 / _RepeatY)) * (1 / _RepeatY)) * _RepeatY;

				uv = uv * _SpriteText_Rect.zw + _SpriteText_Rect.xy;
 
				fixed4 color = tex2D(_MainTex, uv);
 
				return color;
			}
 
			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;
				return c;
			}
			ENDCG
		}
	}
}
