using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class LoadingFinishNoticePopUp11020 : MachinePopUp
    {
        [ComponentBinder("Root/BottomGroup/StartButton")]
        private Button confirmButton;

        public LoadingFinishNoticePopUp11020(Transform inTransform) 
            : base(inTransform)
        {
            ComponentBinder.BindingComponent(this, inTransform);            
        }

        public override void OnOpen()
        {
            confirmButton.onClick.AddListener(
                    () => { 
                        AudioUtil.Instance.PlayAudioFx("Close");
                        Close();
                    }
                );
        }
    }
}
