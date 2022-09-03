//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-30 15:04
//  Ver : 1.0.0
//  Description : BonusFinishPopUp.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    public class BonusFinishPopUp: MachinePopUp
    {
        [ComponentBinder("Root/MainGroup/IntegralText")]
        protected Text txtReSpinWinChips;
        [ComponentBinder("Root/BottomGroup/CollectButton")]
        protected Button collectButton;
        public BonusFinishPopUp(Transform transform) : base(transform)
        {
            
            if (collectButton)
            {
                collectButton.onClick.AddListener(async () =>
                {
                    AudioUtil.Instance.PlayAudioFx("Close");
                    collectButton.enabled = false;
                    await context.state.Get<ExtraState>().SettleBonusProgress();   
                    Close();
                });   
            }
        }

        public void UpdateTotalWin(ulong totalWin)
        {
            if(txtReSpinWinChips)
                txtReSpinWinChips.SetText(totalWin.GetCommaFormat());    
        }
    }
}