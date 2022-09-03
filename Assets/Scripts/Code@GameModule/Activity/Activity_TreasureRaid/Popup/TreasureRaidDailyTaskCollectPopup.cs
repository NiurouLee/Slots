using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidDailyTaskCollect")]
    public class TreasureRaidDailyTaskCollectPopup : Popup
    {
        [ComponentBinder("Root/MainGroup/RewardContent")]
        private Transform _rewardGroup;

        [ComponentBinder("Root/MainGroup/TitleGroup/NumberGroup")]
        private Transform _numberGroup;

        [ComponentBinder("Root/MainGroup/BottonmGroup/Button")]
        private Button _collectButton;

        private CurrencyCoinView _currencyCoinView;
        private Action _collectFinishCallback;

        private Reward _reward;
        public TreasureRaidDailyTaskCollectPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _collectButton.onClick.AddListener(OnCollectButtonClicked);
            AdaptScaleTransform(transform.Find("Root"), ViewResolution.designSize);
        }

        public override void AdaptScaleTransform(Transform transformToScale, Vector2 preferSize)
        {
            var viewSize = ViewResolution.referenceResolutionLandscape;
            if (viewSize.x < preferSize.x)
            {
                var scale = viewSize.x / preferSize.x;
                transformToScale.localScale =  new Vector3(scale, scale, scale);
            }
        }
        
        private void SetBtnState(bool interactable)
        {
            _collectButton.interactable = interactable;
        }

        public void InitRewardContent(Reward reward, uint taskIndex)
        {
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventTreasureraidMission, ("OperationId", taskIndex.ToString()));
            for (int i = 0; i < _numberGroup.childCount; i++)
            {
                _numberGroup.GetChild(i).gameObject.SetActive(i + 1 == taskIndex);
            }
            //转换一下数据
            _reward = reward;
            XItemUtility.InitItemsUI(_rewardGroup, _reward.Items, _rewardGroup.Find("RewardCell"));
        }

        private async void OnCollectButtonClicked()
        {
            SetBtnState(false);
            SoundController.PlayButtonClick();
            Item coinItem = XItemUtility.GetCoinItem(_reward);
            if (coinItem != null)
            {
                _currencyCoinView = await AddCurrencyCoinView();
                _currencyCoinView.Show();
                await XUtility.FlyCoins(_collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Treasure_Level_Completed"), _currencyCoinView);
            }
            RemoveChild(_currencyCoinView);
            Item emeraldItem = XItemUtility.GetItem(_reward.Items, Item.Types.Type.Emerald);
            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, "TreasureRaidFinalReward"));
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
}