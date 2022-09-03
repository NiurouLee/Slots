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
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class ReSpinFinishPopUp:MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/IntegralText")]
        protected Text txtReSpinWinChips;
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button collectButton;
        protected bool isClosing = false;
        public ReSpinFinishPopUp(Transform transform) : base(transform)
        {
            if (collectButton)
            {
                collectButton.onClick.AddListener(async () =>
                {
                    if (!isClosing)
                    {
                        isClosing = true;
                        AudioUtil.Instance.PlayAudioFx("Close");
                        collectButton.enabled = false;
                        await context.state.Get<ReSpinState>().SettleReSpin();
                        Close();
                    }
                });   
            }
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("LinkGameEnd_Open");
            base.OnOpen();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if (txtReSpinWinChips)
                UpdateTotalWin(context.state.Get<ReSpinState>().GetRespinTotalWin());
        }

        public void UpdateTotalWin(ulong totalWin)
        {
            if(txtReSpinWinChips)
                txtReSpinWinChips.SetText(totalWin.GetCommaFormat());    
        }

        public bool IsAutoClose()
        {
            return collectButton == null;
        }
    }
}