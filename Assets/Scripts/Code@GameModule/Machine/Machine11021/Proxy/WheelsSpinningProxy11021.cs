using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelsSpinningProxy11021:WheelsSpinningProxy
    {
        public WheelsSpinningProxy11021(MachineContext context) : base(context)
        {
        }
        
        
        
        


        public async override void OnSpinResultReceived()
        {
            
            await MovePrizeViewToNext();
            
            await ShowPreShanks();
            
            base.OnSpinResultReceived();
            RefreshPrizeView();
        }

        protected async void RefreshPrizeView()
        {
            var prizeView = machineContext.view.Get<TitlePrizeView>();
            prizeView.RefreshInfo();
        }
        
        protected async Task MovePrizeViewToNext()
        {
            var prizeView = machineContext.view.Get<TitlePrizeView>();
            await prizeView.MoveToNext();
        }

        protected async Task ShowPreShanks()
        {
            var extraState = machineContext.state.Get<ExtraState11021>();
            var transitionView = machineContext.view.Get<TransitionView11021>();
            var freeSpinState = machineContext.state.Get<FreeSpinState>();

            if (extraState.HasSpecialBonus())
            {
                if (freeSpinState.IsInFreeSpin)
                {
                    await transitionView.ShowPreFreeShanks();
                }
                else
                {
                    await transitionView.ShowPre2Shanks();
                }
            }
            else
            {
                if (!freeSpinState.IsInFreeSpin)
                {
                    //如果没牛，就有几率出现假预告
                    int ranNum = Random.Range(1, 101);
                    if (ranNum <= 10)
                    {
                        await transitionView.ShowPre1Shanks();
                    }
                }
            }
        }

    }
}