// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/23/18:42
// Ver : 1.0.0
// Description : DailyBonusDayRewardPopUp.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class DailyBonusBoxRewardPopup : Popup
    {
        [ComponentBinder("RewardGroup")] 
        public Transform rewardGroup;

        [ComponentBinder("ConfirmButton")] 
        public Button confirmButton;  
        
        [ComponentBinder("DayText")] 
        public Text dayText;

        private Reward _reward;

        private int _stage = 0;
        public DailyBonusBoxRewardPopup(string address)
            : base(address)
        {
        }

        public void InitPopupContent(MonthReward monthReward, int stage)
        {
            InitRewardContent(monthReward.Reward);
            
            _reward = monthReward.Reward;
            
            if(dayText)
                dayText.text = monthReward.Step.ToString();
            
            animator.Play("Open0" + (stage + 1));
        }
        
        public void InitRewardContent(Reward reward)
        {
            XItemUtility.InitItemsUI(rewardGroup, reward.Items, rewardGroup.Find("DailyBonusCell"));
        }
  
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            confirmButton.onClick.AddListener(OnCollectClicked);
        }
        
        protected async void OnCollectClicked()
        {
            confirmButton.interactable = false;

            var stage = Client.Get<DailyBonusController>().GetMonthStage();
            var loginDay = Client.Get<DailyBonusController>().GetMonthStep();

            var sCollectDailyBonus = await Client.Get<DailyBonusController>().CollectMonthReward();

            if (sCollectDailyBonus != null)
            {
                var items = XItemUtility.GetItems(sCollectDailyBonus.Rewards);
                Item coinItem = XItemUtility.GetCoinItem(items);

                if (coinItem != null)
                {
                    await XUtility.FlyCoins(confirmButton.transform,
                        new EventBalanceUpdate((long) coinItem.Coin.Amount, "DailyBonusMonthReward"));
                }

                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventDailyBonusStage,
                    ("stage", stage.ToString()), ("totalLoginDay", loginDay.ToString()));

                ItemSettleHelper.SettleItems(items);

                EventBus.Dispatch(new EventRefreshUserProfile());
            }

            Close();
        }
    }
    
}