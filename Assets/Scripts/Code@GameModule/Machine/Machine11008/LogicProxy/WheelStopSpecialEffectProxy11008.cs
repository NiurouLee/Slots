using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
namespace GameModule{
    public class WheelStopSpecialEffectProxy11008 : WheelStopSpecialEffectProxy
    {
        public ExtraState11008 extraState;
        public WheelStopSpecialEffectProxy11008(MachineContext machineContext) : base(machineContext)
        {
            extraState = machineContext.state.Get<ExtraState11008>();
        }

        protected override void HandleCustomLogic()
        {
            WolfLogicHandle();
        }

        public async void WolfLogicHandle(){
            var wolf = machineContext.transform.Find("Wheels/wolfPos/Active_W013(Clone)");
            GameObject.Destroy(wolf.gameObject);
            var curWhell = machineContext.state.Get<WheelsActiveState>().GetRunningWheel()[0];
            var targetSymbolPos = curWhell.GetRoll(0).GetVisibleContainer(2);
            // var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
            // var sequenceElement = new SequenceElement(elementConfigSet.GetElementConfig(Constant11008.WildSymbolId),
            //     machineContext);
            // targetSymbolPos.UpdateElement();
            targetSymbolPos.ShiftSortOrder(true);
            targetSymbolPos.PlayElementAnimation("Attack");
            machineContext.WaitSeconds(0.5f,()=>{
                AudioUtil.Instance.PlayAudioFx("Wolf_Blow");
            });
            
            await XUtility.WaitSeconds(0.8f);
            for (int i = 0; i < extraState.NewWilds.count; i++)
            {
                var wildPos = extraState.NewWilds[i];
                var symbol = Constant11008.GetTargetPos(machineContext,wildPos);
                
                var randomNum = (float) UnityEngine.Random.Range(0,3)*0.2f;
                machineContext.WaitSeconds(randomNum,()=>{
                    var curSymbolPos = symbol.transform.localPosition;
                    symbol.ShiftSortOrder(true);
                    symbol.PlayElementAnimation("Fly");
                    // // 旋转
                    // symbol.transform.DOLocalRotate(new Vector3(0, 0, -180), 1f);
                    // // 方向
                    // var rotationValueRandom = Mathf.Floor(UnityEngine.Random.Range(0, 5));
                    // var randomUp = Mathf.Floor(UnityEngine.Random.Range(0, 2));
                    // if (randomUp == 0)
                    //     rotationValueRandom = -rotationValueRandom;
                    // var targetPos = new Vector3(symbol.transform.localPosition.x + 5, rotationValueRandom, 0);
                    // symbol.transform.DOLocalMove(targetPos, 1f).OnComplete(() =>
                    // {
                    machineContext.WaitSeconds(1.167f, () =>
                    {
                        var elementConfigSet = machineContext.state.machineConfig.GetElementConfigSet();
                        var sequenceElement = new SequenceElement(elementConfigSet.GetElementConfig(Constant11008.WildSymbolId),
                            machineContext);
                        symbol.UpdateElement(sequenceElement);
                        symbol.transform.localPosition = curSymbolPos;
                        symbol.transform.DOLocalRotate(new Vector3(0, 0, 0), 0);
                        symbol.PlayElementAnimation("Appear");
                        AudioUtil.Instance.PlayAudioFx("Wild_Appear");
                    });
                    // });
                });
            }
            await XUtility.WaitSeconds(2.5f);
            Proceed();
        }
    }
}

