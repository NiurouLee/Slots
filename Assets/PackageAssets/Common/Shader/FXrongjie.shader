// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// 注意：手动更改此数据可能会妨碍您在 Shader Forge 中打开它
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:33819,y:32789,varname:node_4013,prsc:2|emission-2457-OUT,clip-9282-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32374,y:32585,ptovrint:False,ptlb:tietuyanse,ptin:_tietuyanse,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Tex2d,id:6413,x:32374,y:32784,ptovrint:False,ptlb:tietu,ptin:_tietu,varname:node_6413,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-2791-UVOUT;n:type:ShaderForge.SFN_Multiply,id:1437,x:32790,y:32612,varname:node_1437,prsc:2|A-1304-RGB,B-6413-RGB,C-1304-A;n:type:ShaderForge.SFN_Tex2d,id:9488,x:31969,y:33111,ptovrint:False,ptlb:niuqutu,ptin:_niuqutu,varname:node_9488,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1707,x:32334,y:33179,varname:node_1707,prsc:2|A-9488-R,B-1428-OUT;n:type:ShaderForge.SFN_Slider,id:1428,x:31906,y:33404,ptovrint:False,ptlb:rongjie,ptin:_rongjie,varname:node_1428,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1.5;n:type:ShaderForge.SFN_Add,id:7705,x:32611,y:33222,varname:node_7705,prsc:2|A-1707-OUT,B-1428-OUT;n:type:ShaderForge.SFN_Power,id:2426,x:32988,y:33234,varname:node_2426,prsc:2|VAL-7705-OUT,EXP-5068-OUT;n:type:ShaderForge.SFN_Vector1,id:5068,x:32713,y:33341,varname:node_5068,prsc:2,v1:3;n:type:ShaderForge.SFN_TexCoord,id:205,x:32248,y:33545,varname:node_205,prsc:2,uv:1,uaff:False;n:type:ShaderForge.SFN_Multiply,id:4903,x:32658,y:33514,varname:node_4903,prsc:2|A-1428-OUT,B-8421-OUT;n:type:ShaderForge.SFN_Add,id:3816,x:32962,y:33451,varname:node_3816,prsc:2|A-1428-OUT,B-4903-OUT;n:type:ShaderForge.SFN_Power,id:9418,x:33099,y:33347,varname:node_9418,prsc:2|VAL-3816-OUT,EXP-2658-OUT;n:type:ShaderForge.SFN_Multiply,id:9282,x:33475,y:33245,varname:node_9282,prsc:2|A-2426-OUT,B-9418-OUT,C-9063-A,D-619-A;n:type:ShaderForge.SFN_If,id:8999,x:33012,y:32983,varname:node_8999,prsc:2|A-4787-OUT,B-9282-OUT,GT-6018-OUT,EQ-3935-OUT,LT-3935-OUT;n:type:ShaderForge.SFN_Slider,id:4787,x:32615,y:32857,ptovrint:False,ptlb:rongjiebianyuan,ptin:_rongjiebianyuan,varname:node_4787,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Vector1,id:6018,x:32656,y:32978,varname:node_6018,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:3935,x:32656,y:33055,varname:node_3935,prsc:2,v1:0;n:type:ShaderForge.SFN_Add,id:2457,x:33266,y:32807,varname:node_2457,prsc:2|A-1437-OUT,B-7215-OUT,C-4249-OUT;n:type:ShaderForge.SFN_Multiply,id:7215,x:33402,y:33015,varname:node_7215,prsc:2|A-8999-OUT,B-9984-RGB;n:type:ShaderForge.SFN_Color,id:9984,x:33154,y:33087,ptovrint:False,ptlb:bianyuanyanse,ptin:_bianyuanyanse,varname:node_9984,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_TexCoord,id:2791,x:32021,y:32754,varname:node_2791,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Fresnel,id:8825,x:33055,y:32578,varname:node_8825,prsc:2|NRM-4699-OUT,EXP-6196-OUT;n:type:ShaderForge.SFN_NormalVector,id:4699,x:32860,y:32232,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:6196,x:32633,y:32494,ptovrint:False,ptlb:fne,ptin:_fne,varname:node_6196,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:6;n:type:ShaderForge.SFN_Multiply,id:4249,x:33322,y:32576,varname:node_4249,prsc:2|A-3394-RGB,B-8825-OUT;n:type:ShaderForge.SFN_Color,id:3394,x:33126,y:32422,ptovrint:False,ptlb:node_3394,ptin:_node_3394,varname:node_3394,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:2658,x:32713,y:33407,varname:node_2658,prsc:2,v1:10;n:type:ShaderForge.SFN_Rotator,id:575,x:32488,y:33769,varname:node_575,prsc:2|UVIN-205-UVOUT,ANG-9908-OUT;n:type:ShaderForge.SFN_ComponentMask,id:8421,x:32739,y:33769,varname:node_8421,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-575-UVOUT;n:type:ShaderForge.SFN_Slider,id:9908,x:32068,y:33917,ptovrint:False,ptlb:node_9908,ptin:_node_9908,varname:node_9908,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:6.4;n:type:ShaderForge.SFN_Tex2d,id:9063,x:33113,y:33534,ptovrint:False,ptlb:mask,ptin:_mask,varname:node_9063,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Tex2d,id:619,x:33056,y:33742,ptovrint:False,ptlb:mask_02,ptin:_mask_02,varname:node_619,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;proporder:1304-6413-9488-1428-4787-9984-6196-3394-9908-9063-619;pass:END;sub:END;*/

