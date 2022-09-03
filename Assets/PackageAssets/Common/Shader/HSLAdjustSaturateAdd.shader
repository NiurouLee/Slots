// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HSLAdjustSaturateAdd"
{
    Properties
    {
       [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
       _Color ("Alpha Color Key", Color) = (0,0,0,1)
       _HSLRangeMin ("HSL Affect Range Min", Range(0, 1)) = 0
       _HSLRangeMax ("HSL Affect Range Max", Range(0, 1)) = 1
       _HSLHue ("Hue", Range(-1,1)) = 0
       _HSLSaturation ("Saturation", Range(-1,1)) = 0
       _HSLLight("Light", Range(-1,1)) = 0
       _HSLAlpha ("Alpha", Range(0,1)) = 0
       _StencilComp ("Stencil Comparison", Float) = 8
       _Stencil ("Stencil ID", Float) = 0
       _StencilOp ("Stencil Operation", Float) = 0
       _StencilWriteMask ("Stencil Write Mask", Float) = 255
       _StencilReadMask ("Stencil Read Mask", Float) = 255
       _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }
        ColorMask [_ColorMask]

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha One

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON

            sampler2D _MainTex;
            float4 _Color;
            float _HSLRangeMin;
            float _HSLRangeMax;
            float _HSLHue;
            float _HSLSaturation;
            float _HSLLight;
            float _HSLAlpha;
 

            struct Vertex
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv_MainTex : TEXCOORD0;
            };

            struct Fragment
            {
                float4 vertex : POSITION;
                float4 color    : COLOR;
                float2 uv_MainTex : TEXCOORD0;
            };

            Fragment vert(Vertex v)
            {
                Fragment o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_MainTex = v.uv_MainTex;
                o.color = v.color * _Color;

                return o;
            }

			float3 RGBToHSL(float3 color)
			{
				float3 hsl; // init to 0 to avoid warnings ? (and reverse if + remove first part)
				
				float fmin = min(min(color.r, color.g), color.b);    //Min. value of RGB
				float fmax = max(max(color.r, color.g), color.b);    //Max. value of RGB
				float delta = fmax - fmin;             //Delta RGB value

				hsl.z = (fmax + fmin) / 2.0; // Luminance

				if (delta == 0.0)		//This is a gray, no chroma...
				{
					hsl.x = 0.0;	// Hue
					hsl.y = 0.0;	// Saturation
				}
				else                                    //Chromatic data...
				{
					if (hsl.z < 0.5)
						hsl.y = delta / (fmax + fmin); // Saturation
					else
						hsl.y = delta / (2.0 - fmax - fmin); // Saturation
					
					float deltaR = (((fmax - color.r) / 6.0) + (delta / 2.0)) / delta;
					float deltaG = (((fmax - color.g) / 6.0) + (delta / 2.0)) / delta;
					float deltaB = (((fmax - color.b) / 6.0) + (delta / 2.0)) / delta;

					if (color.r == fmax )
						hsl.x = deltaB - deltaG; // Hue
					else if (color.g == fmax)
						hsl.x = (1.0 / 3.0) + deltaR - deltaB; // Hue
					else if (color.b == fmax)
						hsl.x = (2.0 / 3.0) + deltaG - deltaR; // Hue

					if (hsl.x < 0.0)
						hsl.x += 1.0; // Hue
					else if (hsl.x > 1.0)
						hsl.x -= 1.0; // Hue
				}

				return hsl;
			}

			float HueToRGB(float f1, float f2, float hue)
			{
				if (hue < 0.0)
					hue += 1.0;
				else if (hue > 1.0)
					hue -= 1.0;
				float res;
				if ((6.0 * hue) < 1.0)
					res = f1 + (f2 - f1) * 6.0 * hue;
				else if ((2.0 * hue) < 1.0)
					res = f2;
				else if ((3.0 * hue) < 2.0)
					res = f1 + (f2 - f1) * ((2.0 / 3.0) - hue) * 6.0;
				else
					res = f1;
				return res;
			}

			float3 HSLToRGB(float3 hsl)
			{
				float3 rgb;
				
				if (hsl.y == 0.0)
					rgb = float3(hsl.zzz); // Luminance
				else
				{
					float f2;
					
					if (hsl.z < 0.5)
						f2 = hsl.z * (1.0 + hsl.y);
					else
						f2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);
						
					float f1 = 2.0 * hsl.z - f2;
					
					rgb.r = HueToRGB(f1, f2, hsl.x + (1.0/3.0));
					rgb.g = HueToRGB(f1, f2, hsl.x);
					rgb.b= HueToRGB(f1, f2, hsl.x - (1.0/3.0));
				}
				
				return rgb;
			}

            

            // float Epsilon = 1e-10;
            //
            // float3 rgb2hcv(in float3 RGB)
            // {
            //     // Based on work by Sam Hocevar and Emil Persson
            //     float4 P = lerp(float4(RGB.bg, -1.0, 2.0/3.0), float4(RGB.gb, 0.0, -1.0/3.0), step(RGB.b, RGB.g));
            //     float4 Q = lerp(float4(P.xyw, RGB.r), float4(RGB.r, P.yzx), step(P.x, RGB.r));
            //     float C = Q.x - min(Q.w, Q.y);
            //     float H = abs((Q.w - Q.y) / (6 * C + Epsilon) + Q.z);
            //     return float3(H, C, Q.x);
            // }
            //
            // float3 rgb2hsl(in float3 RGB)
            // {
            //     float3 HCV = rgb2hcv(RGB);
            //     float L = HCV.z - HCV.y * 0.5;
            //     float S = HCV.y / (1 - abs(L * 2 - 1) + Epsilon);
            //     return float3(HCV.x, S, L);
            // }
            //
            // float3 hsl2rgb(float3 c)
            // {
            //     c = float3(frac(c.x), clamp(c.yz, 0.0, 1.0));
            //     float3 rgb = clamp(abs(fmod(c.x * 6.0 + float3(0.0, 4.0, 2.0), 6.0) - 3.0) - 1.0, 0.0, 1.0);
            //     return c.z + c.y * (rgb - 0.5) * (1.0 - abs(2.0 * c.z - 1.0));
            // }

            float4 frag(Fragment IN) : COLOR
            {
                float4 color = tex2D (_MainTex, IN.uv_MainTex) * IN.color;
            	//color.rgb =  pow(color.rgb, half3(2.2, 2.2, 2.2));
            	float3 hsl = RGBToHSL(color.rgb);
                float affectMult = step(_HSLRangeMin,hsl.r) * step( hsl.r, _HSLRangeMax);// - smoothstep(_HSLRangeMax,_HSLRangeMax + 0.02, hsl.r);
                float3 rgb = HSLToRGB(saturate(hsl + float3(_HSLHue, _HSLSaturation, _HSLLight) * affectMult));
            //	rgb =  pow(rgb, half3(1/2.2, 1/2.2, 1/2.2));
                return float4(rgb, color.a + _HSLAlpha);
            }

            ENDCG
        }
    }
}
