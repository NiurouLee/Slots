using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class FreeGameTips11026: TransformHolder
    {
        [ComponentBinder("BtnTips2")]
        protected Transform megaTip;

        [ComponentBinder("BtnTips3")]
        protected Transform superTip;

        public FreeGameTips11026(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public void showFreeTip(bool isMega,bool isSuper)
        {
            if (isMega == true)
            {
                transform.gameObject.SetActive(true);
                megaTip.gameObject.SetActive(true);
                superTip.gameObject.SetActive(false);
            }else if (isSuper == true)
            {
                transform.gameObject.SetActive(true);
                megaTip.gameObject.SetActive(false);
                superTip.gameObject.SetActive(true);
            }
            else
            {
                transform.gameObject.SetActive(false);
                megaTip.gameObject.SetActive(false);
                superTip.gameObject.SetActive(false);
            }
        }

    }
}