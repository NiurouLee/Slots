// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/04/08/14:37
// Ver : 1.0.0
// Description : AdSystemWidget.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIADSButtonInSystemWidget")]
    public class AdTaskSystemWidget: SystemWidgetView<AdTaskSystemWidgetViewController>
    {
        [ComponentBinder("ScheduleNum")] 
        public Text scheduleNum;
        
        [ComponentBinder("Schedule")] 
        public Image progress;
        
        public AdTaskSystemWidget(string address)
            : base(address)
        {
            
        }
        
        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            XDebug.Log("WidgetClicked");
            SoundController.PlayButtonClick();
            viewController.OnWidgetClicked(widgetContainerViewController);
        }
    }
    
    public class AdTaskSystemWidgetViewController : ViewController<AdTaskSystemWidget>
    {
        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventAdTaskConfigRefreshed>(OnStateRefresh);
            SubscribeEvent<EventAdTaskStepUpdated>(OnTaskStepUpdated);
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            UpdateAdTaskProgressText();
        }

        protected  void OnStateRefresh(EventAdTaskConfigRefreshed evt)
        { 
            UpdateAdTaskProgressText();
        }
        
        public void UpdateAdTaskProgressText()
        {
            var adTaskInfo = AdController.Instance.GetAdTaskInfo();
            if (adTaskInfo != null)
            {
                view.scheduleNum.text = $"{adTaskInfo.TaskStep.ToString()}/{adTaskInfo.Rewards.Count.ToString()}";
                view.progress.fillAmount = (float) adTaskInfo.TaskStep / adTaskInfo.Rewards.Count;

                if (adTaskInfo.TaskStep >= adTaskInfo.Rewards.Count)
                {
                    view.systemWidgetContainerViewController.RemoveSystemWidgetView(view);
                }
            }
            else
            {
                view.scheduleNum.text = "0/10";
                view.progress.fillAmount = 0;
                AdController.Instance.RefreshAdLogicConfig();
            }
        }
    
        protected  void OnTaskStepUpdated(EventAdTaskStepUpdated evt)
        { 
            UpdateAdTaskProgressText();
        }
        
        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            if (AdController.Instance.GetAdTaskInfo() != null)
            {
                PopupStack.ShowPopupNoWait<AdRushPopup>();
            }
        }
    }
}