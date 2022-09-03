using System;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;
using Types = DragonU3DSDK.Network.API.ILProtocol.Item.Types;

namespace GameModule
{
    [AssetAddress("UIRushPassIsPaidCollect", "UIRushPassIsPaidCollectV")]
    public class RushPassIsPaidCollectPopup : Popup<RushPassIsPaidCollectPopupViewController>
    {
        [ComponentBinder("Root/MainGroup/IntegralGroup")]
        public RectTransform integralGroup;

        [ComponentBinder("Root/MainGroup/Scroll View/Viewport/Content/MainImg")]
        public RectTransform mainImg;

        [ComponentBinder("Root/BottomGroup/ContinueButton")]
        public Button continueButton;

        [ComponentBinder("Root/MainGroup/Scroll View/Viewport/Content/MainImg/GuideCell")]
        public Transform guideCell;

        [ComponentBinder("Root/MainGroup/IntegralGroup/IntegralText")]
        public Text textCoins;

        [ComponentBinder("Root/MainGroup/IntegralGroup")]
        public RectTransform intergralGroupTrf;

        [ComponentBinder("Root/MainGroup/Scroll View")]
        public RectTransform scrollViewTrf;

        public RushPassIsPaidCollectPopup(string address)
            : base(address)
        {
        }

        public void InitializeItems(RepeatedField<Reward> inRewards,int propsNum)
        {
            if (propsNum<=0)
            {
                intergralGroupTrf.localPosition=new Vector3(intergralGroupTrf.localPosition.x,0,0);
                scrollViewTrf.gameObject.SetActive(false);
                return;
            }
            
            XItemUtility.InitItemsUI(mainImg, inRewards, mainImg.Find("GuideCell"), GetItemDescribe);
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
        }

        private string GetFormatTime(uint time)
        {
            uint h = time / 60;
            uint m = time % 60;
            if (h > 0 && m > 0)
            {
                return h + "H" + m + "M";
            }

            if (h > 0)
            {
                return h + "H";
            }

            if (m > 0)
            {
                return m + "MINS";
            }

            return "";
        }

        //金币需要单独展示
        public void ShowCoins(string s)
        {
            textCoins.text = s;
        }
    }

    public class RushPassIsPaidCollectPopupViewController : ViewController<RushPassIsPaidCollectPopup>
    {
        private Activity_LevelRushRushPass _activity;

        /// <summary>
        /// 金币数量
        /// </summary>
        private ulong _coins = 0;

        private ulong _emerald = 0;

        protected override void SubscribeEvents()
        {
            view.continueButton.onClick.AddListener(OnContinueButtonClicked);
            SubscribeEvent<EventActivityExpire>(OnRushPassExpire);
            SubscribeEvent<EventLevelRushStateChanged>(OnLevelRushExpire);
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
        public override void OnViewEnabled()
        {
            base.OnViewEnabled();

            _activity =
                Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
            ulong coinsNum = 0;
            ulong emerald = 0;
            var rewards = _activity.GetCanCollectReward();
            // List<Item> noCoinsItems = new List<Item>();

            var propsNum = 0;
            var skipList = new List<Item.Types.Type>() {Item.Types.Type.Coin};
            var noCoinsRewards = XItemUtility.GetRewards(rewards, skipList);

            for (int i = 0; i < rewards.Count; i++)
            {
                var reward = rewards[i];
                for (int j = 0; j < reward.Items.count; j++)
                {
                    var item = reward.Items[j];
                    if (item != null)
                    {
                        if (item.Type == Types.Type.Coin)
                        {
                            coinsNum += item.Coin.Amount;
                           
                        }
                        else if (item.Type==Types.Type.Emerald)
                            {
                                emerald += item.Emerald.Amount;
                            }
                            propsNum += 1;
                    }
                }
            }

            _emerald = emerald;
            _coins = coinsNum;
            view.InitializeItems(noCoinsRewards,propsNum);
            view.ShowCoins(_coins.GetCommaFormat());
        }


        public async void OnContinueButtonClicked()
        {
            
            view.continueButton.interactable = false;
            
            var collectSuccess = await _activity.Collect();
            if (collectSuccess)
            {

                var data = _activity.collectResult;
                var items = data.Response.PaidRewardGot.Items;

                if (data.Response.FreeRewardGot != null && data.Response.FreeRewardGot.Items != null &&
                    data.Response.FreeRewardGot.Items.Count > 0) 
                {
                    for (int i = 0; i < data.Response.FreeRewardGot.Items.count; i++)
                    {
                        items.Add(data.Response.FreeRewardGot.Items[i]);
                    }
                }

                
                if (_emerald>0)
                {
                    EventBus.Dispatch(new EventEmeraldBalanceUpdate(_emerald,"Activity_RushPass_Collect"));
                }
                
                if (_coins > 0)
                {
                    await XUtility.FlyCoins(view.continueButton.transform,
                        new EventBalanceUpdate(_coins, "Activity_RushPass_Collect"));
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