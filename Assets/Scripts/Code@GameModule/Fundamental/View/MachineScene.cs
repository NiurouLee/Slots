// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/21/11:37
// Ver : 1.0.0
// Description : MachineScene.cs
// ChangeLog :
// **********************************************
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;

namespace GameModule
{
    public class MachineScene: SceneView<MachineSceneViewController>
    {
        public override SceneType SceneType { get; } = SceneType.TYPE_MACHINE;
        
        [ComponentBinder("UICanvas")]
        protected Canvas uiCanvas;
        
        [ComponentBinder("UICanvas")]
        protected CanvasScaler uiCanvasScaler;  
        
        [ComponentBinder("PopUpCanvas")]
        protected Canvas popUpCanvas;
        
        [ComponentBinder("PopUpCanvas")]
        protected CanvasScaler popUpCanvasScaler;
        
        public override bool IsPortraitScene
        {
            get
            {
                return Client.Get<MachineLogicController>().IsPortraitMachine(viewController.machineContext.assetProvider.AssetsId);
            }
        }

        public MachineScene()
        {
            Is3DScene = true;
        }

        public MachineScene(string address)
            : base(address, SceneType.TYPE_MACHINE)
        {
            Is3DScene = true;
        }
        
        public void InitializeView(Vector2 referenceResolution)
        {
            var mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
            uiCanvas.worldCamera = mainCamera;
            uiCanvas.sortingLayerID = SortingLayer.NameToID("UI");
            uiCanvasScaler.referenceResolution = referenceResolution;

            popUpCanvas.worldCamera = mainCamera;
            popUpCanvas.sortingLayerID = SortingLayer.NameToID("MachinePopup");
            popUpCanvas.sortingOrder = -1;
            
            popUpCanvasScaler.referenceResolution = referenceResolution;
        }
    }

    public class MachineSceneViewController : ViewController<MachineScene>
    {
        public MachineContext machineContext;

        private bool _machineNeedUpdate = true;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            SubscribeEvent<EventFirstPopupShow>(OnFirstPopupShow);
            SubscribeEvent<EventPopupClose>(OnPopupClose);
            SubscribeEvent<EventShowPayTable>(OnShowPayTable);

            SubscribeEvent<EventMusicEnabled>(OnMusicEnabled);
            SubscribeEvent<EventSoundEnabled>(OnSoundEnabled);
            SubscribeEvent<EventStopAutoSpin>(OnEventStopAutoSpin);
            SubscribeEvent<EventOnBalanceIsInsufficient>(OnBalanceIsInsufficient);

            SubscribeEvent<EventPayTableClosed>(OnPayTableClose);
            SubscribeEvent<EventPayTableShowed>(OnPayTableShow);

            SubscribeEvent<EventPauseMachine>(OnPauseMachine);
            SubscribeEvent<EventMaxBetUnlocked>(OnMaxBetUnlocked);
        }

