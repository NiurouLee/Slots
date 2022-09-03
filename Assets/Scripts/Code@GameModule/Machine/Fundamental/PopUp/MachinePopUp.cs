// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/13/18:02
// Ver : 1.0.0
// Description : MachinePopUp.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class MachinePopUp : TransformHolder
    {
        protected Animator animator;

        protected Action popUpCloseAction;

        public Transform container;

        public MachinePopUp(Transform transform) :
            base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
            
            animator = transform.GetComponent<Animator>();
        }


        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
        }

        public virtual float GetPopUpMaskAlpha()
        {
            return 0.63f;
        }

        public virtual bool EnableAutoAdapt()
        {
            return true;
        }

        public void Close()
        {
            PopUpManager.Instance.ClosePopUp(this);
        }
        
        public override void OnOpen()
        {
            if(animator != null && animator.HasState("Open"))
                animator.Play("Open");
        }

        public virtual void SetPopUpCloseAction(Action action)
        {
            popUpCloseAction = action;
        }
        
        public override async Task OnClose()
        {
            if (animator && animator.HasState("Close"))
                await XUtility.PlayAnimationAsync(animator, "Close");
            
        }


        public virtual void DoCloseAction()
        {
            popUpCloseAction?.Invoke();

        }


        public virtual bool IsCloseMustUnpauseMusic()
        {
            return false;
        }
    }
}