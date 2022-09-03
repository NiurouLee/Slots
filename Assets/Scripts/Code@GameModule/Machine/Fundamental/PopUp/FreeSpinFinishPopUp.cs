// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date :2021-06-21 11:51 AM
// Ver : 1.0.0
// Description : FreeSpinFinishPopUp.cs
// ChangeLog :
// **********************************************

using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class FreeSpinFinishPopUp:MachinePopUp
    {
        [ComponentBinder("IntegralText")]
        protected Text freeSpinWinChipText;
        
        [ComponentBinder("CountText")]
        protected Text freeSpinCountText;
        
        [ComponentBinder("CollectButton")]
        protected Button collectButton;

        protected Action finishAction;


        private bool isSettle = false;
        public FreeSpinFinishPopUp(Transform transform)
        :base(transform)
        {
            collectButton.onClick.AddListener(async () =>
            {
                if (!isSettle)
                {
                    AudioUtil.Instance.PlayAudioFx("Close");
                    isSettle = true;
                    await context.state.Get<FreeSpinState>().SettleFreeSpin();
                    finishAction?.Invoke();
                    Close();
                }
                
            });
        }

        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFxOneShot("FreeGameEnd_Open");
            base.OnOpen();
        }

        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if(freeSpinWinChipText)
                freeSpinWinChipText.SetText(context.state.Get<FreeSpinState>().TotalWin.GetCommaFormat());
            if(freeSpinCountText) 
                freeSpinCountText.SetText(context.state.Get<FreeSpinState>().TotalCount.ToString());
        }
         
        public void BindFinishAction(Action inFinishAction)
        {
            finishAction = inFinishAction;
        }
    }
}
