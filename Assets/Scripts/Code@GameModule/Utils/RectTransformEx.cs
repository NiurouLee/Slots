/**
 * RectTransformEx.cs
 * 
 * @author mosframe / https://github.com/mosframe
 * 
 */

namespace GameModule
{
    using UnityEngine;

    /// <summary>
    /// RectTransform Extention
    /// </summary>
    public static class RectTransformEx
    {
        /// <summary>
        /// 设置为最大尺寸
        /// </summary>
        public static RectTransform SetFullSize( this RectTransform self ) {

            self.sizeDelta  = new Vector2(0.0f,0.0f);
            self.anchorMin  = new Vector2(0.0f,0.0f);
            self.anchorMax  = new Vector2(1.0f,1.0f);
            self.pivot      = new Vector2(0.5f,0.5f);
            return self;
        }

        /// <summary>
        /// 取得尺寸
        /// </summary>
        public static Vector2 GetSize( this RectTransform self ) {
            return self.rect.size;
        }

        /// <summary>
        /// 设定尺寸
        /// </summary>
        public static void SetSize( this RectTransform self, Vector2 newSize ) {

            var pivot   = self.pivot;
            var dist    = newSize - self.rect.size;
            self.offsetMin = self.offsetMin - new Vector2( dist.x * pivot.x, dist.y * pivot.y );
            self.offsetMax = self.offsetMax + new Vector2( dist.x * (1f - pivot.x), dist.y * (1f - pivot.y) );
        }

        /// <summary>
        /// 从左侧设置尺寸
        /// </summary>
        public static RectTransform SetSizeFromLeft( this RectTransform self, float rate ) {

            self.SetFullSize();

            var width = self.rect.width;

            self.anchorMin  = new Vector2(0.0f,0.0f);
            self.anchorMax  = new Vector2(0.0f,1.0f);
            self.pivot      = new Vector2(0.0f,1.0f);
            self.sizeDelta  = new Vector2(width*rate,0.0f);

            return self;
        }

        /// <summary>
        /// 从右侧设置尺寸
        /// </summary>
        public static RectTransform SetSizeFromRight( this RectTransform self, float rate ) {

            self.SetFullSize();

            var width = self.rect.width;

            self.anchorMin  = new Vector2(1.0f,0.0f);
            self.anchorMax  = new Vector2(1.0f,1.0f);
            self.pivot      = new Vector2(1.0f,1.0f);
            self.sizeDelta  = new Vector2(width*rate,0.0f);

            return self;
        }

        /// <summary>
        /// 从顶部设置尺寸
        /// </summary>
        public static RectTransform SetSizeFromTop( this RectTransform self, float rate ) {

            self.SetFullSize();

            var height = self.rect.height;

            self.anchorMin  = new Vector2(0.0f,1.0f);
            self.anchorMax  = new Vector2(1.0f,1.0f);
            self.pivot      = new Vector2(0.0f,1.0f);
            self.sizeDelta  = new Vector2(0.0f,height*rate);

            return self;
        }

        /// <summary>
        /// 从底部设置尺寸
        /// </summary>
        public static RectTransform SetSizeFromBottom( this RectTransform self, float rate ) {

            self.SetFullSize();

            var height = self.rect.height;

            self.anchorMin  = new Vector2(0.0f,0.0f);
            self.anchorMax  = new Vector2(1.0f,0.0f);
            self.pivot      = new Vector2(0.0f,0.0f);
            self.sizeDelta  = new Vector2(0.0f,height*rate);

            return self;
        }

        /// <summary>
        /// 设定偏移
        /// </summary>
        public static void SetOffset( this RectTransform self, float left, float top, float right, float bottom ) {

            self.offsetMin = new Vector2( left, top );
            self.offsetMax = new Vector2( right, bottom );
        }

        /// <summary>
        /// 检查Rect中是否包含屏幕坐标
        /// </summary>
        public static bool InScreenRect( this RectTransform self, Vector2 screenPos ) {

            var canvas = self.GetComponentInParent<Canvas>();
            switch( canvas.renderMode )
            {
            case RenderMode.ScreenSpaceCamera:
                {
                    var camera = canvas.worldCamera;
                    if( camera != null )
                    {
                        return RectTransformUtility.RectangleContainsScreenPoint( self, screenPos, camera );
                    }
                }
                break;
            case RenderMode.ScreenSpaceOverlay:
                return RectTransformUtility.RectangleContainsScreenPoint( self, screenPos );
            case RenderMode.WorldSpace:
                return RectTransformUtility.RectangleContainsScreenPoint( self, screenPos );
            }
            return false;
        }

        /// <summary>
        /// 检查RectTransform中是否包含另一个RectTransform
        /// </summary>
        public static bool InScreenRect( this RectTransform self, RectTransform rectTransform ) {

            var rect1 = GetScreenRect( self );
            var rect2 = GetScreenRect( rectTransform );
            return rect1.Overlaps( rect2 );
        }

        /// <summary>
        /// 获取屏幕坐标Rect
        /// </summary>
        public static Rect GetScreenRect( this RectTransform self ) {

            var rect = new Rect();
            var canvas = self.GetComponentInParent<Canvas>();
            var camera = canvas.worldCamera;
            if( camera != null )
            {
                var corners = new Vector3[4];
                self.GetWorldCorners( corners );
                rect.min = camera.WorldToScreenPoint( corners[0] );
                rect.max = camera.WorldToScreenPoint( corners[2] );
            }
            
            return rect;
        }
    }
}