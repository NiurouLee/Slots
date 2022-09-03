
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinStartPopUp11020 : FreeSpinStartPopUp
    {
        [ComponentBinder("Root/MainGroup/CountText")]
        protected Text freeSpinCountText;

        [ComponentBinder("StartButton")]
        protected Button startButton;

        protected Action startAction;
 
        public FreeSpinStartPopUp11020(Transform transform)
            : base(transform)
        {
            
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if (freeSpinCountText)
                freeSpinCountText.text = context.state.Get<FreeSpinState>().LeftCount.ToString();
        }

        public void BindStartAction(Action inStartAction)
        {
            startAction = inStartAction;
        }
    }
}