// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/14/15:44
// Ver : 1.0.0
// Description : StorePaymentHandler.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;


namespace GameModule
{
    public class StorePaymentHandler:PaymentHandler
    {
        protected float storeBonusCountDown = -1;
        protected float storeBonusSyncTime = -1;

        //商城中SuperSpinX商品是否可购买
        public bool spinXAvailable = false;
        
        public StorePaymentHandler()
            : base("store")
        {
            
        }
        
        public override async void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            var collectPopUp = await PopupStack.ShowPopup<StoreRewardCollectPopup>();

            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventStorePurchaseSuccess,
                ("paymentId", verifyExtraInfo.Item.PaymentId.ToString()),
                ("price", verifyExtraInfo.Item.Price.ToString()),
                ("productType", verifyExtraInfo.Item.ProductType));
            
            collectPopUp.viewController.SetUpViewContent(verifyExtraInfo, (callback) =>
            {
                Client.Get<IapController>()
                    .FulfillPayment(purchaseCallbackArgs, verifyExtraInfo,(success, fulfillExtraInfo, collectCallback) =>
                    {
                        callback(success, fulfillExtraInfo);

                        collectPopUp.SubscribeCloseAction(() =>
                        {
                            collectCallback?.Invoke();
                        });
                    });
            });
        }
        
        public async Task<SGetShop> GetShopInfo()
        {
            var cGetShop = new CGetShop();
            var handler = await APIManagerGameModule.Instance.SendAsync<CGetShop,SGetShop>(cGetShop);

            if (handler.ErrorCode == 0)
            {
                storeBonusSyncTime = Time.realtimeSinceStartup;
                storeBonusCountDown = handler.Response.StoreBonus.CountdownTime;

                var shopItemConfigs = handler.Response.ItemList;

                spinXAvailable = false;

                for (var i = 0; i < shopItemConfigs.Count; i++)
                {
                    if (shopItemConfigs[i].SuperSpinBehind)
                    {
                        spinXAvailable = true;
                        break;
                    }
                }
                
                return handler.Response;
            }
            else
                return null;
        }
        
        public async Task<SGetStoreBonus> ClaimStoreBonus()
        {
            var cGetStoreBonus = new CGetStoreBonus();
            var handler = await APIManagerGameModule.Instance.SendAsync<CGetStoreBonus, SGetStoreBonus>(cGetStoreBonus);

            if (handler.ErrorCode == 0)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(handler.Response.UserProfile));
                
                storeBonusSyncTime = Time.realtimeSinceStartup;
                storeBonusCountDown = handler.Response.CountdownTime;
                
                Client.Get<IapController>().UpdateStoreBonusCountDown();
                
                return handler.Response;
            }

            return null;
        }

        public async Task<bool> ClaimGiftBox()
        {
            var cGetGiftBox = new CGetGiftBox();
            var handler = await APIManagerGameModule.Instance.SendAsync<CGetGiftBox, SGetGiftBox>(cGetGiftBox);

            if (handler.ErrorCode == 0)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(handler.Response.UserProfile));

                return true;
            }
            
            return false;
        }

        public float GetStoreBonusCountDown()
        {
            if (storeBonusSyncTime < 0)
            {
                return 0;
            }
            return storeBonusCountDown - (Time.realtimeSinceStartup - storeBonusSyncTime);
        }

    //     public async void OnPaymentSuccess(PurchaseContext context)
    //     {
    //         _currentPurchaseContext = context;
    //     
    //         var collectPopUp = await PopupStack.ShowPopup<StoreRewardCollectPopup>();
    //     
    //         collectPopUp.viewController.SetUpViewContent(OnRewardCollect);
    //     }
    //     
            // public void OnRewardCollect(Product product, Action<bool> callback)
            // {
            //     Client.Get<IapController>().FulfillPayment(product, );
            // }
     }
}