// ReSharper disable CommentTypo
// ReSharper disable StringLiteralTypo
// ReSharper disable MemberCanBePrivate.Global
/************************************************
 * Ad ConfigHub Manager class : AdConfigManager
 * This file is can not be modify !!!
 * If there is some problem, ask yunhan.zeng@dragonplus.com
 ************************************************/

using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using DragonPlus.ConfigHub;
using UnityEngine;
using Newtonsoft.Json;
using DragonU3DSDK.Asset;
using LitJson;

namespace GameModule
{
    public class AdConfigManager : ConfigManagerBase
    {   
        private static AdConfigManager _instance;
        public static AdConfigManager Instance => _instance ?? (_instance = new AdConfigManager());
        public override string Guid => "config_ad";
        public override int VersionMinIOS => 1;
        public override int VersionMinAndroid => 1;
        protected override List<string> SubModules => new List<string> { 
            "AdReward",
            "AdBonus",
            "AdInterstitial",
        };
        private readonly Dictionary<Type, string> typeToEnum = new Dictionary<Type,string> { 
            [typeof(AdReward)] = "AdReward",
            [typeof(AdBonus)] = "AdBonus",
            [typeof(AdInterstitial)] = "AdInterstitial",
        };
        private List<AdReward> AdRewardList;
        private List<AdBonus> AdBonusList;
        private List<AdInterstitial> AdInterstitialList;
        
        public List<T> _GetConfig<T>(CacheOperate cacheOp = CacheOperate.None, long cacheDuration = -1)
        {
            List<T> cfg;
            var subModule = typeToEnum[typeof(T)];
            switch (subModule)
            { 
                case "AdReward": cfg = AdRewardList as List<T>; break;
                case "AdBonus": cfg = AdBonusList as List<T>; break;
                case "AdInterstitial": cfg = AdInterstitialList as List<T>; break;
                default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
            }
            processCache(cacheOp, cacheDuration);
            return cfg;
        }

        public override List<T> GetConfig<T>(CacheOperate cacheOp = CacheOperate.None, long cacheDuration = -1)
        {
            throw new System.NotImplementedException();
        }
    
        protected override bool CheckTable(Hashtable table)
        {   
            if (!table.ContainsKey("adreward")) return false;
            if (!table.ContainsKey("adbonus")) return false;
            if (!table.ContainsKey("adinterstitial")) return false;
            return true;
        }
    
        private Hashtable loadFromLocal()
        {
            var ta = Resources.Load<TextAsset>("Configs/UserGroup/ad");
            if (string.IsNullOrEmpty(ta.text))
            {
                ConfigHubUtil.L("Load Configs/UserGroup/ad error!");
                return null;
            }
            return JsonConvert.DeserializeObject<Hashtable>(ta.text);
        }
    
        public override void InitConfig(MetaData metaData, string jsonData = null)
        {
            var table = !string.IsNullOrEmpty(jsonData) ? JsonConvert.DeserializeObject<Hashtable>(jsonData) : null;
            IsRemote = metaData != null && table != null && CheckTable(table);
            if (!IsRemote)
            {
                table = loadFromLocal();
                metaData = GetMetaDataCached() ?? GetMetaDataDefault();
            }
            MetaData = metaData;
            
            PropertyInfo pInfo;
            foreach (var subModule in SubModules)
            {
                switch (subModule)
                { 
                    case "AdReward": AdRewardList = JsonMapper.ToObject<List<AdReward>>(JsonConvert.SerializeObject(table["adreward"]));   break;
                    case "AdBonus": AdBonusList = JsonMapper.ToObject<List<AdBonus>>(JsonConvert.SerializeObject(table["adbonus"]));   break;
                    case "AdInterstitial": AdInterstitialList = JsonMapper.ToObject<List<AdInterstitial>>(JsonConvert.SerializeObject(table["adinterstitial"]));   break;
                    default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                }
    
                if (IsRemote)
                    continue;
    
                switch (subModule)
                { 
                    case "AdReward": 
                        pInfo = typeof(AdReward).GetProperty("UserGroup");
                        if (pInfo != null && pInfo.PropertyType == typeof(int))
                            AdRewardList = AdRewardList.FindAll(cfg => (int)pInfo.GetValue(cfg) == metaData.GroupId);
                        break;
                    case "AdBonus": 
                        pInfo = typeof(AdBonus).GetProperty("UserGroup");
                        if (pInfo != null && pInfo.PropertyType == typeof(int))
                            AdBonusList = AdBonusList.FindAll(cfg => (int)pInfo.GetValue(cfg) == metaData.GroupId);
                        break;
                    case "AdInterstitial": 
                        pInfo = typeof(AdInterstitial).GetProperty("UserGroup");
                        if (pInfo != null && pInfo.PropertyType == typeof(int))
                            AdInterstitialList = AdInterstitialList.FindAll(cfg => (int)pInfo.GetValue(cfg) == metaData.GroupId);
                        break;
                    default: throw new ArgumentOutOfRangeException(nameof(subModule), subModule, null);
                }
            }
            
            ConfigHubUtil.L($"InitConfig:{getModuleString()}");
            EventBus.Dispatch<EventAdConfigRefresh>();
        }
    
        private List<Rules> RulesList;
        protected override bool HasGroup(int groupId)
        {
            if (RulesList == null || RulesList.Count == 0)
            {
                var table = loadFromLocal();
                RulesList = JsonMapper.ToObject<List<Rules>>(JsonConvert.SerializeObject(table["rules"]));
            }
            return RulesList.Exists(r => r.groupId == groupId);
        }
    }
}

