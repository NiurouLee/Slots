// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "UI/UVliudong"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.BlendMode)] _SrcBlendMode("SrcBlendMode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _DesBlendMode("DesBlendMode", Float) = 10
		[Enum(UnityEngine.Rendering.CullMode)] _CullMode("CullMode", Float) = 2
		_Emission("Emission", Float) = 1
		_Opacity("Opacity", Float) = 0
		_Color("Color", Color) = (0,0,0,0)
		_MainTex("MainTex", 2D) = "white" {}
		_MainTex_U("MainTex_U", Float) = 0
		_MainTex_V("MainTex_V", Float) = 0
		_UvMask("UvMask", 2D) = "white" {}
		_UvMask_U("UvMask_U", Float) = 0
		_UvMask_V("UvMask_V", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_ColorTex("ColorTex", 2D) = "white" {}
		_Lerp_Custom("Lerp_Custom", Float) = 0
		_Custon("Custon", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] _tex4coord2( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull [_CullMode]
		ZWrite Off
		Blend [_SrcBlendMode] [_DesBlendMode]
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Lambert keepalpha noshadow noambient novertexlights nolightmap  nodynlightmap nodirlightmap nofog nometa noforwardadd 
		#undef TRANSFORM_TEX
		#define TRANSFORM_TEX(tex,name) float4(tex.xy * name##_ST.xy + name##_ST.zw, tex.z, tex.w)
		struct Input
		{
			float2 uv_texcoord;
			float4 uv2_tex4coord2;
			float4 vertexColor : COLOR;
		};

		uniform half4 _Color;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform half _MainTex_U;
		uniform half _MainTex_V;
		uniform sampler2D _Custon;
		SamplerState sampler_Custon;
		uniform float4 sampler_Custon_ST;
		uniform half4 _Custon_ST;
		SamplerState sampler_MainTex;
		uniform float4 sampler_MainTex_ST;
		uniform half _Lerp_Custom;
		uniform sampler2D _UvMask;
		uniform float4 _UvMask_ST;
		uniform half _UvMask_U;
		uniform half _UvMask_V;
		uniform sampler2D _Mask;
		uniform half4 _Mask_ST;
		uniform half _Emission;
		uniform sampler2D _ColorTex;
		uniform half4 _ColorTex_ST;
		uniform half _Opacity;
		SamplerState sampler_Mask;
		SamplerState sampler_UvMask;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 appendResult20 = (half4(_MainTex_U , _MainTex_V , 0.0 , 0.0));
			float2 uvsampler_Custon = i.uv_texcoord * sampler_Custon_ST.xy + sampler_Custon_ST.zw;
			float2 uv_Custon = i.uv_texcoord * _Custon_ST.xy + _Custon_ST.zw;
			half4 tex2DNode66 = tex2D( _Custon, uv_Custon );
			half2 appendResult64 = (half2(uvsampler_Custon.x , tex2DNode66.r));
			float4 uv2s4sampler_MainTex = i.uv2_tex4coord2;
			uv2s4sampler_MainTex.xy = i.uv2_tex4coord2.xy * sampler_MainTex_ST.xy + sampler_MainTex_ST.zw;
			half2 appendResult70 = (half2(uv2s4sampler_MainTex.z , uv2s4sampler_MainTex.w));
			half2 lerpResult68 = lerp( appendResult64 , ( uvsampler_Custon + appendResult70 ) , uv2s4sampler_MainTex.y);
			half4 lerpResult61 = lerp( ( half4( uv_MainTex, 0.0 , 0.0 ) + ( _Time.y * appendResult20 ) ) , half4( lerpResult68, 0.0 , 0.0 ) , _Lerp_Custom);
			half4 tex2DNode2 = tex2D( _MainTex, lerpResult61.xy );
			float2 uv_UvMask = i.uv_texcoord * _UvMask_ST.xy + _UvMask_ST.zw;
			half4 appendResult24 = (half4(_UvMask_U , _UvMask_V , 0.0 , 0.0));
			half4 tex2DNode3 = tex2D( _UvMask, ( half4( uv_UvMask, 0.0 , 0.0 ) + ( _Time.y * appendResult24 ) ).xy );
			half4 color41 = IsGammaSpace() ? half4(1,1,1,1) : half4(1,1,1,1);
			float2 uv_Mask = i.uv_texcoord * _Mask_ST.xy + _Mask_ST.zw;
			half4 tex2DNode8 = tex2D( _Mask, uv_Mask );
			half3 desaturateInitialColor49 = tex2DNode8.rgb;
			half desaturateDot49 = dot( desaturateInitialColor49, float3( 0.299, 0.587, 0.114 ));
			half3 desaturateVar49 = lerp( desaturateInitialColor49, desaturateDot49.xxx, 1.0 );
			float2 uv_ColorTex = i.uv_texcoord * _ColorTex_ST.xy + _ColorTex_ST.zw;
			o.Emission = ( half4( ( ( ( ( ( (_Color).rgb * (tex2DNode2).rgb ) * ( (tex2DNode3).rgb * (color41).rgb ) ) * (desaturateVar49).xyz ) * _Emission ) * (tex2D( _ColorTex, uv_ColorTex )).rgb ) , 0.0 ) * i.vertexColor ).rgb;
			o.Alpha = ( i.vertexColor.a * ( _Opacity * ( tex2DNode8.a * tex2DNode2.a * tex2DNode3.a ) ) );
		}

		ENDCG
	}
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18400
-1638;179;1531;833;5529.215;3927.609;5.788877;True;True
Node;AmplifyShaderEditor.RangedFloatNode;17;-1894.031,-484.8135;Half;False;Property;_MainTex_U;MainTex_U;5;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;63;-3123.307,-502.1569;Inherit;True;0;66;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;69;-2827.859,-143.0804;Inherit;True;1;2;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;66;-2885.623,-377.5448;Inherit;True;Property;_Custon;Custon;13;0;Create;True;0;0;False;0;False;-1;7c269681052d8e045a311c7b2c6c589f;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;18;-1896.031,-400.8135;Half;False;Property;_MainTex_V;MainTex_V;6;0;Create;True;0;0;False;0;False;0;0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;16;-1738.878,-545.1259;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;20;-1693.031,-459.8136;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;65;-2550.938,-489.8666;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;70;-2541.76,-46.85151;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-1770.562,5.949146;Half;False;Property;_UvMask_V;UvMask_V;9;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;64;-2567.603,-790.7131;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-1768.562,-77.05077;Half;False;Property;_UvMask_U;UvMask_U;8;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1654.379,-679.0422;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;27;-1584.652,-141.1875;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;24;-1567.562,-53.05078;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SwitchNode;67;-2240.562,-524.7117;Inherit;True;0;2;8;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-2308.76,-124.8513;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1511.031,-482.8135;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;59;-1537.546,-344.9436;Inherit;False;Property;_Lerp_Custom;Lerp_Custom;12;0;Create;True;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;22;-1329.031,-673.8135;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;23;-1658.911,-266.279;Inherit;False;0;3;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;28;-1385.561,-76.05077;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;68;-1913.058,-262.8524;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;61;-1341.929,-410.0288;Inherit;False;3;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;29;-1333.561,-261.0503;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SamplerNode;2;-1183.572,-435.0031;Inherit;True;Property;_MainTex;MainTex;4;0;Create;True;0;0;False;0;False;-1;None;691e587c75d8f0f4196e9d07ae6caadd;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;37;-1084.494,-681.9608;Half;False;Property;_Color;Color;3;1;[HDR];Create;True;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1188.506,-192.4749;Inherit;True;Property;_UvMask;UvMask;7;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;41;-1094.69,22.12227;Half;False;Constant;_ColorTex2;ColorTex2;11;0;Create;True;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;47;-853.6373,18.00528;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;43;-870.7676,-432.8203;Inherit;True;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;45;-868.1871,-193.6019;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;8;-469.3073,-245.3465;Inherit;True;Property;_Mask;Mask;10;0;Create;True;0;0;False;0;False;-1;None;f6029422bfec9824992203e5d415d1a9;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ComponentMaskNode;46;-845.9852,-682.3961;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DesaturateOpNode;49;-200.4977,-368.9393;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-622.2477,-451.9467;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-625.8996,-189.0041;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;48;-139.1946,-245.024;Inherit;False;True;True;True;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-456.4658,-453.8178;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;6;392.2711,-440.8889;Half;False;Property;_Emission;Emission;1;0;Create;True;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;32;229.812,-299.9548;Inherit;True;Property;_ColorTex;ColorTex;11;0;Create;True;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;148.2266,-460.5125;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;617.479,-459.9384;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;50;526.9247,-178.0013;Inherit;False;Property;_Opacity;Opacity;2;0;Create;True;0;0;False;0;False;0;-0.8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;33;550.8118,-299.9548;Inherit;False;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;35;229.8217,-77.95605;Inherit;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;812.8235,-459.6067;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;763.5247,-97.40134;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;73;818.0792,-298.3477;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;74;1098.134,-412.5643;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;991.5374,-62.89069;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1368.539,-378.678;Half;False;True;-1;2;ASEMaterialInspector;0;0;Lambert;UI/UVliudong;False;False;False;False;True;True;True;True;True;True;True;True;False;False;False;False;False;False;False;False;False;Off;2;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;8;5;False;-1;1;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;17;0
WireConnection;20;1;18;0
WireConnection;65;0;63;2
WireConnection;65;1;66;2
WireConnection;70;0;69;3
WireConnection;70;1;69;4
WireConnection;64;0;63;1
WireConnection;64;1;66;1
WireConnection;24;0;26;0
WireConnection;24;1;25;0
WireConnection;67;0;64;0
WireConnection;67;1;65;0
WireConnection;71;0;63;0
WireConnection;71;1;70;0
WireConnection;21;0;16;0
WireConnection;21;1;20;0
WireConnection;22;0;14;0
WireConnection;22;1;21;0
WireConnection;28;0;27;0
WireConnection;28;1;24;0
WireConnection;68;0;67;0
WireConnection;68;1;71;0
WireConnection;68;2;69;2
WireConnection;61;0;22;0
WireConnection;61;1;68;0
WireConnection;61;2;59;0
WireConnection;29;0;23;0
WireConnection;29;1;28;0
WireConnection;2;1;61;0
WireConnection;3;1;29;0
WireConnection;47;0;41;0
WireConnection;43;0;2;0
WireConnection;45;0;3;0
WireConnection;46;0;37;0
WireConnection;49;0;8;0
WireConnection;4;0;46;0
WireConnection;4;1;43;0
WireConnection;42;0;45;0
WireConnection;42;1;47;0
WireConnection;48;0;49;0
WireConnection;38;0;4;0
WireConnection;38;1;42;0
WireConnection;39;0;38;0
WireConnection;39;1;48;0
WireConnection;5;0;39;0
WireConnection;5;1;6;0
WireConnection;33;0;32;0
WireConnection;35;0;8;4
WireConnection;35;1;2;4
WireConnection;35;2;3;4
WireConnection;34;0;5;0
WireConnection;34;1;33;0
WireConnection;51;0;50;0
WireConnection;51;1;35;0
WireConnection;74;0;34;0
WireConnection;74;1;73;0
WireConnection;75;0;73;4
WireConnection;75;1;51;0
WireConnection;0;2;74;0
WireConnection;0;9;75;0
ASEEND*/
//CHKSM=6E8558AACBA2A7E2A8C5849044D1F5F7E02763B9