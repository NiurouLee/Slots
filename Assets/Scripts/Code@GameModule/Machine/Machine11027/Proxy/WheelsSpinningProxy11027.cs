using UnityEngine;
using System.Threading.Tasks;

namespace GameModule
{
    public class WheelsSpinningProxy11027 : WheelsSpinningProxy
    {

        private ExtraState11027 extraState;

        public WheelsSpinningProxy11027(MachineContext context)
            : base(context)
        {
            extraState = machineContext.state.Get<ExtraState11027>();
        }

        public override async void OnSpinResultReceived()
        {
            await HandleAnticipation();
            base.OnSpinResultReceived();
        } 
        
        //期待必中动画
        private async Task HandleAnticipation()
        {
            bool isWheel = extraState.GetIsRolling();
            int wildNum = machineContext.state.Get<WheelState11027>().CalculateWildNums();
            if (isWheel)
            {
                int ranNum = Random.Range(1, 100);
                if (ranNum <= 90)
                {
                    AudioUtil.Instance.PlayAudioFx("Expect1");
                    await machineContext.view.Get<TransitionRevealView11027>().PlayCharacterAnimation();
                }
            }
            else if (wildNum >= 2)
            {
                AudioUtil.Instance.PlayAudioFx("Expect2");
                await machineContext.view.Get<TransitionRevealView11027>().PlayCharacterAnimation();
            }
        }
    }
}