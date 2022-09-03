using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidFinalReward")]
    public class TreasureRaidCollectRewardPopup : Popup<TreasureRaidCollectRewardPopupController>
    {
        [ComponentBinder("Root/MainGroup/RewardGroup")]
        private Transform _rewardGroup;
        
        [ComponentBinder("Root/MainGroup/ConfirmButton/ClickMask")]
        private Transform btnMask;

        [ComponentBinder("Root/MainGroup/ConfirmButton")]
        private Button _collectButton;

        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        private Text coinsCountText;

        private CurrencyCoinView _currencyCoinView;
        private Action _collectFinishCallback;

        private RepeatedField<Reward> _rewards;
        public TreasureRaidCollectRewardPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _collectButton.onClick.AddListener(OnCollectButtonClicked);
            closeButton.gameObject.SetActive(false);
        }

        private void SetBtnState(bool interactable)
        {
            _collectButton.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        private void RefreshRewardContent()
        {
            coinsCountText.SetText(XItemUtility.GetItem(_rewards[0].Items, Item.Types.Type.Coin).Coin.Amount.GetCommaFormat());
            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.Coin);
            XItemUtility.InitItemsUI(_rewardGroup, _rewards[0].Items, _rewardGroup.Find("TreasureRaidGameRewardCell"), null, "StandardType", skipList);
        }

        public void InitRewardContent(RepeatedField<Reward> roundCompleteRewards)
        {
            //转换一下数据
            _rewards = roundCompleteRewards;
            RefreshRewardContent();
        }

        public async void OnCollectButtonClicked()
        {
            SetBtnState(false);
            SoundController.PlayButtonClick();
            Item coinItem = XItemUtility.GetCoinItem(_rewards[0]);
            if (coinItem != null)
            {
                _currencyCoinView = await AddCurrencyCoinView();
                _currencyCoinView.Show();
                await XUtility.FlyCoins(_collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Treasure_Level_Completed"), _currencyCoinView);
            }
            RemoveChild(_currencyCoinView);
            Item emeraldItem = XItemUtility.GetItem(_rewards[0].Items, Item.Types.Type.Emerald);
            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, "TreasureRaidFinalReward"));
            }
            ItemSettleHelper.SettleItems(_rewards[0].Items, async () =>
            {
                _collectFinishCallback?.Invoke();
            });
            Close();
            // EventBus.Dispatch(new EventRefreshUserProfile());
        }

        public void ShowRewardCollect(Action finishCallback)
        {
            _collectFinishCallback = finishCallback;
        }
    }

    public class TreasureRaidCollectRewardPopupController : ViewController<TreasureRaidCollectRewardPopup>
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