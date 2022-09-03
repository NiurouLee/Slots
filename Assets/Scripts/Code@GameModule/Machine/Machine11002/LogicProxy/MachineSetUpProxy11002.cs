// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/08/15:31
// Ver : 1.0.0
// Description : MachineSetUpProxy.cs
// ChangeLog :
// **********************************************

using DragonU3DSDK.Network.API.ILProtocol;

namespace GameModule
{
    public class MachineSetUpProxy11002 : MachineSetUpProxy
    {
        public MachineSetUpProxy11002(MachineContext machineContext) : base(machineContext)
        {
        }

        protected override void HandleCustomLogic()
        {
            var bonusGameState = machineContext.state.Get<ExtraState11002>();
            if (bonusGameState.HasBonusGame())
            {
                machineContext.JumpToLogicStep(LogicStepType.STEP_BONUS, LogicStepType.STEP_MACHINE_SETUP);
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
            }
            else
            {
                base.HandleCustomLogic();
            }
        }

        protected override async void UpdateViewWhenRoomSetUp()
        {
            base.UpdateViewWhenRoomSetUp();
            await machineContext.view.Get<ChillView>().RefreshUI(false, false, true);
            machineContext.view.Get<ChillView>(1)?.RefreshUI(false, false, true);
        }
    }
}