// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/06/09/11:39
// Ver : 1.0.0
// Description : LevelRushController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class LevelRushController : LogicController
    {
        private LevelRushModel _levelRushModel;

        public LevelRushController(Client client) : base(client)
        {
        }


        protected override void Initialization()
        {
            base.Initialization();
            _levelRushModel = new LevelRushModel();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSpinRoundEnd>(OnSpinRoundEnd, HandlerPriorityWhenSpinEnd.LevelRush);
            SubscribeEvent<EventPerformRemoved>(OnEventPerformRemoved);
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            await _levelRushModel.FetchModalDataFromServerAsync();

            if (_levelRushModel.IsEnabled())
            {
                SetUpTimeOutAction();
            }

            finishCallback?.Invoke();
        }

        private CancelableCallback timeOutHandle;
        private uint timeOutTimeStamp = 0;

        public void SetUpTimeOutAction()
        {
            var leftTime = _levelRushModel.GetLeftTime() + 2;

            if (leftTime > 0)
            {
                if (timeOutHandle != null)
                {
                    if (timeOutTimeStamp == _levelRushModel.GetModelData().EndAt)
                    {
                        return;
                    }
                }

                if (timeOutHandle != null)
                {
                    timeOutHandle.CancelCallback();
                }

                timeOutHandle = WaitForSecondsRealTime(leftTime, () =>
                {
                    if (_levelRushModel.GetLeftTime() <= 0 && !_levelRushModel.IsAllRewardReceived())
                    {
                        EventBus.Dispatch(new EventLevelRushStateChanged());
                        EventBus.Dispatch(new EventEnqueuePopup(new PopupArgs(typeof(LevelRushFailPopup))));
                        EventBus.Dispatch(new EventEnqueueFencePopup());
                    }
                });

                timeOutTimeStamp = _levelRushModel.GetModelData().EndAt;
            }
        }

        protected Action spinRoundEndAction;

        protected void OnSpinRoundEnd(Action handleEndCallback, EventSpinRoundEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (_levelRushModel.GetReward() != null || _levelRushModel.levelRushTriggered)
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(LevelRushPopup), "AutoPopup",
                    PerformCategory.LevelRush)));
                EventBus.Dispatch(new EventLevelRushStateChanged());
                _levelRushModel.levelRushTriggered = false;
                spinRoundEndAction = handleEndCallback;
                return;
            }

            handleEndCallback.Invoke();
        }

        protected async void OnEventPerformRemoved(EventPerformRemoved eventPerformRemoved)
        {
            if (spinRoundEndAction != null)
            {
                if (!PerformInAction.CheckHasPerformInAction(PerformCategory.LevelRush))
                {
                    var activityLevelRushPass =
                        Client.Get<ActivityController>().GetDefaultActivity(ActivityType.RushPass) as Activity_LevelRushRushPass;
                    if (activityLevelRushPass != null)
                    {
                        var havePerform = await activityLevelRushPass.HasPerform();
                        if (havePerform)
                        {
                            activityLevelRushPass.ShowSpinRoundPerform();
                        }
                        else
                        {
                            spinRoundEndAction.Invoke();
                            spinRoundEndAction = null;
                        }
                    }
                    else
                    {
                        spinRoundEndAction.Invoke();
                        spinRoundEndAction = null;
                    }
                }
            }
        }

        public LevelRushGameInfo GetFreeLottoGameInfo()
        {
            var data = _levelRushModel.GetModelData();
            if (data != null && data.FreeGamesCanPlay.Count > 0)
            {
                return data.FreeGamesCanPlay[data.FreeGamesCanPlay.Count - 1];
            }

            return null;
        }

        public string GetLevelRushTopPanelDescText()
        {
            if (_levelRushModel.IsInitialized())
            {
                var targetLevel = _levelRushModel.GetModelData().LevelTarget;

                var leftLevel = (long) targetLevel - (long) Client.Get<UserController>().GetUserLevel();

                if (leftLevel <= 0)
                {
                    return "";
                }

                if (leftLevel > 1)
                {
                    return leftLevel + " LEVELS LEFT";
                }

                return 1 + " LEVEL LEFT";
            }

            return "";
        }

        protected override void OnSpinSystemContentUpdate(EventSpinSystemContentUpdate evt)
        {
            if (evt.systemContent != null)
            {
                var levelRushPopupInfo = GetSystemData<LevelRushPopupInfo>(evt.systemContent, "LevelRushPopupInfo");

                if (levelRushPopupInfo != null)
                {
                    XDebug.LogOnExceptionHandler("LevelRushDataUpdated" + Client.Get<UserController>().GetUserLevel());

                    _levelRushModel.UpdateModelData(levelRushPopupInfo);

                    SetUpTimeOutAction();
                }
            }

            base.OnSpinSystemContentUpdate(evt);
        }

        public float GetLevelRushLeftTime()
        {
            return _levelRushModel.GetLeftTime();
        }

        public bool IsLevelRushEnabled()
        {
            return _levelRushModel.IsEnabled();
        }

        public uint GetTargetLevel()
        {
            return _levelRushModel.GetTargetLevel();
        }

        public LevelRushPopupInfo.Types.LevelRewardInfo GetRewardInfo(int index)
        {
            return _levelRushModel.GetRewardInfo(index);
        }

        public Reward GetReward()
        {
            return _levelRushModel.GetReward();
        }

        public void OnRewardClaimFinished()
        {
            if (_levelRushModel.GetTargetLevel() <= Client.Get<UserController>().GetUserLevel())
            {
                if (timeOutHandle != null)
                {
                    timeOutHandle.CancelCallback();
                }
            }

            _levelRushModel.OnRewardClaimFinished();

            if (!IsLevelRushEnabled())
            {
                EventBus.Dispatch(new EventLevelRushStateChanged());
            }
        }

        public int GetRewardNodeIndex()
        {
            return _levelRushModel.rewardIndex;
        }

        public ulong GetFreeWinUpTo()
        {
            if (_levelRushModel.GetModelData() != null)
                return _levelRushModel.GetModelData().FreeGamesCoinUpTo;
            return 0;
        }
    }
}