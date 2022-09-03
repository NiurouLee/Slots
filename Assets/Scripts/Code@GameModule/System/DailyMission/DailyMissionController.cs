//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-17 17:03
//  Ver : 1.0.0
//  Description : DailyMissionController.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class DailyMissionController: LogicController
    {
        private bool _isShowComplete;
        private DailyMissionModel _model;
        public DailyMissionController(Client client):base(client)
        {
            
        }
        
        protected override void Initialization()
        {
            base.Initialization();
            _model = new DailyMissionModel();
            SubscribeEvent<EventUpdateExp>(OnUpdateExp);      
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetDailyMission != null)
            {
                _model.UpdateDailyMissionData(sGetInfoBeforeEnterLobby.SGetDailyMission.DailyMission);
                beforeEnterLobbyServerDataReceived = true;
            }
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await _model.FetchModalDataFromServerAsync();
            }
            
            await CheckNeedRefresh();
            EnableUpdate(1);
            finishCallback?.Invoke();
        }
        
        public async Task<SGetDailyMission> GetDailyMissionData()
        {
            return await _model.GetModalDataFromServer();
        }
        
        private async void OnUpdateExp(EventUpdateExp evt)
        {
            var userController = Client.Get<UserController>();
            var userLevelInfo = userController.GetUserLevelInfo();
            if (IsLocked &&userLevelInfo.LevelChanged && userController.GetUserLevel() == UnlockLevel)
            {
                await GetDailyMissionData();
                EventBus.Dispatch(new EventDailyMissionUnLock());
            }
        }

        public void UpdateDailyMissionState(DailyMission dailyMission)
        {
            if (dailyMission == null) return;
            if (_model.IsLocked) return;
            _model.UpdateDailyMissionData(dailyMission);
        }
        
        public MissionController GetHonorMission()
        {
            return _model.GetHonorMission();
        }

        public async Task CompleteMissionNow(int index, bool isHonor=false, Action callback = null)
        {
            await ViewManager.Instance.DelayShowScreenLoadingView();
            await _model.CompleteMissionNow(index, isHonor, callback);
            ViewManager.Instance.HideScreenLoadingView();
            IsShowComplete = false;
        }
        
        public async Task ClaimMission(int index, bool isHonor=false, Action callback = null)
        {
            await ViewManager.Instance.DelayShowScreenLoadingView();
            await _model.ClaimMission(index, isHonor, callback);
            ViewManager.Instance.HideScreenLoadingView();
            IsShowComplete = false;
        }

        public MissionController GetNormalMission(int index)
        {
            return _model.GetNormalMission(index);
        }
        
        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            UpdateDailyMissionState(GetSystemData<DailyMission>(evt.systemContent,"DailyMission"));
        }


        private bool _waitRefresh = false;
        public override async void Update()
        {
            if (!_waitRefresh)
            {
                _waitRefresh = true;
                await CheckNeedRefresh();
                _waitRefresh = false;
            }
        }

        private async Task CheckNeedRefresh()
        {
            if (_model == null) return;
            if (_model.IsLocked) return;
            if (_model.GetNormalMissionTimeLeft() <= 0 || 
                _model.GetHornorMissionTimeLeft() <= 0 || 
                _model.GetStageTimeLeft() <= 0)
            {
                await _model.RefreshModeDataFromServer();
            }
        }
        
        public string GetHornorMissionTimeLeft()
        {
            var days = TimeSpan.FromSeconds(_model.GetHornorMissionTimeLeft()).Days;
            if ( days >= 1)
            {
                return (days + 1) + " DAYS LEFT"; 
            }
            return TimeSpan.FromSeconds(Math.Max(0,_model.GetHornorMissionTimeLeft())).ToString(@"hh\:mm\:ss");
        }
        
        public string GetNormalMissionTimeLeft()
        {
            var days = TimeSpan.FromSeconds(_model.GetNormalMissionTimeLeft()).Days;
            if ( days >= 1)
            {
                return (days + 1) + " DAYS LEFT"; 
            }
            return TimeSpan.FromSeconds(Math.Max(0,_model.GetNormalMissionTimeLeft())).ToString(@"hh\:mm\:ss");
        }
        
        public string GetStageTimeLeft()
        {
            var days = TimeSpan.FromSeconds(_model.GetStageTimeLeft()).Days;
            if ( days >= 1)
            {
                return (days + 1) + " DAYS LEFT"; 
            }
            return TimeSpan.FromSeconds(Math.Max(0,_model.GetStageTimeLeft())).ToString(@"hh\:mm\:ss");
        }
        
        public long StatePoint => _model.GetCurrentStagePoint();
        
        
        public long StagePointTotal => _model.GetNeedStagePoint();
        

        public RewardController GetMissionReward()
        {
            return _model.GetMissionReward();
        }
        public List<RewardController> GetStageRewards()
        {
            return _model.GetStageRewards();
        }

        public int GetFinishedMissionCount()
        {
            return _model.GetFinishedCount();
        }

        public bool IsLocked => _model == null || _model.IsLocked;
        public uint UnlockLevel => _model.UnlockLevel;

        public DailyMission.Types.Stage GetStateItem(int index)
        {
            return _model.GetStateItem(index);
        }

        public bool IsShowComplete
        {
            get
            {
                return _isShowComplete;
            }
            set
            {
                _isShowComplete = value;
            }
        }

        public int GetNormalMissionFinishedCount()
        {
            var count = 0;
            for (int i = 0; i < 3; i++)
            {
                var mission = GetNormalMission(i);
                if (mission != null && mission.IsFinish())
                {
                    count++;
                }
            }
            return count;
        }
        public float GetStagePointProgress()
        {
            return StatePoint * 1f/
                   StagePointTotal;
        }
        public MissionController CurrentMission => _model.CurrentMission;
        public bool HasFinishedMission => _model.IsHonorMissionFinished() || _model.IsNormalMissionFinished();
        public bool IsNormalMissionFinished => _model.IsNormalMissionFinished();
    }
}