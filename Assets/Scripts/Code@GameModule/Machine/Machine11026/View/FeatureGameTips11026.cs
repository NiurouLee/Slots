using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FeatureGameTips11026: TransformHolder
    {
        
        protected Transform FeatureNotice;
        public FeatureGameTips11026(Transform inTransform) : base(inTransform)
        {
           ComponentBinder.BindingComponent(this, transform);
            FeatureNotice = inTransform;
        }

        public void FeatureTipShow()
        {
            FeatureNotice.gameObject.SetActive(true);
            FeatureNotice.gameObject.GetComponent<Animator>().Play("UILinkGame11026");
        }
        
        public void FeatureTipHide()
        {
            FeatureNotice.gameObject.SetActive(false);
        }
    }
}