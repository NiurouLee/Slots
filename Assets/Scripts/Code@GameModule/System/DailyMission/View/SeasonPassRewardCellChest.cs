//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-30 14:12
//  Ver : 1.0.0
//  Description : SeasonPassRewardCellChest.cs
//  ChangeLog :
//  **********************************************

using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassRewardCellChest:View<SeasonPassRewardCellChessViewController>
    {
        [ComponentBinder("BG")]
        public Button btnGoldenChest;

        [ComponentBinder("BG")]
        public Animator animator;
        
        [ComponentBinder("BG/DescriptionText")]
        public Transform transLockDescribe;
        
        [ComponentBinder("BG/GoldenChestInformationButton/Icon")]
        public Button btnChestInfo;
        
        [ComponentBinder("Guide")]
        public Animator animatorGuide;

        [ComponentBinder("TipsContainer")]
        public Transform transTipContainer;
        
        public Canvas Canvas;
        
        
        protected override void BindingComponent()
        {
            base.BindingComponent();
            Canvas = transform.GetComponent<Canvas>();
        }
    }

    public class SeasonPassRewardCellChessViewController : ViewController<SeasonPassRewardCellChest>
    {
        private Action actionGuide;
        private SeasonPassPageViewController PageViewController;

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.btnChestInfo.onClick.AddListener(OnBtnGoldenChestTipClick);
            view.btnGoldenChest.onClick.AddListener(OnBtnGoldenChestClick);
        }

        public void InitView(SeasonPassPageViewController controller)
        {
            PageViewController = controller;
            CheckAndShowOpen();
            SubscribeEvent<EventSeasonPassChestUpdate>(OnChestUnlockStep);
        }

        public async void CheckAndShowOpen()
        {
            var userGuideController = PageViewController.UserGuide.viewController;
            if (!userGuideController.NeedShowGuide() && userGuideController.NeedChestUnLock())
            {
                await XUtility.WaitNFrame(5,this);
                HideLockInfo();
                XUtility.PlayAnimation(view.animator,"Idle");
            }      
        }

        private void HideLockInfo()
        {
            view.transLockDescribe.gameObject.SetActive(false);  
        }

        private async void OnChestUnlockStep(EventSeasonPassChestUpdate evt)
        {
            if (evt.ActionName == "Up")
            {
                actionGuide = evt.Action;
                HideLockInfo();
                ModifyProgressSortingOrder(true);
                XUtility.PlayAnimation(view.animatorGuide, "Idle");
                await XUtility.WaitSeconds(0.5f);
                view.animatorGuide.gameObject.SetActive(true);
                return;
            }

            if (evt.ActionName == "CloseTip")
            {
                PageViewController.HideGoldenChestTip();
            }
        }
        
        public void ModifyProgressSortingOrder(bool toTop)
        {
            if (toTop)
            {
                view.Canvas.gameObject.SetActive(true);
                view.Canvas.overrideSorting = true;
                view.Canvas.sortingOrder = 20;
                view.Canvas.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }
        
            view.Canvas.overrideSorting = false;
            view.Canvas.sortingOrder = 1;
            view.Canvas.sortingLayerID = SortingLayer.NameToID("UI");
        }

        private async void OnBtnGoldenChestTipClick()
        {
            PageViewController.ToggleGoldenChestTips(view.transTipContainer.parent.TransformPoint(view.transTipContainer.localPosition),view.Canvas.overrideSorting);
        }

        private async void OnBtnGoldenChestClick()
        {
            if (view.Canvas.overrideSorting)
            {
                view.btnGoldenChest.interactable = false;
                view.animatorGuide.gameObject.SetActive(false);
                await XUtility.PlayAnimationAsync(view.animator, "Open");
                ModifyProgressSortingOrder(false);
                view.btnGoldenChest.interactable = true;
                actionGuide?.Invoke();
            }
            else
            {
                PopupStack.ShowPopup<SeasonPassGoldenPrizeShow>();      
            }
        }
    }
}