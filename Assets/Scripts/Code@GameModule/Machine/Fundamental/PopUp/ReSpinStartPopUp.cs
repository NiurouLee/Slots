// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-21 11:50 AM
// Ver : 1.0.0
// Description : ReSpinStartPopUp.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class ReSpinStartPopUp:MachinePopUp
    {
        [ComponentBinder("Root/BottomGroup/StartButton")]
        protected Button startButton;
        public ReSpinStartPopUp(Transform transform) : base(transform)
        {
            if (startButton)
            {
                startButton.onClick.AddListener(() =>
                {
                    AudioUtil.Instance.PlayAudioFx("Close");
                    Close();
                });
            }
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("LinkGameStart_Open");
            base.OnOpen();
        }

        public bool IsAutoClose()
        {
            return startButton == null || !startButton.gameObject.activeInHierarchy;
        }
    }
}