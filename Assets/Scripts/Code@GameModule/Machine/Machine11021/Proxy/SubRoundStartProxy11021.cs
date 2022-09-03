namespace GameModule
{
    public class SubRoundStartProxy11021: SubRoundStartProxy
    {
        public SubRoundStartProxy11021(MachineContext context) : base(context)
        {
        }


        protected override void HandleCustomLogic()
        {
            machineContext.view.Get<TitlePrizeView>().VisibleItemsAll();
            base.HandleCustomLogic();
        }
    }
}