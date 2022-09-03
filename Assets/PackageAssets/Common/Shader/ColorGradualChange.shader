Shader "ColorGradualChange"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _TargetTex("Sprite Texture", 2D) = "white" {}
        _TransformDegree("变换程度",Range(0,1)) = 1
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
            sampler2D _TargetTex;
            fixed _TransformDegree;
            fixed4 TargetSpriteTexture (float2 uv)
            {
                fixed4 color = tex2D (_TargetTex, uv);
                return color;
            }
            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 c = SampleSpriteTexture(IN.texcoord);
                fixed4 target = TargetSpriteTexture(IN.texcoord);
                c = lerp(c,target,_TransformDegree);
                return c;
            }
            ENDCG
        }
    }
}