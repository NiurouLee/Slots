// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/11/17/17:47
// Ver : 1.0.0
// Description : GuideController.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using DragonU3DSDK.Network.API.Protocol;
using BiEventFortuneX = DragonU3DSDK.Network.API.ILProtocol.BiEventFortuneX;


namespace GameModule
{
    public class GuideController:LogicController
    {
        private GuideModel _model;
        
        public GuideController(Client client)
            :base(client)
        {
        
        }

        protected override void Initialization()
        {
            base.Initialization(); 
            _model = new GuideModel();
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd, HandlerPriorityWhenEnterLobby.Guide);
            SubscribeEvent<EventGuideFinished>(OnGuideFinished);
        }

        private async void OnGuideFinished(EventGuideFinished evt)
        {
            var guideType = evt.Guide.Type;
            var result = await CompleteGuide(evt.Guide);

            int tryCount = 0;

            //进游戏检查引导奖励如果领取时候吧，再尝试5次
            if (guideType == "Welcome")
            {
                while (!result)
                {
                    tryCount++;

                    result = await CompleteGuide(evt.Guide);

                    if (tryCount > 5)
                    {
                        EventBus.Dispatch(new EventOnUnExceptedServerError("ClaimWelcomeGuideTaskFailed"));
                        return;
                    }
                }
            }

            evt.Callback?.Invoke();
            await WaitForSeconds(2f);
            if (guideType == "Welcome" && GetWelcomeGuide() == null)
            {
                await WaitForSeconds(1.5f);
                PopupStack.ShowPopupNoWait<GuideEnterGamePopup>();   
            }
            else if (guideType == "EnterMachine" && GetEnterMachineGuide() == null)
            {
                EventBus.Dispatch(new EventLevelGuideUpdate());
            }
            else if (guideType.Contains("Times") ||
                     guideType == "ReachLevel3" ||
                     guideType == "ReachLevel4")
            {
             
                if (guideType.Contains("Times"))
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType.GameEventGuideComplete1);
                }
                else if (guideType.Contains("ReachLevel3"))
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                        .GameEventGuideCompleteReachLevel2);
                }
                else if (guideType.Contains("ReachLevel4"))
                {
                    BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                        .GameEventGuideCompleteUnlockQuest3);
                }

                EventBus.Dispatch(new EventLevelGuideUpdate());  

                var currentGuide = GetCurrentGuide();
                if (currentGuide != null)
                {
                    if (currentGuide.Type.Contains("ReachLevel3"))
                    {
                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                            .GameEventGuidePopReachLevel2);
                    }
                    else if (currentGuide.Type.Contains("ReachLevel4"))
                    {
                        BiManagerGameModule.Instance.SendGameEvent(BiEventFortuneX.Types.GameEventType
                            .GameEventGuidePopUnlockQuest3);
                    }   
                }
            }

            if (GetCurrentGuide() == null)
            {
                UnsubscribeEvent<EventSceneSwitchEnd>(OnSceneSwitchEnd);
                UnsubscribeEvent<EventGuideFinished>(OnGuideFinished);
                EventBus.Dispatch(new EventQuestUnlock());
            }
        }

        public Guide GetWelcomeGuide()
        {
            return _model.GetGuideByName("Welcome");
        }
        
        public Guide GetEnterMachineGuide()
        {
            return _model.GetGuideByName("EnterMachine");
        }

        public bool CheckNeedShowGuideSpin()
        {
            return GetEnterMachineGuide() != null;
        }

        public Guide GetGuideByName(string guideName)
        {
            return _model.GetGuideByName(guideName);
        }

        public Guide GetCurrentGuide()
        {
            return _model.GetCurrentGuide();
        }

        public string GetEnterMachineId()
        {
            return GetEnterMachineGuide().GameId;
        }

        public bool IsLevelGuideFinished()
        {
            return _model.IsGuideComplete("Times")
                   && _model.IsGuideComplete("ReachLevel3")
                   && _model.IsGuideComplete("ReachLevel4");
        }

        public async Task<bool> CompleteGuide(Guide guide)
        {
            CCompleteGuide cCompleteGuide = new CCompleteGuide();
            cCompleteGuide.Id = guide.Id;
            var sCompleteGuide =
                await APIManagerGameModule.Instance.SendAsync<CCompleteGuide, SCompleteGuide>(cCompleteGuide);
            if (sCompleteGuide.ErrorCode == ErrorCode.Success)
            {
                _model.UpdateModelData(sCompleteGuide.Response.GuideInfo);
                EventBus.Dispatch(new EventUserProfileUpdate(sCompleteGuide.Response.UserProfile));
                //EventBus.Dispatch(new EventRefreshUserProfile());
                return true;
            }

            return false;
        }
        
        public async Task<bool> UpdateGuide(Guide guide, uint step)
        {
            CUpdateGuideStep cUpdateGuideStep = new CUpdateGuideStep();
            cUpdateGuideStep.Id = guide.Id;
            cUpdateGuideStep.Step = step;

            var sUpdateGuide =
                await APIManagerGameModule.Instance.SendAsync<CUpdateGuideStep, SUpdateGuideStep>(cUpdateGuideStep);
            if (sUpdateGuide.ErrorCode == ErrorCode.Success)
            {
                _model.UpdateModelData(sUpdateGuide.Response.GuideInfo);
                return true;
            }

            return false;
        }

        public override void OnGetInfoBeforeEnterLobby(SGetInfoBeforeEnterLobby sGetInfoBeforeEnterLobby)
        {
            if (sGetInfoBeforeEnterLobby.SGetGuide != null)
            {
                _model.UpdateModelData(sGetInfoBeforeEnterLobby.SGetGuide.GuideInfo);
                beforeEnterLobbyServerDataReceived = true;
            }
        }

        public override async Task PrepareModelDataBeforeEnterLobby(Action finishCallback = null)
        {
            if (!beforeEnterLobbyServerDataReceived)
            {
                await _model.FetchModalDataFromServerAsync();
            }

            finishCallback?.Invoke();
        }

        protected async void OnSceneSwitchEnd(Action handleEndCallback, EventSceneSwitchEnd eventSceneSwitchEnd,
            IEventHandlerScheduler scheduler)
        {
            if (eventSceneSwitchEnd.lastSceneType == SceneType.TYPE_LOADING && _model.IsInitialized())
            {
                var welcomeGuide = GetWelcomeGuide();
                var enterMachineGuide = GetEnterMachineGuide();

                if (welcomeGuide == null && enterMachineGuide == null)
                {
                    handleEndCallback.Invoke();
                }

                if (welcomeGuide != null)
                {
                    PopupStack.ShowPopupNoWait<GuideWelcomePopup>();
                } 
                else if (enterMachineGuide != null)
                {
                    PopupStack.ShowPopupNoWait<GuideEnterGamePopup>();
                }
            }
            else
            {
                handleEndCallback.Invoke();
            }
        }
    }
}