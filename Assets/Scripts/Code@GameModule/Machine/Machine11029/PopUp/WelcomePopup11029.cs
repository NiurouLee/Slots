// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/23/17:09
// Ver : 1.0.0
// Description : WelcomePopup.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class WelcomePopup11029:MachinePopUp
    {
        private bool abrupted = false;
        public WelcomePopup11029(Transform transform) :
            base(transform)
        {
            var pointerEventCustomHandler = ((TransformHolder) this).transform.gameObject.AddComponent<PointerEventCustomHandler>();
            pointerEventCustomHandler.BindingPointerClick(OnPointerClicked);
        }

        public void OnPointerClicked(PointerEventData evtData)
        {
            if (!abrupted)
            {
                abrupted = true;
                if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Close"))
                {
                    Close();
                }
            }
        }

        public void ShowWelcome()
        {
            if (animator != null)
            {
                animator.Play("Open");
                context.WaitSeconds(3, () =>
                {
                    if (!abrupted)
                        Close();
                });
            }
        }
    }
}