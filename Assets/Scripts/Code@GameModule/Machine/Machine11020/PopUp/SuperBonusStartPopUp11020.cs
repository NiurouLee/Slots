
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class SuperBonusStartPopUp11020 : MachinePopUp
    {
        [ComponentBinder("CountText")]
        protected Text freeSpinCountText;

        [ComponentBinder("Root/BottomGroup/StartButton")]
        protected Button startButton;

        protected Action startAction;
 
        public SuperBonusStartPopUp11020(Transform transform)
            : base(transform)
        {
            startButton.onClick.AddListener(() =>
            {
                AudioUtil.Instance.PlayAudioFx("Close");
                startAction?.Invoke();
                Close();
            });
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