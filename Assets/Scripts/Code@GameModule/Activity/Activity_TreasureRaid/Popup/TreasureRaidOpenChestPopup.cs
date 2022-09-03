using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidOpenBox")]
    public class TreasureRaidOpenChestPopup : Popup<TreasureRaidOpenChestPopupController>
    {
        [ComponentBinder("Root/MainGroup/RewardGroup")]
        private Transform _rewardGroup;
        
        [ComponentBinder("Root/MainGroup/ConfirmButton")]
        private Button _collectButton;

        [ComponentBinder("Root/MainGroup/ConfirmButton/ClickMask")]
        private Transform btnMask;

        private CurrencyCoinView _currencyCoinView;
        private Action _collectFinishCallback;

        private SMonopolyOpenGiftBox sMonopolyOpenGiftBox;
        private MonopolyGiftBox _giftBox;

        private Reward _reward;
        public TreasureRaidOpenChestPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _collectButton.onClick.AddListener(OnCollectButtonClicked);
            closeButton.gameObject.SetActive(false);
        }

        public void InitRewardContent(SMonopolyOpenGiftBox sMonopolyOpenGiftBox, MonopolyGiftBox monopolyGiftBox)
        {
            this.sMonopolyOpenGiftBox = sMonopolyOpenGiftBox;
            _giftBox = monopolyGiftBox;
            //转换一下数据
            _reward = this.sMonopolyOpenGiftBox.Reward;
            XItemUtility.InitItemsUI(_rewardGroup, _reward.Items, _rewardGroup.Find("TreasureRaidGameRewardCell"), null, "SpacingType");
        }

        public async void OnCollectButtonClicked()
        {
            SoundController.PlayButtonClick();
            _collectButton.interactable = false;
            btnMask.gameObject.SetActive(true);
            Item coinItem = XItemUtility.GetCoinItem(_reward);
            if (coinItem != null)
            {
                _currencyCoinView = await AddCurrencyCoinView();
                _currencyCoinView.Show();
                await XUtility.FlyCoins(_collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Treasure_Open_Chest"), _currencyCoinView);
            }
            RemoveChild(_currencyCoinView);

            Item emeraldItem = XItemUtility.GetItem(_reward.Items, Item.Types.Type.Emerald);
            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, "TreasureRaidOpenChest"));
            }
            ItemSettleHelper.SettleItems(_reward.Items, async () =>
            {
                _collectFinishCallback?.Invoke();
            });
            Close();
        }

        public void ShowRewardCollect(Action finishCallback)
        {
            _collectFinishCallback = finishCallback;
        }
    }

    public class TreasureRaidOpenChestPopupController : ViewController<TreasureRaidOpenChestPopup>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventTreasureRaidOnExpire>(ActivityOnExpire);
        }

        private void ActivityOnExpire(EventTreasureRaidOnExpire obj)
        {
            view.Close();
        }
    }
}