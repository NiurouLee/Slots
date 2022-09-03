using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidALevelFinishReward")]
    public class TreasureRaidCollectFinalRewardPopup : Popup<TreasureRaidCollectFinalRewardPopupController>
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
        public TreasureRaidCollectFinalRewardPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _collectButton.onClick.AddListener(OnCollectButtonClicked);
            closeButton.gameObject.SetActive(false);
        }

        private void RefreshRewardContent()
        {
            coinsCountText.SetText(XItemUtility.GetItem(_rewards[0].Items, Item.Types.Type.Coin).Coin.Amount.GetCommaFormat());
            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.Coin);
            XItemUtility.InitItemsUI(_rewardGroup, _rewards[0].Items, _rewardGroup.Find("TreasureRaidGameRewardCell"), null, "StandardType", skipList);
        }

        public void InitRewardContent(RepeatedField<Reward> roundListCompleteRewards)
        {
            //转换一下数据
            _rewards = roundListCompleteRewards;
            RefreshRewardContent();
        }

        private void SetBtnState(bool interactable)
        {
            _collectButton.interactable = interactable;
            btnMask.gameObject.SetActive(!interactable);
        }

        public async void OnCollectButtonClicked()
        {
            // BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCollectionMagicHatComplete);
            SetBtnState(false);
            SoundController.PlayButtonClick();
            Item coinItem = XItemUtility.GetCoinItem(_rewards[0]);
            if (coinItem != null)
            {
                _currencyCoinView = await AddCurrencyCoinView();
                _currencyCoinView.Show();
                await XUtility.FlyCoins(_collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Treasure_AllLevel_Completed"), _currencyCoinView);
            }
            RemoveChild(_currencyCoinView);

            Item emeraldItem = XItemUtility.GetItem(_rewards[0].Items, Item.Types.Type.Emerald);
            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, "TreasureRaidALevelFinishReward"));
            }
            
            ItemSettleHelper.SettleItems(_rewards[0].Items, () =>
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

    public class TreasureRaidCollectFinalRewardPopupController : ViewController<TreasureRaidCollectFinalRewardPopup>
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