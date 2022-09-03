//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-30 11:13
//  Ver : 1.0.0
//  Description : WheelStopSpecialEffectProxy11016.cs
//  ChangeLog :
//  **********************************************

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11016:WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11016(MachineContext machineContext) : base(machineContext)
        {
            
        }

        protected override async void HandleCustomLogic()
        {
            var nextIsFree = machineContext.state.Get<FreeSpinState>().NextIsFreeSpin;
            var extraState = machineContext.state.Get<ExtraState11016>();
            var topFreeView = machineContext.view.Get<FreeTopView11016>();
            if (nextIsFree && !machineContext.state.Get<FreeSpinState>().IsTriggerFreeSpin && !IsFromMachineSetup() && machineContext.state.Get<WheelsActiveState11016>().LastPanelCount < extraState.MaxFreePanelCount)
            {
                await FlyItems(topFreeView.GetEndPos(),() =>
                {
                    var nLastPanelCount = machineContext.state.Get<WheelsActiveState11016>().LastPanelCount;
                    var curPanelCount = extraState.CurrentPanelCount;
                    var nextPanelCount = extraState.GetNextPanelCount(extraState.CurrentPanelCount);
                    if (nLastPanelCount > 0 && curPanelCount > nLastPanelCount)
                    {
                        topFreeView.UpdateCollectCountToZero();
                    }
                    else
                    {
                        topFreeView.UpdateCollectCount(extraState.BombLeftNextLevel,nextPanelCount,true);   
                    }
                });
            }
            base.HandleCustomLogic();
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
                if (triggerElementContainers.Count>0)
                {
                    AudioUtil.Instance.PauseMusic();
                    AudioUtil.Instance.PlayAudioFx("FreeSpin_Scatter_Fly");   
                }
                for (int j = 0; j < triggerElementContainers.Count; j++)
                {
                    var elementContainer = triggerElementContainers[j];
                    elementContainer.UpdateAnimationToStatic();
                    var element = elementContainer.GetElement() as Element11016;
                    var startPos = element.GetStartWorldPos();
                    element.HideTrail();  
                    var flyContainer = machineContext.assetProvider.InstantiateGameObject(flyName,true);
                    flyContainer.transform.position = new Vector3(startPos.x, startPos.y, startPos.z);
                    flyContainer.transform.SetParent(machineContext.transform,false);
                    XUtility.PlayAnimation(flyContainer.GetComponent<Animator>(),"Fly");
                    XUtility.Fly(flyContainer.transform, startPos, endPos, 0, 0.5f, null);
                    listFlyers.Add(flyContainer);
                }
            }

            if (listFlyers.Count>0)
            {
                await machineContext.WaitSeconds(0.5f);
                AudioUtil.Instance.PlayAudioFx("FreeSpin_Scatter_FlyStop");
                action?.Invoke();  
                await machineContext.WaitSeconds(0.3f);
                AudioUtil.Instance.UnPauseMusic();
                for (int i = 0; i < listFlyers.Count; i++)
                {
                    machineContext.assetProvider.RecycleGameObject(flyName,listFlyers[i]);
                }
                
            }
        }
    }
}