using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FeatureGameTips11017: TransformHolder
    {
        public FeatureGameTips11017(Transform inTransform) : base(inTransform)
        {
           ComponentBinder.BindingComponent(this, transform);
        }

        public void FeatureTipShow()
        {
            transform.gameObject.SetActive(true);
            transform.gameObject.GetComponent<Animator>().Play("FeatureNotice");
        }
        
        public void FeatureTipHide()
        {
            transform.gameObject.SetActive(false);
        }
    }
}