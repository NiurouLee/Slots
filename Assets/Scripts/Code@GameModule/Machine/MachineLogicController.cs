// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/05/12:58
// Ver : 1.0.0
// Description : AnticipationConfig.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.Protocol;
using Google.ilruntime.Protobuf;
using Google.ilruntime.Protobuf.Collections;

using UnityEngine;
using IMessage = Google.ilruntime.Protobuf.IMessage;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;

namespace GameModule
{
    public class MachineLogicController : LogicController, IMachineServiceProvider
    {
        protected SEnterGame gameEnterInfo;
 
        private SListGame _gameList;

        private int validGameCount = 0;
        private List<string> validGameIdList;
        private Dictionary<string, GameConfig> gameConfigDict;
        
        public Dictionary<string, Jackpot> lobbyJackpotState;

#if !PRODUCTION_PACKAGE
        private bool _recordProtocol = false;
        private List<IMessage> _messageList;
        private bool _useRecordMessage = false;
#endif
        
        private bool _newBetUnlocked = false;
        
        private Dictionary<string,long> gameNeedDownloadSizeInfo;
        private Dictionary<string, AssetDependenciesDownloadOperation> downloadingOperations;

        private Dictionary<string, ulong> _lastUsedBet;
        
        public string LastGameId { get; private set; }

        public bool p1AssetsLoadingComplete = false;
        public string SpinGuide
        {
            get;
            protected set;
        }

        public MachineLogicController(Client client)
            : base(client)
        {
        }

        protected override void Initialization()
        {
            _lastUsedBet = new Dictionary<string, ulong>();
            base.Initialization();
        }

        protected override void SubscribeEvents()
        {
            SubscribeEvent<EventRequestEnterGame>(OnRequestEnterGame);
            SubscribeEvent<EventPreNoticeLevelChanged>(OnPreNoticeLevelChanged);
            SubscribeEvent<EventLoadingP1AssetComplete>(OnEventLoadingP1AssetComplete);
        }

        protected  void OnEventLoadingP1AssetComplete(EventLoadingP1AssetComplete evt)
        {
            p1AssetsLoadingComplete = true;

            if (validGameIdList != null && validGameIdList.Count > 0)
            {
                InitNeedDownloadSizeInfo();
            }
        }

        protected void OnPreNoticeLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (evt.levelUpInfo.MaxBet > 0)
            {
                _newBetUnlocked = true;
            }
        }
        
