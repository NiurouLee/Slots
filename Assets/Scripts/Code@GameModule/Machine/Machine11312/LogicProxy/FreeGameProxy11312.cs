using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace GameModule
{
    public class FreeGameProxy11312 : FreeGameProxy
    {
        private WheelsActiveState11312 wheelActiveState;
        public FreeGameProxy11312(MachineContext context) : base(context)
        {
            wheelActiveState = machineContext.state.Get<WheelsActiveState11312>();
        }
        protected override async void HandleFreeStartLogic()
        {
            StopBackgroundMusic();
            if (!IsFromMachineSetup())
            {
                await ShowFreeSpinTriggerLineAnimation();
            }
            await ShowFreeGameWheelPanel();
            await ShowFreeGameStartPopUp();
            //PauseBackgroundMusic();
            await ShowFreeSpinStartCutSceneAnimation();
            //UnPauseBackgroundMusic();
            Proceed();
        }
        protected override void RecoverLogicState()
        {
            base.RecoverLogicState();
            machineContext.view.Get<Background11312>().PlayBgAnim(1);
        }
        /// <summary>
        /// 展示free转盘
        /// </summary>
        /// <returns></returns>
        public async Task ShowFreeGameWheelPanel()
        {
            TaskCompletionSource<bool> waitTask = new TaskCompletionSource<bool>();
            machineContext.AddWaitTask(waitTask, null);
            AudioUtil.Instance.PlayAudioFx("Wheel_Appear");
            var freeGameWheel = PopUpManager.Instance.ShowPopUp<UIFreeGameWheel11312>("UIFreeGameWheel11312");
            freeGameWheel.SetWheelRes((int)freeSpinState.freeSpinId);
            freeGameWheel.SetPopUpCloseAction(() =>
            {
                machineContext.RemoveTask(waitTask);
                waitTask.SetResult(true);
            });
            await waitTask.Task;
        }

        protected override void HandleFreeReTriggerLogic()
        {
            AudioUtil.Instance.PlayAudioFx("FreeGameRetrigger_Open");
            base.HandleFreeReTriggerLogic();
        }


        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            await ShowFreeCut(() =>
            {
                machineContext.view.Get<Background11312>().PlayBgAnim(1);
                machineContext.state.Get<BetState>().SetTotalBet(freeSpinState.freeSpinBet);
                wheelActiveState.UpdateRunningWheelState(null);
            });
            UpdateFreeSpinUIState(true, UseAverageBet());
            await Task.CompletedTask;
        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            await ShowFreeCut(() =>
            {
                machineContext.view.Get<Background11312>().PlayBgAnim(0);
                machineContext.view.Get<FreeWheelRowFx11312>().CloseAllRowShow();
                wheelActiveState.UpdateRunningWheelState(null);
                RestoreTriggerWheelElement();
            });
            UpdateFreeSpinUIState(false, UseAverageBet());
            await base.ShowFreeSpinFinishCutSceneAnimation();
        }

        public async Task ShowFreeCut(Action callback)
        {
            AudioUtil.Instance.PlayAudioFx("FreeGame_Transition");
            // 过场时间长 5.467f
            var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FreeCut");
            transitionAnimation.transform.SetParent(machineContext.transform);
            machineContext.WaitSeconds(2f, () =>
            {
                callback?.Invoke();
            });
            await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "Open", machineContext);
            GameObject.Destroy(transitionAnimation);

        }

    }
}

