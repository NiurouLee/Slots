/************************************************
 * Storage class : StorageAd
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using DragonU3DSDK.Storage;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageAd : StorageBase
    {
        // 分层数据
        [JsonProperty]
        StorageUserGroup userGroupData = new StorageUserGroup();
        [JsonIgnore]
        public StorageUserGroup UserGroupData
        {
            get
            {
                return userGroupData;
            }
        }
        // ---------------------------------//
        
        // 视频广告 观看次数
        [JsonProperty]
        StorageDictionary<string,int> adVideoWatchCount = new StorageDictionary<string,int>();
        [JsonIgnore]
        public StorageDictionary<string,int> AdVideoWatchCount
        {
            get
            {
                return adVideoWatchCount;
            }
        }
        // ---------------------------------//
        
        // 视频广告 观看时间点(秒)
        [JsonProperty]
        StorageDictionary<string,long> adVideoWatchTime = new StorageDictionary<string,long>();
        [JsonIgnore]
        public StorageDictionary<string,long> AdVideoWatchTime
        {
            get
            {
                return adVideoWatchTime;
            }
        }
        // ---------------------------------//
        
        // 广告 倒计时
        [JsonProperty]
        long adResetTime;
        [JsonIgnore]
        public long AdResetTime
        {
            get
            {
                return adResetTime;
            }
            set
            {
                if(adResetTime != value)
                {
                    adResetTime = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
        
        // 插屏广告 观看次数
        [JsonProperty]
        int adSettingWatchCount;
        [JsonIgnore]
        public int AdSettingWatchCount
        {
            get
            {
                return adSettingWatchCount;
            }
            set
            {
                if(adSettingWatchCount != value)
                {
                    adSettingWatchCount = value;
                    StorageManager.Instance.LocalVersion++;
                    
                    
                }
            }
        }
        // ---------------------------------//
    }
}