// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/07/23/20:40
// Ver : 1.0.0
// Description : BaseSpinMapTitleView.cs
// ChangeLog :
// **********************************************

using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameModule
{
    public class FreeSpinTitleView : TransformHolder
    {
        [ComponentBinder("IntegralText")] 
        private TextMesh _pigWinText; 
         
        public FreeSpinTitleView(Transform transform) : base(transform)
        {
            ComponentBinder.BindingComponent(this,transform);
        }
        
        public void UpdateContent()
        {
            _pigWinText.SetText(context.state.Get<ExtraState11003>().GetFreeGamePigCoins().GetAbbreviationFormat(1));
        }
    }
}