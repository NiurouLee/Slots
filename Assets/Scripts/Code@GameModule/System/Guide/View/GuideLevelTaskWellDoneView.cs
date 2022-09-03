//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-11-20 14:46
//  Ver : 1.0.0
//  Description : LevelGuideTaskWellDoneView.cs
//  ChangeLog :
//  **********************************************

using System;
using DragonU3DSDK.Network.API.ILProtocol;
using TMPro;
using UnityEngine;

namespace GameModule
{
    public class GuideLevelTaskWellDoneView:View<GuideLevelTaskWellDoneViewController>
    {
        [ComponentBinder("DialogGroupL")]
        public Animator AnimatorTipsL;
        [ComponentBinder("DialogGroupL/ContentGroup/IntergralText")]
        public TextMeshProUGUI TxtChipsL;
        
        [ComponentBinder("DialogGroupR")]
        public Animator AnimatorTipsR;
        [ComponentBinder("DialogGroupR/ContentGroup/IntergralText")]
        public TextMeshProUGUI TxtChipsR;
    }
    public class GuideLevelTaskWellDoneViewController:ViewController<GuideLevelTaskWellDoneView>
    {
        
        public void InitReward(Guide guide)
        {
            if (guide != null)
            {
                var txtChips = XItemUtility.GetCoinItem(guide.Reward).Coin.Amount.GetCommaFormat();
                view.TxtChipsL.text = txtChips;
                view.TxtChipsR.text = txtChips;
            }
        }
        
        public async void UpdateDirection(bool isLeft)
        {
            view.SetTransformActive(view.AnimatorTipsL.transform,isLeft);
            view.SetTransformActive(view.AnimatorTipsR.transform,!isLeft);
            XUtility.PlayAnimation(view.AnimatorTipsL,"Open",null,this);
            XUtility.PlayAnimation(view.AnimatorTipsR,"Open",null,this);
            await XUtility.WaitSeconds(2, this);
            XUtility.PlayAnimation(view.AnimatorTipsL,"Close",null,this);
            XUtility.PlayAnimation(view.AnimatorTipsR,"Close",null,this);
        }
    }
}