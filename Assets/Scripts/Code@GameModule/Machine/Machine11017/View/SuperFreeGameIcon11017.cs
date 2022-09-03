using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class SuperFreeGameIcon11017: TransformHolder
    {
        protected Transform tranSuperFreeIcon;
        public SuperFreeGameIcon11017(Transform inTransform) : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            tranSuperFreeIcon = inTransform;
        }
        
        public void ShowSuperFreeIcon()
        {
            var _extraState = context.state.Get<ExtraState11017>();
            uint level = _extraState.GetLevel();
            if (level == 5)
            {
                tranSuperFreeIcon.gameObject.SetActive(true);
            }
        }
        
        public void HideSuperFreeIcon()
        {
            tranSuperFreeIcon.gameObject.SetActive(false);
        }

    }
}