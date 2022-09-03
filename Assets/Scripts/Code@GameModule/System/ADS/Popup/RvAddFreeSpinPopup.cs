// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/03/01/19:10
// Ver : 1.0.0
// Description : RvAddFreeSpinPoppup.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    [AssetAddress("UIFreeGameReward")]
    public class RvAddFreeSpinPopup : Popup<ViewController>
    {
        public RvAddFreeSpinPopup(string address)
            : base(address)
        {
            
        }

        public override void OnOpen()
        {
            base.OnOpen();
            
            viewController.WaitForSeconds(2, Close);
        }
    }
}