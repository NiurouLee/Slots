// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2022/01/10/21:43
// Ver : 1.0.0
// Description : UIJackpotPopUp.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class UIJackpotPopUp11003:UIJackpotBase
    {
        public UIJackpotPopUp11003(Transform inTransform) : base(inTransform)
        {
            
        }

        public override void OnOpen()
        {
            base.OnOpen();
            AudioUtil.Instance.PlayAudioFx("JpStart_Open");
        }
    }
}