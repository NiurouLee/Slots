// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/14:58
// Ver : 1.0.0
// Description : SuperFreeProgressView11001.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace GameModule
{
    public class SpiningMaskView11026: TransformHolder
    {
        
        
        protected Transform spiningMask;

        
        public SpiningMaskView11026(Transform inTransform)
            : base(inTransform)
        {
            ComponentBinder.BindingComponent(this,transform);
                         spiningMask = inTransform;
        }
        
        public void ShowRollsMasks(Wheel wheel)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }

            for (var i = 0; i < 5; ++i)
            {
                var o = roll.Find("spiningMask").Find("BlackMask" + i).gameObject;
                o.SetActive(true);

                var render = o.GetComponent<SpriteRenderer>();
                DOTween.Kill(render);
                render.DOFade(0f, 0f);
                render.DOFade(0.6f, 1.0f);
            }
        }

        public void FadeOutRollMask(Wheel wheel, int rollIndex)
        {
            var roll = wheel.transform.Find("Rolls");
            if (roll == null)
            {
                return;
            }

            var render = roll.Find("spiningMask").Find("BlackMask" + rollIndex).gameObject.GetComponent<SpriteRenderer>();
            DOTween.Kill(render);
            render.DOFade(0, 0.3f);
        }


    }
}