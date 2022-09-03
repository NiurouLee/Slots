//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-03 20:11
//  Ver : 1.0.0
//  Description : FreeGameProxy11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class FreeGameProxy11016:FreeGameProxy
    {
        private bool shieldSpinUI;
        public FreeGameProxy11016(MachineContext context)
            : base(context)
        {
            
        }
        
        protected override async Task ShowFreeGameStartPopUp()
        {
            var address = "UIFreeGameStart11016";
            var level = machineContext.state.Get<ExtraState11016>().FreeGameLevel;
            if (level == 5 || level == 10)
            {
                address = "UIMegaFreeSpinStart11016"; 
            }
            if (level == 20)
            {
                address = "UISuperFreeSpinStart11016";
            }
            await ShowFreeGameStartPopUp<UIFreeGameStart11016>(address);
        }

        protected override async Task ShowFreeSpinTriggerLineAnimation()
        {
            await machineContext.WaitSeconds(0.5f);
            var collectView = machineContext.view.Get<CollectView11016>();
            var level = machineContext.state.Get<ExtraState11016>().FreeGameLevel;
            await FlyItems(collectView.GetEndPos(level));
            await machineContext.WaitSeconds(0.5f);
        }

        protected override async Task ShowFreeSpinStartCutSceneAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            machineContext.view.Get<FreeCutAnimationView11016>().ShowCutFree(machineContext);
            await machineContext.WaitSeconds(3);
            await base.ShowFreeSpinStartCutSceneAnimation();
            machineContext.state.Get<WheelsActiveState11016>().UpdateFreeWheelState();
            await machineContext.WaitSeconds(1.1f);
        }

        public async Task FlyItems(Vector3 endPos, Action action=null)
        {
            var flyName ="HeartFly";
            var listFlyers = new List<GameObject>();
            var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
            for (int i = 0; i < wheels.Count; i++)
            {
                var wheel = wheels[i];
                var triggerElementContainers = wheel.GetElementMatchFilter(container =>
                {
                    if (Constant11016.IsFeaturedElement(container.sequenceElement.config.id))
                    {
                        return true;
                    }
                    return false;
                });
                AudioUtil.Instance.PlayAudioFx("Scatter_Fly");   
                for (int j = 0; j < triggerElementContainers.Count; j++)
                {
                    var elementContainer = triggerElementContainers[j];
                    elementContainer.UpdateAnimationToStatic();
                    var element = elementContainer.GetElement() as Element11016;
                    var startPos = element.GetStartWorldPos();
                    var flyContainer = machineContext.assetProvider.InstantiateGameObject(flyName,true);
                    flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
                    flyContainer.transform.SetParent(machineContext.transform,false);
                    XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(),"Fly");
                    XUtility.Fly(flyContainer.transform, startPos, endPos, 0, 0.5f, null);
                    listFlyers.Add(flyContainer);
                }
                await machineContext.WaitSeconds(1f);
                AudioUtil.Instance.PlayAudioFx("Scatter_FlyStop");
                var collectView = machineContext.view.Get<CollectView11016>();
                var level = machineContext.state.Get<ExtraState11016>().FreeGameLevel;
                collectView.UpdateFreeCount(level,true);
                await machineContext.WaitSeconds(0.5f);
                AudioUtil.Instance.PlayAudioFx("B01_Trigger");
                for (int j = 0; j < triggerElementContainers.Count; j++)
                {
                    var elementContainer = triggerElementContainers[j];
                    var element = elementContainer.GetElement() as Element11016;
                    if (j == triggerElementContainers.Count-1)
                    {
                        await element.PlayHeartAnimation("Win");   
                    }
                    else
                    {
                        element.PlayHeartAnimation("Win");   
                    }
                }
                for (int j = 0; j < triggerElementContainers.Count; j++)
                {
                    var elementContainer = triggerElementContainers[j];
                    var element = elementContainer.GetElement() as Element11016;
                    element.HideTrail();
                }
            }

            if (listFlyers.Count>0)
            {
                await machineContext.WaitSeconds(0.5f);
                for (int i = 0; i < listFlyers.Count; i++)
                {
                    machineContext.assetProvider.RecycleGameObject(flyName,listFlyers[i]);
                }
                action?.Invoke();   
            }
        }

        protected override void HandleCommonLogic()
        {
            if (!IsFromMachineSetup() && !IsFreeSpinTriggered() && IsFreeSpinReTriggered())
            {
                shieldSpinUI = true;
            }
            base.HandleCommonLogic();
        }

        protected override async void Proceed()
        {
            var nLastPanelCount = machineContext.state.Get<WheelsActiveState11016>().LastPanelCount;
            var nextIsFree = machineContext.state.Get<FreeSpinState>().NextIsFreeSpin;
            var extraState = machineContext.state.Get<ExtraState11016>();
            var curPanelCount = extraState.CurrentPanelCount;
            if (nextIsFree && !IsFromMachineSetup() && nLastPanelCount > 0 && curPanelCount > nLastPanelCount)
            {
                if (freeSpinState.NewCount > 0)
                {
                    await machineContext.WaitSeconds(1f);
                    AudioUtil.Instance.PlayAudioFx("+1Spin_Trigger");
                    var transStart = machineContext.transform.Find("Wheels/ExtraFreeStart");
                    var flyContainer = machineContext.assetProvider.InstantiateGameObject("SpinNotice",true);
                    flyContainer.transform.localScale = Vector3.one;
                    var extraView = new FreeExtraView11016(flyContainer.transform);
                    var startPos = transStart.transform.position;
                    flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
                    flyContainer.transform.SetParent(machineContext.transform,false);
                    
                    var position = machineContext.view.Get<ControlPanel>().GetFreeCountTxtPosition();
                    var cameraZ = Camera.main.transform.position.z;
                    var canvasZ = machineContext.MachineUICanvasTransform.position.z;
                    var factor = Mathf.Abs(cameraZ / canvasZ);
                    var endPos = new Vector3(factor * position.x+0.3f, factor * position.y+0.3f, 0);
                    extraView.Show();
                    extraView.UpdateExtraFreeCount((int)freeSpinState.NewCount);
                    AudioUtil.Instance.PauseMusic();
                    await machineContext.WaitSeconds(1f);
                    AudioUtil.Instance.PlayAudioFx("+1Spin_Fly");
                    XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(),"frees_fly");
                    XUtility.Fly(flyContainer.transform, startPos, endPos, 0, 0.5f, async () =>
                    {
                        await machineContext.WaitSeconds(0.5f);
                        machineContext.assetProvider.RecycleGameObject("SpinNotice",flyContainer);
                    });
                    await machineContext.WaitSeconds(0.5f);
                    AudioUtil.Instance.PlayAudioFx("+1Spin_FlyStop");
                    controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount, true, false);
                    await machineContext.WaitSeconds(1);
                    AudioUtil.Instance.UnPauseMusic();
                    AudioUtil.Instance.FadeInMusic();
                }
                
                machineContext.state.Get<WheelsActiveState11016>().UpdateFreePanels(false);
                var wheels = machineContext.state.Get<WheelsActiveState>().GetRunningWheel();
                var listPositions = machineContext.view.Get<MultiFreePanelView11016>().GetPanelsPositions(curPanelCount);
                for (int i = 0; i < listPositions.Count; i++)
                {
                    var transformPos = listPositions[i];
                    var wheel = wheels[i];
                    if (i>=nLastPanelCount)
                    {
                        wheel.transform.localScale = Vector3.zero;
                        wheel.transform.position = transformPos.position;
                    }
                    else
                    {
                        for (int j = 0; j < wheel.rollCount; j++)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                var elementContainer = wheel.GetRoll(j).GetVisibleContainer(k);
                                var element = elementContainer.GetElement() as Element11016;
                                element.HideTrail();
                            }
                        }
                    }
                }
                await machineContext.WaitNFrame(1);
                AudioUtil.Instance.PlayAudioFx("ExpandSet");
                for (int i = 0; i < listPositions.Count; i++)
                {
                    var transformPos = listPositions[i];
                    var wheel = wheels[i];
                    wheel.transform.DOScale(transformPos.localScale, 0.5f);
                    wheel.transform.DOMove(transformPos.position, 0.5f);
                }
                var topFreeView = machineContext.view.Get<FreeTopView11016>();
                if (curPanelCount == 9)
                {
                    topFreeView.transform.gameObject.SetActive(false);
                }
                await machineContext.WaitSeconds(1);
                if (curPanelCount < 9)
                {
                    AudioUtil.Instance.PlayAudioFx("FreeSpin_Scatter_FlyStop");   
                }
                var nextPanelCount = extraState.GetNextPanelCount(extraState.CurrentPanelCount);
                topFreeView.UpdateCollectCount(extraState.BombLeftNextLevel, nextPanelCount,true);   
                
                await machineContext.WaitSeconds(1);
            }
            machineContext.state.Get<WheelsActiveState11016>().LastPanelCount = curPanelCount;
            if (!IsFromMachineSetup() && !IsFreeSpinTriggered() && NextSpinIsFreeSpin())
            {
                base.UpdateFreeSpinUIState(true,UseAverageBet());   
            }
            HandleToNextStep();
            shieldSpinUI = false;
        }
        
        protected override void UpdateFreeSpinUIState(bool isFreeSpin, bool isAverage = false)
        {
            if (shieldSpinUI) return;
            base.UpdateFreeSpinUIState(isFreeSpin, isAverage);
            // controlPanel.UpdateControlPanelState(isFreeSpin,isAverage);
            // controlPanel.UpdateFreeSpinCountText(freeSpinState.CurrentCount, freeSpinState.TotalCount);
        }

        public override bool UseAverageBet()
        {
            return machineContext.state.Get<ExtraState11016>().IsAverateBet;
        }
        

        protected override async Task ShowFreeSpinFinishCutSceneAnimation()
        {
            AudioUtil.Instance.PlayAudioFx("FreeSpin_Video1");
            machineContext.state.Get<WheelsActiveState11016>().LastPanelCount = 0;
            machineContext.view.Get<FreeCutAnimationView11016>().ShowCutFree(machineContext);
            await machineContext.WaitSeconds(3);
            machineContext.state.Get<WheelsActiveState11016>().UpdateBaseWheelState();
            base.RestoreTriggerWheelElement();
            await base.ShowFreeSpinFinishCutSceneAnimation();
            await machineContext.WaitSeconds(1.1f);
        }
    }
}