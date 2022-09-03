namespace GameModule
{
    public class NextSpinPrepareProxy11005: NextSpinPrepareProxy
    {
        
        public NextSpinPrepareProxy11005(MachineContext context) : base(context)
        {
        }

        protected override void OnBetChange()
        {
            base.OnBetChange();
            machineContext.view.Get<BaseLetterView11005>().ChangeBetLetter(false);
            
            
        }
        
        
        protected override  void HandleCommonLogic()
        {
            base.HandleCommonLogic();

            if (!machineContext.state.Get<AutoSpinState>().IsAutoSpin)
            {
                
                
                machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(true);
            }
        }

        public override void StartNextSpin()
        {
            machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(false);
            base.StartNextSpin();
        }

        
    }
}