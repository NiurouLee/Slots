//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-16 17:05
//  Ver : 1.0.0
//  Description : ReSpinFinishPopUp11007.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    public class ReSpinFinishPopUp11007: ReSpinFinishPopUp
    {
        public ReSpinFinishPopUp11007(Transform transform) : base(transform)
        {
            
        }
        
        public override void Initialize(MachineContext inContext)
        {
            base.Initialize(inContext);

            if(txtReSpinWinChips)
                txtReSpinWinChips.SetText(context.state.Get<ExtraState11007>().TotalWin.GetCommaFormat());
        }
    }
}