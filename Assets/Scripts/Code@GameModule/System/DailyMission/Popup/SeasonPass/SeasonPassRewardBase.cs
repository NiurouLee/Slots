//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-02 12:15
//  Ver : 1.0.0
//  Description : SeasonPassRewardBase.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SeasonPassRewardBase:Popup<SeasonPassRewardViewController>
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        public Button btnCollect;

        [ComponentBinder("Root/RewardGroup")]
        protected RectTransform rewardGroup;
        
        [ComponentBinder("Root/RewardGroup/Viewport/Content")]
        protected Transform _transScrollViewContent;
        
        protected bool isCollectAll;
        protected ulong totalWin;
        protected bool _isGolden;
        protected MissionPassReward _missionPassReward;

        public SeasonPassRewardBase(string assetAddress)
            : base(assetAddress)
        {
            
        }

        public virtual void InitRewards(RepeatedField<Reward> rewards = null, bool isGolden = false, MissionPassReward missionPassReward = null)
        {
            _isGolden = isGolden;
            _missionPassReward = missionPassReward;
        }

        protected virtual int InitRewardContent(RepeatedField<Reward> rewards)
        {
            var template = _transScrollViewContent.transform.Find("DailyMissionRewardCell");
            SetTransformActive(template,false);
            return XItemUtility.InitItemsUI(_transScrollViewContent, rewards, template, GetItemDescribe,"BGBlueType");
        }

        protected override void BindingComponent()
        {
            base.BindingComponent();
            btnCollect.onClick.AddListener(OnBtnCollectClicked);
        }

        private void OnBtnCollectClicked()
        {
            btnCollect.interactable = false;
            if (isCollectAll)
            {
                var freeCount = 0;
                var freeLimitedCount = 0;
                var Level = Client.Get<SeasonPassController>().Level;
                var freeMissionPassRewards = Client.Get<SeasonPassController>().FreeMissionPassRewards;
                var listPaidKeys = freeMissionPassRewards.Keys.ToList();
                for (int i = 0; i < listPaidKeys.Count; i++)
                {
                    var rewards = freeMissionPassRewards[listPaidKeys[i]];
                    for (int j = 0; j < rewards.Count; j++)
                    {
                        if (rewards[j].Level <= Level && !rewards[j].Collected)
                        {
                            if (rewards[j].IsTimed)
                            {
                                freeLimitedCount++;
                            }
                            else
                            {
                                freeCount++;
                            }
                        }
                    }
                }
                var paidCount = 0;
                var paidLimitedCount = 0;
                var paidMissionPassRewards = Client.Get<SeasonPassController>().GoldenMissionPassRewards;
                listPaidKeys = paidMissionPassRewards.Keys.ToList();
                for (int i = 0; i < listPaidKeys.Count; i++)
                {
                    var rewards = paidMissionPassRewards[listPaidKeys[i]];
                    for (int j = 0; j < rewards.Count; j++)
                    {
                        if (rewards[j].Level <= Level && !rewards[j].Collected)
                        {
                            if (rewards[j].IsTimed)
                            {
                                paidLimitedCount++;
                            }
                            else
                            {
                                paidCount++;
                            }
                        }
                    }
                }
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMissionPassCollectAll, 
                    ("Free",freeCount.ToString()),
                    ("FreeLimited",freeLimitedCount.ToString()),
                    ("Mission",paidCount.ToString()),
                    ("MissionLimited",paidLimitedCount.ToString()),
                    ("MissionPassLevel",Client.Get<SeasonPassController>().Level.ToString()));
                    
                Client.Get<SeasonPassController>().CollectAllMissionPass(async (field, action) =>
                {
                    RepeatedField<Item> items = XItemUtility.GetItems(field);
                  
                    if (totalWin > 0)
                    {
                        await XUtility.FlyCoins(btnCollect.transform, new EventBalanceUpdate(totalWin,"FreeSeasonPassRewards"));   
                    }
                    
                    ItemSettleHelper.SettleItems(items);
                    OnCloseAction(action);
                });   
            }
            else
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMissionPassCollect, ("missionType",_isGolden?(_missionPassReward.IsTimed?"MissionLimited":"Mission"):(_missionPassReward.IsTimed?"FreeLimited":"Free")),("MissionPassLevel",Client.Get<SeasonPassController>().Level.ToString()));
                Client.Get<SeasonPassController>().CollectMissionPass(_missionPassReward.Level, _isGolden, _missionPassReward.IsTimed, async (rewards, action) =>
                {
                    RepeatedField<Item> items = XItemUtility.GetItems(rewards);
                    
                    if (totalWin > 0)
                    {
                        await XUtility.FlyCoins(btnCollect.transform, new EventBalanceUpdate(totalWin,"FreeSeasonPassRewards"));   
                    }
                    
                    ItemSettleHelper.SettleItems(items);  
                    OnCloseAction(action);
                });
            }
        }
        
        private void OnCloseAction(Action action)
        {
            action?.Invoke();
            EventBus.Dispatch(new EventSeasonPassUpdate());
            Close();   
        }
        
        public string GetItemDescribe(List<Item> items)
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
            
            if (type == Item.Types.Type.BonusCoupon)
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

            if (type == Item.Types.Type.MonopolyActivityTicket)
            {
                uint amount = 0;
             
                for (int i = 0; i < items.Count; i++)
                {
                    amount += items[i].MonopolyActivityTicket.Amount;
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
        public virtual void OnSeasonPassUpdate(EventSeasonPassUpdate evt)
        {
            
        }
    }

    public class SeasonPassRewardViewController : ViewController<SeasonPassRewardBase>
    {
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSeasonPassUpdate>(view.OnSeasonPassUpdate);
        }
    }
}