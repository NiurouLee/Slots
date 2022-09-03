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

namespace GameModule
{
    public class ControlPanelWinUpdateProxy : LogicStepProxy
    {
        public ControlPanelWinUpdateProxy(MachineContext context)
            : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            bool playWinOutAnim = machineContext.state.Get<WinState>().winLevel < (int)WinLevel.BigWin;

            var winLevel = AddWinChipsToControlPanel(machineContext.state.Get<WinState>().wheelWin, 0,true, playWinOutAnim);
            if (winLevel == WinLevel.NiceWin && machineContext.view.Get<ControlPanel>().GetNumAnimationEndTime() > 0)
            {
                if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin ||
                    !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                    machineContext.view.Get<ControlPanel>().ShowStopButton(true);
            }
            else
            {
                Proceed();
            }
        }

        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params object[] args)
        {
            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_WAIT_EVENT_COMPLETE:
                    var completeEvent = (WaitEvent) args[0];
                    if (completeEvent == WaitEvent.WAIT_WIN_NUM_ANIMTION)
                    {
                        Proceed();
                        return;
                    }
                    else if (completeEvent == WaitEvent.WAIT_BLINK_ALL_WIN_LINE)
                    {   
                        //PlayWinCycle
                        var activeState = machineContext.state.Get<WheelsActiveState>();
                        var runningWheel = activeState.GetRunningWheel();
                       
                        for (var i = 0; i < runningWheel.Count; i++)
                        {
                            if(!runningWheel[i].winLineAnimationController.IsWinCyclePlaying)
                                runningWheel[i].winLineAnimationController.BlinkWinLineOneByOne();
                        }
                        
                    }
                    break;
                case MachineInternalEvent.EVENT_CONTROL_STOP:
                    machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
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