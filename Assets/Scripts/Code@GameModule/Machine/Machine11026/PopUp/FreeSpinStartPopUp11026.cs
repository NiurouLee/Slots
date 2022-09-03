using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinStartPopUp11026 : FreeSpinStartPopUp
    {

        [ComponentBinder("Root/BGCircleGroup/FreeGames")]
        private Transform freeSpinTip;
        
        [ComponentBinder("Root/BGCircleGroup/WithStlckyWildFeature")]
        private Transform megaFreeSpinTip;
        
        [ComponentBinder("Root/BGCircleGroup/img2")]
        private Transform superFreeSpinTip;
        
        [ComponentBinder("Root/BGCircleGroup/More")]
        private Transform freeSpinTip2;
        
        public FreeSpinStartPopUp11026(Transform transform) : base(transform)
        {
            
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            if (context.state.Get<ExtraState11026>().GetIsMega())
            {
                 freeSpinTip.gameObject.SetActive(false);
                 freeSpinTip2.gameObject.SetActive(false);
                 megaFreeSpinTip.gameObject.SetActive(true);
                 superFreeSpinTip.gameObject.SetActive(false);
            }
            else if(context.state.Get<ExtraState11026>().GetIsSuper())
            { 
                 freeSpinTip.gameObject.SetActive(false);
                 freeSpinTip2.gameObject.SetActive(false);
                 megaFreeSpinTip.gameObject.SetActive(false);
                 superFreeSpinTip.gameObject.SetActive(true);
            }
            else
            {
                 freeSpinTip.gameObject.SetActive(true);
                 freeSpinTip2.gameObject.SetActive(false);
                 megaFreeSpinTip.gameObject.SetActive(false);
                 superFreeSpinTip.gameObject.SetActive(false);
            }
        }
    }
}