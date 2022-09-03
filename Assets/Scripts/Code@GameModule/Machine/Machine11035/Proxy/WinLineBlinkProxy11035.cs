namespace GameModule
{
    public class WinLineBlinkProxy11035 : WinLineBlinkProxy
    {
        private UIComboView _uiComboView;

        private UIComboNoticeView _uiComboNoticeView;

        private UIMachineLineGroupView11035 _lineGroupView;


        public WinLineBlinkProxy11035(MachineContext context) : base(context)
        {

        }

        public override void SetUp()
        {
            base.SetUp();

            _uiComboView = machineContext.view.Get<UIComboView>();

            _uiComboNoticeView = machineContext.view.Get<UIComboNoticeView>();

            _lineGroupView = machineContext.view.Get<UIMachineLineGroupView11035>();
        }

        protected override void HandleCommonLogic()
        {
            var wheels = wheelsRunningStatusState.GetRunningWheel();

            if (wheels != null && wheels.Count > 0)
            {
                if (wheels[0].wheelName == "WheelJackpotGame")
                {
                    var elementContainer = wheels[0].GetWinLineElementContainer(0, 0);

                    var element = elementContainer.GetElement();
                    if (element == null || !element.HasAnimState("Win"))
                    {
                        return;
                    }

                    var id = element.sequenceElement.config.id;
                    if (id >= 3 && id <= 7)
                    {
                        AudioUtil.Instance.PlayAudioFxIfNotPlaying("Jackpot_Win", true);
                    }
                    elementContainer.PlayElementAnimation("Win");
                }
                else
                {
                    machineContext.AddWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);

                    for (var i = 0; i < wheels.Count; i++)
                    {
                        wheels[i].winLineAnimationController.BlinkAllWinLine(() =>
                        {
                            machineContext.RemoveWaitEvent(WaitEvent.WAIT_BLINK_ALL_WIN_LINE);
                        });
                    }

                    CheckAndShowFiveOfKindAnimation(wheels);
                }
            }
        }

        protected override void HandleCustomLogic()
        {
            // _uiComboView.Set();

            // _uiComboNoticeView.Set();

            _lineGroupView.PlayLoop();

            // await XUtility.WaitSeconds(1.0f, machineContext);

            base.HandleCustomLogic();
        }
    }
}