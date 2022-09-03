using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UITreasureRaidPuzzleCollect")]
    public class TreasureRaidPuzzleCollectPopup : Popup<TreasureRaidPuzzleCollectPopupController>
    {
        [ComponentBinder("Root/MainGroup/__Collect1")]
        private Transform normalRewardGroup;
        [ComponentBinder("Root/MainGroup/__Collect2")]
        private Transform finalRewardGroup;

        [ComponentBinder("Root/MainGroup/RewardContent")]
        private Transform _rewardGroup;
        
        [ComponentBinder("Root/MainGroup/ButtonGroup/Button")]
        private Button _collectButton;

        [ComponentBinder("Root/MainGroup/CoinGroup/__NumberText")]
        private Text coinsCountText;

        [ComponentBinder("Root/MainGroup/CoinGroup")]
        private Transform coinGroup;

        private CurrencyCoinView _currencyCoinView;
        private Action<SGetMonopolyPuzzleListInfo> _collectFinishCallback;

        private Reward _reward;

        private int currentPageIndex;

        public TreasureRaidPuzzleCollectPopup(string address)
            : base(address)
        {
            
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            _collectButton.onClick.AddListener(OnCollectButtonClicked);
        }

        private void SetBtnState(bool interactable)
        {
            _collectButton.interactable = interactable;
        }

        private void RefreshRewardContent()
        {
            var coinItem = XItemUtility.GetItem(_reward.Items, Item.Types.Type.Coin);
            if (coinItem != null)
            {
                coinsCountText.SetText(coinItem.Coin.Amount.GetCommaFormat());
            }
            else
            {
                coinGroup.gameObject.SetActive(false);
            }
            var skipList = new List<Item.Types.Type>();
            skipList.Add(Item.Types.Type.Coin);
            if ((_reward.Items.Count > 0 && coinItem == null) || (coinItem != null && _reward.Items.Count > 1))
            {
                _rewardGroup.gameObject.SetActive(true);
                XItemUtility.InitItemsUI(_rewardGroup, _reward.Items, _rewardGroup.Find("RewardCell"), null, "StandardType", skipList);
            }
            else
            {
                _rewardGroup.gameObject.SetActive(false);
            }
        }

        public void InitRewardContent(Reward reward, int pageIndex)
        {
            //转换一下数据
            _reward = reward;
            RefreshRewardContent();
            normalRewardGroup.gameObject.SetActive(pageIndex != 6);
            finalRewardGroup.gameObject.SetActive(pageIndex == 6);
            currentPageIndex = pageIndex;
        }

        public async void OnCollectButtonClicked()
        {
            SetBtnState(false);
            SoundController.PlayButtonClick();
            var activityTreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activityTreasureRaid == null)
            {
                Close();
                return;
            }

            var response = await activityTreasureRaid.SendCollectMonopolyPuzzleReward();
            BiManagerGameModule.Instance.SendGameEvent(
                BiEventFortuneX.Types.GameEventType.GameEventTreasureraidWinpuzzle,
                ("OperationId", $"{currentPageIndex}"));
            _reward = response.PuzzleFinishedReward;
            // response.GetMonopolyPuzzleListInfo
            Item coinItem = XItemUtility.GetCoinItem(_reward);
            if (coinItem != null)
            {
                _currencyCoinView = await AddCurrencyCoinView();
                _currencyCoinView.Show();
                await XUtility.FlyCoins(_collectButton.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Treasure_Puzzle_Reward"), _currencyCoinView);
            }
            RemoveChild(_currencyCoinView);
            Item emeraldItem = XItemUtility.GetItem(_reward.Items, Item.Types.Type.Emerald);
            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, "Activity_Treasure_Puzzle_Reward"));
            }
            ItemSettleHelper.SettleItems(_reward.Items, async () =>
            {
                _collectFinishCallback?.Invoke(response.GetMonopolyPuzzleListInfo);
            });
            Close();
        }

        public void ShowRewardCollect(Action<SGetMonopolyPuzzleListInfo> finishCallback)
        {
            _collectFinishCallback = finishCallback;
        }
    }

    public class TreasureRaidPuzzleCollectPopupController : ViewController<TreasureRaidPuzzleCollectPopup>
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