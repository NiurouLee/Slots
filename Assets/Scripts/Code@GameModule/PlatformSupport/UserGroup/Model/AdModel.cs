using System;
using System.Collections.Generic;
using DragonU3DSDK;
using DragonU3DSDK.Storage;
using UnityEngine;

namespace GameModule
{
    public class AdModel
    {
        public Dictionary<int, AdBonus> AdBonusConfigs = new Dictionary<int, AdBonus>();
        public readonly Dictionary<int, AdReward> AdRewardConfigs = new Dictionary<int, AdReward>();
        public readonly Dictionary<int, AdInterstitial> AdInterstitialConfigs = new Dictionary<int, AdInterstitial>();


        public AdModel()
        {
            EventBus.Subscribe<EventAdConfigRefresh>(OnAdConfigRefresh);
        }

        private void OnAdConfigRefresh(EventAdConfigRefresh obj)
        {
            InitAdReward();
        }

        public void InitAdReward()
        {
            AdBonusConfigs.Clear();
            var cfgAdBonus = AdConfigManager.Instance._GetConfig<AdBonus>();
            foreach (var cfg in cfgAdBonus)
                AdBonusConfigs[cfg.id] = cfg;
            AdRewardConfigs.Clear();
            var cfgAdReward = AdConfigManager.Instance._GetConfig<AdReward>();
            foreach (var cfg in cfgAdReward)
                AdRewardConfigs[cfg.placeId] = cfg;
            AdInterstitialConfigs.Clear();
            var cfgAdInterstitial = AdConfigManager.Instance._GetConfig<AdInterstitial>();
            foreach (var cfg in cfgAdInterstitial)
                AdInterstitialConfigs[cfg.placeId] = cfg;
        }

        /// <summary>
        /// Tries the set ad reset time.
        /// </summary>
        public void TryToSetAdResetTime()
        {
            var storage = StorageManager.Instance.GetStorage<StorageAd>();
            var currentTime = TotalSeconds();

            //DebugUtil.Log("AdRest 当前时间为 " + currentTime);
            //DebugUtil.Log("AdRest 存档时间为 " + cookStorage.AdResetTime);
            if (currentTime > storage.AdResetTime)
            {

                //DebugUtil.Log("AdRest : 当时与上次时间为不同天");
                storage.AdResetTime = GetTomorrowTimestamp();
                //reset the rv watch count
                var keys = new List<string>(storage.AdVideoWatchCount.Keys);
                foreach (var key in keys)
                {
                    storage.AdVideoWatchCount[key] = 0;
                }

                //foreach (string key in cookStorage.AdVideoWatchTime.Keys)
                //{
                //    cookStorage.AdVideoWatchTime[key] = 0;
                //}

                //reset ad watch count
                storage.AdSettingWatchCount = 0;
                foreach (var pos in Enum.GetValues(typeof(eAdInterstitial)))
                {
                    PlayerPrefs.SetInt($"ad_Interstitial_pos_{(int)pos}", 0);
                }
                // reset AdRewardConfigs
                InitAdReward();
            }
            else
            {
                //DebugUtil.Log("AdRest : 当时与上次时间为同一天");
            }
        }

        public bool IsRewardedVideoInCD(eAdReward pos)
        {
            var key = ((int)pos).ToString();
            var storage = StorageManager.Instance.GetStorage<StorageAd>();
            if (!storage.AdVideoWatchTime.ContainsKey(key))
                return false;
            return TotalSeconds() - storage.AdVideoWatchTime[key] < GetRewardedVideoCDinSeconds(pos);
        }

        public void ResetRewardedVideoCD(eAdReward pos)
        {
            var key = ((int)pos).ToString();
            var storage = StorageManager.Instance.GetStorage<StorageAd>();
            if (!storage.AdVideoWatchTime.ContainsKey(key))
                return;
            storage.AdVideoWatchTime[key] = 0;
        }

        public void RecordRewardedVideoCD(eAdReward pos)
        {
            var nowStamp = TotalSeconds();
            StorageManager.Instance.GetStorage<StorageAd>().AdVideoWatchTime[((int)pos).ToString()] = nowStamp;
        }

        public long GetRewardedVideoCDinSeconds(eAdReward pos)
        {
            if (!AdRewardConfigs.ContainsKey((int)pos))
                return 0;

            var config = AdRewardConfigs[(int)pos];
            // DebugUtil.Log("位置" + pos + " CD为" + config.ShowInterval);
            return config.showInterval;
        }

        public int GetArg1(eAdReward pos)
        {
            if (!AdRewardConfigs.ContainsKey((int)pos))
                return 0;
            var config = AdRewardConfigs[(int)pos];
            return config.arg1;
        }

