using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteTexRectApply : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sp = transform.GetComponent<SpriteRenderer>();

        if (sp)
        {
            if (sp.material.HasProperty("_SpriteText_Rect"))
            {
                var sprite = sp.sprite;
                Vector4 result = new Vector4(sprite.textureRect.min.x / sprite.texture.width,
                    sprite.textureRect.min.y / sprite.texture.height,
                    sprite.textureRect.width / sprite.texture.width,
                    sprite.textureRect.height / sprite.texture.height);
                sp.material.SetVector("_SpriteText_Rect", result);
            }
        }
    }
}
