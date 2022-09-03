namespace GameModule
{
    public class SubRoundStartProxy11004: SubRoundStartProxy
    {
        public SubRoundStartProxy11004(MachineContext context) : base(context)
        {
        }

        protected async override void HandleCustomLogic()
        {
            machineContext.view.Get<LinkTitleView11004>().RefreshReSpinCount(false,true);
            machineContext.view.Get<LockElementView11004>().ClearLock();
            base.HandleCustomLogic();
        }
    }
}