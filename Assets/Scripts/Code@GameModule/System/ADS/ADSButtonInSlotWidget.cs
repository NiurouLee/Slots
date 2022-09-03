using System;
using DragonPlus;
using TMPro;
using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIADSButtonInSlot")]
    public class ADSButtonInSlotWidget : SystemWidgetView<ADSButtonInSlotWidgetViewController>
    {
        [ComponentBinder("Root/IntegralGroup/IntegralText")]
        public TMP_Text tmpTextIntegral;

        [ComponentBinder("Root/ReminderGroup")]
        public Transform transformReminder;

        [ComponentBinder("Root/TimerGroup/TimerText")]
        public TMP_Text tmpTextTimer;

        private float _lastShowTime = 0;
        private float _duration;
        private float _cd;

        public ADSButtonInSlotWidget(string address) : base(address)
        {
        }

        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            // AdRewardedVideoPlacementMonitor.Bind(transform.parent.gameObject, eAdReward.SlotRV.ToString());
        }

        public override void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            viewController.OnWidgetClicked(widgetContainerViewController);
        }

        public bool ShouldShow()
        {
            var elapsed = Time.realtimeSinceStartup - _lastShowTime;
            return (elapsed < _duration || elapsed > _cd)
            && ADSController.ShouldShowRV(eAdReward.SlotRV);
        }

        public void Update()
        {
            var leftTime = _duration - (Time.realtimeSinceStartup - _lastShowTime);

            if (tmpTextTimer != null)
            {
                tmpTextTimer.text = XUtility.GetTimeText(leftTime);
            }

            if (leftTime <= 0)
            {
                HideWidget();
                return;
            }

            UpdateCount();
        }

        private void UpdateCount()
        {
            if (tmpTextIntegral != null)
            {
                var count = ADSController.GetFirstBonusCount(eAdReward.SlotRV);
                var coin = ADSController.GetRewardCoin(count);
                tmpTextIntegral.text = $"{coin.GetCommaFormat()}";
            }
        }

        public void Set()
        {
            UpdateCount();
            _lastShowTime = Time.realtimeSinceStartup;
            _duration = ADSController.GetArg1(eAdReward.SlotRV);
            _cd = ADSController.GetRewardedVideoCDinSeconds(eAdReward.SlotRV);

            //Test
            //_duration = 5;
            //_cd = 10;
        }
    }

    public class ADSButtonInSlotWidgetViewController : ViewController<ADSButtonInSlotWidget>
    {
        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();
            EnableUpdate(1);
        }

        public override void Update()
        {
            base.Update();

            if (view != null)
            {
                if (view.ShouldShow())
                {
                    if (view.transform != null && view.transform.parent != null
                    && view.transform.parent.gameObject != null && view.transform.parent.gameObject.activeInHierarchy == false)
                    {
                        view.ShowWidget();
                        view.Set();
                    }
                    else
                    {
                        view.Update();
                    }
                }
                else
                {
                    if (view.transform != null && view.transform.parent != null
                    && view.transform.parent.gameObject != null && view.transform.parent.gameObject.activeInHierarchy == true)
                    {
                        view.HideWidget();
                    }
                }
            }
        }

        public void OnWidgetClicked(SystemWidgetContainerViewController widgetContainerViewController)
        {
            view.HideWidget();

            if (!ADSController.ShouldShowRV(eAdReward.SlotRV))
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
            }
            else
            {
                var count = ADSController.GetFirstBonusCount(eAdReward.SlotRV);
                var coin = ADSController.GetRewardCoin(count);
                ADSController.TryShowRewardedVideoWithCollectRewardPopup(eAdReward.SlotRV, coin, coin,
                    eAdReward.SlotRV.ToString());
            }
        }
    }
}