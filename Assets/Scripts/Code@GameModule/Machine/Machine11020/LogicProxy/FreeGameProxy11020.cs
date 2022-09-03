using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Threading.Tasks;

namespace GameModule
{
    public class FreeGameProxy11020 : FreeGameProxy
    {
        private float animatorTransitionTotalTime = 4.0f;
        private float transitionWheelTime = 2f;

        protected Animator animatorMachineContext;
        protected Animator animatorTransition;

        private SuperBonusInfoState11020 superBonusInfoState;

        public FreeGameProxy11020(MachineContext context) 
            : base(context)
        {
            animatorMachineContext = context.transform.GetComponent<Animator>();
            animatorTransition = context.transform.Find("Wheels/Transition" + context.assetProvider.MachineId).GetComponent<Animator>();
            animatorTransition.gameObject.SetActive(false);
        }

        public override void SetUp()
        {
            base.SetUp();
            
            superBonusInfoState = machineContext.state.Get<SuperBonusInfoState11020>();
        }

        protected override async void RecoverCustomFreeSpinState()
        {
           
            
        }

        protected override async void RecoverLogicState()
        {
            if (IsFreeSpinTriggered())
            {
                HandleFreeStartLogic();
            }
            else
            {
                UpdateFreeSpinUIState(true, UseAverageBet());

                var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            
                wheelActiveState.UpdateRunningWheelState(null);


                if (NeedSettle())
                {
                    HandleFreeFinishLogic();
                }
                else
                {
                     controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount, true);

                    await machineContext.view.Get<WheelView11020>().StartSuperBonusWheel();
                             
                    controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount);

                    Proceed();
                }
            }
        }

        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            animatorTransition.gameObject.SetActive(true);

            XUtility.PlayAnimation(animatorMachineContext, "Shake2");
            XUtility.PlayAnimation(animatorTransition, "Open");

            AudioUtil.Instance.PlayAudioFx("Free_Video");
            await machineContext.WaitSeconds(transitionWheelTime);

            var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            
            wheelActiveState.UpdateRunningWheelStateFreeSpin();

            await base.ShowFreeSpinStartCutSceneAnimation();

            controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount, true);

            await machineContext.view.Get<WheelView11020>().StartSuperBonusWheel();

            controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount);

            await machineContext.WaitSeconds(animatorTransitionTotalTime - transitionWheelTime);
            animatorTransition.gameObject.SetActive(false);
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            await base.ShowFreeSpinFinishCutSceneAnimation();

            AudioUtil.Instance.PlayAudioFx("Free_Video");
            animatorTransition.gameObject.SetActive(true);

            XUtility.PlayAnimation(animatorMachineContext, "Shake2");
            XUtility.PlayAnimation(animatorTransition, "Open");
            
            await machineContext.WaitSeconds(transitionWheelTime);
            
            var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            
            var wheel = wheelActiveState.GetRunningWheel()[0];

            var elementInfo = wheel.GetVisibleElementInfo();

            wheelActiveState.baseWheelFromFree = true;

            wheelActiveState.UpdateRunningWheelState(null);

            wheel = wheelActiveState.GetRunningWheel()[0];

            RestoreLastWheelElement(wheel, elementInfo);

            var lockedFramesView = machineContext.view.Get<LockedFramesView11020>();

            if (lockedFramesView != null)
            {
                lockedFramesView.StartWheel(wheel, true);
            }

            await machineContext.WaitSeconds(animatorTransitionTotalTime - transitionWheelTime);
            animatorTransition.gameObject.SetActive(false);
        }
        
        public override bool UseAverageBet()
        {
            return superBonusInfoState.IsBonusGame();
        }

        protected override async Task ShowFreeGameStartPopUp()
        {  
            await machineContext.WaitSeconds(2.0f);
            
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);

            if (superBonusInfoState.IsBonusGame())
            {
                AudioUtil.Instance.PlayAudioFx("FreeGameStart_Open");
                SuperBonusStartPopUp11020 startPopUp1 = PopUpManager.Instance.ShowPopUp<SuperBonusStartPopUp11020>("UISuperBonusStart" + machineContext.assetProvider.MachineId);

                startPopUp1.SetPopUpCloseAction(
                    () =>
                    {
                        machineContext.RemoveTask(waitTask);
                        waitTask.SetResult(true);
                    });
            }
            else
            {   
                AudioUtil.Instance.PlayAudioFx("FreeGameStart_Open");
                FreeSpinStartPopUp startPopUp = PopUpManager.Instance.ShowPopUp<FreeSpinStartPopUp>("UIFreeGameStart" + machineContext.assetProvider.MachineId);

                startPopUp.SetPopUpCloseAction(
                    () =>
                    {
                        machineContext.RemoveTask(waitTask);
                        waitTask.SetResult(true);
                    });
            }
            

            await waitTask.Task;
        }
     
        protected override  async Task  ShowFreeGameFinishPopUp()
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);

            AudioUtil.Instance.PlayAudioFx("FreeGameEnd_Open");
            var startPopUp = PopUpManager.Instance.ShowPopUp<FreeSpinFinishPopUp11020>("UIFreeGameFinish" + machineContext.assetProvider.MachineId);
            startPopUp.BindFinishAction(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });

            if (superBonusInfoState.IsBonusGame())
            {
                var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
                var wheel = wheelActiveState.GetRunningWheel()[0];
                wheelActiveState.RemoveWheelWildElement(wheel);
            }

            await waitTask.Task;
        }

        protected override async void HandleFreeReTriggerLogic()
        {
            await ShowFreeSpinTriggerLineAnimation();

            base.HandleFreeReTriggerLogic();
        }

        protected override Task ShowFreeSpinTriggerLineAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("B01_Trigger");
            return base.ShowFreeSpinTriggerLineAnimation();
        }
        
        protected void RestoreLastWheelElement(Wheel wheel, List<List<SequenceElement>> elementInfo)
        {
            // 1. 超级锁定框 - 蓝色WILD     --> 普通锁定框 + 红色WILD
            // 2. 超级锁定框 - 非WILD图标 -->  普通图标（不要框）
            // 3. 普通锁定框 - 蓝色WILD     -->  普通锁定框 + 红色WILD
            // 4. 普通锁定框 - 非WILD图标 --> 普通锁定框 + 非WILD图标

            var elementConfigSet = machineContext.machineConfig.GetElementConfigSet();

            var count = elementInfo.Count;

            for (var i = 0; i < count; ++i)
            {
                var elements = elementInfo[i];

                for (var k = 0; k < elements.Count; ++k)
                {
                    
                    if (elements[k].config.id == Constant11020.buleWildElement)
                    {
                        elements[k] = new SequenceElement(elementConfigSet.GetElementConfig(Constant11020.wildElement), machineContext);
                    }
                }
            }

            wheel.UpdateVisibleElementOnWheel(elementInfo);

            var wheelActiveState = machineContext.state.Get<WheelsActiveState11020>();
            wheelActiveState.UpdateWheelHighElementSortingOrder();
        }
    }
}