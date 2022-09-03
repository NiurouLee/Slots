namespace GameModule
{
    public class SubRoundStartProxy11005: SubRoundStartProxy
    {
        public SubRoundStartProxy11005(MachineContext context) : base(context)
        {
        }

        protected override void HandleCustomLogic()
        {
            //machineContext.view.Get<BaseLetterView11005>().SetBtnGetMoreEnable(false);

            machineContext.view.Get<JackPotPanel11005>().CloseJackpotAnim();
            
            base.HandleCustomLogic();
        }
    }
}