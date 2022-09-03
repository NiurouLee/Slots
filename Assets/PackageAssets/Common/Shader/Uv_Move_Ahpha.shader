// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|emission-1097-OUT,alpha-6974-OUT;n:type:ShaderForge.SFN_Tex2d,id:1188,x:32814,y:32763,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_1188,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9271-OUT;n:type:ShaderForge.SFN_Color,id:4058,x:32814,y:32556,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_4058,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:8676,x:32814,y:32957,varname:node_8676,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:8059,x:32814,y:33144,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_8059,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:1097,x:33011,y:32666,varname:node_1097,prsc:2|A-4058-RGB,B-1188-RGB,C-8676-RGB,D-8059-RGB;n:type:ShaderForge.SFN_Multiply,id:6974,x:33011,y:32876,varname:node_6974,prsc:2|A-4058-A,B-1188-A,C-8676-A,D-8059-A;n:type:ShaderForge.SFN_TexCoord,id:9043,x:32191,y:33027,varname:node_9043,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Time,id:4769,x:31830,y:32907,varname:node_4769,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:7899,x:31846,y:32674,ptovrint:False,ptlb:U,ptin:_U,varname:node_7899,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:7297,x:31830,y:33155,ptovrint:False,ptlb:V,ptin:_V,varname:_U_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:9099,x:31988,y:32754,varname:node_9099,prsc:2|A-7899-OUT,B-4769-T;n:type:ShaderForge.SFN_Multiply,id:2625,x:31991,y:33027,varname:node_2625,prsc:2|A-4769-T,B-7297-OUT;n:type:ShaderForge.SFN_Append,id:9955,x:32191,y:32866,varname:node_9955,prsc:2|A-9099-OUT,B-2625-OUT;n:type:ShaderForge.SFN_Add,id:9271,x:32412,y:32958,varname:node_9271,prsc:2|A-9955-OUT,B-9043-UVOUT;proporder:1188-4058-8059-7899-7297;pass:END;sub:END;*/

Shader "UI/Uv_Move_Ahpha" {
    Properties {
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendMode("SrcBlendMode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)] _DesBlendMode("DesBlendMode", Float) = 10
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Float) = 2
        _Texture ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Mask ("Mask", 2D) = "white" {}
        _U ("U", Float ) = 0
        _V ("V", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend[_SrcBlendMode][_DesBlendMode]
            Cull[_CullMode]
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP( float, _U)
                UNITY_DEFINE_INSTANCED_PROP( float, _V)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float _U_var = UNITY_ACCESS_INSTANCED_PROP( Props, _U );
                float4 node_4769 = _Time;
                float _V_var = UNITY_ACCESS_INSTANCED_PROP( Props, _V );
                float2 node_9271 = (float2((_U_var*node_4769.g),(node_4769.g*_V_var))+i.uv0);
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9271, _Texture));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 emissive = (_Color_var.rgb*_Texture_var.rgb*i.vertexColor.rgb*_Mask_var.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_Color_var.a*_Texture_var.a*i.vertexColor.a*_Mask_var.a));
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
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma multi_compile_fog
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                V2F_SHADOW_CASTER;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                TRANSFER_SHADOW_CASTER(o)
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