        protected void OnMaxBetUnlocked(EventMaxBetUnlocked evt)
        {
            machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_MAX_BET_UNLOCKED);
        }
        
        protected void OnPayTableClose(EventPayTableClosed evt)
        {
            if (PopupStack.GetPopupCount() == 0)
            {
                _machineNeedUpdate = true;
            }
        }
        
        protected void OnPayTableShow(EventPayTableShowed evt)
        {
            _machineNeedUpdate = false;
            if (machineContext != null && machineContext.transform != null && !machineContext.IsPaused)
                machineContext.PauseMachine();
        }
        
        //临时写一个破产的弹窗逻辑，后续使用弹窗配置来控制这些逻辑代码
        protected void OnBalanceIsInsufficient(EventOnBalanceIsInsufficient evt)
        {
            var bannerController = Client.Get<BannerController>();
            var adv = bannerController.GetFirstTimeSpecialOfferAdv();

            if (adv != null)
            {
                bannerController.TriggerDeal(adv, ReCheckBalanceIsSufficientAndShowRv, "InsufficientBalance");
            }
            else
            {
                EventBus.Dispatch(new EventShowPopup(new PopupArgs(typeof(StorePopup), ReCheckBalanceIsSufficientAndShowRv,"BalanceInsufficient")));
            }
        }

        protected async void ReCheckBalanceIsSufficientAndShowRv()
        {
            if (!machineContext.state.Get<BetState>().IsBalanceSufficient())
            {
                if (AdController.Instance.ShouldShowRV(eAdReward.InsufficientBalanceRV,false))
                {
                    var popup = await PopupStack.ShowPopup<UIADSInsufficientBalanceNoticePopup>();
                    
                    popup.BindUserAction(ShowAnotherInsufficientBalanceRv, CheckAndShowAdRush);
                }
                else
                {
                    CheckAndShowAdRush();
                }
            }
        }
 
        protected async void ShowAnotherInsufficientBalanceRv()
        {
            if (AdController.Instance.ShouldShowRV(eAdReward.InsufficientBalanceRV,false))
            {
                string address = "UIADSCollectRewardGetMore";
                if (ViewManager.Instance.IsPortrait)
                {
                    address += "V";
                }
                
                var popup = await PopupStack.ShowPopup<UIADSInsufficientBalanceNoticePopup>(address);
                
                popup.BindUserAction(ShowAnotherInsufficientBalanceRv, CheckAndShowAdRush);
            }
            else
            {
                CheckAndShowAdRush();
            }
        }
        
        protected async void CheckAndShowAdRush()
        {
            RvAdAvailableFilter filter = new RvAdAvailableFilter();
            if (filter.IsValid(null, "AdTask"))
            {
                var rushPopup = await PopupStack.ShowPopup<AdRushPopup>();
                rushPopup.SubscribeCloseAction(ShowBankruptGift);
            }
            else
            {
                ShowBankruptGift();
            }
        }
        
        protected void ShowBankruptGift()
        {
            if (!machineContext.state.Get<BetState>().IsBalanceSufficient())
            {
                if(Client.Get<UserController>().GetCoinsCount() < 10000)
                    PopupStack.ShowPopupNoWait<UIBankruptPopup>();
            }
        }
        
        protected void OnEventStopAutoSpin(EventStopAutoSpin evt)
        {
            if (machineContext != null)
            {
                if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                {
                    machineContext.state.Get<AutoSpinState>().StopAutoOnNextSpin = true;
                }
            }
        }

        protected void OnMusicEnabled(EventMusicEnabled eventMusicEnabled)
        {
            if (machineContext != null)
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_MUSIC_PREFERENCE_STATUS_CHANGE,
                    eventMusicEnabled.enabled);
        }

        protected void OnSoundEnabled(EventSoundEnabled eventSoundEnabled)
        {
            if (machineContext != null)
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_SOUND_PREFERENCE_STATUS_CHANGE,
                    eventSoundEnabled.enabled);
        }
        
        protected  void OnShowPayTable(EventShowPayTable eventShowPayTable)
        {
            if (machineContext != null)
            {
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_UI_PAY_TABLE);
            }
        }
        protected void OnFirstPopupShow(EventFirstPopupShow firstPopupShow)
        {
            _machineNeedUpdate = false;
            if (machineContext != null && machineContext.transform != null)
                machineContext.PauseMachine();
        }
        
        protected void OnPauseMachine(EventPauseMachine evt)
        {
            _machineNeedUpdate = false;
            if (machineContext != null && machineContext.transform != null && !machineContext.IsPaused)
                machineContext.PauseMachine();
        }
        
        protected void OnPopupClose(EventPopupClose evt)
        {
            if (PopupStack.GetPopupCount() == 0 && !ViewManager.Instance.IsInSwitching())
            {
                _machineNeedUpdate = true;
            }
        }

        public async Task<bool> InitMachineContext(IMachineServiceProvider serviceProvider,
            MachineAssetProvider assetProvider)
        {
           //bool IsPortraitView = MachineConfig.IsPortraitSubject(subjectId);
           
           bool isPortraitView = Client.Get<MachineLogicController>().IsPortraitMachine(assetProvider.AssetsId);
           
           var referenceResolution = ViewResolution.referenceResolutionLandscape;

            if (isPortraitView)
            {
                referenceResolution = ViewResolution.referenceResolutionPortrait;
            }
            
            view.InitializeView(referenceResolution);
            
         
            var builderType = Type.GetType("GameModule.MachineContextBuilder" + assetProvider.AssetsId);
            if (builderType != null)
            {
                MachineContextBuilder contextBuilder =
                    (MachineContextBuilder) Activator.CreateInstance(builderType, assetProvider.AssetsId);
              
                machineContext = new MachineContext(view.transform);

                await machineContext.InitializeContext(contextBuilder, assetProvider, serviceProvider);
                
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_MUSIC_PREFERENCE_STATUS_CHANGE, PreferenceController.IsMusicEnabled());
                machineContext.DispatchInternalEvent(MachineInternalEvent.EVENT_SOUND_PREFERENCE_STATUS_CHANGE, PreferenceController.IsSoundEnabled());
 
                EnableUpdate();
                //
                 EventBus.Dispatch(new EventEnterMachineScene(machineContext));
                //
                // //存储玩家最后进入的关卡Id
                // Client.Storage.SetItem("SubjectId", subjectId);
                //
                // Log.LogStateEvent(State.EnterSlot, null, StepId.ID_ENTER_SLOT, subjectId.ToString());
                //
                // HandlerListener();
                return true;
            }
            
            return false;
        }
 
        public override void Update()
        {
            // bool needUpdate = machineNeedUpdate && !context.InternalPaused;
            //
            // if (needUpdate)
            // {
            //     if (context.ContextPaused)
            //     {
            //         context.UnPauseMachine();
            //     }

            if (_machineNeedUpdate)
            {
                if (machineContext.IsPaused)
                {
                    machineContext.UnPauseMachine();
                }
                
                machineContext?.Update();
            }
         
            //}
        }
        
        public void PauseScene()
        {
            _machineNeedUpdate = false;
            machineContext?.PauseMachine();
        }

        public override void OnViewDestroy()
        {
            machineContext?.OnMachineDestroy();
            base.OnViewDestroy();
        }
        
        public MachineContext GetMachineContext()
        {
            return machineContext;
        }

        public bool IsInSpinning()
        {
            if (machineContext != null)
            {
                var runningStep = machineContext.GetRunningStep();
                if (runningStep != null)
                {
                    return runningStep.StepType != LogicStepType.STEP_NEXT_SPIN_PREPARE 
                           || machineContext.state.Get<AutoSpinState>().IsAutoSpin;
                }
            }

            return false;
        }

        public Vector3 GetRunningWheelPosition()
        {
            var wheelsActiveState = machineContext.state.Get<WheelsActiveState>();
            var wheels = wheelsActiveState.GetRunningWheel();

            if (wheels.Count > 0)
            {
                var wheelMask = wheels[0].transform.Find("WheelMask");
                if (wheelMask != null)
                {
                    return wheelMask.position;
                }
               // return wheels[0].transform.Find("WheelMask").position;
            }
            
            return new Vector3(0, 0, 0);
        }
        
        public bool IsInAutoSpinning()
        {
            if (machineContext != null)
            {
                var runningStep = machineContext.GetRunningStep();
                if (runningStep != null)
                {
                    return machineContext.state.Get<AutoSpinState>().IsAutoSpin;
                }
            }
            return false;
        }

    }
}