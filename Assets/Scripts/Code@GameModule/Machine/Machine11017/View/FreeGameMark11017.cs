using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


namespace GameModule
{
    public class FreeGameMark11017: TransformHolder
    {
        [ComponentBinder("Notice")]
        protected Transform noticeTip;
        
        private bool iconCanClick;
        
        public FreeGameMark11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            iconCanClick = true;
            var pointerEventCustomHandler = noticeTip.parent.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerDown(IconClick);
            pointerEventCustomHandler.BindingPointerUp(IconClickUp);
            
            XUtility.ShowTipAndAutoHide(noticeTip, 5, 0.2f, true, context);
        }

        public void IconClick(PointerEventData pointerEventData)
        {
            if (!iconCanClick) return;
            
            noticeTip.parent.GetComponent<Image>().color = new Color(200f/255f, 200f/255f, 200f/255f, 1);
            
            if (!noticeTip.gameObject.activeSelf)
            {
                AudioUtil.Instance.PlayAudioFxOneShot("Close");
                XUtility.ShowTipAndAutoHide(noticeTip, 5, 0.2f, true, context);
            }
        }

        public void IconClickUp(PointerEventData pointerEventData)
        {
            if(!iconCanClick)return;
            noticeTip.parent.GetComponent<Image>().color = Color.white;
        }
      
        public void NoticeTipDark()
        {
            noticeTip.parent.GetComponent<Image>().color = new Color(1, 1, 1, 0.25f);
             iconCanClick = false;
        }
        
        public void NoticeTipBright()
        {
            noticeTip.parent.GetComponent<Image>().color = Color.white;
            iconCanClick = true;
        }
    }
}