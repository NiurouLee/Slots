// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sheens/Sheen" 
{
	Properties 
	{
		_Color ("Sheen Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SheenTex ("Sheen Texture", 2D) = "white" {}
		_SpriteText_Rect("Sprite Texture Rect", Vector) = (0,0,1,1)
	}

	SubShader 
	{
		Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue"="Transparent" "RenderType"="Transparent" }
		ZWrite Off
		
		Pass
		{
		CGPROGRAM
        #include "UnityCG.cginc"
		#pragma target 2.0
		#pragma vertex vert
		#pragma fragment frag

		sampler2D _MainTex;
		sampler2D _SheenTex;
		float4 _Color;
        float4 _MainTex_ST;
		float4 _SheenTex_ST;
		float4 _SpriteText_Rect;

		struct v2f 
		{
			float4 vertex    : POSITION;
			float2 texcoord  : TEXCOORD0;
			float2 texcoord1 : TEXCOORD1;
		};

		v2f vert(appdata_full i) 
		{
			v2f o;
			o.vertex = UnityObjectToClipPos( i.vertex );
			o.texcoord = TRANSFORM_TEX( i.texcoord, _MainTex);

			float2 localUv = (o.texcoord - _SpriteText_Rect.xy)/ _SpriteText_Rect.zw;
			o.texcoord1 = TRANSFORM_TEX( localUv, _SheenTex);
			return o;
		}

		float4 frag(v2f i) : COLOR
		{
			float4 mtex = tex2D(_MainTex, i.texcoord);
			float4 sheen = tex2D(_SheenTex, i.texcoord1) * _Color;
			//float alpha = (sheen.r + sheen.b + sheen.g) / 3.0f;
			//return float4(i.texcoord1,0.0f,1.0f);
			return float4(mtex.rgb + sheen.rgb * mtex.a, mtex.a);
		}

		ENDCG
		}
	} 
	FallBack "Diffuse"
}
