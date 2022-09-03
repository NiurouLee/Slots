using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class ExtraState11006 : ExtraState<BuffaloGoldGameResultExtraInfo>
    {

        
        public ExtraState11006(MachineState state) : base(state)
        {
        }
        

        

        public override bool HasSpecialEffectWhenWheelStop()
        {
            if (extraInfo.Multiplier > 0)
            {
                return true;
            }

            // if (extraInfo.BuffaloCount != extraInfo.BuffaloCountOld)
            // {
            //     return true;
            // }

            if (machineState.machineContext.state.Get<FreeSpinState>().IsInFreeSpin)
            {
                return true;
            }

            return false;
        }
        
        
        public bool changeBgm = false;

        public BuffaloGoldGameResultExtraInfo.Types.ReelPosition[] GetSubstitutes()
        {
            return extraInfo.Substitutes.array;
        }

        public int GetMultiplier()
        {
            return extraInfo.Multiplier;
        }

        public int GetAllCollect()
        {
            return extraInfo.BuffaloCount;
        }

        public int GetCollectToUpgrade()
        {
            return extraInfo.BuffaloLeftToUpgrade;
        }

        public int GetBuffaloLevel()
        {
            return extraInfo.BuffaloLevel;
        }
        
        
        
        public int GetOldAllCollect()
        {
            return extraInfo.BuffaloCountOld;
        }

        public int GetOldCollectToUpgrade()
        {
            return extraInfo.BuffaloLeftToUpgradeOld;
        }

        public int GetOldBuffaloLevel()
        {
            return extraInfo.BuffaloLevelOld;
        }

    }
}