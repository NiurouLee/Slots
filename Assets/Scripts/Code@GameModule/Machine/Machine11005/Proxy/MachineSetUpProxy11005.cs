namespace GameModule
{
    public class MachineSetUpProxy11005: MachineSetUpProxy
    {
        private FreeSpinState freeSpinState;
        public MachineSetUpProxy11005(MachineContext context) : base(context)
        {
            freeSpinState = machineContext.state.Get<FreeSpinState>();
        }
        
        protected override void HandleCustomLogic()
        {
            
            
            var bonusGameState = machineContext.state.Get<ExtraState11005>();
            if (bonusGameState.HasSpecialBonus())
            {
                machineContext.JumpToLogicStep(LogicStepType.STEP_SPECIAL_BONUS, LogicStepType.STEP_MACHINE_SETUP);
            }
            else
            {
                

                base.HandleCustomLogic();
            }
            
            var baseLetterView = machineContext.view.Get<BaseLetterView11005>();
            baseLetterView.ChangeBetLetter(true);
            baseLetterView.RefreshUINoAnim();

 

        }
    }
}