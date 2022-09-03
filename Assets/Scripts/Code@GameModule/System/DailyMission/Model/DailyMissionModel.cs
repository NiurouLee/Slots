//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-24 19:11
//  Ver : 1.0.0
//  Description : DailyMissionModel.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;

namespace GameModule
{
    public class DailyMissionModel: Model<DailyMission>
    {
        private List<MissionController> _honorMissions;
        private List<MissionController> _normalMissions;
        private RewardController _missionReward;
        private List<RewardController> _stageReward;
        private MissionController _currentMission;
        private bool _isHonorMission;
        private bool _isNormalMission;
        public bool IsHonorMission => _isHonorMission;
        public bool IsNormalMission => _isNormalMission;
        
        public DailyMissionModel() : base(ModelType.TYPE_DAILY_MISSION)
        {
            _honorMissions = new List<MissionController>();
            _normalMissions = new List<MissionController>();
            _stageReward = new List<RewardController>();
        }
        
        public override async Task FetchModalDataFromServerAsync()
        {
            await GetModalDataFromServer();
        }
        
        public async Task<SGetDailyMission> GetModalDataFromServer()
        {
            CGetDailyMission cGetDailyMission = new CGetDailyMission();
            var sGetDailyMission =
                await APIManagerGameModule.Instance.SendAsync<CGetDailyMission, SGetDailyMission>(cGetDailyMission);

            if (sGetDailyMission.ErrorCode == 0)
            {
                UpdateDailyMissionData(sGetDailyMission.Response.DailyMission);
                return sGetDailyMission.Response;
            }
            else
            {
                XDebug.LogError("GetDailyMissionResponseError:" + sGetDailyMission.ErrorInfo);
            }
            return null;
        }

        public async Task RefreshModeDataFromServer()
        {
            CRefreshDailyMission cRefreshDailyMission = new CRefreshDailyMission();
            var sRefreshDailyMission =
                await APIManagerGameModule.Instance.SendAsync<CRefreshDailyMission, SRefreshDailyMission>(cRefreshDailyMission);

            if (sRefreshDailyMission.ErrorCode == 0)
            {
                UpdateDailyMissionData(sRefreshDailyMission.Response.DailyMission);
                EventBus.Dispatch(new EventDailyMissionUpdate());
            }
            else
            {
                XDebug.LogError("RefreshDailyMissionResponseError:" + sRefreshDailyMission.ErrorInfo);
            }
        }
        
        public async Task CompleteMissionNow(int index, bool isHonor=false, Action callback = null)
        {
            CCompleteNowDailyMission cCompleteNowDailyMission = new CCompleteNowDailyMission();
            cCompleteNowDailyMission.Index = (uint)index;
            cCompleteNowDailyMission.IsHonor = isHonor;
            var sCompleteNowDailyMission =
                await APIManagerGameModule.Instance.SendAsync<CCompleteNowDailyMission, SCompleteNowDailyMission>(cCompleteNowDailyMission);

            if (sCompleteNowDailyMission.ErrorCode == 0)
            {
                GetRewards(sCompleteNowDailyMission.Response.MissionReward, sCompleteNowDailyMission.Response.StageRewards);
                UpdateDailyMissionData(sCompleteNowDailyMission.Response.DailyMission);
                EventBus.Dispatch(new EventUserProfileUpdate(sCompleteNowDailyMission.Response.UserProfile));
                callback?.Invoke();
            }
            else
            {
                XDebug.LogError("CompleteDailyMissionResponseError:" + sCompleteNowDailyMission.ErrorInfo);
            }
        }

        public void UpdateDailyMissionData(DailyMission dailyMission)
        {
            UpdateModelData(dailyMission);
            RefreshMissionController();
        }
        public async Task ClaimMission(int index, bool isHonor=false, Action callback = null)
        {
            CCollectDailyMission cCollectDailyMission = new CCollectDailyMission();
            cCollectDailyMission.Index = (uint)index;
            cCollectDailyMission.IsHonor = isHonor;
            var sCollectDailyMission =
                await APIManagerGameModule.Instance.SendAsync<CCollectDailyMission, SCollectDailyMission>(cCollectDailyMission);

            if (sCollectDailyMission.ErrorCode == 0)
            {
                GetRewards(sCollectDailyMission.Response.MissionReward, sCollectDailyMission.Response.StageRewards);
                UpdateDailyMissionData(sCollectDailyMission.Response.DailyMission);
                EventBus.Dispatch(new EventUserProfileUpdate(sCollectDailyMission.Response.UserProfile));
                callback?.Invoke();
            }
            else
            {
                XDebug.LogError("CollectDailyMissionResponseError:" + sCollectDailyMission.ErrorInfo);
            }
        }

