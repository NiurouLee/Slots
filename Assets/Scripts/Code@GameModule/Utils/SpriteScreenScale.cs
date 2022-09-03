using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameModule
{
    public class SpriteScreenScale : MonoBehaviour
    {
        public void UpdateScale()
        {
            var sprite = this.GetComponent<SpriteRenderer>();
            float width = sprite.bounds.size.x;
            float height = sprite.bounds.size.y;

            float worldScreenHeight = Camera.main.orthographicSize * 2.0f;
            float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

            if (height > worldScreenHeight && width > worldScreenWidth)
            {
                return;
            }
            float scaleW = width / height;
            float scaleSW = worldScreenWidth / worldScreenHeight;
            if (scaleW > scaleSW)
            {
                this.transform.localScale = new Vector3(worldScreenHeight / height, worldScreenHeight / height, 1);
            }
            else
            {
                this.transform.localScale = new Vector3(worldScreenWidth / width, worldScreenWidth / width, 1);
            }
        }
    }
}