using System.Collections.Generic;
using UnityEngine;
using System;
using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;
using GameModule;

namespace GameModule
{
    public class ExtraState11013 : ExtraState<CrazyTrainGameResultExtraInfo>
    {
        public ExtraState11013(MachineState state)
            : base(state)
        {
        }


        public override bool HasSpecialEffectWhenWheelStop()
        {
            return true;
        }


        public override bool HasSpecialBonus()
        {
            if (!extraInfo.Chosen || !extraInfo.ChosenAgain)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public RepeatedField<uint> GetFreeSpinCount()
        {
            return extraInfo.FreeSpinCounts;
        }

        public RepeatedField<uint> GetGoldenPosList()
        {
            return extraInfo.GoldenPositionIds;
        }


        public uint GetLastMapStep()
        {
            return lastExtraInfo.MapStep;
        }

        public uint GetNowMapStep()
        {
            return extraInfo.MapStep;
        }

        public uint GetMaxMapStep()
        {
            return extraInfo.MapStepTotal;
        }

        public CrazyTrainGameResultExtraInfo GetExtraInfo()
        {
            return extraInfo;
        }

        public CrazyTrainGameResultExtraInfo GetLastExtraInfo()
        {
            return lastExtraInfo;
        }
    }
}