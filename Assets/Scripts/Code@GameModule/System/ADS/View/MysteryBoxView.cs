// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/21:05
// Ver : 1.0.0
// Description : MysteryBoxView.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIADBoxButton")]
    public class MysteryBoxView:View<MysteryBoxViewController>, IOnContextDestroy
    {
        [ComponentBinder("TimerText")] 
        public Text timerText;

        public Button watchButton;
        public MysteryBoxView(string address)
        :base(address)
        {
            
        }

        protected override void OnViewSetUpped()
        {
            watchButton = transform.GetComponent<Button>();
            base.OnViewSetUpped();
        }

        public void OnContextDestroy()
        {
            Destroy();
        }
    }

    public class MysteryBoxViewController : ViewController<MysteryBoxView>
    {
        protected int countDownTime;
        protected long intervalTime;
        protected float lastTriggerMysteryBox;
        protected float enterMachineTime;
        protected float enterCoolingTime = 60;
        
        protected bool isDrag;
        protected Vector2 mouseOffset;
        protected Vector2 startPos;
         
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            
            view.watchButton.onClick.AddListener(OnWatchRvClicked);
            
            lastTriggerMysteryBox = 0;
            
            var dragDropEventCustomHandler = view.watchButton.gameObject.AddComponent<DragDropEventCustomHandler>();
            dragDropEventCustomHandler.BindingBeginDragAction(OnBeginDrag);
            dragDropEventCustomHandler.BindingEndDragAction(OnEndDrag);
            dragDropEventCustomHandler.BindingDragAction(OnDrag);
        }
        
        protected void OnWatchRvClicked()
        {
            if (isDrag)
            {
                return;
            }

            if (AdController.Instance.ShouldShowRV(eAdReward.MysteryBox,false))
            {
                DisableUpdate();
                view.Hide();
                AdController.Instance.TryShowRewardedVideo(eAdReward.MysteryBox, OnWatchRvFinished);
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
        }

        protected async void OnWatchRvFinished(bool success, string failed)
        {
            if (success)
            {
                var claimResult = await AdController.Instance.ClaimRvReward(eAdReward.MysteryBox);
               
                if (claimResult != null)
                {
                    var claimPopup = await PopupStack.ShowPopup<AdRewardClaimPopup>();
                    
                    claimPopup.viewController.SetUpClaimUI(claimResult, eAdReward.MysteryBox, () =>
                    {
                        EnableUpdate(2);
                    });
                }
                else
                {
                    XDebug.Log("ClaimRewardFromServerFailed");
                    view.watchButton.interactable = true;
                }
            }
            else
            {
                view.watchButton.interactable = true;
            }
        }

        public void OnBeginDrag(PointerEventData pointerEventData)
        {
            isDrag = true;
            RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)view.transform.parent, pointerEventData.position, pointerEventData.pressEventCamera, out startPos);
            mouseOffset = (Vector2)(view.transform.localPosition) - startPos;
        }
        
        public void OnDrag(PointerEventData pointerEventData)
        {
            if (isDrag)
            {
                if (!RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform) view.transform.parent,
                    pointerEventData.position, pointerEventData.pressEventCamera, out Vector2 localCursor))
                    return;
                view.transform.localPosition = localCursor + mouseOffset;
            }
        }
        
        public void OnEndDrag(PointerEventData pointerEventData)
        {
            if(!isDrag) return;

            isDrag = false;
             
            Vector2 contentSize = view.rectTransform.sizeDelta;
             
            var safeRect = GetScreenSafeRect(contentSize);
       
            var localPosition = view.transform.localPosition;
            
            view.transform.localPosition = GetSafePosition(safeRect, localPosition);
        }
        
        public Vector2 GetSafePosition(Rect safeRect, Vector2 localPosition)
        {
            var targetY = localPosition.y;
               
            if (localPosition.y > safeRect.yMax)
            {
                targetY = safeRect.yMax;
            }
            else if (localPosition.y < safeRect.yMin)
            {
                targetY = safeRect.yMin;
            }
                
            var targetX = localPosition.x;
                
            if (localPosition.x <= 0 )
            {
                targetX = safeRect.xMin;
            }
            else if (localPosition.x > 0)
            {
                targetX = safeRect.xMax;
            }
            
            return new Vector2(targetX, targetY);
        }
        
        public void SetUpViewState()
        {
            countDownTime = AdController.Instance.adModel.GetArg1(eAdReward.MysteryBox);;
            intervalTime = AdController.Instance.adModel.GetRewardedVideoCDinSeconds(eAdReward.MysteryBox);

            enterMachineTime = Time.realtimeSinceStartup;
            
            var safeRect = GetScreenSafeRect(view.rectTransform.sizeDelta);

            if (ViewManager.Instance.IsPortrait)
            {
                view.transform.localPosition = new Vector3(safeRect.xMax, (safeRect.yMin + safeRect.yMax) * 0.5f);
            }
            else
            {
                view.transform.localPosition = new Vector3(safeRect.xMax, safeRect.yMin);
            }

            if (GetCountDown() < 0 || !AdController.Instance.ShouldShowRV(eAdReward.MysteryBox,false))
            {
                view.Hide();
            }
            
            EnableUpdate(2);
            
            Update();
        }

        public bool CanShowRvView()
        {
            if (intervalTime - (Time.realtimeSinceStartup - lastTriggerMysteryBox) <= 0
                && (Time.realtimeSinceStartup - enterMachineTime) >= enterCoolingTime)
            {
                return AdController.Instance.ShouldShowRV(eAdReward.MysteryBox, false);
            }
            
            return false;
        }

        public float GetCountDown()
        {
            if (lastTriggerMysteryBox <= 0)
                return -1;
            
            return countDownTime - (Time.realtimeSinceStartup - lastTriggerMysteryBox);
        }

        public override void Update()
        {
            if (view.IsActive())
            {
                var countDown = GetCountDown();

                if (countDown > 0)
                {
                    var duration = TimeSpan.FromSeconds(countDown);
                    view.timerText.text = duration.ToString(@"mm\:ss");
                }
                else
                {
                    view.Hide();
                }
            }
            else
            {
                if (CanShowRvView())
                {
                    lastTriggerMysteryBox = Time.realtimeSinceStartup;
                    view.Show();

                    ShowTriggerMysteryBoxAnimation();
                }
            }
        }

        protected void ShowTriggerMysteryBoxAnimation()
        {
            var animator = view.transform.GetComponent<Animator>();
            if (animator != null)
            {
                animator.Play("Open");
            }
        }
        
        public Rect GetScreenSafeRect(Vector2 contentSize)
        {
            bool isPortrait = ViewManager.Instance.IsPortrait;

            var referenceResolution = ViewResolution.referenceResolutionLandscape;

            if (isPortrait)
            {
                referenceResolution = ViewResolution.referenceResolutionPortrait;
            }

            var controlPanelHeight = MachineConstant.controlPanelHeight;

            var machineUIHeight = MachineConstant.topPanelHeight + MachineConstant.controlPanelHeight;

            float screenCutOut = 0;

            if (isPortrait)
            {
                controlPanelHeight = MachineConstant.controlPanelVHeight;
                machineUIHeight = MachineConstant.topPanelVHeight + MachineConstant.controlPanelVHeight;
            }

#if UNITY_IOS
            if (!isPortrait)
            {
                screenCutOut = MachineConstant.widgetOffset;
            }
#endif

            var safeRect = new Rect(-referenceResolution.x * 0.5f + contentSize.x * 0.5f + screenCutOut,
                -referenceResolution.y * 0.5f + controlPanelHeight + contentSize.y * 0.5f,
                referenceResolution.x - contentSize.x - screenCutOut * 2, referenceResolution.y - machineUIHeight - contentSize.y);

            return safeRect;
        }
    }
}