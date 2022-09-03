using DragonU3DSDK.Network.API.ILProtocol;
using Google.ilruntime.Protobuf.Collections;

using System.Collections.Generic;
namespace GameModule
{
    public class ExtraState11017 : ExtraState<GorillaEatBananaGameResultExtraInfo>
    {
        public ExtraState11017(MachineState state) : base(state)
        {
        }
         private bool isShowSpecialBouns = false;
         private bool isEatnAniFinish =false;
         public override bool HasSpecialEffectWhenWheelStop()
        {
            if (isShowSpecialBouns)
            {
                return true;
            }
            else
            {
                 return false;
            }
        }
        
        public override bool HasSpecialBonus()
        {
           if (isShowSpecialBouns)
           {
                return true;
           }
           else
           {
               return false;
           }
        }

        public override void UpdateStateOnReceiveSpinResult(SSpin spinResult)
		{
			base.UpdateStateOnReceiveSpinResult(spinResult);
            isShowSpecialBouns = spinResult.GameResult.Panels.Count > 1;
        }

        public GorillaEatBananaGameResultExtraInfo.Types.Banana[] GetBananas()
        {
            return extraInfo.Bananas.array;
        }

        public RepeatedField<uint> GetLastEatenPositionId()
        {
            return extraInfo.LastEatenPositionIds;
        }
        
        public uint GetLevel()
        {
            return extraInfo.Level;
        }

        public void SetEatenAniFinish(bool finish)
        {
            isEatnAniFinish = finish;
        }

        public bool GetEatenAniFinish()
        {
            return isEatnAniFinish;
        }
    }
}