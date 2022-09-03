// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/26/18:37
// Ver : 1.0.0
// Description : WheelBonusStartPopUp.cs
// ChangeLog :
// **********************************************

using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class WheelBonusStartPopUp:MachinePopUp
    {
        [ComponentBinder("Root/BottomGroup/Button")]
        private Button startButton;

        private Action startAction;
        
        public WheelBonusStartPopUp(Transform transform)
            : base(transform)
        {
            ComponentBinder.BindingComponent(this, transform);
        }
        
        public override void OnOpen()
        {
            base.OnOpen();
            AudioUtil.Instance.PlayAudioFx("MapBonusStart_Open");
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);
            startButton.onClick.AddListener(OnStartClicked);
        }

        public void BindStartAction(Action inFinishAction)
        {
            startAction = inFinishAction;
        }

        public void OnStartClicked()
        {
            Close();
            startAction?.Invoke();
        }
    }
}