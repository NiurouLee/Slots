// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/03/16:01
// Ver : 1.0.0
// Description : ViewAsyncDataProvider.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using ILRuntime.Runtime.Intepreter;
using UnityEngine.U2D;

namespace GameModule
{
    public interface IViewAsyncDataProvider
    {
        List<Type> GetSupportViewType();
        Task<object> GetAsyncViewData(Type viewType);
    }


    public class FirstTimeSpecialOfferPopupAsyncDataProvider : IViewAsyncDataProvider
    {
        public FirstTimeSpecialOfferPopupAsyncDataProvider()
        {

        }
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(FirstTimeSpecialOfferPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(FirstTimeSpecialOfferPopup))
                return await Client.Get<BannerController>().GetDealOfferInfo("FirstTimeSpecialOffer");
            return null;
        }
    }
    
    public class PiggyBonusOfferAsyncDataProvider : IViewAsyncDataProvider
    {
        public PiggyBonusOfferAsyncDataProvider()
        {

        }
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(PiggyBonusPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(PiggyBonusPopup))
                return await Client.Get<BannerController>().GetDealOfferInfo("PiggyBonus");
            return null;
        }
    }
    
    public class TimeBonusSuperWheelPopupAsyncDataProvider : IViewAsyncDataProvider
    {
        public TimeBonusSuperWheelPopupAsyncDataProvider()
        {

        }
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TimeBonusSuperWheelPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TimeBonusSuperWheelPopup))
                return await Client.Get<TimeBonusController>().GetWheelInfo();
            return null;
        }
    }

    public class TimeBonusWheelBonusPopupAsyncDataProvider : IViewAsyncDataProvider
    {
        public TimeBonusWheelBonusPopupAsyncDataProvider()
        {

        }
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TimeBonusWheelBonusPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TimeBonusWheelBonusPopup))
            {
                if (Client.Get<TimeBonusController>().IsBonusReady(TimerBonusStage.LuckyWheelBonus))
                {
                    return await Client.Get<TimeBonusController>().GetWheelInfo();
                }
                else
                {
                    return await Client.Get<TimeBonusController>().GetGoldenWheelInfo();
                }
            }

            return null;
        }
    }

    public class StorePopupAsyncDataProvider : IViewAsyncDataProvider
    {
        public StorePopupAsyncDataProvider()
        {

        }
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(StorePopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(StorePopup))
                return await Client.Get<IapController>().GetPaymentHandler<StorePaymentHandler>().GetShopInfo();
            return null;
        }
    }

    public class QuestUIAsyncDataProvider : IViewAsyncDataProvider
    {
        public QuestUIAsyncDataProvider()
        {

        }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(QuestStartPopup),
                typeof(QuestDetailWidget)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            var assetReference = await AssetHelper.PrepareAssetAsync<SpriteAtlas>("UIQuestStartAtlas");
            return assetReference;
        }
    }

    public class SeasonPassBuyAsyncDataProvider : IViewAsyncDataProvider
    {
        public SeasonPassBuyAsyncDataProvider()
        {

        }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(SeasonPassPurchaseGolden),
                typeof(SeasonPassBuyLevel)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(SeasonPassPurchaseGolden) || viewType == typeof(SeasonPassBuyLevel))
                return await Client.Get<SeasonPassController>().FetchShopItems();
            return null;
        }
    }

    public class PiggyBankAsyncDataProvider : IViewAsyncDataProvider
    {
        public PiggyBankAsyncDataProvider()
        {

        }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(PiggyBankMainPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(PiggyBankMainPopup))
                return await Client.Get<PiggyBankController>().GetPiggyData();
            return null;
        }
    }

    public class DailyBonusCalendarAsyncDataProvider : IViewAsyncDataProvider
    {
        public DailyBonusCalendarAsyncDataProvider() { }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(DailyBonusCalendarPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(DailyBonusCalendarPopup))
            {
                await Client.Get<DailyBonusController>().RefreshRewardData();
            }
            return Client.Get<DailyBonusController>();
        }
    }

    public class ContactUsAsyncDataProvider : IViewAsyncDataProvider
    {
        public ContactUsAsyncDataProvider() { }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(UIContactUsPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(UIContactUsPopup))
                return await Client.Get<ContactUsController>().SendCListUserComplainMessage();
            return null;
        }
    }

    public class Activity_Valentine2022_MapPopup_AsyncDataProvider : IViewAsyncDataProvider
    {
        public Activity_Valentine2022_MapPopup_AsyncDataProvider() { }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>() { typeof(UIActivity_ValentinesDay_MapPopup) };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(UIActivity_ValentinesDay_MapPopup))
            {
                var controller = Client.Get<ActivityController>();
                var activity = controller.GetDefaultActivity(ActivityType.Valentine2022) as Activity_ValentinesDay;
                if (activity != null)
                {
                    return await activity.PrepareMainPageData();
                }
            }
            return null;
        }
    }

    public class LuckySpinDataProvider : IViewAsyncDataProvider
    {
        public LuckySpinDataProvider()
        {

        }
        
        public List<Type> GetSupportViewType()
        {
            return new List<Type>() { typeof(LuckySpinPopup) };
        }
        
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(LuckySpinPopup))
            {
                return await Client.Get<AlbumController>().GetLuckySpinInfo();
            }
            
            return null;
        }
    }

    public class AmazingInTheHatDataProvider : IViewAsyncDataProvider
    {
        public AmazingInTheHatDataProvider() { }
        public List<Type> GetSupportViewType()
        {
            return new List<Type>() { typeof(AmazingHatMainPopup) };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(AmazingHatMainPopup))
            {
                return await Client.Get<AmazingHatController>().GetHatGameData();
            }
            
            return null;
        }
    }
    public class InboxDataProvider : IViewAsyncDataProvider
    {
        public InboxDataProvider() { }

        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(UIInboxPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(UIInboxPopup))
                return await Client.Get<InboxController>().RefreshAllInboxItem();
            return null;
        }
    }

    public class UIBankruptPopupDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(UIBankruptPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(UIBankruptPopup))
            {
                var cBankrupt = new CBankrupt();
                var sBankrupt = await APIManagerGameModule.Instance.SendAsync<CBankrupt, SBankrupt>(cBankrupt);

                if (sBankrupt.ErrorCode == 0)
                {
                    if (sBankrupt.Response.Available)
                    {
                        EventBus.Dispatch(new EventUserProfileUpdate(sBankrupt.Response.UserProfile));
                        return sBankrupt.Response;
                    }
                }
            }

            return null;
        }
    }

    public class SeasonQuestPaymentDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(SeasonQuestPassPaymentPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(SeasonQuestPassPaymentPopup))
            {
                var cSeasonQuestPaymentItems = new CGetSeasonQuestPaymentItems();
                var sSeasonQuestPaymentItems = await APIManagerGameModule.Instance.SendAsync<CGetSeasonQuestPaymentItems, SGetSeasonQuestPaymentItems>(cSeasonQuestPaymentItems);

                if (sSeasonQuestPaymentItems.ErrorCode == 0)
                {
                    return sSeasonQuestPaymentItems.Response.Items[0];
                }
            }

            return null;
        }
    }
    // public class SeasonQuestRankPopupDataProvider : IViewAsyncDataProvider
    // {
    //     public List<Type> GetSupportViewType()
    //     {
    //         return new List<Type>()
    //         {
    //             typeof(SeasonQuestRankPopup)
    //         };
    //     }
    //     
    //     public async Task<object> GetAsyncViewData(Type viewType)
    //     {
    //         if (viewType == typeof(SeasonQuestRankPopup))
    //         {
    //             var cLeaderboard = new CSeasonQuestLeaderboard();
    //             var sLeaderboard = await APIManagerGameModule.Instance.SendAsync<CSeasonQuestLeaderboard, SSeasonQuestLeaderboard>(cLeaderboard);
    //
    //             if (sLeaderboard.ErrorCode == 0)
    //             {
    //                 return sLeaderboard.Response;
    //             }
    //         }
    //         
    //         return null;
    //     }
    // }

    public class SeasonQuestDifficultyChoosePopupDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(SeasonQuestDifficultChoosePopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(SeasonQuestDifficultChoosePopup))
            {
                var request = new CGetSeasonQuestDifficultyRewards();
                var response = await APIManagerGameModule.Instance.SendAsync<CGetSeasonQuestDifficultyRewards, SGetSeasonQuestDifficultyRewards>(request);

                if (response.ErrorCode == 0)
                {
                    return response.Response;
                }
            }

            return null;
        }

    }


    public class NewBieQuestPayItemDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(QuestPayPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            return await Client.Get<NewBieQuestController>().GetNewBieQuestPayItem();
        }
    }
    
    public class CardHistoryDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
        
                typeof(CardHistoryPopup)
            };
        }

        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(CardHistoryPopup))
            {
                return await Client.Get<AlbumController>().GetCardHistoryInfo();
            }

            return null;
        }
    }
    
    public class CardExchangeDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(CardExchangePopup)
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(CardExchangePopup))
            {
                return await Client.Get<AlbumController>().GetCardRecycleInfo();
            }
    
            return null;
        }
    }

    public class ActivityTreasureRaidMapDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TreasureRaidMapPopup)
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TreasureRaidMapPopup))
            {
                var activityTreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                        Activity_TreasureRaid;
                if (activityTreasureRaid == null)
                {
                    return null;
                }

                if (!activityTreasureRaid.HasLastPuzzleListInfo())
                {
                    var listInfo = await activityTreasureRaid.GetMonopolyPuzzleListInfo();
                    if (listInfo == null)
                    {
                        return null;
                    }
                    activityTreasureRaid.SetLastPuzzleListInfo(listInfo);
                }
                return await activityTreasureRaid.GetRoundListInfo();
            }
    
            return null;
        }
    }

    public class ActivityTreasureRaidMainDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TreasureRaidMainPopup)
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TreasureRaidMainPopup))
            {
                var activityTreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                        Activity_TreasureRaid;
                if (activityTreasureRaid == null)
                {
                    return null;
                }
                if (!activityTreasureRaid.HasLastPuzzleListInfo())
                {
                    var listInfo = await activityTreasureRaid.GetMonopolyPuzzleListInfo();
                    if (listInfo == null)
                    {
                        return null;
                    }
                    activityTreasureRaid.SetLastPuzzleListInfo(listInfo);
                }
                return await activityTreasureRaid.GetMonopolyRoundInfo();
            }
    
            return null;
        }
    }

    public class ActivityTreasureRaidRankDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TreasureRaidRankPopup)
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TreasureRaidRankPopup))
            {
                var activityTreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                        Activity_TreasureRaid;
                if (activityTreasureRaid == null)
                {
                    return null;
                }
                return await activityTreasureRaid.GetMonopolyLeaderboard();
            }
    
            return null;
        }
    }
    
    public class ActivityTreasureRaidPuzzleDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TreasureRaidPuzzlePopup)
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TreasureRaidPuzzlePopup))
            {
                var activityTreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                        Activity_TreasureRaid;
                if (activityTreasureRaid == null)
                {
                    return null;
                }
                return await activityTreasureRaid.GetMonopolyPuzzleListInfo();
            }
    
            return null;
        }
    }

    public class ActivityTreasureRaidBuyTicketDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(TreasureRaidBuyTicketPopup),
                typeof(TreasureRaidBoosterPopup),
                typeof(TreasureRaidBoosterAdPopup),
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(TreasureRaidBuyTicketPopup) || viewType == typeof(TreasureRaidBoosterPopup) || viewType == typeof(TreasureRaidBoosterAdPopup))
            {
                var activityTreasureRaid =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.TreasureRaid) as
                        Activity_TreasureRaid;
                if (activityTreasureRaid == null)
                {
                    return null;
                }
                return await activityTreasureRaid.MonopolyTicketPaymentInfo();
            }
    
            return null;
        }
    }
    public class ActivityJulyCarnivalMainDataProvider : IViewAsyncDataProvider
    {
        public List<Type> GetSupportViewType()
        {
            return new List<Type>()
            {
                typeof(JulyCarnivalMainPopup)
            };
        }
    
        public async Task<object> GetAsyncViewData(Type viewType)
        {
            if (viewType == typeof(JulyCarnivalMainPopup))
            {
                var activityJulycarniva =
                    Client.Get<ActivityController>().GetDefaultActivity(ActivityType.JulyCarnival) as
                        Activity_JulyCarnival;
                if (activityJulycarniva == null)
                {
                    return null;
                }
                return await activityJulycarniva.GetIndependenceDayMainPageInfoData();
            }
            return null;
        }
    }

    public static class ViewAsyncDataProvider
    {
        private static Dictionary<Type, IViewAsyncDataProvider> _asyncDataProviers;

        private static void Initialize()
        {
            var allType = GameModuleLaunch.GetAllType();

            _asyncDataProviers = new Dictionary<Type, IViewAsyncDataProvider>();

            for (var i = 0; i < allType.Count; i++)
            {

                if (!allType[i].Name.Contains("DataProvider"))
                {
                    continue;
                }

                if (!allType[i].IsClass || allType[i].IsGenericType || allType[i].IsGenericTypeDefinition)
                {
                    continue;
                }

                if (allType[i] == typeof(IViewAsyncDataProvider))
                {
                    continue;
                }

                if (typeof(IViewAsyncDataProvider).IsAssignableFrom(allType[i]))
                {
                    var asyncDataProvider = Activator.CreateInstance(allType[i]) as IViewAsyncDataProvider;
                    if (asyncDataProvider != null)
                    {
                        var supportTypes = asyncDataProvider.GetSupportViewType();
                        if (supportTypes != null && supportTypes.Count > 0)
                        {
                            for (var index = 0; index < supportTypes.Count; index++)
                            {
                                _asyncDataProviers.Add(supportTypes[index], asyncDataProvider);
                            }
                        }
                    }
                }
            }
        }

        public static bool NeedAsyncData(Type viewType)
        {
            if (_asyncDataProviers == null)
            {
                Initialize();
            }

            if (_asyncDataProviers != null)
            {
                return _asyncDataProviers.ContainsKey(viewType);
            }

            return false;
        }

        public static async Task<object> GetAsyncData(Type viewType)
        {
            if (_asyncDataProviers == null)
            {
                Initialize();
            }

            if (_asyncDataProviers != null)
            {
                if (_asyncDataProviers.ContainsKey(viewType))
                {
                    return await _asyncDataProviers[viewType].GetAsyncViewData(viewType);
                }
            }

            return null;
        }
    }
}