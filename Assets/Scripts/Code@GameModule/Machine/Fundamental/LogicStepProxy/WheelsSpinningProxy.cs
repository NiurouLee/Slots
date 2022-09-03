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

using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;


namespace GameModule
{
    public class WheelsSpinningProxy : LogicStepProxy
    {
        /// <summary>®
        /// 正在滚动的轮盘
        /// </summary>
        protected List<Wheel> spinningWheel;

        /// <summary>
        /// 这一轮滚动完成的轮盘
        /// </summary>
        protected List<Wheel> finishWheel;

        /// <summary>
        /// 当前是激活状态的的轮盘
        /// </summary>
        protected List<Wheel> runningWheel;
        
        /// <summary>
        /// 当前等待转动的轮盘
        /// </summary>
        protected List<Wheel> waitingWheel;

        protected WheelSpinningOrder spinningOrder;

        public WheelsSpinningProxy(MachineContext context)
            : base(context)
        {
            spinningOrder = WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP;
        }

        public void UpdateSpinningOrder(WheelSpinningOrder inSpinningOrder)
        {
            spinningOrder = inSpinningOrder;
        }

        public override void SetUp()
        {
            finishWheel = new List<Wheel>(5);
            spinningWheel = new List<Wheel>(5);
            waitingWheel = new List<Wheel>(5);
        }

        protected override void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            runningWheel = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            spinningOrder = machineContext.state.Get<WheelsActiveState>().SpinningOrder;
            waitingWheel.Clear();
         
            if (spinningOrder == WheelSpinningOrder.ONE_BY_ONE)
            {
                waitingWheel = new List<Wheel>(runningWheel.ToArray());
            }
            
            spinningWheel.Clear();
            finishWheel.Clear();
        }

        protected override void HandleCustomLogic()
        {
            if (spinningOrder == WheelSpinningOrder.ONE_BY_ONE)
            {
                var wheel = waitingWheel[0];
                spinningWheel.Add(wheel);
                waitingWheel.RemoveAt(0);
                wheel.spinningController.StartSpinning(OnOneWheelSpinningEnd, null, OnCanQuickStop, 0);
            }
            else
            {
                int updateIndex = 0;

                for (var i = 0; i < runningWheel.Count; i++)
                {
                    spinningWheel.Add(runningWheel[i]);
                    switch (spinningOrder)
                    {
                        case WheelSpinningOrder.SAME_TIME:
                            runningWheel[i].spinningController
                                .StartSpinning(OnOneWheelSpinningEnd, null, OnCanQuickStop, updateIndex);
                            break;
                        case WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP:
                            updateIndex = runningWheel[i].spinningController
                                .StartSpinning(OnOneWheelSpinningEnd, OnWheelAnticipationAnimationStop, OnCanQuickStop, updateIndex);
                            break;
                    }
                }
            }
        }

        protected void OnOneWheelSpinningEnd(Wheel wheel)
        {
            finishWheel.Add(wheel);
            spinningWheel.Remove(wheel);

            if (finishWheel.Count == runningWheel.Count)
            {
                OnAllWheelSpinningEnd();
            }
      
            if (spinningOrder == WheelSpinningOrder.ONE_BY_ONE && waitingWheel.Count > 0)
            {
                if (waitingWheel.Count > 0)
                {
                    var nextWheel = waitingWheel[0];
                    waitingWheel.RemoveAt(0);
                    spinningWheel.Add(nextWheel);
                    nextWheel.spinningController.StartSpinning(OnOneWheelSpinningEnd, null, OnCanQuickStop, 0);
                }
            }
        }

        public void OnWheelAnticipationAnimationStop(Wheel wheel)
        {
            if (spinningOrder == WheelSpinningOrder.SAME_TIME_START_ONE_BY_ONE_STOP)
            {
                if (spinningWheel.Count > 0)
                {
                    for (int i = 0; i < spinningWheel.Count; i++)
                    {
                        if (spinningWheel[i] != wheel)
                        {
                            spinningWheel[i].spinningController.CheckAndShowAnticipationAnimation();
                            break;
                        }
                    }
                }
            }
        }
        
         public virtual void OnSpinResultReceived()
         {
             var preWheelHasAnticipation = false;
           
             for (var index = 0; index < runningWheel.Count; index++)
             { 
                 preWheelHasAnticipation = runningWheel[index].spinningController.OnSpinResultReceived(preWheelHasAnticipation);
             }


             if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin
                 || machineContext.state.Get<ReSpinState>().IsInRespin
                 || !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
             {
                 machineContext.view.Get<ControlPanel>().ShowStopButton(true);
             }
         }

         public virtual void OnQuickStopClicked()
         {
             if (machineContext.state.Get<FreeSpinState>().IsInFreeSpin
                 || machineContext.state.Get<ReSpinState>().IsInRespin 
                 || !machineContext.state.Get<AutoSpinState>().IsAutoSpin)
             {
                 //free中无视autoSpin
                 machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
             }

             if (spinningOrder != WheelSpinningOrder.ONE_BY_ONE)
             {
                 for (var i = 0; i < spinningWheel.Count; i++)
                 {
                     spinningWheel[i].spinningController.OnQuickStopped();
                 }
             }
             
             if (!machineContext.state.Get<FreeSpinState>().IsInFreeSpin && !machineContext.state.Get<ReSpinState>().IsInRespin)
                   machineContext.view.Get<ControlPanel>().UpdateWinLabelChips(0);
             
             
             bool isAutoSpin = machineContext.state.Get<AutoSpinState>().IsAutoSpin;
             ulong bet = machineContext.state.Get<BetState>().totalBet;
             BiManagerGameModule.Instance.SendSpinAction(
                 machineContext.machineConfig.machineId.ToString(),
                 BiEventFortuneX.Types.SpinActionType.Stop,isAutoSpin,bet,"");
         }
         
        protected void OnCanQuickStop()
        {
            
        }

        protected void OnAllWheelSpinningEnd()
        {
            machineContext.StartCoroutine(OnStepFinish());
        }

        protected IEnumerator OnStepFinish()
        {
            yield return null;

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin
                || machineContext.state.Get<FreeSpinState>().IsInFreeSpin
                || machineContext.state.Get<ReSpinState>().IsInRespin)
            {
                machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            }

            machineContext.view.Get<FiveOfKindView>().HideFiveOfKind();
            
            Proceed();
        }

        public override void LogicUpdate()
        {
            if (spinningWheel.Count > 0)
            {
                for (var i = 0; i < spinningWheel.Count; i++)
                {
                    spinningWheel[i].spinningController.OnLogicUpdate();
                }
            }
        }
        
        public virtual void OnAutoSpinStopClicked()
        {
            var autoSpinState = machineContext.state.Get<AutoSpinState>();
            
            if (autoSpinState.IsAutoSpin)
            {
                autoSpinState.OnDisableAutoSpin();
                machineContext.view.Get<ControlPanel>().ShowStopButton(false);
            }
        }

        public override void OnMachineInternalEvent(MachineInternalEvent internalEvent, params  object[] args)
        {
            switch (internalEvent)
            {
                case MachineInternalEvent.EVENT_SERVER_SPIN_DATA_RECEIVED:
                    OnSpinResultReceived();
                    break;
                case MachineInternalEvent.EVENT_CONTROL_STOP:
                    OnQuickStopClicked();
                    break;
                case MachineInternalEvent.EVENT_CONTROL_AUTO_SPIN_STOP:
                    OnAutoSpinStopClicked();
                    break;
            }
            
            base.OnMachineInternalEvent(internalEvent, args);
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return true;
        }
    }
}
 