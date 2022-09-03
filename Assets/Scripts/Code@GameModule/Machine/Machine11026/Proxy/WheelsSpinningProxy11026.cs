// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/08/05/18:12
// Ver : 1.0.0
// Description : WheelsSpinningProxy11001.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DG.Tweening;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class WheelsSpinningProxy11026 : WheelsSpinningProxy
    {
         ElementConfigSet elementConfigSet = null;
         private FreeSpinState freeSpinState;
         private ReSpinState reSpinState;
         private LockElementLayer11026 _layer;
         private ExtraState11026 extraState;
         public List<DragonRisingGameResultExtraInfo.Types.Position> lastRandomWildPosList;
        public WheelsSpinningProxy11026(MachineContext context)
            : base(context)
        {
            elementConfigSet = machineContext.state.machineConfig.elementConfigSet;
            lastRandomWildPosList = new List<DragonRisingGameResultExtraInfo.Types.Position>();
            extraState = machineContext.state.Get<ExtraState11026>();
            freeSpinState = machineContext.state.Get<FreeSpinState>();
            reSpinState = machineContext.state.Get<ReSpinState>();
        }

        public override void SetUp()
        {
            base.SetUp();
            var wheel = machineContext.view.Get<IrregularWheel>(1);
            machineContext.view.Add<LockElementLayer11026>(wheel.transform);
            _layer = machineContext.view.Get<LockElementLayer11026>();
            _layer.BindingWheel(wheel);
            _layer.SetSortingGroup("Element",9999);
        }

        public override async void OnSpinResultReceived()
        {
            if (extraState.GetIsMega() && !reSpinState.IsInRespin)
            {  
                //随机投放wild
                await _layer.ShowRandomWildElement();
                await HandleAnticipation();
                base.OnSpinResultReceived();
            }
            else if (extraState.GetIsSuper() && !reSpinState.IsInRespin)
            {
                await HandleAnticipation();
                base.OnSpinResultReceived();
            }
            else
            {
                await HandleAnticipation();
                base.OnSpinResultReceived();
            }
        }

        
        //期待必中动画
        private async Task HandleAnticipation()
        {
            bool isMega = extraState.GetIsMega();
            bool isSuper = extraState.GetIsSuper();
            if (reSpinState.ReSpinTriggered)
            {
                machineContext.view.Get<TransitionsView11026>().PlayAnticipation();
                await machineContext.WaitSeconds(2.67f);
                AudioUtil.Instance.PlayAudioFx("Expect");
                await machineContext.WaitSeconds(5.0f - 2.67f);

            }else if (freeSpinState.IsTriggerFreeSpin)
            {
                bool random = (Random.Range(1, 100) % 2 == 0 )? true : false;
                if (random || isMega || isSuper)
                { 
                    machineContext.view.Get<TransitionsView11026>().PlayAnticipation(); 
                    await machineContext.WaitSeconds(2.67f);
                    AudioUtil.Instance.PlayAudioFx("Expect"); 
                    await machineContext.WaitSeconds(5.0f - 2.67f);
                }
            }
        }
    }
}