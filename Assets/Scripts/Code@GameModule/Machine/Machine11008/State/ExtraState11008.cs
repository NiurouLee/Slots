using System.Collections;
using System.Collections.Generic;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using UnityEngine;
namespace GameModule{

    public class ExtraState11008 : ExtraState<ThreePigsGameResultExtraInfo>
    {
        public RepeatedField<uint> NewWilds;
        public ExtraState11008(MachineState state) : base(state)
        {
            
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
        {
            base.UpdateStateOnReceiveSpinResult(spinResult);
            NewWilds = extraInfo.NewWilds;
        }

        public override bool HasSpecialEffectWhenWheelStop()
        {
            if(NewWilds!=null && NewWilds.count!=0)
                return true;
            return false;
        }
    }
}