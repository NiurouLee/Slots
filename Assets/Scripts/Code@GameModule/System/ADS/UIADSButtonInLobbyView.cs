using System;
using DragonPlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UIADSButtonInLobbyView : View<UIADSButtonInLobbyViewController>
    {
        [ComponentBinder("Root/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("Root/ReminderGroup")]
        public Transform transformReminder;

        [ComponentBinder("Root/TimerGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        [ComponentBinder("")]
        public Button button;

        private float _lastShowTime = 0;
        private float _duration;
        private float _cd;

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();

            AdRewardedVideoPlacementMonitor.Bind(this.gameObject, eAdReward.LobbyRV.ToString());
            button.onClick.AddListener(OnButtonClick);
            button.interactable = true;
        }

        public bool ShouldShow()
        {
            var elasped = Time.realtimeSinceStartup - _lastShowTime;
            return (elasped < _duration || elasped > _cd)
                   && ADSController.ShouldShowRV(eAdReward.LobbyRV);
        }

        private void OnButtonClick()
        {
            gameObject.SetActive(false);
            if (ShouldShow() == false)
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                if (button != null) { button.interactable = true; }
            }
            else
            {
                if (button != null) { button.interactable = false; }

                var count = ADSController.GetFirstBonusCount(eAdReward.LobbyRV);
                var coin = ADSController.GetRewardCoin(count);

                ADSController.TryShowRewardedVideoWithCollectRewardPopup(
                    eAdReward.LobbyRV,
                    coin,
                    coin,
                    eAdReward.LobbyRV.ToString(), (b) =>
                    {
                        if (button != null) { button.interactable = true; }
                    });
            }
        }

        protected override void EnableView()
        {
            base.EnableView();
            gameObject.SetActive(false);
            Set();
        }

        public void Set()
        {
            button.interactable = true;

            var adsController = Client.Get<ADSController>();
            if (adsController == null) { return; }

            if (tmpTextIntegral != null)
            {
                var count = ADSController.GetFirstBonusCount(eAdReward.LobbyRV);
                var coin = ADSController.GetRewardCoin(count);
                tmpTextIntegral.text = $"{coin.GetCommaFormat()} COINS";
            }

            _lastShowTime = Time.realtimeSinceStartup;
            _duration = ADSController.GetArg1(eAdReward.LobbyRV);
            _cd = ADSController.GetRewardedVideoCDinSeconds(eAdReward.LobbyRV);

            //test
            //_duration = 5;
            //_cd = 10;
            //XDebug.Log($"1111111111111 ad config, placeID:{_adPlaceID}, arg1:{_duration}, cd:{_cd}");

            if (tmpTextTimer != null)
            {
                TimeSpan timeSpan;
                tmpTextTimer.text = ADSController.GetTimeLeft(_duration, out timeSpan);
            }
        }

        public void Update()
        {
            TimeSpan timeRemains;
            var timeString = ADSController.GetTimeLeft(_duration - (Time.realtimeSinceStartup - _lastShowTime), out timeRemains);
            if (tmpTextTimer != null) { tmpTextTimer.text = timeString; }

            if (timeRemains.TotalSeconds <= 0) { Hide(); }
        }
    }

    public class UIADSButtonInLobbyViewController : ViewController<UIADSButtonInLobbyView>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);
        }

        public override void Update()
        {
            base.Update();

            if (view.ShouldShow())
            {
                if (view.IsActive() == false)
                {
                    view.Show();
                    view.Set();
                }
                else
                {
                    view.Update();
                }
            }
            else
            {
                if (view.IsActive() == true)
                {
                    view.Hide();
                }
            }
        }
    }
}
