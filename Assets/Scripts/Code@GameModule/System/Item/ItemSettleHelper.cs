// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/28/10:33
// Ver : 1.0.0
// Description : ItemSettleHelper.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;


namespace GameModule
{
    /// <summary>
    /// Settle需要额外操作的道具，金币，钻石，这些直接加数字的不在这里处理
    /// </summary>
    public class ItemSettleHelper
    {
        public static void SettleItems(RepeatedField<Item> items, Action finishCallback = null, int startIndex = 0, string source = "")
        {
            var index = startIndex;

            if (index < items.Count)
            {
                //卡包逻辑处理，获得多个卡包情况下需要合并处理
               
                if (items[index].Type == Item.Types.Type.CardPackage)
                {
                    ReOderCardPackageElement(items, startIndex);

                    var packageItems = XItemUtility.GetItems(items, Item.Types.Type.CardPackage, startIndex);
                    
                    //卡包数量超过1个，走合并逻辑，否则走正常开包的逻辑
                    
                    if (packageItems.Count > 1)
                    {
                        Client.Get<AlbumController>().SettleCardPackages(packageItems,
                            () =>
                            {
                                EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());
                                SettleItems(items, finishCallback, startIndex + packageItems.Count);
                            }, source);
                        return;
                    }
                }
                
                SettleItem(items[index], () => { SettleItems(items, finishCallback, startIndex + 1); }, source);
            }
            else
            {
                finishCallback?.Invoke();
            }
        }

        /// <summary>
        /// 将items中的CardPackage类型的奖励放在一起,如下图所示：c表示金币道具，v表示vip点数道具，cp表示CARDPackage
        /// [c,cp,c,cp,c,v,cp,v] 操作前
        /// [c,cp,cp,cp,c,c,v,v] 操作后
        /// </summary>
        /// <param name="items"></param>
        /// <param name="startIndex"></param>
        private static void ReOderCardPackageElement(RepeatedField<Item> items, int startIndex)
        {
            int itemToSwapIndex = -1;
            
            for (var i = startIndex; i < items.Count; i++)
            {
                if (items[i].Type == Item.Types.Type.CardPackage)
                {
                    if (itemToSwapIndex > 0)
                    {
                        var item = items[i];
                        items[i] = items[itemToSwapIndex];
                        items[itemToSwapIndex] = item;
                            
                        for (var c = itemToSwapIndex + 1; c <= i; c++)
                        {
                            if (items[c].Type != Item.Types.Type.CardPackage)
                            {
                                itemToSwapIndex = c;
                                break;
                            }
                        }
                    }
                    else
                    {
                        itemToSwapIndex = 0;
                    }
                }
                else
                {
                    if (itemToSwapIndex == 0)
                    {
                        itemToSwapIndex = i;
                    }
                }
            }

        }
        
        public static void SettleItem(Item item, Action finishCallback, string source = "")
        {
            switch (item.Type)
            {
                case Item.Types.Type.ShopGiftBox:
                    SettleGiftBox(item, finishCallback, source);
                    break;
                case Item.Types.Type.VipPoints:
                    SettleVipPoint(item, finishCallback, source);
                    break;
                case Item.Types.Type.DoubleExp:
                case Item.Types.Type.LevelUpBurst:
                case Item.Types.Type.NewbieQuestBoost:
                case Item.Types.Type.SeasonQuestStarBoost:
                case Item.Types.Type.SuperWheel:
                case Item.Types.Type.CashBackBuffsBet:    
                case Item.Types.Type.CashBackBuffsNowin:    
                case Item.Types.Type.CashBackBuffsWin:    
                    SettleBuffItem(item, finishCallback, source);
                    break;
                case Item.Types.Type.Avatar:
                    SettleNewAvatar(item, finishCallback, source);
                    break;
                case Item.Types.Type.CardPackage:
                    SettleCardPackage(item, finishCallback,source);
                    break;
                case Item.Types.Type.MonopolyActivityTicket:
                    SettleMonopolyActivityTicket(item, finishCallback);
                    break;
                case Item.Types.Type.MonopolyActivityPortal:
                    SettleMonopolyActivityPortal(item, finishCallback);
                    break;
                case Item.Types.Type.MonopolyActivityBuffMoreTicket:
                case Item.Types.Type.MonopolyActivityBuffMoreDamage:
                    SettleMonopolyActivityBuff(finishCallback);
                    break;
                default:
                    finishCallback?.Invoke();
                    break;
            }
        }

        public static void SettleMonopolyActivityTicket(Item monopolyItem, Action finishCallback)
        {
            var activityTreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activityTreasureRaid != null)
            {
                activityTreasureRaid.AddTicketCount(monopolyItem.MonopolyActivityTicket.Amount);
            }
            finishCallback?.Invoke();
        }

        public static void SettleMonopolyActivityPortal(Item monopolyItem, Action finishCallback)
        {
            var activityTreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activityTreasureRaid != null)
            {
                activityTreasureRaid.AddPortalCount(monopolyItem.MonopolyActivityPortal.Amount);
            }
            finishCallback?.Invoke();
        }
        
        public static async void SettleMonopolyActivityBuff(Action finishCallback)
        {
            var activityTreasureRaid = Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as Activity_TreasureRaid;
            if (activityTreasureRaid != null)
            {
                await Client.Get<BuffController>().SyncBufferData();
            }
            finishCallback?.Invoke();
        }
        
        public static  void SettleNewAvatar(Item avatarItem, Action finishCallback, string source)
        {
            Client.Get<UserController>().AddNewAvatar(avatarItem.Avatar.Id);
            finishCallback?.Invoke();
        }

        public static void SettleCardPackage(Item cardPackageItem, Action finishCallback, string source)
        {
            Client.Get<AlbumController>().SettleCardPackage(cardPackageItem, ()=>
            {
                EventBus.Dispatch(new EventUpdateAlbumRedDotReminder());
                finishCallback.Invoke();
            }, source);
        }
        
        public static async void SettleBuffItem(Item buffItem, Action finishCallback, string source)
        {
            await Client.Get<BuffController>().SyncBufferData();
            finishCallback?.Invoke();
        }

        public static void SettleVipPoint(Item vipItem, Action finishCallback, string source)
        {
            if (vipItem.VipPoints.LevelUpRewardItems != null 
                && vipItem.VipPoints.LevelUpRewardItems.Count > 0)
            {
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventVipLevelUp, ("source", source));
                EventBus.Dispatch( new EventEnqueuePopup(new PopupArgs(typeof(VipLevelUpPopup), vipItem, BlockLevel.HigherLevel1)));
                
                EventBus.Dispatch(new EventEnqueueFencePopup(()=>
                {
                    finishCallback?.Invoke();
                },BlockLevel.HigherLevel1));
            }
            else
            {
                finishCallback?.Invoke();
            }
        }
        
        public static void SettleGiftBox(Item giftBoxItem, Action finishCallback, string source)
        {
            var giftBox = giftBoxItem.ShopGiftBox.GiftBox;
            if (giftBox.ProgressNow >= giftBox.ProgressMax)
            {
               
                EventBus.Dispatch( new EventEnqueuePopup(new PopupArgs(typeof(GiftBoxRewardPopup), giftBoxItem, BlockLevel.HigherLevel1)));
              
                EventBus.Dispatch(new EventEnqueueFencePopup(()=>
                {
                    finishCallback?.Invoke();
                },BlockLevel.HigherLevel1));
            }
            else
            {
                finishCallback.Invoke();
            }
        }
    }
}