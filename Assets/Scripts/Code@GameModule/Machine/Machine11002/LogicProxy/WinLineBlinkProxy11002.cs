namespace GameModule
{
    public class WinLineBlinkProxy11002 : WinLineBlinkProxy
    {
        public WinLineBlinkProxy11002(MachineContext context) : base(context)
        {

        }

        protected override void HandleCustomLogic()
        {
            base.HandleCustomLogic();
            // machineContext.view.Get<CharacterView11002>().AnimWin();
        }
    }
}