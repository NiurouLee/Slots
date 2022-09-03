// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/13/15:51
// Ver : 1.0.0
// Description : GitBoxFaqPopup.cs
// ChangeLog :
// **********************************************
using UnityEngine;
using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("UIStoreGiftBoxNoticeH")]
    public class GitBoxFaqPopup : Popup
    {
        [ComponentBinder("ConfirmButton")] 
        public Button confirmButton;
      
        public GitBoxFaqPopup(string address)
            : base(address)
        {
            
        }

        public override Vector3 CalculateScaleInfo()
        {
            //return base.CalculateScaleInfo();
            if (ViewManager.Instance.IsPortrait)
            {
                return new Vector3(0.7f, 0.7f, 0.7f);
            }
            
            return Vector3.one;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            
            confirmButton.onClick.AddListener(OnCloseClicked);
        }
    }
}