/************************************************
 * Storage class : StorageUserGroup
 * This file is can not be modify !!!
 * If there is some problem, ask bin.guo.
 ************************************************/

using DragonU3DSDK.Storage;
using Newtonsoft.Json;

namespace DragonU3DSDK.Storage
{
    [System.Serializable]
    public class StorageUserGroup : StorageBase
    {
        
        // 缓存配置表
        [JsonProperty]
        StorageDictionary<string,string> cacheConfig = new StorageDictionary<string,string>(true);
        [JsonIgnore]
        public StorageDictionary<string,string> CacheConfig
        {
            get
            {
                return cacheConfig;
            }
        }
        // ---------------------------------//
        
        // 缓存配置表
        [JsonProperty]
        StorageDictionary<string,string> cacheConfigV2 = new StorageDictionary<string,string>(true);
        [JsonIgnore]
        public StorageDictionary<string,string> CacheConfigV2
        {
            get
            {
                return cacheConfigV2;
            }
        }
        // ---------------------------------//
        
    }
}