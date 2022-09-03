//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2022-01-14 19:39
//  Ver : 1.0.0
//  Description : WheelsSpinningProxy11028.cs
//  ChangeLog :
//  **********************************************

using System.Threading.Tasks;

namespace GameModule
{
    public class WheelsSpinningProxy11028:WheelsSpinningProxy
    {
        public WheelsSpinningProxy11028(MachineContext context)
            : base(context)
        {

        }
        public override async void OnSpinResultReceived()
        {
            await ShowCharacterAction();
            base.OnSpinResultReceived();
        }

        private async Task ShowCharacterAction()
        {
            if (machineContext.state.Get<ExtraState11028>().HasBonusGame())
            {
                await machineContext.WaitSeconds(1f);
                await machineContext.view.Get<TransitionView11028>().PlayCharacterAnimation();
            }
            await Task.CompletedTask;
        }
    }
}