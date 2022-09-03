// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/12/10/16:42
// Ver : 1.0.0
// Description : QuestFaqPopUp.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIQuestCollectNormalNodeReward")]
    public class QuestNormalNodeRewardView:View
    {
        [ComponentBinder("MainGroup")]
        private Transform _mainGroup;  
        
        [ComponentBinder("ContentGroup/BottomGroup/ContinueButton")]
        private Button _continueButton;

        protected CurrencyCoinView _currencyCoinView;
        protected Action _collectFinisihCallback;

        private Reward _reward;
        private CancelableCallback cancelableCallback;
        public QuestNormalNodeRewardView(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _continueButton.onClick.AddListener(OnCollectButtonClicked);

            cancelableCallback = new CancelableCallback(() =>
            {
                SoundController.PlaySfx("Wheel_CoinsWin");
            });
          
            XUtility.WaitSeconds(1, cancelableCallback);
        }

        public void InitRewardContent(Reward reward)
        {
            _reward = reward;
            XItemUtility.InitItemsUI(_mainGroup, reward.Items, _mainGroup.Find("CommonCell"));
        }
        
        protected void SentBiCollectEvent(int taskIndex)
        {
            var eventType = (BiEventFortuneX.Types.GameEventType) Enum.Parse(
                typeof(BiEventFortuneX.Types.GameEventType), "GameEventQuestCollect" + (taskIndex + 1));
            BiManagerGameModule.Instance.SendGameEvent(eventType);
        }
        
        public void OnCollectButtonClicked()
        {
            _continueButton.interactable = false;
            
            var newBieQuestController  = Client.Get<NewBieQuestController>();
            var questIndex = (int)newBieQuestController.GetCurrentQuestIndex();
            
            Client.Get<NewBieQuestController>().ClaimCurrentQuest(async (success) =>
            {
                if (success)
                {
                    SentBiCollectEvent(questIndex/2);
                 
                    Item coinItem = XItemUtility.GetCoinItem(_reward);
                    if (coinItem != null)
                    {
                        _currencyCoinView.Show();
                        await XUtility.FlyCoins(_continueButton.transform,
                            new EventBalanceUpdate((long) coinItem.Coin.Amount, "NewBieQuest"), _currencyCoinView);
                    }

                    ItemSettleHelper.SettleItems(_reward.Items);

                    EventBus.Dispatch(new EventRefreshUserProfile());
                    
                    _collectFinisihCallback?.Invoke();
                }
                else
                {
                    _continueButton.interactable = true;
                }
            });
        }

        public void ShowRewardCollect(CurrencyCoinView currencyCoinView, Action finishCallback)
        {
            _currencyCoinView = currencyCoinView;
            _currencyCoinView.Hide();
            _collectFinisihCallback = finishCallback;
        }

        public override void Destroy()
        {
            base.Destroy();
            
            if(cancelableCallback != null)
                cancelableCallback.CancelCallback();
        }
    }
}