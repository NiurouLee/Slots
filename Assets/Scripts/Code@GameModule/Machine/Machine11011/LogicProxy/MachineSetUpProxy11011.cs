//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-19 15:51
//  Ver : 1.0.0
//  Description : MachineSetUpProxy11011.cs
//  ChangeLog :
//  **********************************************

namespace GameModule
{
    public class MachineSetUpProxy11011: MachineSetUpProxy
    {
        public MachineSetUpProxy11011(MachineContext context)
            : base(context)
        {
            
        }
        
        protected override void HandleCustomLogic()
        {
            var bonusGameState = machineContext.state.Get<ExtraState11011>();
            machineContext.view.Get<CollectCoinView11011>().UpdateState(bonusGameState.Exaggerated);
            if (bonusGameState.HasSpecialBonus())
            {
                machineContext.JumpToLogicStep(LogicStepType.STEP_SPECIAL_BONUS, LogicStepType.STEP_MACHINE_SETUP);       
            } else if (bonusGameState.HasBonusGame())
            {
                machineContext.JumpToLogicStep(LogicStepType.STEP_BONUS, LogicStepType.STEP_MACHINE_SETUP);
            }
            else
            {
                base.HandleCustomLogic();
            }
        }

    }
}