        public List<int> GetArg2(eAdReward pos)
        {
            if (!AdRewardConfigs.ContainsKey((int)pos))
                return null;
            var config = AdRewardConfigs[(int)pos];
            return config.arg2;
        }

        public long GetRewardedVideoCDinMilliseconds(eAdReward pos)
        {
            return GetRewardedVideoCDinSeconds(pos) * 1000;
        }

        /// <summary>
        /// Adds the reward video watch count.
        /// </summary>
        /// <param name="pos">Position.</param>
        public void AddRewardedVideoWatchCount(eAdReward pos)
        {
            TryToSetAdResetTime();
            var storage = StorageManager.Instance.GetStorage<StorageAd>();
            if (storage == null)
                return;

            var key = ((int)pos).ToString();
            if (!storage.AdVideoWatchCount.ContainsKey(key))
                storage.AdVideoWatchCount.Add(key, 0);
            storage.AdVideoWatchCount[key]++;

            var nowStamp = TotalSeconds();
            if (!storage.AdVideoWatchTime.ContainsKey(key))
                storage.AdVideoWatchTime.Add(key, nowStamp);
            storage.AdVideoWatchTime[key] = nowStamp;
        }

        /// <summary>
        /// Adds the interstitial watch count.
        /// </summary>
        /// <param name="pos">Position.</param>
        public void AddInterstitialWatchCount(eAdInterstitial pos)
        {
            TryToSetAdResetTime();
            var c = PlayerPrefs.GetInt($"ad_Interstitial_pos_{(int)pos}", 0);
            c++;
            PlayerPrefs.SetInt($"ad_Interstitial_pos_{(int)pos}", c);
            PlayerPrefs.SetString($"ad_Interstitial_last_show_time_pos_{(int)pos}", GetTimeStamp().ToString());
            DebugUtil.Log($"插屏广告次数：{c}");
        }

        /// <summary>
        /// Is the rewarded video meet cap.
        /// </summary>
        /// <param name="pos">Position.</param>
        public bool IsRewardedVideoMeetCap(eAdReward pos)
        {
            TryToSetAdResetTime();

            //获取该位置上限及已观看次数
            var times = GetRewardedVideoWatchCount(pos);
            var cap = GetRewardedVideoWatchCap(pos);
            //DebugUtil.Log("位置" + pos + " 位置未达上限，上限为" + cap + " 观看次数为" + times);
            return times >= cap && cap >= 0;
        }

        private string GetStringKeyViaEnum(eAdReward pos)
        {
            return ((int)pos).ToString();
        }

        public int GetRewardedVideoWatchCount(eAdReward pos)
        {
            TryToSetAdResetTime();
            StorageDictionary<string, int> dic = StorageManager.Instance.GetStorage<StorageAd>().AdVideoWatchCount;

            if (dic == null)
            {
                return 0;
            }
            string key = GetStringKeyViaEnum(pos);

            if (!dic.ContainsKey(key))
            {
                dic.Add(key, 0);
                return 0;
            }

            return dic[key];
        }

        public int GetRewardedVideoWatchCap(eAdReward pos)
        {
            int key = (int)pos;

            if (!AdRewardConfigs.ContainsKey(key))
                return 0;

            var adConfig = AdRewardConfigs[key];
            int cap = adConfig.limitPerDay;
            return cap;
        }

        public List<int> GetRewardedVideoBonus(eAdReward pos)
        {
            int key = (int)pos;

            if (!AdRewardConfigs.ContainsKey(key))
                return new List<int>();

            var adConfig = AdRewardConfigs[key];
            return adConfig.bonus;
        }

        public AdBonus GetAdBonus(int bonusID)
        {
            if (!AdBonusConfigs.ContainsKey(bonusID))
            {
                return null;
            }

            var bonus = AdBonusConfigs[bonusID];
            return bonus;
        }

        public AdInterstitial GetInterstitialConfig(int pos)
        {
            return AdInterstitialConfigs.ContainsKey(pos) ? AdInterstitialConfigs[pos] : null;
        }

        public static long TotalSeconds()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static long TotalMilliseconds()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return Convert.ToInt64(ts.TotalMilliseconds);
        }

        //获取第二天凌晨时间戳
        public static long GetTomorrowTimestamp()
        {
            DateTime tomorrow = DateTime.UtcNow.AddDays(1);
            DateTime tomorrowMidnight = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0, 0, DateTimeKind.Utc);
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = tomorrowMidnight.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        /// <summary>
        /// 获取当前时间戳(毫秒)
        /// </summary>
        /// <returns></returns>
        public static long GetTimeStamp()
        {
            // DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            DateTime startTime = TimeZoneInfo.ConvertTimeFromUtc(new DateTime(1970, 1, 1), TimeZoneInfo.Local);// 当地时区
            return (long)(DateTime.Now - startTime).TotalMilliseconds;
        }
    }
}