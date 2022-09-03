using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIAmazingInTheHatStageFinishReward")]
    public class AmazingHatFinalRewardPopup : Popup<AmazingHatFinalRewardPopupController>
    {
        [ComponentBinder("Root/MainGroup/RewardGroup")]
        private Transform _rewardGroup;  
        
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        private Button _collectButton;

        protected CurrencyCoinView _currencyCoinView;
        protected Action _collectFinisihCallback;

        private Reward _reward;
        public AmazingHatFinalRewardPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _collectButton.onClick.AddListener(OnCollectButtonClicked);
        }

        public void InitRewardContent()
        {
            //转换一下数据
            var rewards = Client.Get<AmazingHatController>().Rewards;
            _reward = rewards[0];
            XItemUtility.InitItemsUI(_rewardGroup, rewards, _rewardGroup.Find("AmazingInTheHatCell"), GetItemDescribe);
            for (int i = 0; i < _reward.Items.Count; i++)
            {
                var itemUI = XItemUtility.GetItemUI(_rewardGroup, _reward.Items[i].Type);
                itemUI.SetAsLastSibling();
            }
        }
        
        private string GetItemDescribe(List<Item> items)
        {
            var type = items[0].Type;
      
            if (type == Item.Types.Type.Coin || type == Item.Types.Type.ShopCoin)
            {
                ulong totalWin = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalWin += items[i].Coin.Amount;
                }
                return totalWin.GetAbbreviationFormat();   
            }
            if (type == Item.Types.Type.Emerald || type == Item.Types.Type.ShopEmerald)
            {
                ulong totalDiamond = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalDiamond += items[i].Emerald.Amount;
                }
                return totalDiamond.GetAbbreviationFormat();   
            }

            
            if (type == Item.Types.Type.LevelUpBurst)
            {
                uint totalTime = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalTime += items[i].LevelUpBurst.Amount;
                }
                return GetFormatTime(totalTime);
            }
            if (type == Item.Types.Type.DoubleExp)
            {
                uint totalTime = 0;
                for (int i = 0; i < items.Count; i++)
                {
                    totalTime += items[i].DoubleExp.Amount;
                }
                return GetFormatTime(totalTime);
            }

            if (type == Item.Types.Type.CardPackage)
            {
                return "+" + items.Count;
            }

            if (type == Item.Types.Type.VipPoints)
            {
                uint amount = 0;
             
                for (int i = 0; i < items.Count; i++)
                {
                    amount += items[i].VipPoints.Amount;
                }

                return "+" + amount;
            }
            
            return XItemUtility.GetItemRewardSimplyDescText(items[0]);
            
            return string.Empty;
        }
        
        private string GetFormatTime(uint time)
        {
            uint h = time / 60;
            uint m = time % 60;
            if (h > 0 && m > 0)
            {
                return h + "H" + m + "M";
            }
            if (h>0)
            {
                return h + "H";
            }
            if (m>0)
            {
                return m + "MINS";
            }
            return "";
        }

        public async void OnCollectButtonClicked()
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatComplete);
            SoundController.PlayButtonClick();
            _collectButton.interactable = false;
            await Client.Get<AmazingHatController>().HatGameCollectHat(async (success, sHatGameCollectRewards) =>
            {
                if (transform == null)
                    return;

                _collectButton.interactable = false;
              
                if (success)
                {
                    Item coinItem = XItemUtility.GetCoinItem(_reward);
                    if (coinItem != null)
                    {
                        _currencyCoinView = await AddCurrencyCoinView();
                        _currencyCoinView.Show();
                        await XUtility.FlyCoins(_collectButton.transform,
                            new EventBalanceUpdate((long) coinItem.Coin.Amount, "AmazingHatReward"), _currencyCoinView);
                    }
                    RemoveChild(_currencyCoinView);

                    ItemSettleHelper.SettleItems(sHatGameCollectRewards.Rewards[0].Items, () =>
                    {
                        _collectFinisihCallback?.Invoke();
                    });
                    Close();
                    EventBus.Dispatch(new EventRefreshUserProfile());
                }
                else
                {
                    _collectButton.interactable = true;
                }
            });
        }

        public void ShowRewardCollect(Action finishCallback)
        {
            _collectFinisihCallback = finishCallback;
        }
    }

    public class AmazingHatFinalRewardPopupController : ViewController<AmazingHatFinalRewardPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventAlbumSeasonEnd>(OnEventAlbumSeasonEnd);
        }

        private void OnEventAlbumSeasonEnd(EventAlbumSeasonEnd evt)
        {
            view.Close();
        }
    }
}