        private void GetRewards(Reward reward, RepeatedField<Reward> stageRewards)
        {
            _stageReward.Clear();
            _missionReward = new RewardController(reward);
            for (int i = 0; i < stageRewards.Count; i++)
            {
                _stageReward.Add(new RewardController(stageRewards[i]));
            }
        }

        private void RefreshMissionController()
        {
            if (IsLocked) return;
            _honorMissions.Clear();
            _normalMissions.Clear();
            var nMissionCount = modelData.HonorMissions.Count;
            for (int i = 0; i < nMissionCount; i++)
            {
                _honorMissions.Add(MissionFactory.CreateMission(modelData.HonorMissions[i]));
            }
            nMissionCount = modelData.NormalMissions.Count;
            for (int i = 0; i < nMissionCount; i++)
            {
                _normalMissions.Add(MissionFactory.CreateMission(modelData.NormalMissions[i]));
            }
            GetCurrentMission();
        }
        private void GetCurrentMission()
        {
            _currentMission = null;
            _isNormalMission = false;
            _isHonorMission = false;
            for (int i = 0; i < 3; i++)
            {
                var mission = GetNormalMission(i);
                if (mission != null && !mission.IsClaimed())
                {
                    _isNormalMission = true;
                    _currentMission = mission;
                    break;
                }
            }

            if (_currentMission == null)
            {
                _isHonorMission = true;
                _currentMission = GetHonorMission();
            }
        }
        
        public MissionController GetHonorMission()
        {
            if (_honorMissions.Count > 0)
            {
                return _honorMissions[_honorMissions.Count-1];
            }
            return null;
        }

        public MissionController GetNormalMission(int index)
        {
            if (_normalMissions.Count > 0)
            {
                return _normalMissions[index];
            }
            return null;
        }

        public long GetHornorMissionTimeLeft()
        {
            return (long) modelData.HonorTimestampLeft -
                   (long) TimeSpan.FromSeconds(Time.realtimeSinceStartup - lastTimeUpdateData).TotalSeconds;
        }
        
        public long GetNormalMissionTimeLeft()
        {
            return (long) modelData.NormalTimestampLeft -
                   (long) TimeSpan.FromSeconds(Time.realtimeSinceStartup - lastTimeUpdateData).TotalSeconds;
        }
        
        public long GetStageTimeLeft()
        {
            return (long) modelData.StageTimestampLeft -
                   (long) TimeSpan.FromSeconds(Time.realtimeSinceStartup - lastTimeUpdateData).TotalSeconds;
        }
        
        public long GetCurrentStagePoint()
        {
            return modelData.StagePoint;
        }

        public DailyMission.Types.Stage GetStateItem(int index)
        {
            if (modelData.Stages.Count <= index)
            {
                return null;
            }
            return modelData.Stages[index];
        }
        
        public long GetNeedStagePoint()
        {
            return modelData.StagePointTotal;
        }

        public bool IsLocked => modelData == null || modelData.IsLocked;
        public uint UnlockLevel => modelData.UnlockLevel;

        public int GetFinishedCount()
        {
            int count = 0;
            if (IsHonorMissionFinished())
            {
                count++;
            }
            if (IsNormalMissionFinished())
            {
                count++;
            }
            return count;
        }

        public RewardController GetMissionReward()
        {
            return _missionReward;
        }
        public List<RewardController> GetStageRewards()
        {
            return _stageReward;
        }
        public MissionController CurrentMission => _currentMission;

        public bool IsHonorMissionFinished()
        {
            var honorMission = GetHonorMission();
            if (honorMission != null && honorMission.IsFinish() && !honorMission.IsClaimed())
            {
                return true;
            }
            return false;
        }

        public bool IsNormalMissionFinished()
        {
            var nMissionCount = modelData != null ? modelData.NormalMissions.Count : 0;
            for (int i = 0; i < nMissionCount; i++)
            {
                var mission = modelData.NormalMissions[i];
                if (mission != null && mission.Finished && !mission.Collected)
                {
                    return true;
                }
            }
            return false;
        }
    }
}