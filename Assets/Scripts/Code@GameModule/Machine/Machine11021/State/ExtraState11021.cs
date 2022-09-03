using System.Linq;
using DragonU3DSDK.Network.API.ILProtocol;
using UnityEngine;

namespace GameModule
{
    public class ExtraState11021: ExtraState<RhinoGameResultExtraInfo>
    {
        public ExtraState11021(MachineState state) : base(state)
        {
            
        }


      


        public override bool HasSpecialBonus()
        {
            var wheelstate = machineState.machineContext.state.Get<WheelState>();
            if (wheelstate.GetJackpotWinLines().Count > 0)
            {
                return true;
            }
            if (wheelstate.GetBonusWinLine().Count > 0)
            {
                return true;
            }
            
            var freeSpinState = machineState.machineContext.state.Get<FreeSpinState>();
            if (freeSpinState.NewCount > 0 || freeSpinState.IsTriggerFreeSpin)
            {
                return true;
            }


            return false;
        }

        

        public override bool IsSpecialBonusFinish()
        {
            return base.IsSpecialBonusFinish();
        }

        public RhinoGameResultExtraInfo.Types.DiskData GetDiskData()
        {
            var betState = machineState.machineContext.state.Get<BetState>();
            
            RhinoGameResultExtraInfo.Types.DiskData diskData = null;

            if (!extraInfo.DiskDataMap.TryGetValue(betState.totalBet, out diskData))
            {
                // var keys = extraInfo.DiskDataMap.Keys;
                // var lastKey = keys.Last();
                // diskData = extraInfo.DiskDataMap[lastKey];

                foreach (var item in extraInfo.DiskDataMap)
                {
                    diskData = item.Value;
                }
                Debug.LogError("========因为服务器不行无法处理新数据，所以客户端用最后一个");
            }
            
            return diskData;
        }
        
    }
}