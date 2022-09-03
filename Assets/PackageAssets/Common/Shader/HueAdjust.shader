Shader "HueAdjust"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        [Toggle(SHIFT_HUE)]
        _ShiftHue ("使用色相偏移", Float) = 0
        _HueValue("色相",Range(0,1)) = 0
        _LightValue("亮度",Range(0,1)) = 0
        _SaturateValue("饱和度",Range(0,1)) = 0
//        _TransformDegree("变换程度",Range(0,1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma shader_feature SHIFT_HUE
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnitySprites.cginc"
            fixed _HueValue;
            fixed _LightValue;
            fixed _SaturateValue;
            // fixed _TransformDegree;
 
            float3 RGBToHSL(float3 color)
            {
                float3 hsl; // init to 0 to avoid warnings ? (and reverse if + remove first part)

                float fmin = min(min(color.r, color.g), color.b); //Min. value of RGB
                float fmax = max(max(color.r, color.g), color.b); //Max. value of RGB
                float delta = fmax - fmin; //Delta RGB value

                hsl.z = (fmax + fmin) / 2.0; // Luminance

                if (delta == 0.0) //This is a gray, no chroma...
                {
                    hsl.x = 0.0; // Hue
                    hsl.y = 0.0; // Saturation
                }
                else //Chromatic data...
                {
                    if (hsl.z < 0.5)
                        hsl.y = delta / (fmax + fmin); // Saturation
                    else
                        hsl.y = delta / (2.0 - fmax - fmin); // Saturation

                    float deltaR = (((fmax - color.r) / 6.0) + (delta / 2.0)) / delta;
                    float deltaG = (((fmax - color.g) / 6.0) + (delta / 2.0)) / delta;
                    float deltaB = (((fmax - color.b) / 6.0) + (delta / 2.0)) / delta;

                    if (color.r == fmax)
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

                    rgb.r = HueToRGB(f1, f2, hsl.x + (1.0 / 3.0));
                    rgb.g = HueToRGB(f1, f2, hsl.x);
                    rgb.b = HueToRGB(f1, f2, hsl.x - (1.0 / 3.0));
                }

                return rgb;
            }
            float GetIntermediateValue(float start,float target,float progress)
            {
                return start + (target-start) * progress;
            }
            fixed3 GetIntermediateValue(fixed3 start,fixed3 target,float progress)
            {
                return start + (target-start) * progress;
            }
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord);
                fixed3 hsl = RGBToHSL(c.rgb);
                #ifdef SHIFT_HUE
                    fixed3 targetHSL = fixed3(_HueValue+hsl.x,hsl.y*_SaturateValue,hsl.z*_LightValue);
                    fixed3 targetRGB = HSLToRGB(targetHSL);
                    c.rgb = targetRGB;
                    // c.rgb = GetIntermediateValue(c.rgb,targetRGB,_TransformDegree);
                #else
                    
                    fixed3 targetHSL = fixed3(_HueValue,hsl.y*_SaturateValue,hsl.z*_LightValue);
                    fixed3 targetRGB = HSLToRGB(targetHSL);
                    c.rgb = targetRGB;
                    // c.rgb = GetIntermediateValue(c.rgb,targetRGB,_TransformDegree);
                #endif
                
                return c;
            }
            ENDCG
        }
    }
}