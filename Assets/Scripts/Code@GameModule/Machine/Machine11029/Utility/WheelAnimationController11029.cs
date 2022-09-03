using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Google.ilruntime.Protobuf.Collections;

namespace GameModule
{
    public class WheelAnimationController11029 : WheelAnimationController
    {
        private List<RollShiftHelper> _rollShiftHelper = new List<RollShiftHelper>();
        private FreeSpinState freeSpinState;
        public override async  void OnRollSpinningStopped(int rollIndex, Action rollLogicEnd)
        {
            var machineContext = ViewManager.Instance.GetSceneView<MachineScene>().viewController.machineContext;
            freeSpinState = machineContext.state.Get<FreeSpinState>();
            var extraState11029 = machineContext.state.Get<ExtraState11029>();
            for (int i = 0; i < 5; i++)
            {
                _rollShiftHelper.Add(new RollShiftHelper(machineContext));
            }
            if (extraState11029.GetIsDrag())
            {
                // if (machineContext.state.Get<AutoSpinState>().IsAutoSpin)
                // {
                //     machineContext.view.Get<ControlPanel>().ShowAutoStopButton(false);
                // }
                var wheel = machineContext.state.Get<WheelsActiveState11029>().GetRunningWheel()[0];
                var wheelState = wheel.wheelState;
                if (rollIndex == 0)
                {
                    var mapPositions = machineContext.state.Get<ExtraState11029>().GetDragPos();
                    if (mapPositions.Count > 0)
                    {
                        wheel.GetRoll(0).elementSupplier.OnForceUpdateReel();
                        
                        List<Task> listTask = new List<Task>();
                        var listKeys = new List<uint>(mapPositions.Keys);
                        for (int i = 0; i < listKeys.Count; i++)
                        {
                            int shiftStep = mapPositions[listKeys[i]];
                            listTask.Add(_rollShiftHelper[(int) rollIndex].ShiftRoll(wheel.GetRoll((int) rollIndex),
                                Math.Abs(shiftStep), shiftStep > 0, 0.5f));
                        }
                        await Task.WhenAll(listTask);
                    }
                    machineContext.view.Get<LightView11029>().ShowLight(false);
                    int rollCount = wheel.rollCount;
                    for (int m = 0; m < rollCount; m++)
                    {
                        for (int n = 0; n < wheel.GetRoll(m).rowCount; n++)
                        {
                            var container = wheel.GetRoll(m).GetVisibleContainer(n);
                            if (Constant11029.StarElementId == container.sequenceElement.config.id)
                            {
                                container.PlayElementAnimation("Blink");
                            }
                        }
                    }
                    AudioUtil.Instance.PlayAudioFx("J01_Trigger");
                    await machineContext.WaitSeconds(0.67f);
                    
                    if (freeSpinState.IsInFreeSpin)
                    {
                        //轮盘扫光 
                        AudioUtil.Instance.PlayAudioFx("BonusGame_Character_Video1");
                        machineContext.view.Get<BgLightView11029>().PlayBgLight();
                        machineContext.view.Get<MeiDuSha11029>().PlayInClose();
                        await machineContext.WaitSeconds(1.0f);
                        machineContext.view.Get<BgLightView11029>().HideBgLight();
                        machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                        var wheelsSpinningProxy11029 =
                            machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as
                                WheelsSpinningProxy11029;
                        //美杜莎更换长图和轮盘拉升同时进行
                        AudioUtil.Instance.PlayAudioFx("BonusGame_UpRaise");
                         AudioUtil.Instance.PlayAudioFx("BonusGame_Character");
                        wheelsSpinningProxy11029._layerBonusFree.ShowBigScatterElement();
                        // wheelState.UpdateCurrentActiveSequence("RealGemReels");
                        // wheel.ForceUpdateElementOnWheel(true, true);
                        //轮盘上升
                        wheel.transform.Find("spiningMask").gameObject.SetActive(true);
                        Animator animatorWheel = machineContext.view.Get<Wheel>("WheelMagicBonusGame").transform
                            .GetComponent<Animator>();
                        XUtility.PlayAnimation(animatorWheel, "Open");
                        await machineContext.WaitSeconds(0.67f);
                        wheelState.GetWheelConfig().extraTopElementCount = 0;
                        wheelState.GetWheelConfig().rollRowCount = 6;
                        if (wheel.rollCount > 0)
                        {
                            for (var i = 1; i < wheel.rollCount; i++)
                            {
                                wheel.GetRoll(i).ChangeRowCount(6);
                                wheel.GetRoll(i).ChangeExtraTopElementCount(0);
                            }
                            
                            wheel.spinningController.ForceUpdateReels("BelleGemReels");
                        }
                        machineContext.view.Get<HighLightView11029>().PlayHighLight(rollIndex);
                        var ani  = (WheelSpinningController11029<WheelAnimationController11029>)wheel.spinningController;
                        ani.OnSpinResultReceivedNew();
                    }
                    else
                    {
                        AudioUtil.Instance.PlayAudioFx("BonusGame_Character_Video1");
                        machineContext.state.Get<WheelsActiveState11029>().ShowRollsMasks(wheel);
                        var wheelsSpinningProxy11029 =
                            machineContext.GetLogicStepProxy(LogicStepType.STEP_WHEEL_SPINNING) as
                                WheelsSpinningProxy11029;
                        machineContext.view.Get<MeiDuSha11029>().PlayInClose();
                        await machineContext.WaitSeconds(1.0f);
                        machineContext.view.Get<MeiDuSha11029>().ShowMeiDuSha(false);
                        AudioUtil.Instance.PlayAudioFx("BonusGame_Character");
                        wheelsSpinningProxy11029._layerBase.ShowBigScatterElement();
                        await machineContext.WaitSeconds(0.67f);
                        wheel.spinningController.ForceUpdateReels("GemReels");
                        machineContext.view.Get<LightBaseView11029>().PlayBaseLight(rollIndex);
                        var ani =
                            (WheelSpinningController11029<WheelAnimationController11029>) wheel.spinningController;
                        ani.OnSpinResultReceivedNew();
                    }
                }
                else
                {
                    if (freeSpinState.IsInFreeSpin)
                    {
                        if (wheel.wheelState.playerQuickStopped)
                        {
                            machineContext.view.Get<HighLightView11029>().HideHighLight();
                        }
                        else
                        {
                            machineContext.view.Get<HighLightView11029>().PlayHighLight(rollIndex);
                        }
                    }
                    else
                    {
                        if (wheel.wheelState.playerQuickStopped)
                        {
                             machineContext.view.Get<LightBaseView11029>().HideBaseLight();
                        }
                        else
                        {
                            machineContext.view.Get<LightBaseView11029>().PlayBaseLight(rollIndex);
                        }
                    }
                }
            }
            base.OnRollSpinningStopped(rollIndex, rollLogicEnd);
        }
    }
}
