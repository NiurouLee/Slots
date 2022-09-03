Shader "Unlit/HoleCircleFocus"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Alpha("Alpha",Range(0,1)) = 0.5
        _centerX("CenterX",Range(0,1)) = 0.5
        _centerY("CenterY",Range(0,1)) = 0.5
        _emptyR("EmptyR",Range(0,1))=0.4
        _fadeR("FadeR",Range(0,1))=0.6

    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }
        LOD 100

        Pass
        {
            ZWrite Off
            ////Cull Off
            //Lighting Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Alpha;
            float _emptyR;
            float _fadeR;
            float _centerX;
            float _centerY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                col.w = _Alpha;

                float2 offset = i.uv - float2(_centerX, _centerY);
                float len = length(offset);

                if (len < _emptyR)
                {
                    col = float4(1, 1, 1, 0);
                }
                else if (len < _fadeR)
                {
                    col = float4(0, 0, 0, clamp((len - _emptyR)/(_fadeR- _emptyR) * _Alpha, 0, _Alpha));
                }

                

                return col;
            }
            ENDCG
        }
    }
}
