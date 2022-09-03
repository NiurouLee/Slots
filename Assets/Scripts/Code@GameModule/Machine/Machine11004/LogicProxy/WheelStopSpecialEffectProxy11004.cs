using System.Threading.Tasks;
using UnityEngine;

namespace GameModule
{
    public class WheelStopSpecialEffectProxy11004: WheelStopSpecialEffectProxy
    {
        public WheelStopSpecialEffectProxy11004(MachineContext machineContext) : base(machineContext)
        {
        }


        protected async override void HandleCustomLogic()
        {
            await machineContext.view.Get<LockElementView11004>().RefreshStopLock();
            //machineContext.view.Get<LinkTitleView11004>().RefreshUI();
            
            var respinState = machineContext.state.Get<ReSpinState11004>();
            if (!respinState.IsInRespin)
            {
                await Constant11004.ShowJackpot(machineContext,this);

            }

            await machineContext.view.Get<LinkTitleView11004>().RefreshReSpinCount(false,false);
            base.HandleCustomLogic();
        }
        
        
        
    }
}