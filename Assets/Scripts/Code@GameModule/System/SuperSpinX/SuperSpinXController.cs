// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/20/16:48
// Ver : 1.0.0
// Description : SuperSpinXController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class SuperSpinXController : LogicController
    {
        public SuperSpinXController(Client client)
            : base(client)
        {
            
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventPaymentFinish>(OnEventPaymentFinish);
        }

        protected async void OnEventPaymentFinish(EventPaymentFinish evt)
        {
            if (evt.verifyExtraInfo != null)
            {
                var shopItem = evt.verifyExtraInfo.Item;
                if (shopItem.SuperSpinBehind)
                {
                    var superSpinShopItemConfig = await GetSuperSpinPaymentInfo(); 
                    if (superSpinShopItemConfig != null)
                    {
                        //由于网络问题玩家可能连续购买两笔订单，导致弹出两个弹窗，所以这里先关掉之前弹出的
                    
                        if (PopupStack.HasPopup(typeof(SuperSpinXPopup)))
                        {
                            PopupStack.ClosePopup<SuperSpinXPopup>();
                        }

                        EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SuperSpinXPopup), superSpinShopItemConfig)));
                    }
                    else
                    {
                        XDebug.Log("SuperSpinX ShopItem Get Failed");
                    }
                }
            }
        }


        public async  void TestFunc()
        {
            var superSpinShopItemConfig = await GetSuperSpinPaymentInfo();
                
            if (superSpinShopItemConfig != null)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(SuperSpinXPopup), superSpinShopItemConfig)));
            }
        }

        // public void PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        // {
        //     return base.PrepareModelDataBeforeEnterLobby(finishCallback);
        //     EventPaymentFinish
        // }

        public async Task<ShopItemConfig> GetSuperSpinPaymentInfo()
        {
            CGetSuperSpinPaymentInfo c = new CGetSuperSpinPaymentInfo();

            var response = await APIManagerGameModule.Instance
                .SendAsync<CGetSuperSpinPaymentInfo, SGetSuperSpinPaymentInfo>(c);

            if (response.ErrorCode == 0)
            {
                if (response.Response.CanBuy)
                    return response.Response.ShopItem;
            }

            return null;
        }
    }
}