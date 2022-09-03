//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-09-14 14:44
//  Ver : 1.0.0
//  Description : UIPiggyBankHelpPopup.cs
//  ChangeLog :
//  **********************************************

using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIPiggyBankHelp")]
    public class PiggyBankHelpPopup: Popup
    {
        public PiggyBankHelpPopup(string address)
            : base(address)
        {
            contentDesignSize = new Vector2(1365, 768);  
        }
        
        [ComponentBinder("Root/ConfirmButton")]
        private void OnBtnConfirmClick()
        {
            Close();
        }
    }
}