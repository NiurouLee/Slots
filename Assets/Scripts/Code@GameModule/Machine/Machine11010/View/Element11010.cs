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
    public class Element11010: Element
    {
        private int _winRate;
        [ComponentBinder("GrayLayer")] protected SpriteRenderer _spriteGray;
        [ComponentBinder("Root/LinkState")] protected Transform _transLink;
        [ComponentBinder("Root/IntegralText")] protected TextMesh integralText;

        [ComponentBinder("Root/BetterIntegralText")]
        private TextMesh betterIntegralText;
        [ComponentBinder("Root/LinkState/LinkSpriteBg")] public Transform TransLinkBg;
        [ComponentBinder("Root/LinkState/LinkOut")] public Transform _transLinkOut;
        [ComponentBinder("Root/NormalState")] protected Transform _transNormal;
        public Element11010(Transform transform, bool inIsStatic)
            : base(transform, inIsStatic)
        {
            ComponentBinder.BindingComponent(this, transform);
        }
        private SpriteRenderer GetSpriteRender(string jackpotName)
        {
            var transformJackpot = transform.Find(jackpotName);
            if (transformJackpot == null)
            {
                transformJackpot = transform.Find("Sprite/"+jackpotName);
            }

            if (transformJackpot)
            {
                return transformJackpot.GetComponent<SpriteRenderer>();
            }

            return null;
        }
        
        public override void UpdateOnAttachToContainer(Transform containerTransform, SequenceElement element)
        {
            base.UpdateOnAttachToContainer(containerTransform, element);
            ToggleJackpot(true);
            if (Constant11010.IsLinkElement(sequenceElement.config.id))
            {
                int winRate = sequenceElement.config.GetExtra<int>("winRate");
                UpdateElementContent(winRate);   
            }
            if (Constant11010.IsNormalElementId(sequenceElement.config.id))
            {
                UpdateGraySprite();
            }
        }


        public void UpdateWinRate(int winRate, bool needAnim)
        {
            _winRate = winRate;
            if (!needAnim)
            {
                UpdateElementContent(winRate);
            }
        }

        public virtual void UpdateElementContent(int winRate)
        {
            _winRate = winRate;
            if (integralText != null)
            {
                integralText.text = "";
                var chips = sequenceElement.machineContext.state.Get<BetState>().GetPayWinChips(winRate);
                if (chips > 0)
                {
                    integralText.text = chips.GetAbbreviationFormat();
                }
                if (betterIntegralText)
                {
                    float multi = chips * 1f/ sequenceElement.machineContext.state.Get<BetState>().totalBet;
                    var isShowBetter = multi >= 5;
                    betterIntegralText.text = integralText.text;
                    integralText.gameObject.SetActive(!isShowBetter);
                    betterIntegralText.gameObject.SetActive(isShowBetter);   
                }
            }
            
            if (_transLink)
            {
                _transLink.gameObject.SetActive(IsLinkWheel()); 
            }

            if (_transNormal)
            {
                _transNormal.gameObject.SetActive(!IsLinkWheel());
            }

            if (_transLinkOut)
            {
                _transLinkOut.gameObject.SetActive(IsLinkWheel());
            }
        }

        private void UpdateGraySprite()
        {
            if (_spriteGray)
            {
                _spriteGray.gameObject.SetActive(IsLinkWheel());
            }
        }

        private bool IsLinkWheel()
        {
            return sequenceElement.machineContext.state.Get<WheelsActiveState11010>().isLinkWheel;
        }

        public bool NeedChangeAnim(int winRate)
        {
            return _winRate < winRate;
        }

        public async void DoChangeAnim()
        {
            AudioUtil.Instance.PlayAudioFx("Bonus_J01_MoneyAdd");
            integralText.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.1f).SetEase(Ease.InQuad);
            if (betterIntegralText)
            {
                betterIntegralText.transform.DOScale(new Vector3(0.6f, 0.6f, 0.6f), 0.1f).SetEase(Ease.InQuad);  
            }
            await sequenceElement.machineContext.WaitSeconds(0.1f);
            UpdateElementContent(_winRate);
            ChangeLinkState();
            integralText.transform.DOScale(new Vector3(1.2f,1.2f,1.2f), 0.08f);
            integralText.transform.DOScale(new Vector3(1f,1f,1f), 0.05f).SetDelay(0.08f);
            if (betterIntegralText)
            {
                betterIntegralText.transform.DOScale(new Vector3(1.2f,1.2f,1.2f), 0.08f);
                betterIntegralText.transform.DOScale(new Vector3(1f,1f,1f), 0.05f).SetDelay(0.08f);
            }
        }

        public void ToggleJackpot(bool visible)
        {
            if (integralText)
            {
                integralText.gameObject.SetActive(!visible);
            }
        }

        public override void PlayAnimation(string animationName, bool maskByWheelMask, Action endCallback = null)
        {
            if (animationName == "LoopInLink")
            {
                ChangeLinkState();
            }
            base.PlayAnimation(animationName,maskByWheelMask,endCallback);
        }

        public void ChangeLinkState()
        {
            _transLink.gameObject.SetActive(true);
            _transNormal.gameObject.SetActive(false);
            if (_transLinkOut)
            {
                _transLinkOut.gameObject.SetActive(false);   
            }
        }
    }
}