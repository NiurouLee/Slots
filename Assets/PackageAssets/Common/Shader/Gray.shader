Shader "Hidden/Gray"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Toggle(_STRAIGHT_ALPHA_INPUT)] _StraightAlphaInput("Straight Alpha Texture", Int) = 0
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
            Blend One OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            Lighting Off


            Pass
            {
                Cull off
                //alphatest greater[_alphaValue]

                CGPROGRAM
                #pragma shader_feature _ _STRAIGHT_ALPHA_INPUT
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color:COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color:COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float finalAlpha = (col.a * i.color.a);

            #if defined(_STRAIGHT_ALPHA_INPUT)
                col.rgb *= col.a;
            #endif

                float grey = dot(col.rgb, float3(0.299, 0.587, 0.114));
                col.rgb = float3(grey, grey, grey);

                return fixed4(col.rgb,finalAlpha);
            }
            ENDCG
        }
    }
}
