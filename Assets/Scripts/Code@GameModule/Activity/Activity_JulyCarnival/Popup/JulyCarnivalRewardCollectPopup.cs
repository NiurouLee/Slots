using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using UnityEngine.UI;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    [AssetAddress("UIIndependenceDayReward", "UIIndependenceDayRewardV")]
    public class JulyCarnivalRewardCollectPopup : Popup<JulyCarnivalRewardCollectPopupController>
    {
#region Free
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup")]
        private Transform freeRewardGroup;

        
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/FreeawardGroup")]
        private ScrollRect freeRewardRect;
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/FreeawardGroup/Viewport/Content")]
        private Transform freeRewardCollectGroup;

        
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/ValentineGroup")]
        private ScrollRect freePayRewardRect;
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/ValentineGroup/Viewport/Content")]
        private Transform freeRewardPayCollectGroup;

        
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/SpecialRewardLockStateBottomGroup/PriceButton")]
        private Button buyBtn;
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/SpecialRewardLockStateBottomGroup/PriceButton/PriceText")]
        private Text priceText;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/SpecialRewardLockStateBottomGroup/ExtraContentsButton")]
        private Button benefitBtn;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/SpecialRewardLockStateBottomGroup/CollectButton")]
        private Button freeCollectBtn;

        
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/FreeawardGroup/PreviousButton")]
        private Button freePreviousBtn;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/FreeawardGroup/PreviousButton")]
        private void OnBtnFreePreviousClick()
        {
            freeRewardRect.horizontalNormalizedPosition = 0;
        }

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/FreeawardGroup/NextButton")]
        private Button freeNextBtn;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/FreeawardGroup/NextButton")]
        private void OnBtnFreeNextClick()
        {
            freeRewardRect.horizontalNormalizedPosition = 1;
        }
        
        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/ValentineGroup/PreviousButton")]
        private Button freePayPreviousBtn;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/ValentineGroup/PreviousButton")]
        private void OnBtnFreePayPreviousClick()
        {
            freePayRewardRect.horizontalNormalizedPosition = 0;
        }

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/ValentineGroup/NextButton")]
        private Button freePayNextBtn;

        [ComponentBinder("Root/RewardGroup/FreeRewardGroup/ValentineGroup/NextButton")]
        private void OnBtnFreePayNextClick()
        {
            freePayRewardRect.horizontalNormalizedPosition = 1;
        }

        /// <summary>
        /// 获取未领取的免费奖励
        /// </summary>
        /// <returns></returns>
        private RepeatedField<Reward> GetFreeCurrentNormalRewards()
        {
            var data = viewController.currentData;

            var rewards = new RepeatedField<Reward>();
            for (int i = 0; i < data.IndependenceRewards.Count; i++)
            {
                var reward = data.IndependenceRewards[i];
                if (reward.NormalRewardStatus ==
                    SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus
                        .Unlocked)
                {
                    rewards.Add(reward.NormalReward);
                }
            }
            return rewards;
        }
        
        /// <summary>
        /// 获取内购后可领取的付费奖励
        /// </summary>
        /// <returns></returns>
        private RepeatedField<Reward> GetFreeCurrentSpecialRewards()
        {
            var data = viewController.currentData;

            var rewards = new RepeatedField<Reward>();
            for (int i = 0; i < data.IndependenceRewards.Count; i++)
            {
                var reward = data.IndependenceRewards[i];
                if (reward.SpecialRewardStatus !=
                    SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus
                        .Received && data.Step >= reward.Step )
                {
                    rewards.Add(reward.SpecialReward);
                }
            }
            return rewards;
        }
        
#endregion


#region Pay

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup")]
        private Transform payRewardGroup;

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/ValentineGroup")]
        private ScrollRect payRewardRect;
        
        
        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/FreeawardGroup")]
        private Transform payRewardCoinsTr;
        
        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/FreeawardGroup/TextGroup/Text")]
        private Text payRewardCoinsText;

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/ValentineGroup/Viewport/Content")]
        private Transform payRewardPayCollectGroup;

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/FreePassStateBottomGroup/CollectButton")]
        private Button payCollectBtn;
        
        
        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/ValentineGroup/PreviousButton")]
        private Button payPreviousBtn;

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/ValentineGroup/PreviousButton")]
        private void OnBtnPayPreviousClick()
        {
            payRewardRect.horizontalNormalizedPosition = 0;
        }

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/ValentineGroup/NextButton")]
        private Button payNextBtn;

        [ComponentBinder("Root/RewardGroup/SpecialRewardGroup/ValentineGroup/NextButton")]
        private void OnBtnPayNextClick()
        {
            payRewardRect.horizontalNormalizedPosition = 1;
        }

        /// <summary>
        /// 获取当前所有可领取的奖励
        /// </summary>
        /// <returns></returns>
        private RepeatedField<Reward> GetPaidCurrentRewardsToCollect()
        {
            var data = viewController.currentData;

            var rewards = new RepeatedField<Reward>();
            for (int i = 0; i < data.IndependenceRewards.Count; i++)
            {
                var reward = data.IndependenceRewards[i];
                if (reward.NormalRewardStatus ==
                    SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus
                        .Unlocked)
                {
                    rewards.Add(reward.NormalReward);
                }
                if (reward.SpecialRewardStatus == 
                    SGetIndependenceDayMainPageInfo.Types.IndependenceDayStepReward.Types.IndependenceDayRewardStatus.Unlocked )
                {
                    rewards.Add(reward.SpecialReward);
                }
            }
            return rewards;
        }
        
#endregion

        private CurrencyCoinView _currencyCoinView;

        public JulyCarnivalRewardCollectPopup(string address)
            : base(address)
        {
            // contentDesignSize = new Vector2(1465, 768);
        }
        
        protected override void OnViewSetUpped()
        {
            base.OnViewSetUpped();
            buyBtn.onClick.AddListener(OnBuyBtnClicked);
            benefitBtn.onClick.AddListener(OnBenefitBtnClicked);
            freeCollectBtn.onClick.AddListener(OnFreeCollectBtnClicked);
            payCollectBtn.onClick.AddListener(OnFreeCollectBtnClicked);
            freeRewardRect.onValueChanged.AddListener(OnFreeRewardRectValueChanged);
            freePayRewardRect.onValueChanged.AddListener(OnFreePayRewardRectValueChanged);
            payRewardRect.onValueChanged.AddListener(OnPayRewardRectValueChanged);

            freePreviousBtn.interactable = true;
            freeNextBtn.interactable = true;
            freePayPreviousBtn.interactable = true;
            freePayNextBtn.interactable = true;
            payPreviousBtn.interactable = true;
            payNextBtn.interactable = true;
        }

        private void OnPayRewardRectValueChanged(Vector2 arg0)
        {
            if (payRewardPayCollectGroup.childCount <= 6)
                return;
            if (arg0.x <= 0.1f)
            {
                payPreviousBtn.gameObject.SetActive(false);
                payNextBtn.gameObject.SetActive(true);
            }
            else if (arg0.x >= 1)
            {
                payPreviousBtn.gameObject.SetActive(true);
                payNextBtn.gameObject.SetActive(false);
            }
            else
            {
                payPreviousBtn.gameObject.SetActive(true);
                payNextBtn.gameObject.SetActive(true);
            }
        }

        private void OnFreeRewardRectValueChanged(Vector2 arg0)
        {
            if (freeRewardCollectGroup.childCount <= 6)
                return;
            if (arg0.x <= 0.1f)
            {
                freePreviousBtn.gameObject.SetActive(false);
                freeNextBtn.gameObject.SetActive(true);
            }
            else if (arg0.x >= 1)
            {
                freePreviousBtn.gameObject.SetActive(true);
                freeNextBtn.gameObject.SetActive(false);
            }
            else
            {
                freePreviousBtn.gameObject.SetActive(true);
                freeNextBtn.gameObject.SetActive(true);
            }
        }

        private void OnFreePayRewardRectValueChanged(Vector2 arg0)
        {
            if (freeRewardPayCollectGroup.childCount <= 6)
                return;
            if (arg0.x <= 0.1f)
            {
                freePayPreviousBtn.gameObject.SetActive(false);
                freePayNextBtn.gameObject.SetActive(true);
            }
            else if (arg0.x >= 1)
            {
                freePayPreviousBtn.gameObject.SetActive(true);
                freePayNextBtn.gameObject.SetActive(false);
            }
            else
            {
                freePayPreviousBtn.gameObject.SetActive(true);
                freePayNextBtn.gameObject.SetActive(true);
            }
        }

        private void SetBtnStatus(bool interactable)
        {
            buyBtn.interactable = interactable;
            benefitBtn.interactable = interactable;
            freeCollectBtn.interactable = interactable;
            payCollectBtn.interactable = interactable;
        }

        private async void OnFreeCollectBtnClicked()
        {
            SetBtnStatus(false);
            SoundController.PlayButtonClick();
            // send collect
            var activity = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.JulyCarnival) as Activity_JulyCarnival;
            if (activity == null)
                return;
            
            var response = await activity.SendCollect();
            if (response == null)
                return;
            Item coinItem = XItemUtility.GetCoinItem(response.Rewards[0]);
            var collectBtn = freeCollectBtn.gameObject.activeInHierarchy ? freeCollectBtn : payCollectBtn;
            if (coinItem != null)
            {
                _currencyCoinView = await AddCurrencyCoinView();
                _currencyCoinView.Show();
                await XUtility.FlyCoins(collectBtn.transform,
                    new EventBalanceUpdate((long) coinItem.Coin.Amount, "Activity_Independence_Collect_Reward"), _currencyCoinView);
            }
            RemoveChild(_currencyCoinView);
            Item emeraldItem = XItemUtility.GetItem(response.Rewards[0].Items, Item.Types.Type.Emerald);
            if (emeraldItem != null)
            {
                EventBus.Dispatch(new EventEmeraldBalanceUpdate(emeraldItem.Emerald.Amount, "Activity_Independence_Collect_Reward"));
            }
            ItemSettleHelper.SettleItems(response.Rewards[0].Items, async () =>
            {
                //TODO 获取独立日最新数据，然后播放动画
                var pageInfo = await activity.GetIndependenceDayMainPageInfoData();
                var popup = PopupStack.GetPopup<JulyCarnivalMainPopup>();
                popup.ShowCollectRewardAni(pageInfo);
            });
            Close();
        }

        private async void OnBenefitBtnClicked()
        {
            SetBtnStatus(false);
            ShopItemConfig shopItemConfig = viewController.currentData.PayItem;
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
            SetBtnStatus(true);
        }

        private void OnBuyBtnClicked()
        {
            SetBtnStatus(false);
            Client.Get<IapController>().BuyProduct(viewController.currentData.PayItem);
            SetBtnStatus(true);
        }

        public void RefreshUI()
        {
            var data = viewController.currentData;
            var bPaid = data.PaymentAlreadyPaid;
            freeRewardGroup.gameObject.SetActive(!bPaid);
            payRewardGroup.gameObject.SetActive(bPaid);

            if (bPaid)
            {
                ulong coins = 0;
                var rewards = GetPaidCurrentRewardsToCollect();
                for (int i = 0; i < rewards.Count; i++)
                {
                    var reward = rewards[i];
                    var coinItem = XItemUtility.GetItem(reward.Items, Item.Types.Type.Coin);
                    if (coinItem != null)
                    {
                        reward.Items.Remove(coinItem);
                        coins += coinItem.Coin.Amount;
                    }
                }

                payRewardCoinsTr.gameObject.SetActive(coins != 0);
                payRewardCoinsText.SetText(coins.GetCommaFormat());
                XItemUtility.InitItemsUI(payRewardPayCollectGroup, rewards, payRewardPayCollectGroup.Find("UIValentinesDay2022RewardCell"),
                    GetItemDescribe, "StandarType");
            }
            else
            {
                var freeRewards = GetFreeCurrentNormalRewards();
                XItemUtility.InitItemsUI(freeRewardCollectGroup, freeRewards,
                    freeRewardCollectGroup.Find("UIValentinesDay2022RewardCell"),
                    GetItemDescribe, "StandarType");
                
                var payRewards = GetFreeCurrentSpecialRewards();
                XItemUtility.InitItemsUI(freeRewardPayCollectGroup, payRewards,
                    freeRewardPayCollectGroup.Find("UIValentinesDay2022RewardCell"),
                    GetItemDescribe, "StandarType");
                
                priceText.SetText($"$ {viewController.currentData.PayItem.Price}");
            }

            if (freeRewardCollectGroup.childCount > 6)
            {
                freeRewardRect.horizontalNormalizedPosition = 0;
                freePreviousBtn.gameObject.SetActive(false);
                freeNextBtn.gameObject.SetActive(true);
            }
            else
            {
                freePreviousBtn.gameObject.SetActive(false);
                freeNextBtn.gameObject.SetActive(false);
            }
            
            if (freeRewardPayCollectGroup.childCount > 6)
            {
                freePayRewardRect.horizontalNormalizedPosition = 0;
                freePayPreviousBtn.gameObject.SetActive(false);
                freePayNextBtn.gameObject.SetActive(true);
            }
            else
            {
                freePayPreviousBtn.gameObject.SetActive(false);
                freePayNextBtn.gameObject.SetActive(false);
            }

            if (payRewardPayCollectGroup.childCount > 6)
            {
                payRewardRect.horizontalNormalizedPosition = 0;
                payPreviousBtn.gameObject.SetActive(false);
                payNextBtn.gameObject.SetActive(true);
            }
            else
            {
                payPreviousBtn.gameObject.SetActive(false);
                payNextBtn.gameObject.SetActive(false);
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
    }

    public class JulyCarnivalRewardCollectPopupController : ViewController<JulyCarnivalRewardCollectPopup>
    {
        public SGetIndependenceDayMainPageInfo currentData;

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            view.RefreshUI();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventActivityExpire>(OnEventActivityExpire);
        }

        private void OnEventActivityExpire(EventActivityExpire evt)
        {
            if (evt.activityType != ActivityType.JulyCarnival)
                return;
            view.Close();
        }

        public override void BindingView(View inView, object inExtraData, object inExtraAsyncData = null)
        {
            base.BindingView(inView, inExtraData, inExtraAsyncData);
            if (inExtraData != null)
            {
                if (inExtraData is PopupArgs args)
                {
                    currentData = args.extraArgs as SGetIndependenceDayMainPageInfo;
                }
            }

            if (currentData == null)
            {
                var activityIndependenceDay =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.JulyCarnival) as
                        Activity_JulyCarnival;
                currentData = activityIndependenceDay?.GetIndependenceDayMainPageInfo();
            }
        }

        public void SetCurrentPageInfoAndRefreshUI(SGetIndependenceDayMainPageInfo pageInfo)
        {
            currentData = pageInfo;
            view.RefreshUI();
        }
    }
}