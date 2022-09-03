// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/17/11:06
// Ver : 1.0.0
// Description : SeasonQuest.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using System;
using DragonU3DSDK.Network.API.Protocol;
using UnityEngine;

namespace GameModule
{
    public class SeasonQuestController : LogicController
    {
        private SeasonQuestModel _model;

        private SSeasonQuestLeaderboard _sSeasonQuestLeaderboard = null;

        private int _rankChangeAmount = 0;
        private int _lastRank = -1;

        private CancelableCallback _requestRewardCallback;
        private float questEndTime = -1;

        public SeasonQuestController(Client client)
            : base(client)
        {
        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new SeasonQuestModel();
        }


        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            SubscribeEvent<EventOnCollectSystemWidget>(OnEventCollectSystemWidget,
                HandlerPrioritySystemCollectWidget.SeasonQuestWidget);

            SubscribeEvent<EventPreNoticeLevelChanged>(OnEventPreNoticeLevelChanged);
            // SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.SeasonQuest);
        }

        public bool seasonQuestNewUnlocked = false;

        protected async void OnEventPreNoticeLevelChanged(EventPreNoticeLevelChanged evt)
        {
            if (IsLocked())
            {
                if (Client.Get<UserController>().GetUserLevel() >= GetUnlockLevel())
                {
                    if (Client.Get<NewBieQuestController>().IsQuestFinished() ||
                        Client.Get<NewBieQuestController>().IsTimeOut())
                    {
                        await RefreshSeasonQuestData();
                        if (!IsLocked() && !IsTimeOut())
                        {
                            seasonQuestNewUnlocked = true;
                            return;
                        }
                    }
                }
            }

            seasonQuestNewUnlocked = false;
        }

