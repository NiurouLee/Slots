//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-08 15:14
//  Ver : 1.0.0
//  Description : SeasonPassController.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class SeasonPassController: LogicController
    {
        private  SeasonPassModel _model;
        public bool IsLocked => _model == null || _model.IsLocked;
        public uint UnlockLevel => _model.UnlockLevel;
        public uint Season => _model.Season;
        public uint Level => _model.Level;
        public bool IsLevel100 => _model.Level >= 100;
        public ulong Exp => _model.Exp;
        public ulong ExpTotal => _model.ExpTotal;
        public bool Paid => _model.Paid;

        public Reward FinalReward => _model.FinalReward;
        public int CollectRewardCount => _model.GetCollectRewardCount();
        public RepeatedField<Reward> GetGoldenRewards() => _model.GetGoldenRewards();
        public RepeatedField<Reward> GetFreeRewards() => _model.GetFreeRewards();
        public RepeatedField<ShopItemConfig> ShopItemConfigs => _model.ShopItemConfigs;
        public Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>> FreeMissionPassRewards=>_model.FreeMissionPassRewards;
        public Dictionary<int, Google.Protobuf.Collections.RepeatedField<MissionPassReward>> GoldenMissionPassRewards=>_model.GoldenMissionPassRewards;
        public SeasonPassController(Client client):base(client)
        {
            
        }
        protected override void Initialization()
        {
            base.Initialization();
            _model = new SeasonPassModel();
            SubscribeEvent<EventUpdateExp>(OnUpdateExp);
            SubscribeEvent<EventEnterMachineScene>(OnEnterMachineScene);
            SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget, HandlerPrioritySystemCollectWidget.SeasonPassWidget);
        }

        private async void OnEnterMachineScene(EventEnterMachineScene evt)
        {
            XDebug.Log("XXX");
        }
        
        protected async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt, IEventHandlerScheduler eventHandlerScheduler)
        {
            if (IsLocked)
            {
                handleEndAction.Invoke();
                return;
            }
            
            var questWidget = await View.CreateView<SeasonPassWidget>();
            evt.viewController.AddSystemWidget(questWidget);
            handleEndAction.Invoke();
            
            // evt.context.view.Add<SeasonPassMachineView>(view.transform);
            // evt.context.view.Get<SeasonPassMachineView>().InitView(view);
        }

        
        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetMissionPass != null)
            {
                _model.UpdateSeasonPassData(sGetInfoBeforeEnterLobby.SGetMissionPass.MissionPass);
                beforeEnterLobbyServerDataReceived = true;
            }
        }

        
        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await GetSeasonPassData();
            }

            EnableUpdate(1);
            finishCallback?.Invoke();
        }
        
        public async Task<SGetMissionPassPaymentItems> FetchShopItems()
        {
            return await _model.FetchShopItems();
        }
        
        public async Task GetSeasonPassData()
        {
            await _model.FetchModalDataFromServerAsync();
        }
        public async Task RefreshSeasonPassData()
        {
            await _model.RefreshModeDataFromServer();
        }
        
        public void CollectMissionPass(uint level, bool isPaid, bool isTimed, Action<RepeatedField<Reward>, Action> action)
        {
           _model.CollectMissionPass(level,isPaid,isTimed, action);
        }
        
        public async Task CollectAllMissionPass(Action<RepeatedField<Reward>, Action> callback)
        {
            await _model.CollectAllMissionPass(callback);
        }
        
        private async void OnUpdateExp(EventUpdateExp evt)
        {
            var userController = Client.Get<UserController>();
            var userLevelInfo = userController.GetUserLevelInfo();
            if (IsLocked &&userLevelInfo.LevelChanged && userController.GetUserLevel() == UnlockLevel 
                         && !evt.updateToFull)
            {
                await GetSeasonPassData();
               
                EventBus.Dispatch(new EventSeasonPassUnlocked());
                
                var seasonPassWidget = await View.CreateView<SeasonPassWidget>();
                EventBus.Dispatch(new EventSystemWidgetNeedAttach(seasonPassWidget,0));
            }
        }
        
        public string GetLimitedMissionTimeLeft(MissionPassReward reward)
        {
            var days = TimeSpan.FromSeconds(_model.GetLimitedMissionTimeLeft(reward)).Days;
            if ( days >= 1)
            {
                return (days + 1) + " DAYS"; 
            }
            return TimeSpan.FromSeconds(Math.Max(0,_model.GetLimitedMissionTimeLeft(reward))).ToString(@"hh\:mm\:ss");
        }
        public long GetLimitedMissionTimeLeftLong(MissionPassReward reward)
        {
            return _model.GetLimitedMissionTimeLeft(reward);
        }

        public string GetSeasonPassTimeLeft()
        {
            var days = TimeSpan.FromSeconds(_model.GetSeasonPassTimeLeft()).Days;
            if ( days >= 1)
            {
                return (days + 1) + " DAYS LEFT"; 
            }
            return TimeSpan.FromSeconds(Math.Max(0,_model.GetSeasonPassTimeLeft())).ToString(@"hh\:mm\:ss");
        }
        public string GetSeasonPassTimeLeftInMachine()
        {
            var days = TimeSpan.FromSeconds(_model.GetSeasonPassTimeLeft()).Days;
            if ( days >= 1)
            {
                return (days + 1) + " DAYS"; 
            }
            return TimeSpan.FromSeconds(Math.Max(0,_model.GetSeasonPassTimeLeft())).ToString(@"hh\:mm\:ss");
        }
        
        public long GetSeasonPassTimeLeftLong()
        {
            return _model.GetSeasonPassTimeLeft();
        }

        private bool isRefreshing = false;
        public override async void Update()
        {
            base.Update();
            if (!isRefreshing && !IsLocked && GetSeasonPassTimeLeftLong() <= 0)
            {
                isRefreshing = true;
                await RefreshSeasonPassData();
                isRefreshing = false;
            }
        }
        
        private string GetRefreshingKey()
        {
            return "SeasonPass_NewSeason_" + Client.Get<UserController>().GetUserId();
        }

        public void SaveNewSeason(bool isNewSeason)
        {
            Client.Storage.SetItem(GetRefreshingKey(), isNewSeason ? "true" : "false");
        }

        public bool IsNewSeason()
        {
            return "true" == Client.Storage.GetItem(GetRefreshingKey(), "false");
        }
    }
}
