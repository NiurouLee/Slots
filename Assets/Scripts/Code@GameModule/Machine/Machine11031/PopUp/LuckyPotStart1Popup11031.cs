// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/05/23/18:19
// Ver : 1.0.0
// Description : MiniGameFinishPopup.cs
// ChangeLog :
// **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class LuckyPotStart1Popup11031 : MachinePopUp
    {
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button startButton;

        public LuckyPotStart1Popup11031(Transform transform)
            : base(transform)
        {
            if (startButton)
            {
                startButton.onClick.AddListener(OnBtnCloseClick);
            }
        }

        public override void OnOpen()
        {
            if (animator != null && animator.HasState("Open"))
                animator.Play("Open");
        }

        private bool isClosing = false;

        private void OnBtnCloseClick()
        {
            if (!isClosing)
            {
                AudioUtil.Instance.PlayAudioFx("Close");
                Close();
                isClosing = true;
            }
        }
    }
}