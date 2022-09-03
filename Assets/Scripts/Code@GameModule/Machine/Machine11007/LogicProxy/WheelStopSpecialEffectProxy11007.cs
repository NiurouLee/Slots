//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-19 15:14
//  Ver : 1.0.0
//  Description : WheelStopSpecialEffectProxy11007.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11007: WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11007(MachineContext machineContext) : base(machineContext)
        {
            
        }
        
        //准备撒Wild
        protected override async void HandleCustomLogic()
        {
            var bonusState = machineContext.state.Get<BonusWheelState11007>();
            var listRandomWilds = machineContext.state.Get<ExtraState11007>().GetWorldPosition();
            if (bonusState.IsBonusRandomFreeWild() && listRandomWilds.Count>0)
            {
                for (int i = 0; i < listRandomWilds.Count; i++)
                {
                    var item = listRandomWilds[i];
                    var wheel = machineContext.state.Get<WheelsActiveState11007>().GetRunningWheel()[0];
                    var roll = wheel.GetRoll((int)item.X);
                    var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                    var config = elementConfigSet.GetElementConfig(Constant11007.ELEMENT_WILD);
                    var seqElement = new SequenceElement(config, machineContext);
                    roll.GetVisibleContainer((int)item.Y).UpdateElement(seqElement,true);
                    roll.GetVisibleContainer((int)item.Y).PlayElementAnimation("Open");
                    AudioUtil.Instance.PlayAudioFx("Respin_AddWild");
                    await machineContext.WaitSeconds(0.5f);
                    var resultSequenceElement = bonusState.GetSpinResultSequenceElement(roll);
                    resultSequenceElement[(int)item.Y+1] = seqElement;
                }
                await machineContext.WaitSeconds(1);
            }

            if (bonusState.IsBonusTwoWild())
            {
                var transform = machineContext.transform.Find("Wheels/WheelRespinBonus");
                var elementTrans0 = transform.Find("TwoWild/Static_W05_0");
                var elementTrans2 = transform.Find("TwoWild/Static_W05_2");
                elementTrans0.gameObject.SetActive(false);
                elementTrans2.gameObject.SetActive(false);
            }

            if (bonusState.IsBonusSpinSameWin())
            {
                bonusState.SetRollLockState(1, !bonusState.HasNormalWinLine());
            }
            Proceed();
        }

        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return base.CheckCurrentStepHasLogicToHandle() ||
                   machineContext.state.Get<BonusWheelState11007>().IsBonusTwoWild() ||
                   machineContext.state.Get<BonusWheelState11007>().IsBonusSpinSameWin();
        }
    }
}