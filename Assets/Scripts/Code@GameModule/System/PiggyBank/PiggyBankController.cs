//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-16 15:05
//  Ver : 1.0.0
//  Description : PiggyBankController.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class PiggyBankController: LogicController
    {
        private PiggyBankModel _model;
        public PiggyBankController(Client client):base(client)
        {

        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new PiggyBankModel();
            SubscribeEvent<EventUpdateExp>(OnUpdateUserExp);
            SubscribeEvent<EventPiggyConsumeComplete>(OnPiggyConsumeComplete);
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetPiggyBank != null)
            {
                _model.UpdatePiggyData(sGetInfoBeforeEnterLobby.SGetPiggyBank);
                beforeEnterLobbyServerDataReceived = true;
            }
            base.OnGetInfoBeforeEnterLobby(sGetInfoBeforeEnterLobby);
        }
 
        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await GetPiggyData();
            }

            finishCallback?.Invoke();
        }
        
        public async Task<SGetPiggyBank> GetPiggyData()
        {
            return await _model.GetModalDataFromServer();
        }

        private async void OnPiggyConsumeComplete(EventPiggyConsumeComplete evt)
        {
            await _model.FetchModalDataFromServerAsync();
            EventBus.Dispatch(new EventPiggyBankUpdated());
        }
        
        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            UpdatPiggyBankSpin(GetSystemData<PiggyBankResult>(evt.systemContent,"PiggyBankResult"));
        }
        
        private async void OnUpdateUserExp(EventUpdateExp evtUpdateExp)
        {
            var userController = Client.Get<UserController>();
            var userLevelInfo = userController.GetUserLevelInfo();
            if (IsLocked && userLevelInfo.LevelChanged && UnlockLevel <= userController.GetUserLevel())
            {
                await GetPiggyData();
                EventBus.Dispatch(new EventPiggyUnLock());
            }
        }

        public void UpdatPiggyBankSpin(PiggyBankResult piggyBankResult)
        {
            if (piggyBankResult == null) return;
            if (_model.IsLocked) return;
            _model.UpdatePiggyBankSpin(piggyBankResult);
        }

        public bool IsLocked => _model.IsLocked;
        public bool IsPiggyFull => _model.IsPiggyFull;
        public ulong UnlockLevel => _model.LimitUserLevel;
        public ulong CurrentCoins => _model.CurrentCoins;
        public ulong FirstBonus => _model.FirstBonus;
        public ShopItemConfig ShopItemConfig => _model.ShopItemConfig;
    }
}