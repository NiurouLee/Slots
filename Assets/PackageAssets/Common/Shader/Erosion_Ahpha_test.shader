// Shader created with Shader Forge v1.38 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:9361,x:33435,y:32623,varname:node_9361,prsc:2|emission-8794-OUT,alpha-4377-OUT;n:type:ShaderForge.SFN_Tex2d,id:9863,x:32687,y:32770,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:node_9863,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:8732,x:32703,y:32430,ptovrint:False,ptlb:color,ptin:_color,varname:node_8732,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_VertexColor,id:5967,x:32572,y:32924,varname:node_5967,prsc:2;n:type:ShaderForge.SFN_Multiply,id:8794,x:33167,y:32598,varname:node_8794,prsc:2|A-7411-RGB,B-3278-RGB,C-9863-RGB,D-3070-OUT,E-8732-RGB;n:type:ShaderForge.SFN_Tex2d,id:1576,x:32096,y:32949,ptovrint:False,ptlb:Erosion_texture,ptin:_Erosion_texture,varname:_Texture_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8365-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2950,x:32096,y:33146,ptovrint:False,ptlb:soft_value,ptin:_soft_value,varname:node_2950,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:3951,x:32345,y:33025,varname:node_3951,prsc:2|A-1576-R,B-2950-OUT;n:type:ShaderForge.SFN_TexCoord,id:1119,x:32096,y:33222,varname:node_1119,prsc:2,uv:1,uaff:True;n:type:ShaderForge.SFN_Vector1,id:2800,x:32096,y:33399,varname:node_2800,prsc:2,v1:-1.5;n:type:ShaderForge.SFN_Lerp,id:4413,x:32366,y:33267,varname:node_4413,prsc:2|A-2950-OUT,B-2800-OUT,T-1119-U;n:type:ShaderForge.SFN_Subtract,id:629,x:32522,y:33105,varname:node_629,prsc:2|A-3951-OUT,B-4413-OUT;n:type:ShaderForge.SFN_Clamp01,id:3128,x:32687,y:33168,varname:node_3128,prsc:2|IN-629-OUT;n:type:ShaderForge.SFN_Tex2d,id:7411,x:32839,y:32309,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_7411,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-3212-OUT;n:type:ShaderForge.SFN_Time,id:7195,x:31952,y:32301,varname:node_7195,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:7597,x:31952,y:32193,ptovrint:False,ptlb:mask_v,ptin:_mask_v,varname:node_7597,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:6018,x:31962,y:32460,ptovrint:False,ptlb:mask_u,ptin:_mask_u,varname:node_6018,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:479,x:32167,y:32222,varname:node_479,prsc:2|A-7597-OUT,B-7195-T;n:type:ShaderForge.SFN_Multiply,id:2317,x:32165,y:32408,varname:node_2317,prsc:2|A-7195-T,B-6018-OUT;n:type:ShaderForge.SFN_Append,id:6669,x:32369,y:32245,varname:node_6669,prsc:2|A-479-OUT,B-2317-OUT;n:type:ShaderForge.SFN_TexCoord,id:67,x:32369,y:32398,varname:node_67,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:3212,x:32570,y:32189,varname:node_3212,prsc:2|A-6669-OUT,B-67-UVOUT;n:type:ShaderForge.SFN_Multiply,id:3070,x:32895,y:32914,varname:node_3070,prsc:2|A-5967-RGB,B-3128-OUT;n:type:ShaderForge.SFN_Multiply,id:3023,x:33093,y:33139,varname:node_3023,prsc:2|A-3278-A,B-9863-A,C-5967-A,D-3128-OUT;n:type:ShaderForge.SFN_Tex2d,id:3278,x:32703,y:32590,ptovrint:False,ptlb:node_3278,ptin:_node_3278,varname:node_3278,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:4377,x:33247,y:32856,varname:node_4377,prsc:2|A-7411-A,B-3023-OUT,C-1576-A;n:type:ShaderForge.SFN_Time,id:807,x:31197,y:33017,varname:node_807,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:7199,x:31197,y:32909,ptovrint:False,ptlb:mask_v_copy,ptin:_mask_v_copy,varname:_mask_v_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:3337,x:31207,y:33176,ptovrint:False,ptlb:mask_u_copy,ptin:_mask_u_copy,varname:_mask_u_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:3885,x:31412,y:32938,varname:node_3885,prsc:2|A-7199-OUT,B-807-T;n:type:ShaderForge.SFN_Multiply,id:2028,x:31410,y:33124,varname:node_2028,prsc:2|A-807-T,B-3337-OUT;n:type:ShaderForge.SFN_Append,id:760,x:31614,y:32961,varname:node_760,prsc:2|A-3885-OUT,B-2028-OUT;n:type:ShaderForge.SFN_TexCoord,id:1857,x:31614,y:33114,varname:node_1857,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:8365,x:31815,y:32905,varname:node_8365,prsc:2|A-760-OUT,B-1857-UVOUT;proporder:9863-8732-1576-2950-7411-7597-6018-3278-7199-3337;pass:END;sub:END;*/

Shader "magesbox/Erosion_Ahpha_test" {
    Properties {
        _Texture ("Texture", 2D) = "white" {}
        [HDR]_color ("color", Color) = (0.5,0.5,0.5,1)
        _Erosion_texture ("Erosion_texture", 2D) = "white" {}
        _soft_value ("soft_value", Float ) = 0
        _Mask ("Mask", 2D) = "white" {}
        _mask_v ("mask_v", Float ) = 0
        _mask_u ("mask_u", Float ) = 0
        _node_3278 ("node_3278", 2D) = "white" {}
        _mask_v_copy ("mask_v_copy", Float ) = 0
        _mask_u_copy ("mask_u_copy", Float ) = 0
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
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _color;
            uniform sampler2D _Erosion_texture; uniform float4 _Erosion_texture_ST;
            uniform float _soft_value;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _mask_v;
            uniform float _mask_u;
            uniform sampler2D _node_3278; uniform float4 _node_3278_ST;
            uniform float _mask_v_copy;
            uniform float _mask_u_copy;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 texcoord1 : TEXCOORD1;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 uv1 : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.uv1 = v.texcoord1;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float4 node_7195 = _Time;
                float2 node_3212 = (float2((_mask_v*node_7195.g),(node_7195.g*_mask_u))+i.uv0);
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(node_3212, _Mask));
                float4 _node_3278_var = tex2D(_node_3278,TRANSFORM_TEX(i.uv0, _node_3278));
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(i.uv0, _Texture));
                float4 node_807 = _Time;
                float2 node_8365 = (float2((_mask_v_copy*node_807.g),(node_807.g*_mask_u_copy))+i.uv0);
                float4 _Erosion_texture_var = tex2D(_Erosion_texture,TRANSFORM_TEX(node_8365, _Erosion_texture));
                float node_3128 = saturate(((_Erosion_texture_var.r*_soft_value)-lerp(_soft_value,(-1.5),i.uv1.r)));
                float3 emissive = (_Mask_var.rgb*_node_3278_var.rgb*_Texture_var.rgb*(i.vertexColor.rgb*node_3128)*_color.rgb);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,(_Mask_var.a*(_node_3278_var.a*_Texture_var.a*i.vertexColor.a*node_3128)*_Erosion_texture_var.a));
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
            #pragma only_renderers d3d9 d3d11 glcore gles 
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
