// author: whj 
Shader "Unlit/shader_eff_alpha"
{
    Properties
    {
        [Toggle]_UseMainTex("是否使用主贴图【0不使用1使用】", float) = 0
        _MainColor("主贴图颜色", Color) = (0.5,0.5,0.5,1)
        _MainTex ("贴图1", 2D) = "white" {}
        _UV1Speed("贴图1UV速度向量", Vector) = (0,0,0,0)
        _MainMask("主贴图遮罩", 2D) = "white" {}
        _MaskSpeed("遮罩UV速度向量", Vector) = (0,0,0,0)
        _MainRota("主贴图旋转【0到360°】", Range(0, 360)) = 0
        [Toggle]_UseTex2("是否使用贴图2【0不使用1使用】", float) = 0
        _Main2Color("贴图2颜色", Color) = (0.5,0.5,0.5,1)
        _MainTex2 ("贴图2", 2D) = "white" {}
        _UvSpeed("贴图2UV速度向量", Vector) = (0,0,0,0)
        _UvMask("贴图2遮罩", 2D) = "white" {}
        _Mask2Speed("遮罩UV速度向量", Vector) = (0,0,0,0)
        _MainRota2("副贴图旋转【0到360°】", Range(0, 360)) = 0
        [Toggle]_UseDissolve("是否使用溶解【0不使用1使用】", float) = 0
        _DissolveTex("溶解噪声贴图", 2D) = "white" {}
        _DissolveFactor("溶解阈值【溶解进度】", Range(0,1)) = 0.2
        _UseSoftColor("是否叠加软边颜色【0不叠加1叠加】", float) = 1
        _EdgeColor("溶解边界颜色", Color) = (1,0,0,1)
        _EdgeVale("溶解软边宽度", Range(0,1)) = 0.2
        [Toggle]_UseEdge("是否显示边界颜色【0不显示1显示】", float) = 1
        [Toggle]_UseUVNosize("是否使用UV扰动【0不使用1使用】", float) = 0
        _NosizeUVTex("UV扰动贴图", 2D) = "white" {}
        _NosizeStrength("横向扰动宽度", Range(0,1)) = 0.01
        _NosizeUvSpeed("扰动贴图UV速度向量", Vector) = (0,0,0,0)
		[Toggle]_UseFnl("是否使用菲涅尔【0不使用1使用】", float) = 0
		_FnlP("菲涅尔阈值", Range(0,10)) = 0.1
		_FnlColor("菲涅尔颜色", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { 
            "Queue" = "Transparent"
            "RenderType"="Transparent" 
        }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #pragma multi_compile __ UNITY_UI_CLIP_RECT
            #include "UnityUI.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
				float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float2 uv4 : TEXCOORD3;
                float2 uv5 : TEXCOORD4;
                float2 uv6 : TEXCOORD5;
                float4 worldPosition : TEXCOORD6;
				float3 viewDir : TEXCOORD7;
				float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            float _MainRota;
            float _UseMainTex;
            fixed4 _MainColor;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _MainMask;
            float4 _MainMask_ST;
            float4 _MaskSpeed;
            float4 _UV1Speed;
            float _UseTex2;
            fixed4 _Main2Color;
            sampler2D _MainTex2;
            float4 _MainTex2_ST;
            float _MainRota2;
            float4 _UvSpeed;
            sampler2D _UvMask;
            fixed4 _UvMask_ST;
            fixed4 _Mask2Speed;
            sampler2D _DissolveTex;
            fixed4 _DissolveTex_ST;
            float _DissolveFactor;
            float _UseDissolve;
            float _UseSoftColor;
            float4 _EdgeColor;
            float _EdgeVale;
            float _UseEdge;
            float _UseUVNosize;
            sampler2D _NosizeUVTex;
            fixed4 _NosizeUVTex_ST;
            fixed _NosizeStrength;
            fixed4 _NosizeUvSpeed;
            float4 _ClipRect;
			float _UseFnl;
			float _FnlP;
			float4 _FnlColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv, _MainTex2);
                o.uv3 = TRANSFORM_TEX(v.uv, _MainMask);
                o.uv4 = TRANSFORM_TEX(v.uv, _UvMask);
                o.uv5 = TRANSFORM_TEX(v.uv, _DissolveTex);
                o.uv6 = TRANSFORM_TEX(v.uv, _NosizeUVTex);
                o.color = v.color;
				o.normal = normalize(v.normal);
				o.viewDir = normalize(ObjSpaceViewDir(v.vertex));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = fixed4(0,0,0,1);
                fixed4 uvnosize = fixed4(0,0,0,0);
                if(_UseMainTex > 0){
                    fixed2 uv1 = i.uv;
                    if(_MainRota > 0 ){
                        float Angle = (_MainRota / 180) * 3.141592;
                        float2 tempUv = uv1 - float2(_MainTex_ST.x, _MainTex_ST.y) * 0.5;
                        uv1.x = tempUv.x * cos(Angle) + tempUv.y * sin(Angle);
                        uv1.y = -tempUv.x * sin(Angle) + tempUv.y  * cos(Angle);
                        uv1 += float2(_MainTex_ST.x, _MainTex_ST.y) * 0.5;
					}

                    if(_UseUVNosize > 0){
                       uvnosize = tex2D(_NosizeUVTex, i.uv6 + _Time.x * _NosizeUvSpeed.xy);
                       uv1 += uvnosize.xy * _NosizeStrength;
				    }

                    col = tex2D(_MainTex, uv1 +  _Time.x * _UV1Speed.xy);
                    col *= _MainColor * i.color;
                    fixed4 mask = tex2D(_MainMask, i.uv3 + _Time.x * _MaskSpeed.xy);
                    col.a *= mask.r;
				}

                if(_UseTex2 > 0){
                    fixed2 uv2 = i.uv2;
                    if(_MainRota2 > 0 ){
                        float Angle = (_MainRota2 / 180) * 3.141592;
                        float2 tempUv = uv2 - float2(_MainTex2_ST.x, _MainTex2_ST.y) * 0.5;
                        uv2.x = tempUv.x * cos(Angle) + tempUv.y * sin(Angle);
                        uv2.y = -tempUv.x * sin(Angle) + tempUv.y  * cos(Angle);
                        uv2 += float2(_MainTex2_ST.x, _MainTex2_ST.y) * 0.5;
					}
                    if(_UseUVNosize > 0){
                       uvnosize = tex2D(_NosizeUVTex, i.uv2 + _Time.x * _NosizeUvSpeed.xy);
                       uv2 += uvnosize.xy * _NosizeStrength;
				    }
                    fixed4 col2 =  tex2D(_MainTex2, uv2 + _Time.x * _UvSpeed.xy);
                    col2 *= _Main2Color * i.color;
                    fixed4 mask2 = tex2D(_UvMask, i.uv4 + _Time.x * _Mask2Speed.xy);
                    col2 *= mask2.r;
                    if(_UseMainTex > 0){
                        fixed a = 1 - col2.a;
                        col.rgb *= a;
					}else{
                        col.a = col2.a;
					}
                    col += col2;
				}

#ifdef UNITY_UI_CLIP_RECT
                col.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#endif
				if(_UseFnl > 0){
					half rim = 1.0 - saturate(dot(i.normal, i.viewDir));
					rim = pow(rim, _FnlP);
					col.rgb += _FnlColor.rgb * rim;
				}

                if(_UseDissolve > 0){
                    float4 noise = tex2D(_DissolveTex, i.uv5);
                    fixed minus = noise.r - _DissolveFactor;
                    clip(minus);
                    minus = saturate(minus);
                    fixed4 linec = fixed4(0,0,0,0);
                    float t = 0;
                    if(_EdgeVale > 0){
                        float m = 1 / _EdgeVale;
                        t = minus * m;
                        if(_UseSoftColor > 0){
                            if(minus < _EdgeVale)
                            {
                                linec = _EdgeColor * t ;
					        }
						}
                        t = saturate(t);
                        col.a *= t;
					}

                    linec.a *= col.a;
                    if(_UseEdge > 0){
                        col += linec * t;
					}

                    return col;
				}

                return col;
            }
            ENDCG
        }
    }
}
