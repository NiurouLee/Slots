using System.Collections.Generic;
using System.Threading.Tasks;

namespace GameModule
{
    public class ReSpinProxy11035 : ReSpinLogicProxy
    {
        private Wheel _wheel;

        private ExtraState11035 _extraState;

        private ReSpinState _respinState;

        private UITransitionView11035 _transitionView;


        public ReSpinProxy11035(MachineContext context) : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();

            _wheel = machineContext.view.Get<Wheel>();

            _extraState = machineContext.state.Get<ExtraState11035>();

            _respinState = machineContext.state.Get<ReSpinState>();

            _transitionView = machineContext.view.Get<UITransitionView11035>();
        }

        protected override async void HandleCustomLogic()
        {
            if (!_extraState.JackpotUncollected())
            {
                if (_respinState.NextIsReSpin || _respinState.IsInRespin || _respinState.ReSpinTriggered)
                {
                    _wheel.wheelState.SetRollLockState(1, _respinState.NextIsReSpin);

                    if (_extraState.HasJackpotWheel() || reSpinState.NextIsReSpin)
                    {
                        UpdateBalance();
                    }
                }
                else
                {
                    _wheel.wheelState.SetRollLockState(1, false);
                }
            }
 
            if (_extraState.JackpotUncollected())
            {
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;
                machineContext.view.Get<JackPotPanel>().UpdateJackpotAndLockJackpotPanel();
                await _extraState.SettleBonusProgress();
                
                if(IsFromMachineSetup())
                {
                    ForceUpdateWinChipsToDisplayTotalWin(0.5f);
                }
                
                machineContext.state.Get<JackpotInfoState>().LockJackpot = false;
                machineContext.view.Get<JackPotPanel>().UnlockJackpotPanel();
                
            }
            else if (_extraState.HasJackpotWheel())
            {
                if (!_respinState.ReSpinTriggered && !reSpinState.IsInRespin)
                {
                    UpdateBalance();
                }
                
                machineContext.state.Get<JackpotInfoState>().LockJackpot = true;

                if (IsFromMachineSetup())
                {
                    machineContext.state.Get<WheelsActiveState>().UpdateRunningWheel(new List<string>() { "WheelBaseGame" });
                }
                
                await HandleJackpotWheelTrigger();
            }

            base.HandleCustomLogic();
        }

        protected async Task HandleJackpotWheelTrigger()
        {
            var uiComboNoticeView = machineContext.view.Get<UIComboNoticeView>();
            var uiComboView = machineContext.view.Get<UIComboView>();

            var uiFireJackpotNoticeView = machineContext.view.Get<UIFireJackpotNoticeView>();
    
            await XUtility.WaitSeconds(3.0f, machineContext);

            uiComboNoticeView.Hide();

            uiFireJackpotNoticeView.Open();

            AudioUtil.Instance.PlayAudioFxIfNotPlaying("Jackpot_Open");

            await XUtility.WaitSeconds(2.0f, machineContext);

            uiFireJackpotNoticeView.Hide();

            machineContext.state.Get<WheelsActiveState>().UpdateRunningWheel(new List<string>() { "WheelJackpotGame" });
            
            uiComboView.Set();
        }
        
        protected void UpdateBalance()
        {
            var winState = machineContext.state.Get<WinState>();
            if(winState.totalWin > 0)
                EventBus.Dispatch(new EventBalanceUpdate((long) winState.totalWin, "SpinWin"));
        }
        public override bool CheckCurrentStepHasLogicToHandle()
        {
            return reSpinState.ReSpinTriggered || reSpinState.IsInRespin || _extraState.HasJackpotWheel() || _extraState.JackpotUncollected();
        }

        protected override Task HandleReSpinStartLogic()
        {
            _transitionView.PlayLoop();

            return base.HandleReSpinStartLogic();
        }
    }
}