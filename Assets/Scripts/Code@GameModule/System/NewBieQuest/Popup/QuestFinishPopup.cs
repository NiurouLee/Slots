// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/05/19:07
// Ver : 1.0.0
// Description : QuestFinishPopup.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    [AssetAddress("UIQuestFinishAll")]
    public class QuestFinishPopup : Popup
    {
        [ComponentBinder("MainGroup")] 
        private Transform mainGroup;

        [ComponentBinder("CollectButton")] 
        private Button collectButton;  
        
        [ComponentBinder("IconContainer")] 
        private Button iconContainer;

        protected Reward reward;
        
        public QuestFinishPopup(string address)
            : base(address)
        {
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            collectButton.onClick.AddListener(OnCollectClicked);

            var quest = Client.Get<NewBieQuestController>().GetCurrentQuest();
            
            XItemUtility.InitItemsUI(mainGroup, quest.Reward.Items, mainGroup.Find("CommonCell"));
            
            reward = quest.Reward;
        }

        protected void SentBiCollectEvent(int taskIndex)
        {
            var eventType = (BiEventFortuneX.Types.GameEventType) Enum.Parse(
                typeof(BiEventFortuneX.Types.GameEventType), "GameEventQuestCollect" + (taskIndex + 1));
            BiManagerGameModule.Instance.SendGameEvent(eventType);
        }
        
        protected void OnCollectClicked()
        {
            collectButton.interactable = false;
            SoundController.PlayButtonClick();
            
            var index = (int) Client.Get<NewBieQuestController>().GetCurrentQuestIndex();
            
            Client.Get<NewBieQuestController>().ClaimCurrentQuest(async (success) =>
            {
                if (success)
                {
                    SentBiCollectEvent(index/2);
                    
                    var currencyCoinView = await AddCurrencyCoinView();
                 
                    Item coinItem = XItemUtility.GetCoinItem(reward);
                    if (coinItem != null)
                    {
                        currencyCoinView.Show();
                        await XUtility.FlyCoins(collectButton.transform,
                            new EventBalanceUpdate((long) coinItem.Coin.Amount, "QuestFinish"), currencyCoinView);
                    }
                    
                    ItemSettleHelper.SettleItems(reward.Items);
                    
                    EventBus.Dispatch(new EventRefreshUserProfile());
                    
                    Close();
                }
                else
                {
                    collectButton.interactable = true;
                }
            });
        }
    }
}