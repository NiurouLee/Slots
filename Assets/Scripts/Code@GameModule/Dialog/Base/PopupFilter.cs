// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2020-12-01 8:28 PM
// Ver : 1.0.0
// Description : DialogArgs.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public abstract class PopupValidFilter
    {
        public abstract bool IsValid(PopupArgs popupArgs, string args);
    }
    
    public class LevelLowerFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var level = UInt64.Parse(args);
            return Client.Get<UserController>().GetUserLevel() < level;
        }
    }
    
    public class LevelEqualFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var level = UInt64.Parse(args);
            return Client.Get<UserController>().GetUserLevel() == level;
        }
    }
    
    public class LevelGreatEqualFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var level = UInt64.Parse(args);
            return Client.Get<UserController>().GetUserLevel() > level;
        }
    }
    
    public class IsIapUserFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return false;
        }
    }
    
    public class IapLargeThanFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return false;
        }
    }
    
    public class IapSmallThanFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return false;
        }
    }
    
    public class IsFreeUserFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return true;
        }
    }

    public class RvAdAvailableFilter : PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var placeId = (eAdReward)Enum.Parse(typeof(eAdReward), args);
           
            if (placeId == eAdReward.AdTask)
            {
                var adTaskInfo = AdController.Instance.GetAdTaskInfo();

                if (adTaskInfo == null || adTaskInfo.TaskStep >= adTaskInfo.Rewards.Count)
                {
                    return false;
                }
            }
            return AdController.Instance.ShouldShowRV(placeId, false);
        }
    }
    
    public class FirstPurchaseAvailableFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return Client.Get<BannerController>().GetFirstTimeSpecialOfferAdv() != null;
        }
    }
    
    public class FirstPurchaseNotAvailableFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return Client.Get<BannerController>().GetFirstTimeSpecialOfferAdv() == null;
        }
    }
    
    public class SeasonQuestAvailableFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var seasonQuestController = Client.Get<SeasonQuestController>();
            if (seasonQuestController.IsLocked() ||
                seasonQuestController.IsTimeOut())
            {
                return false;
            }
            
            return true;
        }
    }
    
    public class CanShowSmashNoticePopupFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var crazeSmashController = Client.Get<CrazeSmashController>();
            
            return crazeSmashController.CanShowAD();
        }
    }
    
    public class SeasonQuestNewUnlockFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var seasonQuestController = Client.Get<SeasonQuestController>();

            return seasonQuestController.seasonQuestNewUnlocked;
        }
    }
    
    public class NewBieQuestNewUnlockFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var newBieQuestController = Client.Get<NewBieQuestController>();

            return newBieQuestController.IsNewUnlocked;
        }
    }
    
    public class NewBieQuestAvailableFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var newBieQuestController = Client.Get<NewBieQuestController>();

            return (!newBieQuestController.IsLocked() 
                    && !newBieQuestController.IsQuestFinished() 
                    && !newBieQuestController.IsTimeOut());
        }
    }
    
    public class AlbumSeasonIsNewStartFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var albumController = Client.Get<AlbumController>();

            if (albumController.IsOpen() && albumController.IsUnlocked() &&
                albumController.IsNewSeasonStart())
            {
                return true;
            }
            
            return false;
        }
    }
    
    public class AlbumSeasonIsAlmostOverFilter:PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            var albumController = Client.Get<AlbumController>();

            if (albumController.IsOpen() && albumController.IsUnlocked() &&
                albumController.GetSeasonFinishCountDown() <= 5 * 24 * 3600)
            {
                return true;
            }
            return false;
        }
    }
    
    public class PiggyPortraitAvailableFilter : PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return Client.Get<BannerController>().GetDealOfferAdv("PiggyBonus") != null;
        }
    }
    
    public class CoinDashAvailableFilter : PopupValidFilter
    {
        public override bool IsValid(PopupArgs popupArgs, string args)
        {
            return Client.Get<CoinDashController>().IsOpen();
        }
    }

    
    public static class PopupFilter
    {
        public class FilterConfig
        {
            public string filterName;
            public Type filterType;
            public string filterArgs;
        }
        
        private static Dictionary<Type, PopupValidFilter> filterList;
        private static Dictionary<int, FilterConfig> filterConfigList;
        
        static PopupFilter()
        {
            var allTypes = GameModuleLaunch.GetAllType();
            
            XDebug.Log("TypeCount:" + allTypes.Count);
            filterList = new Dictionary<Type, PopupValidFilter>();
            
            for (var i = 0; i < allTypes.Count; i++)
            {
                if (allTypes[i].IsSubclassOf(typeof(PopupValidFilter)))
                {
                    filterList.Add(allTypes[i], (PopupValidFilter)Activator.CreateInstance(allTypes[i]));
                }
            }
        }

        public static void InitFilterConfig(RepeatedField<PopUpConfig.Types.PopupConfigFilter> configFilters)
        {
            if (configFilters.Count > 0)
            {
                filterConfigList = new Dictionary<int, FilterConfig>();
                for (var i = 0; i < configFilters.Count; i++)
                {
                    var type = Type.GetType($"GameModule.{configFilters[i].FilterType}");
                  
                    if (type != null && filterList.ContainsKey(type))
                    {
                        FilterConfig c = new FilterConfig();
                        c.filterArgs = configFilters[i].Arg1;
                        c.filterName = configFilters[i].FilterType;
                        c.filterType = type;
                        filterConfigList.Add((int) configFilters[i].Id, c);
                    }
                }
            }
        }
        
        public static bool CheckValid(int filterId, PopupArgs popUpArgs)
        {
            if (filterConfigList.ContainsKey(filterId))
            {
                var result =  filterList[filterConfigList[filterId].filterType].IsValid(popUpArgs, filterConfigList[filterId].filterArgs);

                if (!result)
                {
                    XDebug.Log($"Failed OnFilter {filterId} FilterName:{filterConfigList[filterId].filterName}, FilterArgs:{filterConfigList[filterId].filterArgs}");
                }
                
                return result;
            }
            
            XDebug.Log($"Failed OnFilter {filterId} Reason:FilterNotExist");
            
            return false;
        }
    }
}