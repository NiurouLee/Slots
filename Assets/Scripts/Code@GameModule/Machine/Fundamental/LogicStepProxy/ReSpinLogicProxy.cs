// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-07-05 12:15 PM
// Ver : 1.0.0
// Description : LogicProxyFreeGame.cs
// ChangeLog :
// **********************************************

using System.Threading.Tasks;

namespace GameModule
{
    public class ReSpinLogicProxy : LogicStepProxy
    {
        protected ReSpinState reSpinState;
        public ReSpinLogicProxy(MachineContext context)
            : base(context)
        {
        }
        
        protected override void RegisterInterestedWaitEvent()
        {
            waitEvents.Add(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
        }

        public override void SetUp()
        {
            base.SetUp();
            reSpinState = machineContext.state.Get<ReSpinState>();
        }
        
        protected virtual void RestoreTriggerWheelElement()
        {
            var triggerPanels = reSpinState.triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
            {
                machineContext.state.Get<WheelState>().UpdateWheelStateInfo(triggerPanels[0]);
                machineContext.view.Get<Wheel>().ForceUpdateElementOnWheel();   
            }
        }

        protected override void HandleCommonLogic()
        {
            StopWinCycle();
        }

        protected override async void HandleCustomLogic()
        {
            if (reSpinState.ReSpinTriggered)
            {
                await HandleReSpinStartLogic();
            }
            
            Proceed();
        }
        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return reSpinState.ReSpinTriggered || reSpinState.IsInRespin;
        }

        protected virtual async Task HandleReSpinStartLogic()
        {
            if (!IsFromMachineSetup())
            {
                await ShowReSpinTriggerLineAnimation();
            }
        }
        
        protected virtual async Task ShowReSpinTriggerLineAnimation()
        {
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            if(wheels.Count > 0)
                await wheels[0].winLineAnimationController.BlinkReSpinLine();
        }
    }
}