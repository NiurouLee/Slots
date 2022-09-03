// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/14/15:20
// Ver : 1.0.0
// Description : PaymentInfoModal.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule 
{
    public class PaymentInfoModal : Model<PaymentBaseInfo>
    {
        public PaymentInfoModal()
            : base(ModelType.TYPE_PAYMENT_INFO)
        {
        }

        public override async Task FetchModalDataFromServerAsync()
        {
            var cGetPaymentBaseInfo = new CGetPaymentBaseInfo();
            
            var handler = await APIManagerGameModule.Instance.SendAsync<CGetPaymentBaseInfo, SGetPaymentBaseInfo>(cGetPaymentBaseInfo);
            if (handler.ErrorCode == 0)
            {
                UpdateModelData(handler.Response.BaseInfo);   
            }
        }


        public bool IsStoreBonusCollectable()
        {
            if (modelData != null && modelData.StoreBonus != null)
            {
                var expireTime = Time.realtimeSinceStartup - lastTimeUpdateData;
                return modelData.StoreBonus.CountdownTime - expireTime <= 0;
            }

            return false;
        }
        
        public List<string> GetConsumableProductIds()
        {
            if (modelData != null && modelData.ItemList.Count > 0)
            {
                var productIdList = new List<string>(modelData.ItemList.Count);

                for (var i = 0; i < modelData.ItemList.Count; i++)
                {
#if UNITY_ANDROID
                    productIdList.Add(modelData.ItemList[i].ProductIdAndroid);
#else
                    productIdList.Add(modelData.ItemList[i].ProductIdIos);
#endif
                }

                return productIdList;
            }  
            
            return new List<string>()
            {
                "com.ustargames.cashcraze.shop.coin1",
                "com.ustargames.cashcraze.shop.coin2",
                "com.ustargames.cashcraze.shop.coin3",
                "com.ustargames.cashcraze.shop.coin4",
                "com.ustargames.cashcraze.shop.coin5",
                "com.ustargames.cashcraze.shop.coin6",
            };
        }
    }
}