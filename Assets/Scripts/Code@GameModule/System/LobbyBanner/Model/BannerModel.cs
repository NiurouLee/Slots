// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/02/19:22
// Ver : 1.0.0
// Description : BannerModel.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class BannerModel:Model<AdvertisementInfoList>
    {
        public BannerModel()
            : base(ModelType.TYPE_LOBBY_BANNER)
        {
            
        }
 
        public override async Task FetchModalDataFromServerAsync()
        {
            CGetAdvertisement cGetAdvertisement = new CGetAdvertisement();
            var sGetAdvertisement =
                await APIManagerGameModule.Instance.SendAsync<CGetAdvertisement, SGetAdvertisement>(cGetAdvertisement);

            if (sGetAdvertisement.ErrorCode == 0)
            {
                UpdateModelData(sGetAdvertisement.Response.AdvsInfo);
            }
            else
            {
                XDebug.LogError("GetBannerInfoError:" + sGetAdvertisement.ErrorInfo);
            }
        }

        public List<Advertisement> GetValidAdvertisement(string checkPoint)
        {
            if (modelData != null)
            {
                List<Advertisement> list = new List<Advertisement>();
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (AdvertisementValidator.IsValid(modelData.Advs[i],
                        modelData.Now + (ulong) TimeElapseSinceLastUpdate(), checkPoint))
                    {
                        list.Add(modelData.Advs[i]);
                    }
                }
                return list;
            }

            return null;
        }
        
        public List<Advertisement> GetValidDealAdvertisement(string checkPoint = "")
        {
            var list = GetValidAdvertisement(AdvertisementType.Deal, checkPoint);
            
            if (list != null && list.Count > 1)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    for (var j = 0; j < list.Count; j++)
                    {
                        if (list[i].DealInfo.Priority < list[j].DealInfo.Priority)
                        {
                            var advertisement = list[i];
                            list[i] = list[j];
                            list[j] = advertisement;
                        }
                    }
                }
            }
            return list;
        }

        public List<uint> GetValidAdIds(string checkPoint)
        {
            List<uint> ids = new List<uint>();
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (AdvertisementValidator.IsValid(modelData.Advs[i],
                        modelData.Now + (ulong) TimeElapseSinceLastUpdate(),checkPoint))
                    {
                        ids.Push(modelData.Advs[i].Id);
                    }
                }
            }
            return ids;
        }

        public Dictionary<uint, List<Advertisement>> GetValidScrollableAdvertisement()
        {
            var bannerDict = new Dictionary<uint, List<Advertisement>>();
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (AdvertisementValidator.IsValid(modelData.Advs[i],
                        modelData.Now + (ulong) TimeElapseSinceLastUpdate(), "LobbyScrollableAdvertisement") && modelData.Advs[i].Position > 0)
                    {
                        if (!bannerDict.ContainsKey(modelData.Advs[i].Position))
                        {
                            bannerDict[modelData.Advs[i].Position] = new List<Advertisement>();
                        }

                        bannerDict[modelData.Advs[i].Position].Add(modelData.Advs[i]);
                    }
                }
            }

            return bannerDict;
        }

        public List<Advertisement> GetLobbyFixedAdvertisement()
        {
            var bannerList = new List<Advertisement>();
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (AdvertisementValidator.IsValid(modelData.Advs[i],
                        modelData.Now + (ulong) TimeElapseSinceLastUpdate(), "LobbyFixedAdvertisement") && modelData.Advs[i].Position == 0)
                    {
                        //临时代码，100级之后不显示Deal的广告牌
                        if (Client.Get<UserController>().GetUserLevel() >= 100)
                        {
                            if (modelData.Advs[i].Type == AdvertisementType.Deal)
                            {
                                continue;
                            }
                        }
                        
                        bannerList.Add(modelData.Advs[i]);
                    }
                }
            }

            return bannerList;
        }

        public bool IsValidAdvertisement(Advertisement advertisement)
        {
            return (AdvertisementValidator.IsValid(advertisement,
                modelData.Now + (ulong) TimeElapseSinceLastUpdate()));
        }
         
        public List<Advertisement> GetValidAdvertisement(AdvertisementType advertisementType, string checkPoint = "")
        {
            if (modelData != null)
            {
                List<Advertisement> list = new List<Advertisement>();
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (AdvertisementValidator.IsValid(modelData.Advs[i],
                            modelData.Now + (ulong) TimeElapseSinceLastUpdate(), checkPoint)
                        && modelData.Advs[i].Type == advertisementType)
                    {
                        list.Add(modelData.Advs[i]);
                    }
                }
                return list;
            }

            return null;
        }

        public Advertisement GetPiggyBonusDeal()
        {
            if (modelData != null)
            {
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (AdvertisementValidator.IsValid(modelData.Advs[i],
                        modelData.Now + (ulong) TimeElapseSinceLastUpdate()))
                    {
                        if (modelData.Advs[i].Type == AdvertisementType.Deal && modelData.Advs[i].Jump == "PiggyBonus")
                        {
                            
                            return modelData.Advs[i];
                        }
                    }
                }
            }
            
            return null;
        }
        
        public void UpdateAdvertisement(Advertisement advertisementToUpdate, ulong now)
        {
            if (modelData != null && modelData.Advs.Count > 0)
            {
                for (var i = 0; i < modelData.Advs.Count; i++)
                {
                    if (modelData.Advs[i].Id == advertisementToUpdate.Id)
                    {
                        modelData.Advs[i] = advertisementToUpdate;
                        modelData.Now = now;
                        lastTimeUpdateData = Time.realtimeSinceStartup;
                        break;
                    }
                }
            }
        }
        
        public Advertisement GetAdvertisement(string jumpArgs)
        {
            return null;
        }
 
        
    }
}