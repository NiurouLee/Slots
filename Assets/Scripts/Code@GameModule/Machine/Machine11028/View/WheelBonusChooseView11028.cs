//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-12 12:14
//  Ver : 1.0.0
//  Description : WheelBonusChooseView11028.cs
//  ChangeLog :
//  **********************************************

using System;
using UnityEngine;

namespace GameModule
{
    public class WheelBonusChooseView11028:TransformHolder
    {
        private Animator _animator;
        [ComponentBinder("DayButton/IconGroup/IconBG")] 
        private Transform transDayBtn;
        [ComponentBinder("NightButton/IconGroup/IconBG")] 
        private Transform transNightBtn;
        public Action<bool> SelectAction;
        
        public WheelBonusChooseView11028(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this,inTransform);
            _animator = transform.GetComponent<Animator>();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var pointerEventDayHandler = transDayBtn.Bind<PointerEventCustomHandler>(true);
            pointerEventDayHandler.BindingPointerClick((e)=>OnRapidHitDayClicked());
            var pointerEventNightHandler = transNightBtn.Bind<PointerEventCustomHandler>(true);
            pointerEventNightHandler.BindingPointerClick((e)=>OnRapidHitNightClicked());
        }

        private void OnRapidHitDayClicked()
        {
            if (SelectAction != null)
            {
                SelectAction.Invoke(false);
                SelectAction = null;   
            }
        }

        private void OnRapidHitNightClicked()
        {
            if (SelectAction != null)
            {
                SelectAction.Invoke(true);
                SelectAction = null;   
            }
        }

        public void PlayAnimation(string animName)
        {
            XUtility.PlayAnimation(_animator,animName);
        }

        public void OpenChoose()
        {
            Show();
            PlayAnimation("Idle");
        }
    }
}