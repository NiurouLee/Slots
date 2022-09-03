using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinStartPopUp11003 : FreeSpinStartPopUp
    {

        [ComponentBinder("IntegralGroupText")]
        private Text txtIntegral;
        
        public FreeSpinStartPopUp11003(Transform transform) : base(transform)
        {
            
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            ulong num = context.state.Get<ExtraState11003>().GetFreeGamePigCoins();

            txtIntegral.SetText(num.GetAbbreviationFormat(1));
        }
    }
}