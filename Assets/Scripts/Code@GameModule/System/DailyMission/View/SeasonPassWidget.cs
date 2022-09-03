//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-27 15:02
//  Ver : 1.0.0
//  Description : SeasonPassMachineEntranceView.cs
//  ChangeLog :
//  **********************************************

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UISeasonPassButton")]
    public class SeasonPassWidget: SystemWidgetView<SeasonPassWidgetViewController>
    {
        [ComponentBinder("Root/TimerGroup/TimerText")]
        public TextMeshProUGUI txtTimeLeft;
        [ComponentBinder("Root/ReminderGroup")]
        public Transform transReminderGroup;
        [ComponentBinder("Root/ReminderGroup/NoticeText")]
        public TextMeshProUGUI txtReminderNotice;
        public SeasonPassWidget(string address) : base(address)
        {
            
        }

        protected override void EnableView()
        {

        }
        
        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            viewController.OnWidgetClicked(widgetContainerViewController);
        }

    }

    public class SeasonPassWidgetViewController : ViewController<SeasonPassWidget>
    {
        private SeasonPassController _passController;
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        //    SubscribeEvent<EventSeasonPassUnlocked>(OnSeasonPassUnlocked);
        }

        public override void OnViewDidLoad()
        {
            _passController = Client.Get<SeasonPassController>();
            base.OnViewDidLoad();
            Update();
            EnableUpdate(1);
            view.transform.gameObject.SetActive(!_passController.IsLocked);
        }
        
        public override void Update()
        {
            if (_passController.IsLocked) return;
            var collectRewardCount = Client.Get<SeasonPassController>().CollectRewardCount;
            view.txtReminderNotice.text = collectRewardCount.ToString();
            view.transReminderGroup.gameObject.SetActive(collectRewardCount>0);
            view.txtTimeLeft.text = Client.Get<SeasonPassController>().GetSeasonPassTimeLeftInMachine();
        }

        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            if (_passController.IsLocked)
                return;
            
            EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(DailyMissionMainPopup), (object)new []{"SeasonPass","Machine"})));    
        }

    //     private void OnSeasonPassUnlocked(EventSeasonPassUnlocked evt)
    //     {
    //      //   view.Show();
    //         Update();
    //     }
     }
}