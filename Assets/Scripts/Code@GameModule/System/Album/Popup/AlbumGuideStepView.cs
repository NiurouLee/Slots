// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/22/15:20
// Ver : 1.0.0
// Description : AlbumGuideStepView.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class AlbumGuideStepView:View
    {
        [ComponentBinder("Mask")]
        Transform mask;

        public Action handler;
       
        public AlbumGuideStepView(string address)
            : base(address)
        {
         
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            var pointerEventCustomHandler = mask.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnMaskClicked);
        }

        private float blockStartTime = 0;
        public void SetGuideClickHandler(Action inHandler)
        {
            handler = inHandler;

            blockStartTime = Time.realtimeSinceStartup;
        }

        private bool inResponse = false;
        public void OnMaskClicked(PointerEventData pd)
        {
            if(inResponse) 
                return;

            var deltaTime = Time.realtimeSinceStartup - blockStartTime;

            if (deltaTime < 1)
            {
                return;
            }
            
            inResponse = true;
            
            var animator = transform.GetComponent<Animator>();
            XUtility.PlayAnimation(animator, "Close", () =>
            {
                handler.Invoke();
                Destroy();
            });
          
        }
    }
}