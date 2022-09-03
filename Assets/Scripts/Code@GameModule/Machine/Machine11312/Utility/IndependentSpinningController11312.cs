using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace GameModule
{
    // public class IndependentSpinningController11312<TWheelAnimationController> : WheelSpinningController<TWheelAnimationController>
    //     where TWheelAnimationController : IWheelAnimationController
    // {
    //     
    //     public virtual int StartSpinning(Action<Wheel> inOnWheelSpinningEnd, Action<Wheel> inOnWheelAnticipationEnd, Action inOnCanEnableQuickStop,
    //         int inUpdaterIndex = 0)
    //     {
    //         onWheelSpinningEnd = inOnWheelSpinningEnd;
    //         onCanEnableQuickStop = inOnCanEnableQuickStop;
    //         onWheelAnticipationEnd = inOnWheelAnticipationEnd;
    //
    //         var maxUpdaterCount = wheelToControl.GetMaxSpinningUpdaterCount();
    //
    //         IRollUpdaterEasingConfig easingConfig = wheelState.GetEasingConfig();
    //
    //         startUpdaterIndex = updaterIndex = inUpdaterIndex;
    //         
    //         finishUpdaterIndex = inUpdaterIndex;
    //       
    //         anticipationIsPlaying = false;
    //
    //         spinResultReceived = false;
    //         
    //         wheelToControl.UpdateElementMaskInteraction(false);
    //        
    //         animationController.OnWheelStartSpinning();
    //
    //         for (var rollIndex = 0; rollIndex < maxUpdaterCount; rollIndex++)
    //         {
    //             if (!wheelState.IsRollLocked(rollIndex))
    //             {
    //                 var spinningUpdater = GetUpdater(easingConfig.GetUpdaterType(),
    //                     wheelToControl.GetStopRoll(rollIndex),
    //                     updaterIndex);
    //
    //                 updaterIndex++;
    //                  
    //                 runningUpdater.Add(spinningUpdater);
    //                 //Debug.LogError($"================= Start===wheel:{wheelState.wheelName}");
    //                
    //                 
    //                 spinningUpdater.StartSpinning(easingConfig, rollSpinningEventsCallback);
    //             }
    //         }
    //
    //         if (updaterIndex > inUpdaterIndex)
    //         {
    //             isSpinningFinished = false;
    //         }
    //         else
    //         {
    //             onWheelSpinningEnd.Invoke(wheelToControl);
    //         }
    //
    //         return updaterIndex;
    //     }
    //     
    //     public override int StartSpinning(Action<Wheel> inOnWheelSpinningEnd, Action<Wheel> inOnWheelAnticipationEnd, Action inOnCanEnableQuickStop,
    //         int inUpdaterIndex = 0)
    //     {
    //         if (runningUpdater.Count > 0)
    //         {
    //             for (var i = 0; i < runningUpdater.Count; i++)
    //             {
    //                 RecycleUpdater(runningUpdater[i]);
    //             }
    //             runningUpdater.Clear();
    //         }
    //
    //         onWheelSpinningEnd = inOnWheelSpinningEnd;
    //         onCanEnableQuickStop = inOnCanEnableQuickStop;
    //         onWheelAnticipationEnd = inOnWheelAnticipationEnd;
    //
    //         var maxUpdaterCount = wheelToControl.GetMaxSpinningUpdaterCount();
    //
    //         IRollUpdaterEasingConfig easingConfig = wheelState.GetEasingConfig();
    //
    //         startUpdaterIndex = updaterIndex = inUpdaterIndex;
    //
    //         finishUpdaterIndex = inUpdaterIndex;
    //
    //         anticipationIsPlaying = false;
    //         spinResultReceived = false;
    //         wheelToControl.UpdateElementMaskInteraction(false);
    //
    //         animationController.OnWheelStartSpinning();
    //         
    //         
    //       
    //      
    //         
    //         //TODO:
    //         //  StopWheelRollSoundEffect();
    //         // PlayWheelRollSoundEffect();
    //         for (var rollIndex = 0; rollIndex < maxUpdaterCount; rollIndex++)
    //         {
    //             if (!wheelState.IsRollLocked(rollIndex))
    //             {
    //                 // var type = easingConfig.GetUpdaterType(wheelToControl.GetContext().assetProvider.MachineId);
    //                 var type = easingConfig.GetUpdaterType();
    //                 var spinningUpdater = GetUpdater(type,
    //                     wheelToControl.GetStopRoll(rollIndex),
    //                     updaterIndex);
    //                 updaterIndex++;
    //                 runningUpdater.Add(spinningUpdater);
    //                 //Debug.LogError($"================= Start===wheel:{wheelState.wheelName}");
    //
    //                 spinningUpdater.StartSpinning(easingConfig, rollSpinningEventsCallback);
    //
    //             }
    //         }
    //
    //         if (updaterIndex > inUpdaterIndex)
    //         {
    //             isSpinningFinished = false;
    //         }
    //         else
    //         {
    //             EventBus.Dispatch(new EventOnWheelSpinEnd());
    //             onWheelSpinningEnd?.Invoke(wheelToControl);
    //         }
    //
    //         return updaterIndex;
    //     }
    // }
}

