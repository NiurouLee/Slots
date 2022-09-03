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
    public class ReSpinFinishPopUp11022:ReSpinFinishPopUp
    {
        public ReSpinFinishPopUp11022(Transform transform) : base(transform)
        {
            txtReSpinWinChips = transform.Find("Root/MainGroup/IntegralBG/IntegralText").GetComponent<Text>();
        }
        public override void OnOpen()
        {
            AudioUtil.Instance.PlayAudioFx("LockGameEnd_Open");
            base.OnOpen();
        }
    }
}