        public override async void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetSeasonQuest != null)
            {
                _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetSeasonQuest.SeasonQuest);
                beforeEnterLobbyServerDataReceived = true;
                await _model.CheckNeedRefresh();
                UpdateRefreshSeasonQuestAction();
            }
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await _model.FetchModalDataFromServerAsync();
                UpdateRefreshSeasonQuestAction();
            }

            finishCallback?.Invoke();
        }

        public void UpdateRefreshSeasonQuestAction()
        {
            if (IsLocked() || IsTimeOut())
            {
                if (_requestRewardCallback != null)
                {
                    _requestRewardCallback.CancelCallback();
                    _requestRewardCallback = null;
                }

                return;
            }

            var countDown = _model.GetQuestCountDown();
            var endTime = Time.realtimeSinceStartup + countDown + 5;

            if ((questEndTime < 0 && endTime > 0) || Math.Abs(endTime - questEndTime) > 5)
            {
                questEndTime = endTime;

                if (_requestRewardCallback != null)
                {
                    _requestRewardCallback.CancelCallback();
                }

                _requestRewardCallback = WaitForSeconds(countDown + 5, async () =>
                {
                    XDebug.Log("[[ShowOnExceptionHandler]] FetchModalDataToTriggerServerSendRankReward");
                    await _model.FetchModalDataFromServerAsync();
                });
            }
        }

        protected async void OnEventCollectSystemWidget(Action handleEndAction, EventOnCollectSystemWidget evt,
            IEventHandlerScheduler eventHandlerScheduler)
        {
            if (IsLocked() || IsTimeOut())
            {
                handleEndAction.Invoke();
                return;
            }

            var questWidget = await View.CreateView<SeasonQuestWidget>();
            evt.viewController.AddSystemWidget(questWidget);
            handleEndAction.Invoke();
        }

        public bool IsTimeOut()
        {
            return GetQuestCountDown() <= 0;
        }

        public bool IsLocked()
        {
            return _model.IsLocked();
        }

        // protected async void OnQuestUnlock(EventSeasonQuestUnlock evt)
        // {
        //     var userLevel = Client.Get<UserController>().GetUserLevel();
        //     if(userLevel >= _model.GetUnlockLevel() && IsLocked())
        //     { 
        //         await _model.FetchModalDataFromServer();
        //         
        //         PopupStack.ShowPopupNoWait<QuestUnlockPopup>();
        //         
        //         var questWidget = await View.CreateView<QuestWidget>();
        //         
        //         EventBus.Dispatch(new EventSystemWidgetNeedAttach(questWidget,0));
        //     }
        // }

        protected async void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING && _model.IsInitialized())
            {
                if (IsLocked() || IsTimeOut())
                {
                    handleEndCallback.Invoke();
                    return;
                }

                {
                    EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(SeasonQuestSeasonStartPopup))));
                    EventBus.Dispatch(new EventEnqueueFencePopup(handleEndCallback));
                    return;
                }
            }

            handleEndCallback.Invoke();

            // if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY)
            // {
            //     if (IsLocked())
            //     {
            //         if (Client.Get<UserController>().GetUserLevel() >= _model.GetUnlockLevel())
            //         {
            //             if (Client.Get<NewBieQuestController>().IsQuestFinished() ||
            //                 Client.Get<NewBieQuestController>().IsTimeOut())
            //             {
            //                 await _model.FetchModalDataFromServer();
            //                 UpdateRefreshSeasonQuestAction();
            //                 
            //                 if (!IsLocked() && !IsTimeOut())
            //                 {
            //                     EventBus.Dispatch(new EventSeasonQuestUnlock());
            //                     EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(SeasonQuestSeasonStartPopup))));
            //                     EventBus.Dispatch(new EventEnqueueFencePopup(handleEndCallback));
            //                     return;
            //                 }
            //             }
            //         }
            //     }
            // }

            handleEndCallback.Invoke();
        }

        public PhrasedQuest GetCurrentQuest()
        {
            return _model.GetCurrentQuest();
        }

        public uint GetCurrentQuestIndex()
        {
            return _model.GetCurrentQuestIndex();
        }

        public int GetQuestTotalCount()
        {
            return _model.GetTotalQuestCount();
        }

        public bool IsPhaseIndex(int index)
        {
            if (_model.GetTotalQuestCount() - 1 == index)
                return true;

            var quest = _model.GetQuestNode(index);
            var nextQuest = _model.GetQuestNode(index + 1);

            if (quest.Phrase != nextQuest.Phrase)
                return true;

            return false;
        }

        public PhrasedQuest GetQuest(int index)
        {
            return _model.GetQuestNode(index);
        }

        public float GetQuestCountDown()
        {
            return _model.GetQuestCountDown();
        }

        public uint GetDiamondCostBuySpeedUpBuff()
        {
            return _model.GetDiamondCostBuySpeedUpBuff();
        }

        public uint GetSpeedUpBuffDuration()
        {
            return _model.GetSpeedUpBuffDuration();
        }

        public List<MissionController> GetCurrentMission()
        {
            return _model.GetCurrentMission();
        }

        public bool NeedChooseDifficultyLevel()
        {
            return _model.NeedChooseDifficultyLevel();
        }

        public uint GetUnlockLevel()
        {
            return _model.GetUnlockLevel();
        }

        public async void ChooseDifficulty(uint difficulty, Action<bool> callback)
        {
            var send = new CChooseSeasonQuestDifficulty();

            send.Difficulty = difficulty;

            var receive = await APIManagerGameModule.Instance
                .SendAsync<CChooseSeasonQuestDifficulty, SChooseSeasonQuestDifficulty>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                _model.UpdateModelData(receive.Response.SeasonQuest);
                callback.Invoke(true);
                return;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(receive.ErrorInfo));
            }

            callback.Invoke(false);
        }

        public async void ClaimCurrentQuest(Action<bool> callback)
        {
            var send = new CCollectSeasonQuestReward();
            send.Index = _model.GetCurrentQuestIndex();

            var receive = await APIManagerGameModule.Instance
                .SendAsync<CCollectSeasonQuestReward, SCollectSeasonQuestReward>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(receive.Response.UserProfile));

                _model.UpdateModelData(receive.Response.SeasonQuest);
                callback.Invoke(true);
                return;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(receive.ErrorInfo));
            }

            callback.Invoke(false);
        }

        public async void BuySpeedUpBuff(Action<bool> callback)
        {
            var send = new CSeasonQuestBuyDoubleExp();

            var receive = await APIManagerGameModule.Instance
                .SendAsync<CSeasonQuestBuyDoubleExp, SSeasonQuestBuyDoubleExp>(send);

            if (receive.ErrorCode == ErrorCode.Success)
            {
                EventBus.Dispatch(new EventUserProfileUpdate(receive.Response.UserProfile));
                EventBus.Dispatch(new EventRefreshUserProfile());

                await Client.Get<BuffController>().SyncBufferData();

                callback.Invoke(true);
                return;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(receive.ErrorInfo));
            }

            callback.Invoke(false);
        }

        public PhrasedQuest GetPhasedRewardQuest()
        {
            return _model.GetPhasedRewardQuest();
        }

        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            var seasonQuest = GetSystemData<SeasonQuest>(evt.systemContent, "SeasonQuest");
            if (seasonQuest != null)
            {
                _model.UpdateModelData(seasonQuest);
                EventBus.Dispatch(new EventSeasonQuestDataUpdated());
            }
        }


        public async Task RefreshSeasonQuestData()
        {
            await _model.FetchModalDataFromServerAsync();
            UpdateRefreshSeasonQuestAction();
        }

        public bool IsInQuestMachineScene()
        {
            var currentQuest = GetCurrentQuest();

            if (currentQuest == null)
                return false;

            var machineScene = ViewManager.Instance.GetSceneView<MachineScene>();

            if (machineScene != null)
            {
                return machineScene.viewController.GetMachineContext().assetProvider.MachineId == currentQuest.GameId;
            }

            return false;
        }

        public int GetCurrentPhrase()
        {
            return _model.GetCurrentPhrase();
        }

        public ulong GetQuestStarCount()
        {
            return _model.GetQuestStarCount();
        }

        public async Task<SSeasonQuestLeaderboard> RefreshLeaderBoard()
        {
            var cLeaderboard = new CSeasonQuestLeaderboard();
            var sLeaderboard =
                await APIManagerGameModule.Instance.SendAsync<CSeasonQuestLeaderboard, SSeasonQuestLeaderboard>(
                    cLeaderboard);

            if (sLeaderboard.ErrorCode == 0)
            {
                _sSeasonQuestLeaderboard = sLeaderboard.Response;

                if (_lastRank > 0)
                {
                    _rankChangeAmount = (int) _sSeasonQuestLeaderboard.MyRank - (int) _lastRank;
                }
                else
                {
                    _rankChangeAmount = 0;
                }

                _lastRank = (int) _sSeasonQuestLeaderboard.MyRank;
                return _sSeasonQuestLeaderboard;
            }

            return null;
        }

        public int GetMyRank()
        {
            return _lastRank;
        }

        public int GetRankChangeAmount()
        {
            return _rankChangeAmount;
        }

        public ulong GetSeasonId()
        {
            return _model.GetSeasonId();
        }
    }
}