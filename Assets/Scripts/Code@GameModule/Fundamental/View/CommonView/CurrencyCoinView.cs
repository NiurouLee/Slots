// **********************************************
// Copyright(c) 2021 by com.ustar
// All right reserved
// 
// Author : Jian.Wang
// Date : 2021/10/27/18:00
// Ver : 1.0.0
// Description : CurrencyCoinView.cs
// ChangeLog :
// **********************************************

using DG.Tweening;
using TMPro;
using UnityEngine;

using UnityEngine.UI;

namespace GameModule
{
    [AssetAddress("CurrencyCoin")]
    public class CurrencyCoinView:View<CurrencyCoinViewController>
    {
        [ComponentBinder("CurrencyGroup/Coin/BG")]
        public Button coinButton;
        
        [ComponentBinder("CurrencyGroup/Coin/CountText")]
        public TextMeshProUGUI coinCountText;
        
        [ComponentBinder("CurrencyGroup/Coin/Icon")]
        public Transform coinIcon;
        
        [ComponentBinder("CurrencyGroup/Coin")]
        public Transform coinGroup;

        public CurrencyCoinView(string assetAddress)
            : base(assetAddress)
        {
        }
        
        public Transform GetCoinIcon()
        {
            return coinIcon;
        }

        protected override void SetUpExtraView()
        {
            base.SetUpExtraView();
            AdaptUI();
        }
        
        protected void AdaptUI()
        {
            if (!ViewManager.Instance.IsPortrait)
            {
                if (ViewResolution.referenceResolutionLandscape.x < ViewResolution.designSize.x)
                {
                    var localScale = ViewResolution.referenceResolutionLandscape.x / ViewResolution.designSize.x;
                    transform.localScale = new Vector3(localScale, localScale, localScale);
                }
            }
        }

        
        private Tween _coinTween;
        public void UpdateCoinCount(long startCoinNum, long delta, bool hasRollAnimation)
        {
            long target = startCoinNum + delta;
            
            if (_coinTween != null)
            {
                _coinTween.Kill();
                _coinTween = null;
            }
            
            DOTween.Kill(coinCountText);

            if (hasRollAnimation)
            {
                coinCountText.text = startCoinNum.GetCommaFormat();

                long v = startCoinNum;

                _coinTween = DOTween.To(() => v, (x) =>
                {
                    v = x;
                    coinCountText.text = v.GetCommaFormat();
                }, target, 2.0f).OnComplete(() => { coinCountText.text = target.GetCommaFormat(); });
            }
            else
            {
                coinCountText.text = target.GetCommaFormat();
            }
        }
    }
    public class CurrencyCoinViewController : ViewController<CurrencyCoinView>
    {
        protected long coinCountNum;

 
        public override void OnViewEnabled()
        {
            coinCountNum = TopPanel.GetCoinCountNum();
            view.UpdateCoinCount(coinCountNum,0,false);
        }

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
            
            SubscribeEvent<EventBalanceUpdate>(OnBalanceUpdate);
            
        }

        protected void OnBalanceUpdate(EventBalanceUpdate eventBalanceUpdate)
        {
            view.UpdateCoinCount(coinCountNum, eventBalanceUpdate.delta, eventBalanceUpdate.hasAnimation);
            coinCountNum += eventBalanceUpdate.delta;
        }

        public void ShowCollectFx()
        {
            var animator = view.coinGroup.transform.GetComponent<Animator>();
            animator.Play("Open");
        }

    }
}