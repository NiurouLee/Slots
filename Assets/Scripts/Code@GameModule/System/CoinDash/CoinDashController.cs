// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/31/19:47
// Ver : 1.0.0
// Description : CoinDashController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class CoinDashController : LogicController
    {
        private CoinDashModel _model;

        public CoinDashController(Client client)
            : base(client)
        {
            
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetCoinDashInfo != null)
            {
                _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetCoinDashInfo.CoinDashInfo);
                beforeEnterLobbyServerDataReceived = true;
            }
            
            base.OnGetInfoBeforeEnterLobby(sGetInfoBeforeEnterLobby);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await _model.FetchModalDataFromServerAsync();
            }
            finishCallback?.Invoke();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventCommonPaymentComplete>(OnEventCommonPaymentComplete);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnLevelChanged);
        }

        protected void OnLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (IsOpen())
            {
                _model.FetchModalDataFromServer();
            }
        }
        protected async void OnEventCommonPaymentComplete(EventCommonPaymentComplete evt)
        {
            if (evt.shopType == ShopType.CoinDash)
            {
                await _model.FetchModalDataFromServerAsync();
                EventBus.Dispatch(new EventCoinDashDataUpdate());
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCoindashCollect,("index", (GetActiveItemIndex()).ToString()));
            }
        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new CoinDashModel();
        }

        public bool IsOpen()
        {
            return _model.IsOpen();
        }
         
        public CoinDashInfo.Types.Goods GetCoinDashItemInfo(int index)
        {
            return _model.GetCoinDashItemInfo(index);
        } 
        
        public int GetActiveItemIndex()
        {
            return _model.GetActiveItemIndex();
        }
        
        public int GetCoinDashItemCount()
        {
            return _model.GetCoinDashItemCount();
        }

        public float GetLeftTime()
        {
            return _model.GetLeftTime();
        }
        
        public async void ClaimDashReward(uint goodsId,Action<Reward> claimCallback)
        {
            CBuyCoinDashFreeGoods c = new CBuyCoinDashFreeGoods();
            
            c.GoodsId = goodsId;
            var handler = await APIManagerGameModule.Instance.SendAsync<CBuyCoinDashFreeGoods, SBuyCoinDashFreeGoods>(c);

            if (handler.ErrorCode == 0)
            {
                _model.UpdateModelData(handler.Response.CoinDashInfo);
                
                EventBus.Dispatch(new EventUserProfileUpdate(handler.Response.UserProfile));
                BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventCoindashCollect,("index", GetActiveItemIndex().ToString()));
                claimCallback.Invoke(handler.Response.Reward);
            }
            else
            {
                claimCallback.Invoke(null);
            }
        }
    }
}