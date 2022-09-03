using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class SuperFreeGameGlowIcon11017: TransformHolder
    {
        public SuperFreeGameGlowIcon11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }
        
        public void ShowSuperFreeGlowIcon()
        {
            var extraState = context.state.Get<ExtraState11017>();
            uint level = extraState.GetLevel();
            if (level == 5)
            {
                transform.gameObject.SetActive(true);
                transform.gameObject.GetComponent<Animator>().Play("SFGGlow");
            }
        }
        
        public void HideSuperFreeGlowIcon()
        {
            transform.gameObject.SetActive(false);
        }

    }
}