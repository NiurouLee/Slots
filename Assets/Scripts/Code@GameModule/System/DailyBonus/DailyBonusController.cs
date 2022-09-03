// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/23/13:54
// Ver : 1.0.0
// Description : DailyBonusController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;


namespace GameModule
{
    public class DailyBonusController : LogicController
    {
        private DailyBonusModel _model;

        
        public DailyBonusController(Client client) : base(client)
        {
        }

        protected override void Initialization()
        {
            base.Initialization();
            _model = new DailyBonusModel();
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetDailyBonus != null && sGetInfoBeforeEnterLobby.SGetDailyBonus.DailyBonus != null)
            {
                _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetDailyBonus.DailyBonus);
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

            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.DailyBonus);
        }

        //玩家等级或者VIP升级之后，都会导致奖励产生变化，所以添加一个函数，在每次打开DailyBonusCalendar的时候，刷新奖励数据
        public async Task RefreshRewardData()
        {
            await _model.FetchModalDataFromServerAsync();
        }

        protected  async void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.currentSceneType == SceneType.TYPE_LOBBY && _model.IsInitialized())
            {
                if (eventSceneSwitchEnd.lastSceneType != SceneType.TYPE_LOADING)
                {
                    await _model.FetchModalDataFromServerAsync();
                }

                if (ViewManager.Instance.IsInSwitching() || !ViewManager.Instance.InLobbyScene())
                {
                    return;
                }
                
                if (_model.CheckHasMonthRewardToClaim())
                {
                    CheckAndClaimMonthReward(() =>
                    {
                        if(_model.CheckHasDailyRewardToClaim())
                            ClaimDailyReward(handleEndCallback);
                        else
                        {
                            handleEndCallback.Invoke();
                        }
                    });
                    return;
                }
                
                if (_model.CheckHasDailyRewardToClaim())
                {
                    ClaimDailyReward(handleEndCallback);
                    return;
                }
            }

            handleEndCallback?.Invoke();
        }

        protected async void ShowDailyRewardCalendar(bool showCheckAnimation, Action finishAction)
        {
            var popup = await PopupStack.ShowPopup<DailyBonusCalendarPopup>("UIDailyBonusMain");
           
            if (showCheckAnimation)
            {
                await WaitForSeconds(0.2f);
                popup.viewController.ShowCheckAnimation();
            }

            popup.viewController.SubscribeCheckMonthRewardAction(() =>
            {
                CheckAndClaimMonthReward(finishAction);
            });

            popup.SubscribeCloseAction(() =>
            {
                if(!CheckHasMonthRewardToClaim())
                    finishAction.Invoke();
            });
        }
         
        public Reward GetDailyRewardInfo(int dayIndex)
        {
            return _model.GetDailyRewardInfo(dayIndex);
        }

        public MonthReward GetMonthRewardInfo(int stageIndex)
        {
            return _model.GetMonthRewardInfo(stageIndex);
        }

        public int GetWeekStep()
        {
            return _model.GetWeekStep();
        }
        
        public int GetMonthStep()
        {
            return _model.GetMonthStep();
        }
        
        //该函数只在CalendarPopUp 上使用
        public int GetWeekSignStep()
        {
            return (_model.GetWeekStep() - 1 + 7) % 7;
        }
        
        public string GetWatchRvExtraCoinDesc()
        {
            return _model.GetWatchRvExtraCoinDesc();
        }
        
        public int GetMonthStage()
        {
            return _model.GetMonthStage();
        }

        public long GetLeftTime()
        {
            return _model.GetLeftTime();
        }


        public bool CheckHasMonthRewardToClaim()
        {
            return _model.CheckHasMonthRewardToClaim();
        }

        public async void ClaimDailyReward(Action finishCallback)
        {
            var popup = await PopupStack.ShowPopup<DailyBonusDayRewardPopup>("UIDailyBonusDayReward");
            popup.InitRewardContent(_model.GetDailyRewardInfo((int) _model.GetWeekRewardIndex()));
            popup.SubscribeCloseClickAction(() => { ShowDailyRewardCalendar(true, finishCallback); });
        }
        
        public async void CheckAndClaimMonthReward(Action finishCallback)
        {
            if (_model.CheckHasMonthRewardToClaim())
            {
                var boxRewardPopUp = await PopupStack.ShowPopup<DailyBonusBoxRewardPopup>("UIDailyBonusBoxReward");
                boxRewardPopUp.InitPopupContent(_model.GetMonthRewardInfo(_model.GetMonthStage()), _model.GetMonthStage());
                boxRewardPopUp.SubscribeCloseAction(finishCallback);
            }
        }
        
        public async Task<SCollectDailyBonus> CollectMonthReward()
        {
            var cCollectDailyBonus = new CCollectDailyBonus();
            cCollectDailyBonus.IsMonth = true;
            var asyncHandler =
                await APIManagerGameModule.Instance.SendAsync<CCollectDailyBonus, SCollectDailyBonus>(cCollectDailyBonus);

            if (asyncHandler.ErrorCode == 0)
            {
                //协议还需要修改
                _model.UpdateModelData(asyncHandler.Response.DailyBonus);
                
                EventBus.Dispatch(new EventUserProfileUpdate(asyncHandler.Response.UserProfile));
                return asyncHandler.Response;
            }
            
            return null;
        }

        public uint GetWatchRvExtraCoinAddition()
        {
            return _model.GetWatchRvExtraCoinAddition();
        }
        
        public async Task<SCollectDailyBonus> CollectWeekReward(bool watchedRv)
        {
            var cCollectDailyBonus = new CCollectDailyBonus();
            
            cCollectDailyBonus.IsMonth = false;
            cCollectDailyBonus.IsAdWatched = watchedRv;
            cCollectDailyBonus.PlaceId = (ulong)eAdReward.DailyBonusRV;
            
            var asyncHandler = await APIManagerGameModule.Instance.SendAsync<CCollectDailyBonus, SCollectDailyBonus>(cCollectDailyBonus);
 
            if (asyncHandler.ErrorCode == 0)
            {
                //协议还需要修改
                _model.UpdateModelData(asyncHandler.Response.DailyBonus);

                EventBus.Dispatch(new EventUserProfileUpdate(asyncHandler.Response.UserProfile));
                return asyncHandler.Response;
            }
            else
            {
                EventBus.Dispatch(new EventOnUnExceptedServerError(asyncHandler.ErrorInfo));
                return null;
            }
        }
    }
}