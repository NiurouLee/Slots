// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/29/14:14
// Ver : 1.0.0
// Description : DealOfferHandler.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class DealOfferHandler
    {
        public Advertisement adv = null;
        public DealOfferHandler(Advertisement inAdv)
        {
            adv = inAdv;
        }

        public async Task ActiveOffer(float lastTime)
        {
            if (adv != null)
            {
                if (adv.DealInfo.HiddenCountDown < lastTime)
                {
                    await GetDealInfo();
                }
            }
        }

        public bool IsActive(float lastTime)
        {
            if (adv.DealInfo.Interval < 0 && adv.DealInfo.LastPopAt > 0)
            {
                return true;
            }

            if (adv.DealInfo.HiddenCountDown > lastTime)
            {
                return true;
            }

            return false;
        }

        public ulong GetCountDown(float lastTime)
        {
            return adv.DealInfo.HiddenCountDown - (ulong)lastTime;
        }
        
        public async Task<SGetAdvertisementItem> GetDealInfo()
        {
            CGetAdvertisementItem cGetAdvertisementItem = new CGetAdvertisementItem();
            cGetAdvertisementItem.AdvertId = adv.Id;
            var sGetAdvertisementItem 
                = await APIManagerGameModule.Instance.SendAsync<CGetAdvertisementItem, SGetAdvertisementItem>(cGetAdvertisementItem);
            
            if (sGetAdvertisementItem.ErrorCode == ErrorCode.Success)
            {
                adv = sGetAdvertisementItem.Response.Adv;
                return sGetAdvertisementItem.Response;
            }

            return null;
        }
    }
}