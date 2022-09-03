using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelsSpinningProxy11006: WheelsSpinningProxy
    {
        private WheelState11006 wheelState;
        private FreeSpinState freeSpinState;
        private Animator animatroZhenPing;
        private GameObject objTransitionNiu;
        private WinState _winState;
        public WheelsSpinningProxy11006(MachineContext context) : base(context)
        {
            _winState = machineContext.state.Get<WinState>();
            wheelState = context.state.Get<WheelState11006>();
            freeSpinState = context.state.Get<FreeSpinState>();
            animatroZhenPing = context.transform.Find("ZhenpingAnim").GetComponent<Animator>();
            objTransitionNiu = context.transform.Find("TransitionNiu").gameObject;
        }


        public override async void OnSpinResultReceived()
        {
            wheelState.SetSkipAnticipation(false);
            if (wheelState.HasAnticipationAnimationInRollIndex() && 
                (freeSpinState.IsTriggerFreeSpin || freeSpinState.NewCount>0))
            {
                int ranNum = Random.Range(1, 100);
                if (ranNum < 80)
                {
                    wheelState.SetSkipAnticipation(true);
                    await ShowNiu();
                    base.OnSpinResultReceived();
                }
                else
                {
                    base.OnSpinResultReceived();
                }
            }
            else
            {


                if (_winState.winLevel >= (int)WinLevel.BigWin)
                {
                    await ShowNiu();
                    base.OnSpinResultReceived();
                }
                else
                {
                    base.OnSpinResultReceived();
                }

            }

        }

        protected async virtual Task ShowNiu()
        {
            AudioUtil.Instance.PlayAudioFx("WildBuffalo_Video");
            animatroZhenPing.Play("Open");
            objTransitionNiu.SetActive(true);
            await machineContext.WaitSeconds(5);
            animatroZhenPing.Play("Normal");
            objTransitionNiu.SetActive(false);
        }
    }
}