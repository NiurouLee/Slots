using System.Collections.Generic;
using System.Linq;
using DragonU3DSDK.Network.API;
using GameModule;
using LitJson;
using UnityEngine;

namespace GameModule
{
    public class APIConfigLoad
    {
        public static void Load()
        {
            // if (!APIConfig.APIEntries.ContainsKey("CHeartBeat"))
            // {
            //     APIConfig.APIEntries.Add("CHeartBeat", new APIEntry {
            //         uri = "/api/heart_beat",
            //         method = "GET",
            //         scheme = "https",
            //         timeout = 10,
            //         gzip = false,
            //         ignoreAuth = false,
            //     });
            // }

            AssetHelper.PrepareAsset<TextAsset>("protocol_map", (assetRef) =>
            {
                var jsonData = LitJson.JsonMapper.ToObject(assetRef.GetAsset<TextAsset>().text);
                foreach (KeyValuePair<string,JsonData> item in jsonData)
                {
                    var data = item.Value;
                    if (!APIConfig.APIEntries.ContainsKey(item.Key))
                    {
                        APIConfig.APIEntries.Add(item.Key, new APIEntry {
                            uri = data["uri"].ToString(),
                            method = data["method"].ToString(),
                            scheme = "https",
                            timeout = 10,
                            gzip = false,
                            ignoreAuth = false,
                        });
                    }
                }
                
                assetRef.ReleaseOperation();
            });

        }
    }
}