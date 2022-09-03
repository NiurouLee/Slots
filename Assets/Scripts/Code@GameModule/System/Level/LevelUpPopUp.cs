// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/19/21:16
// Ver : 1.0.0
// Description : LevelUpPopUp.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UILevelUpRewardH","UILevelUpRewardV")]
    public class LevelUpPopUp : Popup<LevelUpViewController>
    {
        [ComponentBinder("LevelText")]
        public Text levelText;

        [ComponentBinder("CoinRewardText")]
        public TextMeshProUGUI coinRewardText;

        [ComponentBinder("BetPropertyText")]
        public TextMeshProUGUI betPropertyText;

        [ComponentBinder("VipPropertyText")]
        public TextMeshProUGUI vipPropertyText;

        [ComponentBinder("CollectButton")]
        public Button collectButton;

        [ComponentBinder("Root/BottomGroup")]
        public Transform transformBottomGroup;

        [ComponentBinder("Root/BottomADSGroup")]
        public Transform transformBottomADSGroup;

        [ComponentBinder("Root/BottomADSGroup/CollectDoubleButton")]
        public Button collectDoubleButton;

        [ComponentBinder("Root/BottomADSGroup/CollectADSButton")]
        public Button collectADSButton;

        [ComponentBinder("Vip")]
        public Transform vip;

        [ComponentBinder("MaxBet")]
        public Transform maxBet;

        [ComponentBinder("BubbleGroup")]
        public Transform bubbleGroup;

        [ComponentBinder("RewardMultipleNumberText")]
        public TextMeshProUGUI rewardMultipleNumberText;

        public LevelUpPopUp(string address)
        : base(address)
        {

        }
    }

    public class LevelUpViewController : ViewController<LevelUpPopUp>
    {
        private LevelUpInfo _levelUpInfo;

        private eAdReward _adPlaceID = eAdReward.LevelUpRV;

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);

            var popupArgs = extraData as PopupArgs;
            if (popupArgs != null)
            {
                _levelUpInfo = popupArgs.extraArgs as LevelUpInfo;
            }
        }

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            SoundController.PlaySfx("General_Levelup-02");
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            view.collectButton.onClick.AddListener(OnCollectClicked);

            view.collectADSButton.onClick.AddListener(OnCollectADSClicked);
            view.collectDoubleButton.onClick.AddListener(OnCollectDoubleClicked);
        }

        private void SetButton()
        {
            if (ADSController.ShouldShowRV(_adPlaceID))
            {
                view.transformBottomGroup.gameObject.SetActive(false);
                view.transformBottomADSGroup.gameObject.SetActive(true);
                SetADSButtonsInteractable(true);
            }
            else
            {
                view.transformBottomGroup.gameObject.SetActive(true);
                view.transformBottomADSGroup.gameObject.SetActive(false);
                if (view.collectButton != null) { view.collectButton.interactable = true; }
            }
        }

        private void SetADSButtonsInteractable(bool interactable)
        {
            if (view.collectDoubleButton != null) { view.collectDoubleButton.interactable = interactable; }
            if (view.collectADSButton != null) { view.collectADSButton.interactable = interactable; }
        }

        protected void OnCollectDoubleClicked()
        {
            if (!ADSController.ShouldShowRV(_adPlaceID))
            {
                CommonNoticePopup.ShowCommonNoticePopUp("ad reward loading...");
                SetADSButtonsInteractable(true);
            }
            else
            {
                SetADSButtonsInteractable(false);

                long rewardCoinCount =
                    (long)XItemUtility.GetItem(_levelUpInfo.RewardItems, Item.Types.Type.Coin).Coin.Amount;

                ADSController.TryShowRewardedVideoWithTransform(
                    _adPlaceID, (ulong)rewardCoinCount, (ulong)rewardCoinCount * 2, _adPlaceID.ToString(), view.collectDoubleButton.transform,
                    OnWatchRVFinished);
            }
        }

        protected void OnWatchRVFinished(bool success, string reason)
        {
            if (success)
            {
                CollectReward(null);
            }
            else
            {
                SetButton();
            }
        }

        protected void OnCollectADSClicked()
        {
            SetADSButtonsInteractable(false);
            
            CollectReward(view.collectADSButton);
        }

        protected void OnCollectClicked()
        {
            CollectReward(view.collectButton);
        }

        protected async void CollectReward(Button collectButton)
        {
            if (collectButton != null)
            {
                collectButton.interactable = false;
                long rewardCoinCount =
                    (long) XItemUtility.GetItem(_levelUpInfo.RewardItems, Item.Types.Type.Coin).Coin.Amount;

                await XUtility.FlyCoins(collectButton.transform, new EventBalanceUpdate(rewardCoinCount, "LevelUp"));
            }

            EventBus.Dispatch(new EventUpdateExp(false));

            if (_levelUpInfo.UnlockedMachines != null && _levelUpInfo.UnlockedMachines.Count > 0)
            {
                EventBus.Dispatch(new EventLevelUpUnlockNewMachine(_levelUpInfo));
            }

            view.Close();

            var vipItem = XItemUtility.GetItem(_levelUpInfo.RewardItems, Item.Types.Type.VipPoints);

            if (vipItem != null && vipItem.VipPoints.LevelUpRewardItems != null &&
                vipItem.VipPoints.LevelUpRewardItems.Count > 0)
            {
                var closeAction = view.GetCloseAction();

                view.ResetCloseAction();

                ItemSettleHelper.SettleItem(vipItem, closeAction, "LevelUp");
            }
            else if (_levelUpInfo.ShowDeal > 0)
            {
                Client.Get<BannerController>().TriggerActiveDealOffer("LevelUp", () =>
                {
                    if (_levelUpInfo.MaxBet > 0)
                    {
                        EventBus.Dispatch(new EventMaxBetUnlocked(_levelUpInfo.MaxBet));
                    }
                });
            }
            else if (_levelUpInfo.MaxBet > 0)
            {
                EventBus.Dispatch(new EventMaxBetUnlocked(_levelUpInfo.MaxBet));
            }
        }

        public override void OnViewDidLoad()
        {
            base.OnViewDidLoad();

            view.levelText.text = _levelUpInfo.Level.ToString();

            long rewardCoinCount =
                (long)XItemUtility.GetItem(_levelUpInfo.RewardItems, Item.Types.Type.Coin).Coin.Amount;

            view.coinRewardText.text = rewardCoinCount.GetCommaFormat();

            var itemVip = XItemUtility.GetItem(_levelUpInfo.RewardItems, Item.Types.Type.VipPoints);

            if (itemVip != null)
            {
                long vipPointCount = itemVip.VipPoints.Amount;
                view.vipPropertyText.text = vipPointCount.GetCommaFormat();
            }

            if (_levelUpInfo.RewardMultiplier > 1)
            {
                view.rewardMultipleNumberText.text = "X" + _levelUpInfo.RewardMultiplier;
            }
            else
            {
                view.bubbleGroup.gameObject.SetActive(false);
            }

            if (_levelUpInfo.MaxBet > 0)
            {
                view.betPropertyText.text = _levelUpInfo.MaxBet.GetAbbreviationFormat();
            }
            else
            {
                view.maxBet.gameObject.SetActive(false);
            }

            SetButton();
        }
    }
}