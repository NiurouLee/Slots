//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-01 12:44
//  Ver : 1.0.0
//  Description : SeasonPassUserGuide.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassUserGuide:View<SeasonPassUserGuideViewController>
    {
        [ComponentBinder("ProgressGroup")]
        public Canvas canasProgress;
        [ComponentBinder("GuideGroup")]
        public Transform transGuideGroup;
        [ComponentBinder("GuideGroup/Button")]
        public Button btnGuide;
        [ComponentBinder("GuideGroup/GuideBubbleLevelNum")]
        public Canvas transLevelUpGuide;
        [ComponentBinder("GuideGroup/GuideBubbleGoldenChest")]
        public Canvas transOpenGoldenChestGuide;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            btnGuide.interactable = true;
            transGuideGroup.gameObject.SetActive(false);
        }

    }

    public class SeasonPassUserGuideViewController : ViewController<SeasonPassUserGuide>
    {
        public int nGuideStep;
        public Action finishAction;
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            view.btnGuide.GetComponent<Button>().onClick.AddListener(OnBtnClick);
        }

        public async void CheckAndShowGuide(Action action, Action finish)
        {
            finishAction = finish;
            if (view.transGuideGroup.gameObject.activeInHierarchy)
            {
                finishAction?.Invoke();
                return;
            }
            await XUtility.WaitNFrame(5,this);
            if (CanShowGuide())
            {
                action?.Invoke();
                StartUserGuide();     
            }
            else
            {
                finishAction?.Invoke();
            }
        }
        
        
        public bool NeedShowGuide()
        {
            return "false" == Client.Storage.GetItem(GetGuideKey(), "false");
        }
        
        
        public bool NeedChestUnLock()
        {
            return Client.Get<SeasonPassController>().IsLevel100 && Client.Get<SeasonPassController>().Paid;
        }

        public bool CanShowGuide()
        {
            return NeedShowGuide() && NeedChestUnLock();
        }
        
        private string GetGuideKey()
        {
            return "SeasonPass_20_Guide" + Client.Get<UserController>().GetUserId();
        }
        
                

        private void OnBtnClick()
        {
            nGuideStep++;
            nGuideStep = Math.Min(nGuideStep, 2);
            StartUserGuide();
        }

        private async void StartUserGuide()
        {
            view.transGuideGroup.gameObject.SetActive(true);
            switch (nGuideStep)
            {
                case 0:
                    view.transLevelUpGuide.gameObject.SetActive(true);
                    view.transOpenGoldenChestGuide.gameObject.SetActive(false);
                    ModifyProgressSortingOrder(true);
                    break;
                case 1:
                    view.btnGuide.interactable = false;
                    view.transLevelUpGuide.gameObject.SetActive(false);        
                    view.transOpenGoldenChestGuide.gameObject.SetActive(true);
                    EventBus.Dispatch(new EventSeasonPassChestUpdate("Up", OnBtnClick));
                    break;
                case 2:
                    finishAction?.Invoke();
                    view.transGuideGroup.gameObject.SetActive(false);
                    ModifyProgressSortingOrder(false);
                    Client.Storage.SetItem(GetGuideKey(), "true");
                    break;
            }
        }
        
        public void ModifyProgressSortingOrder(bool toTop)
        {
            if (toTop)
            {
                view.canasProgress.gameObject.SetActive(true);
                view.canasProgress.overrideSorting = true;
                view.canasProgress.sortingOrder = 5;
                view.canasProgress.sortingLayerID = SortingLayer.NameToID("SystemPopup");
                return;
            }
        
            view.canasProgress.overrideSorting = false;
            view.canasProgress.sortingOrder = 1;
            view.canasProgress.sortingLayerID = SortingLayer.NameToID("UI");
        }
    }
}