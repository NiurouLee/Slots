
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class UICrazeSmashMain : View
    {
        [ComponentBinder("SilverEggGroup")]
        public Transform transformSilverEggGroup;

        [ComponentBinder("GoldenEggGroup")]
        public Transform transformGoldEggGroup;

        [ComponentBinder("InformationGroup/StateGroup/TheDayAndFinishState")]
        public Transform transformTheDayState;

        [ComponentBinder("InformationGroup/StateGroup/TheDayAndFinishState/TimerGroup/TimerText")]
        public Text textTheDayTimer;

        [ComponentBinder("InformationGroup/StateGroup/WaitingState")]
        public Transform transformWaitState;

        [ComponentBinder("InformationGroup/StateGroup/WaitingState/TimerGroup/DayTimer")]
        public Transform transformWaitDayStyle;

        [ComponentBinder("InformationGroup/StateGroup/WaitingState/TimerGroup/Timer")]
        public Transform transformWaitTimeStyle;

        [ComponentBinder("InformationGroup/StateGroup/WaitingState/TimerGroup/DayTimer/TimerText")]
        public Text textWaitDay;

        [ComponentBinder("InformationGroup/StateGroup/WaitingState/TimerGroup/Timer/TimerText")]
        public Text textWaitTimer;


        public UICrazeSmashEggGroupView silverGroup;
        public UICrazeSmashEggGroupView goldGroup;

        private CrazeSmashController _smashController;

        protected override void BindingComponent()
        {
            base.BindingComponent();
            silverGroup = AddChild<UICrazeSmashEggGroupView>(transformSilverEggGroup);
            goldGroup = AddChild<UICrazeSmashEggGroupView>(transformGoldEggGroup);
        }

        protected override void OnViewSetUpped()
        {
            _smashController = Client.Get<CrazeSmashController>();
            base.OnViewSetUpped();
        }

        public void RefreshTime()
        {
            var available = _smashController.available;
            if (available)
            {
                transformTheDayState.gameObject.SetActive(true);
                transformWaitState.gameObject.SetActive(false);
                
                textTheDayTimer.text = XUtility.GetTimeText(XUtility.GetLeftTime(_smashController.eggInfo.EndTimestamp * 1000));
              
                if (!_smashController.eggInfo.SilverOver)
                {
                    silverGroup.SetTheDayStyle(HasWinSilverBigPrize());
                }

                if (!_smashController.eggInfo.GoldOver)
                {
                    goldGroup.SetTheDayStyle(HasWinGoldBigPrize());
                }
            }
            else
            {
                transformTheDayState.gameObject.SetActive(false);
                transformWaitState.gameObject.SetActive(true);

                var leftTime = XUtility.GetLeftTime(_smashController.eggInfo.StartTimestamp * 1000);

                if (leftTime > XUtility.SecondsOfOneDay)
                {
                    transformWaitDayStyle.gameObject.SetActive(true);
                    transformWaitTimeStyle.gameObject.SetActive(false);
                    var day  = Mathf.Ceil(leftTime/XUtility.SecondsOfOneDay);
                    textWaitDay.text = day.ToString();
                }
                else
                {
                    transformWaitDayStyle.gameObject.SetActive(false);
                    transformWaitTimeStyle.gameObject.SetActive(true);
                    textWaitTimer.text = XUtility.GetTimeText(XUtility.GetLeftTime(_smashController.eggInfo.StartTimestamp * 1000));
                }
            }
        }

        public bool HasWinSilverBigPrize()
        {
            var eggInfo = _smashController.eggInfo;
           
            for (var i = 0; i < eggInfo.SilverEggs.Count; i++)
            {
                if (eggInfo.SilverEggs[i].Open && eggInfo.SilverEggs[i].Win)
                {
                    return true;
                }
            }

            return false;
        }

        public bool HasWinGoldBigPrize()
        {
            var eggInfo = _smashController.eggInfo;
           
            for (var i = 0; i < eggInfo.GoldEggs.Count; i++)
            {
                if (eggInfo.GoldEggs[i].Open && eggInfo.GoldEggs[i].Win)
                {
                    return true;
                }
            }
            
            return false;
        }
        
        public void Refresh()
        {
            var eggInfo = _smashController.eggInfo;

            if (eggInfo == null) { return; }

            if (eggInfo.SilverOver)
            {
                silverGroup.SetFinishStyle();
            }
            else
            {
                silverGroup.SetWaitingStyle();
                silverGroup.SetProgress(eggInfo.SilverHammer);
                silverGroup.SetItems(eggInfo.SilverTotalFinalReward?.Items);
            }

            if (eggInfo.GoldOver)
            {
                goldGroup.SetFinishStyle();
            }
            else
            {
                goldGroup.SetWaitingStyle();
                goldGroup.SetProgress(eggInfo.GoldHammer);
                goldGroup.SetItems(eggInfo.GoldTotalFinalReward?.Items);
            }

            RefreshTime();
        }
    }
}