// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/08/19:32
// Ver : 1.0.0
// Description : CommonTextBubbleView.cs
// ChangeLog :
// **********************************************

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class CommonTextBubbleView:View<ViewController>
    {
        [ComponentBinder("ContentText")] 
        public TextMeshProUGUI txtContent;
        
        public static CommonTextBubbleView activeInstance;
 
        public Animator animator;

        public bool isStandalone = false;
        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            animator = transform.GetComponent<Animator>();
            var selectEventCustomHandler = transform.Bind<SelectEventCustomHandler>(true);
            selectEventCustomHandler.BindingDeselectedAction((baseEventData) =>
            {
                if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Close")) {
                    Hide();
                }
            });
        }

        protected override void EnableView()
        {
          //  base.EnableView();
        }

        public void SetText(string bubbleText)
        {
            txtContent.text = bubbleText;
        }

        private CancelableCallback _callback;
        public async void ShowBubble(float autoHide = 3)
        {
            if (transform.gameObject.activeSelf)
                return;
         
            if (!isStandalone)
            {
                if (activeInstance != null && activeInstance.animator != null && !activeInstance.animator.GetCurrentAnimatorStateInfo(0).IsName("Close"))
                {
                    activeInstance.Hide();
                }

                activeInstance = this;
            }

            Show();
           
            await XUtility.PlayAnimationAsync(animator, "Open", viewController);
            
            EventSystem.current.SetSelectedGameObject(transform.gameObject);

            if (_callback != null)
            {
                _callback.CancelCallback();
            }
            
            _callback = viewController.WaitForSeconds(autoHide, Hide);
        }
        
        public override async void Hide()
        {
            if (animator && animator.HasState("Close"))
                await XUtility.PlayAnimationAsync(animator, "Close", viewController);
            base.Hide();
        }
    }
}