Shader "Shader Forge/FXrongjie" {
    Properties {
        [HDR]_tietuyanse ("tietuyanse", Color) = (1,1,1,1)
        _tietu ("tietu", 2D) = "white" {}
        _niuqutu ("niuqutu", 2D) = "white" {}
        _rongjie ("rongjie", Range(0, 1.5)) = 0
        _rongjiebianyuan ("rongjiebianyuan", Range(0, 1)) = 0
        _rongjiebianyuanbantou ("rongjiebianyuanbantou", Range(0, 1)) = 0
        [HDR]_bianyuanyanse ("bianyuanyanse", Color) = (0.5,0.5,0.5,1)
        _fne ("fne", Range(0, 6)) = 1
        [HDR]_node_3394 ("node_3394", Color) = (0.5,0.5,0.5,1)
        _node_9908 ("node_9908", Range(0, 6.4)) = 0
        _mask ("mask", 2D) = "white" {}
        _mask_02 ("mask_02", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Cull Off
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x n3ds wiiu 
            #pragma target 3.0
            uniform float4 _tietuyanse;
            uniform sampler2D _tietu; uniform float4 _tietu_ST;
            uniform sampler2D _niuqutu; uniform float4 _niuqutu_ST;
            uniform float _rongjie;
            uniform float _rongjiebianyuan;
            uniform float _rongjiebianyuanbantou; 
            uniform float4 _bianyuanyanse;
            uniform float _fne;
            uniform float4 _node_3394;
            uniform float _node_9908;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _mask_02; uniform float4 _mask_02_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float2 uv1 : TEXCOORD1;
                float4 posWorld : TEXCOORD2;
                float3 normalDir : TEXCOORD3;
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                i.normalDir = normalize(i.normalDir);
                i.normalDir *= faceSign;
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float4 _niuqutu_var = tex2D(_niuqutu,TRANSFORM_TEX(i.uv0, _niuqutu));
                float node_575_ang = _node_9908;
                float node_575_spd = 1.0;
                float node_575_cos = cos(node_575_spd*node_575_ang);
                float node_575_sin = sin(node_575_spd*node_575_ang);
                float2 node_575_piv = float2(0.5,0.5);
                float2 node_575 = (mul(i.uv1-node_575_piv,float2x2( node_575_cos, -node_575_sin, node_575_sin, node_575_cos))+node_575_piv);
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float4 _mask_02_var = tex2D(_mask_02,TRANSFORM_TEX(i.uv0, _mask_02));
                float node_9282 = (pow(((_niuqutu_var.r*_rongjie)+_rongjie),3.0)*pow((_rongjie+(_rongjie*node_575.r)),10.0)*_mask_var.a*_mask_02_var.a);
                clip(node_9282 - 0.5);
////// Lighting:
////// Emissive:
                float4 _tietu_var = tex2D(_tietu,TRANSFORM_TEX(i.uv0, _tietu));
                float node_8999_if_leA = smoothstep(_rongjiebianyuan, _rongjiebianyuan + _rongjiebianyuanbantou, node_9282);
                float node_8999_if_leB = smoothstep(_rongjiebianyuan - _rongjiebianyuanbantou, _rongjiebianyuan, node_9282);
                float node_3935 = 0.0;
                float3 emissive = ((_tietuyanse.rgb*_tietu_var.rgb*_tietuyanse.a)+(lerp((node_8999_if_leA*node_3935)+(node_8999_if_leB*1.0),node_3935,node_8999_if_leA*node_8999_if_leB)*_bianyuanyanse.rgb)+(_node_3394.rgb*pow(1.0-max(0,dot(i.normalDir, viewDirection)),_fne)));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "ShadowCaster"
            Tags {
                "LightMode"="ShadowCaster"
            }
            Offset 1, 1
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_SHADOWCASTER
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles gles3 metal d3d11_9x n3ds wiiu 
            #pragma target 3.0
            uniform sampler2D _niuqutu; uniform float4 _niuqutu_ST;
            uniform float _rongjie;
            uniform float _node_9908;
            uniform sampler2D _mask; uniform float4 _mask_ST;
            uniform sampler2D _mask_02; uniform float4 _mask_02_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
                float2 uv0 : TEXCOORD1;
                float2 uv1 : TEXCOORD2;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                float4 _niuqutu_var = tex2D(_niuqutu,TRANSFORM_TEX(i.uv0, _niuqutu));
                float node_575_ang = _node_9908;
                float node_575_spd = 1.0;
                float node_575_cos = cos(node_575_spd*node_575_ang);
                float node_575_sin = sin(node_575_spd*node_575_ang);
                float2 node_575_piv = float2(0.5,0.5);
                float2 node_575 = (mul(i.uv1-node_575_piv,float2x2( node_575_cos, -node_575_sin, node_575_sin, node_575_cos))+node_575_piv);
                float4 _mask_var = tex2D(_mask,TRANSFORM_TEX(i.uv0, _mask));
                float4 _mask_02_var = tex2D(_mask_02,TRANSFORM_TEX(i.uv0, _mask_02));
                float node_9282 = (pow(((_niuqutu_var.r*_rongjie)+_rongjie),3.0)*pow((_rongjie+(_rongjie*node_575.r)),10.0)*_mask_var.a*_mask_02_var.a);
                clip(node_9282 - 0.5);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
