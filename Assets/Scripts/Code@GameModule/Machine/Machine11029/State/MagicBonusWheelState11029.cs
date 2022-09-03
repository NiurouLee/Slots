// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/17/19:57
// Ver : 1.0.0
// Description : WheelState.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class MagicBonusWheelState11029 : WheelState
    {
        public MagicBonusWheelState11029(MachineState state) : base(state)
        {
        }

        public override void UpdateWheelStateInfo(Panel panel)
        {
            if (panel.Columns.Count > 1)
            {
                wheelConfig.extraTopElementCount = panel.Columns[1].Symbols.Count > 3 ? 0 : 3;
            }
            base.UpdateWheelStateInfo(panel);
        }
        
        public override int GetExtraTopElementCount()
        {
            return 3;
        }

        // public void CorrectUpdateStopPostion()
        // {
        //     
        // }
    }
}