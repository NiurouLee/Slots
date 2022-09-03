namespace GameModule
{
    public class WinLineBlinkProxy11021: WinLineBlinkProxy
    {
        public WinLineBlinkProxy11021(MachineContext context) : base(context)
        {
        }


        protected override void HandleCustomLogic()
        {
            machineContext.view.Get<TitlePrizeView>().NoVisibleItemsAll();
            base.HandleCustomLogic();
        }
    }
}