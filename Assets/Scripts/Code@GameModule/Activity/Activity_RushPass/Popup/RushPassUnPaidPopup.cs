using System.ComponentModel.Design;
using System.Collections.Generic;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIRushPassUnPaidCollect", "UIRushPassUnPaidCollectV")]
    public class RushPassUnPaidCollectPopup : Popup<RushPassUnPaidCollectPopupViewController>
    {
        [ComponentBinder("Root/MainGroup/FreeItems")]
        public RectTransform freeItemsTrf;

        [ComponentBinder("Root/MainGroup/Paid/Scroll View/Viewport/Content/PaidItems")]
        public Transform paidItemsTrf;


        [ComponentBinder("Root/RushPassGrop/RushPassBtn")]
        public Button rushPassBtn;

        [ComponentBinder("Root/RushPassGrop/PurchaseBtn")]
        public Button purchaseBtn;

        [ComponentBinder("Root/CollectRewardsBtn")]
        public Button collectRewardsBtn;

        [ComponentBinder("Root/RushPassGrop/RushPassBtn/TextPrice")]
        public Text textPrice;

        public RushPassUnPaidCollectPopup(string address)
            : base(address)
        {
        }

        /// <summary>
        /// 初始化items 
        /// </summary>
        /// <param name="freeItems">免费的items</param>
        /// <param name="paidRewards">付费的items</param>
        public void InitializeItems(RepeatedField<Reward> freeRewards, RepeatedField<Reward> paidRewards)
        {
            XItemUtility.InitItemsUI(freeItemsTrf,freeRewards,freeItemsTrf.Find("GuideCell"),GetItemDescribe);
            
            XItemUtility.InitItemsUI(paidItemsTrf,paidRewards,paidItemsTrf.Find("GuideCell"),GetItemDescribe);
            
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

    public class RushPassUnPaidCollectPopupViewController : ViewController<RushPassUnPaidCollectPopup>
    {
        private Activity_LevelRushRushPass _activity;
        private ShopItemConfig shopItemConfig;

        private RepeatedField<Reward> freeRewards;
        private RepeatedField<Reward> paidRewards;

        public override void OnViewEnabled()
        {
            base.OnViewEnabled();
            _activity =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
            shopItemConfig = _activity.ShopItemConfig;
            view.textPrice.text = shopItemConfig.Price.ToString();
            freeRewards = _activity.GetCanCollectReward();
            paidRewards = _activity.GetPaidRewards((int) _activity.GetFreeCanCollectDay());
            view.InitializeItems(freeRewards, paidRewards);
        }

        protected override void SubscribeEvents()
        {
            view.rushPassBtn.onClick.AddListener(OnRushPassBtnClicked);
            view.purchaseBtn.onClick.AddListener(OnPurchaseBtnClicked);
            view.collectRewardsBtn.onClick.AddListener(OnCollectRewardsBtnClicked);
            SubscribeEvent<EventRushPassPaidFinish>(PurchaseSuccess);
            SubscribeEvent<EventActivityExpire>(OnRushPassExpire);
            SubscribeEvent<EventLevelRushStateChanged>(OnLevelRushExpire);
            SubscribeEvent<EventRushPassPaidFail>(PurchaseFail);
        }

      

        private void OnLevelRushExpire(EventLevelRushStateChanged obj)
        {
            var levelRushIsEnable = Client.Get<LevelRushController>().IsLevelRushEnabled();
            if (!levelRushIsEnable)
            {
                view.Close();
            }
        }
        private void OnRushPassExpire(EventActivityExpire obj)
        {
            if (obj.activityType==ActivityType.RushPass)
            {
                view.Close();
            }
        }
        /// <summary>
        /// 购买成功
        /// </summary>
        /// <param name="obj"></param>
        private void PurchaseSuccess(EventRushPassPaidFinish obj)
        { 
            view.ResetCloseAction();
            view.Close();
        }

        /// <summary>
        /// 购买失败
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PurchaseFail(EventRushPassPaidFail obj)
        {
            OnCollectRewardsBtnClicked();
        }


        /// <summary>
        /// 购买
        /// </summary>
        public void OnRushPassBtnClicked()
        {   
           
            _activity.SetPayBiInfo("2");
            Client.Get<IapController>().BuyProduct(shopItemConfig);
        }

        /// <summary>
        /// 通用物品弹板
        /// </summary>
        public async void OnPurchaseBtnClicked()
        {
            SoundController.PlayButtonClick();
            var purchaseBenefitsView = await PopupStack.ShowPopup<PurchaseBenefitsView>();
            purchaseBenefitsView.SetUpBenefitsView(shopItemConfig.SubItemList);
        }


        /// <summary>
        /// 收集
        /// </summary>
        public void OnCollectRewardsBtnClicked()
        {
            Collect();
            view.rushPassBtn.interactable = false;
            view.collectRewardsBtn.interactable = false;
            view.purchaseBtn.interactable = false;
        }


        private async void Collect()
        {
            var collectSuccess = await _activity.Collect();
            long coins = 0;
            long emerald = 0;
            if (collectSuccess)
            {
                var data = _activity.collectResult;
                var items = data.Response.FreeRewardGot.Items;
                    for (int j = 0; j < items.count; j++)
                    {
                        var item = items[j];
                        if (item.Type == Item.Types.Type.Coin)
                        {
                            coins = (long) item.Coin.Amount;
                            
                        }
                        else if(item.Type==Item.Types.Type.Emerald)
                        {
                            emerald += item.Emerald.Amount;
                        }
                    }
                    
                    
                    if (emerald>0)
                    {
                        EventBus.Dispatch(new EventEmeraldBalanceUpdate(emerald,"Activity_RushPass_Collect"));
                    }  
                    
                if (coins > 0)
                {
                    await XUtility.FlyCoins(view.collectRewardsBtn.transform,
                        new EventBalanceUpdate(coins, "Activity_RushPass_Collect"));
                    view.Close();
                    
                    ItemSettleHelper.SettleItems(items);
                }
                else
                {
                    view.Close();
                    ItemSettleHelper.SettleItems(items);
                }
            }
            else
            {
                view.ResetCloseAction();
                view.Close();
            }
        }
        
    
    }
}