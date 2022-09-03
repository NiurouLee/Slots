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
    public class PreRoundEndNumInterruptProxy : LogicStepProxy
    {
        protected FreeSpinState freeSpinState;

        public PreRoundEndNumInterruptProxy(MachineContext context)
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
            bool needWinNumberInterrupt = freeSpinState.NextIsFreeSpin || freeSpinState.IsInFreeSpin;
            return machineContext.HasWaitEvent(WaitEvent.WAIT_WIN_NUM_ANIMTION) 
                   && machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime() > 0.5f && needWinNumberInterrupt;
        }
        
        protected override void HandleCustomLogic()
        {
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
                    //顺序不能反，否则会走到下一个LogicProxy，才会调用StopWinCycle
                    StopWinCycle(true);
                    if (machineContext.HasWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE))
                    {
                        machineContext.RemoveWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                    }
                    machineContext.view.Get<ControlPanel>().StopWinAnimation();
                    break;
            }

            base.OnMachineInternalEvent(internalEvent, args);
        }
    }
}