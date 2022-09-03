//  **********************************************
//  Copyright(c) 2021 by com.ustar
//  All right reserved
// 
//  Author : Besure.Chen
//  Date :2021-08-31 19:31
//  Ver : 1.0.0
//  Description : Element11010.cs
//  ChangeLog :
//  **********************************************

using System;
using DG.Tweening;
using UnityEngine;

namespace GameModule
{
    public class Element11011: Element
    {
        public long TotalWin;
        public int WinRate;
        [ComponentBinder("IntegralText")] protected TextMesh integralText;
        
        public Element11011(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }

        public Vector3 GetStartWorldPos()
        {
            if (integralText)
            {
                return integralText.transform.position;   
            }
            return transform.position;
        }

        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            WinRate = 0;
            TotalWin = 0;
            base.UpdateOnAttachToContainer(containerTransform, element);
            containerTransform.transform.localScale = Vector3.one;
            var isLinkElement = Constant11011.IsLinkElement(sequenceElement.config.id);
            var isWrapElement = Constant11011.IsWrapElement(sequenceElement.config.id);
            if ((isLinkElement || isWrapElement)&& sequenceElement.config.id!=Constant11011.ElementOnlyAddSpin)
            {
                containerTransform.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
            }
            if (isLinkElement)
            {
                int winRate = sequenceElement.config.GetExtra<int>("winRate");
                UpdateElementContent(winRate);   
            }
            if (isWrapElement)
            {
                int winRate = IsLinkWheel() ? 0 : (int)sequenceElement.machineContext.state.Get<ExtraState11011>().FreeGameWinRate;
                UpdateElementContent(winRate);   
            }
        }

        protected override void UpdateShowGrayLayer(bool isEnable)
        {
            base.UpdateShowGrayLayer(IsLinkWheel());
        }

        private bool IsLinkWheel()
        {
            return sequenceElement.machineContext.state.Get<WheelsActiveState11011>().IsLinkWheel;
        }

        public virtual void UpdateElementContent(int winRate)
        {
            if (integralText != null)
            {
                WinRate = winRate;
                integralText.SetText("");
                TotalWin = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (TotalWin > 0)
                {
                    integralText.SetText(TotalWin.GetAbbreviationFormat());
                }
            }
        }
    }
}