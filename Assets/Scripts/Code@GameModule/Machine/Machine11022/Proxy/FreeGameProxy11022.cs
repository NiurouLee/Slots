//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-24 20:38
//  Ver : 1.0.0
//  Description : FreeGameProxy11010.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule
{
    public class FreeGameProxy11022: FreeGameProxy
    {
        public FreeGameProxy11022(MachineContext context) : base(context)
        {
        }
        
        protected override void RecoverCustomFreeSpinState()
        {
            machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            machineContext.state.Get<WheelsActiveState11022>().UpdateFreeWheelState();
        }

        public override void RecoverFreeSpinStateWhenRoomSetup()
        {
            if (!IsFreeSpinTriggered())
            {
                UpdateFreeSpinUIState(true, UseAverageBet());
                RecoverCustomFreeSpinState();
                AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
            }
        }
        
        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            // AudioUtil.Instance.PlayAudioFx("Video");
            // machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            // await machineContext.WaitSeconds(0.5f);
            try
            {
                // if (!IsFromMachineSetup())
                // {
                    var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FreeGameStartCutPopup");

                    var sortingGroup = transitionAnimation.GetComponent<SortingGroup>();
                    sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                    sortingGroup.sortingOrder = 100;
 
                    transitionAnimation.transform.SetParent(machineContext.transform);

                    machineContext.WaitSeconds(2.5f, () =>
                    {
                        RecoverCustomFreeSpinState();
                        AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetFreeBackgroundMusicName());
                    });

                    AudioUtil.Instance.PlayAudioFx("Free_Transition");
                    await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionFG", machineContext);

                    GameObject.Destroy(transitionAnimation);   
                // }

                await base.ShowFreeSpinStartCutSceneAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override async void HandleFreeStartLogic()
        {
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            StopBackgroundMusic();
            if (!IsFromMachineSetup())
            {
                await ShowFreeSpinTriggerLineAnimation();
            }

            // await ShowFreeGameStartPopUp();
            
            await ShowFreeSpinStartCutSceneAnimation();
            //UnPauseBackgroundMusic();
            Proceed();
        }
        
        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            try
            {
                var transitionAnimation = machineContext.assetProvider.InstantiateGameObject("FreeGameStartCutPopup");

                var sortingGroup = transitionAnimation.GetComponent<SortingGroup>();
                sortingGroup.sortingLayerID = SortingLayer.NameToID("LocalFx");
                sortingGroup.sortingOrder = 100;
 
                transitionAnimation.transform.SetParent(machineContext.transform);

                machineContext.WaitSeconds(2.5f, () =>
                {
                    machineContext.state.Get<WheelsActiveState11022>().UpdateBaseWheelState();
                    RestoreTriggerWheelElement();
                    AudioUtil.Instance.StopMusic();
                    // AudioUtil.Instance.PlayMusic(machineContext.machineConfig.audioConfig.GetBaseBackgroundMusicName());
                });

                AudioUtil.Instance.PlayAudioFx("Free_Transition");
                await XUtility.PlayAnimationAsync(transitionAnimation.GetComponent<Animator>(), "TransitionFG", machineContext);

                GameObject.Destroy(transitionAnimation);
  
                await base.ShowFreeSpinFinishCutSceneAnimation();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        protected override void RestoreTriggerWheelElement()
        {
            var triggerPanels = machineContext.state.Get<FreeSpinState>().triggerPanels;
            if (triggerPanels != null && triggerPanels.Count > 0 && triggerPanels[0] != null)
            {
                var runningWheel = machineContext.state.Get<WheelsActiveState11022>().GetRunningWheel()[0];
                runningWheel.wheelState.UpdateWheelStateInfo(triggerPanels[0]);
                runningWheel.ForceUpdateElementOnWheel();
                for (var i = 0; i < runningWheel.GetMaxSpinningUpdaterCount(); i++)
                {
                    var roll = runningWheel.GetRoll(i);
                    for (var j = 0; j < runningWheel.GetRollRowCount(0,runningWheel.wheelState.GetWheelConfig()); j++)
                    {
                        var container = roll.GetVisibleContainer(j);
                        if (container.sequenceElement.config.id == 13)
                        {
                            container.UpdateElement(container.sequenceElement,true);
                            container.UpdateElementMaskInteraction(true);
                            container.ShiftSortOrder(true);
                        }
                    }
                }
            }
        }
        protected override async void HandleFreeReTriggerLogic()
        {
            machineContext.view.Get<ControlPanel>().ShowSpinButton(false);
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            await wheels[0].winLineAnimationController.BlinkFreeSpinTriggerLine();
            
            var assetName = "UIFreeGameReTrigger" + machineContext.assetProvider.AssetsId;
            if (machineContext.assetProvider.GetAsset<GameObject>(assetName) != null)
            {
                var popUp = ShowFreeSpinReTriggeredPopup(assetName);
                popUp.SetPopUpCloseAction(() =>
                {
                    UpdateFreeSpinUIState(true, UseAverageBet());
                    HandleFreeReTriggerEnd();
                });
                popUp.SetExtraCount(freeSpinState.NewCount);
            }
            else
            {
                UpdateFreeSpinUIState(true, UseAverageBet());
                HandleFreeReTriggerEnd();
            }
        }
        protected override FreeSpinReTriggerPopUp ShowFreeSpinReTriggeredPopup(string assetName)
        {
            return PopUpManager.Instance.ShowPopUp<UIFreeGameReTrigger11022>(assetName);
        }
        
        protected override async Task ShowFreeGameFinishPopUp()
        {
            await ShowFreeGameFinishPopUp<FreeSpinFinishPopUp11022>();
        }

        protected override void HandleFreeFinishLogic()
        {
            base.HandleFreeFinishLogic();
            machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
        }
    }
}