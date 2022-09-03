Shader "WinFrameAdd"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}

        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)

        _Color ("Tint", Color) = (1,1,1,1)
        _UvMultiplier("UvMultiplier",Range(0,10)) = 2
        _Offset("Offset",Range(0,50)) = 0
        _HueOffset("HueOffset",Range(0,1)) = 0
        _RectangleArgs("RectangleArgs",Vector) = (0.4,0.0,0.1,1)
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

       
        Lighting Off
        ZWrite Off
        Blend SrcAlpha One

        Pass
        {
            CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnitySprites.cginc"
            fixed _HolePosX;
            fixed _HolePosY;
            fixed _HoleSizeX;
            fixed _HoleSizeY;
            fixed _Gradient;
            fixed _Offset;
            fixed _HueOffset;
            fixed _UvMultiplier;
            fixed4 _RectangleArgs;

           
            float roundedFrame(fixed2 pos, fixed2 size, float radius, float thickness)
            {
                float d = length(max(abs(pos), size) - size) - radius;
                return smoothstep(0.55, 0.45, abs(d / thickness) * 3.0);
            }

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

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed2 offset = IN.texcoord - fixed2(0.5, 0.5);

                fixed y = length(max(abs(offset), _RectangleArgs.xx) - _RectangleArgs.xx) - _RectangleArgs.y;
                fixed sy = sign(y);
                y = abs(y / _RectangleArgs.z)*5;
                y = y < 0.999 ? y : 1;
                y = sy < 0 ? 0 : y;
  
               // offset = clamp(offset, - _RectangleArgs.x, _RectangleArgs.x);
             
                 fixed2 transformUv = fixed2((atan2(offset.y, offset.x) + UNITY_PI) / (2 * UNITY_PI) * _UvMultiplier, y);
               // transformUv = clamp(transformUv, 0, _UvMultiplier);
               
               // transformUv.x = abs(offset.y - 0.001) < 0.001 ? 0: transformUv.x;
                  transformUv.x += _Offset;
                
               
                fixed4 c = SampleSpriteTexture(transformUv);

                if(_HueOffset > 0) {
                    fixed3 hsl = RGBToHSL(c.rgb);
                     c.rgb = HSLToRGB(hsl + fixed3(_HueOffset, 0, 0));
                }
              //  c.x = transformUv.x;
             //   c.a = 1;
              
                return c;
            }
            ENDCG
        }
    }
}