// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/07/11:16
// Ver : 1.0.0
// Description : WinNumInterruptProxy.cs
// ChangeLog :
// **********************************************

namespace GameModule
{
    public class WinNumInterruptProxy : LogicStepProxy
    {
        protected bool needStopWinCycle = false;
        protected FreeSpinState freeSpinState;

        public WinNumInterruptProxy(MachineContext context)
            : base(context)
        {
            
        }

        public override void SetUp()
        {
            base.SetUp();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            //NICE WIN 才执行打断
            return machineContext.HasWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION) 
            && machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime() > 2.0f;
        }
        
        protected override void HandleCustomLogic()
        {
            needStopWinCycle = false;
            machineContext.view.Get<ControlPanel>().ShowStopButton(true);
        }

     
        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                {
                    var waitEvent = (WaitEvent) args[0];
                    if (waitEvent == WaitEvent.WAIT_WIN_NUM_ANIMTION)
                    {
                        machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
       
                        Proceed();
                    }
                    break;
                }
                
                case MachineInternalEvent.EVENT_CONTROL_STOP:
                    machineContext.view.Get<ControlPanel>().StopWinAnimation();
                    break;
                
                case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP:
                    if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    {
                        machineContext.state.Get<AutoSpinState>().OnDisableAutoSpin();
                        machineContext.view.Get<ControlPanel>().ShowStopButton(true);
                    }
                    break;
            }

            base.OnMachineInternalEvent(internalEvent, args);
        }
    }
}