// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/02/14/10:59
// Ver : 1.0.0
// Description : AdTaskLobbyEntranceView.cs
// ChangeLog :
// **********************************************
using UnityEngine.UI;
namespace GameModule
{
    public class AdTaskLobbyEntranceView:View<AdTaskLobbyEntranceViewController>
    {
        [ComponentBinder("ScheduleNum")] 
        public Text scheduleNum;

        [ComponentBinder("Schedule")] 
        public Image progress;
    }

   
    public class AdTaskLobbyEntranceViewController : ViewController<AdTaskLobbyEntranceView>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            var entranceButton = view.transform.GetComponent<Button>();
            entranceButton.onClick.AddListener(OnEntranceButtonClicked);
            
            UpdateAdTaskProgressText();
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventAdTaskConfigRefreshed>(OnStateRefresh);
            SubscribeEvent<EventAdTaskStepUpdated>(OnTaskStepUpdated);
        }
        
        protected  void OnTaskStepUpdated(EventAdTaskStepUpdated evt)
        { 
            UpdateAdTaskProgressText();
        }

        protected  void OnStateRefresh(EventAdTaskConfigRefreshed evt)
        { 
            UpdateAdTaskProgressText();
        }

        protected void OnEntranceButtonClicked()
        {
            if (AdController.Instance.GetAdTaskInfo() != null)
            {
                PopupStack.ShowPopupNoWait<AdRushPopup>();
            }
            else
            {
                AdController.Instance.RefreshAdLogicConfig();
            }
        }

        public void UpdateAdTaskProgressText()
        {
           var adTaskInfo = AdController.Instance.GetAdTaskInfo();
           if (adTaskInfo != null)
           {
               view.scheduleNum.text = $"{adTaskInfo.TaskStep.ToString()}/{adTaskInfo.Rewards.Count.ToString()}";
               view.progress.fillAmount = (float) adTaskInfo.TaskStep / adTaskInfo.Rewards.Count;
           }
           else
           {
               view.scheduleNum.text = "0/10";
               view.progress.fillAmount = 0;
               AdController.Instance.RefreshAdLogicConfig();
           }
        }
    }
}