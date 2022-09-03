// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/09/01/16:30
// Ver : 1.0.0
// Description : CoinElement11001.cs
// ChangeLog :
// **********************************************

using UnityEngine;

namespace GameModule
{
    public class CoinElement11001:Element
    {
        [ComponentBinder("IntegralText")] 
        protected TextMesh integralText;

        public CoinElement11001(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }
 
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            UpdateElementContent();
        }
        
        public  void UpdateElementContent()
        { 
            long winRate = sequenceElement.config.GetExtra<int>("winRate");
            var extraState =  sequenceElement.machineContext.state.Get<ExtraState11001>();
            var bingoData = extraState.GetBingoData(); 
           
            var chips = BetState.GetWinChips(winRate, (long)bingoData.Bet);
            integralText.text = XUtility.GetLimitLengthAbbreviationFormat(chips);
        }
    }
}