// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/22/11:57
// Ver : 1.0.0
// Description : PaymentHandler.cs
// ChangeLog :
// **********************************************

using System;
using DragonU3DSDK;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class PaymentHandler
    {
        public string shopType;

        public PaymentHandler(string inShopType)
        {
            shopType = inShopType;
        }
        
        public virtual void HandlePaymentSuccess(PurchaseCallbackArgs purchaseCallbackArgs, VerifyExtraInfo verifyExtraInfo)
        {
            
        }
        //
        // public void HandlePaymentFailed(PurchaseContext purchaseContext, Action handleCallback)
        // {
        // }
    }
}