        protected async void OnRequestEnterGame(EventRequestEnterGame evt)
        {
            var machineId = evt.machineId;
             
            CEnterGame enterGame = new CEnterGame();
            enterGame.GameId = machineId;
            enterGame.GameMode = GameMode.Easy;

            LastGameId = enterGame.GameId;

            var switchActionId = ViewManager.SwitchActionId;
            
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
            
            // SpinDataRecord.LoadRecord( "GameRecord" + machineId);
            
            if (SpinDataRecord.usingRecord)
            {
                gameEnterInfo = SpinDataRecord.GetRecord<SEnterGame>();
                if (gameEnterInfo != null)
                {
                    if (gameEnterInfo.GameConfigs.Count>0)
                    {
                        string[] cheatId = gameEnterInfo.GameConfigs[0].CheatIds.array;
                        UIDebuggerElementCheat.Instance.RefreshGameCheatId(cheatId);   
                    }
                    
                    await WaitNFrame(2);
                    
                    EventBus.Dispatch(new EventSceneSwitchMask(SwitchMask.MASK_SERVER_READY, switchActionId));
                    return;
                }
            }
#endif
            var enterGameInfo = await APIManagerGameModule.Instance.SendAsync<CEnterGame, SEnterGame>(enterGame);

            if (enterGameInfo.ErrorCode == ErrorCode.Success)
            {
                //XDebug.Log(enterGameInfo.Response.GameConfigs[0].CheatIds.ToString());
                gameEnterInfo = enterGameInfo.Response;
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                if (enterGameInfo.Response.GameConfigs.Count>0)
                {
                    string[] cheatId = enterGameInfo.Response.GameConfigs[0].CheatIds.array;
                    UIDebuggerElementCheat.Instance.RefreshGameCheatId(cheatId);   
                }

                if (SpinDataRecord.isRecording)
                {
                    SpinDataRecord.Record(gameEnterInfo);
                }
#endif
                EventBus.Dispatch(new EventSceneSwitchMask(SwitchMask.MASK_SERVER_READY, switchActionId));
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(enterGameInfo.ErrorInfo));
            }
        }
        
        public bool  IsBetListNeedUpdate()
        {
            return _newBetUnlocked;
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SListGame != null)
            {
                _gameList = sGetInfoBeforeEnterLobby.SListGame;
                beforeEnterLobbyServerDataReceived = true;
                SyncMachineInfo();
            }
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await GetListNameAsync();
            }
            finishCallback?.Invoke();
        }
        
        public async Task GetListNameAsync()
        {
            CListGame listGame = new CListGame();
            var request = await APIManagerGameModule.Instance.SendAsync<CListGame, SListGame>(listGame);

            if (request.ErrorCode == ErrorCode.Success)
            {
                _gameList = request.Response;
                SyncMachineInfo();
            }
        }

        protected void SyncMachineInfo()
        {
            validGameIdList = new List<string>();

            gameConfigDict = new Dictionary<string, GameConfig>();

            lobbyJackpotState = new Dictionary<string, Jackpot>();

            for (var i = 0; i < _gameList.Games.count; i++)
            {
                string gameId = _gameList.Games[i].GameId;

                if (Type.GetType($"GameModule.MachineContextBuilder{gameId}") != null || _gameList.Games[i].FlagComingSoon)
                {
                    validGameCount++;
                 
                    validGameIdList.Add(gameId);

                    gameConfigDict.Add(gameId, _gameList.Games[i]);
                     
                    //-----------JackpotState值服务器只下发了最大档------------------------
                    if (_gameList.Games[i].JackpotConfigs.Count > 0)
                    {
                        var jackpotConfig =
                            _gameList.Games[i].JackpotConfigs[_gameList.Games[i].JackpotConfigs.Count - 1];
                        var jackpotState = _gameList.Games[i].JackpotState[0];

                        var jackpot = new Jackpot(jackpotConfig, jackpotState);

                        lobbyJackpotState.Add(gameId, jackpot);
                    }

                    XDebug.Log("ServerGameList:" + gameId);
                }
            }

            if(p1AssetsLoadingComplete)
                InitNeedDownloadSizeInfo();
        }

      
        public ulong GetLobbyJackpotValue(string gameId)
        {
            if (lobbyJackpotState.ContainsKey(gameId))
            {
                var betList = GetAvailableBetList(gameConfigDict[gameId].Bets);
                return lobbyJackpotState[gameId].GetJackpotValue(betList[betList.Count - 1]);
            }

            return 0;
        }

        public void InitNeedDownloadSizeInfo()
        {
            gameNeedDownloadSizeInfo = new Dictionary<string, long>();

            var opCount = gameConfigDict.Keys.Count;
            foreach (var gameInfo in gameConfigDict)
            {
                if (gameInfo.Value.FlagComingSoon)
                {
                    gameNeedDownloadSizeInfo[gameInfo.Key] = 0;
                    continue;
                }
#if UNITY_EDITOR
              gameNeedDownloadSizeInfo[gameInfo.Key] = 0;
#else
              //gameNeedDownloadSizeInfo[gameInfo.Key] = await AssetHelper.GetNeedDownloadSize("Machine" + gameInfo.Value.GameId);
              var key = gameInfo.Key;
              AssetHelper.GetNeedDownloadSize("Machine" + gameInfo.Value.GameId,
                  (size) =>
                  {
                   
                      gameNeedDownloadSizeInfo[key] = size;
                      XDebug.Log($"[[ShowOnExceptionHandler]] Game:{key} NeedDownload: + {gameNeedDownloadSizeInfo[key]}");
                      
                      opCount--;
                      if (opCount == 0)
                      {
                          EventBus.Dispatch(new EventMachineDownloadSizeUpdated());
                      }  
              });
#endif
            }
        }

        public void UpdateNeedDownloadSizeInfo(string assetId)
        {
            if (gameNeedDownloadSizeInfo.ContainsKey(assetId))
                gameNeedDownloadSizeInfo[assetId] = 0;
        }

        public int GetValidMachineCount()
        {
            return validGameCount;
        }

        public string GetGameIdByIndex(int index)
        {
            if (validGameIdList != null && index < validGameCount)
                return validGameIdList[index];
            return "NotFound";
        }

        public int GetGameIndexById(string gameId)
        {
            if (validGameIdList != null)
                return validGameIdList.IndexOf(gameId);
            return 0;
        }
        
        public ulong GetUnlockLevel(string gameId)
        {
            if (gameConfigDict.ContainsKey(gameId))
                return gameConfigDict[gameId].UnlockLevel;
            return 0;
        }
        
        public bool IsMachineExist(string gameId)
        {
            return validGameIdList.Contains(gameId) && !gameConfigDict[gameId].FlagComingSoon;
        }
        
        public bool HasMachineConfig(string gameId)
        {
            return gameConfigDict.ContainsKey(gameId);
        }
        public bool HasNewFlag(string gameId)
        {
            if (gameConfigDict.ContainsKey(gameId))
                return gameConfigDict[gameId].FlagNew;
            return false;
        }

        public bool HasHotFlag(string gameId)
        {
            if (gameConfigDict.ContainsKey(gameId))
                return gameConfigDict[gameId].FlagHot;
            return false;
        }

        public bool HasComingSoonFlag(string gameId)
        {
            if (gameConfigDict.ContainsKey(gameId))
                return gameConfigDict[gameId].FlagComingSoon;
            return false;
        }

        public GameConfig GetGameConfig(string machineId)
        {
            if (gameConfigDict != null && gameConfigDict.ContainsKey(machineId))
            {
                return gameConfigDict[machineId];
            }

            return null;
        }

        public bool IsValidGameId(string machineId)
        {
            if (gameConfigDict != null && gameConfigDict.ContainsKey(machineId))
            {
                return true;
            }
            
            return false;
        }
        
        public bool IsPortraitMachine(string machineId)
        {
            if (gameConfigDict != null && gameConfigDict.ContainsKey(machineId))
            {
                return gameConfigDict[machineId].GameOrientation == GameOrientation.Portrait;
            }

            return false;
        }

        public async Task<bool> PrepareLoadingAsset(string assetId)
        {
            var waitTask = new TaskCompletionSource<bool>();

            AddWaitTask(waitTask, null);
            AssetHelper.PrepareAsset<GameObject>($"Loading{assetId}", reference =>
            {
                if (reference != null)
                {
                    reference.ReleaseOperation();
                    RemoveTask(waitTask);
                    waitTask.SetResult(true);
                }
                else
                {
                    waitTask.SetResult(false);
                }
            });
            
            return await waitTask.Task;
        }

        public RepeatedField<ulong> GetAvailableBetList(RepeatedField<GameBetConfig> bets)
        {
            var level = Client.Get<UserController>().GetUserLevel();
            _newBetUnlocked = false;
            
            for (var i = 0; i < bets.Count; i++)
            {
                if (bets[i].LevelMin <= level && bets[i].LevelMax > level)
                {
                    return bets[i].Bets;
                }
            }
            
            return bets[bets.Count - 1].Bets;
        }


        public RepeatedField<ulong> GetAvailableUnlockFeature(RepeatedField<GameUnlockConfig> unlockConfigs)
        {
            var level = Client.Get<UserController>().GetUserLevel();

            for (var i = 0; i < unlockConfigs.Count; i++)
            {
                if (unlockConfigs[i].LevelMin <= level && unlockConfigs[i].LevelMax > level)
                {
                    return unlockConfigs[i].UnlockBets;
                }
            }

            return unlockConfigs[unlockConfigs.Count - 1].UnlockBets;
        }

        public async void EnterGame(string gameId, string assetId = "", string source = "", bool allowBackButton = true)
        {
            if(string.IsNullOrEmpty(assetId))
                assetId = gameId;
            
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMachineClick, ("source",source), ("gameId",gameId));
                
            ViewManager.Instance.BlockingUserClick(true,"MachineLogicController:EnterGame");
          
            bool result = await PrepareLoadingAsset(assetId);

            if (result)
            {
                EventBus.Dispatch(new EventSwitchScene(SceneType.TYPE_MACHINE, gameId, assetId, allowBackButton,source));
            }
            else
            {
                CommonNoticePopup.ShowCommonNoticePopUp("Assets Check Failed");
            }

            ViewManager.Instance.BlockingUserClick(false,"MachineLogicController:EnterGame");
        }

        public SEnterGame GetEnterGameInfo()
        {
            return gameEnterInfo;
        }

        public bool IsBalanceSufficient(ulong totalBet)
        {
            return Client.Get<UserController>().GetCoinsCount() >= totalBet;
        }

        public async void GetSpinResult(ulong totalBet,bool isAuto,MachineContext context,Action<SSpin> spinCallback)
        {
            try
            {
                CSpin cSpin = new CSpin();
                cSpin.Bet = totalBet;
 
                SpinGuide = System.Guid.NewGuid().ToString();
                cSpin.RequestId = SpinGuide;
                cSpin.UserGroup = AdConfigManager.Instance.MetaData.GroupId.ToString();
                

#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                cSpin.CheatId = UIDebuggerElementCheat.Instance.GetActiveCheatId();
                
                if (SpinDataRecord.usingRecord)
                {
                    var sSpin = SpinDataRecord.GetRecord<SSpin>();
                    if (sSpin != null)
                    {
                        await WaitNFrame(2);
                        spinCallback.Invoke(sSpin);
                        return;
                    }
                }
#endif
                try
                {
                    string featureName = "Base";
                    var freeSpinState = context.state.Get<FreeSpinState>();
                    var reSpinState = context.state.Get<ReSpinState>();
                    if (freeSpinState.IsInFreeSpin)
                    {
                        featureName = "Free";
                    }

                    if (reSpinState.IsInRespin)
                    {
                        if (featureName == "Base")
                        {
                            featureName = "Link";
                        }
                        else
                        {
                            featureName = $"{featureName};Link";
                        }
                    }
                    BiManagerGameModule.Instance.SendSpinAction(gameEnterInfo.GameConfigs[0].GameId,
                        BiEventFortuneX.Types.SpinActionType.Spin,isAuto,totalBet,SpinGuide,
                        ("FEATURE_NAME",featureName));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                

                var infoSpin = await APIManagerGameModule.Instance.SendAsync<CSpin, SSpin>(cSpin);
 
                if (infoSpin.ErrorCode == DragonU3DSDK.Network.API.Protocol.ErrorCode.Success)
                {
                    EventBus.Dispatch(new EventUserProfileUpdate(infoSpin.Response.UserProfile));
                    EventBus.Dispatch(new EventSpinSystemContentUpdate(infoSpin.Response.SystemContent, "Spin"));
                    EventBus.Dispatch(new EventSpinSuccess());
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                    if (SpinDataRecord.isRecording)
                    {
                        SpinDataRecord.Record(infoSpin.Response);
                    }
#endif
                    spinCallback.Invoke(infoSpin.Response);
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError(infoSpin.ErrorInfo));
                    Debug.LogError(infoSpin.ErrorInfo);
                    
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
 
        public async Task<SBonusProcess> SendBonusResult(IMessage bonusPb,MachineContext context)
        {
            try
            {
                CBonusProcess bonusProcess = new CBonusProcess();
                SpinGuide = System.Guid.NewGuid().ToString();
                bonusProcess.RequestId = SpinGuide;
                bonusProcess.UserGroup = AdConfigManager.Instance.MetaData.GroupId.ToString();
                if (bonusPb != null)
                    bonusProcess.Pb = ProtocolUtils.ToAnyStruct(bonusPb);

#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                bonusProcess.CheatId = UIDebuggerElementCheat.Instance.GetActiveCheatId();
                
                if (SpinDataRecord.usingRecord)
                {
                    var sbp = SpinDataRecord.GetRecord<SBonusProcess>();
                    if (sbp != null)
                    {
                        await WaitNFrame(2);
                        return sbp;
                    }
                }
#endif
                try
                {
                    string featureName = "Bonus";
                    ulong bet = context.state.Get<BetState>().totalBet;
                    BiManagerGameModule.Instance.SendSpinAction(gameEnterInfo.GameConfigs[0].GameId,
                        BiEventFortuneX.Types.SpinActionType.Spin,false,bet,SpinGuide,
                        ("FEATURE_NAME",featureName));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
                var bonusInfo = await APIManagerGameModule.Instance.SendAsync<CBonusProcess, SBonusProcess>(bonusProcess);
                
                if (bonusInfo.ErrorCode == DragonU3DSDK.Network.API.Protocol.ErrorCode.Success)
                {
                    EventBus.Dispatch(new EventUserProfileUpdate(bonusInfo.Response.UserProfile));
                    if (bonusInfo.Response.SystemContent != null)
                    {
                        EventBus.Dispatch(new EventSpinSystemContentUpdate(bonusInfo.Response.SystemContent, "Spin"));
                    }
                    
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                    if (SpinDataRecord.isRecording)
                    {
                        SpinDataRecord.Record(bonusInfo.Response);
                    }
#endif
                    return bonusInfo.Response;
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError(bonusInfo.ErrorInfo));
                    XDebug.LogError(bonusInfo.ErrorInfo);
                }
            }
            catch (Exception e)
            {
                XDebug.LogError("Exception" + e.Message);
            }

            return null;
        }

        public async Task<SSettleProcess> SettleGameProgress()
        {
            try
            {
                CSettleProcess settleProcess = new CSettleProcess();
                settleProcess.UserGroup = AdConfigManager.Instance.MetaData.GroupId.ToString();
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                settleProcess.CheatId = UIDebuggerElementCheat.Instance.GetActiveCheatId();
                
                if (SpinDataRecord.usingRecord)
                {
                    var settleResult = SpinDataRecord.GetRecord<SSettleProcess>();
                    if (settleResult != null)
                    {
                        await WaitNFrame(2);
                        return settleResult;
                    }
                }
#endif

                var settleProcessResponse =
                    await APIManagerGameModule.Instance.SendAsync<CSettleProcess, SSettleProcess>(settleProcess);

                if (settleProcessResponse.ErrorCode == DragonU3DSDK.Network.API.Protocol.ErrorCode.Success)
                {
                    EventBus.Dispatch(new EventUserProfileUpdate(settleProcessResponse.Response.UserProfile));
                    if (settleProcessResponse.Response.SystemContent != null)
                    {
                        EventBus.Dispatch(new EventSpinSystemContentUpdate(settleProcessResponse.Response.SystemContent, "Spin"));
                    }
                    
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                    if (SpinDataRecord.isRecording)
                    {
                        SpinDataRecord.Record(settleProcessResponse.Response);
                    }
#endif
                    return settleProcessResponse.Response;
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError(settleProcessResponse.ErrorInfo));
                    Debug.LogError(settleProcessResponse.ErrorInfo);
                }
            }
            catch (Exception e)
            {
                XDebug.LogError("EXP" + e.Message);
            }

            return null;
        }
        
        public async  Task<SFulfillExtraFreeSpin> SendClaimRvExtraFreeSpin()
        {
            try
            {
                var extraFreeSpin = new CFulfillExtraFreeSpin();
          
                var claimResponse =
                    await APIManagerGameModule.Instance.SendAsync<CFulfillExtraFreeSpin, SFulfillExtraFreeSpin>(extraFreeSpin);

                if (claimResponse.ErrorCode == ErrorCode.Success)
                {
                    return claimResponse.Response;
                }
                else
                {
                    Debug.LogError(claimResponse.ErrorInfo);
                }
            }
            catch (Exception e)
            {
                XDebug.LogError("EXP" + e.Message);
            }

            return null;
        }

        public async Task<SSpecialProcess> SendSpecialProcess(string jsonData)
        {
            try
            {
                CSpecialProcess specialProcess = new CSpecialProcess();
                SpinGuide = System.Guid.NewGuid().ToString();
                specialProcess.RequestId = SpinGuide;
                specialProcess.UserGroup = AdConfigManager.Instance.MetaData.GroupId.ToString();
                if (jsonData != null)
                    specialProcess.Json = jsonData;

#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                specialProcess.CheatId = UIDebuggerElementCheat.Instance.GetActiveCheatId();
                
                if (SpinDataRecord.usingRecord)
                {
                    var sbp = SpinDataRecord.GetRecord<SSpecialProcess>();
                    if (sbp != null)
                    {
                        await WaitNFrame(2);
                        return sbp;
                    }
                }
#endif

                var specialInfo = await APIManagerGameModule.Instance.SendAsync<CSpecialProcess, SSpecialProcess>(specialProcess);
                
                if (specialInfo.ErrorCode == DragonU3DSDK.Network.API.Protocol.ErrorCode.Success)
                {
                    EventBus.Dispatch(new EventUserProfileUpdate(specialInfo.Response.UserProfile));
                    if (specialInfo.Response.SystemContent != null)
                    {
                        EventBus.Dispatch(new EventSpinSystemContentUpdate(specialInfo.Response.SystemContent, "Spin"));
                    }
                    
#if UNITY_EDITOR || !PRODUCTION_PACKAGE
                    if (SpinDataRecord.isRecording)
                    {
                        SpinDataRecord.Record(specialInfo.Response);
                    }
#endif
                    return specialInfo.Response;
                }
                else
                {
                    EventBus.Dispatch(new EventOnUnExceptedServerError(specialInfo.ErrorInfo));
                    XDebug.LogError(specialInfo.ErrorInfo);
                }
            }
            catch (Exception e)
            {
                XDebug.LogError("Exception" + e.Message);
            }

            return null;
        }

        public int GetRecommendBetLevel(List<ulong> bets, string machineId)
        {
            // var userId = Client.Get<UserController>().GetUserId();
            //
            // var totalBet = Client.Storage.GetItem($"StoreBet_{userId}_{machineId}", "0").ToULong();

            ulong totalBet = 0;

            if (_lastUsedBet.ContainsKey(machineId))
            {
                totalBet = _lastUsedBet[machineId];
            }
            else
            {
                var balance = Client.Get<UserController>().GetCoinsCount();
                totalBet = balance / ((ulong) Math.Sqrt(balance / 1000000.0) + 100);
            }

            for (var i = 1; i < bets.Count; i++)
            {
                if (bets[i] >= totalBet)
                {
                    return i;
                }
            }
            
            return bets.Count - 1;
        }

        public void StoreRecommendBetLevel(ulong totalBet, string machineId)
        {
            if (_lastUsedBet.ContainsKey(machineId))
            {
                _lastUsedBet[machineId] = totalBet;
            }
            else
            {
                _lastUsedBet.Add(machineId, totalBet);
            }
            
            // var userId = Client.Get<UserController>().GetUserId();
            // Client.Storage.SetItem($"StoreBet_{userId}_{machineId}", totalBet.ToString());
        }
        
        
        /// <summary>
        /// 检查当前关卡是否需要下载资源
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public bool NeedDownloadAsset(string assetId)
        {
            if (gameNeedDownloadSizeInfo.ContainsKey(assetId))
            {
                return gameNeedDownloadSizeInfo[assetId] > 0;
            }
            
            return true;
        }
        /// <summary>
        /// 获取正在下载的关卡资源AsyncOperation
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public AssetDependenciesDownloadOperation GetDownloadingAssetAsyncOperation(string assetId)
        {
            if (downloadingOperations !=null && downloadingOperations.ContainsKey(assetId))
            {
                return downloadingOperations[assetId];
            }

            return null;
        }
        
        /// <summary>
        /// 下载关卡资源
        /// </summary>
        /// <param name="assetId"></param>
        /// <returns></returns>
        public AssetDependenciesDownloadOperation DownloadMachineAsset(string assetId)
        {
            if (downloadingOperations == null)
            {
                downloadingOperations = new Dictionary<string, AssetDependenciesDownloadOperation>();
            }
            
            if (downloadingOperations.ContainsKey(assetId))
            {
                return downloadingOperations[assetId];
            }
            BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMachineDownloadStart, ("gameId", assetId));
            var downloadingOperation = AssetHelper.DownloadDependencies("Machine" + assetId, (operation) =>
            {
                if (operation)
                {
                    gameNeedDownloadSizeInfo[assetId] = 0;
                    downloadingOperations.Remove(assetId);
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMachineDownloadSuccess, ("gameId", assetId));
                    EventBus.Dispatch(new EventMachineAssetDownloadFinished(assetId,true));
                }
                else
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventMachineDownloadFail, ("gameId", assetId));
                    downloadingOperations.Remove(assetId);
                    EventBus.Dispatch(new EventMachineAssetDownloadFinished(assetId,false));
                }
            }, true);
 
            downloadingOperations.Add(assetId, downloadingOperation);
            
            EventBus.Dispatch(new EventStartDownloadMachineAsset(assetId));

            return downloadingOperation;
        }
        
        public override void CleanUp()
        {
            base.CleanUp();
            downloadingOperations?.Clear();
        }
    }
}