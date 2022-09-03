// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/27/13:44
// Ver : 1.0.0
// Description : NextSpinPrepareProxy11001.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class NextSpinPrepareProxy11029:NextSpinPrepareProxy
    {
        public NextSpinPrepareProxy11029(MachineContext machineContext) : base(machineContext)
        {
            
        }
        
        protected override  void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<ProgressBar11029>().EnableButtonResponse(true);
                // machineContext.view.Get<JackpotPanel11026>().EnableButtonResponse(true);
            }

            if (IsFromMachineSetup())
            {
                var welcomePopup11029 = PopUpManager.Instance.ShowPopUp<WelcomePopup11029>("UIWelcome11029");
                welcomePopup11029.ShowWelcome();
            }
        }
        
        public override void OnAutoSpinStopClicked()
        {
            base.OnAutoSpinStopClicked();
            
            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                machineContext.view.Get<ProgressBar11029>().EnableButtonResponse(true);
                // machineContext.view.Get<JackpotPanel11026>().EnableButtonResponse(true);
            }
        }
        
        public override void StartNextSpin()
        {
            machineContext.view.Get<ProgressBar11029>().EnableButtonResponse(false);
            // machineContext.view.Get<JackpotPanel11026>().EnableButtonResponse(false);
            base.StartNextSpin();
        }
        
        protected override void OnBetChange()
        {
            base.OnBetChange();
            var progressBar11029 = machineContext.view.Get<ProgressBar11029>();
            progressBar11029.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0));
            // machineContext.view.Get<JackpotPanel11026>().UpdateJackpotLockState(true,false);
        }
        
        protected override void OnUnlockBetFeatureConfigChanged()
        {
            var progressBar11029 = machineContext.view.Get<ProgressBar11029>();
            progressBar11029.LockSuperFree(!machineContext.state.Get<BetState>().IsFeatureUnlocked(0),false);
            // machineContext.view.Get<JackpotPanel11026>().UpdateJackpotLockState(false,false);
        }

    }
}