using UnityEngine;

namespace GameModule
{
    public class NextSpinPrepareProxy11002 : NextSpinPrepareProxy
    {
        private Animator _animator;

        public NextSpinPrepareProxy11002(MachineContext context) : base(context)
        {
        }

        public override void SetUp()
        {
            base.SetUp();
            _animator = machineContext.transform.Find("ZhenpingAnim").GetComponent<Animator>();
        }

        protected override void HandleCustomLogic()
        {
            machineContext.view.Get<JackPotPanel>().transform.gameObject.SetActive(true);
            _animator.Play("Normal", 0, 0);
            base.HandleCustomLogic();
        }

        protected override async void OnBetChange()
        {
            base.OnBetChange();
            await machineContext.view.Get<ChillView>().RefreshUI(false, false, true, true);
            (machineContext.view.Get<Wheel>().winLineAnimationController as WinLineAnimationController11002).StopAllElementAnimation(true);
            // (machineContext.view.Get<Wheel>(1).winLineAnimationController as WinLineAnimationController11002).StopAllElementAnimation(true);
        }
    }
}