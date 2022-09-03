namespace GameModule
{
    public class WinLineBlinkProxy11006: WinLineBlinkProxy
    {
        
        private ExtraState11006 extraState;
        public WinLineBlinkProxy11006(MachineContext context) : base(context)
        {
            extraState = machineContext.state.Get<ExtraState11006>();
        }


        protected override void HandleCustomLogic()
        {
            RefreshMultiplierView();
            base.HandleCustomLogic();
        }
        
        
        public void RefreshMultiplierView()
        {
            int mult = extraState.GetMultiplier();
            if (mult > 1)
            {
                var view = machineContext.view.Get<MultiplierNoticeView11006>();
                view.RefreshMultiplier(mult);
                view.Open();
            }
        }
        
        
        
    }
}