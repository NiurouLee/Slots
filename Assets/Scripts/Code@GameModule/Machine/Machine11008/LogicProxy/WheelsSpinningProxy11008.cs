using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace GameModule{
    public class WheelsSpinningProxy11008 : WheelsSpinningProxy
    {
        public WheelsSpinningProxy11008(MachineContext context) : base(context)
        {
        }

        public override void OnSpinResultReceived()
        {
            NewWildsLogicHandle(()=>{
                base.OnSpinResultReceived();
            });
        }

        public async void NewWildsLogicHandle(Action callcak){
            var extraState = machineContext.state.Get<ExtraState11008>();
            // 如果newWilds不为null时
            if (extraState.NewWilds != null && extraState.NewWilds.count !=0)
            {
                // 创建active大图标，播放动画。
                var wolf = machineContext.assetProvider.InstantiateGameObject("Active_W013");
                var wolfPos =  machineContext.transform.Find("Wheels/wolfPos");
                wolf.transform.SetParent(wolfPos.transform);
                wolf.transform.localPosition = Vector3.zero;
                wolf.transform.localScale = Vector3.one;
                var sort = wolf.AddComponent<SortingGroup>();
                sort.sortingLayerName = "LocalUI";
                wolf.GetComponent<Animator>().Play("Appear");
                machineContext.WaitSeconds(1.067f,()=>{
                    var wheel = machineContext.transform.Find("Wheels");
                    wheel.GetComponent<Animator>().Play("Wheels");
                    AudioUtil.Instance.PlayAudioFx("Wolf_Roar");
                    machineContext.WaitSeconds(1.833f,()=>{
                        AudioUtil.Instance.PlayAudioFx("Wolf_Jump");
                    });
                });
                machineContext.WaitSeconds(1.233f,()=>{
                    machineContext.view.Get<BgView11008>().PlayBgViewAnim("Free");
                    if(machineContext.state.Get<FreeSpinState>().IsOver){
                        machineContext.view.Get<WheelBaseGame11008>().PlayBgViewAnim("Free");
                    }else{
                        machineContext.view.Get<WheelFreeGame11008>().PlayBgViewAnim("Free");
                    }
                        
                });
                await XUtility.WaitSeconds(5.2f);
                callcak?.Invoke();
            }else{
                callcak?.Invoke();
            }
        }
    }
}

