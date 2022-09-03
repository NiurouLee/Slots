// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/21/17:44
// Ver : 1.0.0
// Description : BuffController.cs
// ChangeLog :
// **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class BuffController : LogicController
    {
        private BuffDataModel _model;
        
        public BuffController(Client client)
            : base(client)
        {
        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new BuffDataModel();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetBuff);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (_model.LastTimeUpdateData == 0)
            {
                await _model.FetchModalDataFromServerAsync();
            }

            finishCallback?.Invoke();
        }
        
        public async Task SyncBufferData()
        {
            await _model.FetchModalDataFromServerAsync();
            
            EventBus.Dispatch(new EventBuffDataUpdated());
        }

        public void SetUpBuffEndHandler()
        {
            
        }

        public T GetBuff<T>() where T : ClientBuff
        {
            if (!_model.IsInitialized())
            {
                return null;
            }

            return _model.GetBuff<T>();
        }

        public bool HasBuff<T>() where T : ClientBuff
        {
            if (!_model.IsInitialized())
            {
                return false;
            }

            return _model.HasBuff<T>();
        }

        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            var serverBuff = GetSystemData<BuffTimerbonusSpin>(evt.systemContent, "BuffTimerbonusSpin");

            if (serverBuff != null)
            {
                var timerbonusSpinBuff = this.GetBuff<TimerbonusSpinBuff>();
                if(timerbonusSpinBuff != null)
                    timerbonusSpinBuff.UpdateBuff(serverBuff);
            }
        }

        protected async void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            var cashBackView =
                await View.CreateView<CashBackMachineWidget>("UICashBackTimeInSlot",
                    evt.context.MachineUICanvasTransform);
            
            cashBackView.transform.SetAsFirstSibling();
            evt.context.SubscribeDestroyEvent(cashBackView);
        }
    }
}