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
    [AssetAddress("UIQuestSeasonOneRewardCollectPhaseReward")]
    public class SeasonQuestPhaseNodeRewardView : Popup
    {
        [ComponentBinder("MainGroup")] private Transform _mainGroup;

        [ComponentBinder("ContentGroup/BottomGroup/ContinueButton")]
        private Button _continueButton;

        private Reward _reward;

        public SeasonQuestPhaseNodeRewardView(string address)
            : base(address)
        {
            
        }
        protected CurrencyCoinView _currencyCoinView;
        protected Action _collectFinisihCallback;


        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _continueButton.onClick.AddListener(OnCollectButtonClicked);
        }

        public void InitRewardContent(Reward reward)
        {
            _reward = reward;
            XItemUtility.InitItemsUI(_mainGroup, reward.Items, _mainGroup.Find("CommonCell"));
        }

        public void OnCollectButtonClicked()
        {
            _continueButton.interactable = false;
            
            var seasonQuestController  = Client.Get<SeasonQuestController>();
            
            var countDown = seasonQuestController.GetQuestCountDown();
            var questIndex = seasonQuestController.GetCurrentQuestIndex();
            var seasonId = seasonQuestController.GetSeasonId();
 
            Client.Get<SeasonQuestController>().ClaimCurrentQuest(async (success) =>
            {
                _continueButton.interactable = false;
              
                if (success)
                {
                    Item coinItem = XItemUtility.GetCoinItem(_reward);
                    if (coinItem != null)
                    {
                        _currencyCoinView.Show();
                        await XUtility.FlyCoins(_continueButton.transform,
                            new EventBalanceUpdate((long) coinItem.Coin.Amount, "SeasonQuest"), _currencyCoinView);
                    }

                    ItemSettleHelper.SettleItems(_reward.Items);

                    EventBus.Dispatch(new EventRefreshUserProfile());
                
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventQuestSeasonCollect,
                        ("questNodeIndex", questIndex.ToString()),("seasonId",seasonId.ToString()),("countDown",countDown.ToString()));

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
    }
}