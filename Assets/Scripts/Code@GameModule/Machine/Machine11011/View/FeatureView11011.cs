//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-10-20 13:22
//  Ver : 1.0.0
//  Description : LinkView11011.cs
//  ChangeLog :
//  **********************************************

using TMPro;
using UnityEngine;

namespace GameModule
{
    public class FeatureView11011:TransformHolder
    {
        [ComponentBinder("TotalWinGroup")] 
        private Animator _animator;
        [ComponentBinder("TotalWinGroup/Symbol")]
        private Transform transFlyEnd;
        [ComponentBinder("EachWinGroup/IntegarlText")]
        private TextMeshPro txtEachWin;
        [ComponentBinder("NextWinGroup/IntegarlText")]
        private TextMeshPro txtNextWin;
        [ComponentBinder("Level4Group/Text")]
        private TextMeshPro txtGrandWin;
        public FeatureView11011(Transform inTransform):base(inTransform)
        {
            ComponentBinder.BindingComponent(this, transform);
            if (txtNextWin == null)
            {
                txtNextWin = transform.Find("TotalWinGroup/IntegarlText").GetComponent<TextMeshPro>();   
            }
            EnableUpdate();
        }

        public override void Update()
        {
            base.Update();
            if (txtGrandWin)
            {
                var numGrand = context.state.Get<JackpotInfoState>().GetJackpotValue(4);
                txtGrandWin.text = numGrand.GetCommaFormat();   
            }
        }

        public void UpdateEachWin(ulong winRate)
        {
            var totalWins = context.state.Get<BetState>().GetPayWinChips(winRate);
            txtEachWin.text = totalWins.GetCommaFormat();   
        }
        
        public void UpdateNextWin(ulong winRate)
        {
            var totalWins = context.state.Get<BetState>().GetPayWinChips(winRate);
            txtNextWin.text = totalWins.GetCommaFormat();   
        }

        public Vector3 GetFlyEndPos()
        {
            return transFlyEnd.position;
        }

        public TextMeshPro GetTotalTxt()
        {
            return txtNextWin;
        }

        public void PlayCollect()
        {
            XUtility.PlayAnimation(_animator, "Collect");
        }
    }
}