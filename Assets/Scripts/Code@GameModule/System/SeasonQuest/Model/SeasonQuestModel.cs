// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/17/11:09
// Ver : 1.0.0
// Description : SeasonQuestModel.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;

namespace GameModule
{
    public class SeasonQuestModel : Model<SeasonQuest>
    {
        private PhrasedQuest _currentQuest;
        private List<MissionController> _currentMission;

        private uint _currentNodeIndex = 0;
        
        public SeasonQuestModel()
            : base(ModelType.TYPE_SEASON_QUEST)
        {
        }

        private int _featchRetryCount = 0;
        public override async Task FetchModalDataFromServerAsync()
        {
            CGetSeasonQuest cGetSeasonQuest = new CGetSeasonQuest();
            var request = await APIManagerGameModule.Instance.SendAsync<CGetSeasonQuest, SGetSeasonQuest>(cGetSeasonQuest);
            
            if (request.ErrorCode == ErrorCode.Success)
            {
                _featchRetryCount = 0;
                UpdateModelData(request.Response.SeasonQuest);
                
                await CheckNeedRefresh();
            }
            else
            {
                _featchRetryCount++;
                XDebug.Log("[[ShowOnExceptionHandler]] FetchSeasonQuestDataFailed");
                if (_featchRetryCount <= 5)
                {
                    await FetchModalDataFromServerAsync();
                }
            }
        }
        
        public override void UpdateModelData(SeasonQuest inModelData)
        {
            base.UpdateModelData(inModelData);
            SetUpCurrentMissionData();
        }

        public async Task CheckNeedRefresh()
        {
            if (modelData.SeasonId != modelData.CurrentSeasonId && modelData.CurrentSeasonId != 0)
            {
                CRefreshSeasonQuest cRefreshSeasonQuest = new CRefreshSeasonQuest();
                    
                var refreshRequest =
                    await APIManagerGameModule.Instance
                        .SendAsync<CRefreshSeasonQuest, SRefreshSeasonQuest>(cRefreshSeasonQuest);

                if (refreshRequest.ErrorCode == ErrorCode.Success)
                {
                    UpdateModelData(refreshRequest.Response.SeasonQuest);
                }
            }
        }
         
        public uint GetCurrentQuestIndex()
        {
            return _currentNodeIndex;
        }
         
        public int GetCurrentPhrase()
        {
            return (int)modelData.Phrase;
        }
        
        public bool NeedChooseDifficultyLevel()
        {
            return modelData.Difficulty == 0;
        }

        public PhrasedQuest GetPhasedRewardQuest()
        {
            for (int i = (int)_currentNodeIndex; i < modelData.Quests.Count; i++)
            {
                if (modelData.Quests[i].Phrase == modelData.Phrase + 1)
                {
                    return modelData.Quests[i - 1];
                }
            }

            return modelData.Quests[modelData.Quests.Count - 1];
        }
        
        public PhrasedQuest GetCurrentQuest()
        {
            return _currentQuest;
        }
        
        public List<MissionController> GetCurrentMission()
        {
            return _currentMission;
        }
        
        public float GetQuestCountDown()
        {
            return (float)modelData.TimestampLeft - TimeElapseSinceLastUpdate();
        }
        
        public uint GetUnlockLevel()
        {
            return  modelData.UnlockLevel;
        }
        
        public int GetTotalQuestCount()
        {
            return modelData.Quests.Count;
        }
         
        public PhrasedQuest GetQuestNode(int index)
        {
            if (modelData.Quests.Count > index)
                return modelData.Quests[index];
            return null;
        }

        
        public bool IsLocked()
        {
            return modelData == null || modelData.IsLocked;
        }

        public uint GetDiamondCostBuySpeedUpBuff()
        {
            return modelData.BuffDiamond;
        }
        
        public uint GetSpeedUpBuffDuration()
        {
            return modelData.BuffMinutes;
        }

        public ulong GetQuestStarCount()
        {
            return modelData.Stars;
        }
        
        protected void SetUpCurrentMissionData()
        {
            if (modelData.Quests.Count == 0)
                return;
            
            for (var i = 0; i < modelData.Quests.Count; i++)
            {
                if (!modelData.Quests[i].Collected)
                {
                    _currentNodeIndex = (uint)i;
                    break;
                }
            }
            
            _currentQuest = modelData.Quests[(int)_currentNodeIndex];
            
            _currentMission = new List<MissionController>();

            if (_currentQuest.Missions != null && _currentQuest.Missions.Count > 0)
            {
                for (var i = 0; i < _currentQuest.Missions.Count; i++)
                {
                    var mission = MissionFactory.CreateMission(_currentQuest.Missions[i]);
                    if (mission != null)
                        _currentMission.Add(mission);
                }
            }
        }

        public ulong GetSeasonId()
        {
            return  modelData.SeasonId;
        }
    }
}