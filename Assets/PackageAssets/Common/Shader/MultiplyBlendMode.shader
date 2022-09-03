Shader "MultiBlendMode"
{
    Properties
    {
        [HideInInspector] _MainTex ("Sprite Texture", 2D) = "white" {}
        _R("R",Range(0,255)) = 0
        _G("G",Range(0,255)) = 0
        _B("B",Range(0,255)) = 0
        _A("A",Range(0,255)) = 0
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
            #pragma vertex SpriteVert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnitySprites.cginc"
            fixed _R;
            fixed _G;
            fixed _B;
            fixed _A;

            float GetScale(float value,float alpha)
            {
                return (255 - (255-value)*alpha/255)/255;
                // return value / (value + (255-value) * alpha/255);
            }
            
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord);
                fixed3 rgb = c.rgb;
                float3 newRgb;
                newRgb.r = rgb.r * GetScale(_R,_A);
                newRgb.g = rgb.g * GetScale(_G,_A);
                newRgb.b = rgb.b * GetScale(_B,_A);
                c.rgb = newRgb;
                return c;
            }
            ENDCG
        }
    }
}