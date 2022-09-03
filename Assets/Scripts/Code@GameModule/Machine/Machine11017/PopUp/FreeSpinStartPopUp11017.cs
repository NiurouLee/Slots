using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinStartPopUp11017 : FreeSpinStartPopUp
    {

        [ComponentBinder("Image1")]
        private Transform freeSpinTip;
        
        [ComponentBinder("MainGroupSuperGame")]
        private Transform superFreeSpinTip;
        
        public FreeSpinStartPopUp11017(Transform transform) : base(transform)
        {
            
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            var _extraState = context.state.Get<ExtraState11017>();
            uint level = _extraState.GetLevel();
            if (level == 5)
            {
                 freeSpinTip.gameObject.SetActive(false);
                 superFreeSpinTip.gameObject.SetActive(true);
            }
            else
            {
                 freeSpinTip.gameObject.SetActive(true);
                 superFreeSpinTip.gameObject.SetActive(false);
            }
        }
    }
}