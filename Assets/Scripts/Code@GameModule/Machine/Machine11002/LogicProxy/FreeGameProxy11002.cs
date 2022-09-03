// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/14:12
// Ver : 1.0.0
// Description : BonusLogicProxy11002.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using System.Threading.Tasks;
using DragonU3DSDK.Network.API.ILProtocol;
using GameModule;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class FreeGameProxy11002 : FreeGameProxy
    {
        private CharacterView11002 _characterView;
        private Animator _animator;

        private Animator _animatorTransition;

        public FreeGameProxy11002(MachineContext context) :
            base(context)
        {
            _characterView = machineContext.view.Get<CharacterView11002>();
            _animator = machineContext.transform.Find("ZhenpingAnim").GetComponent<Animator>();
            _animatorTransition = context.transform.Find("Transition11002").GetComponent<Animator>();
            var sortingGroup = _animatorTransition.gameObject.AddComponent<SortingGroup>();
            sortingGroup.sortingLayerName = "LocalFx";
        }

        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            var popup = PopUpManager.Instance.GetPopup<UIWheelBonus11002>();
            if (popup != null)
            {
                PopUpManager.Instance.ClosePopUp(popup);
                popup.SetPopUpCloseAction(() =>
                {
                    machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
                });
            }
            machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);

            _animatorTransition.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Free_Transition");
            await XUtility.WaitSeconds(1.5f, machineContext);

            machineContext.state.Get<WheelsActiveState11002>().UpdateRunningWheel(new List<string>() { "Wheel1" });

            var freeWheel = machineContext.view.Get<Wheel>(1);
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.count > 0)
            {
                var triggerPanel = triggerPanels[0];
                freeWheel.wheelState.UpdateWheelStateInfo(triggerPanel);
            }
            freeWheel.ForceUpdateElementOnWheel();
            await machineContext.view.Get<ChillView>(1).RefreshUI(false, false);

            machineContext.state.Get<BetState>().SetTotalBet(GetFreeSpinBet());
            UpdateSpinUiViewTotalBet(false, false);
            UpdateFreeSpinUIState(true, UseAverageBet());
            controlPanel.ShowSpinButton(false);

            await XUtility.WaitSeconds(1.5f, machineContext);
            _animatorTransition.gameObject.SetActive(false);

        }

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            _animatorTransition.gameObject.SetActive(true);
            AudioUtil.Instance.PlayAudioFx("Free_Transition");
            await XUtility.WaitSeconds(1.5f, machineContext);

            machineContext.state.Get<WheelsActiveState11002>().UpdateRunningWheel(new List<string>() { "Wheel0" });
            var normalWheel = machineContext.view.Get<Wheel>(0);
            var freeWheel = machineContext.view.Get<Wheel>(1);
            var elementInfo = freeWheel.GetVisibleElementInfo();
            normalWheel.ForceUpdateElementOnWheel();
            normalWheel.UpdateVisibleElementOnWheel(elementInfo);
            await machineContext.view.Get<ChillView>().RefreshUI(false, false);

            await XUtility.WaitSeconds(1.5f, machineContext);
            _animatorTransition.gameObject.SetActive(false);

            await base.ShowFreeSpinFinishCutSceneAnimation();
        }

        protected override async void HandleFreeReTriggerLogic()
        {
            var freeWheel = machineContext.view.Get<Wheel>("Wheel1");
            var reTriggerElementContainers = freeWheel.GetElementMatchFilter((container) =>
            {
                if (container.sequenceElement.config.id == 17)
                {
                    return true;
                }

                return false;
            });

            if (reTriggerElementContainers.Count > 0)
            {
                for (var i = 0; i < reTriggerElementContainers.Count; i++)
                {
                    var container = reTriggerElementContainers[i];
                    container.ShiftSortOrder(true);
                    container.PlayElementAnimation("Trigger");
                }
                AudioUtil.Instance.PlayAudioFx("B05_BLINK");

                await XUtility.WaitSeconds(2, machineContext);
            }

            UpdateFreeSpinUIState(true);

            Proceed();
        }

        protected override async void HandleFreeFinishLogic()
        {
            var winState = machineContext.state.Get<WinState>();
            if (winState.displayCurrentWin == 0)
            {
                await machineContext.WaitSeconds(0.8f);
            }
            StopBackgroundMusic();
            await machineContext.WaitSeconds(2.0f);

            await ShowFreeGameFinishPopUp();

            UpdateFreeSpinUIState(false);
            await ShowFreeSpinFinishCutSceneAnimation();
            await ShowFreeSpinBigWinEffect();
            //UnPauseBackgroundMusic();

            OnHandleFreeFinishLogicEnd();

            _characterView.Show();
            _characterView.AnimIdle();
            machineContext.view.Get<ChillView>().StopAllFullWildAnimation();
            // RestoreTriggerWheelElement();
        }

        protected override void HandleFreeStartLogic()
        {
            _characterView.Hide();
            _animator.Play("Normal", 0, 0);
            base.HandleFreeStartLogic();
        }
    }
}